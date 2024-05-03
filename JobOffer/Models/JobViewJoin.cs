using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobOffer.Models
{
    public class JobViewJoin
    {
        public Addressh Address { get; set; }

        public Jobh Job { get; set; }

        public Useraccounth Account { get; set; }

        [NotMapped]
        public virtual IFormFile PdfFile { get; set; }
    }
}
