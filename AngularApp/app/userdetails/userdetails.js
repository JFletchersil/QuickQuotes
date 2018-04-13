"use strict";
/**
 * A collection of controllers designed to run and operate the user details screen
 * @module quoteTool.userdetails
 * @param ui.router Object The router object that controls how the system handles navigation requests
 * @param ngAnimate Object The animation object which provides Jquery animations in AngularJS Fashion
 */
angular.module("quoteTool.userdetails", ["ui.router", "ngAnimate"])
    /**
     * The main UserDetails controller that manages the user details screen
     * This controller is used to manage the user meta data such as their full name, the company they work for, as
     * well as other data that a user may wish to have but should not be present within the main user object
     * This traces all changes made, and will only allow a save to be performed if changes have been made
     * This controller can not delete user details, aside from blanking them, that is the responsibility
     * of the administration screen
     * Upon loading, this controller will immediately retrieve the details for a given user
     * @class UserDetails
     * @param $scope Object The local $scope of the controller
     * @param $http Object The $HTTP module, this allows us to make HTTP requests
     * @param UserService Object The UserService responsible for saving data for the user
     * @param __env JSON This stores environment values that the application will use
     */
    .controller("UserDetails", ["$scope", "$http", "UserService", "__env",
        function ($scope, $http, UserService, __env) {
            /**
             * A collection of the current user's meta data
             * @property $scope.userModel
             * @type {{}}
             */
            $scope.userModel = {};
            /**
             * Tracks if the user details have changed or not
             * If they have not changed, then you will not be able to save to the database
             * @property $scope.hasChangedUserDetails
             * @type {boolean}
             */
            $scope.hasChangedUserDetails = false;

            /**
             * Retrieves all of the relevant meta data details of a given user
             * @method getUserDetails
             */
            $scope.getUserDetails = function () {
                $http.post(__env.apiUrl + "/UserDetails/ReturnUserModel", UserService.getItem().fullName)
                    .then(function (response) {
                        $scope.userModel = response.data;
                    })
                    .catch(function (error) {

                    });
            };

            /**
             * Saves any changes made to the meta data of a given user
             * @method saveUserDetails
             */
            $scope.saveUserDetails = function() {
                $http.post(__env.apiUrl + "/Users/SaveUserModel", $scope.userModel)
                    .then(function (response) {
                    })
                    .catch(function (error) {

                    });
            };

            /**
             * Tracks the form to determine if any changes have been made, and as such, should prevent the ability
             * to save the quote without recalculating the quote again
             * @method trackFormChange
             */
            $scope.trackFormChange = function () {
                $scope.hasChangedUserDetails = true;
            };

            $scope.getUserDetails();
        }]);