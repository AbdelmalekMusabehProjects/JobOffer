using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace JobOffer.Models
{
    public partial class Jobh
    {
        public Jobh()
        {
            Applyjobs = new HashSet<Applyjob>();
        }

        public decimal Jobid { get; set; }
        public string Jobname { get; set; }
        public string Jobdescription { get; set; }
        public decimal Jobsalary { get; set; }
        public string Status { get; set; }
        public string Jobtype { get; set; }
        public decimal? Userid { get; set; }
        public decimal? Addressid { get; set; }
        public decimal? Jobcategoryid { get; set; }
        public string Jobimage { get; set; }

        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }

        public virtual Addressh Address { get; set; }
        public virtual Jobcategoryh Jobcategory { get; set; }
        public virtual Useraccounth User { get; set; }
        public virtual ICollection<Applyjob> Applyjobs { get; set; }
    }
}
