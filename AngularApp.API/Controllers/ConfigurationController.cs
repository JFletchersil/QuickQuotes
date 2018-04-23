using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels.ConfigurationViewModels;
using AngularApp.API.Models.WebViewModels.PagingModels;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using AngularApp.API.Helpers;

namespace AngularApp.API.Controllers
{
    /// <summary>
    /// The controller responsible for working with the Database to configure the application
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    /// <remarks>
    /// This controller is designed only to update the work involving the configuration of the application.
    /// It is the second most complicated controller due to this, as it must be relatively
    /// flexible to account for the multiple different configuration types.
    /// </remarks>
    [EnableCors("*", "*", "*")]
    public class ConfigurationController : ApiController
    {
        // Provides a context location to the database containing all application user data
        /// <summary>
        /// The database context
        /// </summary>
        private readonly Entities _dbContext;

        private readonly CommonFunctionsHelper _helper;

        /// <summary>
        /// Initializes the ConfigurationController Controller with a entity context, allowing access to the Quote Database.
        /// Constructs the default details for the controller on start up.
        /// </summary>
        /// <remarks>
        /// It's the only constructor, so you must use this.
        /// </remarks>
        public ConfigurationController()
        {
            _dbContext = new Entities();
            _helper = new CommonFunctionsHelper();
        }

        /// <summary>
        /// Provides an initialisation for the Entities, for UnitTests to give the Account Controller a Entities
        /// </summary>
        /// <param name="context">A unit tested Entities</param>
        public ConfigurationController(Entities context)
        {
            _dbContext = context;
            _helper = new CommonFunctionsHelper();
        }

        /// <summary>
        /// Performs a search against all configuration options to locate a matching configuration
        /// </summary>
        /// <param name="viewModel">A model containing the Search Parameter as well as the Configuration Type</param>
        /// <returns>
        /// A list of matching configurations based on the filter text and how close the response is
        /// </returns>
        /// <remarks>
        /// This function works for all configuration types currently configured, it uses the
        /// TestFunction and LINQ to test all the relevant fields for matches.
        /// It also uses AutoMapper to map to and from the Database models before sending the models back to the user.
        /// </remarks>
        [HttpPost]
        public IHttpActionResult SearchDefaultConfigurations(DefaultConfigurationSearchWebViewModel viewModel)
        {
            try
            {
                switch (viewModel.ConfigType)
                {
                    case "QuoteDefaults":
                        // Gathers all the associated configuration items and lists them for mapping and filter.
                        var defaults = _dbContext.QuoteDefaults.ToList();
                        var defaultQuotes = Mapper.Map<List<QuoteDefaultsViewModel>>(defaults);
                        // As mentioned, the Test Function performs text based comparisons, further information is detailed in the test function.
                        defaultQuotes = defaultQuotes.Where(x => TestFunction(x.ElementDescription, viewModel.FilterText) ||
                                                                 TestFunction(x.MonthlyRepayableTemplate, viewModel.FilterText) ||
                                                                 TestFunction(x.TypeID.ToString(), viewModel.FilterText) ||
                                                                 TestFunction(x.TotalRepayableTemplate, viewModel.FilterText) ||
                                                                 TestFunction(x.XMLTemplate, viewModel.FilterText)).ToList();
                        return Ok(defaultQuotes);
                    case "QuoteStatuses":
                        var status = _dbContext.QuoteStatuses.ToList();
                        var defaultStatus = Mapper.Map<List<QuoteStatusesViewModel>>(status);
                        defaultStatus = defaultStatus.Where(x => TestFunction(x.State, viewModel.FilterText) ||
                                                                 TestFunction(x.StatusID.ToString(), viewModel.FilterText)).ToList();
                        return Ok(defaultStatus);
                    case "QuoteTypes":
                        var types = _dbContext.QuoteTypes.ToList();
                        var defaultTypes = Mapper.Map<List<QuoteTypesViewModel>>(types);
                        defaultTypes = defaultTypes.Where(x => TestFunction(x.QuoteType, viewModel.FilterText) ||
                                                               TestFunction(x.ProductParentID.ToString(), viewModel.FilterText) ||
                                                               TestFunction(x.TypeID.ToString(), viewModel.FilterText)).ToList();
                        return Ok(defaultTypes);
                    case "ProductTypes":
                        var pTypes = _dbContext.ProductTypes.ToList();
                        var defaultpTypes = Mapper.Map<List<ProductTypesViewModel>>(pTypes);
                        defaultpTypes = defaultpTypes.Where(x => TestFunction(x.IncProductType, viewModel.FilterText) ||
                                                                 TestFunction(x.ProductTypeID.ToString(), viewModel.FilterText)).ToList();
                        return Ok(defaultpTypes);
                    default:
                        // If unable to find matching type, returns server error.
                        return InternalServerError();
                }
            }
            catch (Exception)
            {
                return BadRequest("An exception occured due to your search");
            }
            // Selects the correct configuration type based on the string provided by the search model
        }

