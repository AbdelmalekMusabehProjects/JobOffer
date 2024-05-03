using System;
using System.Collections.Generic;

#nullable disable

namespace JobOffer.Models
{
    public partial class Addressh
    {
        public Addressh()
        {
            Jobhs = new HashSet<Jobh>();
        }

        public decimal Addressid { get; set; }
        public string Addressname { get; set; }
        public string Addresscity { get; set; }
        
        public virtual ICollection<Jobh> Jobhs { get; set; }
    }
}
