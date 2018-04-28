using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Web.Http;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels.ConfigurationViewModels;
using AutoMapper;

namespace AngularApp.API.Test.Controllers.UserDetails
{
    [TestClass()]
    public class UserDetailsControllerTests
    {
        private readonly Entities _entity;

        public UserDetailsControllerTests()
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

        [TestMethod()]
        public void ReturnUserModel_WithValidFullName_ReturnUserDetails()
        {
            var controller = CreateAndReturnController();
            var fullName = "Joshua Fletcher";
            var retVal = controller.ReturnUserModel(new HttpRequestMessage { Content = new StringContent(fullName) });
            Assert.IsInstanceOfType(retVal, typeof(Models.DBModels.UserDetails));
            Assert.IsTrue(retVal.Title == "NotAProgrammer");
        }

        [TestMethod()]
        public void ReturnUserModel_WithInvalidFullName_ReturnNull()
        {
            var controller = CreateAndReturnController();
            var fullName = "Joshua";
            var retVal = controller.ReturnUserModel(new HttpRequestMessage { Content = new StringContent(fullName) });
            Assert.IsTrue(retVal == null);
        }

        [TestMethod()]
        public void SaveUserModel_WithValidFullName_ReturnSuccessBool()
        {
            var controller = CreateAndReturnController();
            var fullName = "Joshua Fletcher";
            var retVal = controller.ReturnUserModel(new HttpRequestMessage { Content = new StringContent(fullName) });
            Assert.IsInstanceOfType(retVal, typeof(Models.DBModels.UserDetails));
            Assert.IsTrue(retVal.Title == "NotAProgrammer");
            retVal.Title = "Test Test";
            var boolRetVal = controller.SaveUserModel(retVal);
            Assert.IsTrue(boolRetVal);
            retVal.Title = "NotAProgrammer";
            controller.SaveUserModel(retVal);
        }

        [TestMethod()]
        public void SaveUserModel_WithNullObject_ReturnFailureBool()
        {
            var controller = CreateAndReturnController();
            var boolRetVal = controller.SaveUserModel(null);
            Assert.IsTrue(!boolRetVal);
        }

        private API.Controllers.UserDetailsController CreateAndReturnController()
        {
            var controller = new API.Controllers.UserDetailsController()
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            return controller;
        }
    }
}