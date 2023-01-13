
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tournament_Organizer.Models
{
    public class Team
    {

        [Key]
        public Guid ID { get; set; }
        /// <summary>
        /// Gets or sets the Creation Time.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Created Date Time")]
        public DateTimeOffset? Created_Date_Time { get; set; } = DateTimeOffset.Now;

        [Required]
        [StringLength(250)]
        [Display(Name = "Team Name")]
        public string Team_Name { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Jersey Colour")]
        public string JerseyColour { get; set; }


        [Required]
        [Range(5, 30,
        ErrorMessage = "Value for must be between 5 and 30")]
        [Display(Name = "Max Players In Team")]
        public uint Max_Player_Team { get; set; }

        [Display(Name = "Image")]
        public byte[] Image { get; set; }

        public bool? Ban { get; set; } = false;
        [Required]
        [StringLength(150)]
        [Display(Name = "Country")]
        public string Country { get; set; }
        [Required]
        [StringLength(150)]
        public string City { get; set; }
        [Required]
        [StringLength(150)]
        public string Address { get; set; }
        [Required]
        [StringLength(150)]
        [Display(Name = "Zip Code")]
        [DataType(DataType.PostalCode)]
        public string Zip_Code { get; set; }

        public double Review { get; set; }


        [Required]
        [Display(Name = "Division")]
        public Guid Division_Id { get; set; }
        // Relationships

        public List<Division> Division { get; set; }

        public List<Team_User>  Team_User { get; set; }

        public List<Tournament_Team> Tournament_Team { get; set; }



        public List<Tournament_Leaderboard> Tournament_Team_One { get; set; }
        public List<Tournament_Leaderboard> Tournament_Team_Two { get; set; }



        public List<Review_User_Team> Review_User_Team { get; set; }
    }
}
