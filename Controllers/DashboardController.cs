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
    private readonly ILogger<DashboardController> _logger;
    private readonly PizzashopDbContext _context;
    private readonly  JwtService _JwtService;

    public DashboardController(ILogger<DashboardController> logger, PizzashopDbContext context, JwtService JwtService)
    {
        _logger = logger;
        _context = context;
        _JwtService = JwtService;
    }

    public IActionResult Index(){
        var token = Request.Cookies["SuperSecretAuthToken"];
        var email = _JwtService.GetClaimValue(token, "email");
        // var role = _JwtService.GetClaimValue(token, "role");

        var user = _context.Users.Where(e => e.Email == email).FirstOrDefault();
        DashboardViewModel model = new DashboardViewModel();
        ViewData["UserName"] = user.Username;
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
        }
        return View(model);
    }
}
