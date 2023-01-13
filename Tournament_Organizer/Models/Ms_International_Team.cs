using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tournament_Organizer.Models
{
    public class Ms_International_Team
    {
        [Key]
        public Guid ID { get; set; }
        /// <summary>
        /// Gets or sets the Creation Time.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTimeOffset Created_Date_Time { get; set; }
        [Required]
        [StringLength(150)]
        public string International_Name { get; set; }
        [Required]
        [Display(Name = "international_logo")]
        public byte[] International_Logo { get; set; }

    }
}
