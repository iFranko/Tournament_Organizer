using System;
using System.ComponentModel.DataAnnotations;

namespace Tournament_Organizer.Models
{
    public class Tournament_Team
    {

        public Guid Team_ID { get; set; }
        public Team Team { get; set; }
        public Guid Tournament_ID { get; set; }
        public Tournament Tournament { get; set; }
        [RegularExpression("Pending|Approved|Rejected", ErrorMessage = "Please Select one of the 3 options Pending, Approved, Rejected")]
        public string Status { get; set; }

       
        [RegularExpression("Yes|No", ErrorMessage = "Yes or No")]
        public string Active { get; set; }

        public string Group { get; set; }

        public int? NumberOfMatches { get; set; } = 0;

        public int? Points { get; set; } = 0;
        public int? Scores { get; set; } = 0;


    }
}
