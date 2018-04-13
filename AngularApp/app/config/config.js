"use strict";
/**
 * A collection of controllers designed to run and operate the configuration system for the application
 * @module quoteTool.applicationconfiguration
 * @param ui.router Object The router object that controls how the system handles navigation requests
 * @param ngAnimate Object The animation object which provides Jquery animations in AngularJS Fashion
 */
angular.module("quoteTool.applicationconfiguration", ["ui.router", "ngAnimate"])
    /**
     * The main controller that handles the configuration of the application
     * This controller is responsible for handling the four different configuration types it is responsible for
     * It can edit, delete, add to these types as needed
     * It includes a search feature, like the other tables within the application
     * This is one of the most complex controllers in the application, due to the fact that it is partially generic
     * like the quote generation controller
     * This controller also tracks all changes made to the data, and will not allow save operations unless the state
     * has changed
     * @class AppConfiguration
     * @param $scope Object The local $scope of the controller
     * @param $http Object The $HTTP module, this allows us to make HTTP requests
     * @param $filter Object The $filter module, this allows filters to be placed on text
     * @param UserService Object The UserService responsible for saving data for the user
     * @param __env JSON This stores environment values that the application will use
     */
    .controller("AppConfiguration", ["$scope", "$http", "$filter", "UserService", "__env",
        function ($scope, $http, $filter, UserService, __env) {
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
             * The query sent to the database when querying for configuration types
             * @property $scope.query
             * @type {{order: string, limit: number, page: number}}
             */
            $scope.query = {
                limit: $scope.pagingModel.PageSize,
                page: $scope.pagingModel.PageNumber
            };

            /**
             * The tabs to be shown to the user, generated based on the current tables within the database
             * This describes how the tables operate as well as what the datatypes and validations, if any, work
             * In short, this is vital, do not touch it unless you have altered the backing database tables
             * @property $scope.tabs
             * @type {*[]}
             */
            $scope.tabs = [{
                    Label: "Quote Defaults",
                    OnSelect: "QuoteDefaults",
                    QueryOrder: "ElementDescription",
                    Columns: {
                        ElementOne: {
                            Name: "TypeID",
                            Binding: "configData.TypeID",
                            Type: "text",
                            Disabled: true
                        },
                        ElementTwo: {
                            Name: "ElementDescription",
                            Binding: "configData.ElementDescription",
                            Type: "textarea"
                        },
                        ElementThree: {
                            Name: "XMLTemplate",
                            Binding: "configData.XMLTemplate",
                            Type: "textarea"
                        },
                        ElementFour: {
                            Name: "TotalRepayableTemplate",
                            Binding: "configData.TotalRepayableTemplate",
                            Type: "textarea"
                        },
                        ElementFive: {
                            Name: "MonthlyRepayableTemplate",
                            Binding: "configData.MonthlyRepayableTemplate",
                            Type: "textarea"
                        },
                        ElementSix: {
                            Name: "Enabled",
                            Binding: "configData.Enabled",
                            Type: "checkbox"
                        }
                    },
                    Disabled: false
                },
                {
                    Label: "Quote Statuses",
                    OnSelect: "QuoteStatuses",
                    QueryOrder: "State",
                    Columns: {
                        ElementOne: {
                            Name: "State",
                            Binding: "configData.State",
                            Type: "text"
                        },
                        ElementTwo: {
                            Name: "Enabled",
                            Binding: "configData.Enabled",
                            Type: "checkbox"
                        }
                    },
                    Disabled: false
                },
                {
                    Label: "Quote Types",
                    OnSelect: "QuoteTypes",
                    QueryOrder: "QuoteType",
                    Columns: {
                        ElementOne: {
                            Name: "TypeID",
                            Binding: "configData.TypeID",
                            Type: "text",
                            Disabled: true
                        },
                        ElementTwo: {
                            Name: "QuoteType",
                            Binding: "configData.QuoteType",
                            Type: "text"
                        },
                        ElementThree: {
                            Name: "ProductParentID",
                            Binding: "configData.ProductParentID",
                            Type: "text"
                        },
                        ElementFour: {
                            Name: "Enabled",
                            Binding: "configData.Enabled",
                            Type: "checkbox"
                        }
                    },
                    Disabled: false
                },
                {
                    Label: "Product Types",
                    OnSelect: "ProductTypes",
                    QueryOrder: "ProductType",
                    Columns: {
                        ElementOne: {
                            Name: "ProductType",
                            Binding: "configData.ProductType",
                            Type: "text"
                        },
                        ElementTwo: {
                            Name: "Enabled",
                            Binding: "configData.Enabled",
                            Type: "checkbox"
                        }
                    },
                    Disabled: false
                }
            ];

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
             * The old items prior to starting a search via the search filter
             * @property $scope.oldItems
             * @type {Array}
             */
            $scope.oldItems = [];
            /**
             * Tracks any changes made to the table it is looking at, and how the new record or removed record works
             * @property $scope.stateTrack
             * @type {Array}
             */
            $scope.stateTrack = [];

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
             * Tracks if a change or update has been to the table or not
             * @property $scope.hasNewItemOrUpdate
             * @type {boolean}
             */
            $scope.hasNewItemOrUpdate = false;

            /**
             * Successes the specified return data wrapped inside a promise.
             * This is used when making requests to the database, and is used to evaluate
             * how long it will take before a promise resolves.
             * @method success
             * @return {*|Object} Updates the total number of users, the total number of paged users, and provides a list of users
             * @param returnData Object The return data.
             */
            function success(returnData) {
                $scope.TotalSize = returnData.TotalPages;
                $scope.TotalItems = returnData.TotalItems;
                $scope.configData = returnData.ConfigResult;
                $scope.stateTrack = angular.copy(returnData.ConfigResult);
            }

            /**
             * Changes the current database page that the user of the application is using
             * @method pageChangeHandler
             * @param newPageNumber number The new page number that the user wishes to view
             * @param configurationType string The configuration type that is being looked at
             * @param newLimit number The size of the paging to be used on the server
             * @return {*|Object} A new list of users who would be on the current page
             */
            $scope.pageChangeHandler = function (newPageNumber, configurationType, newLimit) {
                if (isNaN(newPageNumber))
                    $scope.pagingModel.OrderBy = newPageNumber;
                if (configurationType !== null && configurationType !== undefined)
                    $scope.pagingModel.ConfigurationType = configurationType;
                if (configurationType !== null && configurationType !== undefined && !isNaN(newLimit))
                    $scope.pagingModel.PageSize = newLimit;

                $scope.promise =
                    $http.post(__env.apiUrl + "/Configuration/ReturnDefaultConfigurations", $scope.pagingModel).
                        then(function (response) {
                            success(response.data);
                        });
            };

            /**
             * Performs a search within the database for the given search term
             * @method generalSearch
             * @param searchTerm string The search term
             * @return {*|Object} A list of users who match the search term
             */
            $scope.generalSearch = function (searchTerm) {
                let model = {
                    ConfigType: $scope.pagingModel.ConfigurationType,
                    FilterText: searchTerm
                };
                $http.post(__env.apiUrl + "/Configuration/SearchDefaultConfigurations", model).
                    then(function (response) {
                        $scope.configData = response.data;
                    });
            };

            /**
             * Saves any updated values or new configuration options to the database
             * @method saveUpdatedValues
             * @param configType string The table that has updated values
             */
            $scope.saveUpdatedValues = function (configType) {
                let updateModel = { ConfigType: configType, ConfigsToBeSaved: $scope.configData };
                $http.post(__env.apiUrl + "/Configuration/SaveDefaultConfigurations", updateModel)
                    .then(function (response) {
                        $scope.hasNewItemOrUpdate = false;
                        console.log(response);
                    })
                    .catch(function (error) {
                        console.log(error);
                        alert("Item Save Failed, Please Check the Item and Try Again");
                    });
            };

            /**
             * Tracks if a row within the table has been changed or not
             * @method onChange
             * @param data Object The configuration object that has been changed or altered
             * @param index number The index of the configuration object that has been changed or altered
             */
            $scope.onChange = function (data, index) {
                let newData = {};

                Object.keys(data).forEach(function (key) {
                    if (data[key] !== undefined && key !== "newOrModified") {
                        newData[key] = data[key];
                    }
                    if (key === "Enabled") {
                        newData[key] = (data[key].toUpperCase() === 'TRUE');
                    }
                });

                let oldData = $scope.stateTrack[index];
                data.newOrModified = !angular.equals(oldData, newData);

                $scope.hasNewItemOrUpdate = !(angular.equals(oldData, newData) && $scope.stateTrack.length === $scope.configData.length);
            };

            /**
             * Adds a new item to the configData, allowing for a configuration item to be created
             * @method addItem
             */
            $scope.addItem = function () {
                $scope.configData.push({ newOrModified: true });
                $scope.hasNewItemOrUpdate = true;
            };

            /**
             * Allows the user to change the tab, if the user has multiple tabs in which
             * they can move between
             * @method onTabSelected
             * @param tabSelected number The tab number that is being moved towards
             * @param configurationType string The configuration type that is currently being viewed
             */
            $scope.onTabSelected = function (tabSelected, configurationType) {
                $scope.stateTrack = [];
                $scope.pageChangeHandler(1, configurationType);
                if ($scope.hasNewItemOrUpdate) {
                    $scope.hasNewItemOrUpdate = false;
                }
            };

            /**
             * Changes the item limit and page number
             * @method onItemLimitChange
             * @param newPageNumber int The new page number
             * @param newLimit int The new configuration object limit
             */
            $scope.onItemLimitChange = function (newPageNumber, newLimit) {
                $scope.pageChangeHandler(newPageNumber, undefined, newLimit);
            };

            /**
             * Resets the configuration list, and removes the filter that the user was searching for
             * within the application
             * @method removeFilter
             * @return {*|Object} The normal state of the application prior to searching for configurations
             */
            $scope.removeFilter = function () {
                if ($scope.filter.hasFiltered) {
                    $scope.filter.show = false;
                    $scope.query.filter = '';

                    if ($scope.filter.form.$dirty) {
                        $scope.filter.form.$setPristine();
                    }
                    $scope.configData = $scope.oldItems;
                    $scope.oldItems = [];
                } else {
                    $scope.filter.show = false;
                }

                $scope.tabs.forEach(function (element) {
                    if (!($scope.pagingModel.ConfigurationType === element.OnSelect)) {
                        element.Disabled = false;
                    }
                });

                $scope.filter.hasFiltered = false;
            };

            /**
             * Filters out the current list of configurations for the filter search text
             * If it cannot find it present within the current list of users, it will
             * search the database
             * @method filterItems
             * @return {*|Object} A list of users that matches the current filter search text
             */
            $scope.filterItems = function () {
                if ($scope.oldItems.length === 0) {
                    $scope.oldItems = $scope.configData;
                    $scope.configData = $filter('filter')($scope.configData, $scope.query.filter)
                } else {
                    $scope.configData = $scope.oldItems;
                    $scope.configData = $filter('filter')($scope.configData, $scope.query.filter)
                }
                if ($scope.configData.length === 0) {
                    $scope.generalSearch($scope.query.filter);
                }
            };

            /**
             * Shows the filter location for the user to enter his search text
             * Takes into account which tab is currently being searched
             * @method activateFilter
             * @return {*|Object} A visible search box for the user to use
             */
            $scope.activateFilter = function () {
                $scope.filter.show = true;
                $scope.tabs.forEach(function (element) {
                    if (!($scope.pagingModel.ConfigurationType === element.OnSelect)) {
                        element.Disabled = true;
                    }
                });
                $scope.filter.hasFiltered = true;
            }
        }]);