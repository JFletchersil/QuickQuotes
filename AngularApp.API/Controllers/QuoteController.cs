using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AngularApp.API.Controllers
{
    /// <summary>
    /// Manages all aspects involving the configuration, presentation and creation of quotes
    /// </summary>
    /// <remarks>
    /// This is the most complicated controller inside the application, treat it with caution.
    /// No aspect of this controller will delete quotes, that is handled entirely by the database via a midnight job.
    /// </remarks>
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/Quote")]
    public class QuoteController : ApiController
    {
        /// <summary>
        /// Creates a private, readonly connection to the database for gathering the details required for the management
        /// of quotes themselves.
        /// </summary>
        private readonly Entities _dbContext = new Entities();

        /// <summary>
        /// An ENUM representation of the table type that is being worked with
        /// </summary>
        /// <remarks>
        /// This is used later to determine which table needs to be worked with in other parts of the controller.
        /// </remarks>
        private enum TableType
        {
            Quotes,
            Results
        }

        /// <summary>
        /// Returns the page configuration for a given quote type
        /// </summary>
        /// <param name="quoteType">The quote type that requires it's configuration details</param>
        /// <returns>A list of elements to be displayed on the page as well as their validation information</returns>
        /// <remarks>
        /// This is used to generically configure the quote page, based off JSON data, so as to avoid needing mutliple
        /// different pages in order to cover multiple different quote types.
        /// Returned is an item for each element that must be on the page, as well as the associated form validation data.
        /// </remarks>
        [HttpGet]
        public List<ElementConfigurationWebViewModel> GetElementConfiguration(string quoteType)
        {
            var configuration = _dbContext.QuoteDefaults.FirstOrDefault(x =>
                x.TypeID == _dbContext.QuoteTypes.FirstOrDefault(y => y.IncQuoteType == quoteType).QuoteTypeID);

            return configuration != null ?
                JsonConvert.DeserializeObject<List<ElementConfigurationWebViewModel>>(configuration.ElementDescription) : null;
        }

        /// <summary>
        /// Calculates the quote details based on a given Json object converted into a string
        /// </summary>
        /// <param name="request">A Json Object with element data used to calculate the quote results</param>
        /// <returns>A quote result, containing information about fees, repayables and interest</returns>
        /// <remarks>
        /// This function is highly specialised, it is designed to use configuration data from the database
        /// in order to calculate the values to be returned to the user.
        /// Alternately, rather than using a given calcuation formula stored within the database, 
        /// it can use an external calculator in order to get the result.
        /// Additionally, should a commission calculator be given, it can also use that to calculate basic comission
        /// structures.
        /// If the database calculation is used, then it takes it in a string format and converts it into a mathmatical
        /// formula after inserting/replacing the values in the string formula with the values inside the request object.
        /// If this database calculation is used, then you must ensure that the names for the elements across all
        /// parts of the quote configuration match, otherwise inconsistent or wrong results will be returned.
        /// </remarks>
        [HttpPost]
        public async Task<QuotationResultWebViewModel> CalculateQuoteAsync(HttpRequestMessage request)
        {
            // Breaks down and converts the request into a dict as well as JObject
            // The JObject is used to assertain the type, in order to get the defaults
            // The Dict is used to perform the calculations later on
            var dataText = request.Content.ReadAsStringAsync().Result;
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataText);
            var objType = JsonConvert.DeserializeObject<JObject>(dataText).GetValue("Type").Value<string>();
            var quoteDefaults = _dbContext.QuoteDefaults.FirstOrDefault(x => x.QuoteTypeID.IncQuoteType == objType);
            // Placeholder for storing the interest rates, this will be adjusted later
            var interestRate = 5d;
            dict.Add("InterestRate", "5");

            // Performs a check to see if there is an active Commission Calculator, if there is then
            // a request is made to that endpoint and the data is processed into the general data dict
            // for the calculation
            if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["CommissionCalculatorCall"]))
            {
                using (HttpClient client = new HttpClient())
                {
                    // TODO - Requires more information about how the ComissionCalculator Works, as well as conversion methods.
                    var httpRequest = new HttpRequestMessage(HttpMethod.Post, WebConfigurationManager.AppSettings["CommissionCalculatorCall"]);
                    var returnRequest = await client.SendAsync(httpRequest);
                    var returnValue = returnRequest.Content.ReadAsStringAsync().Result;
                    dict.Add("ComissionValue", "500");
                }
            }

            // Performs a check to see if a Calculator Endpoint exists, if it does exist then
            // the dict is handed to the endpoint and the result is immediately returned to the user
            if (!string.IsNullOrEmpty(WebConfigurationManager.AppSettings["CalculatorAPICall"]))
            {
                using (HttpClient client = new HttpClient())
                {
                    // TODO - Requires more information about how the Calculator Works, as well as conversion methods, but basically done except for transformation function
                    var httpRequest = new HttpRequestMessage(HttpMethod.Post, WebConfigurationManager.AppSettings["CalculatorAPICall"]);
                    return Mapper.Map<QuotationResultWebViewModel>(await client.SendAsync(httpRequest));
                }
            }

            // If the calculator endpoint is not used, then we are expected to us the database template to
            // calculate the Total Repayable, the values within the template that need to be replaced
            // are replaced within the function below
            var retVal = PopularDefaultsWithString(dict, quoteDefaults.TotalRepayableTemplate);
            // Uses the datatable functionality to compute strings as if they were mathmatical formulas to get
            // a value for the total repayable. From this, we can calculate the other parameters
            var totalRepayable = Convert.ToDecimal(new DataTable().Compute(retVal, null));

            // Returns the calculated quotation result based on inferences from other values such as
            // the totalRepyable and the given term in months
            return new QuotationResultWebViewModel()
            {
                Fees = 50.00m,
                InterestRate = interestRate,
                MonthlyRepayable = Math.Round((totalRepayable / Convert.ToDecimal(dict["TermInMonths"])), 2),
                TotalRepayable = Math.Round(totalRepayable, 2)
            };
        }

        /// <summary>
        /// Saves the quote to the main quote table, as well as the specifc quote tables
        /// </summary>
        /// <param name="saveModel">Contains the quote details as well as calulation, parent product id and it's own id</param>
        /// <returns>A 200 or 500 server response depending on if the action was successful or not</returns>
        /// <remarks>
        /// This function is responsible for saving quotes once they have been generated by the previous function
        /// This function operates on the database in three distinct steps, it can enter a state in which a 
        /// quote can only be partially saved.
        /// This function generates the database inserts manually, this is due to the generic nature of the scripting
        /// process as well as the lack of knowledge about what parameters can be expected.
        /// Due to this flexibility, this function is a prime area for analysis when it comes to determining 
        /// if something is wrong within the saving process.
        /// </remarks>
        [HttpPost]
        public IHttpActionResult SaveQuote(QuotationSaveWebViewModels saveModel)
        {
            // Gets the product ID for a given parent string name, due to the fact that these are strings
            // and not the expected IDs, this needs to happen.
            var first = _dbContext.ProductTypes.ToList().FirstOrDefault
                (x => x.IncProductType != null && string.Equals(x.IncProductType, saveModel.ParentId, StringComparison.CurrentCultureIgnoreCase));
            var productId = first.ProductTypeID;
            // Can Remain Defaulted, the first state of any quote should be QQActive anyway
            var quoteStatusId = _dbContext.QuoteStatuses.ToList().FirstOrDefault(x => x.State != null && x.State == "QQActive")?.StatusID;
            // Gets the quote type id for a given quote type string,  due to the fact that these are strings
            // and not the expected IDs, this needs to happen.
            var quoteType = _dbContext.QuoteTypes.ToList()
                .FirstOrDefault(x => x.IncQuoteType != null && string.Equals(x.IncQuoteType, saveModel.QuoteId,
                                         StringComparison.CurrentCultureIgnoreCase));
            var quoteTypeId = quoteType?.QuoteTypeID;

            // Generates a new Quote GUID for the quote
            var quoteGuid = Guid.NewGuid();

            // Adds the quote into main quote table, this can be done as there is nothing in this 
            // table that requires other tables to be correct
            _dbContext.Quotes.Add(new Quote()
            {
                QuoteReference = quoteGuid,
                ProductType = productId,
                QuoteAuthor = "TestAuthor",
                QuoteDate = DateTime.Now,
                QuoteStatus = quoteStatusId ?? 0,
                QuoteType = quoteTypeId ?? 0
            });
            // Evaluates if this was able to be saved to the database or not
            var success = _dbContext.SaveChanges() == 1;
            if (!success)
                return InternalServerError(new Exception("Unable to Perform Table Operations"));

            // Inserts into the quote specifc quote table the details of the quotation
            // More information about the function can be found in the function description
            success = InsertIntoTables(quoteType?.IncQuoteType, quoteGuid, saveModel.QuotationDetails, TableType.Quotes);
            if (!success)
                return InternalServerError(new Exception("Unable to Perform Table Operations"));

            // Inserts into the quote specifc results table the details of the quotation result
            var resultsObj = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(saveModel.QuotationCalculation));
            success = InsertIntoTables(quoteType?.IncQuoteType, quoteGuid, resultsObj, TableType.Results);
            if (!success)
                return InternalServerError(new Exception("Unable to Perform Table Operations"));

            // If all of the checks pass, return a 200 to state that they worked
            return Ok();
        }

        /// <summary>
        /// Retrieves a single quote when provided with that quote's GUID
        /// </summary>
        /// <param name="request">The quote's GUID</param>
        /// <returns>The quote whose GUID matches the GUID given to the function. If the Quote does not
        /// exist then it will return a 500 error</returns>
        /// <remarks>
        /// Thankfully simple, this just returns all related Calculations, Details of a quote 
        /// in the form of a single object when presented with a valid GUID.
        /// This allows a user to direct hot jump into the correct quote when performing searches.
        /// </remarks>
        [HttpPost]
        public IHttpActionResult RetrieveWithQuoteReference(HttpRequestMessage request)
        {
            var actGuid = new Guid(request.Content.ReadAsStringAsync().Result);
            var quote = _dbContext.Quotes.ToList().FirstOrDefault(x => x.QuoteReference == actGuid);

            if (quote == null) return InternalServerError(new Exception("Unable to Perform Table Operations"));
            {
                var quoteResult = _dbContext.QuoteTypes.FirstOrDefault(x => x.QuoteTypeID == quote.QuoteType);

                var returnResult = new QuotationSaveWebViewModels
                {
                    // In this case, we need to do the reverse and generically pull the results back from 
                    // the associated tables. More information is within the functions present. Describing
                    // how they work.
                    QuotationCalculation = ReturnFromTables(actGuid, quoteResult?.IncQuoteType, TableType.Results),
                    QuotationDetails = ReturnFromTables(actGuid, quoteResult?.IncQuoteType, TableType.Quotes),
                    QuoteId = quoteResult?.IncQuoteType,
                    ParentId = _dbContext.ProductTypes.ToList()
                        .FirstOrDefault(x => x.ProductTypeID == quote.ProductType)
                        ?.IncProductType
                };

                return Ok(returnResult);
            }
        }

        /// <summary>
        /// Gives a main table quote a new GUID
        /// </summary>
        /// <param name="quoteGuid">The old main table quote GUID</param>
        /// <param name="quoteType">The current quote type that the quote is</param>
        /// <returns>A 200 response if the procedure succeeds, else there will be an exception</returns>
        [HttpGet]
        public IHttpActionResult UpdateQuoteReference(string quoteGuid, string quoteType)
        {
            var quote = _dbContext.Quotes.ToList().FirstOrDefault(x => x.QuoteReference == new Guid(quoteGuid));
            var status = _dbContext.QuoteStatuses.ToList().FirstOrDefault(x => x.State == quoteType && x.Enabled == true);
            quote.QuoteStatus = status.StatusID;
            return Ok();
        }

        /// <summary>
        /// Searches via a regex for signs of replacable text and then replaces them with string representations
        /// of the value that the replacable text should be
        /// </summary>
        /// <param name="frontEndData">A Key Value representation of the parameters sourced from the front end</param>
        /// <param name="templateXML">The Template XML/Calculation that has the strings that are to be replaced</param>
        /// <returns>
        /// A string containing an XML/Calculation template with the replacable text replaced with
        /// the values from the front end
        /// </returns>
        /// <remarks>
        /// Effectively, this seeks to replace {axnx} with the value axnx from the KeyValue pair dictonary.
        /// This allows us to substitue the empty text place holders with actual text values from the front 
        /// end.
        /// This makes templating XML for sending to calculators, or doing a generic calculation from the database
        /// in text format possible and or easier.
        /// </remarks>
        /// <example>
        /// Prior to Function
        /// {TermInMonths} * 40
        /// After Function
        /// 36 * 40
        /// </example>
        private string PopularDefaultsWithString(Dictionary<string, string> frontEndData, string templateXML)
        {
            return Regex.Replace(templateXML, @"\{(.+?)\}", m => frontEndData[m.Groups[1].Value]);
        }

        /// <summary>
        /// Returns a JObject representation of a table row where a matching quote Guid is provided
        /// </summary>
        /// <param name="quoteGuid">The quote Guid for the table row</param>
        /// <param name="quoteType">The quote type that is being requested from the database</param>
        /// <param name="type">The table type being operated on, either a Calculations or a Quote Result table</param>
        /// <returns>A JObject representation of a row of the table</returns>
        /// <remarks>
        /// This is the first instance where we are dynamically accessing the table via the quote type/table type
        /// The specification of the TableType in this format, as well as not making this public, is a specific
        /// defence against people being malicious.
        /// This presumes that the GUID is unique, it may cause problems if the GUID is not unique.
        /// </remarks>
        private JObject ReturnFromTables(Guid quoteGuid, string quoteType, TableType type)
        {
            // Makes a connection to the database via PetaPoco
            var db = new Database("QuoteDB");
            // Formats the table name according to the type of table we are working on as well as the the quote type
            var tablename = $"{quoteType}{type.ToString()}";
            // Fetches, using PetaPoco, all of the row details where there is matching quote reference
            // This is converted into a String, Object relation to be further converted into a JObject later
            // This assumes there is only one correct match, so if there are more than one row with the same GUID
            var returnObject = db.Fetch<dynamic>($"SELECT * FROM {tablename} WHERE QuoteReference = '{quoteGuid.ToString()}'").Cast<IDictionary<string, object>>().FirstOrDefault();
            var retVal = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(returnObject));
            // Removes the Quote Reference column from the object as there is no need for it on the front end
            retVal.Remove("QuoteReference");
            return retVal;
        }

        /// <summary>
        /// Generically inserts into a table a JObject based on the quote type, guid, and table type
        /// </summary>
        /// <param name="quoteType">The quote type that is being requested from the database</param>
        /// <param name="quoteGuid">The quote Guid for the table row</param>
        /// <param name="resultsObj">A JObject representation of the quote details being saved to either the Results or quotes table</param>
        /// <param name="type">The table type being operated on, either a Calculations or a Quote Result table</param>
        /// <returns>A bool signifying if the save operation amanged to succeed or not</returns>
        /// <remarks>
        /// This is the most tricky of all the functions in the API, changes here should be made with great care.
        /// This effectively makes a string object for direct SQL insertion, this was due to the need to have
        /// a completely configurable and generic solution that would allow users to drive the configuration.
        /// All values are entered via the standard string insertion methods to prevent injection attacks.
        /// This function needs to be treated with great care, as it can be a somewhat large security risk.
        /// In future, work maybe done in order to more fully secure the hole that is presented here.
        /// The JObject is the biggest hole due to the fact that it is difficult to check.
        /// </remarks>
        private bool InsertIntoTables(string quoteType, Guid quoteGuid, JObject resultsObj, TableType type)
        {
            try
            {
                // Makes a connection to the database via PetaPoco
                var db = new Database("QuoteDB");
                // Formats the table name according to the type of table we are working on as well as the the quote type
                var tablename = $"{quoteType}{type.ToString()}";
                // Selects the column configurations based off sql string configuration, preventing SQL attacks
                var returnValuesExecute =
                    db.Fetch<ColumnMapper>("WHERE TABLE_NAME = @tablename", new{ tablename });
                // Removes the quote reference column, it doesn't make sense to add it here as we'll be working that 
                // one out generically
                returnValuesExecute.Remove(returnValuesExecute.FirstOrDefault(x => x.ColumnName == "QuoteReference"));
                // Formats the start of the insert statement before entering the loop
                var query = new StringBuilder($"INSERT INTO {quoteType}{type.ToString()} VALUES ('{quoteGuid}'");
                // Loops over the Columns in order to fill the values in
                foreach (var col in returnValuesExecute)
                {
                    // Iterates over the the columns and places them correctly depending on
                    // if they are either an int, string or double value.
                    var value = resultsObj.Value<string>(col.ColumnName);
                    if (int.TryParse(value, out var result))
                    {
                        query.Append($",'{result}'");
                    }
                    else if (double.TryParse(value, out var doubleResult))
                    {
                        query.Append($",'{doubleResult}'");
                    }
                    else
                    {
                        query.Append($",'{value}'");
                    }
                }
                // Closes the insert statement with a bracket for the script execution
                query.Append(")");
                // Executes and evaluates the success of the script
                var success = db.Execute(query.ToString());
                return success == 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}