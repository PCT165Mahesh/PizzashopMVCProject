using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzashopMVCProject.Models;
using PizzashopMVCProject.Utilty;
using PizzashopMVCProject.ViewModels;

namespace PizzashopMVCProject.Controllers;


[Authorize]
public class DashboardController : Controller
{
    private readonly PizzashopDbContext _context;
    private readonly JwtService _JwtService;
    private readonly EncryptionService _encrypt;


    public DashboardController(PizzashopDbContext context, JwtService JwtService
                            , EncryptionService encrypt)
    {
        _context = context;
        _JwtService = JwtService;
        _encrypt = encrypt;

    }

    public IActionResult Index()
    {
        var token = Request.Cookies["SuperSecretAuthToken"];


        var userName = _JwtService.GetClaimValue(token, "userName");
        var imgUrl = _JwtService.GetClaimValue(token, "imgUrl");
        

        // var role = _JwtService.GetClaimValue(token, "role");
        ViewData["ActiveLink"] = "Dashboard";

        DashboardViewModel model = new DashboardViewModel();
        ViewData["UserName"] = userName;
        ViewData["ImgUrl"] = imgUrl;
        return View(model);
    }

    [HttpGet]
    public IActionResult ProfileDetails()
    {
        var token = Request.Cookies["SuperSecretAuthToken"];

         //  Validate the Token
    


        var email = _JwtService.GetClaimValue(token, "email");
        var userName = _JwtService.GetClaimValue(token, "userName");
        var imgUrl = _JwtService.GetClaimValue(token, "imgUrl");


        var user = _context.Users.Where(e => e.Email == email).FirstOrDefault();
        var roleObj = _context.Roles.Where(u=>u.RoleId == user.Roleid).FirstOrDefault();

        ProfileDataViewModel model = new ProfileDataViewModel();
        if (user != null)
        {
            model.email = user.Email;
            model.firstName = user.Firstname;
            model.lastName = user.Lastname;
            model.userName = user.Username;
            model.Role = roleObj.Rolename;
            model.phoneNo = user.Phone;
            model.zipcode = user.Zipcode;
            model.address = user.Address;
            model.Imgurl = user.Imgurl;
            model.CountryId = user.Countryid;
            model.CityId = user.Cityid;
            model.StateId = user.Stateid;
            model.Countries = _context.Countries.ToList();
            model.States = _context.States.Where(s => s.Countryid == user.Countryid).ToList();
            model.Cities = _context.Cities.Where(c => c.Stateid == user.Stateid).ToList();
        }

        ViewData["UserName"] = userName;
        ViewData["ImgUrl"] = imgUrl;
        return View(model);
    }


    // Update User Profile Details
    [HttpPost]
    public async Task<IActionResult> ProfileDetails(ProfileDataViewModel model)
    {
        var token = Request.Cookies["SuperSecretAuthToken"];
     

        var email = _JwtService.GetClaimValue(token, "email");

        var user = _context.Users.FirstOrDefault(e => e.Email == email);
        if (user != null)
        {
            // Update user details
            user.Firstname = model.firstName;
            user.Lastname = model.lastName;
            user.Username = model.userName;
            user.Phone = model.phoneNo;
            user.Zipcode = model.zipcode;
            user.Address = model.address;
            user.Countryid = model.CountryId;
            user.Stateid = model.StateId;
            user.Cityid = model.CityId;
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = user.Id;

            // Handle Image Upload
            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName = $"{Guid.NewGuid()}_{model.ProfileImage.FileName}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(fileStream);
                }

                user.Imgurl = $"/uploads/{fileName}"; // Store relative path in DB
            }
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProfileDetails");
        }
        return View(model);
    }



    [HttpGet]
    public IActionResult ChangePassword()
    {
        var token = Request.Cookies["SuperSecretAuthToken"];

        

        var userName = _JwtService.GetClaimValue(token, "userName");
        var imgUrl = _JwtService.GetClaimValue(token, "imgUrl");

        
        ViewData["UserName"] = userName;
        ViewData["ImgUrl"] = imgUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePassViewModel model)
    {
        var token = Request.Cookies["SuperSecretAuthToken"];
         //  Validate the Token
        
        var email = _JwtService.GetClaimValue(token, "email");

        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found!";
            return RedirectToAction("ChangePassword");
        }

        if (user.Password != _encrypt.EncryptPassword(model.CurrentPassword))
        {
            TempData["ErrorMessage"] = "Current password is incorrect.";
            return RedirectToAction("ChangePassword"); // Redirect ensures TempData persists
        }

        if (model.NewPassword != model.ConfirmNewPassword)
        {
            TempData["ErrorMessage"] = "New passwords do not match.";
            return RedirectToAction("ChangePassword");
        }

        // Update password
        user.Password = _encrypt.EncryptPassword(model.NewPassword);
        user.UpdatedAt = DateTime.Now;
        user.UpdatedBy = user.Id;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Password updated successfully!";
        return RedirectToAction("ChangePassword");
    }



    [HttpGet]
    public JsonResult GetCountries()
    {
        var countries = _context.Countries
            .Select(c => new { c.CountryId, c.Name })
            .ToList();
        return Json(countries);
    }


    [HttpGet]
    public JsonResult GetStatesByCountry(long countryId)
    {
        var states = _context.States
            .Where(s => s.Countryid == countryId)
            .Select(s => new { s.StateId, s.Name })
            .ToList();

        return Json(states);
    }


    [HttpGet]
    public JsonResult GetCitiesByState(long id)
    {
        Console.WriteLine($"State ID received: {id}");

        var cities = _context.Cities
            .Where(c => c.Stateid == id)
            .Select(c => new { c.CityId, c.Name })
            .ToList();

        Console.WriteLine($"Cities Found: {cities.Count}");

        return Json(cities);
    }

    [HttpGet]
    public JsonResult GetRoles()
    {

        var roles = _context.Roles.ToList();
        return Json(roles);
    }
}