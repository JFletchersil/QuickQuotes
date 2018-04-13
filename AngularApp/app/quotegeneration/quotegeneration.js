"use strict";
/**
 * A collection of controllers designed to manage the generic quotation generation page as well as making HTTP
 * requests.
 * @module quoteTool.quotegeneration
 * @param ui.router Object The router object that controls how the system handles navigation requests
 * @param ngAnimate Object The animation object which provides Jquery animations in AngularJS Fashion
 * @param CurrencyDirective Object The currency directive that formats the currency object such that it works correctly
 * @param ngInputCurrency Object The currency directive that creates the skeleton allowing currency elements to work
 */
angular.module("quoteTool.quotegeneration", ["ui.router", "ngAnimate", "CurrencyDirective", "ngInputCurrency"])
    /**
     * The main QuotationGeneration controller that manages the creation of generic elements and their submission to
     * the main API
     * This controller handles both editing old quotes and the creation of new quotes
     * The page is designed to be generic, it does not care what the quote type is, it will render it according the
     * definition given
     * @class QuoteGeneration
     * @param $scope Object The local $scope of the controller
     * @param $http Object The $HTTP module, this allows us to make HTTP requests
     * @param $stateParams Object Allows the controller to access the state parameters passed, used to differentiate between different states the controller needs to handle
     * @param __env JSON This stores environment values that the application will use
     */
    .controller("QuoteGeneration", ["$scope", "$http", "$stateParams", "__env",
        function ($scope, $http, $stateParams, __env) {
            /**
             * Monitors if the quote has had it's details changed or not
             * If true, it will prevent a quote from being saved
             * @property $scope.hasChangedQuoteDetails
             * @type {boolean}
             */
            $scope.hasChangedQuoteDetails = false;
            /**
             * The type of product that is the parent of the quote being used
             * @property $scope.parentID
             * @type {string}
             */
            $scope.parentID = $stateParams.parentID;
            /**
             * The type of quote currently being quoted
             * @property $scope.quoteID
             * @type {string}
             */
            $scope.quoteID = $stateParams.quoteID;
            /**
             * The quote reference of the quote that may be being edited
             * @property $scope.quoteReference
             * @type {string}
             */
            $scope.quoteReference = $stateParams.quoteReference;
            /**
             * The current collection of quotation details
             * @property $scope.quotationDetails
             * @type {{}}
             */
            $scope.quotationDetails = {};

            /**
             * Submits the current form to the API endpoint in order to save the current state of the quote
             * This can only be performed if all the validation is reporting that nothing is invalid
             * @method submitForm
             */
            $scope.submitForm = function () {
                $scope.submitData = {};
                $scope.submitData.QuotationCalculation = $scope.quotationResult;
                $scope.submitData.QuotationDetails = $scope.quotationDetails;
                $scope.submitData.ParentID = $scope.parentID;
                $scope.submitData.QuoteID = $scope.quoteID;
                $http.post(__env.apiUrl + "/Quote/SaveQuote", $scope.submitData)
                    .then(function (response) {
                        $scope.hasChangedQuoteDetails = false;
                        $scope.quotationDetails = {};
                        $scope.quotationResult = {};
                        $scope.submitData = {};
                        $scope.elements = {};
                        $scope.retrieveElementConfiguration();
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
                $scope.hasChangedQuoteDetails = false;
            };

            /**
             * Generates a new quote based on the current state of the quotation details
             * This quote takes the elements, and converts them into key,value pairs in order to generate a Json string
             * These elements, due to their generic nature, are not stored according to the quotationDetails object,
             * they must be manually placed within the object
             * This is further combined with the current type, established as the quoteID, and then this is sent to the API
             * The response is then parsed as the quotation result, or reported to the console as an error
             * @method generateQuote
             */
            $scope.generateQuote = function () {
                // Due to how the data is stored within each generic element, the values must be manually placed
                // as key value pairs into the quotation details object
                $scope.elements.forEach(function (element) {
                    $scope.quotationDetails[element.ElementName] = element.Value;
                });
                let quoteDets = $scope.quotationDetails;
                quoteDets.Type = $scope.quoteID;
                let quoteText = JSON.stringify(quoteDets);
                $http.post(__env.apiUrl + "/Quote/CalculateQuote", quoteText)
                    .then(function (response) {
                        $scope.quotationResult = response.data;
                        $scope.hasChangedQuoteDetails = true;
                    })
                    .catch(function (error) {
                        console.log(error)
                    });
            };

            /**
             * Retrieves a quote from the database when given a quote reference
             * This is only used when editing an existing quote via the queue screen, and is one of the alternate
             * states that the controller can be used in
             * Positive Response is a full retrieval of all valid information about the quote
             * Negative Response is printing the error to the console
             * @method retrieveWithQuoteReference
             */
            $scope.retrieveWithQuoteReference = function () {
                $http.post(__env.apiUrl + "/Quote/RetrieveWithQuoteReference", $scope.quoteReference)
                    .then(function (response) {
                        $scope.guidRetrevial = response.data;
                        $scope.quotationResult = $scope.guidRetrevial.QuotationCalculation;
                        $scope.quotationDetails = $scope.guidRetrevial.QuotationDetails;
                        $scope.parentID = $scope.guidRetrevial.ParentId;
                        $scope.quoteID = $scope.guidRetrevial.QuoteId;
                        $scope.retrieveElementConfiguration();
                    })
                    .catch(function (error) {
                        console.log(error)
                    });
            };

            /**
             * Retrieves the configuration of the page, based on the type of quote that is being used
             * This is run frequently due to the fact that it configures and generates the quote part of the page
             * This needs to be refreshed when saving quote details, it also needs to be refreshed after retrieving
             * valid quote data from the database
             * @method retrieveElementConfiguration
             */
            $scope.retrieveElementConfiguration = function() {
                $http.get(__env.apiUrl + "/Quote/GetElementConfiguration", { params: { quoteType: $scope.quoteID } })
                    .then(function (response) {
                        $scope.elements = response.data;
                        // Due to how the data is stored within each generic element, the values must be manually placed
                        // as key value pairs into the quotation details object. This process must be reversed when
                        // first retrieving the configuration values
                        $scope.elements.forEach(function (element) {
                            element.Value = $scope.quotationDetails[element.ElementName];
                        });
                    })
                    .catch(function (error) {

                    });
            };

            /**
             * Splits a string based on the Upper characters within the string
             * Put the string back together with spaces between the Upper characters
             * @method splitOnUpper
             * @param string string The string that is being split
             * @return {*} A string split based on Upper characters and then put spaces between them
             */
            $scope.splitOnUpper = function (string) {
                if (string === undefined) {
                    return "";
                } else {
                    return string.split(/(?=[A-Z])/).join(" ");
                }
            };

            if ($scope.quoteReference !== undefined || $scope.quoteID !== undefined) {
                $scope.retrieveWithQuoteReference();
            } else{
                console.log("Unable to determine quote type")
            }
        }]);