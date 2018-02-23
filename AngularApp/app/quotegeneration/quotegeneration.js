"use strict";
angular.module("quoteTool.quotegeneration", ["ui.router", "ngAnimate", "CurrencyDirective", "ngInputCurrency"])
    .controller("QuoteGeneration", ["$scope", "$http", "$stateParams", function ($scope, $http, $stateParams) {
        $scope.hasChangedQuoteDetails = false;
        $scope.parentID = $stateParams.parentID;
        $scope.quoteID = $stateParams.quoteID;
        $scope.quoteReference = $stateParams.quoteReference;
        $scope.quotationDetails = {};

        $scope.submitForm = function () {
            $scope.submitData = {};
            $scope.submitData.QuotationCalculation = $scope.quotationResult;
            $scope.submitData.QuotationDetails = JSON.stringify($scope.quotationDetails);
            $scope.submitData.ParentID = $scope.parentID;
            $scope.submitData.QuoteID = $scope.quoteID;
            $http.post("http://localhost:8080/api/Quote/SaveQuote", $scope.submitData)
                .then(function (response) {
                })
                .catch(function (error) {

                });
        };

        $scope.trackFormChange = function () {
            $scope.hasChangedQuoteDetails = false;
        }

        $scope.generateQuote = function () {
            $scope.elements.forEach(function (element) {
                $scope.quotationDetails[element.ElementName] = element.Value;
            });
            var quoteDets = $scope.quotationDetails;
            quoteDets.Type = $scope.quoteID;
            var quoteText = JSON.stringify(quoteDets);
            $http.post("http://localhost:8080/api/Quote/CalculateQuote", quoteText)
                .then(function (response) {
                    $scope.quotationResult = response.data;
                    $scope.hasChangedQuoteDetails = true;
                })
                .catch(function (error) {

                });
        }

        $scope.retrieveWithQuoteReference = function () {
            $http.post("http://localhost:8080/api/Quote/RetrieveWithQuoteReference", $scope.quoteReference)
                .then(function (response) {
                    $scope.guidRetrevial = response.data;
                    $scope.quotationResult = $scope.guidRetrevial.QuotationCalculation;
                    $scope.quotationDetails = $scope.guidRetrevial.QuotationDetails;
                    $scope.parentID = $scope.guidRetrevial.ParentId;
                    $scope.quoteID = $scope.guidRetrevial.QuoteId;
                    $scope.retrieveElementConfiguration();
                })
                .catch(function (error) {

                });
        }

        $scope.retrieveElementConfiguration = function() {
            $http.get("http://localhost:8080/api/Quote/GetElementConfiguration", { params: { quoteType: $scope.quoteID } })
                .then(function (response) {
                    $scope.elements = response.data;
                    $scope.elements.forEach(function (element) {
                        element.Value = $scope.quotationDetails[element.ElementName];
                    });
                })
                .catch(function (error) {

                });
        }

        $scope.splitOnUpper = function (string) {
            if (string === undefined) {
                return "";
            } else {
                return string.split(/(?=[A-Z])/).join(" ");
            }
        }

        if ($scope.quoteReference !== undefined) {
            $scope.retrieveWithQuoteReference();
        }

        if ($scope.quoteID !== undefined) {
            $scope.retrieveElementConfiguration();
        }
    }]);