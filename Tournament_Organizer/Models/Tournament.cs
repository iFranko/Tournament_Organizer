
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tournament_Organizer.Models
{
    public class Tournament
    {

        [Key]
        public Guid ID { get; set; }
        /// <summary>
        /// Gets or sets the Creation Time.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Created Date & Time")]
        public DateTimeOffset? Created_Date_Time { get; set; }= DateTimeOffset.Now;

        [Required]
        [Display(Name = "Tournament Name")]
        [StringLength(250, MinimumLength = 2)]
        //[ForeignKey("Passport")]
        public string Tournament_Name { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset Start_Date { get; set; }
        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset End_Date { get; set; }
        [Required]
        [StringLength(150)]
        [Display(Name = "Tournament Type")]
        [RegularExpression("Soccer", ErrorMessage = "Only Soccer is allowed no other sport")]
        public string Tournament_Type { get; set; }

       

        [Display(Name = "Image")]
        public byte[] Image { get; set; }

        [Required]
        [Range(8, 32,
        ErrorMessage = "Value for must be between 4 and 16.")]
        [Display(Name = "Max Teams In Tournament")]
        public uint Max_Teams_Tournament { get; set; }


        [Required]
        [Range(5, 30,
        ErrorMessage = "Value for must be between 5 and 30.")]
        [Display(Name = "Max Players Per Team")]
        public uint Max_Players_Per_Team { get; set; }


        [Required]
        [Display(Name = "Division")]
        public Guid Division_Id { get; set; }

        public double Review { get; set; }
       

        public bool? Ban { get; set; } = false;

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Country { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string City { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Zip Code")]
        [DataType(DataType.PostalCode)]
        [StringLength(150, MinimumLength = 3)]
        public string Zip_Code { get; set; }

        [Required]
        [Display(Name = "1st Playoffs Rule")]
        public string Rule_One { get; set; }
        [Required]
        [Display(Name = "2nd Quarter Rule")]
        public string Rule_Two { get; set; }
        [Required]
        [Display(Name = "3rd Semi Final Rule")]
        public string Rule_Three { get; set; }
        [Required]
        [Display(Name = "4th Final Rule")]
        public string Rule_Four { get; set; }


        public uint Stage { get; set; }

        public uint NextStageMaxTeams { get; set; }


        //realtionships 


        public List<Team> Teams { get; set; }

        public List<Tournament_Team> Tournament_Team { get; set; }

        public List<Tournament_User> Tournament_User { get; set; }


        public List<Tournament_Leaderboard> Tournament_Leaderboard { get; set; }

        public List<Review_User_Tournament> Review_User_Tournament { get; set; }

    }

}
