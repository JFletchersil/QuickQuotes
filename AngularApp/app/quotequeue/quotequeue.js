'use strict';

angular.module('quoteTool.quotequeue', ['ngRoute', 'angularUtils.directives.dirPagination'])

    .config(['$routeProvider', function ($routeProvider) {
        $routeProvider.when('/quotequeue', {
            templateUrl: 'quotequeue/quotequeue.html',
            controller: 'QuoteQueue'
        });
    }])
    
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
        //$scope.options = {
        //    myFunction: function () { console.log('hello'); }
        //};
        //$scope.columns = [
        //    {
        //        id: 'foo',
        //        key: 'foo',
        //        classes: '',
        //        lockWidth: true
        //    },
        //    {
        //        id: 'bar',
        //        key: 'bar',
        //        classes: '',
        //        lockWidth: true
        //    }
        //];
        //$scope.rows = [{foo:"10", bar: "20"}]
    }]);