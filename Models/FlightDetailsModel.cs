using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgencyBot.Models
{
    public class FlightDetailsModel
    {
        [Required]
        public Guid FlightId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string DepartureDate { get; set; }
        public string ReturnDate { get; set; }
        public double Price { get; set; } // Optional: calculated in summary

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Created On")]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedOn { get; set; }
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }
        [Display(Name = "Modified On")]
        [Column(TypeName = "datetime2")]
        public DateTime ModifiedOn { get; set; }
        [Display(Name = "Is Active?")]
        public bool isActive { get; set; }
        [Display(Name = "Is Deleted?")]
        public bool isDeleted { get; set; }
    }
}
