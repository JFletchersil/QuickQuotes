'use strict';

angular.module('quoteTool.quotegeneration', ['ui.router', 'ngAnimate'])
    .controller('QuoteGeneration', ['$scope', function ($scope) {
        //$.material.init();
        $scope.submitForm = function () {
            alert("Fuck the World");
        };
    }]);