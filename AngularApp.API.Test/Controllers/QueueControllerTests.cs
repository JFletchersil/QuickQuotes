using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels.ConfigurationViewModels;
using AngularApp.API.Models.WebViewModels.PagingModels;
using AngularApp.API.Models.WebViewModels.QueueDisplayModels;
using AutoMapper;

namespace AngularApp.API.Test.Controllers.Queue
{ 
    [TestClass()]
    public class QueueControllerTests
    {
        private readonly Entities _entity;

        public QueueControllerTests()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg => {
                cfg.CreateMap<QuoteDefault, QuoteDefaultsViewModel>();
                cfg.CreateMap<QuoteDefaultsViewModel, QuoteDefault>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
                cfg.CreateMap<QuoteType, QuoteTypesViewModel>()
                    .ForMember(dest => dest.IncQuoteType, opt => opt.MapFrom(src => src.IncQuoteType))
                    .ForMember(dest => dest.TypeID, opt => opt.MapFrom(src => src.QuoteTypeID));
                cfg.CreateMap<QuoteTypesViewModel, QuoteType>()
                    .ForMember(dest => dest.IncQuoteType, opt => opt.MapFrom(src => src.IncQuoteType))
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


        [TestMethod()]
        public void ShowPaginatedQuotes_ValidModel_ValidEntity_ReturnsPaginatedQueueResult()
        {
            var controller = CreateAndReturnController(false);
            var model = new QueuePagingParameterWebViewModel()
            {
                OrderBy = "QuoteDate",
                PageNumber = 1,
                PageSize = 2
            };
            var retVal = controller.ShowPaginatedQuotes(model);
            Assert.IsInstanceOfType(retVal, typeof(PaginatedQueueResult));
            Assert.IsTrue(retVal.QueueDisplay.Count() == 2);
        }

        [TestMethod()]
        public void ShowPaginatedQuotes_ValidModel_NonsensicalPageSize_ValidEntity_ReturnsPaginatedQueueResult()
        {
            var controller = CreateAndReturnController(false);
            var model = new QueuePagingParameterWebViewModel()
            {
                OrderBy = "QuoteDate",
                PageNumber = 1,
                PageSize = 0
            };
            var retVal = controller.ShowPaginatedQuotes(model);
            Assert.IsInstanceOfType(retVal, typeof(PaginatedQueueResult));
            Assert.IsTrue(!retVal.QueueDisplay.Any());
        }

        [TestMethod()]
        public void ShowPaginatedQuotes_ValidModel_InvalidEntity_Null()
        {
            var controller = CreateAndReturnController(true);
            var model = new QueuePagingParameterWebViewModel()
            {
                OrderBy = "QuoteDate",
                PageNumber = 1,
                PageSize = 2
            };
            var retVal = controller.ShowPaginatedQuotes(model);
            Assert.IsTrue(retVal == null);
        }

        [TestMethod()]
        public void SearchQueueSearchResults_ValidModel_ValidEntity_ReturnsPaginatedQueueResult()
        {
            var controller = CreateAndReturnController(false);
            var model = _entity.Quotes.ToList().FirstOrDefault().QuoteReference.ToString();
            var retVal = controller.SearchQueueSearchResults(new HttpRequestMessage(){Content = new StringContent(model) });
            Assert.IsInstanceOfType(retVal, typeof(OkNegotiatedContentResult<List<QueueDisplayWebViewModel>>));
            Assert.IsTrue((retVal as OkNegotiatedContentResult<List<QueueDisplayWebViewModel>>).Content.FirstOrDefault().QuoteReference == model);
        }

        [TestMethod()]
        public void SearchQueueSearchResults_ValidModel_ValidEntity_ReturnsEmptyList()
        {
            var controller = CreateAndReturnController(false);
            var model = _entity.Quotes.ToList().FirstOrDefault().QuoteReference.ToString() + 4;
            var retVal = controller.SearchQueueSearchResults(new HttpRequestMessage() { Content = new StringContent(model) });
            Assert.IsInstanceOfType(retVal, typeof(OkNegotiatedContentResult<List<QueueDisplayWebViewModel>>));
            Assert.IsTrue(!(retVal as OkNegotiatedContentResult<List<QueueDisplayWebViewModel>>).Content.Any());
        }

        [TestMethod()]
        public void SearchQueueSearchResults_ValidModel_InvalidEntity_Null()
        {
            var controller = CreateAndReturnController(true);
            var model = _entity.Quotes.ToList().FirstOrDefault().QuoteReference.ToString();
            var retVal = controller.SearchQueueSearchResults(new HttpRequestMessage() { Content = new StringContent(model) });
            Assert.IsInstanceOfType(retVal, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod()]
        public void ReturnSearchResults_ValidModel_ValidEntity_QuickSearchResult()
        {
            var controller = CreateAndReturnController(false);
            var model = new QuickSearchRequest()
            {
                ResultNumber = 10,
                SearchText = "TestAuthor"
            };
            var retVal = controller.ReturnSearchResults(model);
            Assert.IsInstanceOfType(retVal, typeof(OkNegotiatedContentResult<List<QuickSearchResult>>));
        }

        [TestMethod()]
        public void ReturnSearchResults_ValidModel_ValidEntity_QuickSearchResult_ListIsEmptyExcludingDefaultItem()
        {
            var controller = CreateAndReturnController(false);
            var model = new QuickSearchRequest()
            {
                ResultNumber = 10,
                SearchText = "12345671213123"
            };
            var retVal = controller.ReturnSearchResults(model);
            Assert.IsInstanceOfType(retVal, typeof(OkNegotiatedContentResult<List<QuickSearchResult>>));
            var data = (retVal as OkNegotiatedContentResult<List<QuickSearchResult>>).Content;
            Assert.IsTrue(data.Count == 1);
        }

        [TestMethod()]
        public void ReturnSearchResults_NullModel_ValidEntity_BadRequestReturn()
        {
            var controller = CreateAndReturnController(false);
            var retVal = controller.ReturnSearchResults(null);
            Assert.IsInstanceOfType(retVal, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod()]
        public void ReturnSearchResults_ValidModel_InvalidEntity_BadRequestReturn()
        {
            var controller = CreateAndReturnController(true);
            var model = new QuickSearchRequest()
            {
                ResultNumber = 10,
                SearchText = "TestAuthor"
            };
            var retVal = controller.ReturnSearchResults(model);
            Assert.IsInstanceOfType(retVal, typeof(BadRequestErrorMessageResult));
        }

        private API.Controllers.QueueController CreateAndReturnController(bool useNullEntity)
        {
            var entityToUse = (!useNullEntity) ? _entity : null;
            var controller = new API.Controllers.QueueController(entityToUse)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            return controller;
        }
    }
}