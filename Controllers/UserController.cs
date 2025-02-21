using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            
            var query = _context.Users.Where(u => !u.Isdeleted);
            UserListViewModel model = new UserListViewModel();
            if(query != null){
                model.Users = query.ToList();
            }
            ViewData["ActiveLink"] = "Users";
            return View(model);
        }
    }
}
