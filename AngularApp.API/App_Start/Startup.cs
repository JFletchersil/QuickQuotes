using System.Web.Http;
using AngularApp.API;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels.ConfigurationViewModels;
using AutoMapper;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace AngularApp.API
{
    /// <summary>
    /// The start up class that is run when the Api starts
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configurations the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
            Mapper.Initialize(cfg => {
                cfg.CreateMap<QuoteDefault, QuoteDefaultsViewModel>();
                cfg.CreateMap<QuoteDefaultsViewModel, QuoteDefault>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
                cfg.CreateMap<QuoteType, QuoteTypesViewModel>().ForMember(dest => dest.QuoteType, opt => opt.MapFrom(src => src.IncQuoteType));
                cfg.CreateMap<QuoteTypesViewModel, QuoteType>().ForMember(dest => dest.IncQuoteType, opt => opt.MapFrom(src => src.QuoteType))
                .IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
                cfg.CreateMap<QuoteStatus, QuoteStatusesViewModel>();
                cfg.CreateMap<QuoteStatusesViewModel, QuoteStatus>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
                cfg.CreateMap<ProductType, ProductTypesViewModel>().ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.IncProductType));
                cfg.CreateMap<ProductTypesViewModel, ProductType>().ForMember(dest => dest.IncProductType, opt => opt.MapFrom(src => src.ProductType))
                .IgnoreAllPropertiesWithAnInaccessibleSetter().IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
            });
        }
    }
}