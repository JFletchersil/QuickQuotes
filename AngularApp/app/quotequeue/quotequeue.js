"use strict";

angular.module("quoteTool.quotequeue", ["ui.router", "ngAnimate", "ngMaterial", "md.data.table"])
    .controller("QuoteQueue", ["$scope", "$http", "$filter", "$location", "$stateParams", "__env", function ($scope, $http, $filter, $location, $stateParams, __env) {
        $scope.referenceId = $stateParams.quoteReference;

        $scope.selected = [];
        $scope.quotes = [];
        $scope.oldItems = [];
        $scope.searchData = [];

        $scope.pagingModel = {
            PageNumber: 1,
            PageSize: 5
        };

        $scope.filter = {
            options: {
                debounce: 500
            },
            hasFiltered: false
        };

        $scope.query = {
            order: "QuoteReference",
            limit: $scope.pagingModel.PageSize,
            page: $scope.pagingModel.PageNumber
        };

        $scope.TotalSize = 0;
        $scope.TotalItems = 0;

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

        $scope.generalSearch = function (searchTerm) {
            var searchModel = {
                SearchText: searchTerm,
                ResultNumber: __env.baseSearch
            }
            $http.post(__env.apiUrl + "/Queue/SearchQueueSearchResults", searchModel).
                then(function (response) {
                    $scope.quotes = response.data;
                });
        }

        $scope.pageChangeHandler = function (newPageNumber) {
            if (!isNaN(newPageNumber)) {
                $scope.pagingModel.PageNumber = newPageNumber;
            }
            $scope.promise =
                $http.post(__env.apiUrl + "/Queue/ShowPaginatedQuotes", $scope.pagingModel).
                    then(function (response) {
                        success(response.data);
                    });

        }

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
        };

        $scope.filterItems = function () {
            if ($scope.oldItems.length === 0) {
                $scope.oldItems = $scope.quotes;
                $scope.quotes = $filter('filter')($scope.quotes, $scope.query.filter)
                $scope.filter.hasFiltered = true;
            } else {
                $scope.quotes = $scope.oldItems;
                $scope.quotes = $filter('filter')($scope.quotes, $scope.query.filter)
                $scope.filter.hasFiltered = true;
            }

            if ($scope.quotes.length === 0) {
                $scope.generalSearch($scope.query.filter);
            }
        };

        $scope.moveToQuote = function () {
            $location.path("/quoteretrieval/" + $scope.selected[0].QuoteType + "/" + $scope.selected[0].QuoteReference);
        }

        $scope.splitOnUpper = function (string) {
            return string.split(/(?=[A-Z])/).join(" ");
        }
        $scope.pageChangeHandler(1);
    }]);