using AngularApp.API.Models.DBModels;
using System;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Web.Http.Cors;

namespace AngularApp.API.Controllers
{
    /// <summary>
    /// Provides a list of all the quote and product types within the database.
    /// </summary>
    /// <remarks>
    /// Adding and removing Product and Quote types is handled within the Configuration controller.
    /// This is effectively just a standard returner of all values.
    /// </remarks>
    [EnableCors("*", "*", "*")]
    public class ProductQuoteTypeController : ApiController
    {
        /// <summary>
        /// Creates a private, readonly connection to the database for gathering the quote and product types
        /// </summary>
        /// <remarks>
        /// Unless this changes due to developer actions, this should readonly as there is no need for
        /// this controller to have write permissions to the database.
        /// </remarks>
        private readonly Entities _dbContext = new Entities();

        /// <summary>
        /// Returns all current product types within the database
        /// </summary>
        /// <returns>
        /// All product types within the database
        /// </returns>
        /// <remarks>
        /// As per the enabled feature might suggest, the return object is altered depending on if product type
        /// should be used or not. This prevents disabled product types from appearing within the list of valid
        /// types.
        /// </remarks>
        [HttpGet]
        public JObject GetAllProductTypes()
        {
            try
            {
                var productTypes = _dbContext.ProductTypes.ToList();
                var approvedStructures = new JObject();
                foreach (var product in productTypes)
                {
                    approvedStructures.Add(product.Enabled
                        ? new JProperty(product.IncProductType, true)
                        : new JProperty(product.IncProductType, false));
                }

                return approvedStructures;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns all current quote types within the database
        /// </summary>
        /// <returns>
        /// An ordered list of quote types, ordered by parent product type. 
        /// </returns>
        /// <remarks>
        /// As per the enabled feature might suggest, the return object is altered depending on if quote type
        /// should be used or not. This prevents disabled product types from appearing within the list of valid
        /// types.
        /// This is expected to return an ordered list of quote types ordered by product type.
        /// There is no concept of two or more product types sharing the same quote type, the program
        /// expects there to be a one to many relationship from quote type to product type.
        /// </remarks>
        /// <example>
        /// Finance Type 1
        /// - Quote Type 1
        /// - Quote Type 2
        /// </example>
        [HttpGet]
        public JObject GetAllQuoteTypes()
        {
            var productTypes = _dbContext.ProductTypes.ToList();
            var orderedQuoteTypes = _dbContext.QuoteTypes.GroupBy(x => x.ProductParentID);
            var groupedApprovedStructures = new JObject();
            foreach (var productType in orderedQuoteTypes)
            {
                // Adds to the JObject, as properties, the quotes ordered by associated product type
                // This is required for the front end to properly display all finance and quote types
                var retProductType = productTypes.FirstOrDefault(x => x.ProductTypeID == productType.Key)?.IncProductType;
                groupedApprovedStructures.Add(new JProperty(retProductType, productType.Select(x => x.IncQuoteType).ToList()));
            }

            return groupedApprovedStructures;
        }
    }
}
