"use strict";

angular.module("quoteTool.applicationconfiguration", ["ui.router", "ngAnimate"])
    .controller("AppConfiguration", ["$scope", "$http", "$filter", "UserService", "__env", function ($scope, $http, $filter, UserService, __env) {
        $scope.pagingModel = {
            PageNumber: 1,
            PageSize: 5
        };

        $scope.query = {
            limit: $scope.pagingModel.PageSize,
            page: $scope.pagingModel.PageNumber
        };

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
        ]

        $scope.filter = {
            options: {
                debounce: 500
            },
            hasFiltered: false
        };

        $scope.oldItems = [];
        $scope.stateTrack = [];

        $scope.TotalSize = 0;
        $scope.TotalItems = 0;
        $scope.configurationValueDetails = 0;
        $scope.hasNewItemOrUpdate = false;

        function success(returnData) {
            $scope.TotalSize = returnData.TotalPages;
            $scope.TotalItems = returnData.TotalItems;
            $scope.configData = returnData.ConfigResult;
            $scope.stateTrack = angular.copy(returnData.ConfigResult);
        }

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
        }

        $scope.generalSearch = function (searchTerm) {
            var model = {
                ConfigType: $scope.pagingModel.ConfigurationType,
                FilterText: searchTerm
            };
            $http.post(__env.apiUrl + "/Configuration/SearchDefaultConfigurations", model).
                then(function (response) {
                    $scope.configData = response.data;
                });
        }

        $scope.saveUpdatedValues = function (configType) {
            var updateModel = { ConfigType: configType, ConfigsToBeSaved: $scope.configData }
            $http.post(__env.apiUrl + "/Configuration/SaveDefaultConfigurations", updateModel)
                .then(function (response) {
                    $scope.hasNewItemOrUpdate = false;
                    console.log(response);
                })
                .catch(function (error) {
                    alert("Item Save Failed, Please Check the Item and Try Again");
                });
        }

        $scope.onChange = function (data, index) {
            var newData = {};

            Object.keys(data).forEach(function (key) {
                if (data[key] !== undefined && key !== "newOrModified") {
                    newData[key] = data[key];
                }
                if (key === "Enabled") {
                    newData[key] = (data[key].toUpperCase() === 'TRUE');
                }
            });

            var oldData = $scope.stateTrack[index];
            if (angular.equals(oldData, newData)) {
                data.newOrModified = false;
            } else {
                data.newOrModified = true;
            }

            if (angular.equals(oldData, newData) && $scope.stateTrack.length === $scope.configData.length) {
                $scope.hasNewItemOrUpdate = false;
            } else {
                $scope.hasNewItemOrUpdate = true;
            }
        }

        $scope.addItem = function () {
            $scope.configData.push({ newOrModified: true });
            $scope.hasNewItemOrUpdate = true;
        }

        $scope.onTabSelected = function (tabSelected, configurationType) {
            $scope.stateTrack = [];
            $scope.pageChangeHandler(1, configurationType);
            if ($scope.hasNewItemOrUpdate) {
                $scope.hasNewItemOrUpdate = false;
            }
        }

        $scope.onItemLimitChange = function (newPageNumber, newLimit) {
            $scope.pageChangeHandler(newPageNumber, undefined, newLimit);
        }

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