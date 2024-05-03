using System;
using System.Collections.Generic;

#nullable disable

namespace JobOffer.Models
{
    public partial class Jobcategoryh
    {
        public Jobcategoryh()
        {
            Jobhs = new HashSet<Jobh>();
        }

        public decimal Jobcategoryid { get; set; }
        public string Jobcategoryname { get; set; }

        public virtual ICollection<Jobh> Jobhs { get; set; }
    }
}
