using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using JobOffer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using static JobOffer.Enums.ApplicationEnums;
using AspNetCoreHero.ToastNotification.Abstractions;
using JobOffer.GeneralComponent;

namespace JobOffer.Controllers
{
    public class JobsController : Controller
    {
        #region Objects
        private readonly INotyfService _notyf;
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;
        #endregion

        #region Constructors
        public JobsController(ModelContext context, IWebHostEnvironment webHostEnviroment, INotyfService notyf)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
            _notyf = notyf;
        }
        #endregion

        #region Methods

        #region Post a Job

        #region Get
        public IActionResult postJob()
        {
            #region Values Of Drop Down List(AddressName, AddressCity, CatygoryJob)
            ViewData["AddressName"] = new SelectList(_context.Addresshes, "Addressid", "Addressname");
            ViewData["AddressCity"] = new SelectList(_context.Addresshes, "Addressid", "Addresscity");
            ViewData["Jobcategoryid"] = new SelectList(_context.Jobcategoryhs, "Jobcategoryid", "Jobcategoryname"); 
            #endregion

            return View();
        }
        #endregion

        #region Post
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult postJob([Bind("Jobname, Jobdescription, Jobtype, Jobsalary, Addressid, Jobcategoryid, Jobimage, ImageFile")] Jobh job)
        {

            try
            {
                #region 'Set The User Id:The Current User' and 'set Status:Pending'
                job.Userid = (decimal)HttpContext.Session.GetInt32("UserId");
                job.Status = "Pending";
                #endregion
                
                job.Jobimage = job.ImageFile != null ?
                               new Localization().SaveImage(_webHostEnviroment, job.ImageFile.FileName, "JobsImages", job.ImageFile) :
                               string.Empty;
                
                    
                _context.Add(job);
                _context.SaveChanges();
                _notyf.Success("Successfully Posted");
                _notyf.Information("Under process By Admin, You Will Receive an email from Admin when accept", 6);

                #region Objects
                var UserId = HttpContext.Session.GetInt32("UserId");
                var UserInfo = _context.Useraccounths.Where(x => x.Userid == UserId).FirstOrDefault();
                var JobInfo = _context.Jobhs.Where(x => x.Jobid == job.Jobid).FirstOrDefault();
                var EmailInfo = _context.Useraccounths.Where(x => x.Userid == JobInfo.Userid).FirstOrDefault();
                var AdminInfo = _context.Useraccounths.Where(x => x.Roleid == 1).FirstOrDefault();
                Applyjob applyjob = new Applyjob();
                #endregion

                #region Sending Email To Admin
                string subject = "Post a Job For " + JobInfo.Jobname + " " + UserInfo.Fullname;
                string body = "The User " + " " + UserInfo.Fullname + " "
                                             + "Post For The Job "
                                             + JobInfo.Jobname
                                             + " at "
                                             + DateTime.Now.ToLongDateString()
                                             + " Check The Page For Your Manage Jobs";

                new Localization().sendEmail(AdminInfo.Email, subject, body);
                #endregion

                #region Sending Email To User

                subject = "Post a Job For " + JobInfo.Jobname;
                body = "Ms / Mrs" + " " + UserInfo.Fullname + " "
                                            + "Thank You For Posting The Job "
                                            + JobInfo.Jobname
                                            + " The Admin Will Response To You As Possible as :) \n"
                                            + "Note: See Your Page 'My Post Job' If The Admin Accepted";

                new Localization().sendEmail(UserInfo.Email, subject, body);
                #endregion

                #region Values Of Drop Down List(AddressName, AddressCity, CatygoryJob)
                ViewData["AddressName"] = new SelectList(_context.Addresshes, "Addressid", "Addressname", job.Addressid);
                ViewData["AddressCity"] = new SelectList(_context.Addresshes, "Addressid", "Addresscity", job.Addressid);
                ViewData["Jobcategoryid"] = new SelectList(_context.Jobcategoryhs, "Jobcategoryid", "Jobcategoryname", job.Jobcategory);
                #endregion

                return View();
            }
            catch (Exception ex)
            {
                _notyf.Error("Something Went Wrong ...");
                _notyf.Information("Error Msg :" + ex.Message + "\n" +
                                   "StackTrace : " + ex.StackTrace);
                return View();
            }
        }
        #endregion

        #endregion
        
        #region My Post Jobs

        public IActionResult MyPostJob()
        {
            #region List Of (Job, Address, UserAccount)
            var JobList = _context.Jobhs.Where(x => x.Status == Status.Accept.ToString()).ToList();
            var AddressList = _context.Addresshes.ToList();
            var AccountsList = _context.Useraccounths.ToList();
            #endregion

            #region Join Tables Between(Job, Address)
            var modelView = from addr in AddressList
                            join job in JobList on addr.Addressid equals job.Addressid
                            join Acc in AccountsList on job.Userid equals Acc.Userid
                            where job.Userid == HttpContext.Session.GetInt32("UserId")
                            select new JobViewJoin { Job = job, Address = addr, Account = Acc };

            #endregion

            return View(modelView);
        }

