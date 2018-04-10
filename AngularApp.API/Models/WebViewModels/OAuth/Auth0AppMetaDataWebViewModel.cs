namespace AngularApp.API.Models.WebViewModels.OAuthModels
{
    /// <summary>
    /// Provides a collection of models used to work the Auth0 Implementation within the application
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Provides a representation of the Auth0AppMetaData
    /// </summary>
    /// <remarks>
    /// This is comprised of several different levels of models, 
    /// the first one represents a standard Auth0 Authorization model
    /// </remarks>
    /// <seealso cref="Auth0AuthorizationWebViewModel"/>
    public class Auth0AppMetaDataWebViewModel
    {
        /// <summary>
        /// Gets or sets the authorization model.
        /// </summary>
        /// <value>
        /// The authorization model.
        /// </value>
        public Auth0AuthorizationWebViewModel Authorization { get; set; }
    }
}