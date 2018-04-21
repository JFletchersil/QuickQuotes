using Microsoft.VisualStudio.TestTools.UnitTesting;
using AngularApp.API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels.ConfigurationViewModels;
using AutoMapper;
using AngularApp.API.Models.WebViewModels.QuotationSaveModels;
using Newtonsoft.Json;
using PetaPoco;

namespace AngularApp.API.Test.Controllers.Quote
{
    [TestClass()]
    public class QuoteControllerTests
    {
        private readonly Entities _entity;

        public QuoteControllerTests()
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
        public void GetElementConfiguration_WithValidQuoteType_ReturnElementConfiguration()
        {
            var quoteType = _entity.QuoteTypes.FirstOrDefault().IncQuoteType;
            var controller = CreateAndReturnController(false);
            var retVal = controller.GetElementConfiguration(quoteType);
            Assert.IsNotNull(retVal);
            Assert.IsTrue(retVal.Any());
        }

        [TestMethod()]
        public void GetElementConfiguration_WithInvalidQuoteType_ReturnNull()
        {
            var controller = CreateAndReturnController(false);
            var retVal = controller.GetElementConfiguration(null);
            Assert.IsNull(retVal);
        }

        [TestMethod()]
        public void GetElementConfiguration_WithInvalidQuoteType_WithNullEntity_ReturnNull()
        {
            var controller = CreateAndReturnController(true);
            var retVal = controller.GetElementConfiguration(null);
            Assert.IsNull(retVal);
        }

        [TestMethod()]
        public void CalculateQuote_WithValidJsonString_ReturnQuotationResultWebViewModel()
        {
            var controller = CreateAndReturnController(false);
            var quoteDetails = controller.RetrieveWithQuoteReference(new HttpRequestMessage() { Content = new StringContent("C7688665-14B1-45CE-8373-ED05A22636CE") });
            Assert.IsInstanceOfType(quoteDetails, typeof(OkNegotiatedContentResult<QuotationSaveWebViewModels>));
            var returnValues = (quoteDetails as OkNegotiatedContentResult<QuotationSaveWebViewModels>).Content;
            returnValues.QuotationDetails.Add("Type", "PersonalLoan");
            var retVal = controller.CalculateQuote(new HttpRequestMessage{Content = new StringContent(JsonConvert.SerializeObject(returnValues.QuotationDetails)) });
            Assert.IsInstanceOfType(retVal, typeof(QuotationResultWebViewModel));
            Assert.IsNotNull(retVal);
            Assert.IsTrue(retVal.Fees == 50m);
        }

        [TestMethod()]
        public void SaveQuote_ValidSaveModel_ReturnsOk()
        {
            var controller = CreateAndReturnController(false);
            var quoteDetails = controller.RetrieveWithQuoteReference(new HttpRequestMessage() { Content = new StringContent("C7688665-14B1-45CE-8373-ED05A22636CE") });
            Assert.IsInstanceOfType(quoteDetails, typeof(OkNegotiatedContentResult<QuotationSaveWebViewModels>));
            var returnValues = (quoteDetails as OkNegotiatedContentResult<QuotationSaveWebViewModels>).Content;
            var retVal = controller.SaveQuote(returnValues);
            Assert.IsInstanceOfType(retVal, typeof(OkResult));
            var entity = new Entities();
            var quotes = entity.Quotes.ToList().OrderByDescending(x => x.QuoteDate).Where(x => x.QuoteDate.Date == DateTime.Now.Date).ToList();
            var db = new Database("QuoteDB");
            foreach (var quote in quotes)
            {
                var temp = quote.QuoteReference;
                db.Execute($"DELETE FROM {returnValues.QuoteId}Quotes WHERE QuoteReference = '{temp.ToString()}'");
                db.Execute($"DELETE FROM {returnValues.QuoteId}Results WHERE QuoteReference = '{temp.ToString()}'");
                entity.Quotes.Remove(quote);
            }
            entity.SaveChanges();
        }

        [TestMethod()]
        public void SaveQuote_NullModel_ReturnsBadRequest()
        {
            var controller = CreateAndReturnController(false);
            var retVal = controller.SaveQuote(null);
            Assert.IsInstanceOfType(retVal, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod()]
        public void SaveQuote_NullModel_NullEntity_ReturnsBadRequest()
        {
            var controller = CreateAndReturnController(true);
            var retVal = controller.SaveQuote(null);
            Assert.IsInstanceOfType(retVal, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod()]
        public void CalculateQuote_InvalidJsonString_Null()
        {
            var controller = CreateAndReturnController(false);
            var retVal = controller.CalculateQuote(new HttpRequestMessage { Content = new StringContent(JsonConvert.SerializeObject("")) });
            Assert.IsNull(retVal);
        }

        [TestMethod()]
        public void RetrieveWithQuoteReference_WithValidReference_ReturnQuotationSaveWebViewModel()
        {
            var controller = CreateAndReturnController(false);
            var retVal = controller.RetrieveWithQuoteReference(new HttpRequestMessage(){Content = new StringContent("C7688665-14B1-45CE-8373-ED05A22636CE") });
            Assert.IsInstanceOfType(retVal, typeof(OkNegotiatedContentResult<QuotationSaveWebViewModels>));
            var returnValues = (retVal as OkNegotiatedContentResult<QuotationSaveWebViewModels>);
            Assert.IsNotNull(returnValues.Content);
            Assert.IsTrue(returnValues.Content.QuoteId == "PersonalLoan");
        }

        [TestMethod()]
        public void RetrieveWithQuoteReference_WithInvalidReference_ReturnExceptionResult()
        {
            var controller = CreateAndReturnController(false);
            var retVal = controller.RetrieveWithQuoteReference(new HttpRequestMessage() { Content = new StringContent("C7688665-14B1-45CE-8373-ED05A22636C1") });
            Assert.IsInstanceOfType(retVal, typeof(ExceptionResult));
        }

        [TestMethod()]
        public void RetrieveWithQuoteReference_WithInvalidEntity_ReturnBadRequest()
        {
            var controller = CreateAndReturnController(true);
            var retVal = controller.RetrieveWithQuoteReference(new HttpRequestMessage() { Content = new StringContent("C7688665-14B1-45CE-8373-ED05A22636CE") });
            Assert.IsInstanceOfType(retVal, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod()]
        public void UpdateQuoteReference_ReturnsBadRequest_StatingFunctionNotEnabled()
        {
            var controller = CreateAndReturnController(false);
            var retVal = controller.UpdateQuoteReference("", "");
            Assert.IsInstanceOfType(retVal, typeof(BadRequestErrorMessageResult));
        }

        private QuoteController CreateAndReturnController(bool useNullEntity)
        {
            var entityToUse = (!useNullEntity) ? _entity : null;
            var controller = new QuoteController(entityToUse)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            return controller;
        }
    }
}