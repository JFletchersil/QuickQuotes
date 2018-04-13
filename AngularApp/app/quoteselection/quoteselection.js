"use strict";
/**
 * A collection of controllers designed to run and operate the product and quote selection screen
 * @module quoteTool.quoteselection
 * @param ui.router {Object} The router object that controls how the system handles navigation requests
 * @param ngAnimate {Object} The animation object which provides Jquery animations in AngularJS Fashion
 */
angular.module("quoteTool.quoteselection", ["ui.router", "ngAnimate"])
    /**
     * The main QuoteSelection controller that manages the quote selection screen
     * If the Product Types are not present within local storage, this controller will make a call
     * to retrieve the product types and their children quote types in order to place them on the HTML screen
     * If they are present, then these quote types and product types are placed directly on the HTML Screen
     * This page is used to select the quote type that the user would like to make a quote for, it performs no
     * other operation
     * @class QuoteSelection
     * @param $scope {Object} The local $scope of the controller
     * @param UserService {Object} The UserService responsible for saving data for the user
     * @param $http {Object} The $HTTP module, this allows us to make HTTP requests
     * @param __env {JSON} This stores environment values that the application will use
     */
    .controller("QuoteSelection", ["$scope", "$http", "UserService", "__env",
        function ($scope, $http, UserService, __env) {
            /**
             * A list of product types from the database
             * @property $scope.productTypes
             * @type {{}}
             */
            $scope.productTypes = {};
            /**
             * A list of quote types from the database
             * @property $scope.quoteTypes
             * @type {{}}
             */
            $scope.quoteTypes = {};
            if (UserService.getItem().quoteTypeData === undefined) {
                $http.get(__env.apiUrl + "/ProductQuoteType/GetAllQuoteTypes")
                    .then(function(response) {
                        $scope.quoteTypes = response.data;
                        UserService.addItem({ quoteTypeData: $scope.quoteTypes });
                        $scope.hasChangedQuoteDetails = true;
                    })
                    .catch(function(error) {

                    });
            } else {
                $scope.quoteTypes = UserService.getItem().quoteTypeData;
            }

            /**
             * Removes all spaces within a string
             * @method noSpaces
             * @param string {string} The string to have all of it's spaces removed
             * @return {string} A string without spaces in it
             */
            $scope.noSpaces = function(string) {
                return string.replace(/ /g, "");
            };

            /**
             * Splits a string based on the Upper characters within the string
             * Put the string back together with spaces between the Upper characters
             * @method splitOnUpper
             * @param string {string} The string that is being split
             * @return {string} A string split based on Upper characters and then put spaces between them
             */
            $scope.splitOnUpper = function (string) {
                return string.split(/(?=[A-Z])/).join(" ");
            }
        }]);