using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.DBModels
{
    public class UserDetails
    {
        public string Id
        {
            get; set;
        }

        public string FirstName
        {
            get;set;
        }

        public string LastName
        {
            get; set;
        }

        public string ShortName
        {
            get; set;
        }
    }
}