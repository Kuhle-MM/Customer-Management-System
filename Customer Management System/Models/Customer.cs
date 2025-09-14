using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Customer_Management_System.Controllers;

namespace Customer_Management_System.Models
{
    //This class is for defining the customer object
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        [Required(ErrorMessage = "Full Name is Required")] //ensuring that the name and address can not be null
        [Display(Name = "Full Name" )]
        public string Name { get; set; }

        public int AddressID { get; set; } // To find the correct address in the address table 
        [Required(ErrorMessage = "Address is Required")] 
        public Address Address { get; set; } //could make this a list if there is a need for multiple address

        [NotMapped] // for display purposes combines the attributes of address
        public string FullAddress =>
            Address == null
                ? string.Empty
                : $"{Address.StreetName}, {Address.Suburb}, {Address.City}, {Address.Province}, {Address.PostalCode}";

        [StringLength(50, ErrorMessage = "Name cannot be longer than 100 characters")]
        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [StringLength(100,ErrorMessage = "Name cannot be longer than 100 characters")]
        [Display(Name = "Contact Name")]
        public string? ContactName { get; set; }

        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        [EmailAddress]
        [Display(Name = "Contact Email")]
        public string? ContactEmail { get; set; }

        [VatNumber] // Validation check for South African VAT numbers
        [Display(Name = "VAT Number")]
        public string? VATNumber { get; set; }

        public bool IsVerified { get; set; } = false;
        [MaxLength(200)]
        public string? VerificationToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
    }
}
