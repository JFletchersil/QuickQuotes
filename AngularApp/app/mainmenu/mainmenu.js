'use strict';

angular.module('quoteTool.mainmenu', ['ui.router', 'ngAnimate'])
    .controller('MainMenu', ['$scope', 'UserService', function ($scope, UserService) {
        $scope.user = {
            "username": UserService.getItem().shortname,
            "shortname": UserService.getItem().shortname
        };
    }]);