"use strict";

angular.module("quoteTool.quotequeue", ["ui.router", "ngAnimate", "ngMaterial", "md.data.table"])    
    .controller("QuoteQueue", ["$scope", "$http", "$location", "__env", function ($scope, $http, $location, __env) {
        $scope.selected = [];
        $scope.quotes = [];

        $scope.pagingModel = {
            PageNumber: 1,
            PageSize: 5
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
        }

        $scope.pageChangeHandler = function (newPageNumber) {
            $scope.pagingModel.PageNumber = newPageNumber;
            $scope.promise =
                $http.post(__env.apiUrl + "/Queue/ShowPaginatedQuotes", $scope.pagingModel).
                then(function(response) {
                    success(response.data);
                });
        }

        $scope.moveToQuote = function () {
            $location.path("/hirepurchase/" + $scope.selected[0].QuoteReference);
        }

        $scope.splitOnUpper = function (string) {
            return string.split(/(?=[A-Z])/).join(" ");
        }
        $scope.pageChangeHandler(1);
    }]);