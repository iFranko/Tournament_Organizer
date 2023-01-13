using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace Tournament_Organizer.Models
{
    public class Review_User_Team
    {

        public Guid Team_ID { get; set; }
        public Team Team { get; set; }

        public string User_ID { get; set; }
        public User User { get; set; }

        public int Review { get; set; }
        [Display(Name = "User Role")]
        public string User_Role { get; set; }

    }
}
