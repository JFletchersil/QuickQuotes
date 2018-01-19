'use strict';

angular.module('quoteTool.quotequeue', ['ui.router', 'ngAnimate', 'angularUtils.directives.dirPagination'])

    //.config(['$routeProvider', function ($routeProvider) {
    //    $routeProvider.when('/quotequeue', {
    //        templateUrl: 'quotequeue/quotequeue.html',
    //        controller: 'QuoteQueue'
    //    });
    //}])
    
    .controller('QuoteQueue', ['$scope', function ($scope) {
        //$.material.init();
        $scope.currentPage = 1;
        $scope.quotes = [
            {ReferenceNumber: "Test One", Type: "Test Two", Status: "Test Three", Age: "Test Four", Author: "Test Five" },
            {ReferenceNumber: "Test One", Type: "Test Two", Status: "Test Three", Age: "Test Four", Author: "Test Five" },
            { ReferenceNumber: "Test One", Type: "Test Two", Status: "Test Three", Age: "Test Four", Author: "Test Five" },
            { ReferenceNumber: "Test One", Type: "Test Two", Status: "Test Three", Age: "Test Four", Author: "Test Five" },
            { ReferenceNumber: "Test One", Type: "Test Two", Status: "Test Three", Age: "Test Four", Author: "Test Five" },
            { ReferenceNumber: "Test One", Type: "Test Two", Status: "Test Three", Age: "Test Four", Author: "Test Five" },
            { ReferenceNumber: "Test One", Type: "Test Two", Status: "Test Three", Age: "Test Four", Author: "Test Five" },
            { ReferenceNumber: "Test One", Type: "Test Two", Status: "Test Three", Age: "Test Four", Author: "Test Five" },
            { ReferenceNumber: "Test One", Type: "Test Two", Status: "Test Three", Age: "Test Four", Author: "Test Five" },
            { ReferenceNumber: "Test One", Type: "Test Two", Status: "Test Three", Age: "Test Four", Author: "Test Five" },
            { ReferenceNumber: "Test One", Type: "Test Two", Status: "Test Three", Age: "Test Four", Author: "Test Five" }
        ]
    }]);