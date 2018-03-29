using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels
{
    public class UserTypeViewModel
    {
        public string Guid { get; set; }
        public bool IsAdmin { get; set; }
    }


    public class UserDeleteViewModel
    {
        public string Guid { get; set; }
        public bool IsDeleting { get; set; }
    }

    public class UserEditViewModel
    {
        [Required]
        public string Guid { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        [EmailAddress]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}