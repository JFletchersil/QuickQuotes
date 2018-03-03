"use strict";

angular.module("quoteTool.mainmenu", ["ui.router", "ngAnimate"])
    .controller("MainMenu", ["$scope", "UserService", "AuthenticationService", "$location", "__env", "$http",
        function ($scope, UserService, AuthenticationService, $location, __env, $http) {
            $scope.searchText = "";
            $scope.User = {
                "userName": UserService.getItem().userName,
                "shortName": UserService.getItem().fullName,
                "isSuperAdmin": UserService.getItem().isSuperAdmin,
                "isAdmin": UserService.getItem().isAdmin
            };

            $scope.querySearch = function () {
                return $http.post(__env.apiUrl + "/Queue/ReturnSearchResults", $scope.searchText)
                    .then(function (data) {
                        return data.data;
                    })
                    .catch(function (error) {
                        return data.data;
                    });
            };

            $scope.logOut = function () {
                UserService.setItem() = {};
                AuthenticationService.ClearCredentials();
            };
        }]);