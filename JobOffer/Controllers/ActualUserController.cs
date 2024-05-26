using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobOffer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static JobOffer.Enums.ApplicationEnums;
using AspNetCoreHero.ToastNotification.Abstractions;
using JobOffer.GeneralComponent;

namespace JobOffer.Controllers
{
    public class ActualUserController : Controller
    {
        #region Object 
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;
        private readonly INotyfService _notyf;

        #endregion

        #region Constructors
        public ActualUserController(ModelContext context, IWebHostEnvironment webHostEnviroment, INotyfService notyf)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
            _notyf = notyf;
        }
        #endregion

        #region Methods

        #region Home
        public IActionResult Home()
        {

            try
            {
                #region Get The Username and UserId From Session  
                ViewBag.Username = HttpContext.Session.GetString("ActualUser");
                HttpContext.Session.GetInt32("UserId");
                #endregion

                #region List Of (Job, Address, UserAccount)
                var JobList = _context.Jobhs.Where(x => x.Status == Status.Accept.ToString()).ToList();
                var AddressList = _context.Addresshes.ToList();
                var AccountsList = _context.Useraccounths.ToList();
                #endregion

                #region Get The Numbers Of (Job Posted, Candidates, Companies)
                ViewBag.NumberOfJobPosted = JobList.Count(x => x.Status == Status.Accept.ToString());
                ViewBag.NumberOfPeople = AccountsList.Count(x => x.Roleid == 2);
                ViewBag.NumberOfCompanies = AccountsList.Count();
                #endregion

                #region Join Tables Between(Job, Address, UserAccount)
                var modelView = from addr in AddressList
                                join job in JobList on addr.Addressid equals job.Addressid
                                join Acc in AccountsList on job.Userid equals Acc.Userid
                                select new JobViewJoin { Job = job, Address = addr, Account = Acc };
                #endregion

                return modelView != null ? View(modelView) : View();
            }
            catch (Exception ex)
            {
                _notyf.Error("Something Went Wrong!");
                _notyf.Information("ErrMsg: " + ex.Message + "\n\n" +
                                   "StacKTrace: " + ex.StackTrace, 6);
                return View();
            }
        }
        #endregion

        #region About
        public IActionResult About()
        {
            try
            {
                #region Get The Username and UserId From Session
                ViewBag.Username = HttpContext.Session.GetString("ActualUser");
                HttpContext.Session.GetInt32("UserId");
                #endregion

                return View();
            }

            catch (Exception ex)
            {
                _notyf.Error("Something Went Wrong!");
                _notyf.Information("ErrMsg: " + ex.Message + "\n\n" +
                                   "StacKTrace: " + ex.StackTrace, 6);
                return View();
            }
        }
        #endregion

        #region Contact

