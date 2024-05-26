using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobOffer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static JobOffer.Enums.ApplicationEnums;
using AspNetCoreHero.ToastNotification.Abstractions;
using JobOffer.GeneralComponent;

namespace JobOffer.Controllers
{
    public class AdminController : Controller 
    {
        #region Objects
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;
        private readonly INotyfService _notyf;
        List<string> status = new List<string> { "Pending", "Accept", "Reject" };
        List<string> JobType = new List<string> { "Part Time", "Full Time" };
        #endregion

        #region Constructor
        public AdminController(ModelContext context, IWebHostEnvironment webHostEnviroment, INotyfService notyf)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
            _notyf = notyf;

        }
        #endregion

        #region Methods

        #region Dashboard
        public IActionResult Dashboard()
        {
            try
            {
                #region Get The Numbers Of (Job Posted, Candidates, Companies)
                ViewBag.NumberOfJobPosted = _context.Jobhs.Count(x => x.Status == Status.Accept.ToString());
                ViewBag.NumberOfJobNotPosted = _context.Jobhs.Count(x => x.Status == Status.Pending.ToString() || x.Status == Status.Reject.ToString());
                ViewBag.NumberOfPeople = _context.Useraccounths.Count(x => x.Roleid == 2);
                ViewBag.NumberOfCompanies = _context.Useraccounths.Count();
                #endregion
                ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
                string AdminName = ViewBag.AdminUser;
                ViewBag.PImage = HttpContext.Session.GetString("imagePath");

                return View("~/Views/Admin/Dashboard/Dashboard.cshtml");
            }
            catch (Exception ex)
            {
                _notyf.Error("Something Went Wrong!");
                _notyf.Information("ErrMsg: " + ex.Message + "\n\n" +
                                   "StacKTrace: " + ex.StackTrace, 6);
                return View("~/Views/Admin/Dashboard/Dashboard.cshtml");
            }
        }
        #endregion

        #region Manage Users

        #region Get Data Users
        public async Task<IActionResult> MainUser()
        {
            try
            {
                ViewBag.PImage = HttpContext.Session.GetString("imagePath");
                var modelContext = _context.Useraccounths.Include(u => u.Role);
                return View("~/Views/Admin/ManageUsers/MainUser.cshtml", modelContext);
            }
            catch (Exception ex)
            {
                _notyf.Error("Something Went Wrong!");
                _notyf.Information("ErrMsg: " + ex.Message + "\n\n" +
                                   "StacKTrace: " + ex.StackTrace, 6);
                return View("~/Views/Admin/ManageUsers/MainUser.cshtml");
            }
        }
        #endregion

        #region Get Data Users Details
        public async Task<IActionResult> DetailsUser(decimal? id)
        {
            try
            {
                ViewBag.PImage = HttpContext.Session.GetString("imagePath");

                if (id == null)
                {
                    return NotFound();
                }

                var useraccounth = await _context.Useraccounths
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(m => m.Userid == id);
                if (useraccounth == null)
                {
                    return NotFound();
                }

                return View("~/Views/Admin/ManageUsers/DetailsUser.cshtml", useraccounth);
            }
            catch (Exception ex)
            {
                _notyf.Error("Something Went Wrong!");
                _notyf.Information("ErrMsg: " + ex.Message + "\n\n" +
                                   "StacKTrace: " + ex.StackTrace, 6);
                return View("~/Views/Admin/ManageUsers/DetailsUser.cshtml");
            }
        }
        #endregion

        #region Create User

