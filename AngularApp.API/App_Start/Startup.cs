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
    /// Starts and configures the project on start up.
    /// Creates a series of AutoMapper maps between different types
    /// Configures the WebApi Interface
    /// </summary>
    /// <remarks>
    /// The startup class can be troublesome if the OWIN details are changed, if you wish
    /// to alter the OWIN details make sure you have a backup prior to doing so.
    /// </remarks>
    public class Startup
    {
        /// <summary>
        /// Runs the configuration setup on an IAppBuilder
        /// </summary>
        /// <param name="app">Represents the configuration options of the app, includes the WebApi Configuration</param>
        /// <remarks>
        /// Unless otherwise needed, this should be left alone. This should only be altered if
        /// more global mappers must be added, or if more changes need to be made to the WebApi.
        /// JF - 08/04/2018 - Further work on Auth0 would involve this area, this would place the 
        ///                   middleware layer for authentication between the API and the controllers.
        /// </remarks>
        public void Configuration(IAppBuilder app)
        {
            // Creates a new HTTPConfiguration object for our WebApi
            var config = new HttpConfiguration();
            // Registers the WebApi as active
            // WARNING - Removing this line will stop the Api from working
            WebApiConfig.Register(config);
            // Tells the application to use this WebApi as the main WebApi.
            app.UseWebApi(config);
            // Creates a bunch of mapping relations
            // These relations map from the configuration database model to a web view model
            // This allows us to strip a few items out, and to remove the relational data from the db models
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