using System;
using System.Collections.Generic;

#nullable disable

namespace JobOffer.Models
{
    public partial class Applyjob
    {
        public decimal Applyid { get; set; }
        public decimal? Attachid { get; set; }
        public decimal? Userid { get; set; }
        public decimal? Jobid { get; set; }

        public virtual Attchmenth Attach { get; set; }
        public virtual Jobh Job { get; set; }
        public virtual Useraccounth User { get; set; }
    }
}
