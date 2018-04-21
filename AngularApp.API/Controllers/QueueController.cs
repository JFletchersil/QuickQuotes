using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using AngularApp.API.Helpers;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels.PagingModels;
using AngularApp.API.Models.WebViewModels.QueueDisplayModels;

namespace AngularApp.API.Controllers
{
    /// <summary>
    /// Manages aspects related to the Quotes and how they work with the Queues
    /// </summary>
    /// <remarks>
    /// This focuses primarily on the Quotes Queue, as functionality such as Quote searching across a
    /// general area is tied into the Queue this functionality is also present.
    /// </remarks>
    /// <seealso cref="System.Web.Http.ApiController" />
    [EnableCors("*", "*", "*")]
    public class QueueController : ApiController
    {
        /// <summary>
        /// Creates a private, read only connection to the database for gathering the details required for the Quote Queue
        /// </summary>
        private readonly Entities _dbContext;

        private readonly CommonFunctionsHelper _helper;

        /// <summary>
        /// Initializes the QueueController Controller with a entity context, allowing access to the Quote Database.
        /// Constructs the default details for the controller on start up.
        /// </summary>
        public QueueController()
        {
            _dbContext = new Entities();
            _helper = new CommonFunctionsHelper();
        }

        /// <summary>
        /// Provides an initialisation for the Entities, for UnitTests to give the Queue Controller an Entities object
        /// </summary>
        /// <param name="entities">A unit tested Entities</param>
        public QueueController(Entities entities)
        {
            _dbContext = entities;
            _helper = new CommonFunctionsHelper();
        }

