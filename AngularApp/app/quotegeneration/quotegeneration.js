'use strict';

angular.module('quoteTool.quotegeneration', ['ngRoute'])

    .config(['$routeProvider', function ($routeProvider) {
        $routeProvider.when('/quotegeneration', {
            templateUrl: 'quotegeneration/quotegeneration.html',
            controller: 'QuoteGeneration'
        });
    }])

    .controller('QuoteGeneration', [function () {
        $.material.init();
    }]);