using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels.AccountViewModels;
using AngularApp.API.Models.WebViewModels.PagingModels;
using AngularApp.API.Models.WebViewModels.UserActionModels;
using AngularApp.API.Models.WebViewModels.UserQueueDisplayModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PetaPoco;

namespace AngularApp.API.Test.Controllers.AccountController
{
    [TestClass]
    public class AccountControllerTests
    {
        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public async Task LoginTest_ValidModel_ValidUserAsync()
        {
            var model = new LoginViewModel
            {
                Email = "FirstAdmin",
                Password = "Test123!"
            };

            var controller = new API.Controllers.AccountController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var retVal = await controller.Login(model);
            var contentResult = retVal as OkNegotiatedContentResult<JObject>;
            Assert.IsInstanceOfType(retVal, typeof(OkNegotiatedContentResult<JObject>));
            Assert.IsTrue(contentResult.Content.Property("UserName").Value.ToString() == model.Email);
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public async Task LoginTest_ValidModel_InvalidUser()
        {
            var model = new LoginViewModel
            {
                Email = "FirstAdmin",
                Password = "Test123"
            };

            var controller = new API.Controllers.AccountController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var retVal = await controller.Login(model);
            Assert.IsInstanceOfType(retVal, typeof(BadRequestResult));
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public async Task LoginTest_ValidModel_ValidUser_InvalidDetails()
        {
            var model = new LoginViewModel
            {
                Email = "FirstAdmin",
                Password = "Test123!"
            };

            var db = new Database("AngularUsers");
            var returnData = db.Query<Models.DBModels.UserDetails>($"SELECT * FROM AspNetUserDetails WHERE Id = '472bd65a-168d-431a-bfc6-7107cd3bfb15'").FirstOrDefault();
            returnData.FullName = "";
            db.Update("AspNetUserDetails", "Id", returnData);

            var controller = new API.Controllers.AccountController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var retVal = await controller.Login(model);
            returnData.FullName = "Joshua Fletcher";
            db.Update("AspNetUserDetails", "Id", returnData);
            Assert.IsInstanceOfType(retVal, typeof(BadRequestResult));
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public async Task RegisterTest_ValidModel_ValidUser()
        {
            var model = new RegisterViewModel
            {
                UserName = "RegisterTest",
                Email = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                IsAdministrator = false
            };
            var controller = new API.Controllers.AccountController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            var retVal = await controller.Register(model);
            var db = new Database("AngularUsers");
            var returnData = db.Query<string>($"SELECT Id FROM AspNetUsers WHERE UserName = 'RegisterTest'").FirstOrDefault();

            DeleteUser(new Guid(returnData));

            Assert.IsInstanceOfType(retVal, typeof(OkResult));
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public async Task RegisterTest_InvalidModel_ValidUser()
        {
            var model = new RegisterViewModel
            {
                UserName = "RegisterTest",
                Email = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123",
                IsAdministrator = false
            };
            var controller = new API.Controllers.AccountController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            var retVal = await controller.Register(model);
            Assert.IsInstanceOfType(retVal, typeof(BadRequestResult));
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void ReturnAllUsersTest_ValidModel_GetAdminUsers()
        {
            var model = new AccountPagingParameterWebViewModel
            {
                PageNumber = 1,
                PageSize = 1,
                ReturnAll = true
            };
            var controller = new API.Controllers.AccountController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            var retVal = controller.ReturnAllUsers(model);
            var contentResult = retVal as OkNegotiatedContentResult<PaginatedQueueUserResult>;
            Assert.IsInstanceOfType(retVal, typeof(OkNegotiatedContentResult<PaginatedQueueUserResult>));
            Assert.IsTrue(contentResult.Content.QueueDisplay.FirstOrDefault().IsAdmin);
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void ReturnAllUsersTest_ValidModel_GetStandardUsers()
        {
            var model = new AccountPagingParameterWebViewModel
            {
                PageNumber = 1,
                PageSize = 5,
                ReturnAll = false
            };
            var controller = new API.Controllers.AccountController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            var retVal = controller.ReturnAllUsers(model);
            var contentResult = retVal as OkNegotiatedContentResult<PaginatedQueueUserResult>;
            Assert.IsInstanceOfType(retVal, typeof(OkNegotiatedContentResult<PaginatedQueueUserResult>));
            Assert.IsTrue(contentResult.Content.TotalPages == 1);
            Assert.IsTrue(!contentResult.Content.QueueDisplay.FirstOrDefault().IsAdmin);
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void ReturnAllUsersTest_ValidModel_GetAdminUsers_FilteredAscending()
        {
            var model = new AccountPagingParameterWebViewModel
            {
                PageNumber = 1,
                PageSize = 5,
                ReturnAll = true,
                OrderBy = "Id"
            };
            var controller = new API.Controllers.AccountController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            var retVal = controller.ReturnAllUsers(model);
            var contentResult = retVal as OkNegotiatedContentResult<PaginatedQueueUserResult>;
            Assert.IsInstanceOfType(retVal, typeof(OkNegotiatedContentResult<PaginatedQueueUserResult>));
            Assert.IsTrue(contentResult.Content.TotalPages == 1);
            Assert.IsTrue(contentResult.Content.QueueDisplay.FirstOrDefault().IsAdmin);
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void ReturnAllUsersTest_ValidModel_GetAdminUsers_FilteredDecending()
        {
            var model = new AccountPagingParameterWebViewModel
            {
                PageNumber = 1,
                PageSize = 5,
                ReturnAll = true,
                OrderBy = "-Id"
            };
            var controller = new API.Controllers.AccountController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            var retVal = controller.ReturnAllUsers(model);
            var contentResult = retVal as OkNegotiatedContentResult<PaginatedQueueUserResult>;
            Assert.IsInstanceOfType(retVal, typeof(OkNegotiatedContentResult<PaginatedQueueUserResult>));
            Assert.IsTrue(contentResult.Content.TotalPages == 1);
            Assert.IsTrue(contentResult.Content.QueueDisplay.FirstOrDefault().IsAdmin);
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void ReturnAllUsersAtLevelSearch_ValidModel_ValidReturn_AdminUsers()
        {
            var model = new SearchAccounthWebViewModel
            {
                ReturnAll = true,
                FilterTerm = "GmanAdmin"
            };
            var controller = new API.Controllers.AccountController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            var retVal = controller.ReturnAllUsersAtLevelForSearch(model);
            var contentResult = retVal as OkNegotiatedContentResult<List<QueueUserWebViewModel>>;
            Assert.IsInstanceOfType(retVal, typeof(OkNegotiatedContentResult<List<QueueUserWebViewModel>>));
            Assert.IsTrue(contentResult.Content.Count == 1);
            Assert.IsTrue(contentResult.Content.FirstOrDefault().IsAdmin);
            Assert.IsTrue(contentResult.Content.FirstOrDefault().UserName.Contains("GmanAdmin"));
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void ReturnAllUsersAtLevelSearch_ValidModel_ValidReturn_StandardUsers()
        {
            var model = new SearchAccounthWebViewModel
            {
                ReturnAll = false,
                FilterTerm = "Gmandam"
            };
            var controller = new API.Controllers.AccountController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            var retVal = controller.ReturnAllUsersAtLevelForSearch(model);
            var contentResult = retVal as OkNegotiatedContentResult<List<QueueUserWebViewModel>>;
            Assert.IsInstanceOfType(retVal, typeof(OkNegotiatedContentResult<List<QueueUserWebViewModel>>));
            Assert.IsTrue(contentResult.Content.Count == 1);
            Assert.IsTrue(!contentResult.Content.FirstOrDefault().IsAdmin);
            Assert.IsTrue(contentResult.Content.FirstOrDefault().UserName.Contains("Gmandam"));
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void DeleteUser_ValidModel_IsDeletingTrue()
        {
            var controller = CreateAndReturnController();
            var returnData = RegisterAndRetrieveUserGuidAsync();

            var retVal = controller.DeleteUser(new UserDeleteViewModel
            {
                Guid = returnData.ToString(),
                IsDeleting = true
            });
            DeleteUser(returnData);
            Assert.IsInstanceOfType(retVal, typeof(OkResult));
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void DeleteUser_ValidModel_IsDeletingFalse()
        {

            var controller = CreateAndReturnController();
            var returnData = RegisterAndRetrieveUserGuidAsync();
            var retVal = controller.DeleteUser(new UserDeleteViewModel
            {
                Guid = returnData.ToString(),
                IsDeleting = false
            });
            DeleteUser(returnData);
            Assert.IsInstanceOfType(retVal, typeof(OkResult));
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void DeleteUser_ValidModel_IsDeletingTrue_InvalidGuid()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var controller = CreateAndReturnController();

            var retVal = controller.DeleteUser(new UserDeleteViewModel
            {
                Guid = Guid.NewGuid().ToString(),
                IsDeleting = true
            });
            DeleteUser(userGuid);
            Assert.IsInstanceOfType(retVal, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public async Task AlterAccount_ValidModel_AlterUserNameAsync()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var controller = CreateAndReturnController();
            var model = new UserEditViewModel
            {
                UserName = "AlterTest",
                EmailAddress = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                Guid = userGuid.ToString(),
                PhoneNumber = "01685 371019"
            };
            var retVal = await controller.AlterAccountLoginDetails(model);

            Assert.IsInstanceOfType(retVal, typeof(OkResult));
            var db = new Database("AngularUsers");
            var returnData = db.Query<string>($"SELECT UserName FROM AspNetUsers WHERE Id = '{userGuid.ToString()}'").FirstOrDefault();
            DeleteUser(userGuid);
            Assert.AreEqual(model.UserName, returnData);
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public async Task AlterAccount_InvalidGuid_AlterUserNameAsync()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var controller = CreateAndReturnController();
            var model = new UserEditViewModel
            {
                UserName = "AlterTest",
                EmailAddress = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                Guid = Guid.NewGuid().ToString(),
                PhoneNumber = "01685 371019"
            };
            var retVal = await controller.AlterAccountLoginDetails(model);

            Assert.IsInstanceOfType(retVal, typeof(InvalidModelStateResult));
            var db = new Database("AngularUsers");
            var returnData = db.Query<string>($"SELECT UserName FROM AspNetUsers WHERE Id = '{userGuid.ToString()}'").FirstOrDefault();
            DeleteUser(userGuid);
            Assert.AreEqual("RegisterTest", returnData);
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void CloneUser_ValidOldUser_ValidNewUser()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var controller = CreateAndReturnController();
            var model = new CloneUserViewModel
            {
                UserName = "AlterTest",
                EmailAddress = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                OldUserGuid = userGuid.ToString(),
                PhoneNumber = "01685 371019"
            };

            var retVal = controller.CloneUser(model);
            Assert.IsInstanceOfType(retVal, typeof(OkResult));
            var db = new Database("AngularUsers");
            var orginalUser = db.Query<string>($"SELECT Id FROM AspNetUsers WHERE UserName = 'AlterTest'").FirstOrDefault();
            DeleteUser(userGuid);
            DeleteUser(new Guid(orginalUser));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(orginalUser));
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void CloneUser_InvalidOldUser_ValidNewUser()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var controller = CreateAndReturnController();
            var model = new CloneUserViewModel
            {
                UserName = "AlterTest",
                EmailAddress = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                OldUserGuid = Guid.NewGuid().ToString(),
                PhoneNumber = "01685 371019"
            };

            var retVal = controller.CloneUser(model);
            Assert.IsInstanceOfType(retVal, typeof(OkResult));
            var db = new Database("AngularUsers");
            var orginalUser = db.Query<string>($"SELECT Id FROM AspNetUsers WHERE UserName = 'AlterTest'").FirstOrDefault();
            DeleteUser(userGuid);
            DeleteUser(new Guid(orginalUser));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(orginalUser));
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void CloneUser_InvalidOldUser_MissingPassword()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var controller = CreateAndReturnController();
            var model = new CloneUserViewModel
            {
                UserName = "AlterTest",
                EmailAddress = "test@test.com",
                OldUserGuid = Guid.NewGuid().ToString(),
                PhoneNumber = "01685 371019"
            };

            var retVal = controller.CloneUser(model);
            DeleteUser(userGuid);
            Assert.IsInstanceOfType(retVal, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void CloneUser_InvalidOldUser_MissingUsername()
        {
            var userGuid = RegisterAndRetrieveUserGuidAsync();
            var controller = CreateAndReturnController();
            var model = new CloneUserViewModel
            {
                EmailAddress = "test@test.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                OldUserGuid = Guid.NewGuid().ToString(),
                PhoneNumber = "01685 371019"
            };

            var retVal = controller.CloneUser(model);
            DeleteUser(userGuid);
            Assert.IsInstanceOfType(retVal, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void ChangeUserType_ValidUser_FromAdmin_ToUser()
        {
            var model = new AccountPagingParameterWebViewModel
            {
                PageNumber = 1,
                PageSize = 5,
                ReturnAll = true,
                OrderBy = "Id"
            };
            var controller = CreateAndReturnController();
            var retVal = controller.ReturnAllUsers(model);
            var contentResult = (retVal as OkNegotiatedContentResult<PaginatedQueueUserResult>).Content;
            var user = contentResult.QueueDisplay.FirstOrDefault();
            var result = controller.ChangeUserType(new UserTypeViewModel()
            {
                Guid = user.Guid,
                IsAdmin = user.IsAdmin
            });
            Assert.IsInstanceOfType(result, typeof(OkResult));
            controller.ChangeUserType(new UserTypeViewModel()
            {
                Guid = user.Guid,
                IsAdmin = !user.IsAdmin
            });
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void ChangeUserType_ValidUser_FromUser_ToAdmin()
        {
            var model = new AccountPagingParameterWebViewModel
            {
                PageNumber = 1,
                PageSize = 5,
                ReturnAll = false,
                OrderBy = "Id"
            };
            var controller = CreateAndReturnController();
            var retVal = controller.ReturnAllUsers(model);
            var contentResult = (retVal as OkNegotiatedContentResult<PaginatedQueueUserResult>).Content;
            var user = contentResult.QueueDisplay.FirstOrDefault();
            var result = controller.ChangeUserType(new UserTypeViewModel()
            {
                Guid = user.Guid,
                IsAdmin = user.IsAdmin
            });
            Assert.IsInstanceOfType(result, typeof(OkResult));
            controller.ChangeUserType(new UserTypeViewModel()
            {
                Guid = user.Guid,
                IsAdmin = !user.IsAdmin
            });
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void ChangeUserType_InvalidUser_SuperAdmin()
        {
  
            var controller = CreateAndReturnController();
            var result = controller.ChangeUserType(new UserTypeViewModel()
            {
                Guid = "472bd65a-168d-431a-bfc6-7107cd3bfb15",
                IsAdmin = true
            });
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        [TestCategory("Controllers.UserControler")]
        public void ChangeUserType_InvalidUser_NoUserPresent()
        {

            var controller = CreateAndReturnController();
            var result = controller.ChangeUserType(new UserTypeViewModel()
            {
                Guid = Guid.NewGuid().ToString(),
                IsAdmin = true
            });
            Assert.IsInstanceOfType(result, typeof(ExceptionResult));
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

        private API.Controllers.AccountController CreateAndReturnController()
        {
            var controller = new API.Controllers.AccountController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            return controller;
        }

        private void DeleteUser(Guid returnData)
        {
            try
            {
                CreateAndReturnController().DeleteUser(new UserDeleteViewModel
                {
                    Guid = returnData.ToString(),
                    IsDeleting = true
                });
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}