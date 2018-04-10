using System.Collections.Generic;

/// <summary>
/// A collection of models representing the User Queue, this is the queue that 
/// forms the focus of the administration screen.
/// </summary>
namespace AngularApp.API.Models.WebViewModels.UserQueueDisplayModels
{
    /// <summary>
    /// A class representation of a single user row from within the identification
    /// database
    /// </summary>
    /// <remarks>
    /// This class represents a single user within the database, it stores all of their
    /// relevant data so that the application can adjust the user as needed.
    /// </remarks>
    /// <see cref="Controllers.AccountController" />
    /// <seealso cref="PaginatedQueueUserResult"/>
    public class QueueUserWebViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public string Guid { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>
        /// The email address.
        /// </value>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [email confirmed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [email confirmed]; otherwise, <c>false</c>.
        /// </value>
        public bool EmailConfirmed { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is admin.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is admin; otherwise, <c>false</c>.
        /// </value>
        public bool IsAdmin { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [account locked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [account locked]; otherwise, <c>false</c>.
        /// </value>
        public bool AccountLocked { get; set; }
    }

    /// <summary>
    /// This class provides a representation of the result of performing pagination upon
    /// the user table within the database    
    /// </summary>
    /// <remarks>
    /// This holds all of the relevant information needed to fully manage the 
    /// pagination of the contents of the user table within the database
    /// </remarks>
    /// <see cref="Controllers.AccountController"/>
    public class PaginatedQueueUserResult
    {
        /// <summary>
        /// A collection of users
        /// </summary>
        /// <value>
        /// A collection of users.
        /// </value>
        public IEnumerable<QueueUserWebViewModel> QueueDisplay { get; set; }

        /// <summary>
        /// The total pages
        /// </summary>
        /// <value>
        /// The total pages based on the pagination performed by the controller.
        /// </value>
        public int TotalPages { get; set; }
        /// <summary>
        /// Gets or sets the total items.
        /// </summary>
        /// <value>
        /// The total number of users within the database.
        /// </value>
        public int TotalItems { get; set; }
    }
}