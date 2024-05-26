using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JobOffer.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using JobOffer.GeneralComponent;

namespace JobOffer.Controllers
{
    public class AuthController : Controller
    {
        #region Objects
        private readonly INotyfService _notyf;
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;
        #endregion

        #region Constructors
        public AuthController(ModelContext context, IWebHostEnvironment webHostEnviroment, INotyfService notyf)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
            _notyf = notyf;
        }
        #endregion

        #region Methods

        #region Login

        #region Get
        public IActionResult Login()
        {
            return View();
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Username, Password")] Useraccounth user)
        {
            #region query To Authnticate The User
            var auth = _context.Useraccounths.Where(x => x.Username == user.Username && x.Password == user.Password).FirstOrDefault();
            #endregion

            if (auth != null)
            {
                switch (auth.Roleid)
                {
                    case 1:
                        #region Session For Admin's Username and AdminId
                        HttpContext.Session.SetString("AdminUser", auth.Username);
                        HttpContext.Session.SetInt32("AdminId", (int)auth.Userid);
                        HttpContext.Session.SetString("imagePath", auth.Imagepath);
                        #endregion

                        return RedirectToAction("Dashboard", "Admin");
                    case 2:
                        #region Session For User's Username and UserId
                        HttpContext.Session.SetString("ActualUser", user.Username);
                        HttpContext.Session.SetInt32("UserId", Convert.ToInt32(auth.Userid));
                        #endregion

                        return RedirectToAction("Home", "ActualUser");
                }
            }
            else
            {
                Response.WriteAsync("<script>alert('Try Again Username or Password incorect')</script>");
            }
            return View();
        }
        #endregion

        #endregion

        #region Sign Up

        #region Get
        public IActionResult signUp()
        {
            return View();
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> signUp([Bind("Fullname, Username, Email, Phonenumber, Industialname, Password, Imagepath, ImageFile")] Useraccounth user)
        {
            try
            {
                if (user != null)
                {
                    user.Roleid = 2;
                    if (user.Password.Length < 6)
                    {
                        _notyf.Warning("Password Must Be Greater than 6 Character", 10);
                        return View();
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            user.Imagepath = user.ImageFile != null ?
                                   new Localization().SaveImage(_webHostEnviroment, user.ImageFile.FileName, "JobsImages", user.ImageFile) :
                                   string.Empty;

                            user.Industialname = string.IsNullOrEmpty(user.Industialname) ? "  " : user.Industialname;

                              _context.Add(user);
                             await _context.SaveChangesAsync();
                             _notyf.Success("Successfully Created!", 10);

                            string body = "Dear " + user.Fullname + ", \n\n" +
                                "Thank You For Your Registration For JobBoard, \n\n" +
                                "Enjoy Our Services And Easiest Appling Jobs,\n\n" +
                                "All The Best and Good Luck.\n\n" +
                                "Best Regards. \n\n" +
                                "JobBoard Team",

                                subject = "JobBoard New Account :)";
                           
                            new Localization().sendEmail(user.Email, subject, body);

                            
                            return RedirectToAction("Login", "Auth");
                           
                        }
                    }


                }
                return View();
            }
            catch (Exception ex)
            {

                _notyf.Error("Something Went Wrong ...");
                _notyf.Information("Error Msg :" + ex.Message + "\n" +
                                   "StackTrace : " + ex.StackTrace);
                throw;
            }
        }
        #endregion

        #endregion

        #region LogOut

        public IActionResult logOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        #endregion

        #region ForgotPass

        #region Get
        public IActionResult ForgotPass()
        {
            return View();
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPass(string ConfrirmPass, [Bind("Username, Password")] Useraccounth useraccount)
        {
            var UserInfo = _context.Useraccounths.Where(x => x.Username == useraccount.Username).FirstOrDefault();
            if (UserInfo != null)
                if (!string.IsNullOrEmpty(ConfrirmPass) && !string.IsNullOrEmpty(useraccount.Password))
                    if (useraccount.Password.Equals(ConfrirmPass))
                    {
                        UserInfo.Password = ConfrirmPass;
                        _context.SaveChanges();
                    }
                    else
                    {
                        Response.WriteAsync("<script>alert('The Password Not Equal Confirm Password')</script>");
                        return View();
                    }

                else
                {
                    Response.WriteAsync("<script>alert('Sohuld Enter Not Null Value')</script>");
                    return View();
                }
            else
                return View();

            return RedirectToAction("Login", "Auth");


        } 
        #endregion
        #endregion

        #endregion
    }
}
