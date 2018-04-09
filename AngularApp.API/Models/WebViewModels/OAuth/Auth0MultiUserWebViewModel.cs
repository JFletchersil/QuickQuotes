using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuth
{
    /// <summary>
    /// Models a collection of multiple users in a form that allows the string value to be deserialised
    /// </summary>
    /// <remarks>
    /// To handle the issues provided with deserialising text data, I have had to encase the the 
    /// List<Auth0LightUserWebViewModel> inside another model to provide easy deserialisation. 
    /// This makes this class purely a wrapper class.
    /// </remarks>
    public class Auth0MultiUserWebViewModel
    {
        public List<Auth0LightUserWebViewModel> Users;
    }
}