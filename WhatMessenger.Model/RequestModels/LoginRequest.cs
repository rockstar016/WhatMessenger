namespace API.Models.RequestModels
{
    public class LoginRequest
    {
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }
        public string IMEI { get; set; }
    }
}