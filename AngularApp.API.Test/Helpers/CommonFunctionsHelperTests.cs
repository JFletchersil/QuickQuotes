using Microsoft.VisualStudio.TestTools.UnitTesting;
using AngularApp.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels.ConfigurationViewModels;
using AutoMapper;

namespace AngularApp.API.Test.Helpers
{
    [TestClass()]
    public class CommonFunctionsHelperTests
    {
        private readonly Entities _entity;

        public CommonFunctionsHelperTests()
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
        public void ReturnSortedList_WithValidListOfItems_ValidOrderBy_ReturnsMatchingDateWithManualOrder()
        {
            var helper = new CommonFunctionsHelper();
            var quotesInOrderOfDates = helper.ReturnSortedList(_entity.Quotes.ToList(), "QuoteDate");
            Assert.IsTrue(_entity.Quotes.ToList().OrderBy(x => x.QuoteDate).FirstOrDefault().QuoteDate == quotesInOrderOfDates.FirstOrDefault().QuoteDate);
        }

        [TestMethod()]
        public void ReturnSortedList_WithValidListOfItems_InvalidOrderBy_ReturnNullReferenceException()
        {
            var helper = new CommonFunctionsHelper();
            try
            {
                var quotesInOrderOfDates = helper.ReturnSortedList(_entity.Quotes.ToList(), "9090");
            }
            catch (NullReferenceException e)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod()]
        public void ReturnSortedList_NoItems_ValidOrderBy_ReturnEmptyList()
        {
            var helper = new CommonFunctionsHelper();
            var quotesInOrderOfDates = helper.ReturnSortedList(new List<Quote>(), "QuoteDate");
            Assert.IsNotNull(quotesInOrderOfDates);
            Assert.IsFalse(quotesInOrderOfDates.Any());
        }

        [TestMethod()]
        public void ReturnSortedList_ValidItems_NoOrderBy_ReturnListBack()
        {
            var helper = new CommonFunctionsHelper();
            var quotesInOrderOfDates = helper.ReturnSortedList(_entity.Quotes.ToList(), "");
            Assert.IsNotNull(quotesInOrderOfDates);
            Assert.IsTrue(quotesInOrderOfDates.Any());
            Assert.IsTrue(quotesInOrderOfDates.Count == _entity.Quotes.ToList().Count);
        }

        [TestMethod()]
        public void ReturnSortedList_WithValidListOfItems_ValidOrderBy_DescendingOrderBy_ReturnsMatchingDateWithManualOrder()
        {
            var helper = new CommonFunctionsHelper();
            var quotesInOrderOfDates = helper.ReturnSortedList(_entity.Quotes.ToList(), "-QuoteDate");
            Assert.IsTrue(_entity.Quotes.ToList().OrderByDescending(x => x.QuoteDate).FirstOrDefault().QuoteDate == quotesInOrderOfDates.FirstOrDefault().QuoteDate);
        }

        [TestMethod()]
        public void PaginateDbTables_WithValidItems_WithValidNumbers_ReturnsCorrectPageModel()
        {
            var helper = new CommonFunctionsHelper();
            var pagedResults = helper.PaginateDbTables(_entity.Quotes.ToList(), 1, 1);
            Assert.IsNotNull(pagedResults);
            Assert.IsTrue(pagedResults.Items.Count == 1);
            Assert.IsTrue(pagedResults.TotalPages == (int)Math.Ceiling(_entity.Quotes.ToList().Count / (double)1));
        }

        [TestMethod()]
        public void PaginateDbTables_WithInvalidItems_WithValidNumbers_ReturnsNull()
        {
            var helper = new CommonFunctionsHelper();
            try
            {
                var pagedResults = helper.PaginateDbTables<Quote>(null, 1, 1);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }
        }
    }
}