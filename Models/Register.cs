namespace TALKPOLL.Models
{
    public class Register
    {
        public string userid { get; set; } 
        
        public string firstname { get; set; } 
        
        public string lastname { get; set; }
       
        public string email { get; set; }
       
        public string password { get; set; }

        public string gender { get; set; }

        public string phonenumber { get; set; }

        public string usertype { get; set; }
        public DateTime date_of_birth { get; set; }
    }
}