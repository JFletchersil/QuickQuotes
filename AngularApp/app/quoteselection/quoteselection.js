"use strict";

angular.module("quoteTool.quoteselection", ["ui.router", "ngAnimate"])
    .controller("QuoteSelection", ["$scope", "$http", "UserService", function ($scope, $http, UserService) {
        $scope.productTypes = {};
        $scope.quoteTypes = {};
        if (UserService.getItem().quoteTypeData === undefined) {
            $http.get("http://localhost:8080/api/ProductQuoteType/GetAllQuoteTypes")
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

        $scope.noSpaces = function(string) {
            return string.replace(/ /g, "");
        };

        $scope.splitOnUpper = function (string) {
            return string.split(/(?=[A-Z])/).join(" ");
        }
    }]);