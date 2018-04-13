"use strict";
/**
 * A collection of controllers designed to manage and control the quote queue screen
 * @module quoteTool.quotequeue
 * @param ui.router Object The router object that controls how the system handles navigation requests
 * @param ngAnimate Object The animation object which provides Jquery animations in AngularJS Fashion
 * @param ngMaterial Object The object that allows interaction with the Material Design of the page in an AngularJS Fashion
 * @param md.data.table Object The object that allows JQuery DataTables to be constructed within the scope of the module
 */
angular.module("quoteTool.quotequeue", ["ui.router", "ngAnimate", "ngMaterial", "md.data.table"])
    /**
     * The main QuoteQueue controller that manages the quote quote screen
     * This controller handles the retrieval of all quotes within the database, as well as defining the size
     * of the database pages and the number of items per page
     * The page is capable of navigating to the quote generation page in order to allow a user to edit an old quote
     * The page also provides a more in depth quote search functionality than is present within the main menu
     * @class QuoteQueue
     * @param $scope Object The local $scope of the controller
     * @param $http Object The $HTTP module, this allows us to make HTTP requests
     * @param $filter Object The $filter module, this allows filters to be placed on text
     * @param $location Object Allows the queue screen to redirect users if they require a more in depth search of the database
     * @param $stateParams Object Allows the controller to set state parameters as well as act when it receives state parameters that alter how it acts
     * @param __env JSON This stores environment values that the application will use
     */
    .controller("QuoteQueue", ["$scope", "$http", "$filter", "$location", "$stateParams", "__env",
        function ($scope, $http, $filter, $location, $stateParams, __env) {

            /**
             * The quote reference that may be passed to the quote queue
             * @property $scope.referenceId
             * @type {string}
             */
            $scope.referenceId = $stateParams.quoteReference;

            /**
             * The currently selected quotes
             * @property $scope.selected
             * @type {Array}
             */
            $scope.selected = [];
            /**
             * The list of quotes currently visible within the table
             * @property $scope.quotes
             * @type {Array}
             */
            $scope.quotes = [];
            /**
             * The old items prior to starting a search via the search filter
             * @property $scope.oldItems
             * @type {Array}
             */
            $scope.oldItems = [];
            /**
             * The current search data within the queue table
             * @property $scope.searchData
             * @type {Array}
             */
            $scope.searchData = [];
            /**
             * The paging model used to page the table
             * @property $scope.pagingModel
             * @type {{PageNumber: number, PageSize: number}}
             */
            $scope.pagingModel = {
                PageNumber: 1,
                PageSize: 5
            };

            /**
             * The filter for the table
             * @property $scope.filter
             * @type {{options: {debounce: number}, hasFiltered: boolean}}
             */
            $scope.filter = {
                options: {
                    debounce: 500
                },
                hasFiltered: false
            };

            /**
             * The query sent to the database when querying for users
             * @property $scope.query
             * @type {{order: string, limit: number, page: number}}
             */
            $scope.query = {
                order: "QuoteReference",
                limit: $scope.pagingModel.PageSize,
                page: $scope.pagingModel.PageNumber
            };

            /**
             * The total number of pages within the database at the current paging size
             * @property $scope.TotalSize
             * @type {number}
             */
            $scope.TotalSize = 0;

            /**
             * The total number of users within the database
             * @property $scope.TotalItems
             * @type {number}
             */
            $scope.TotalItems = 0;

            /**
             * Successes the specified return data wrapped inside a promise.
             * This is used when making requests to the database, and is used to evaluate
             * how long it will take before a promise resolves.
             * Forces a reset of the filter system if the reference ID is present, allowing a direct search when being
             * moved from the global search on the main menu
             * @method success
             * @return {*|Object} Updates the total number of quote, the total number of paged quote, and provides a list of quote
             * @param returnData Object The return data.
             */
            function success(returnData) {
                $scope.TotalSize = returnData.TotalPages;
                $scope.TotalItems = returnData.TotalItems;
                $scope.quotes = returnData.QueueDisplay;
                if ($scope.referenceId !== undefined) {
                    $scope.filter.show = true;
                    $scope.query.filter = $scope.referenceId;
                    $scope.filterItems();
                }
            }

            /**
             * Performs a search within the database for the given search term
             * @method generalSearch
             * @param searchTerm string The search term
             * @return {*|Object} A list of users who match the search term
             */
            $scope.generalSearch = function (searchTerm) {
                $http.post(__env.apiUrl + "/Queue/SearchQueueSearchResults", searchTerm).
                    then(function (response) {
                        $scope.quotes = response.data;
                    });
            };

            /**
             * Changes the current database page that the user of the application is using
             * @method pageChangeHandler
             * @param newPageNumber number The new page number that the user wishes to view
             * @return {*|Object} A new list of users who would be on the current page
             */
            $scope.pageChangeHandler = function (newPageNumber) {
                if (isNaN(newPageNumber)) {
                    $scope.pagingModel.OrderBy = newPageNumber;
                }
                $scope.promise =
                    $http.post(__env.apiUrl + "/Queue/ShowPaginatedQuotes", $scope.pagingModel).
                        then(function (response) {
                            success(response.data);
                        });

            };

            /**
             * Resets the quotes list, and removes the filter that the user was searching for
             * within the application
             * @method removeFilter
             * @return {*|Object} The normal state of the application prior to searching for quotes
             */
            $scope.removeFilter = function () {
                if ($scope.filter.hasFiltered) {
                    $scope.filter.show = false;
                    $scope.query.filter = '';

                    if ($scope.filter.form.$dirty) {
                        $scope.filter.form.$setPristine();
                    }
                    $scope.quotes = $scope.oldItems;
                    $scope.oldItems = [];
                } else {
                    $scope.filter.show = false;
                }

                $scope.filter.hasFiltered = false;
            };

            /**
             * Filters out the current list of quotes for the filter search text
             * If it cannot find it present within the current list of quotes, it will
             * search the database
             * @method filterItems
             * @return {*|Object} A list of quotes that matches the current filter search text
             */
            $scope.filterItems = function () {
                if ($scope.oldItems.length === 0) {
                    $scope.oldItems = $scope.quotes;
                    $scope.quotes = $filter('filter')($scope.quotes, $scope.query.filter)
                } else {
                    $scope.quotes = $scope.oldItems;
                    $scope.quotes = $filter('filter')($scope.quotes, $scope.query.filter)
                }

                if ($scope.quotes.length === 0) {
                    $scope.generalSearch($scope.query.filter);
                }
            };

            /**
             * Shows the filter location for the user to enter his search text
             * Simpler than other filters, as it does not need to lock different tabs down
             * @method activateFilter
             * @return {*|Object} A visible search box for the user to use
             */
            $scope.activateFilter = function () {
                $scope.filter.show = true;
                $scope.filter.hasFiltered = true;
            };

            /**
             * When clicking on a given quote, the user is moved to the quote generation page with the given parameters
             * These state parameters inform the application that it should attempt to retrieve the quote, rather than
             * attempting to generate a new quote
             * @method moveToQuote
             */
            $scope.moveToQuote = function () {
                $location.path("/quoteretrieval/" + $scope.selected[0].QuoteType + "/" + $scope.selected[0].QuoteReference);
            };

            /**
             * Splits a string based on the Upper characters within the string
             * Put the string back together with spaces between the Upper characters
             * @method splitOnUpper
             * @param string string The string that is being split
             * @return {*} A string split based on Upper characters and then put spaces between them
             */
            $scope.splitOnUpper = function (string) {
                return string.split(/(?=[A-Z])/).join(" ");
            };

            $scope.pageChangeHandler(1);
        }]);