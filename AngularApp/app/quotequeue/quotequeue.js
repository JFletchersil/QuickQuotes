'use strict';

angular.module('quoteTool.quotequeue', ['ngRoute'])

    .config(['$routeProvider', function ($routeProvider) {
        $routeProvider.when('/quotequeue', {
            templateUrl: 'quotequeue/quotequeue.html',
            controller: 'QuoteQueue'
        });
    }])

    .controller('QuoteQueue', [function () {

    }]);