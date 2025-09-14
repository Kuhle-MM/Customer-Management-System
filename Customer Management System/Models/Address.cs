using System.ComponentModel.DataAnnotations;

namespace Customer_Management_System.Models
{
    public class Address
    {

        [Required]
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "Street Name cannot be longer than 100 characters")]        
        [Required]
        [Display(Name = "Street Name")]
        public string StreetName { get; set; }

        [StringLength(100, ErrorMessage = "Suburb cannot be longer than 100 characters")]
        [Required]
       
        public string Suburb { get; set; }

        [StringLength(100, ErrorMessage = "City cannot be longer than 100 characters")]
        [Required]
        public string City { get; set; }
        
        [StringLength(100, ErrorMessage = "Province cannot be longer than 100 characters")]
        [Required]
        public string Province { get; set; }
        
        [StringLength(50, ErrorMessage = "Postal cannot be longer than 100 characters")]
        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

    }
}
