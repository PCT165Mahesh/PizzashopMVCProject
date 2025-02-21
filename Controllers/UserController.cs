using System;
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
        public async Task<IActionResult> Index(int pageNo = 1, int pageSize = 3)
        {

            // var query = from u in _context.Users
            //             join r in _context.Roles on u.Roleid equals r.RoleId
            //             where u.Isdeleted == false
            //             select new UserListViewModel
            //             {
            //                 UserName = u.Username,
            //                 Email = u.Email,
            //                 Phone = u.Phone,
            //                 Status = u.Status,
            //                 RoleName = r.Rolename
            //             };


            // // var query = _context.Users.Where(u=>u.Status == false);


            // int totalRecords = await query.CountAsync();

            // var users = await query.OrderBy(u => u.UserName).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

            // var model = new PaginationViewModel()
            // {
            //     UserList = users,
            //     PageNo = pageNo,
            //     PageSize = pageSize,
            //     TotalRecords = totalRecords,
            //     TotalPages = (totalRecords + pageSize - 1) / pageSize
            // };

            ViewData["ActiveLink"] = "Users";
            return View();
        }

        public JsonResult GetUserList(int pageNo, int pageSize)
        {
            var query = from u in _context.Users
                        join r in _context.Roles on u.Roleid equals r.RoleId
                        where u.Isdeleted == false
                        select new UserListViewModel
                        {
                            UserName = u.Username,
                            Email = u.Email,
                            Phone = u.Phone,
                            Status = u.Status,
                            RoleName = r.Rolename
                        };
            var users =  query.OrderBy(u => u.UserName).Skip((pageNo - 1) * pageSize).Take(pageSize);
            var userJson = new JsonResult(users);
            return userJson;
        }
    }
}
