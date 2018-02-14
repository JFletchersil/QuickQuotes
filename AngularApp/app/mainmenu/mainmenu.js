"use strict";

angular.module("quoteTool.mainmenu", ["ui.router", "ngAnimate"])
    .controller("MainMenu", ["$scope", "UserService", "AuthenticationService", "$location", function ($scope, UserService, AuthenticationService, $location) {
        $scope.User = {
            "userName": UserService.getItem().userName,
            "shortName": UserService.getItem().fullName,
            "isSuperAdmin": UserService.getItem().isSuperAdmin,
            "isAdmin": UserService.getItem().isAdmin
        };

        $scope.logOut = function() {
            AuthenticationService.ClearCredentials();
        };
    }]);