        /// <summary>
        /// Returns a paginated list of all given configuration types when given a correct paging view model.
        /// </summary>
        /// <param name="parameterWebView">Contains the configuration type that is being examined</param>
        /// <returns>
        /// Returns a paginated view model, containing a set number of values, the number of possible pages as well as a list of configuration types.
        /// </returns>
        /// <remarks>
        /// This works in conjunction with the GenerateSelectedItemReference to return a paginated result setup.
        /// This also works to Sort configuration types based on the OrderBy value contained within the parameter configurations
        /// More details on both are in their associated function remarks.
        /// </remarks>
        [HttpPost]
        public IHttpActionResult ReturnDefaultConfigurations(ConfigurationPagingParameterWebViewModel parameterWebView)
        {
            try
            {
                switch (parameterWebView.ConfigurationType)
                {
                    case "QuoteDefaults":
                        var defaults = _dbContext.QuoteDefaults.ToList();
                        defaults = _helper.ReturnSortedList(defaults, parameterWebView.OrderBy);
                        return Ok(GenerateSelectedItemReferences(defaults, parameterWebView.PageSize, parameterWebView.PageNumber));
                    case "QuoteStatuses":
                        var status = _dbContext.QuoteStatuses.ToList();
                        status = _helper.ReturnSortedList(status, parameterWebView.OrderBy);
                        return Ok(GenerateSelectedItemReferences(status, parameterWebView.PageSize, parameterWebView.PageNumber));
                    case "QuoteTypes":
                        var types = _dbContext.QuoteTypes.ToList();
                        types = _helper.ReturnSortedList(types, parameterWebView.OrderBy);
                        return Ok(GenerateSelectedItemReferences(types, parameterWebView.PageSize, parameterWebView.PageNumber));
                    case "ProductTypes":
                        var pTypes = _dbContext.ProductTypes.ToList();
                        pTypes = _helper.ReturnSortedList(pTypes, parameterWebView.OrderBy);
                        return Ok(GenerateSelectedItemReferences(pTypes, parameterWebView.PageSize, parameterWebView.PageNumber));
                    default:
                        return InternalServerError();
                }
            }
            catch (Exception)
            {
                return BadRequest("An exception occured due to your search");
            }
        }

