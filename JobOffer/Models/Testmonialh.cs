using System;
using System.Collections.Generic;

#nullable disable

namespace JobOffer.Models
{
    public partial class Testmonialh
    {
        public decimal Testmonialid { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public decimal? Userid { get; set; }

        public virtual Useraccounth User { get; set; }
    }
}
