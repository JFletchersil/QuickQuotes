using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels;

namespace AngularApp.API.Controllers
{
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/Quote")]
    public class QuoteController : ApiController
    {
        private readonly Entities _dbContext = new Entities();


        [HttpGet]
        public string TestMethod()
        {
            return "Fuck the Sky!";
        }

        [HttpPost]
        public CarHirePurchaseWebQuotationResultViewModel CalculateQuote(CarHirePurchaseWebViewModel quoteGenerationModel)
        {
            // TODO - Make some call here that will ask the DB for the various Quote Details and Stuff
            var interestRate = 5d;
            var totalRepayable = quoteGenerationModel.LoanAmount - quoteGenerationModel.Deposit;
            totalRepayable = totalRepayable + (totalRepayable * ((decimal)interestRate / 100));


            return new CarHirePurchaseWebQuotationResultViewModel()
            {
                Fees = 50.00m,
                InterestRate = interestRate,
                MonthlyRepayable = (totalRepayable / quoteGenerationModel.TermInMonths),
                TotalRepayable = totalRepayable
            };
        }

        // TODO - We need to make a generic view model that can handle all the finance types.
        [HttpPost]
        public bool SaveQuote(QuotationSaveWebViewModels saveModel)
        {
            var first = _dbContext.ProductTypes.ToList().FirstOrDefault
                (x => x.ACProductType != null && string.Equals(x.ACProductType, saveModel.ParentId, StringComparison.CurrentCultureIgnoreCase));
            // Can Remain Defaulted, the first state of any quote should be QQActive anyway
            var quoteStatusId = _dbContext.QuoteStatuses.ToList().FirstOrDefault(x => x.State != null && x.State == "QQActive")?.StatusID;
            // TODO - We need to get the Quote Type as well.
            var quoteTypeId = _dbContext.QuoteTypes.ToList()
                .FirstOrDefault(x => x.ACQuoteType != null && string.Equals(x.ACQuoteType, saveModel.QuoteId, StringComparison.CurrentCultureIgnoreCase))?.TypeID;
            var productId = first.ProductTypeID;

            // TODO - We need to pull together the specific pathing for the various different types.

            var quoteGuid = Guid.NewGuid();
            _dbContext.Quotes.Add(new Quote()
            {
                QuoteReference = quoteGuid,
                ProductType = productId,
                QuoteAuthor = "TestAuthor",
                QuoteDate = DateTime.Now,
                QuoteStatus = quoteStatusId ?? 0,
                QuoteType = quoteTypeId ?? 0
            });
            _dbContext.HirePurchaseQuotes.Add(new HirePurchaseQuote()
            {
                Deposit = (double)saveModel.QuotationDetails.Deposit,
                LoanAmount = (double)saveModel.QuotationDetails.LoanAmount,
                TermInMonths = saveModel.QuotationDetails.TermInMonths,
                QuoteReference = quoteGuid
            });
            _dbContext.HirePurchaseQuoteResults.Add(new HirePurchaseQuoteResult()
            {
                Fees = (double)saveModel.QuotationCalculation.Fees,
                InterestRate = saveModel.QuotationCalculation.InterestRate,
                MonthlyAmount = (double)saveModel.QuotationCalculation.MonthlyRepayable,
                TotalRepayable = (double)saveModel.QuotationCalculation.TotalRepayable,
                QuoteReference = quoteGuid
            });
            _dbContext.SaveChanges();
            return true;
        }

        [HttpPost]
        public QuotationSaveWebViewModels RetrieveWithQuoteReference(HttpRequestMessage request)
        {

            var actGuid = new Guid(request.Content.ReadAsStringAsync().Result);
            var hirePurchaseQuoteResult = _dbContext.HirePurchaseQuoteResults.ToList()
                .FirstOrDefault(x => x.QuoteReference == actGuid);
            var hirePurchaseQuote =
                _dbContext.HirePurchaseQuotes.ToList().FirstOrDefault(x => x.QuoteReference == actGuid);
            var quote = _dbContext.Quotes.ToList()
                .FirstOrDefault(x => x.QuoteReference == actGuid);
            var returnResult = new QuotationSaveWebViewModels();

            if (hirePurchaseQuoteResult != null)
                returnResult.QuotationCalculation = new CarHirePurchaseWebQuotationResultViewModel
                {
                    Fees = (decimal) hirePurchaseQuoteResult.Fees,
                    InterestRate = hirePurchaseQuoteResult.InterestRate,
                    MonthlyRepayable = (decimal) hirePurchaseQuoteResult.MonthlyAmount,
                    TotalRepayable = (decimal) hirePurchaseQuoteResult.TotalRepayable
                };

            if (hirePurchaseQuote != null)
                returnResult.QuotationDetails = new CarHirePurchaseWebViewModel
                {
                    Deposit = (decimal)hirePurchaseQuote.Deposit,
                    LoanAmount = (decimal)hirePurchaseQuote.LoanAmount,
                    TermInMonths = hirePurchaseQuote.TermInMonths
                };

            if (quote == null) return returnResult;
            {
                returnResult.QuoteId = _dbContext.QuoteTypes.ToList().FirstOrDefault(x => x.TypeID == quote.QuoteType)
                    ?.ACQuoteType;
                returnResult.ParentId = _dbContext.ProductTypes.ToList().FirstOrDefault(x => x.ProductTypeID == quote.ProductType)
                    ?.ACProductType;
            }

            return returnResult;
        }
    }
}