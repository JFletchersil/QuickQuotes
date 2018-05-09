using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AngularApp.API.Models.WebViewModels.ConfigurationViewModels
{
    /// <summary>
    /// Provides a collection of models to manage the configuration of the web application
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// An abstract class establishing that other classes are related to this class
    /// </summary>
    /// <remarks>
    /// This class remains empty due to the fact that it is used solely to group the following
    /// classes together inside the same overall class structure.
    /// </remarks>
    /// <seealso cref="QuoteTypesViewModel" />
    /// <seealso cref="QuoteStatusesViewModel" />
    /// <seealso cref="QuoteDefaultsViewModel" />
    /// <seealso cref="ProductTypesViewModel" />
    public abstract class ConfigurationWebViewModels
    {
    }

    /// <summary>
    /// A data class establishing the structure of the QuoteTypes configuration table
    /// within the database
    /// </summary>
    /// <seealso cref="AngularApp.API.Models.WebViewModels.ConfigurationViewModels.ConfigurationWebViewModels" />
    /// <remarks>
    /// This is used to manage QuoteTypes within the application, it is the main focus
    /// of adding new QuoteTypes as well as deleting them from the database.
    /// It is also used in the start up to allow for mapping via AutoMap.
    /// </remarks>
    /// <seealso cref="Controllers.ConfigurationController" />
    /// <seealso cref="Startup" />
    /// <seealso cref="AutoMapper" />
    public class QuoteTypesViewModel : ConfigurationWebViewModels
    {
        /// <summary>
        /// The unique ID, not guid, that represents the type of quote
        /// </summary>
        /// <value>
        /// The type identifier.
        /// </value>
        public int TypeID { get; set; }
        /// <summary>
        /// A non unique quote type string identifier
        /// </summary>
        /// <value>
        /// The type of the quote.
        /// </value>
        /// <remarks>
        /// For users to have an easy readable way to know which quote type
        /// is what quote type.
        /// </remarks>
        public string QuoteType { get; set; }
        /// <summary>
        /// The product ID of which this quote type is a child
        /// </summary>
        /// <value>
        /// The product parent identifier.
        /// </value>
        public int ProductParentID { get; set; }
        /// <summary>
        /// Determines if the quote type is enabled or not
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// A data class providing a representation of the QuoteStatuses
    /// </summary>
    /// <seealso cref="AngularApp.API.Models.WebViewModels.ConfigurationViewModels.ConfigurationWebViewModels" />
    /// <remarks>
    /// This provides a model for the QuoteStatues configuration table to allow
    /// easy edits, deletions and additions to the QuoteStatuses configuration table.
    /// </remarks>
    /// <seealso cref="Controllers.ConfigurationController" />
    /// <seealso cref="Startup" />
    public class QuoteStatusesViewModel : ConfigurationWebViewModels
    {
        /// <summary>
        /// The unique ID, not guid, that represents the type of quote status
        /// </summary>
        /// <value>
        /// The status identifier.
        /// </value>
        public int StatusID { get; set; }
        /// <summary>
        /// The string representation of the quote state
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        /// <remarks>
        /// This is used to determine how old a quote is
        /// </remarks>
        public string State { get; set; }
        /// <summary>
        /// Represents if a quote status is in use or not
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This is used to determine if a quote status should be ignored by the application
        /// </remarks>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// A data class providing a representation of a single row of the QuoteDefaults database table
    /// </summary>
    /// <seealso cref="AngularApp.API.Models.WebViewModels.ConfigurationViewModels.ConfigurationWebViewModels" />
    /// <remarks>
    /// This provides an easy representation of the Quote Defaults table, allowing for
    /// additions, deletions and edits to be made using the class.
    /// Strips data from the database row if it is not needed for operations elsewhere
    /// </remarks>
    /// <seealso cref="Controllers.ConfigurationController" />
    /// <seealso cref="Startup" />
    public class QuoteDefaultsViewModel : ConfigurationWebViewModels
    {
        /// <summary>
        /// The unique ID, not guid, that represents the quote default
        /// </summary>
        /// <value>
        /// The type identifier.
        /// </value>
        public int TypeID { get; set; }
        /// <summary>
        /// The description of the elements needed for the quote default
        /// </summary>
        /// <value>
        /// The element description.
        /// </value>
        /// <remarks>
        /// This should be formatted as if it was a Json String so it can be used
        /// to create the elements needed for the quote to be captured properly.
        /// WARNING - There are only minor protections for this, it will check if
        /// the Json is valid on the FRONT END but will not do so on the BACK END,
        /// if you use this you must validate the Json as well.
        /// </remarks>
        public string ElementDescription { get; set; }
        /// <summary>
        /// The XML template, to be populated when performing quote calculations
        /// </summary>
        /// <value>
        /// The XML template.
        /// </value>
        /// <remarks>
        /// This would only be used in cases where we would be using an external quotation
        /// calculator that would expect an XML Template response. As such, it may
        /// not be needed if no external quotation calculator is used.
        /// </remarks>
        public string XMLTemplate { get; set; }
        /// <summary>
        /// The template used to calculate the Total Repayable Amount based off quotation results
        /// </summary>
        /// <value>
        /// The total repayable template.
        /// </value>
        public string TotalRepayableTemplate { get; set; }
        /// <summary>
        /// The template used to calculate the Total Repayable Amount based off quotation results
        /// </summary>
        /// <value>
        /// The monthly repayable template.
        /// </value>
        public string MonthlyRepayableTemplate { get; set; }
        /// <summary>
        /// Represents if a quote type and default configuration is in use or not
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This is used to determine if a quote default should be ignored by the application
        /// </remarks>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// A data class providing a representation of a single row within the ProductTypes database table
    /// </summary>
    /// <seealso cref="AngularApp.API.Models.WebViewModels.ConfigurationViewModels.ConfigurationWebViewModels" />
    /// <remarks>
    /// This provides an easy representation of the ProductTypes table, allowing for
    /// additions, deletions and edits to be made using the class.
    /// Strips data from the database row if it is not needed for operations elsewhere
    /// </remarks>
    /// <seealso cref="Controllers.ConfigurationController" />
    /// <seealso cref="Startup" />
    public class ProductTypesViewModel : ConfigurationWebViewModels
    {
        /// <summary>
        /// The unique ID, not guid, that represents the quote default
        /// </summary>
        /// <value>
        /// The product type identifier.
        /// </value>
        public int ProductTypeID { get; set; }
        /// <summary>
        /// A string representation of the Product Type
        /// </summary>
        /// <value>
        /// The type of the product.
        /// </value>
        /// <remarks>
        /// Used to help users identity which product type is being used or worked on at any one time.
        /// </remarks>
        public string ProductType { get; set; }
        /// <summary>
        /// Represents if a product type is in use or not
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This is used to determine if a product type should be ignored by the application
        /// </remarks>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// A paginated summation of configuration options containing a number
    /// of configuration options, the total number of items and pages
    /// within the database
    /// </summary>
    /// <remarks>
    /// This is on a per configuration basis, there is no mixing of Configuration Results
    /// from other configuration types. Do not expect to see QuoteDefaults and ProductTypes
    /// in the same list even if technically possible.
    /// </remarks>
    /// <seealso cref="Controllers.ConfigurationController" />
    public class PaginatedConfigResult
    {
        /// <summary>
        /// A collection of configuration results that has been paginated
        /// </summary>
        public IEnumerable<JObject> ConfigResult;
        /// <summary>
        /// The total number of pages based on the limit per page set else where
        /// </summary>
        public int TotalPages;
        /// <summary>
        /// The total number of configuration items of the same configuration type
        /// within the database
        /// </summary>
        /// <value>
        /// The total items.
        /// </value>
        public int TotalItems { get; set; }
    }

    /// <summary>
    /// A model representing a collection of the same configuration type to be saved
    /// </summary>
    /// <remarks>
    /// As before, this will only work if you use the same collection type for all
    /// the items being saved.
    /// It will not work if you mix configuration types within the collection,
    /// even if there's no particular restriction that prevents you from adding
    /// multiple configurations inside the same save object.
    /// </remarks>
    /// <seealso cref="Controllers.ConfigurationController" />
    public class SaveConfigurationViewModel
    {
        /// <summary>
        /// The configuration type of the List of Objects to be saved.
        /// </summary>
        /// <value>
        /// The type of the configuration.
        /// </value>
        public string ConfigType { get; set; }
        /// <summary>
        /// The list of configuration types to be saved
        /// </summary>
        /// <value>
        /// The configuration to be saved.
        /// </value>
        /// <remarks>
        /// To make the view model generic, these are saved as JObjects,
        /// rather than being more model specific.
        /// </remarks>
        public List<JObject> ConfigsToBeSaved { get; set; }
    }

    /// <summary>
    /// The model used to capture the search text for performing searches against
    /// configuration types within the database.
    /// </summary>
    /// <remarks>
    /// Effectively, this is used to dictate what to search for, and where
    /// within the Configuration Controller. It will then respond with
    /// appropriate models later on.
    /// </remarks>
    /// <seealso cref="Controllers.ConfigurationController" />
    public class DefaultConfigurationSearchWebViewModel
    {
        /// <summary>
        /// The configuration type within the database, displayed in text format,
        /// which should be searched
        /// </summary>
        /// <value>
        /// The type of the configuration.
        /// </value>
        public string ConfigType { get; set; }

        /// <summary>
        /// The filter text.
        /// </summary>
        /// <value>
        /// The filter text.
        /// </value>
        public string FilterText { get; set; }
    }
}