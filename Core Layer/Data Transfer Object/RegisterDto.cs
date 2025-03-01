using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Data_Transfer_Object
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string? ImageUrl { get; set; }
        
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password Name is Required")]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[a-zA-Z0-9])(?=.*[^a-zA-Z0-9]).{8,}$",
        ErrorMessage = "The password must contain at least one lowercase letter, " +
            "one uppercase letter, and one number. and non alpha numeric ")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password Is Required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm Password doesn't Match")]
        public string ConfirmPassword  { get; set;}
        public bool IsBlind { get; set; }
    }
}
