using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tournament_Organizer.Models
{
    public class Tournament_Leaderboard
    {
        [Required]
        [Display(Name = "Home Team ID")]
        public Guid Team_One_ID { get; set; }
        [Display(Name = "Home Team")]
        public Team Team_One { get; set; }
        [Display(Name = "Home Team Score")]
        [Range(0, 50, ErrorMessage = "Value must be between 1 to 50")]
        public int  Team_One_Score { get; set; }
        [Display(Name = "Away Team ID")]
        [Required]
        public Guid Team_Two_ID { get; set; }
        [Display(Name = "Away Team")]
        public Team Team_Two { get; set; }
        [Display(Name = "Away Team Score")]
        [Range(0, 50, ErrorMessage = "Value must be between 1 to 50")]
        public int Team_Two_Score { get; set; }
        [Display(Name = "Tournament ID")]
        public Guid Tournament_ID { get; set; }
        [Display(Name = "Tournament Name")]
        public Tournament Tournament { get; set; }

        [Required]
        [Display(Name = "Game Date")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset Game_Date { get; set; }

        [StringLength(1000)]
        [Display(Name = "Game Notes")]
        public string GameNotes { get; set; }

        public uint Stage { get; set; }

        [Display(Name = "Game Status")]
        public string Game_Status { get; set; }

      
    }
}
