using System.ComponentModel.DataAnnotations;

namespace AngularApp.API.Models.WebViewModels.UserActionModels
{
    /// <summary>
    /// A collection of models designed to allow operations upon the account details of a user
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal class NamespaceDoc
    {

    }

    /// <summary>
    /// A class designed to represent what type of user a user is
    /// </summary>
    /// <seealso cref="Controllers.AccountController"/>
    public class UserTypeViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public string Guid { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is admin.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is admin; otherwise, <c>false</c>.
        /// </value>
        public bool IsAdmin { get; set; }
    }


    /// <summary>
    /// A class designed represent a single request that a user be deleted from the
    /// database
    /// </summary>
    /// <seealso cref="Controllers.AccountController"/>
    public class UserDeleteViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public string Guid { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleting.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deleting; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleting { get; set; }
    }

    /// <summary>
    /// A class designed to represent a user whose details are being edited
    /// </summary>
    /// <remarks>
    /// This class has more parameters than the other two classes due to the fact
    /// that all of the parameters of this class could be edited upon request.
    /// </remarks>
    /// <seealso cref="Controllers.AccountController"/>
    /// <seealso cref="Repository.AuthRepository"/>
    public class UserEditViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        [Required]
        public string Guid { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        [EmailAddress]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the confirm password.
        /// </summary>
        /// <value>
        /// The confirm password.
        /// </value>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}