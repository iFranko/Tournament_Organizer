using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tournament_Organizer.Models
{
    public class Ms_Player_Position
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
        public string Position_Name { get; set; }
        [StringLength(2)]
        public string Position_Code { get; set; }

    }
}
