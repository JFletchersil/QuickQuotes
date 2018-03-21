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
    public class QueueController : ApiController
    {
        private readonly Entities _dbContext = new Entities();
        Func<string, string, bool> containsString = (x, y) => x != null && x.ToUpper().Contains(y.ToUpper());

        [HttpPost]
        public PaginatedQueueResult ShowPaginatedQuotes(PagingParameterWebViewModel parameterWebView)
        {
            var quotes = _dbContext.Quotes.ToList();

            var totalPages = (int)Math.Ceiling(quotes.Count() / (double)parameterWebView.PageSize);
            var pagedQuotes = quotes.Skip((parameterWebView.PageNumber - 1) * parameterWebView.PageSize).Take(parameterWebView.PageSize).ToList();
            var returnItems = pagedQuotes.Select(x => new QueueDisplayWebViewModel()
            {
                QuoteReference = x.QuoteReference.ToString(),
                QuoteStatus = _dbContext.QuoteStatuses.ToList().FirstOrDefault(y => y.StatusID == x.QuoteStatus)?.State,
                QuoteAuthor = x.QuoteAuthor,
                QuoteDate = x.QuoteDate,
                QuoteType = _dbContext.QuoteTypes.ToList().FirstOrDefault(y => y.TypeID == x.QuoteType)?.IncQuoteType
            });
            return new PaginatedQueueResult()
            {
                QueueDisplay = returnItems,
                TotalPages = totalPages,
                TotalItems = quotes.Count
            };
        }

        [HttpPost]
        public IHttpActionResult SearchQueueSearchResults(QuickSearchRequest request)
        {
            var quotes = SearchQueryFiltering(request.SearchText);
            var returnItems = quotes.Select(x => new QueueDisplayWebViewModel()
            {
                QuoteReference = x.QuoteReference.ToString(),
                QuoteStatus = _dbContext.QuoteStatuses.ToList().FirstOrDefault(y => y.StatusID == x.QuoteStatus)?.State,
                QuoteAuthor = x.QuoteAuthor,
                QuoteDate = x.QuoteDate,
                QuoteType = _dbContext.QuoteTypes.ToList().FirstOrDefault(y => y.TypeID == x.QuoteType)?.IncQuoteType
            });
            return Ok(returnItems);
        }

        [HttpPost]
        public IHttpActionResult ReturnSearchResults(QuickSearchRequest request)
        {
            var quoteType = _dbContext.QuoteTypes.ToList();

            var query = SearchQueryFiltering(request.SearchText).Select(x => new QuickSearch
            {
                QuoteAuthor = x.QuoteAuthor,
                QuoteReference = x.QuoteReference.ToString(),
                QuoteType = quoteType.FirstOrDefault(y => y.TypeID == x.QuoteType && y.Enabled).IncQuoteType
            });

            var queryList = query.ToList().GetRange(0, request.ResultNumber - 1);
            queryList.Add(new QuickSearch
            {
                QuoteAuthor = "If you wish to see",
                QuoteType = "Additional quotes present",
                QuoteReference = "ExecuteOrder66"
            });
            return Ok(queryList);
        }

        private List<Quote> SearchQueryFiltering(string searchText)
        {
            var quoteStatus = _dbContext.QuoteStatuses.ToList();
            var productType = _dbContext.ProductTypes.ToList();
            var quoteType = _dbContext.QuoteTypes.ToList();

            var query = _dbContext.Quotes.ToList().Where(x => TestFunction(x.QuoteAuthor, searchText) ||
                                                     TestFunction(x.QuoteReference.ToString(), searchText) ||
                                                     TestFunction(x.QuoteAuthor, searchText) ||
                                                     TestFunction(quoteStatus.FirstOrDefault(y => y.StatusID == x.QuoteStatus && y.Enabled).State, searchText) ||
                                                     TestFunction(productType.FirstOrDefault(y => y.ProductTypeID == x.ProductType && y.Enabled).IncProductType, searchText) ||
                                                     TestFunction(quoteType.FirstOrDefault(y => y.TypeID == x.QuoteType && y.Enabled).IncQuoteType, searchText)).ToList();
            return query;
        }

        private bool TestFunction(string str1, string str2)
        {
            return str1 != null && str1.ToUpper().Contains(str2.ToUpper());
        }

    }
}
