using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Provides a collection of models designed to make saving and updating
/// the quotations within the database easier.
/// </summary>
namespace AngularApp.API.Models.WebViewModels.QuotationSaveModels
{
    /// <summary>
    /// A class representing a full quote from the database
    /// </summary>
    /// <remarks>
    /// This class represnets a complete quote, including Calculations, details
    /// as well as the parent id and it's own id.
    /// This is used in cases when the full quote details are needed, in most cases
    /// only part of this model is needed and thus is further broken down elsewhere in
    /// the application.
    /// </remarks>
    /// <see cref="Controllers.QuoteController"/>
    /// <seealso cref="QueueDisplay"/>
    public class QuotationSaveWebViewModels
    {
        /// <summary>
        /// Gets or sets the quotation calculation.
        /// </summary>
        /// <value>
        /// The quotation calculation.
        /// </value>
        public JObject QuotationCalculation { get; set; }
        /// <summary>
        /// Gets or sets the quotation details.
        /// </summary>
        /// <value>
        /// The quotation details.
        /// </value>
        public JObject QuotationDetails { get; set; }
        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>
        /// The parent identifier.
        /// </value>
        public string ParentId { get; set; }
        /// <summary>
        /// Gets or sets the quote identifier.
        /// </summary>
        /// <value>
        /// The quote identifier.
        /// </value>
        public string QuoteId { get; set; }
    }

    /// <summary>
    /// A class used to showcase how the results of a quotation should be stored.
    /// </summary>
    /// <remarks>
    /// This class is used to respond with the quotation details when a request
    /// has been made for quotation results.
    /// </remarks>
    public class QuotationResultWebViewModel
    {
        /// <summary>
        /// Gets or sets the monthly repayable.
        /// </summary>
        /// <value>
        /// The monthly repayable.
        /// </value>
        [Required]
        public decimal MonthlyRepayable { get; set; }
        /// <summary>
        /// Gets or sets the total repayable.
        /// </summary>
        /// <value>
        /// The total repayable.
        /// </value>
        [Required]
        public decimal TotalRepayable { get; set; }
        /// <summary>
        /// Gets or sets the fees.
        /// </summary>
        /// <value>
        /// The fees.
        /// </value>
        [Required]
        public decimal Fees { get; set; }
        /// <summary>
        /// Gets or sets the interest rate.
        /// </summary>
        /// <value>
        /// The interest rate.
        /// </value>
        [Required]
        public double InterestRate { get; set; }
    }
}