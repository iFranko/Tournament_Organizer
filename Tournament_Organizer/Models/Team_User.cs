using System;
using System.ComponentModel.DataAnnotations;

namespace Tournament_Organizer.Models
{
    public class Team_User
    {

      
        public Guid Team_ID { get; set; }
        public Team Team { get; set; }

        public string User_ID { get; set; }

        public User User { get; set; }

        public string Status { get; set; }
        [Display(Name = "User Role")]
        public string User_Role { get; set; }
    }
}
