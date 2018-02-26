"use strict";

angular.module("quoteTool.Userdetails", ["ui.router", "ngAnimate"])
    .controller("UserDetails", ["$scope", "$http", "UserService", "__env", function ($scope, $http, UserService, __env) {
        $scope.userModel = {};
        $scope.hasChangedUserDetails = false;

        $scope.getUserDetails = function () {
            $http.post(__env.apiUrl + "/Users/ReturnUserModel", UserService.getItem().fullName)
                .then(function (response) {
                    $scope.userModel = response.data;
                })
                .catch(function (error) {

                });
        }

        $scope.saveUserDetails = function() {
            $http.post(__env.apiUrl + "/Users/SaveUserModel", $scope.userModel)
                .then(function (response) {
                })
                .catch(function (error) {

                });
        }

        $scope.trackFormChange = function () {
            $scope.hasChangedUserDetails = true;
        }

        $scope.getUserDetails();
    }]);