using System;
using System.Collections.Generic;

#nullable disable

namespace JobOffer.Models
{
    public partial class Roleh
    {
        public Roleh()
        {
            Useraccounths = new HashSet<Useraccounth>();
        }

        public decimal Roleid { get; set; }
        public string Rolename { get; set; }

        public virtual ICollection<Useraccounth> Useraccounths { get; set; }
    }
}
