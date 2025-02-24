using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzashopMVCProject.Models;
using PizzashopMVCProject.Utilty;
using PizzashopMVCProject.ViewModels;

namespace PizzashopMVCProject.Controllers
{

    [Authorize]
    public class UserController : Controller
    {
        private readonly PizzashopDbContext _context;
        private readonly JwtService _JwtService;
        private readonly EncryptionService _encrypt;


        public UserController(PizzashopDbContext context, JwtService JwtService, EncryptionService encrypt)
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

            ViewData["UserName"] = userName;
            ViewData["ImgUrl"] = imgUrl;
            

            ViewData["ActiveLink"] = "Users";
            return View();
        }

        public IActionResult GetUserList(int pageNo = 1, int pageSize = 3, string search = "")
        {
            // var token = Request.Cookies["SuperSecretAuthToken"];

           

            var query = from u in _context.Users
                        join r in _context.Roles on u.Roleid equals r.RoleId
                        where u.Isdeleted == false &&
                            (string.IsNullOrEmpty(search) ||
                            EF.Functions.ILike(u.Firstname, $"%{search}%") ||
                            EF.Functions.ILike(u.Lastname, $"%{search}%") ||
                            EF.Functions.ILike(u.Email, $"%{search}%") ||
                            EF.Functions.ILike(r.Rolename, $"%{search}%")
                            )
                        select new UserListViewModel
                        {
                            UserId = u.Id,
                            FirstName = u.Firstname,
                            LastName = u.Lastname,
                            Email = u.Email,
                            Phone = u.Phone,
                            Status = u.Status,
                            RoleName = r.Rolename,
                            ImgUrl = u.Imgurl
                        };

                var totalRecords = query.Count();
                var users = query.OrderBy(u => u.FirstName)
                            .Skip((pageNo - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            return Json(new { users, totalRecords });
        }




        // Add User Page 
        [HttpGet]
        public IActionResult AddUser()
        {
            var token = Request.Cookies["SuperSecretAuthToken"];
            var userName = _JwtService.GetClaimValue(token, "userName");
            var imgUrl = _JwtService.GetClaimValue(token, "imgUrl");
            var email = _JwtService.GetClaimValue(token, "email");

            var user = _context.Users.Where(u => u.Email == email).Select(u => new {u.Id}).FirstOrDefault();

            ViewData["UserName"] = userName;
            ViewData["ImgUrl"] = imgUrl;
            ViewData["userId"] = user.Id;
            

            // ViewData["ActiveLink"] = "Users";
            return View();
        }

        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            var token = Request.Cookies["SuperSecretAuthToken"];
            var email = _JwtService.GetClaimValue(token, "email");

            if(ModelState.IsValid){
                var user = _context.Users.Where(u => u.Email == email).Select(u => new {u.Id}).FirstOrDefault();

                // if model state are valid
                var newUser = new User{
                    Firstname = model.FirstName,
                    Lastname = model.LastName,
                    Username = model.UserName,
                    Password = _encrypt.EncryptPassword(model.Password),
                    Email = model.Email,
                    Roleid = model.RoleId,
                    Countryid = model.CountryId,
                    Stateid = model.StateId,
                    Cityid = model.CityId,
                    Address = model.Address,
                    Zipcode = model.Zipcode,
                    Phone = model.Phone,
                    CreatedBy = user.Id
                };

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

                    newUser.Imgurl = $"/uploads/{fileName}"; // Store relative path in DB
                }


                // Add user to the Databse
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User Added successfully!";
                return RedirectToAction("AddUser", "User");
            }
            TempData["SuccessMessage"] = "Error Adding User!";
            return View(model);
        }


        // Edit User Action
        [HttpGet]
        public async Task<IActionResult> EditUser(long id){

            var token = Request.Cookies["SuperSecretAuthToken"];

            var userName = _JwtService.GetClaimValue(token, "userName");
            var imgUrl = _JwtService.GetClaimValue(token, "imgUrl");

            var user = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

            EditUserViewModel model = new EditUserViewModel();
            if(user != null){
                model.FirstName = user.Firstname;
                model.LastName = user.Lastname;
                model.UserName = user.Username;
                model.RoleId = user.Roleid;
                model.Email = user.Email;
                model.Status = user.Status;
                model.CountryId = user.Countryid;
                model.StateId = user.Stateid;
                model.CityId = user.Cityid;
                model.Phone = user.Phone;
                model.Address = user.Address;
                model.Zipcode = user.Zipcode;
                model.Country = _context.Countries.Where(u=>u.CountryId == user.Countryid).FirstOrDefault().Name;
                model.State = _context.States.Where(u=>u.StateId == user.Stateid).FirstOrDefault().Name;
                model.City = _context.Cities.Where(u=>u.CityId == user.Cityid).FirstOrDefault().Name;
            }

            ViewData["UserName"] = userName;
            ViewData["ImgUrl"] = imgUrl;
            return View(model);
        }
    }
}