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
    }
}
