using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tournament_Organizer.Models
{
    public class User : IdentityUser
    {

       
        [Display(Name = "First Name")]
       // [Required(ErrorMessage = "First Name is Required.")]
        [StringLength(150)]
        public string First_Name { get; set; }

        [Display(Name = "Last Name")]
        //[Required(ErrorMessage = "Last Name is Required.")]
        [StringLength(150)]
        public string Last_Name { get; set; }

        [Display(Name = "User Role")]
        [Required(ErrorMessage = "Role is Required.")]
        [RegularExpression("Player|Organizer|Admin|Manager", ErrorMessage = "Please Select one of the 3 options Organizer, Manager, or Player")]
        [StringLength(150)]
        public string User_Role { get; set; }

        public uint Age { get; set; }

        [Display(Name = "Image")]
        public byte[] Image { get; set; }

        [Display(Name = "Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime? Created_Date_Time { get; set; } = DateTime.Now;


        public bool? Ban { get; set; } = false;

        //Relationships

        [Display(Name = "Team ID")]
        public List<Team_User> Team_User { get; set; }

        public List<Tournament_User> Tournament_User { get; set; }

        [Display(Name = "Favorite International Team")]
        [StringLength(150)]
        public string Fav_International_Team { get; set; }

        [Display(Name = "Favorite Club Team")]
        [StringLength(150)]
        public string Fav_Club_Team { get; set; }

        [Display(Name = "Position")]
        [StringLength(150)]
        public string Fav_Position { get; set; }


        public List<Review_User_Team> Review_User_Team { get; set; }
        public List<Review_User_Tournament> Review_User_Tournament { get; set; }

    }
}
