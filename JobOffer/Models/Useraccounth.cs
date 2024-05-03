using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace JobOffer.Models
{
    public partial class Useraccounth
    {
        public Useraccounth()
        {
            Applyjobs = new HashSet<Applyjob>();
            Attchmenths = new HashSet<Attchmenth>();
            Jobhs = new HashSet<Jobh>();
            Testmonialhs = new HashSet<Testmonialh>();
        }

        public decimal Userid { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phonenumber { get; set; }
        public string Industialname { get; set; }
        public decimal? Roleid { get; set; }
        public string Password { get; set; }
        public string Imagepath { get; set; }

        [NotMapped]
        public virtual IFormFile ImageFile { get; set; }
        public virtual Roleh Role { get; set; }
        public virtual ICollection<Applyjob> Applyjobs { get; set; }
        public virtual ICollection<Attchmenth> Attchmenths { get; set; }
        public virtual ICollection<Jobh> Jobhs { get; set; }
        public virtual ICollection<Testmonialh> Testmonialhs { get; set; }
    }
}
