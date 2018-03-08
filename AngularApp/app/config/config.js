"use strict";

angular.module("quoteTool.applicationconfiguration", ["ui.router", "ngAnimate"])
    .controller("AppConfiguration", ["$scope", "$http", "UserService", "__env", function ($scope, $http, UserService, __env) {
        $scope.pagingModel = {
            PageNumber: 1,
            PageSize: 5
        };

        $scope.query = {
            limit: $scope.pagingModel.PageSize,
            page: $scope.pagingModel.PageNumber
        };

        $scope.tabs = {
            QuoteDefaults:
            {
                Label: "Quote Defaults",
                OnSelect: "QuoteDefaults",
                QueryOrder: "ElementDescription",
                Columns: {
                    ElementOne: {
                        Name: "ElementDescription",
                        Binding: "configData.ElementDescription",
                        Type: "textarea"
                    }, 
                    ElementTwo: {
                        Name: "XMLTemplate",
                        Binding: "configData.XMLTemplate",
                        Type: "textarea"
                    },
                    ElementThree: {
                        Name: "TotalRepayableTemplate",
                        Binding: "configData.TotalRepayableTemplate",
                        Type: "textarea"
                    }, 
                    ElementFour: {
                        Name: "MonthlyRepayableTemplate",
                        Binding: "configData.MonthlyRepayableTemplate",
                        Type: "textarea"
                    }, 
                    ElementFive: {
                        Name: "Enabled",
                        Binding: "configData.Enabled",
                        Type: "Checkbox"
                    }
                }
            },
            QuoteStatuses:
            {
                Label: "Quote Statuses",
                OnSelect: "QuoteStatuses",
                QueryOrder: "State",
                Columns: {
                    ElementOne: {
                        Name: "State",
                        Binding: "configData.State",
                        Type: "Text"
                    },
                    ElementTwo: {
                        Name: "Enabled",
                        Binding: "configData.Enabled",
                        Type: "Checkbox"
                    }
                }
            },
            QuoteTypes:
            {
                Label: "Quote Types",
                OnSelect: "QuoteTypes",
                QueryOrder: "QuoteType",
                Columns: {
                    ElementOne: {
                        Name: "QuoteType",
                        Binding: "configData.QuoteType",
                        Type: "Text"
                    },
                    ElementTwo: {
                        Name: "ProductParentID",
                        Binding: "configData.ProductParentID",
                        Type: "Text"
                    },
                    ElementThree: {
                        Name: "Enabled",
                        Binding: "configData.Enabled",
                        Type: "Checkbox"
                    }
                }
            },
            ProductTypes:
            {
                Label: "Product Types",
                OnSelect: "ProductTypes",
                QueryOrder: "ProductType",
                Columns: {
                    ElementOne: {
                        Name: "ProductType",
                        Binding: "configData.ProductType",
                        Type: "Text"
                    },
                    ElementTwo: {
                        Name: "Enabled",
                        Binding: "configData.Enabled",
                        Type: "Checkbox"
                    }
                }
            }
        }

        $scope.selected = {};
        $scope.TotalSize = 0;
        $scope.TotalItems = 0;
        $scope.configurationValueDetails = 0;

        function success(returnData) {
            $scope.TotalSize = returnData.TotalPages;
            $scope.TotalItems = returnData.TotalItems;
            $scope.configData = returnData.ConfigResult;
            //$scope.configData.forEach(function (element) {
            //    element.Value = $scope.selected[element.ElementName];
            //});
        }

        $scope.pageChangeHandler = function (newPageNumber, configurationType) {
            $scope.pagingModel.PageNumber = newPageNumber;
            $scope.pagingModel.ConfigurationType = configurationType;
            $scope.promise =
                $http.post(__env.apiUrl + "/Configuration/DefaultConfigurations", $scope.pagingModel).
                    then(function (response) {
                        success(response.data);
                    });
        }

        $scope.onTabSelected = function (tabSelected, configurationType) {
            $scope.pagingModel.ReturnAll = tabSelected === 0;
            $scope.pageChangeHandler(1, configurationType);
        }
    }]);