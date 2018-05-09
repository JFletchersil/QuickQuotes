using Microsoft.VisualStudio.TestTools.UnitTesting;
using AngularApp.API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels.ConfigurationViewModels;
using AutoMapper;
using Newtonsoft.Json.Linq;

namespace AngularApp.API.Test.Controllers.ProductQuoteType
{
    [TestClass()]
    public class ProductQuoteTypeControllerTests
    {
        private readonly Entities _entity;

        public ProductQuoteTypeControllerTests()
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
                cfg.CreateMap<ProductType, ProductTypesViewModel>().ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.IncProductType));
                cfg.CreateMap<ProductTypesViewModel, ProductType>().ForMember(dest => dest.IncProductType, opt => opt.MapFrom(src => src.ProductType))
                    .IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
            });
            _entity = new Entities();
            var config = GlobalConfiguration.Configuration;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling
                = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }

        [TestMethod()]
        public void GetAllProductTypes_ValidEntity_ReturnsJObject()
        {
            var controller = CreateAndReturnController(false);
            var retVal = controller.GetAllProductTypes();
            Assert.IsInstanceOfType(retVal, typeof(JObject));
            Assert.IsTrue(retVal.Count == _entity.ProductTypes.ToList().Count);
        }

        [TestMethod()]
        public void GetAllProductTypes_InvalidEntity_ReturnsNull()
        {
            var controller = CreateAndReturnController(true);
            var retVal = controller.GetAllProductTypes();
            Assert.IsTrue(retVal == null);
        }

        [TestMethod()]
        public void GetAllQuoteTypes_ValidEntity_ReturnsJObject()
        {
            var controller = CreateAndReturnController(false);
            var retVal = controller.GetAllQuoteTypes();
            Assert.IsInstanceOfType(retVal, typeof(JObject));
            Assert.IsTrue(retVal.Count == _entity.ProductTypes.ToList().Count);
        }

        [TestMethod()]
        public void GetAllQuoteTypes_InvalidEntity_ReturnsNull()
        {
            var controller = CreateAndReturnController(true);
            var retVal = controller.GetAllQuoteTypes();
            Assert.IsTrue(retVal == null);
        }

        private ProductQuoteTypeController CreateAndReturnController(bool useNullEntity)
        {
            var entityToUse = (!useNullEntity) ? _entity : null;
            var controller = new API.Controllers.ProductQuoteTypeController(entityToUse)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            return controller;
        }

    }
}