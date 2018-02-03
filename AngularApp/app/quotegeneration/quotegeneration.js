"use strict";
//angular.module('CurrencyDirective', []);
angular.module("quoteTool.quotegeneration", ["ui.router", "ngAnimate", "CurrencyDirective", "ngInputCurrency"])
    .controller("QuoteGeneration", ["$scope", "$http", "$stateParams", function ($scope, $http, $stateParams) {
        //$.material.init();
        $scope.hasChangedQuoteDetails = false;
        $scope.parentID = $stateParams.parentID;
        $scope.quoteID = $stateParams.quoteID;
        $scope.quoteReference = $stateParams.quoteReference;

        $scope.submitForm = function () {
            $scope.submitData = {};
            $scope.submitData.QuotationCalculation = $scope.quotationResult;
            $scope.submitData.QuotationDetails = $scope.quotationDetails;
            $scope.submitData.ParentID = $scope.parentID;
            $scope.submitData.QuoteID = $scope.quoteID;
            $http.post("http://localhost:8080/api/Quote/SaveQuote", $scope.submitData)
                .then(function(response) {
                })
                .catch(function(error) {

                });
        };

        $scope.trackFormChange = function () {
            $scope.hasChangedQuoteDetails = false;
        }

        $scope.generateQuote = function () {
            //console.log($stateParams.parentID);
            $http.post("http://localhost:8080/api/Quote/CalculateQuote", $scope.quotationDetails)
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
                })
                .catch(function (error) {

                });
        }

        if ($scope.quoteReference !== undefined) {
            $scope.retrieveWithQuoteReference();
        }
    }]);