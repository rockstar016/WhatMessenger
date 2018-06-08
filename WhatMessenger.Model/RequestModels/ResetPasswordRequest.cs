namespace API.Models.RequestModels
{
    public class ResetPasswordRequest
    {
        public string EMAIL { get; set; }
        public string IMEI { get; set; }
        public string OLD_PASS { get; set; }
        public string NEW_PASS { get; set; }
    }
}