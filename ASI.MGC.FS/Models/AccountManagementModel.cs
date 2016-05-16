using System.ComponentModel.DataAnnotations;

namespace ASI.MGC.FS.Models
{
    public class UserLoginViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(250)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(25,MinimumLength=8)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class UserRegistartionViewModal
    {
        private const string Pattern = @"^[a-zA-Z][a-zA-Z0-9]{5,11}$";
        [Required]
        [StringLength(250)]
        [RegularExpression(@"^[a-zA-Z\s]+$")]
        [Display(Name="First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(250)]
        [RegularExpression(@"^[a-zA-Z\s]+$")]
        [Display(Name="Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(250)]
        [RegularExpression(Pattern)]
        public string UserName { get; set; }

        [Required]
        [StringLength(250)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(25,MinimumLength=8)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePassword
    {
        [Required]
        [StringLength(250)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 8)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 8)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}