'use strict';

// Declare app level module which depends on views, and components
angular.module('quoteTool', [
    'ngRoute',
    'quoteTool.home',
    'quoteTool.quotequeue',
    'quoteTool.quoteselection',
    'quoteTool.version'
])
.config(['$locationProvider', '$routeProvider', function ($locationProvider, $routeProvider) {
    $locationProvider.hashPrefix('!');
    $routeProvider.otherwise({ redirectTo: '/home' });
}])
.controller('MainMenu', ['$scope', function ($scope) {
    $scope.user = {
        "username": "Joshua Fletcher",
        "shortname":""
    };
}]);