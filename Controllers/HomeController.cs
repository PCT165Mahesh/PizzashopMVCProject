using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzashopMVCProject.Models;
using PizzashopMVCProject.Utilty;
using PizzashopMVCProject.ViewModels;

namespace PizzashopMVCProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PizzashopDbContext _context;
    private readonly IEmailSender _emailSender;

    public HomeController(ILogger<HomeController> logger, PizzashopDbContext context, IEmailSender emailSender)
    {
        _logger = logger;
        _context = context;
        _emailSender = emailSender;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if(Request.Cookies["userMail"] != null){
            return RedirectToAction("Successful");
        }
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model){
       
        // var users = await _context.Users.FirstOrDefaultAsync(q => q.Email == model.Email);

        CookieOptions options = new CookieOptions();
        options.Expires = DateTime.Now.AddDays(30);

        if(ModelState.IsValid){
            var users = await _context.Users
                    .Where(p => p.Email == model.Email)
                    .Select(x => new
                    {
                        x.Email,
                        x.Password,
                    })
                    .FirstOrDefaultAsync();

            if(users != null && users.Password == model.Password){

                // Remember Me :- Using Cookie
                if(model.RememberMe == true){
                    Response.Cookies.Append("userMail", model.Email, options);
                }
                return RedirectToAction("Successful");
            }
            else{
                ModelState.AddModelError("password", "Incorrect Credentials");
            }
            return View(model);
        }

        return View(model);
    }

    public IActionResult Successful()
    {
        return View();
    }

    // HttpGet Logout Action
    public IActionResult Logout(){
        Response.Cookies.Delete("userMail");
        return RedirectToAction("Login");
    }

    // Forgot Password HTTPGET
    public IActionResult ForgotPassword(string email){
        
        if(string.IsNullOrEmpty(email)){
            ViewBag.UserEmail = "";
        }
        ViewBag.UserEmail = email;
        return View();
    }

    // Forgot Password HTTPGET
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPassViewModel model){
        
        if(!string.IsNullOrEmpty(model.Email)){
            await _emailSender.SendEmailAsync(model.Email, "Password Reset Link", "<h3>Click here to reset the password</h3>");
            return RedirectToAction("Successful");
        }
        
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


