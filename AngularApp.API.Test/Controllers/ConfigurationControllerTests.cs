using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels.ConfigurationViewModels;
using AngularApp.API.Models.WebViewModels.PagingModels;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AngularApp.API.Test.Controllers.Configuration
{
    [TestClass()]
    public class ConfigurationControllerTests
    {
        private Entities _entity;

        public ConfigurationControllerTests()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg => {
                cfg.CreateMap<QuoteDefault, QuoteDefaultsViewModel>();
                cfg.CreateMap<QuoteDefaultsViewModel, QuoteDefault>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
                cfg.CreateMap<QuoteType, QuoteTypesViewModel>()
                    .ForMember(dest => dest.QuoteType, opt => opt.MapFrom(src => src.IncQuoteType))
                    .ForMember(dest => dest.TypeID, opt => opt.MapFrom(src => src.QuoteTypeID));
                cfg.CreateMap<QuoteTypesViewModel, QuoteType>()
                    .ForMember(dest => dest.IncQuoteType, opt => opt.MapFrom(src => src.QuoteType))
                    .ForMember(dest => dest.QuoteTypeID, opt => opt.MapFrom(src => src.TypeID))
                    .IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
                cfg.CreateMap<QuoteStatus, QuoteStatusesViewModel>();
                cfg.CreateMap<QuoteStatusesViewModel, QuoteStatus>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
                cfg.CreateMap<ProductType, ProductTypesViewModel>().ForMember(dest => dest.IncProductType, opt => opt.MapFrom(src => src.IncProductType));
                cfg.CreateMap<ProductTypesViewModel, ProductType>().ForMember(dest => dest.IncProductType, opt => opt.MapFrom(src => src.IncProductType))
                    .IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
            });
            _entity = new Entities();
            var config = GlobalConfiguration.Configuration;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling
                = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SearchDefaultConfigurations_ValidModel_SearchQuoteDefaults()
        {
            var controller = CreateAndReturnController();
            var model = CreateConfigurationSearchWebViewModel("1", "QuoteDefaults");
            var result = controller.SearchDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<List<QuoteDefaultsViewModel>>));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SearchDefaultConfigurations_ValidModel_QuoteStatuses()
        {
            var controller = CreateAndReturnController();
            var model = CreateConfigurationSearchWebViewModel("1", "QuoteStatuses");
            var result = controller.SearchDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<List<QuoteStatusesViewModel>>));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SearchDefaultConfigurations_ValidModel_QuoteTypes()
        {
            var controller = CreateAndReturnController();
            var model = CreateConfigurationSearchWebViewModel("1", "QuoteTypes");
            var result = controller.SearchDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<List<QuoteTypesViewModel>>));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SearchDefaultConfigurations_ValidModel_ProductTypes()
        {
            var controller = CreateAndReturnController();
            var model = CreateConfigurationSearchWebViewModel("1", "ProductTypes");
            var result = controller.SearchDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<List<ProductTypesViewModel>>));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SearchDefaultConfigurations_ValidModel_InvalidRandomType()
        {
            var controller = CreateAndReturnController();
            var model = CreateConfigurationSearchWebViewModel("1", "Random");
            var result = controller.SearchDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(InternalServerErrorResult));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SearchDefaultConfigurations_ValidModel_NullOnInputs()
        {
            var controller = CreateAndReturnController();
            var model = CreateConfigurationSearchWebViewModel(null, null);
            var result = controller.SearchDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(InternalServerErrorResult));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SearchDefaultConfigurations_InvalidModel_Exception()
        {
            var controller = CreateAndReturnController();
            //var model = CreateConfigurationSearchWebViewModel(null, null);
            var result = controller.SearchDefaultConfigurations(null);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void ReturnDefaultConfigurations_ValidModel_SearchQuoteDefaults()
        {
            var controller = CreateAndReturnController();
            var model = CreateConfigurationPagingParameterWebViewModel("TypeID", "QuoteDefaults", 1);
            var result = controller.ReturnDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PaginatedConfigResult>));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void ReturnDefaultConfigurations_ValidModel_QuoteStatuses()
        {
            var controller = CreateAndReturnController();
            var model = CreateConfigurationPagingParameterWebViewModel("StatusID", "QuoteStatuses", 1);
            var result = controller.ReturnDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PaginatedConfigResult>));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void ReturnDefaultConfigurations_ValidModel_QuoteTypes()
        {
            var controller = CreateAndReturnController();
            var model = CreateConfigurationPagingParameterWebViewModel("QuoteTypeID", "QuoteTypes", 1);
            var result = controller.ReturnDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PaginatedConfigResult>));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void ReturnDefaultConfigurations_ValidModel_ProductTypes()
        {
            var controller = CreateAndReturnController();
            var model = CreateConfigurationPagingParameterWebViewModel("ProductTypeID", "ProductTypes", 1);
            var result = controller.ReturnDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PaginatedConfigResult>));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void ReturnDefaultConfigurations_ValidModel_InvalidRandomType()
        {
            var controller = CreateAndReturnController();
            var model = CreateConfigurationPagingParameterWebViewModel("", "Random", 1);
            var result = controller.ReturnDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(InternalServerErrorResult));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void ReturnDefaultConfigurations_ValidModel_NullOnInputs()
        {
            var controller = CreateAndReturnController();
            var model = CreateConfigurationPagingParameterWebViewModel(null, null, 1);
            var result = controller.ReturnDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(InternalServerErrorResult));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void ReturnDefaultConfigurations_InvalidModel_Exception()
        {
            var controller = CreateAndReturnController();
            var result = controller.ReturnDefaultConfigurations(null);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SaveDefaultConfigurations_ValidModel_Update_QuoteDefaults()
        {
            var controller = CreateAndReturnController();
            var quoteDefaults = _entity.QuoteDefaults.ToList();
            quoteDefaults.FirstOrDefault().Enabled = !quoteDefaults.FirstOrDefault().Enabled;


            var model = CreateConfigurationViewModel("QuoteDefaults", JsonConvert.DeserializeObject<List<JObject>>(JsonConvert.SerializeObject(quoteDefaults, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            })));
            var result = controller.SaveDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkResult));

            quoteDefaults.FirstOrDefault().Enabled = !quoteDefaults.FirstOrDefault().Enabled;
            model = CreateConfigurationViewModel("QuoteDefaults", JsonConvert.DeserializeObject<List<JObject>>(JsonConvert.SerializeObject(quoteDefaults, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            })));
            controller.SaveDefaultConfigurations(model);
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SaveDefaultConfigurations_ValidModel_Update_QuoteStatuses()
        {
            var controller = CreateAndReturnController();
            var quoteStatuses = _entity.QuoteStatuses.ToList();
            quoteStatuses.FirstOrDefault().Enabled = !quoteStatuses.FirstOrDefault().Enabled;
            var model = CreateConfigurationViewModel("QuoteStatuses", JsonConvert.DeserializeObject<List<JObject>>(JsonConvert.SerializeObject(quoteStatuses, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            })));
            var result = controller.SaveDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkResult));
            quoteStatuses.FirstOrDefault().Enabled = !quoteStatuses.FirstOrDefault().Enabled;
            model = CreateConfigurationViewModel("QuoteStatuses", JsonConvert.DeserializeObject<List<JObject>>(JsonConvert.SerializeObject(quoteStatuses, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            })));
            controller.SaveDefaultConfigurations(model);
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SaveDefaultConfigurations_ValidModel_Update_QuoteTypes()
        {
            var controller = CreateAndReturnController();
            var quoteTypes = _entity.QuoteTypes.ToList();
            quoteTypes.FirstOrDefault().Enabled = !quoteTypes.FirstOrDefault().Enabled;
            var model = CreateConfigurationViewModel("QuoteTypes", ReturnAllGeneratedItems(quoteTypes));

            var result = controller.SaveDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkResult));
            quoteTypes.FirstOrDefault().Enabled = !quoteTypes.FirstOrDefault().Enabled;
            model = CreateConfigurationViewModel("QuoteTypes", ReturnAllGeneratedItems(quoteTypes));
            controller.SaveDefaultConfigurations(model);
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SaveDefaultConfigurations_ValidModel_Update_ProductTypes()
        {
            var controller = CreateAndReturnController();
            var productTypes = _entity.ProductTypes.ToList();
            productTypes.FirstOrDefault().Enabled = !productTypes.FirstOrDefault().Enabled;
            var model = CreateConfigurationViewModel("ProductTypes", ReturnAllGeneratedItems(productTypes));
            var result = controller.SaveDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkResult));
            productTypes.FirstOrDefault().Enabled = !productTypes.FirstOrDefault().Enabled;
            model = CreateConfigurationViewModel("ProductTypes", ReturnAllGeneratedItems(productTypes));
            controller.SaveDefaultConfigurations(model);
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SaveDefaultConfigurations_ValidModel_Insert_QuoteDefaults()
        {
            var controller = CreateAndReturnController();
            var quoteDefaults = _entity.QuoteDefaults.ToList();
            _entity.QuoteDefaults.RemoveRange(quoteDefaults);
            _entity.SaveChanges();
            var newObj = new List<QuoteDefault>()
            {
                new QuoteDefault()
                {
                    Enabled = true,
                    ElementDescription = "Test",
                    MonthlyRepayableTemplate = "Test",
                    TotalRepayableTemplate = "Test",
                    TypeID = 1,
                    XMLTemplate = "Test"
                }
            };
            var model = CreateConfigurationViewModel("QuoteDefaults", JsonConvert.DeserializeObject<List<JObject>>(JsonConvert.SerializeObject(newObj, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            })));
            var result = controller.SaveDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkResult));
            var entity = new Entities();
            entity.QuoteDefaults.Remove(entity.QuoteDefaults.ToList().LastOrDefault());
            entity.SaveChanges();
            entity.QuoteDefaults.AddRange(quoteDefaults);
            entity.SaveChanges();
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SaveDefaultConfigurations_ValidModel_Insert_QuoteStatuses()
        {
            var controller = CreateAndReturnController();
            var newObj = new List<QuoteStatus>()
            {
                new QuoteStatus()
                {
                    Enabled = true,
                    State = "Test"
                }
            };
            var model = CreateConfigurationViewModel("QuoteStatuses", JsonConvert.DeserializeObject<List<JObject>>(JsonConvert.SerializeObject(newObj, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            })));
            var result = controller.SaveDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkResult));
            var entity = new Entities();
            entity.QuoteStatuses.Remove(entity.QuoteStatuses.ToList().LastOrDefault());
            entity.SaveChanges();
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SaveDefaultConfigurations_ValidModel_Insert_QuoteTypes()
        {
            var controller = CreateAndReturnController();
            var newObj = new QuoteType()
            {
                Enabled = true,
                ProductParentID = 4,
                IncQuoteType = "Test"
            };
            var model = CreateConfigurationViewModel("QuoteTypes", ReturnAllGeneratedItems(new List<QuoteType>(){ newObj }));

            var result = controller.SaveDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkResult));
            var entity = new Entities();
            entity.QuoteTypes.Remove(entity.QuoteTypes.ToList().LastOrDefault());
            entity.SaveChanges();
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SaveDefaultConfigurations_ValidModel_Insert_ProductTypes()
        {
            var controller = CreateAndReturnController();
            var newObj = new ProductType()
            {
                Enabled = true,
                IncProductType = "Test"
            };
            var model = CreateConfigurationViewModel("ProductTypes", ReturnAllGeneratedItems(new List<ProductType>(){ newObj }));
            var result = controller.SaveDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(OkResult));
            var entity = new Entities();
            entity.ProductTypes.Remove(entity.ProductTypes.ToList().LastOrDefault());
            entity.SaveChanges();
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SaveDefaultConfigurations_ValidModel_InvalidRandomType()
        {
            var controller = CreateAndReturnController();
            var model = CreateConfigurationViewModel("Random", new List<JObject>());
            var result = controller.SaveDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(InternalServerErrorResult));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SaveDefaultConfigurations_ValidModel_NullOnInputs()
        {
            var controller = CreateAndReturnController();
            var model = CreateConfigurationViewModel(null, new List<JObject>());
            var result = controller.SaveDefaultConfigurations(model);
            Assert.IsInstanceOfType(result, typeof(InternalServerErrorResult));
        }

        [TestMethod]
        [TestCategory("Controllers.ConfigurationController")]
        public void SaveDefaultConfigurations_InvalidModel_Exception()
        {
            var controller = CreateAndReturnController();
            var result = controller.SaveDefaultConfigurations(null);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        private API.Controllers.ConfigurationController CreateAndReturnController()
        {
            var controller = new API.Controllers.ConfigurationController(_entity)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            return controller;
        }

        private DefaultConfigurationSearchWebViewModel CreateConfigurationSearchWebViewModel(string filterText, string type)
        {
            return new DefaultConfigurationSearchWebViewModel()
            {
                ConfigType = type,
                FilterText = filterText
            };
        }

        private ConfigurationPagingParameterWebViewModel CreateConfigurationPagingParameterWebViewModel(string orderBy, string type, int pageSize)
        {
            return new ConfigurationPagingParameterWebViewModel()
            {
                ConfigurationType = type,
                OrderBy = orderBy,
                PageNumber = 1,
                PageSize = pageSize
            };
        }

        private SaveConfigurationViewModel CreateConfigurationViewModel(string type, List<JObject> obj)
        {
            return new SaveConfigurationViewModel()
            {
                ConfigType = type,
                ConfigsToBeSaved = obj
            };
        }

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
    }
}