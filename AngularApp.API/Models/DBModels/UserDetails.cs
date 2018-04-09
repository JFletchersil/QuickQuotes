using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.DBModels
{
    /// <summary>
    /// Provides a data model representation of the details of a given user.
    /// More clearly, it is a collection of personal details of a user.
    /// </summary>
    /// <remarks>
    /// This is used in the user details section of the project, it contains meta data such as the
    /// assigned business/businesses, as well first name, last name and middle name.
    /// </remarks>
    public class UserDetails
    {
        /// <summary>
        /// The GUID of the user whose details are present
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The first name of the user
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The full and complete name of the user
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The business that the user works for
        /// </summary>
        public string Business { get; set; }

        /// <summary>
        /// The employment title of the user at the business they work for
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The customers for which the user is authorised to work for
        /// </summary>
        /// <remarks>
        /// This has not been implemented yet
        /// </remarks>
        public string BoundCustomers { get; set; }
    }
}