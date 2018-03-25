using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public IHttpActionResult SearchDefaultConfigurations(DefaultConfigurationSearchWebViewModel viewModel)
        {
            switch (viewModel.ConfigType)
            {
                case "QuoteDefaults":
                    var defaults = _dbContext.QuoteDefaults.ToList();
                    var defaultQuotes = Mapper.Map<List<QuoteDefaultsViewModel>>(defaults);
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
                    defaultpTypes = defaultpTypes.Where(x => TestFunction(x.ProductType, viewModel.FilterText) ||
                                                             TestFunction(x.ProductTypeID.ToString(), viewModel.FilterText)).ToList();
                    return Ok(defaultpTypes);
                default:
                    return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult ReturnDefaultConfigurations(ConfigurationPagingParameterWebViewModel parameterWebView)
        {
            switch (parameterWebView.ConfigurationType)
            {
                case "QuoteDefaults":
                    var defaults = _dbContext.QuoteDefaults.ToList();
                    defaults = SortConfigurationTypes(defaults, parameterWebView.OrderBy);
                    return Ok(GenerateSelectedItemReferences(defaults, parameterWebView.PageSize, parameterWebView.PageNumber));
                case "QuoteStatuses":
                    var status = _dbContext.QuoteStatuses.ToList();
                    status = SortConfigurationTypes(status, parameterWebView.OrderBy);
                    return Ok(GenerateSelectedItemReferences(status, parameterWebView.PageSize, parameterWebView.PageNumber));
                case "QuoteTypes":
                    var types = _dbContext.QuoteTypes.ToList();
                    types = SortConfigurationTypes(types, parameterWebView.OrderBy);
                    return Ok(GenerateSelectedItemReferences(types, parameterWebView.PageSize, parameterWebView.PageNumber));
                case "ProductTypes":
                    var pTypes = _dbContext.ProductTypes.ToList();
                    pTypes = SortConfigurationTypes(pTypes, parameterWebView.OrderBy);
                    return Ok(GenerateSelectedItemReferences(pTypes, parameterWebView.PageSize, parameterWebView.PageNumber));
                default:
                    return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult SaveDefaultConfigurations(SaveConfigurationViewModel saveConfigs)
        {
            switch (saveConfigs.ConfigType)
            {
                case "QuoteDefaults":
                    var defaults = JsonConvert.DeserializeObject<List<QuoteDefaultsViewModel>>
                        (JsonConvert.SerializeObject(saveConfigs.ConfigsToBeSaved));

                    var defaultList = ReturnUpdatedList(_dbContext.QuoteDefaults.ToList(), defaults);

                    foreach (var item in defaultList)
                    {
                        var result = _dbContext.QuoteDefaults.SingleOrDefault(x => x.TypeID == item.TypeID);
                        if (result != null)
                        {
                            result.MonthlyRepayableTemplate = item.MonthlyRepayableTemplate;
                            result.QuoteTypeID = item.QuoteTypeID;
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
                        var result = _dbContext.ProductTypes.SingleOrDefault(x => x.ProductTypeID == item.ProductTypeID);
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

        private List<T> SortConfigurationTypes<T>(List<T> sortingList, string orderBy)
        {
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                if (orderBy.Contains("-"))
                {
                    sortingList.OrderByDescending(x => x.GetType().GetProperty(orderBy.Split('-')[1]));
                }
                else
                {
                    sortingList.OrderBy(x => x.GetType().GetProperty(orderBy));
                }
            }
            return sortingList;
        }

        private List<T> ReturnUpdatedList<T, J>(List<T> oldList, List<J> newList)
        {
            var mapList = new Dictionary<Type, Type>
            {
                {typeof(QuoteDefault), typeof(QuoteDefaultsViewModel)},
                {typeof(QuoteType), typeof(QuoteTypesViewModel)},
                {typeof(QuoteStatus), typeof(QuoteStatusesViewModel)},
                {typeof(ProductType), typeof(ProductTypesViewModel)}
            };

            Type destinationType = typeof(T);
            Type sourceType = mapList[destinationType];
            var destination = new List<object>();
            foreach (var item in newList)
            {
                destination.Add(Mapper.Map(item, sourceType, destinationType));
            }
            var castedNewList = destination.Cast<T>().ToList();
            return castedNewList;
        }

        private PaginatedConfigResult GenerateSelectedItemReferences<T>(List<T> items, int pageSize, int pageNumber)
        {
            var totalPages = (int)Math.Ceiling(items.Count() / (double)pageSize);
            var pagedQuotes = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedConfigResult()
            {
                ConfigResult = ReturnAllGeneratedItems(pagedQuotes),
                TotalPages = totalPages,
                TotalItems = items.Count
            };
        }

        private List<JObject> ReturnAllGeneratedItems<T>(List<T> pagedQuotes)
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
            foreach (var item in pagedQuotes)
            {
                destination.Add(Mapper.Map(item, sourceType, destinationType));
            }

            return JsonConvert.DeserializeObject<List<JObject>>(JsonConvert.SerializeObject(destination));
        }

        private bool TestFunction(string str1, string str2)
        {
            return str1 != null && str1.ToUpper().Contains(str2.ToUpper());
        }
    }
}
