using System.ComponentModel.DataAnnotations;

namespace AngularApp.API.Models.WebViewModels
{
    public class QuotationResultWebViewModel
    {
        [Required]
        public decimal MonthlyRepayable { get; set; }
        [Required]
        public decimal TotalRepayable { get; set; }
        [Required]
        public decimal Fees { get; set; }
        [Required]
        public double InterestRate { get; set; }
    }
}