        /// <summary>
        /// Saves a given default configuration depending on what type the configuration type is being worked with
        /// </summary>
        /// <param name="saveConfigs">Contains the configuration type that is being saved, as well as the
        /// list of configurations that is to be saved.</param>
        /// <returns>
        /// A 200 or 500 HTTP response depending on if the result was a success or not
        /// </returns>
        /// <remarks>
        /// The function presumes that you wish to update only one type of configuration at a time, but
        /// with any number of configurations within that range.
        /// It will break if you attempt to mix different configuration types, however it will allow you to do this.
        /// </remarks>
        [HttpPost]
        public IHttpActionResult SaveDefaultConfigurations(SaveConfigurationViewModel saveConfigs)
        {
            try
            {
                switch (saveConfigs.ConfigType)
                {
                    case "QuoteDefaults":
                        // Converts the Object from a List of JObjects to a List of the QuoteDefaults
                        // The same code is used throughout the switch statements
                        var defaults = JsonConvert.DeserializeObject<List<QuoteDefaultsViewModel>>
                            (JsonConvert.SerializeObject(saveConfigs.ConfigsToBeSaved));

                        var defaultList = ReturnUpdatedList(_dbContext.QuoteDefaults.ToList(), defaults);

                        // This code block, similar to each configuration type, checks to see if this is a new
                        // configuration type or an old one. If it is an old one, it updates the model for saving
                        // if it's a new one, it just adds to the context for adding to the database
                        foreach (var item in defaultList)
                        {
                            var result = _dbContext.QuoteDefaults.SingleOrDefault(x => x.TypeID == item.TypeID);
                            if (result != null)
                            {
                                result.MonthlyRepayableTemplate = item.MonthlyRepayableTemplate;
                                result.TotalRepayableTemplate = item.TotalRepayableTemplate;
                                result.XMLTemplate = item.XMLTemplate;
                                result.Enabled = item.Enabled;
                                result.ElementDescription = item.ElementDescription;
                            }
                            else
                            {
                                _dbContext.QuoteDefaults.Add(item);
                            }
                        }

                        _dbContext.SaveChanges();
                        return Ok();
                    case "QuoteStatuses":
                        var status = JsonConvert.DeserializeObject<List<QuoteStatusesViewModel>>
                            (JsonConvert.SerializeObject(saveConfigs.ConfigsToBeSaved));

                        var statusList = ReturnUpdatedList(_dbContext.QuoteStatuses.ToList(), status);

                        foreach (var item in statusList)
                        {
                            var result = _dbContext.QuoteStatuses.SingleOrDefault(x => x.StatusID == item.StatusID);
                            if (result != null)
                            {
                                result.State = item.State;
                                result.Enabled = item.Enabled;
                            }
                            else
                            {
                                _dbContext.QuoteStatuses.Add(item);
                            }
                        }

                        _dbContext.SaveChanges();
                        return Ok();
                    case "QuoteTypes":
                        var types = JsonConvert.DeserializeObject<List<QuoteTypesViewModel>>
                            (JsonConvert.SerializeObject(saveConfigs.ConfigsToBeSaved));

                        var typeList = ReturnUpdatedList(_dbContext.QuoteTypes.ToList(), types);

                        foreach (var item in typeList)
                        {
                            var result = _dbContext.QuoteTypes.SingleOrDefault(x => x.QuoteTypeID == item.QuoteTypeID);
                            if (result != null)
                            {
                                result.IncQuoteType = item.IncQuoteType;
                                result.ProductParentID = item.ProductParentID;
                                result.Enabled = item.Enabled;
                            }
                            else
                            {
                                _dbContext.QuoteTypes.Add(item);
                            }
                        }

                        _dbContext.SaveChanges();
                        return Ok();
                    case "ProductTypes":
                        var pTypes = JsonConvert.DeserializeObject<List<ProductTypesViewModel>>
                            (JsonConvert.SerializeObject(saveConfigs.ConfigsToBeSaved));

                        var pTypesList = ReturnUpdatedList(_dbContext.ProductTypes.ToList(), pTypes);

                        foreach (var item in pTypesList)
                        {
                            var result =
                                _dbContext.ProductTypes.SingleOrDefault(x => x.ProductTypeID == item.ProductTypeID);
                            if (result != null)
                            {
                                result.IncProductType = item.IncProductType;
                                result.Enabled = item.Enabled;
                            }
                            else
                            {
                                _dbContext.ProductTypes.Add(item);
                            }
                        }

                        _dbContext.SaveChanges();
                        return Ok();
                    default:
                        return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest("An exception occured due to your search");
            }
        }

        /// <summary>
        /// Returns a casted, mapped list from the web view models to the database models
        /// </summary>
        /// <typeparam name="T">The type of the old list being mapped from</typeparam>
        /// <typeparam name="J">The type of the new list being mapped to</typeparam>
        /// <param name="oldList">This list is used to ascertain the type being converted to, it is not actually used for data processing</param>
        /// <param name="newList">The list of web view models being mapped away from</param>
        /// <returns>
        /// A mapped and casted list of web view models into database view models.
        /// </returns>
        /// <remarks>
        /// The purpose of this list is to take the web view models and convert them into database view models
        /// for purposes of saving them to the database.
        /// </remarks>
        private List<T> ReturnUpdatedList<T, J>(List<T> oldList, List<J> newList)
        {
            var mapList = new Dictionary<Type, Type>
            {
                {typeof(QuoteDefault), typeof(QuoteDefaultsViewModel)},
                {typeof(QuoteType), typeof(QuoteTypesViewModel)},
                {typeof(QuoteStatus), typeof(QuoteStatusesViewModel)},
                {typeof(ProductType), typeof(ProductTypesViewModel)}
            };

            // Identifies the destination and source type based on the values given from the old and new list
            Type destinationType = typeof(T);
            Type sourceType = mapList[destinationType];
            // Creates a new list to hold the objects in question
            var destination = new List<object>();
            // Iterates over each item in the items being converted away from and maps them to their new
            // type
            foreach (var item in newList)
            {
                destination.Add(Mapper.Map(item, sourceType, destinationType));
            }
            // Casts them to the correct type, for easier use later.
            var castedNewList = destination.Cast<T>().ToList();
            return castedNewList;
        }

        /// <summary>
        /// Paginates a given list of items and makes a configuration model to return the data to the front end
        /// </summary>
        /// <typeparam name="T">The type of configuration being worked with</typeparam>
        /// <param name="items">The full list of database models that is being paginated</param>
        /// <param name="pageSize">Sets the given size of the pages, I.e. 5 would make pages 5 items long</param>
        /// <param name="pageNumber">Sets the current page number the front end is on, I.e. 2 would give us 6-10 paginated items</param>
        /// <returns>
        /// A paginated object, containing all the details to properly represent the given pagination configuration and the results
        /// </returns>
        /// <remarks>
        /// It is expected that the results will be ordered prior to being given to the function to be paginated.
        /// Objects returned are made out of a web view model, rather than a database model, this is important
        /// to prevent cyclical referencing errors with other types.
        /// </remarks>
        private PaginatedConfigResult GenerateSelectedItemReferences<T>(List<T> items, int pageSize, int pageNumber)
        {
            var pagedConfigurations = _helper.PaginateDbTables(items, pageNumber, pageSize);
            // Returns the values for usage elsewhere in the controller.
            return new PaginatedConfigResult()
            {
                // Maps the results to the correct view model, away from the database view model
                // THIS IS IMPORTANT, YOU WILL GET ERRORS UNLESS IT IS MAPPED AWAY
                ConfigResult = ReturnAllGeneratedItems(pagedConfigurations.Items),
                TotalPages = pagedConfigurations.TotalPages,
                TotalItems = items.Count
            };
        }

        /// <summary>
        /// Maps the given values away from database view models to web view models, breaking any object references and giving only data
        /// </summary>
        /// <typeparam name="T">The database model being mapped away from</typeparam>
        /// <param name="pagedConfigurations">The paged configuration values being mapped to view models</param>
        /// <returns>
        /// A List of JObjects representing the paginated configuration results
        /// </returns>
        /// <remarks>
        /// JObjects are used, as they are generic across all of the quote types, this allows us to simplify
        /// the code down and use the same functions for all the configuration types.
        /// This does not pose a reference problem, due to the fact that they are all accurate prior to this
        /// simplification.
        /// </remarks>
        private List<JObject> ReturnAllGeneratedItems<T>(List<T> pagedConfigurations)
        {
            var mapList = new Dictionary<Type, Type>
            {
                {typeof(QuoteDefault), typeof(QuoteDefaultsViewModel)},
                {typeof(QuoteType), typeof(QuoteTypesViewModel)},
                {typeof(QuoteStatus), typeof(QuoteStatusesViewModel)},
                {typeof(ProductType), typeof(ProductTypesViewModel)}
            };

            Type sourceType = typeof(T);
            Type destinationType = mapList[sourceType];
            var destination = new List<object>();
            foreach (var item in pagedConfigurations)
            {
                destination.Add(Mapper.Map(item, sourceType, destinationType));
            }

            // Object is serialised to a string, prior to being deserialised down to a JObject list, else rest
            // is similar to details provided above.
            return JsonConvert.DeserializeObject<List<JObject>>(JsonConvert.SerializeObject(destination));
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
