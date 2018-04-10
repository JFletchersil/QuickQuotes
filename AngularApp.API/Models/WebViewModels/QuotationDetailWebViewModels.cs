using System.ComponentModel.DataAnnotations;

/// <summary>
/// Provides a collection of models designed to provide examples on how data should be structured
/// within the Quotation Detail table
/// </summary>
namespace AngularApp.API.Models.WebViewModels.ExampleModels
{
    /// <summary>
    /// An example class used to showcase how the details of a quotation should be stored.
    /// </summary>
    /// <remarks>
    /// This class has since been superceded by the need for complete generification, as such
    /// it is no longer actively used.
    /// This should be considered an example class, used to help visualise how the generic
    /// objects would look.
    /// </remarks>
    public class PersonalLoanWebViewModel
    {
        /// <summary>
        /// Gets or sets the term in months.
        /// </summary>
        /// <value>
        /// The term in months.
        /// </value>
        [Required]
        public int TermInMonths { get; set; }
        /// <summary>
        /// Gets or sets the loan amount.
        /// </summary>
        /// <value>
        /// The loan amount.
        /// </value>
        [Required]
        public decimal LoanAmount { get; set; }
        /// <summary>
        /// Gets or sets the deposit.
        /// </summary>
        /// <value>
        /// The deposit.
        /// </value>
        [Required]
        public decimal Deposit { get; set; }
    }
}