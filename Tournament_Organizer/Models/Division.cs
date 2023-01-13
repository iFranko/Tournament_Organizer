using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tournament_Organizer.Models
{
    
    public partial class Division
    {

        [Key]
        public Guid Division_Id { get; set; }
        [Display(Name = "Division Name")]
        [StringLength(50)]
        public string DivisionName { get; set; }
    }
}
