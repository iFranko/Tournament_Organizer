using System;
using System.ComponentModel.DataAnnotations;

namespace Tournament_Organizer.Models
{
    public class Tournament_User
    {

        public Guid Tournament_ID { get; set; }
        public Tournament Tournament { get; set; }

        public string User_ID { get; set; }

        public User User { get; set; }

    }
}
