using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AngularApp.API.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ConfigurationController : ApiController
    {
        private readonly Entities _dbContext = null;

        public ConfigurationController()
        {
            _dbContext = new Entities();
        }

        [HttpPost]
        public IHttpActionResult DefaultConfigurations(PagingParameterWebViewModel parameterWebView)
        {
            switch (parameterWebView.ConfigurationType)
            {
                case "QuoteDefaults":
                    var defaults = _dbContext.QuoteDefaults.ToList();
                    return Ok(GenerateSelectedItemReferences(defaults, parameterWebView.PageSize, parameterWebView.PageNumber));
                case "QuoteStatuses":
                    var status = _dbContext.QuoteStatuses.ToList();
                    return Ok(GenerateSelectedItemReferences(status, parameterWebView.PageSize, parameterWebView.PageNumber));
                case "QuoteTypes":
                    var types = _dbContext.QuoteTypes.ToList();
                    return Ok(GenerateSelectedItemReferences(types, parameterWebView.PageSize, parameterWebView.PageNumber));
                case "ProductTypes":
                    var pTypes = _dbContext.ProductTypes.ToList();
                    return Ok(GenerateSelectedItemReferences(pTypes, parameterWebView.PageSize, parameterWebView.PageNumber));
                default:
                    return InternalServerError();
            }
        }

        private PaginatedConfigResult GenerateSelectedItemReferences<T>(List<T> items, int pageSize, int pageNumber)
        {
            var mapList = new Dictionary<Type, Type>
            {
                {typeof(QuoteDefault), typeof(QuoteDefaultsViewModel)},
                {typeof(QuoteType), typeof(QuoteTypesViewModel)},
                {typeof(QuoteStatus), typeof(QuoteStatusesViewModel)},
                {typeof(ProductType), typeof(ProductTypesViewModel)}
            };
            var totalPages = (int)Math.Ceiling(items.Count / (double)pageSize);
            var pagedQuotes = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            Type sourceType = typeof(T);
            Type destinationType = mapList[sourceType];
            var destination = new List<object>();
            foreach (var item in items)
            {
                destination.Add(Mapper.Map(item, sourceType, destinationType));
            }
            return new PaginatedConfigResult()
            {
                ConfigResult = JsonConvert.DeserializeObject<List<JObject>>(JsonConvert.SerializeObject(destination)),
                TotalPages = totalPages,
                TotalItems = items.Count
            };
        }
    }
}
