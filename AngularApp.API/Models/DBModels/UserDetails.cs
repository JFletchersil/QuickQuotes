using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.DBModels
{
    public class UserDetails
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string Business { get; set; }

        public string Title { get; set; }

        public string BoundCustomers { get; set; }
    }
}