using AngularApp.API.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Web.Http.Cors;

namespace AngularApp.API.Controllers
{
    [EnableCors("*", "*", "*")]
    //[RoutePrefix("api/ProductQuoteType")]
    public class ProductQuoteTypeController : ApiController
    {
        private readonly Entities _dbContext = new Entities();

        [HttpGet]
        public JObject GetAllProductTypes()
        {
            var productContexts = new object();
            var productTypes = _dbContext.ProductTypes.ToList();
            var returnData = productTypes.Select(x => x.Enabled);
            var approvedStructures = new JObject();
            foreach(var product in productTypes)
            {
                approvedStructures.Add(product.Enabled
                    ? new JProperty(product.ACProductType, true)
                    : new JProperty(product.ACProductType, false));
            }
            return approvedStructures;
        }

        [HttpGet]
        public JObject GetAllQuoteTypes()
        {
            var productTypes = _dbContext.ProductTypes.ToList();
            var orderedQuoteTypes = _dbContext.QuoteTypes.GroupBy(x => x.ProductParentID);
            var groupedApprovedStructures = new JObject();
            foreach (var productType in orderedQuoteTypes)
            {
                var retProductType = productTypes.FirstOrDefault(x => x.ProductTypeID == productType.Key)?.ACProductType;
                groupedApprovedStructures.Add(new JProperty(retProductType, productType.Select(x => x.ACQuoteType).ToList()));
            }

            return groupedApprovedStructures;
        }
    }
}
