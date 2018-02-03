"use strict";

angular.module("quoteTool.Userdetails", ["ui.router", "ngAnimate"])
    .controller("UserDetails", ["$scope", "$http", "UserService", function ($scope, $http, UserService) {
        $scope.userModel = {};
        $scope.hasChangedUserDetails = false;

        $scope.getUserDetails = function () {
            $http.post("http://localhost:8080/api/Users/ReturnUserModel", UserService.getItem().fullName)
                .then(function (response) {
                    $scope.userModel = response.data;
                })
                .catch(function (error) {

                });
        }

        $scope.saveUserDetails = function() {
            $http.post("http://localhost:8080/api/Users/SaveUserModel", $scope.userModel)
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