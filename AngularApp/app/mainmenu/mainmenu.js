"use strict";
/**
 * A collection of controllers designed to help operate and control the MainMenu
 * @module quoteTool.mainmenu
 * @param ui.router Object The router object that controls how the system handles navigation requests
 * @param ngAnimate Object The animation object which provides Jquery animations in AngularJS Fashion
 */
angular.module("quoteTool.mainmenu", ["ui.router", "ngAnimate"])
    /**
     * The main controller responsible for controlling the main menu
     * This controller manages the global quote system, as well as the main menu and the log out button
     * The controller has only a few functions and is relatively lightweight
     * @class MainMenu
     * @param $scope Object The local $scope of the controller
     * @param UserService Object The UserService responsible for saving data for the user
     * @param AuthenticationService Object The Authentication service that does the majority of the grunt work in logging a user into the system
     * @param $location Object Allows the main menu to redirect users if they require a more in depth search of the database
     * @param __env JSON This stores environment values that the application will use
     * @param $http Object The $HTTP module, this allows us to make HTTP requests
     */
    .controller("MainMenu", ["$scope", "UserService", "AuthenticationService", "$location", "__env", "$http",
        function ($scope, UserService, AuthenticationService, $location, __env, $http) {
            /**
             * The search/filter text to be used when using the global search
             * @property $scope.searchText
             * @type {string}
             */
            $scope.searchText = "";
            /**
             * A model of the current user who has logged in
             * Includes their user name, short name, and access rights
             * @property $scope.User
             * @type {{userName: *, shortName: (string|{type: string}|fullName|{type}|*|string), isSuperAdmin: string, isAdmin: string}}
             */
            $scope.User = {
                "userName": UserService.getItem().userName,
                "shortName": UserService.getItem().fullName,
                "isSuperAdmin": UserService.getItem().isSuperAdmin,
                "isAdmin": UserService.getItem().isAdmin
            };
            /**
             * Performs a database query search when the user gives the main menu a search term
             * This searches the quote database for quotes, it will only search quotes
             * @method querySearch
             * @return {*} Either the data needed in order to perform searches, or a 500 error response explaining why the server could not finish the request
             */
            $scope.querySearch = function () {
                let searchModel = {
                    SearchText: $scope.searchText,
                    ResultNumber: __env.baseSearch
                };
                return $http.post(__env.apiUrl + "/Queue/ReturnSearchResults", searchModel)
                    .then(function (data) {
                        data.data[data.data.length - 1].hideLast = true;
                        return data.data;
                    })
                    .catch(function (error) {
                        return error;
                    });
            };

            /**
             * Performs the location change if the quotes provided by the search function did not contain the
             * required quote
             * This will redirect the user to the main queue page, including either the quote reference in order to
             * interact with the quotes, or the search text if none of the items were the correct quote
             * @method selectedItemChanged
             * @param item Object The search item selected when viewing the search items
             */
            $scope.selectedItemChanged = function (item) {
                if (item !== undefined) {
                    if (item.QuoteReference === "ExecuteOrder66") {
                        $location.path("/quotequeue/" + $scope.searchText);
                        $scope.searchText = "";
                    } else {
                        $location.path("/quotequeue/" + item.QuoteReference);
                    }
                }
            };

            /**
             * Logs a user out of the application
             * @method logOut
             */
            $scope.logOut = function () {
                UserService.addItem({});
                AuthenticationService.ClearCredentials();
            };
        }]);