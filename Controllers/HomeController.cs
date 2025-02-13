using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzashopMVCProject.Models;
using PizzashopMVCProject.ViewModels;

namespace PizzashopMVCProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PizzashopDbContext _context;
    public HomeController(ILogger<HomeController> logger, PizzashopDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model){
       
        // var users = await _context.Users.FirstOrDefaultAsync(q => q.Email == model.Email);

        if(ModelState.IsValid){
            var users = _context.Users
                    .Where(p => p.Email == model.Email)
                    .Select(x => new
                    {
                        x.Email,
                        x.Password,
                    })
                    .FirstOrDefault();

            if(users != null && users.Password == model.Password){
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


