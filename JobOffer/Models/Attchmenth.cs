using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace JobOffer.Models
{
    public partial class Attchmenth
    {
        public Attchmenth()
        {
            Applyjobs = new HashSet<Applyjob>();
        }

        public decimal Attachid { get; set; }
        public string Attchpath { get; set; }
        public decimal? Userid { get; set; }
        [NotMapped]
        public virtual IFormFile PdfFile { get; set; }


        public virtual Useraccounth User { get; set; }
        public virtual ICollection<Applyjob> Applyjobs { get; set; }
    }
}
