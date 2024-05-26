using AspNetCoreHero.ToastNotification.Abstractions;
using JobOffer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using static JobOffer.Enums.ApplicationEnums;


namespace JobOffer.Controllers
{
    public class GuestUserController : Controller
    {
        #region Object 
        private readonly INotyfService _notyf;
        private readonly ModelContext _context;
        #endregion

        #region Constructors
        public GuestUserController(ModelContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }
        #endregion

        #region Methods

        #region Home
        public IActionResult Home()
        {
            try
            {
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


                return View(modelView);
            }
            catch (Exception ex)
            {
                _notyf.Warning(ex.Message);
                return View();
            }
        }
        #endregion

        #region About
        public IActionResult About()
        {
            return View();
        }
        #endregion

        #region Contact
        public IActionResult Contact()
        {
            return View();
        }
        #endregion

        #region Services
        public IActionResult Services()
        {
            return View();
        }
        #endregion
        
        #region Testmonial
        public IActionResult Testmonial()
        {
            #region Join Tables Between(Testmonial, UserAccount)
            var modelView = from user in _context.Useraccounths.ToList()
                            join test in _context.Testmonialhs.Where(x => x.Status == Status.Accept.ToString()).ToList() on user.Userid equals test.Userid
                            where test.Status == Status.Accept.ToString()
                            select new TestmonialViewJoin { Test = test, Account = user };
            #endregion

            return View(modelView);
        }
        #endregion

        #region Search

        public IActionResult Search(string job)
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
                return View(nameof(Home), result);
            }
            #endregion

        }

        #endregion

        #endregion

    }
}
