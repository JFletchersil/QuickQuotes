"use strict";

angular.module("quoteTool.mainmenu", ["ui.router", "ngAnimate"])
    .controller("MainMenu", ["$scope", "UserService", function ($scope, UserService) {
        $scope.User = {
            "Username": UserService.getItem().fullName,
            "shortname": UserService.getItem().fullName
        };
    }]);