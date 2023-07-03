using CI_PLATFORM.DataDB;
using CI_PLATFORM.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;




namespace CI_PLATFORM.Controllers
{


    public class UserAuthenticationController : Controller
    {

        private readonly CiPlatformContext _db;
        private readonly IConfiguration _configuration;

        public UserAuthenticationController(CiPlatformContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;


        }


        public IActionResult Login(/*string ReturnUrl = ""*/)
        {
            //LoginViewModel loginobj = new LoginViewModel()
            //{
            //    ReturnUrl = ReturnUrl
            //};
            return View(/*loginobj*/);
        }
        [HttpPost]
        [Route("UserAuthentication/Login", Name = "UserLogin")]
        public async Task<IActionResult> Login(Models.LoginViewModel loginobj/*,string ReturnUrl*/)
        {
            if (ModelState.IsValid)
            {

                var user = _db.Users.Where(x => x.Email == loginobj.Email && x.Password == loginobj.Password).FirstOrDefault();

                if (user == null)
                {
                    ModelState.AddModelError("Password", "Invalid Credentials !!");
                    return View(loginobj);
                }
                else
                {
                    var claims = new List<Claim>
                        {
                        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Sid, user.UserId.ToString()),
                        new Claim(ClaimTypes.Actor, user.Avatar),
                        };
                    var identity = new ClaimsIdentity(claims, "AuthCookie");
                    var Principle = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync("AuthCookie", Principle);

                    HttpContext.Session.SetString("UserId", user.UserId.ToString());

                    var options = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(30),
                        IsEssential = true,
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict
                    };
                
                    Response.Cookies.Append("Email", loginobj.Email, new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddDays(7),
                        IsEssential = true // Optional, but recommended.
                    });

                    //if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    //{
                    //    return Redirect(ReturnUrl);
                    //}
                    //else
                    //{
                        return RedirectToAction("MissionListing", "Mission");
                    //}

                    //return RedirectToAction("MissionListing", "Mission");
                }

            }
            else
            {
                return View();
            }
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
           
            Response.Cookies.Delete("Email");
            return RedirectToAction("Login", "UserAuthentication");
        }



        public IActionResult ForgotPassword()
        {
            return View();
        }

       
        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            string UserName = _configuration.GetSection("credential")["FromEmail"];
            string MyPassword = _configuration.GetSection("credential")["MyPassword"];
            string Port = _configuration.GetSection("credential")["Port"];
            string FromEmail = _configuration.GetSection("credential")["FromEmail"];
            string Host = _configuration.GetSection("credential")["Host"];


            // Validate the model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Generate a unique password reset token

            string token = Guid.NewGuid().ToString();


            // Save the token in your data store along with the user's email address
            DataDB.PasswordReset Token = new DataDB.PasswordReset()
            {
                Email = model.Email,
                Token = token,
                CreatedAt = DateTime.Now
            };
            _db.Add(Token);
            _db.SaveChanges();

            // Construct the reset password link with the token
            var resetLink = Url.Action("ResetPassword", "UserAuthentication", new {token}, Request.Scheme);

            // Send the email with the reset password link
            var message = new MailMessage();
            message.From = new MailAddress(FromEmail);
            message.To.Add(new MailAddress(model.Email));
            message.Subject = "Password Reset Request";
            message.Body = $"Please click the following link to reset your password: {resetLink}";
            message.IsBodyHtml = true;

            using var smtp = new SmtpClient(Host, int.Parse(Port))
            {
                Credentials = new NetworkCredential(UserName, MyPassword),
                EnableSsl = true
            };

            try
            {
                smtp.Send(message);
                TempData["Success"] = "Reset Password link has been successfully sent to your accounts..!!!";
                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }




        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [Route("UserAuthentication/Registration", Name = "UserRegistration")]
        public IActionResult Registration(Models.RegistrationViewModel registrationobj)
        {
            if (ModelState.IsValid)
            {
                if (registrationobj.Password != registrationobj.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password not matching..!");
                    return View(registrationobj);
                }
                DataDB.User existedUser = _db.Users.Where(u => u.Email == registrationobj.Email).FirstOrDefault();
                if (existedUser == null)
                {
                    DataDB.User user = new User()
                    {
                        FirstName = registrationobj.FirstName,
                        LastName = registrationobj.LastName,
                        PhoneNumber = registrationobj.PhoneNumber,
                        Email = registrationobj.Email,
                        Password = registrationobj.Password
                    };

                    _db.Users.Add(user);
                    _db.SaveChanges();
                    return View("Login");
                }
                else
                {
                    ModelState.AddModelError("Email", "Email Already Exists.!!");
                    return View(registrationobj);
                }

            }
            else
            {

                return View();

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public IActionResult ResetPassword(string token = "", string email = "")
        {
            DataDB.PasswordReset passwordReset = new DataDB.PasswordReset()
            {
                Token = token,
                Email = email
            };
            return View();
        }

        [HttpPost]
        [Route("UserAuthentication/ResetPassword", Name = "ResetPassword")]
        public IActionResult ResetPassword(Models.ResetPasswordViewModel model)
        {
            if (model.Password == model.ConfirmPassword && model.Password != null && model.ConfirmPassword != null)
            {
                DataDB.PasswordReset passwordReset = _db.PasswordResets.Where(p => p.Token == model.Token && p.CreatedAt.Value.AddMinutes(1) >= DateTime.Now).FirstOrDefault();

                if (passwordReset != null)
                {
                    // Change the password
                    User user = _db.Users.Where(u => u.Email == passwordReset.Email).FirstOrDefault();
                    user.Password = model.Password;
                    _db.Users.Update(user);
                    _db.Remove(passwordReset);
                    _db.SaveChanges();
                    return RedirectToAction("Login", "UserAuthentication");
                }
                else
                {

                    ModelState.AddModelError("ConfirmPassword", "Token is not valid");
                    _db.Remove(passwordReset);
                    _db.SaveChanges();
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("ConfirmPassword", "Password is not matching...!!!");
                return View(model);
            }

            return View();

        }
    }
}



