'use strict';

angular.module('quoteTool.quoteselection', ['ngRoute'])

    .config(['$routeProvider', function ($routeProvider) {
        $routeProvider.when('/quoteselection', {
            templateUrl: 'quoteselection/quoteselection.html',
            controller: 'QuoteSelection'
        });
    }])

    .controller('QuoteSelection', [function () {
        $.material.init();
    }]);