        #region Get
        public IActionResult Contact()
        {
            return View();
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact([Bind("fname, lname, email, subject, message")] SendEmail Semail)
        {
            try
            {
                #region Get The Username and UserId From Session
                ViewBag.Username = HttpContext.Session.GetString("ActualUser");
                HttpContext.Session.GetInt32("UserId");
                var userid = HttpContext.Session.GetInt32("UserId");
                #endregion

                #region Get User
                var user = _context.Useraccounths.Where(x => x.Userid == userid).FirstOrDefault();
                #endregion

                #region Send Email To Admin And User

                string body = "The Problem From " + " " + Semail.fname + " " + Semail.lname + " " + Semail.message;
                new Localization().sendEmail("201910668@students.asu.edu.jo", Semail.subject, body);

                string bodyUser = "Ms / Mrs" + " " + Semail.fname + " " + Semail.lname + " " + "Thank You For Contacting Us :)";
                new Localization().sendEmail(Semail.email, "Contacting Us!", bodyUser);

                #endregion

                return View();
            }
            catch (Exception ex)
            {
                _notyf.Error("Something Went Wrong!");
                _notyf.Information("ErrMsg: " + ex.Message + "\n\n" +
                                   "StacKTrace: " + ex.StackTrace, 6);
                return View();
            }
        }
        #endregion

        #endregion

        #region Services
        public IActionResult Services()
        {
            try
            {
                #region Get The Username and UserId From Session
                ViewBag.Username = HttpContext.Session.GetString("ActualUser");
                HttpContext.Session.GetInt32("UserId");
                #endregion

                return View();
            }
            catch (Exception ex)
            {
                _notyf.Error("Something Went Wrong!");
                _notyf.Information("ErrMsg: " + ex.Message + "\n\n" +
                                   "StacKTrace: " + ex.StackTrace, 6);
                return View();
            }
        }
        #endregion

        #region Testmonial
        public async Task<IActionResult> Testmonial([Bind("Testmonialid, Message, Status, Userid")] Testmonialh testmonial)
        {
            try
            {
                #region Get The Username and UserId From Session
                ViewBag.Username = HttpContext.Session.GetString("ActualUser");
                HttpContext.Session.GetInt32("UserId");
                var userId = HttpContext.Session.GetInt32("UserId");
                #endregion

                #region Set The Status And User Id
                testmonial.Status = Status.Pending.ToString();
                testmonial.Userid = userId;
                #endregion

                #region insert Testmonial
                if (!string.IsNullOrEmpty(testmonial.Message))
                {
                    await _context.AddAsync(testmonial);
                    await _context.SaveChangesAsync();
                }

                #endregion

                #region Join Tables Between(Testmonial, UserAccount)
                var modelView = from user in _context.Useraccounths.ToList()
                                join test in _context.Testmonialhs.Where(x => x.Status == Status.Accept.ToString()).ToList() on user.Userid equals test.Userid
                                where test.Status == Status.Accept.ToString()
                                select new TestmonialViewJoin { Test = test, Account = user };
                #endregion

                return modelView != null ? View(modelView) : View();
            }
            catch (Exception ex)
            {
                _notyf.Error("Something Went Wrong!");
                _notyf.Information("ErrMsg: " + ex.Message + "\n\n" +
                                   "StacKTrace: " + ex.StackTrace, 6);
                return View();
            }
        }

        #endregion

        #region View Profile

        public IActionResult MyProfile()
        {
            try
            {
                #region Get The Username and UserId From Session
                ViewBag.Username = HttpContext.Session.GetString("ActualUser");
                ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
                int UserI = ViewBag.UserId;
                ViewBag.FullName = _context.Useraccounths.Where(x => x.Userid == UserI).Select(x => x.Fullname).FirstOrDefault();
                #endregion

                #region Query To Get Profile By Id
                var myProfile = _context.Useraccounths.Where(x => x.Userid == UserI).FirstOrDefault();
                #endregion

                return View(myProfile);
            }
            catch (Exception ex)
            {
                _notyf.Error("Something Went Wrong!");
                _notyf.Information("ErrMsg: " + ex.Message + "\n\n" +
                                   "StacKTrace: " + ex.StackTrace, 6);
                return View();
            }
        }

        #endregion

        #region Edit Profile

        #region Get
        public IActionResult EditMyProfile()
        {
            return View();
        }
        #endregion

        #region post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMyProfile(decimal UserId, [Bind("Userid,Fullname,Username,Email,Phonenumber,Password,Industialname,Imagepath, ImageFile")] Useraccounth useraccounth)
        {
            try
            {
                useraccounth.Roleid = _context.Useraccounths.Where(x => x.Userid == UserId).Select(x => x.Roleid).FirstOrDefault();

                if (useraccounth.ImageFile != null)
                {
                    useraccounth.Imagepath = useraccounth.ImageFile != null ?
                                   new Localization().SaveImage(_webHostEnviroment, useraccounth.ImageFile.FileName, "PersonalImages", useraccounth.ImageFile) :
                                   string.Empty;
                }
                else
                {
                    useraccounth.Imagepath = _context.Useraccounths.Where(x => x.Userid == UserId).AsNoTracking<Useraccounth>().FirstOrDefault().Imagepath;
                }

                _context.Update(useraccounth);
                _context.SaveChanges();
                _notyf.Success("Updated Successfully");
                return RedirectToAction("Home", "ActualUser");
            }

            catch (Exception ex)
            {
                _notyf.Error("Something Went Wrong!");
                _notyf.Information("ErrMsg: " + ex.Message + "\n\n" +
                                   "StacKTrace: " + ex.StackTrace, 6);
                return RedirectToAction("Home", "ActualUser");
            }
        }
        #endregion

        #endregion

        #region Search

        public IActionResult Search(string job)
            {
            try
            {
                #region List Of (Job, Address, UserAccount)
                var JobList = _context.Jobhs.Where(x => x.Status == Status.Accept.ToString()).ToList();
                var AddressList = _context.Addresshes.ToList();
                var AccountsList = _context.Useraccounths.ToList();
                #endregion

                #region Join Tables Between(Job, Address, UserAccount)
                var modelView = from addr in AddressList
                                join jobb in JobList on addr.Addressid equals jobb.Addressid
                                join Acc in AccountsList on jobb.Userid equals Acc.Userid
                                select new JobViewJoin { Job = jobb, Address = addr, Account = Acc };
                #endregion

                #region Search Process
                if (job == null)
                {
                    return View(nameof(Home), modelView);
                }
                else
                {

                    var result = modelView.Where(x => x.Job.Jobname.ToLower().Contains(job.ToLower()) || x.Job.Jobname.ToLower().StartsWith(job.ToLower())).ToList();
                    //   return RedirectToAction ("Home", "ActualUser");
                    return View(nameof(Home), result);
                }
                #endregion
            }
            catch (Exception ex)
            {

                _notyf.Error("Something Went Wrong!");
                _notyf.Information("ErrMsg: " + ex.Message + "\n\n" +
                                   "StacKTrace: " + ex.StackTrace, 6);
                return View();
            }

        }

        #endregion

        #endregion
    }


}
