using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        public UserController(PizzashopDbContext context, JwtService JwtService)
        {
            _context = context;
            _JwtService = JwtService;
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
        // [HttpGet]
        // public IActionResult AddUser()
        // {
        //     var token = Request.Cookies["SuperSecretAuthToken"];
        //     var userName = _JwtService.GetClaimValue(token, "userName");
        //     var imgUrl = _JwtService.GetClaimValue(token, "imgUrl");

        //     ViewData["UserName"] = userName;
        //     ViewData["ImgUrl"] = imgUrl;
            

        //     ViewData["ActiveLink"] = "Users";
        //     return View();
        // }
    }
}