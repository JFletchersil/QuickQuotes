using System;
using System.Collections.Generic;

namespace AngularApp.API.Models.WebViewModels.QueueDisplayModels
{
    /// <summary>
    /// This is a collection of models that are used to manage how the main quote queue screen is displayed
    /// </summary>
    /// <remarks>
    /// In this case, it is used solely in conjunction with the QueueController in order to
    /// manage how the Queue is displayed and functions relating to the queue
    /// </remarks>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal class NamespaceDoc
    {

    }

    /// <summary>
    /// Provides a model for responses to search requests
    /// </summary>
    /// <remarks>
    /// This models a simple amount of information to allow the front end 
    /// to redirect and quickly get the correct quote. It is used entirely in conjunction
    /// with the quick search for quotes bar.
    /// This quick search is displayed on the top right hand side of the graphical interface
    /// in the program.
    /// </remarks>
    /// <seealso cref="Controllers.QueueController"/>
    public class QuickSearchResult
    {
        /// <summary>
        /// Gets or sets the quote reference.
        /// </summary>
        /// <value>
        /// The quote reference.
        /// </value>
        public string QuoteReference { get; set; }
        /// <summary>
        /// Gets or sets the type of the quote.
        /// </summary>
        /// <value>
        /// The type of the quote.
        /// </value>
        public string QuoteType { get; set; }
        /// <summary>
        /// Gets or sets the quote author.
        /// </summary>
        /// <value>
        /// The quote author.
        /// </value>
        public string QuoteAuthor { get; set; }
    }

    /// <summary>
    /// A model used to make quick search requests of the API
    /// </summary>
    /// <remarks>
    /// This model is used to ask the API for a highly truncated return
    /// of possible matching quotes to be used as a hook on the main queue quote page.
    /// This hook then searches based off the quote reference, and displays this quote
    /// inside the quote table queue.
    /// </remarks>
    /// <seealso cref="Controllers.QueueController"/>
    /// <seealso cref="QuickSearchResult"/>
    public class QuickSearchRequest
    {
        /// <summary>
        /// Gets or sets the result number.
        /// </summary>
        /// <remarks>
        /// The result number is the maximum number of results that the search request wants back.
        /// If set to 10, and there are 11 results, it will return 10.
        /// If set to 10, and there are 9 results, it will return 9.
        /// </remarks>
        /// <value>
        /// The result number.
        /// </value>
        public int ResultNumber { get; set; }
        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        /// <value>
        /// The search text.
        /// </value>
        public string SearchText { get; set; }
    }

    /// <summary>
    /// A model representing a single row within the quote queue screen on the front end.
    /// </summary>
    /// <remarks>
    /// This represents a single row within the table that contains all quotes within
    /// the database. A collection of these is used to display all the results.
    /// </remarks>
    /// <seealso cref="Controllers.QueueController"/>
    /// <seealso cref="PaginatedQueueResult"/>
    public class QueueDisplayWebViewModel
    {
        /// <summary>
        /// Gets or sets the quote reference.
        /// </summary>
        /// <value>
        /// The quote reference.
        /// </value>
        public string QuoteReference { get; set; }
        /// <summary>
        /// Gets or sets the type of the quote.
        /// </summary>
        /// <value>
        /// The type of the quote.
        /// </value>
        public string QuoteType { get; set; }
        /// <summary>
        /// Gets or sets the quote status.
        /// </summary>
        /// <value>
        /// The quote status.
        /// </value>
        public string QuoteStatus { get; set; }
        /// <summary>
        /// Gets or sets the quote date.
        /// </summary>
        /// <value>
        /// The quote date.
        /// </value>
        public DateTime QuoteDate{ get; set; }
        /// <summary>
        /// Gets or sets the quote author.
        /// </summary>
        /// <value>
        /// The quote author.
        /// </value>
        public string QuoteAuthor { get; set; }
    }

    /// <summary>
    /// This class provides a representation of the result of performing pagination upon
    /// the quotes table within the database
    /// </summary>
    /// <remarks>
    /// This is used to hold the result of performing pagination onto the quotes within the 
    /// database
    /// </remarks>
    /// <seealso cref="Controllers.QueueController"/>
    public class PaginatedQueueResult
    {
        /// <summary>
        /// The queue display
        /// </summary>
        public IEnumerable<QueueDisplayWebViewModel> QueueDisplay;

        /// <summary>
        /// The total number of pages based off the quote result
        /// </summary>
        /// <value>
        /// The total number of pages.
        /// </value>
        public int TotalPages { get; set; }
        /// <summary>
        /// Gets or sets the total items.
        /// </summary>
        /// <value>
        /// The total number of quotes within the database.
        /// </value>
        public int TotalItems { get; set; }
    }
}