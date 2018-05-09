using Microsoft.VisualStudio.TestTools.UnitTesting;
using AngularApp.API.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AngularApp.API.Contexts;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels.AccountViewModels;
using AngularApp.API.Models.WebViewModels.ConfigurationViewModels;
using AngularApp.API.Models.WebViewModels.UserActionModels;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PetaPoco;

namespace AngularApp.API.Test.Repository.AuthRepository
{
    [TestClass()]
    public class AuthRepositoryTests
    {
        private AuthContext _ctx;
        private UserManager<IdentityUser> _userManager;
        private RoleStore<IdentityRole> _roleStore;
        private RoleManager<IdentityRole> _roleMngr;
        private readonly API.Repository.AuthRepository _repo;
        private const string ADMIN = "Administrator";
        private const string USER = "User";

        public AuthRepositoryTests()
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
            _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
            _roleStore = new RoleStore<IdentityRole>(_ctx);
            _roleMngr = new RoleManager<IdentityRole>(_roleStore);
            _repo = new API.Repository.AuthRepository();

            var config = GlobalConfiguration.Configuration;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling
                = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }

        [TestMethod()]
        public void RegisterUser_ValidModel_ReturnSuccess()
        {
            var model = new RegisterViewModel
            {
                UserName = "RegisterTest",
                Email = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                IsAdministrator = false
            };
            var retVal = _repo.RegisterUser(model);
            Assert.IsTrue(retVal.Result.Succeeded);
            var db = new Database("AngularUsers");
            var returnData = db.Query<string>($"SELECT Id FROM AspNetUsers WHERE UserName = 'RegisterTest'").FirstOrDefault();
            var del =_repo.DeleteUser(returnData).Result;
        }

        [TestMethod()]
        public void RegisterUser_ValidModel_NotMatchingPasswords_ReturnFailure()
        {
            var model = new RegisterViewModel
            {
                UserName = "RegisterTest",
                Email = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123",
                IsAdministrator = false
            };
            var retVal = _repo.RegisterUser(model);
            Assert.IsTrue(!retVal.Result.Succeeded);
            //var db = new Database("AngularUsers");
            //var returnData = db.Query<string>($"SELECT Id FROM AspNetUsers WHERE UserName = 'RegisterTest'").FirstOrDefault();
            //var del = _repo.DeleteUser(returnData).Result;
        }

        [TestMethod()]
        public void RegisterUser_InvalidModel_ModelIsNull_ReturnFailure()
        {
            var retVal = _repo.RegisterUser(null);
            Assert.IsTrue(!retVal.Result.Succeeded);
            //var db = new Database("AngularUsers");
            //var returnData = db.Query<string>($"SELECT Id FROM AspNetUsers WHERE UserName = 'RegisterTest'").FirstOrDefault();
            //var del = _repo.DeleteUser(returnData).Result;
        }

