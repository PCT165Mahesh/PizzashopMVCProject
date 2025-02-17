using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzashopMVCProject.Models;
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


     // Passsword Encryption
    public static string EncryptPassword(string password){
        if(string.IsNullOrEmpty(password)){
            return null;
        }
        else{
            byte[] storedPassword = ASCIIEncoding.ASCII.GetBytes(password);
            string encryptedPassword = Convert.ToBase64String(storedPassword);
            return encryptedPassword;
        }
    }

    // Passsword Decryption
    public static string DecryptPassword(string password){
        if(string.IsNullOrEmpty(password)){
            return null;
        }
        else{
            byte[] encryptedPassword = Convert.FromBase64String(password);
            string decyptedPassword = ASCIIEncoding.ASCII.GetString(encryptedPassword);
            return decyptedPassword;
        }
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

        var emailToken = Guid.NewGuid().ToString();
        var resetPasswordLink = Url.Action("ResetPassword", "Home", new {model.Email, emailToken}, Request.Scheme);

        string emailBody = $@"<div
        class=''
        style='background-color: #0066a7; display:flex; justify-content:center; align-items: center;'
        >
        <img src='https://images.vexels.com/media/users/3/128437/isolated/preview/2dd809b7c15968cb7cc577b2cb49c84f-pizza-food-restaurant-logo.png' alt='' width='50px' />
        <span style='color: #ffffff; font-weight: 550; font-size: 25px'
            >PIZZASHOP</span
        >
        </div>
        <div style='background-color: #f2f2f2'>
        <p>Pizza shop,</p>
        <p>Please click <a style='color:blue; text-decoration:underline;' href='{resetPasswordLink}' >here</a> for reset your Account Password.</p>
        <p>
            If you encounter any issues or have any questions, please do not
            hesitate to contact our support team.
        </p>
        <p>
            <span style='color: orange'>Important Note:</span>
            For security reasons, the link will expire in 24 hours, if you did not
            request a password reset, please ignore this email or contact our
            support team immediately.
        </p>
        </div>
            ";
        
        
        var user = await _context.Users
                    .Where(p => p.Email == model.Email)
                    .Select(x => new
                    {
                        x.Email,
                        x.Password,
                    })
                    .FirstOrDefaultAsync();
        
        if(ModelState.IsValid){
            if(user != null){
                await _emailSender.SendEmailAsync(model.Email, "Password Reset Link", emailBody);
            }
            else{
                ModelState.AddModelError("", "Email Is not found");
            }
            return View();
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult ResetPassword(string email){
        // Console.WriteLine(email);
        ViewBag.Email = email;
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPassViewModel model, string email){
        if(ModelState.IsValid){
            if(model.Password == model.ConfirmPassword){
                var user = await _context.Users.Where(u=>u.Email == email).FirstOrDefaultAsync();
                if(user != null){
                    user.Password = EncryptPassword(model.Password);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login");
                }
                else{
                    ModelState.AddModelError("", "user not found");
                    return View(model);
                }
            }
            else{
                ModelState.AddModelError("", "Password and Confirm Password Do not Match");
                return View(model);
            }
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