        #region Get 
        public IActionResult CreateUser()
        {
            ViewBag.PImage = HttpContext.Session.GetString("imagePath");

            ViewData["Roleid"] = new SelectList(_context.Rolehs, "Roleid", "Rolename");
            return View("~/Views/Admin/ManageUsers/CreateUser.cshtml");
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("Userid,Fullname,Username,Email,Phonenumber,Industialname,Roleid,Password,Imagepath, ImageFile")] Useraccounth useraccounth)
        {
            try
            {
                ViewBag.PImage = HttpContext.Session.GetString("imagePath");

                if (ModelState.IsValid)
                {
                    useraccounth.Imagepath = useraccounth.ImageFile != null ?
                               new Localization().SaveImage(_webHostEnviroment, useraccounth.ImageFile.FileName, "PersonalImages", useraccounth.ImageFile) :
                               string.Empty;
                    
                    _context.Add(useraccounth);
                    if(_context.SaveChangesAsync().IsCompletedSuccessfully)
                    {
                         _notyf.Success("Successfully Added");
                    } 
                    
                    return RedirectToAction(nameof(MainUser));
                }
                ViewData["Roleid"] = new SelectList(_context.Rolehs, "Roleid", "Rolename", useraccounth.Roleid);
                return View("~/Views/Admin/ManageUsers/CreateUser.cshtml", useraccounth);
            }
            catch (Exception ex)
            {
                _notyf.Error("Something Went Wrong!");
                _notyf.Information("ErrMsg: " + ex.Message + "\n\n" +
                                   "StacKTrace: " + ex.StackTrace, 6);
                return View("~/Views/Admin/ManageUsers/CreateUser.cshtml");
            }
        }
        #endregion

        #endregion

        #region Edit User

        #region Get
        public async Task<IActionResult> EditUser(decimal? id)
        {
            ViewBag.PImage = HttpContext.Session.GetString("imagePath");

            if (id == null)
            {
                return NotFound();
            }

            var useraccounth = await _context.Useraccounths.FindAsync(id);
            if (useraccounth == null)
            {
                return NotFound();
            }
            ViewData["Roleid"] = new SelectList(_context.Rolehs, "Roleid", "Rolename", useraccounth.Roleid);
            return View("~/Views/Admin/ManageUsers/EditUser.cshtml", useraccounth);
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(decimal id, [Bind("Userid,Fullname,Username,Email,Phonenumber,Industialname,Roleid,Password,Imagepath, ImageFile")] Useraccounth useraccounth)
        {
            ViewBag.PImage = HttpContext.Session.GetString("imagePath");

            if (id != useraccounth.Userid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (useraccounth.ImageFile != null)
                    {
                        useraccounth.Imagepath = useraccounth.ImageFile != null ?
                               new Localization().SaveImage(_webHostEnviroment, useraccounth.ImageFile.FileName, "PersonalImages", useraccounth.ImageFile) :
                               string.Empty;
  
                    }
                    else
                    {
                        useraccounth.Imagepath = _context.Useraccounths.Where(x => x.Userid == id).AsNoTracking<Useraccounth>().FirstOrDefault().Imagepath;

                    }

                    _context.Update(useraccounth);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Successfully Updated!");


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UseraccounthExists(useraccounth.Userid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MainUser));
            }
            ViewData["Roleid"] = new SelectList(_context.Rolehs, "Roleid", "Rolename", useraccounth.Roleid);
            return View("~/Views/Admin/ManageUsers/EditUser.cshtml", useraccounth);
        }
        #endregion

        #endregion

        #region Delete User

        #region Get
        public async Task<IActionResult> DeleteUser(decimal? id)
        {
            ViewBag.PImage = HttpContext.Session.GetString("imagePath");

            if (id == null)
            {
                return NotFound();
            }

            var useraccounth = await _context.Useraccounths
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (useraccounth == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/ManageUsers/DeleteUser.cshtml", useraccounth);
        }
        #endregion

        #region Post
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(decimal id)
        {
            try
            {
                ViewBag.PImage = HttpContext.Session.GetString("imagePath");

                var useraccounth = await _context.Useraccounths.FindAsync(id);
                _context.Useraccounths.Remove(useraccounth);
                await _context.SaveChangesAsync();
                _notyf.Success("Successfully Deleted!");

                return RedirectToAction(nameof(MainUser));
            }
            catch (Exception ex)
            {
                _notyf.Error("Something Went Wrong!");
                _notyf.Information("ErrMsg: " + ex.Message + "\n\n" +
                                   "StacKTrace: " + ex.StackTrace, 6);
                return RedirectToAction(nameof(MainUser));

            }
        }
        #endregion

        #endregion

        #region Check Table Exist
        private bool UseraccounthExists(decimal id)
        {
            return _context.Useraccounths.Any(e => e.Userid == id);
        }
        #endregion

        #endregion

        #region Manage Jobs

        #region Manage Pending Jobs
        public async Task<IActionResult> ManagePendingJobs()
        {

            var modelContext = _context.Jobhs.Include(j => j.Address).
                                              Include(j => j.Jobcategory).
                                              Include(j => j.User).
                                              Where(x => x.Status == Status.Pending.ToString());
           
			return View("~/Views/Admin/ManageJobs/ManagePendingJobs.cshtml", modelContext); 
        }
        #endregion

        #region Manage Reject Jobs
        public async Task<IActionResult> ManageRejectingJobs()
        {
			var modelContext = _context.Jobhs.Include(j => j.Address).
											  Include(j => j.Jobcategory).
											  Include(j => j.User).
											  Where(x => x.Status == Status.Reject.ToString());
			return View("~/Views/Admin/ManageJobs/ManageRejectingJobs.cshtml", await modelContext.ToListAsync()); // Abu Shdooh <3
        }
        #endregion

        #region Accept Job
        public IActionResult AcceptJob(decimal id)
        {
            var JobStatus = _context.Jobhs.Where(x => x.Jobid == id).FirstOrDefault(); // Get All Information for the job
            JobStatus.Status = Status.Accept.ToString();

            var UserInfo = _context.Useraccounths.Where(x => x.Userid == JobStatus.Userid).FirstOrDefault(); // Get All Information for the User in this job
            _context.SaveChanges();

            #region Sending Email To User
            string subject = "Accepted For Posting " + JobStatus.Jobname;
            string body = "Ms / Mrs" + " " + UserInfo.Fullname + " "
                                        + "The Job That You Posted Before Is Accepted Which Is "
                                        + JobStatus.Jobname
                                        + " \n"
                                        + " See Your Page 'My Post Job' ";

            new Localization().sendEmail("201910668@students.asu.edu.jo", subject, body);
            #endregion

            return RedirectToAction("MainJobs", "Admin");
        }
        #endregion

        #region Reject Job
        public IActionResult RejectJob(decimal id)
        {
            var JobStatus = _context.Jobhs.Where(x => x.Jobid == id).FirstOrDefault();
            var UserInfo = _context.Useraccounths.Where(x => x.Userid == JobStatus.Userid).FirstOrDefault();
            JobStatus.Status = Status.Reject.ToString();
            _context.SaveChanges();

            #region Sending Email To User


            string Subject = "Rejecting For Posting Job " + JobStatus.Jobname;
            string body = "Ms / Mrs" + " " + UserInfo.Fullname + " "
                                        + "The Job That You Posted Before Is Rejected Which Is "
                                        + JobStatus.Jobname
                                        + " \n"
                                        + " Unfortunately Try Next Time and Thank You For Your Time";
            new Localization().sendEmail(UserInfo.Email, Subject, body);
           
            #endregion

            return RedirectToAction("MainJobs", "Admin");
        }
        #endregion

        #region Get Data Jobs 
        public async Task<IActionResult> MainJobs()
        {
            var modelContext = _context.Jobhs.Include(j => j.Address).Include(j => j.Jobcategory).Include(j => j.User);
            return View("~/Views/Admin/ManageJobs/MainJobs.cshtml", await modelContext.ToListAsync()); // Abu Shdooh <3
        }
        #endregion

        #region Get Data Details Jobs
        public async Task<IActionResult> DetailsJobs(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobh = await _context.Jobhs
                .Include(j => j.Address)
                .Include(j => j.Jobcategory)
                .Include(j => j.User)
                .FirstOrDefaultAsync(m => m.Jobid == id);
            if (jobh == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/ManageJobs/DetailsJobs.cshtml", jobh);
        }
        #endregion

        #region Create Jobs

        #region Get
        public IActionResult CreateJobs()
        {
           
            ViewData["Status"] = new SelectList(status);
            ViewData["JobType"] = new SelectList(JobType);
            ViewData["Addressid"] = new SelectList(_context.Addresshes, "Addressid", "Addresscity");
            ViewData["Jobcategoryid"] = new SelectList(_context.Jobcategoryhs, "Jobcategoryid", "Jobcategoryname");
            ViewData["Userid"] = new SelectList(_context.Useraccounths, "Userid", "Email");
            return View("~/Views/Admin/ManageJobs/CreateJobs.cshtml");
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJobs([Bind("Jobid,Jobname,Jobdescription,Jobsalary,Status,Jobtype,Userid,Addressid,Jobcategoryid, Jobimage, ImageFile")] Jobh job)
        {
            if (ModelState.IsValid)
            {
                string wwwrootPath = _webHostEnviroment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + "_" + job.ImageFile.FileName;
                string extension = Path.GetExtension(job.ImageFile.FileName);

                string path = Path.Combine(wwwrootPath + "/JobsImages/" + fileName);
                using (var filestream = new FileStream(path, FileMode.Create))
                {
                    job.ImageFile.CopyToAsync(filestream);
                }
                job.Jobimage = fileName;
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MainJobs));
            }
			ViewData["Status"] = new SelectList(status);
            ViewData["JobType"] = new SelectList(JobType);
            ViewData["Addressid"] = new SelectList(_context.Addresshes, "Addressid", "Addresscity", job.Addressid);
            ViewData["Jobcategoryid"] = new SelectList(_context.Jobcategoryhs, "Jobcategoryid", "Jobcategoryname", job.Jobcategoryid);
            ViewData["Userid"] = new SelectList(_context.Useraccounths, "Userid", "Email", job.Userid);
            return View("~/Views/Admin/ManageJobs/CreateJobs.cshtml", job);
        }
        #endregion

        #endregion

        #region Edit Jobs

        #region Get
        public async Task<IActionResult> EditJobs(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobh = await _context.Jobhs.FindAsync(id);
            if (jobh == null)
            {
                return NotFound();
            }

            ViewData["Status"] = new SelectList(status);
            ViewData["JobType"] = new SelectList(JobType);
            ViewData["Addressid"] = new SelectList(_context.Addresshes, "Addressid", "Addresscity", jobh.Addressid);
            ViewData["Jobcategoryid"] = new SelectList(_context.Jobcategoryhs, "Jobcategoryid", "Jobcategoryname", jobh.Jobcategoryid);
            ViewData["Userid"] = new SelectList(_context.Useraccounths, "Userid", "Email", jobh.Userid);
            return View("~/Views/Admin/ManageJobs/EditJobs.cshtml", jobh);
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJobs(decimal id, [Bind("Jobid,Jobname,Jobdescription,Jobsalary,Status,Jobtype,Userid,Addressid,Jobcategoryid, Jobimage, ImageFile")] Jobh jobh)
        {
            if (id != jobh.Jobid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (jobh.ImageFile != null)
                    {
                        string wwwrootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" + jobh.ImageFile.FileName;
                        string extension = Path.GetExtension(jobh.ImageFile.FileName);

                        string path = Path.Combine(wwwrootPath + "/JobsImages/" + fileName);
                        using (var filestream = new FileStream(path, FileMode.Create))
                        {
                            jobh.ImageFile.CopyToAsync(filestream);
                        }
                        jobh.Jobimage = fileName;
                    }
                    else
                    {
                        jobh.Jobimage = _context.Jobhs.Where(x => x.Jobid == id).AsNoTracking<Jobh>().FirstOrDefault().Jobimage;
                    }
                    _context.Update(jobh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobhExists(jobh.Jobid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MainJobs));
            }
            

            ViewData["Status"] = new SelectList(status);
			ViewData["JobType"] = new SelectList(JobType);
			ViewData["Addressid"] = new SelectList(_context.Addresshes, "Addressid", "Addresscity", jobh.Addressid);
            ViewData["Jobcategoryid"] = new SelectList(_context.Jobcategoryhs, "Jobcategoryid", "Jobcategoryname", jobh.Jobcategoryid);
            ViewData["Userid"] = new SelectList(_context.Useraccounths, "Userid", "Email", jobh.Userid);
            return View("~/Views/Admin/ManageJobs/EditJobs.cshtml", jobh);
        }
        #endregion

        #endregion

        #region Delete Jobs

        #region Get
        public async Task<IActionResult> DeleteJobs(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobh = await _context.Jobhs
                .Include(j => j.Address)
                .Include(j => j.Jobcategory)
                .Include(j => j.User)
                .FirstOrDefaultAsync(m => m.Jobid == id);
            if (jobh == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/ManageJobs/DeleteJobs.cshtml", jobh);
        }

        #endregion

        #region Post
        [HttpPost, ActionName("DeleteJobs")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJobs(decimal id)
        {
            var jobh = await _context.Jobhs.FindAsync(id);
            _context.Jobhs.Remove(jobh);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MainJobs));
        }
        #endregion

        #endregion

        #region Table Jobs Checked
        private bool JobhExists(decimal id)
        {
            return _context.Jobhs.Any(e => e.Jobid == id);
        }
        #endregion

        #endregion

        #region Manage Address

        #region Get Data Address
        public async Task<IActionResult> MainAddress()
        {
            return View("~/Views/Admin/ManageAddress/MainAddress.cshtml", await _context.Addresshes.ToListAsync());
        }
        #endregion

        #region Get Data Details Address
        public async Task<IActionResult> DetailsAddress(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addressh = await _context.Addresshes
                .FirstOrDefaultAsync(m => m.Addressid == id);
            if (addressh == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/ManageAddress/DetailsAddress.cshtml", addressh);
        }
        #endregion

        #region Create Address

        #region Get
        public IActionResult CreateAddress()
        {
            return View("~/Views/Admin/ManageAddress/CreateAddress.cshtml");
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAddress([Bind("Addressid,Addressname,Addresscity")] Addressh addressh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(addressh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MainAddress));
            }
            return View("~/Views/Admin/ManageAddress/CreateAddress.cshtml", addressh);
        }
        #endregion

        #endregion

        #region Edit Address

        #region Get
        public async Task<IActionResult> EditAddress(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addressh = await _context.Addresshes.FindAsync(id);
            if (addressh == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/ManageAddress/EditAddress.cshtml", addressh);
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(decimal id, [Bind("Addressid,Addressname,Addresscity")] Addressh addressh)
        {
            if (id != addressh.Addressid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(addressh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddresshExists(addressh.Addressid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MainAddress));
            }
            return View("~/Views/Admin/ManageAddress/EditAddress.cshtml", addressh);
        }
        #endregion

        #endregion

        #region Delete Address

        #region Get
        public async Task<IActionResult> DeleteAddress(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addressh = await _context.Addresshes
                .FirstOrDefaultAsync(m => m.Addressid == id);
            if (addressh == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/ManageAddress/DeleteAddress.cshtml", addressh);
        }
        #endregion

        #region Post
        [HttpPost, ActionName("DeleteAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddress(decimal id)
        {
            var addressh = await _context.Addresshes.FindAsync(id);
            _context.Addresshes.Remove(addressh);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MainAddress));
        }
        #endregion

        #endregion

        #region Table Address Checked
        private bool AddresshExists(decimal id)
        {
            return _context.Addresshes.Any(e => e.Addressid == id);
        }
        #endregion

        #endregion

        #region Manage Apply Job

        #region Get Data Apply Job
        public async Task<IActionResult> MainApplyJob()
        {
            var modelContext = _context.Applyjobs.Include(a => a.Attach).Include(a => a.Job).Include(a => a.User);
            return View("~/Views/Admin/ManageApplyjob/MainApplyJob.cshtml", await modelContext.ToListAsync());
        }
        #endregion

        #region Get Data Details Apply Job
        public async Task<IActionResult> DetailsApplyJob(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applyjob = await _context.Applyjobs
                .Include(a => a.Attach)
                .Include(a => a.Job)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Applyid == id);
            if (applyjob == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/ManageApplyjob/DetailsApplyJob.cshtml", applyjob);
        }
        #endregion

        #region Create Apply Job

        #region Get
        public IActionResult CreateApplyJob()
        {
            ViewData["Attachid"] = new SelectList(_context.Attchmenths, "Attachid", "Attachid");
            ViewData["Jobid"] = new SelectList(_context.Jobhs, "Jobid", "Jobdescription");
            ViewData["Userid"] = new SelectList(_context.Useraccounths, "Userid", "Email");
            return View("~/Views/Admin/ManageApplyjob/CreateApplyJob.cshtml");
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateApplyJob([Bind("Applyid,Attachid,Userid,Jobid")] Applyjob applyjob)
        {
            if (ModelState.IsValid)
                if (ModelState.IsValid)
                {
                    _context.Add(applyjob);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(MainApplyJob));
                }
            ViewData["Attachid"] = new SelectList(_context.Attchmenths, "Attachid", "Attachid", applyjob.Attachid);
            ViewData["Jobid"] = new SelectList(_context.Jobhs, "Jobid", "Jobdescription", applyjob.Jobid);
            ViewData["Userid"] = new SelectList(_context.Useraccounths, "Userid", "Email", applyjob.Userid);
            return View("~/Views/Admin/ManageApplyjob/CreateApplyJob.cshtml", applyjob);
        }
        #endregion

        #endregion

        #region Edit Apply Job

        #region Get
        public async Task<IActionResult> EditApplyJob(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applyjob = await _context.Applyjobs.FindAsync(id);
            if (applyjob == null)
            {
                return NotFound();
            }
            ViewData["Attachid"] = new SelectList(_context.Attchmenths, "Attachid", "Attachid", applyjob.Attachid);
            ViewData["Jobid"] = new SelectList(_context.Jobhs, "Jobid", "Jobdescription", applyjob.Jobid);
            ViewData["Userid"] = new SelectList(_context.Useraccounths, "Userid", "Email", applyjob.Userid);
            return View("~/Views/Admin/ManageApplyjob/EditApplyJob.cshtml", applyjob);
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditApplyJob(decimal id, [Bind("Applyid,Attachid,Userid,Jobid")] Applyjob applyjob)
        {
            if (id != applyjob.Applyid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applyjob);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplyjobExists(applyjob.Applyid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Attachid"] = new SelectList(_context.Attchmenths, "Attachid", "Attachid", applyjob.Attachid);
            ViewData["Jobid"] = new SelectList(_context.Jobhs, "Jobid", "Jobdescription", applyjob.Jobid);
            ViewData["Userid"] = new SelectList(_context.Useraccounths, "Userid", "Email", applyjob.Userid);
            return View("~/Views/Admin/ManageApplyjob/EditApplyJob.cshtml", applyjob);
        }
        #endregion

        #endregion

        #region Delete Apply Job

        #region Get
        public async Task<IActionResult> DeleteApplyJob(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applyjob = await _context.Applyjobs
                .Include(a => a.Attach)
                .Include(a => a.Job)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Applyid == id);
            if (applyjob == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/ManageApplyjob/DeleteApplyJob.cshtml", applyjob);

        }
        #endregion

        #region Post
        [HttpPost, ActionName("DeleteApplyJob")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteApplyJob(decimal id)
        {
            var applyjob = await _context.Applyjobs.FindAsync(id);
            _context.Applyjobs.Remove(applyjob);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MainApplyJob));
        }
        #endregion

        #endregion

        #region Table Apply Job Checked
        private bool ApplyjobExists(decimal id)
        {
            return _context.Applyjobs.Any(e => e.Applyid == id);
        }
        #endregion

        #endregion

        #region Manage Testmonial

        #region Manage Pending Testmonial
        public async Task<IActionResult> ManagePendingTestmonial()
        {
			var modelContext = _context.Testmonialhs.Include(t => t.User).Where(x => x.Status == Status.Pending.ToString());
			return View("~/Views/Admin/ManageTestmonial/ManagePendingTestmonial.cshtml", await modelContext.ToListAsync()); // Abu Shdooh <3
        }
        #endregion

        #region Manage Reject Jobs
        public async Task<IActionResult> ManageRejectingTestmonial()
        {
			var modelContext = _context.Testmonialhs.Include(t => t.User).Where(x => x.Status == Status.Reject.ToString());
			return View("~/Views/Admin/ManageTestmonial/ManageRejectingTestmonial.cshtml", await modelContext.ToListAsync()); // Abu Shdooh <3
        }
        #endregion

        #region Accept Testmonial
        public IActionResult AcceptTestmonial(decimal id)
        {
            var TestmonialStatus = _context.Testmonialhs.Where(x => x.Testmonialid == id).FirstOrDefault();

            TestmonialStatus.Status = Status.Accept.ToString();
            _context.SaveChanges();

            return RedirectToAction("MainTestmonials", "Admin");
        }
        #endregion

        #region Reject Testmonial
        public IActionResult RejectTestmonial(decimal id)
        {
            var TestmonialStatus = _context.Testmonialhs.Where(x => x.Testmonialid == id).FirstOrDefault();

            TestmonialStatus.Status = Status.Reject.ToString();
            _context.SaveChanges();

            return RedirectToAction("MainTestmonials", "Admin");
        }
        #endregion

        #region Get Data Testmonial 
        public async Task<IActionResult> MainTestmonials()
        {
            var modelContext = _context.Testmonialhs.Include(t => t.User);
            return View("~/Views/Admin/ManageTestmonial/MainTestmonials.cshtml", await modelContext.ToListAsync()); // Abu Shdooh <3
        }
        #endregion

        #region Get Data Details Testmonial
        public async Task<IActionResult> DetailsTestmonial(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testmonialh = await _context.Testmonialhs
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Testmonialid == id);
            if (testmonialh == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/ManageTestmonial/DetailsTestmonial.cshtml", testmonialh);

        }
        #endregion

        #region Create Testmonial

        #region Get
        public IActionResult CreateTestmonial()
        {
            ViewData["Status"] = new SelectList(status);
            ViewData["Userid"] = new SelectList(_context.Useraccounths, "Userid", "Email");
            return View("~/Views/Admin/ManageTestmonial/CreateTestmonial.cshtml");
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTestmonial([Bind("Testmonialid,Message,Status,Userid")] Testmonialh testmonialh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(testmonialh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MainTestmonials));
            }

            ViewData["Status"] = new SelectList(status);
            ViewData["Userid"] = new SelectList(_context.Useraccounths, "Userid", "Email", testmonialh.Userid);
            return View("~/Views/Admin/ManageTestmonial/CreateTestmonial.cshtml", testmonialh);
        }
        #endregion

        #endregion

        #region Edit Testmonial

        #region Get
        public async Task<IActionResult> EditTestmonial(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testmonialh = await _context.Testmonialhs.FindAsync(id);
            if (testmonialh == null)
            {
                return NotFound();
            }

            ViewData["Status"] = new SelectList(status);
            ViewData["Userid"] = new SelectList(_context.Useraccounths, "Userid", "Email", testmonialh.Userid);
            return View("~/Views/Admin/ManageTestmonial/EditTestmonial.cshtml", testmonialh);
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTestmonial(decimal id, [Bind("Testmonialid,Message,Status,Userid")] Testmonialh testmonialh)
        {
            if (id != testmonialh.Testmonialid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testmonialh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestmonialhExists(testmonialh.Testmonialid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MainTestmonials));
            }

            ViewData["Status"] = new SelectList(status);
            ViewData["Userid"] = new SelectList(_context.Useraccounths, "Userid", "Email", testmonialh.Userid);
            return View("~/Views/Admin/ManageTestmonial/EditTestmonial.cshtml", testmonialh);
        }
        #endregion

        #endregion

        #region Delete Testmonial

        #region Get
        public async Task<IActionResult> DeleteTestmonial(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testmonialh = await _context.Testmonialhs
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Testmonialid == id);
            if (testmonialh == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/ManageTestmonial/DeleteTestmonial.cshtml", testmonialh);

        }

        #endregion

        #region Post
        [HttpPost, ActionName("DeleteTestmonial")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTestmonial(decimal id)
        {
            var testmonialh = await _context.Testmonialhs.FindAsync(id);
            _context.Testmonialhs.Remove(testmonialh);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MainTestmonials));
        }
        #endregion

        #endregion

        #region Table Testmonial Checked
        private bool TestmonialhExists(decimal id)
        {
            return _context.Testmonialhs.Any(e => e.Testmonialid == id);
        }
        #endregion

        #endregion

        #region Manage My Prorfile

        #region View Profile
        public IActionResult MyProfile()
        {
            #region Get The Username and UserId From Session
            ViewBag.Username = HttpContext.Session.GetString("AdminUser");
            ViewBag.UserId = HttpContext.Session.GetInt32("AdminId");

            int UserI = ViewBag.UserId;
            ViewBag.FullName = _context.Useraccounths.Where(x => x.Userid == UserI).Select(x => x.Fullname).FirstOrDefault();
            #endregion

            #region Query To Get Profile By Id
            var myProfile = _context.Useraccounths.Where(x => x.Userid == UserI).FirstOrDefault();
            #endregion

            return View("~/Views/Admin/ManageMyProfile/MyProfile.cshtml", myProfile);
        }
        #endregion

        #region Edit Profile

        #region Get
        public IActionResult EditMyProfile()
        {
            return View("~/Views/Admin/ManageMyProfile/EditMyProfile.cshtml");
        }
        #endregion

        #region post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMyProfile(decimal UserId, [Bind("Userid,Fullname,Username,Email,Phonenumber,Password,Industialname,Imagepath")] Useraccounth useraccounth)
        {
            #region Query To Get Role Id
            useraccounth.Roleid = _context.Useraccounths.Where(x => x.Userid == UserId).Select(x => x.Roleid).FirstOrDefault();
            #endregion

            _context.Update(useraccounth);
            _context.SaveChanges();
            return RedirectToAction("Dashboard", "Admin");
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }
}
