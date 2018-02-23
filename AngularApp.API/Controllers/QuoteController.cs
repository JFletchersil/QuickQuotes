using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using PetaPoco;

namespace AngularApp.API.Controllers
{
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/Quote")]
    public class QuoteController : ApiController
    {
        private readonly Entities _dbContext = new Entities();

        private enum TableType
        {
            Quotes,
            Results
        }

        [HttpGet]
        public List<ElementConfigurationWebViewModel> GetElementConfiguration(string quoteType)
        {
            var configuration = _dbContext.QuoteDefaults.FirstOrDefault(x =>
                x.TypeID == _dbContext.QuoteTypes.FirstOrDefault(y => y.IncQuoteType == quoteType).TypeID);

            return configuration != null ?
                JsonConvert.DeserializeObject<List<ElementConfigurationWebViewModel>>(configuration.ElementDescription) : null;
        }

        [HttpPost]
        public QuotationResultWebViewModel CalculateQuote(HttpRequestMessage request)
        {
            var dataText = request.Content.ReadAsStringAsync().Result;
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataText);
            var objType = JsonConvert.DeserializeObject<JObject>(dataText).GetValue("Type").Value<string>();
            var quoteDefaults = _dbContext.QuoteDefaults.FirstOrDefault(x => x.QuoteType.IncQuoteType == objType);
            var interestRate = 5d;
            //var retVal = PopularDefaultsWithString(dict, quoteDefaults.XMLTemplate);
            //var quoteGenerationModel = JsonConvert.DeserializeObject<PersonalLoanWebViewModel>(dataText);
            dict.Add("InterestRate", "5");
            var retVal = PopularDefaultsWithString(dict, quoteDefaults.TotalRepayableTemplate);


            var totalRepayable = Convert.ToDecimal(new DataTable().Compute(retVal, null));


            return new QuotationResultWebViewModel()
            {
                Fees = 50.00m,
                InterestRate = interestRate,
                MonthlyRepayable = (totalRepayable / Convert.ToDecimal(dict["TermInMonths"])),
                TotalRepayable = totalRepayable
            };
        }

        // TODO - We need to make a generic view model that can handle all the finance types.
        [HttpPost]
        public IHttpActionResult SaveQuote(QuotationSaveWebViewModels saveModel)
        {
            var first = _dbContext.ProductTypes.ToList().FirstOrDefault
                (x => x.IncProductType != null && string.Equals(x.IncProductType, saveModel.ParentId, StringComparison.CurrentCultureIgnoreCase));
            var productId = first.ProductTypeID;
            // Can Remain Defaulted, the first state of any quote should be QQActive anyway
            var quoteStatusId = _dbContext.QuoteStatuses.ToList().FirstOrDefault(x => x.State != null && x.State == "QQActive")?.StatusID;
            var quoteType = _dbContext.QuoteTypes.ToList()
                .FirstOrDefault(x => x.IncQuoteType != null && string.Equals(x.IncQuoteType, saveModel.QuoteId,
                                         StringComparison.CurrentCultureIgnoreCase));
            var quoteTypeId = quoteType?.TypeID;

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

            var success = _dbContext.SaveChanges() == 1;
            if (!success)
                return InternalServerError(new Exception("Unable to Perform Table Operations"));

            success = InsertIntoTables(quoteType?.IncQuoteType, quoteGuid, saveModel.QuotationDetails, TableType.Quotes);
            if (!success)
                return InternalServerError(new Exception("Unable to Perform Table Operations"));

            var resultsObj = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(saveModel.QuotationCalculation));
            success = InsertIntoTables(quoteType?.IncQuoteType, quoteGuid, resultsObj, TableType.Results);
            if (!success)
                return InternalServerError(new Exception("Unable to Perform Table Operations"));

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult RetrieveWithQuoteReference(HttpRequestMessage request)
        {

            var actGuid = new Guid(request.Content.ReadAsStringAsync().Result);
            var quote = _dbContext.Quotes.ToList().FirstOrDefault(x => x.QuoteReference == actGuid);

            if (quote == null) return InternalServerError(new Exception("Unable to Perform Table Operations"));
            {
                var quoteResult = _dbContext.QuoteTypes.FirstOrDefault(x => x.TypeID == quote.QuoteType);

                var returnResult = new QuotationSaveWebViewModels
                {
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

        private string PopularDefaultsWithString(Dictionary<string, string> frontEndData, string templateXML)
        {
            return Regex.Replace(templateXML, @"\{(.+?)\}", m => frontEndData[m.Groups[1].Value]);
        }

        private JObject ReturnFromTables(Guid quoteGuid, string quoteType, TableType type)
        {
            var db = new Database("QuoteDB");
            var tablename = $"{quoteType}{type.ToString()}";
            var returnObject = db.Fetch<dynamic>($"SELECT * FROM {tablename} WHERE QuoteReference = '{quoteGuid.ToString()}'").Cast<IDictionary<string, object>>().FirstOrDefault();
            var retVal = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(returnObject));
            retVal.Remove("QuoteReference");
            return retVal;
        }

        private bool InsertIntoTables(string quoteType, Guid quoteGuid, JObject resultsObj, TableType type)
        {
            try
            {
                var db = new Database("QuoteDB");
                var tablename = $"{quoteType}{type.ToString()}";
                var returnValuesExecute =
                    db.Fetch<ColumnMapper>("WHERE TABLE_NAME = @tablename", new{ tablename });
                returnValuesExecute.Remove(returnValuesExecute.FirstOrDefault(x => x.ColumnName == "QuoteReference"));
                var query = new StringBuilder($"INSERT INTO {quoteType}{type.ToString()} VALUES ('{quoteGuid}'");
                foreach (var col in returnValuesExecute)
                {
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

                query.Append(")");
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