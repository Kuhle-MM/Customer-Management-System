using System.ComponentModel.DataAnnotations;

namespace Customer_Management_System.Controllers
{
    public class VatNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value == null)
            {
                return null;
            }
            
            if (value is string vatNumber)
            {
                if (!string.IsNullOrWhiteSpace(vatNumber) && vatNumber.StartsWith("4") && vatNumber.Length == 10){
                    return ValidationResult.Success;
                }
                return new ValidationResult("Please enter a valid South African VAT number");
            }


            return base.IsValid(value, validationContext);
        }
    }
}
