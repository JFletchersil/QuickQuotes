using System.ComponentModel.DataAnnotations;

namespace AngularApp.API.Models.WebViewModels
{
    public class CarHirePurchaseWebViewModel
    {
        [Required]
        public int TermInMonths { get; set; }
        [Required]
        public decimal LoanAmount { get; set; }
        [Required]
        public decimal Deposit { get; set; }
    }
}