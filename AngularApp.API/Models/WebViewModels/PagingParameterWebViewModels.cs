using System.ComponentModel.DataAnnotations;

/// <summary>
/// Provides a collection of models to manage the pagination required within the application
/// </summary>
/// <remarks>
/// These pagination models are used in several different contexts, and are arranged here
/// due to the ease that comes with ensuring that all the paging models are together.
/// Pagination view models are used to manage the number of pages, as well
/// as what page to return, they are DISTINCT from other models
/// which RETURN the results of pagination. These models are more
/// thought of how you describe what kind of pagination you need.
/// </remarks>
namespace AngularApp.API.Models.WebViewModels.PagingModels
{
    /// <summary>
    /// The basic Paging view model, these are the basic items needed to
    /// manage pagination within the application
    /// </summary>
    /// <see cref="AccountPagingParameterWebViewModel" />
    /// <seealso cref="QueuePagingParameterWebViewModel" />
    /// <seealso cref="ConfigurationPagingParameterWebViewModel" />
    public class PagingParameterWebViewModel
    {
        /// <summary>
        /// The page number, once the items have been paginated, that you wish to recieve
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        /// <remarks>
        /// This is defaulted to 1, if no number is given.
        /// This is required.
        /// </remarks>
        [Required]
        public int PageNumber { get; set; } = 1;
        /// <summary>
        /// The number of items on each page when paginating the items within the table
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        /// <remarks>
        /// This is defaulted to 10 if no number is given.
        /// This is required.
        /// </remarks>
        [Required]
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// Provides the ordering by which the table being paginated should be ordered by
        /// </summary>
        /// <value>
        /// The order by.
        /// </value>
        /// <remarks>
        /// This is not required.
        /// This must also be implemented individually depending on if they are needed in other sections.
        /// </remarks>
        public string OrderBy { get; set; }
    }

    /// <summary>
    /// Provides an Account Controller specific paging view model
    /// </summary>
    /// <seealso cref="AngularApp.API.Models.WebViewModels.PagingModels.PagingParameterWebViewModel" />
    /// <remarks>
    /// This enhances the default view model by adding on an additional
    /// parameter which determines if all the items should be returned or not.
    /// This overrides the standard paging system.
    /// </remarks>
    /// <see cref="Controllers.AccountController" />
    public class AccountPagingParameterWebViewModel : PagingParameterWebViewModel
    {
        /// <summary>
        /// Determines if the return from a page request should ignore the pagination and return all results
        /// </summary>
        /// <value>
        ///   <c>true</c> if [return all]; otherwise, <c>false</c>.
        /// </value>
        [Required]
        public bool ReturnAll { get; set; }
    }

    /// <summary>
    /// Provides a Queue specific paging parameter view model for additional functionality
    /// </summary>
    /// <seealso cref="AngularApp.API.Models.WebViewModels.PagingModels.PagingParameterWebViewModel" />
    /// <remarks>
    /// In this case, there is no need for any enhancements to the model,
    /// however it exists so that if it needs specific pagination functionality
    /// that this functionality can be added.
    /// </remarks>
    /// <see cref="Controllers.QueueController" />
    public class QueuePagingParameterWebViewModel : PagingParameterWebViewModel
    {
    }

    /// <summary>
    /// Provides Configuration specific paging parameter functionality
    /// </summary>
    /// <seealso cref="AngularApp.API.Models.WebViewModels.PagingModels.PagingParameterWebViewModel" />
    /// <remarks>
    /// This enahances the standard model by giving it the ability to specify the ConfigurationType
    /// that is being operated on. This allows us to search only the relevant table, as well as
    /// preventing the mixing of table results
    /// </remarks>
    public class ConfigurationPagingParameterWebViewModel : PagingParameterWebViewModel
    {
        /// <summary>
        /// Specifies which specific configuration table is being paginated
        /// </summary>
        /// <value>
        /// The type of the configuration.
        /// </value>
        public string ConfigurationType { get; set; }
    }
}