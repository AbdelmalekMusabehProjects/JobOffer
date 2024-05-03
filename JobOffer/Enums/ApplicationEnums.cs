namespace JobOffer.Enums
{
    public class ApplicationEnums
    {
        #region Status Enum
       public enum Status
        {
            Accept,
            Pending,
            Reject
        }
        #endregion

        #region Job Type Enum
        enum jobType
        {
            PartTime = 1,
            FullTime = 2
        }
        #endregion
    }
}
