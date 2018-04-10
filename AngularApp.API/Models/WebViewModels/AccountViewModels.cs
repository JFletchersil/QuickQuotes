using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace AngularApp.API.Models.WebViewModels.AccountViewModels
{
    /// <summary>
    /// Provides a collection of view models for managing user accounts within the system
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// View model for external login via 0Auth Service
    /// </summary>
    /// <remarks>
    /// This class is not used in this project, and can be safely ignored until needed.
    /// </remarks>
    public class ExternalLoginConfirmationViewModel
    {
        /// <summary>
        /// The email of the user who is logging in via the external login service
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    /// <summary>
    /// The URL that the user should be returned to when logging in via an external service
    /// </summary>
    /// <remarks>
    /// This class is not used in this project, and can be safely ignored until needed.
    /// </remarks>
    public class ExternalLoginListViewModel
    {
        /// <summary>
        /// The return URL the user should be returned to
        /// </summary>
        /// <value>
        /// The return URL.
        /// </value>
        public string ReturnUrl { get; set; }
    }

    /// <summary>
    /// The model that is used to evaluate the correctness of a provided third party code
    /// </summary>
    /// <remarks>
    /// This class is not used in this project, and can be safely ignored until needed.
    /// This class would mainly be used in contexts such as mobile authentication or the like, where
    /// a code would be provided.
    /// </remarks>
    public class VerifyCodeViewModel
    {
        /// <summary>
        /// The provider that needs to be checked for the correctness of the third party code
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        [Required]
        public string Provider { get; set; }
        /// <summary>
        /// The third party code to be authenticated
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        /// <summary>
        /// The return URL to which the user should be returned to
        /// </summary>
        /// <value>
        /// The return URL.
        /// </value>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// If the user should be remembered at this location or not
        /// </summary>
        /// <value>
        ///   <c>true</c> if [remember browser]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// I.e. Make it a valid authenticated location for access.
        /// </remarks>
        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }
        /// <summary>
        /// If the user should remain logged in at this location even if they close the browser or not
        /// </summary>
        /// <value>
        ///   <c>true</c> if [remember me]; otherwise, <c>false</c>.
        /// </value>
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// A model used to request a reset email to the authentication service
    /// </summary>
    /// <remarks>
    /// This class is not used in this project, and can be safely ignored until needed.
    /// </remarks>
    public class ForgotViewModel
    {
        /// <summary>
        /// The email of the user who needs a password reset
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        /// <remarks>
        /// Used both to confirm that the user has an account present, as well as a place to send the
        /// recovery email
        /// </remarks>
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    /// <summary>
    /// The model used to log a user in, including both a user name/email and password
    /// </summary>
    /// <remarks>
    /// This is actually used inside the project, and as such should not be altered.
    /// It is used inside the AccountController.
    /// </remarks>
    /// <seealso cref="Controllers.AccountController" />
    /// <seealso cref="Controllers.Auth0Controller" />
    public class LoginViewModel
    {
        /// <summary>
        /// The email that a user is logging in with
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// The password that the user is using to log into the account
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// If the user should remain logged in at the current browser/location
        /// </summary>
        /// <value>
        ///   <c>true</c> if [remember me]; otherwise, <c>false</c>.
        /// </value>
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// This is the model used to register new users into the current account system
    /// </summary>
    /// <remarks>
    /// In future, this would be replaced with the various Auth0 models, but until then, this remains
    /// the registration model.
    /// This model is used within the program, at Controllers.AccountController and Repository.AuthRepository
    /// </remarks>
    /// <seealso cref="Controllers.AccountController" />
    /// <seealso cref="Repository.AuthRepository" />
    /// <seealso cref="Controllers.Auth0Controller" />
    public class RegisterViewModel
    {
        /// <summary>
        /// The email with which the user will be using as both user name and email address for the account system
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// The starting password that the user will use to log into the system
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// A copy of the previous property, this is the password that the user will use to log into the system
        /// </summary>
        /// <value>
        /// The confirm password.
        /// </value>
        /// <remarks>
        /// This must match the Password property, otherwise it will be rejected out of hand, this is to
        /// prevent typos inside the password entry
        /// </remarks>
        /// <seealso cref="Password" />
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Sets the initial configuration for if the user is an Administration user
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is administrator; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// By default, the user is assumed to not be an Administration level user.
        /// </remarks>
        [Display(Name = "Is Administration")]
        public bool IsAdministrator { get; set; } = false;

        /// <summary>
        /// The user name that the user will have within the system, this is also used in conjunction with email
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        /// <seealso cref="Email" />
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string UserName { get; set; }
    }

    /// <summary>
    /// The model used when the user requests a password reset
    /// </summary>
    /// <remarks>
    /// This is not currently used anywhere within the project and can be safely ignored.
    /// </remarks>
    public class ResetPasswordViewModel
    {
        /// <summary>
        /// The email of the user having their password reset
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        /// <remarks>
        /// A reset email will be sent to the given email address, but only if the email address is valid
        /// within the database.
        /// </remarks>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// The new password the user has requested
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// A confirmation of the new password the user has requested
        /// </summary>
        /// <value>
        /// The confirm password.
        /// </value>
        /// <remarks>
        /// This must match the previous property.
        /// </remarks>
        /// <seealso cref="Password" />
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// The code that identifies if the user used other methods such as Two Factor Authentication to
        /// request the reset or not.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }
    }

    /// <summary>
    /// The model used when the user manually requests a password reset
    /// </summary>
    /// <remarks>
    /// This is not used in the project and can be safely ignored
    /// </remarks>
    public class ForgotPasswordViewModel
    {
        /// <summary>
        /// The email of the user having their password reset
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        /// <remarks>
        /// A reset email will be sent to the given email address, but only if the email address is valid
        /// within the database.
        /// </remarks>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    /// <summary>
    /// This is the search model used to search through the database, this used in the management of users
    /// in the administration screen.
    /// </summary>
    /// <remarks>
    /// This is used wholly to manage large collections of users by allowing a direct database search.
    /// </remarks>
    /// <seealso cref="Controllers.AccountController" />
    public class SearchAccounthWebViewModel
    {
        /// <summary>
        /// Gets or sets the filter term.
        /// </summary>
        /// <value>
        /// The filter term.
        /// </value>
        public string FilterTerm { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [return all].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [return all]; otherwise, <c>false</c>.
        /// </value>
        public bool ReturnAll { get; set; }
    }
}
