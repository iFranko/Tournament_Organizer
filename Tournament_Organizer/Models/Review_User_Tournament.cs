using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Tournament_Organizer.Models
{
    public class Review_User_Tournament
    {

        public Guid Tournament_ID { get; set; }
        public Tournament Tournament { get; set; }

        public string User_ID { get; set; }
        public User User { get; set; }
        public int Review { get; set; }
        [Display(Name = "User Role")]
        public string User_Role { get; set; }

    }
}
