'use strict';

angular.module('quoteTool.home', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/home', {
        templateUrl: 'home/home.html',
        controller: 'Home'
    });
}])

.controller('Home', [function () {

}]);