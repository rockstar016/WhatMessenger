using System.IO;

namespace API.Models.RequestModels
{
    public class RegisterRequest
    {
        public string USERNAME { get; set; }
        public string USERPASSWORD { get; set; }
        public string EMAIL { get; set; }
        public string IMEI { get; set; }
        //public string USERPHONE;
        /**
         * 1: chinese, 0: english
         * */
        public string USERLANG { get; set; }

        public Stream FILE_STREAM { get; set; }
        public string FILE_NAME { get; set;} 
        public string FILE_PATH { get; set; }
    }
}