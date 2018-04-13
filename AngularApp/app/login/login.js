"use strict";
/**
 * Initialises the User Module so that it's UserService begins to run when the user attempts to access the login screen
 * @module User
 */
angular.module("User", []);
/**
 * A collection of controllers designed to run and operate the log in screen of the application
 * @module quoteTool.login
 * @param ui.router {Object} The router object that controls how the system handles navigation requests
 * @param ngAnimate {Object} The animation object which provides Jquery animations in AngularJS Fashion
 */
angular.module("quoteTool.login", ["ui.router", "ngAnimate"])
    /**
     * The controller that gathers log in data and performs the initial login.
     * This controller also sets the basic user information so that the User will remain logged into the system
     * and will not be kicked out of the system.
     * The controller also sets the access rights of the user, based on what the model received tells the application
     * @class Login
     * @param $scope {Object} The local $scope of the controller
     * @param $rootScope {Object} The $rootScope of the application as a whole
     * @param $location {Object} Allows the login form to redirect users if their login process was successful
     * @param AuthenticationService {Object} The Authentication service that does the majority of the grunt work in logging a user into the system
     * @param UserService {Object} The UserService responsible for saving data for the user
     */
    .controller("Login", ["$scope", "$rootScope", "$location", "AuthenticationService", "UserService",
        function ($scope, $rootScope, $location, AuthenticationService, UserService) {
            // reset login status
            /**
             * Logs the user in, if the submission form is valid
             * Sets the user details upon receiving a valid login return value, as well as setting the user access level
             * Logs why an error occurred if an error occurred
             * Moves the user to the home screen if the log in attempt as successful
             * @method submitForm
             */
            $scope.submitForm = function () {
                $scope.dataLoading = true;
                AuthenticationService.Login($scope.Username, $scope.password, function (response) {
                    if (response.status === 200) {
                        UserService.addItem({
                            userName: response.data.UserName,
                            fullName: response.data.FullName,
                            email: $scope.Username,
                            isSuperAdmin: response.data.IsSuperAdmin,
                            isAdmin: response.data.IsAdmin
                        });
                        AuthenticationService.SetCredentials($scope.Username, $scope.password);
                        $location.path("/home");
                    } else {
                        $scope.error = response.message;
                        $scope.dataLoading = false;
                    }
                });
            };
        }]);