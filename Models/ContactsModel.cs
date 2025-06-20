using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgencyBot.Models
{
    public class ContactsModel
    {
        [Required]
        [StringLength(50)]

        public string Phone { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public double Address { get; set; }


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