        /// <summary>
        /// Returns quotes in a paginated format, ordered according how the active column was ordered
        /// </summary>
        /// <param name="parameterWebView">The model containing the PageNumber, Size and Ordering for the paginated results</param>
        /// <returns>
        /// The paginated results to be returned from the database
        /// </returns>
        /// <remarks>
        /// Differently from how some of the others work, this only works with returning quotes.
        /// This is a rather standard paginated system, it used via Post.
        /// OrderBy is not required, however it is advised to be present.
        /// </remarks>
        [HttpPost]
        public PaginatedQueueResult ShowPaginatedQuotes(QueuePagingParameterWebViewModel parameterWebView)
        {
            try
            {
                var quotes = _dbContext.Quotes.ToList();
                // Sorts the list in a centralised location
                quotes = _helper.ReturnSortedList(quotes, parameterWebView.OrderBy);

                // Calculates the total number of pages in the database, based off the number of items and the page size
                var pagingResults = _helper.PaginateDbTables(quotes, parameterWebView.PageNumber, parameterWebView.PageSize);

                // Processes the results from the database view model to the web view model
                // As per other instances of this, this is important to remove references and data that is not needed
                // on the front end
                var returnItems = pagingResults.Items.Select(x => new QueueDisplayWebViewModel()
                {
                    QuoteReference = x.QuoteReference.ToString(),
                    QuoteStatus = _dbContext.QuoteStatuses.ToList().FirstOrDefault(y => y.StatusID == x.QuoteStatus)
                        ?.State,
                    QuoteAuthor = x.QuoteAuthor,
                    QuoteDate = x.QuoteDate,
                    QuoteType = _dbContext.QuoteTypes.ToList().FirstOrDefault(y => y.QuoteTypeID == x.QuoteType)
                        ?.IncQuoteType
                });

                // Returns the values for usage on the front end
                return new PaginatedQueueResult()
                {
                    QueueDisplay = returnItems,
                    TotalPages = pagingResults.TotalPages,
                    TotalItems = quotes.Count
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Performs a database search for matching results to the filter result given by the user
        /// </summary>
        /// <param name="request">The filter/search text that is being searched for in the quotation table</param>
        /// <returns>
        /// A list of all potential matches to the filter/search text from the quotation table
        /// </returns>
        /// <remarks>
        /// This is normally only going to be used in cases where we do not have the value already present within
        /// the front end. This works only in the context of the front end queue screen, rather than the hot search bar
        /// in the top right of the user interface.
        /// </remarks>
        [HttpPost]
        public IHttpActionResult SearchQueueSearchResults(HttpRequestMessage request)
        {
            try
            {
                var dataText = request.Content.ReadAsStringAsync().Result;
                var quotes = SearchQueryFiltering(dataText);
                if (!quotes.Any()) return Ok(new List<QueueDisplayWebViewModel>());
                var returnItems = quotes.Select(x => new QueueDisplayWebViewModel()
                {
                    QuoteReference = x.QuoteReference.ToString(),
                    QuoteStatus = _dbContext.QuoteStatuses.ToList().FirstOrDefault(y => y.StatusID == x.QuoteStatus)?.State,
                    QuoteAuthor = x.QuoteAuthor,
                    QuoteDate = x.QuoteDate,
                    QuoteType = _dbContext.QuoteTypes.ToList().FirstOrDefault(y => y.QuoteTypeID == x.QuoteType)?.IncQuoteType
                });
                return Ok(returnItems.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        /// <summary>
        /// Returns a list of truncated search results that are used to quickly find the correct search result
        /// </summary>
        /// <param name="request">Contains the maximum number of search results to return, as well as the search text that is being searched for</param>
        /// <returns>
        /// A list of search results which are heavily redacted, along with a potential final search result prompting the user back to the main quote queue page
        /// </returns>
        /// <remarks>
        /// This is to be used only within the context of the hot bar in the top right of the user interface.
        /// It is designed to provide only the quote type as well as the author and reference to allow the main search function,
        /// SearchQueueSearchResults.
        /// At the end of each return from this function, it is expected that a dummy object which redirects the user to the
        /// main quote menu will be added.
        /// </remarks>
        [HttpPost]
        public IHttpActionResult ReturnSearchResults(QuickSearchRequest request)
        {
            try
            {
                var quoteType = _dbContext.QuoteTypes.ToList();

                var query = SearchQueryFiltering(request.SearchText).Select(x => new QuickSearchResult
                {
                    QuoteAuthor = x.QuoteAuthor,
                    QuoteReference = x.QuoteReference.ToString(),
                    QuoteType = quoteType.FirstOrDefault(y => y.QuoteTypeID == x.QuoteType && y.Enabled).IncQuoteType
                });

                // This places the very last item, no matter what case, as this dummy item is used by the 
                // front end to force users to the main quote menu if the result they want is not present
                var queryList = new List<QuickSearchResult>();
                if (query.Any())
                {
                    queryList = query.ToList().GetRange(0, request.ResultNumber - 1);
                }
                queryList.Add(new QuickSearchResult
                {
                    QuoteAuthor = "If you wish to see",
                    QuoteType = "Additional quotes present",
                    QuoteReference = "ExecuteOrder66"
                });
                return Ok(queryList);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        /// <summary>
        /// Returns a list of possible quotes that match a given search text
        /// </summary>
        /// <param name="searchText">The search text/filter text that is the user is searching for</param>
        /// <returns>
        /// A list of matching quotes that one or more of their columns/properties matches the search text
        /// </returns>
        /// <remarks>
        /// This follows a similar pattern to the ones present in the account and configuration setup.
        /// In this instance, other details are pulled in, in order to give textual representations of the quote and product types,
        /// rather than an ID.
        /// </remarks>
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
                                                     TestFunction(quoteType.FirstOrDefault(y => y.QuoteTypeID == x.QuoteType && y.Enabled).IncQuoteType, searchText)).ToList();
            return query;
        }

        /// <summary>
        /// Compares two strings, and checks if the value of str1 contains a string or sub string of str2
        /// </summary>
        /// <param name="str1">The value which is being searched, normally a database model</param>
        /// <param name="str2">The search or filter text, which the user is searching for, generally a string.</param>
        /// <returns>
        /// A true or false value which signals if the str2 value is present within str1
        /// </returns>
        private bool TestFunction(string str1, string str2)
        {
            // Checks if the string is null, prior to doing a search, then forces both strings to upper for 
            // true comparison.
            return str1 != null && str1.ToUpper().Contains(str2.ToUpper());
        }
    }
}
