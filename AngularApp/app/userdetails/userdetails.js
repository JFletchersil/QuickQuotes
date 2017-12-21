'use strict';

angular.module('quoteTool.userdetails', ['ngRoute'])

    .config(['$routeProvider', function ($routeProvider) {
        $routeProvider.when('/userdetails', {
            templateUrl: 'userdetails/userdetails.html',
            controller: 'UserDetails'
        });
    }])

    .controller('UserDetails', [function () {

    }]);