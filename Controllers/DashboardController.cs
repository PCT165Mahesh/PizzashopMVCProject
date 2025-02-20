using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzashopMVCProject.Models;
using PizzashopMVCProject.Utilty;
using PizzashopMVCProject.ViewModels;

namespace PizzashopMVCProject.Controllers;


[Authorize]
public class DashboardController : Controller
{
    private readonly PizzashopDbContext _context;
    private readonly  JwtService _JwtService;
    private readonly EncryptionService _encrypt;


    public DashboardController(PizzashopDbContext context, JwtService JwtService
                            , EncryptionService encrypt)
    {
        _context = context;
        _JwtService = JwtService;
        _encrypt = encrypt;

    }

    public IActionResult Index(){
        var token = Request.Cookies["SuperSecretAuthToken"];
        var email = _JwtService.GetClaimValue(token, "email");
        // var role = _JwtService.GetClaimValue(token, "role");

        var user = _context.Users.Where(e => e.Email == email).FirstOrDefault();
        DashboardViewModel model = new DashboardViewModel();
        ViewData["UserName"] = user.Username;
        ViewData["ImgUrl"] = user.Imgurl;
        return View(model);
    }


     public IActionResult ProfileDetails(){
        var token = Request.Cookies["SuperSecretAuthToken"];
        var email = _JwtService.GetClaimValue(token, "email");
        // var role = _JwtService.GetClaimValue(token, "role");

        
        var user = _context.Users.Where(e => e.Email == email).FirstOrDefault();
        ViewData["UserName"] = user.Username;

        ProfileDataViewModel model = new ProfileDataViewModel();
        if(user != null){
            model.email = user.Email;
            model.firstName = user.Firstname;
            model.lastName = user.Lastname;
            model.userName = user.Username;
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
        return View(model);
    }


    [HttpGet]
    public IActionResult ChangePassword(){
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePassViewModel model){
        var token = Request.Cookies["SuperSecretAuthToken"];
        var email = _JwtService.GetClaimValue(token, "email");

        var user = _context.Users.Where(u=>u.Email == email).FirstOrDefault();
        if(user != null && user.Password == _encrypt.EncryptPassword(model.CurrentPassword)){
            if(model.NewPassword == model.ConfirmNewPassword){
                user.Password = _encrypt.EncryptPassword(model.NewPassword);
                user.UpdatedAt = DateTime.Now;
                user.UpdatedBy = user.Id;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Home");
            }
            else{
                ModelState.AddModelError("NotMatched", "Password are not matching");
                return View(model);
            }
        }
        return View(model);
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
}
