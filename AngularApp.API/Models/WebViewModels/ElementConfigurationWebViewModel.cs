using AngularApp.API.Models.WebViewModels.ConfigurationViewModels;

namespace AngularApp.API.Models.WebViewModels.ElementConfigurationModels
{
    /// <summary>
    /// Provides data models designed to configure single HTML elements
    /// </summary>
    /// <remarks>
    /// This model collection is designed to model HTML elements to allow for
    /// a single generic quotation page to operate. Due to the fact that the configuration
    /// is driven from the database, this allows the user of the application to configure the quote
    /// type.
    /// </remarks>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal class NamespaceDoc
    {

    }

    /// <summary>
    /// Provides a class representation of the configuration of a single HTML element of a single quote type
    /// </summary>
    /// <remarks>
    /// This models a single HTML element, as well as it's validations and configurations
    /// for usage on the front end.
    /// This effectively allows the database to hold how the quote type
    /// should display fields to capture the data required to calculate the quote. The
    /// consequence of this is that we don't need to add a new front end page for
    /// each new quote type, instead we can describe how the page will look and the
    /// the angular front end will make provide the elements.
    /// </remarks>
    /// <seealso cref="Controllers.QuoteController" />
    /// <seealso cref="QuoteDefaultsViewModel" />
    /// <seealso cref="ElementDescription" />
    public class ElementConfigurationWebViewModel
    {
        /// <summary>
        /// The id of the element, this is used by the form to provide validation
        /// as well as a few other bits
        /// </summary>
        /// <value>
        /// The name of the element.
        /// </value>
        public string ElementName { get; set; }
        /// <summary>
        /// The name used when the validation fails
        /// </summary>
        /// <value>
        /// The warning label.
        /// </value>
        /// <example>
        /// If Warning Label = Wonky Work
        /// and ElementName = Work Work
        /// and the error is that you've exceeded the bounds of the min max
        /// then the error message would display as
        /// Error Message = Wonky Work has exceeded it's Maximum of 5000
        /// </example>
        public string WarningLabel { get; set; }
        /// <summary>
        /// What the text for the label of the element should be set as
        /// </summary>
        /// <value>
        /// The name of the label.
        /// </value>
        public string LabelName { get; set; }
        /// <summary>
        /// The description of the element's type, as well as it's validations
        /// </summary>
        /// <value>
        /// The element description.
        /// </value>
        /// <remarks>
        /// This effectively describes how the element should be validated and displayed,
        /// I.e. as either a text element with Required, or a currency element that
        /// is not required but needs to be within the min and max.
        /// </remarks>
        public ElementDescription ElementDescription { get; set; }
    }

    /// <summary>
    /// Describes how a HTML element should be configured for a given single HTML element
    /// </summary>
    /// <remarks>
    /// This is used solely in conjunction with ElementConfigurationWebViewModel in order
    /// to completely describe the behaviour of a single HTML element that is provided
    /// to the front end in order to capture all the information needed to calculate
    /// a quote.
    /// </remarks>
    /// <seealso cref="ElementConfigurationWebViewModel" />
    /// <seealso cref="Controllers.QuoteController" />
    public class ElementDescription
    {
        /// <summary>
        /// The input type of the HTML input element
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        /// <example>
        /// Includes type such as text, number and input
        /// </example>
        public string Type { get; set; }
        /// <summary>
        /// Denotes if the element is a currency element or not
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is currency; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Due to how numbers work, and the need to display the local currency,
        /// if an element is a currency element, then TYPE must be set to "text"
        /// as quoted.
        /// </remarks>
        /// <seealso cref="Type" />
        public bool IsCurrency { get; set; }
        /// <summary>
        /// Denotes if an element is required to filled in or not before submitting
        /// the information for a quotation
        /// </summary>
        /// <value>
        ///   <c>true</c> if required; otherwise, <c>false</c>.
        /// </value>
        public bool Required { get; set; }
        /// <summary>
        /// Sets the Max number can not exceed on the front end
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public decimal Max { get; set; }
        /// <summary>
        /// Sets the Min number can not exceed on the front end
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public decimal Min { get; set; }
    }
}