        #endregion

        #region Apply a Job        
        public IActionResult ApplyJob(decimal? Jobid)
        {
            ViewBag.Jobid = Jobid;

            #region List Of (Job, Address, UserAccount)
            var JobList = _context.Jobhs.Where(x => x.Status == Status.Accept.ToString()).ToList();
            var AddressList = _context.Addresshes.ToList();
            var AccountsList = _context.Useraccounths.ToList();
            #endregion

            #region Join Tables Between(Job, Address)
            var modelView = from addr in AddressList
                            join job in JobList on addr.Addressid equals job.Addressid
                            join Acc in AccountsList on job.Userid equals Acc.Userid
                            where job.Jobid == Jobid
                            select new JobViewJoin { Job = job, Address = addr, Account = Acc };
            #endregion

            return View(modelView);
        }

        #endregion
            
        #region Apply a Job With Submission
        public IActionResult ApplyJobSubmission( decimal JobId, [Bind("Attchpath, PdfFile")] Attchmenth attchment)
        {
            #region Objects
            var UserId = HttpContext.Session.GetInt32("UserId");
            var UserInfo = _context.Useraccounths.Where(x => x.Userid == UserId).FirstOrDefault();
            var JobInfo = _context.Jobhs.Where(x => x.Jobid == JobId).FirstOrDefault();
            var EmailInfo = _context.Useraccounths.Where(x => x.Userid == JobInfo.Userid).FirstOrDefault();
            Applyjob applyjob = new Applyjob(); 
            #endregion

            #region Make a File
            string wwwrootPath = _webHostEnviroment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + "_" + attchment.PdfFile.FileName;
            string extension = Path.GetExtension(attchment.PdfFile.FileName);

            string path = Path.Combine(wwwrootPath + "/CvFiles/" + JobInfo.Jobcategory + "/" + JobInfo.Jobid +"_" + fileName);
            using (var filestream = new FileStream(path, FileMode.Create))
            {
                attchment.PdfFile.CopyToAsync(filestream);
            }
            #endregion

            #region Fill Data Attachment
            attchment.Attchpath = fileName;
            attchment.Userid = UserId;
            _context.Add(attchment);
            _context.SaveChanges(); 
            #endregion

            #region Fill Data Apply
            applyjob.Attachid = _context.Attchmenths.Where(x => x.Userid == UserId).Select(x => x.Attachid).FirstOrDefault();
            applyjob.Userid = UserId;
            applyjob.Jobid = JobId; 
            _context.Add(applyjob);
            _context.SaveChanges();
            #endregion

            #region Sending Email To Admin

            string subjectAdmin = "Applied Job For "  + JobInfo.Jobname + " " + UserInfo.Fullname;
            string bodyAdmin = "The User " + " " + UserInfo.Fullname + " "
                                         + "Applied For The Job"
                                         + JobInfo.Jobname
                                         + " at "
                                         + DateTime.Now.ToLongDateString();

            new Localization().sendEmail(EmailInfo.Email, subjectAdmin, bodyAdmin);

         
              
            #endregion

            #region Sending Email To User
         
            string subject = "Apply Job For " + JobInfo.Jobname;
            string body = "Ms / Mrs" + " " + UserInfo.Fullname + " "
                                                               + "Thank You For Applying The Job "
                                                                + JobInfo.Jobname;
            
            new Localization().sendEmail(UserInfo.Email, subject, body);

            #endregion

            return RedirectToAction("Home", "ActualUser");
        }

        #endregion

        #region My Applied Jobs
        public IActionResult MyAppliedJobs()
        {
            
            var UserId = HttpContext.Session.GetInt32("UserId");
            var JobId = _context.Applyjobs.Where(x => x.Userid == UserId).Select(x => x.Jobid).FirstOrDefault();
            var JobAppliedList = from JobApply in _context.Applyjobs 
                                 join acc in _context.Useraccounths on JobApply.Userid equals acc.Userid
                                 join job in _context.Jobhs on acc.Userid equals job.Userid
                                 join add in _context.Addresshes on job.Addressid equals add.Addressid
                                 where JobApply.Userid == UserId 
                                 where job.Status == Status.Accept.ToString()
                                 select new ApplyJobViewJoin { Job = job, JobApp = JobApply, Address = add, Account = acc};


            return View(JobAppliedList);
        }
        #endregion


        #endregion
    }


}