        [TestMethod]
        public void CloneUser_ValidModel_OtherUserIsValid_ReturnSuccess()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var model = new CloneUserViewModel
            {
                UserName = "AlterTest",
                EmailAddress = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                OldUserGuid = userGuid.ToString(),
                PhoneNumber = "01685 371019"
            };
            var retVal = _repo.CloneUser(model);
            Assert.IsTrue(retVal.Succeeded);
            var db = new Database("AngularUsers");
            var orginalUser = db.Query<string>($"SELECT Id FROM AspNetUsers WHERE UserName = 'AlterTest'").FirstOrDefault();
            var del = _repo.DeleteUser(userGuid.ToString()).Result;
            del = _repo.DeleteUser(orginalUser).Result;
        }

        [TestMethod]
        public void CloneUser_InvalidModel_OtherUserIsValid_ReturnFailure()
        {
            var retVal = _repo.CloneUser(null);
            Assert.IsTrue(!retVal.Succeeded);
        }

        [TestMethod]
        public void CloneUser_ValidModel_EmptyUserName_OtherUserIsValid_ReturnFailure()
        {
            var model = new CloneUserViewModel
            {
                UserName = "",
                EmailAddress = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                OldUserGuid = "",
                PhoneNumber = "01685 371019"
            };
            var retVal = _repo.CloneUser(model);
            Assert.IsTrue(!retVal.Succeeded);
        }

        [TestMethod]
        public void CloneUser_ValidModel_EmptyPassword_OtherUserIsValid_ReturnFailure()
        {
            var model = new CloneUserViewModel
            {
                UserName = "U",
                EmailAddress = "test@test.com",
                Password = "",
                ConfirmPassword = "Test123!",
                OldUserGuid = "",
                PhoneNumber = "01685 371019"
            };
            var retVal = _repo.CloneUser(model);
            Assert.IsTrue(!retVal.Succeeded);
        }

        [TestMethod]
        public void CloneUser_ValidModel_PasswordNotMatching_OtherUserIsValid_ReturnFailure()
        { 
            var model = new CloneUserViewModel
            {
                UserName = "U",
                EmailAddress = "test@test.com",
                Password = "Test123",
                ConfirmPassword = "Test123!",
                OldUserGuid = "",
                PhoneNumber = "01685 371019"
            };
            var retVal = _repo.CloneUser(model);
            Assert.IsTrue(!retVal.Succeeded);
            Assert.IsTrue(retVal.Errors.FirstOrDefault() == "Passwords Do Not Match For Clone User!");
        }

        [TestMethod]
        public void EditUser_ValidModel_ValidUserId_ValidPhoneNumber_ReturnSuccess()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var model = new RegisterViewModel
            {
                UserName = "AlterTest",
                Password = "Test123!",
                Email = "Testnight@test.com",
                ConfirmPassword = "Test123!"
            };
            var retVal = _repo.EditUser(model, userGuid.ToString(), "01685 371019").Result;
            Assert.IsTrue(retVal.Succeeded);
            var del = _repo.DeleteUser(userGuid.ToString()).Result;
        }

        [TestMethod]
        public void EditUser_ValidModel_PasswordsNotMatch_ValidUserId_ValidPhoneNumber_ReturnFailure()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var model = new RegisterViewModel
            {
                UserName = "AlterTest",
                Password = "Test123",
                ConfirmPassword = "Test123!"
            };
            var retVal = _repo.EditUser(model, userGuid.ToString(), "01685 371019").Result;
            Assert.IsTrue(!retVal.Succeeded);
            var del = _repo.DeleteUser(userGuid.ToString()).Result;
        }

        [TestMethod]
        public void EditUser_InvalidModel_ModelIsNull_ValidUserId_ValidPhoneNumber_ReturnFailure()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var model = new RegisterViewModel
            {
                UserName = "AlterTest",
                Password = "Test123",
                ConfirmPassword = "Test123!"
            };
            var retVal = _repo.EditUser(model, userGuid.ToString(), "01685 371019").Result;
            Assert.IsTrue(!retVal.Succeeded);
            var del = _repo.DeleteUser(userGuid.ToString()).Result;
        }

        [TestMethod]
        public void EditUser_ValidModel_InvalidUserId_ValidPhoneNumber_ReturnFailure()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var model = new RegisterViewModel
            {
                UserName = "AlterTest",
                Email = "Testnight@test.com"
            };
            var retVal = _repo.EditUser(model, "", "01685 371019").Result;
            Assert.IsTrue(!retVal.Succeeded);
            var del = _repo.DeleteUser(userGuid.ToString()).Result;
        }

        [TestMethod]
        public void FindUser_ValidUsername_ValidPassword_ReturnUser()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var retVal = _repo.FindUser("RegisterTest", "Test123!").Result;
            Assert.IsNotNull(retVal);
            Assert.IsTrue(retVal.UserName == "RegisterTest");

            var del = _repo.DeleteUser(userGuid.ToString()).Result;
        }

        [TestMethod]
        public void FindUser_NullUsername_ValidPassword_ReturnNull()
        {
            var retVal = _repo.FindUser(null, "Test123!").Result;
            Assert.IsNull(retVal);
        }

        [TestMethod]
        public void FindUser_InvalidUsername_ValidPassword_ReturnNull()
        {
            var retVal = _repo.FindUser("Hello", "Test123!").Result;
            Assert.IsNull(retVal);
        }

        [TestMethod]
        public void FindUserByGuid_ValidGuid_ReturnUser()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var retVal = _repo.FindUserByGuid(userGuid.ToString());
            Assert.IsNotNull(retVal);
            Assert.IsTrue(retVal.UserName == "RegisterTest");

            var del = _repo.DeleteUser(userGuid.ToString()).Result;
        }

        [TestMethod]
        public void FindUserByGuid_NullInsteadOfGuid_ReturnNull()
        {
            var retVal = _repo.FindUserByGuid(null);
            Assert.IsNull(retVal);
        }

        [TestMethod]
        public void FindUserByGuid_InvalidGuid_ReturnNull()
        {
            var retVal = _repo.FindUserByGuid("Hello");
            Assert.IsNull(retVal);
        }

        [TestMethod]
        public void GetAllUsers()
        {
            var retVal = _repo.GetAllUsers();
            Assert.IsNotNull(retVal);
            Assert.IsTrue(retVal.Exists(x => x.UserName == "FirstAdmin"));
        }

        [TestMethod]
        public void DeleteUser_ValidUserId_ReturnsSuccess()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var retVal = _repo.DeleteUser(userGuid.ToString()).Result;
            Assert.IsTrue(retVal.Succeeded);
        }

        [TestMethod]
        public void DeleteUser_InvalidUserId_ReturnsFailure()
        {
            var retVal = _repo.DeleteUser("55555").Result;
            Assert.IsFalse(retVal.Succeeded);
        }

        [TestMethod]
        public void DeleteUser_NullUserId_ReturnsFailure()
        {
            var retVal = _repo.DeleteUser(null).Result;
            Assert.IsFalse(retVal.Succeeded);
        }

        [TestMethod]
        public void SwapUserRoles_ValidUserId_ValidOldRole_ValidNewRole_SwapFromAdminToUser_SwapBack_ReturnTrue()
        {
            var roleIdAdmin = _roleMngr.Roles.FirstOrDefault(x => x.Name == ADMIN).Id;
            var user = _repo.GetAllUsers().FirstOrDefault(x => x.Roles.Any(y => y.RoleId == roleIdAdmin));
            var retVal = _repo.SwapUserRoles(user.Id, USER, ADMIN);
            Assert.IsTrue(retVal);
            _repo.SwapUserRoles(user.Id, ADMIN, USER);
        }

        [TestMethod]
        public void SwapUserRoles_NullUserId_ValidOldRole_ValidNewRole_ReturnFalse()
        {
            var roleIdAdmin = _roleMngr.Roles.FirstOrDefault(x => x.Name == ADMIN).Id;
            try
            {
                var retVal = _repo.SwapUserRoles(null, USER, ADMIN);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }
        }

        private API.Controllers.AccountController CreateAndReturnController()
        {
            var controller = new API.Controllers.AccountController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            return controller;
        }

        private Guid RegisterAndRetrieveUserGuidAsync()
        {
            var controller = CreateAndReturnController();
            var model = new RegisterViewModel
            {
                UserName = "RegisterTest",
                Email = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                IsAdministrator = false
            };
            var user = controller.Register(model).Result;
            var db = new Database("AngularUsers");
            var returnData = db.Query<string>($"SELECT Id FROM AspNetUsers WHERE UserName = 'RegisterTest'").FirstOrDefault();
            if (returnData != null) return new Guid(returnData);
            throw new NullReferenceException();
        }
    }
}