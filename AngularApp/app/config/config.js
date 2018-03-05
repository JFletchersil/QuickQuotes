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
                        Type: "TextArea"
                    }, 
                    ElementTwo: {
                        Name: "XMLTemplate",
                        Type: "TextArea"
                    },
                    ElementThree: {
                        Name: "TotalRepayableTemplate",
                        Type: "TextArea"
                    }, 
                    ElementFour: {
                        Name: "MonthlyRepayableTemplate",
                        Type: "TextArea"
                    }, 
                    ElementFive: {
                        Name: "Enabled",
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
                        Type: "Text"
                    },
                    ElementTwo: {
                        Name: "Enabled",
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
                        Type: "Text"
                    },
                    ElementTwo: {
                        Name: "ProductParentID",
                        Type: "Text"
                    },
                    ElementThree: {
                        Name: "Enabled",
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
                        Type: "Text"
                    },
                    ElementTwo: {
                        Name: "Enabled",
                        Type: "Checkbox"
                    }
                }
            }
        }

        $scope.selected = {};
        $scope.TotalSize = 0;
        $scope.TotalItems = 0;

        function success(returnData) {
            $scope.TotalSize = returnData.TotalPages;
            $scope.TotalItems = returnData.TotalItems;
            $scope.configData = returnData.QueueDisplay;
        }

        $scope.pageChangeHandler = function (newPageNumber, configurationType) {
            $scope.pagingModel.PageNumber = newPageNumber;
            //$scope.promise =
            //    $http.post(__env.apiUrl + "/Configuration/" + configurationType, $scope.pagingModel).
            //        then(function (response) {
            //            success(response.data);
            //        });
        }

        $scope.onTabSelected = function (tabSelected, configurationType) {
            $scope.pagingModel.ReturnAll = tabSelected === 0;
            $scope.pageChangeHandler(1, configurationType);
        }
    }]);