namespace JobOffer.Models
{
    public class ApplyJobViewJoin
    {
        public Useraccounth Account { get; set; }

        public Addressh Address { get; set; }
        public Applyjob JobApp { get; set; }
        public Attchmenth Attach { get; set; }
        public Jobh Job { get; set; }
    }
}
