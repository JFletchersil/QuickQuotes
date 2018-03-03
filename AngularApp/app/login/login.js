"use strict";
angular.module("User", []);
angular.module("quoteTool.login", ["ui.router", "ngAnimate"])
    .controller("Login", ["$scope", "$rootScope", "$location", "AuthenticationService", "UserService", "__env",
        function ($scope, $rootScope, $location, AuthenticationService, UserService, __env) {
            // reset login status
            $scope.submitForm = function () {
                $scope.dataLoading = true;
                AuthenticationService.Login($scope.Username, $scope.password, function (response) {
                    if (response.status === 200) {
                        UserService.addItem({
                            userName: response.data.UserName,
                            fullName: response.data.FullName,
                            email: $scope.Username,
                            isSuperAdmin: response.data.IsSuperAdmin,
                            isAdmin: response.data.IsAdmin
                        });
                        AuthenticationService.SetCredentials($scope.Username, $scope.password);
                        $location.path("/home");
                    } else {
                        $scope.error = response.message;
                        $scope.dataLoading = false;
                    }
                });
            };
        }]);