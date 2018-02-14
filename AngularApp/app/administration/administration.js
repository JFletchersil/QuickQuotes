"use strict";

angular.module("quoteTool.administration", ["ui.router", "ngAnimate", "ngMaterial", "md.data.table"])
    .controller("Administration", ["$scope", "$http", "UserService", function ($scope, $http, UserService) {
        $scope.pagingModel = {
            PageNumber: 1,
            PageSize: 5
        };

        $scope.query = {
            order: "UserName",
            limit: $scope.pagingModel.PageSize,
            page: $scope.pagingModel.PageNumber
        };

        $scope.newUserModel = {
            IsAdministrator: false
        };

        $scope.showOptions = false;
        $scope.isSuperAdmin = UserService.getItem().isSuperAdmin;
        $scope.isAdmin = UserService.getItem().isAdmin;
        $scope.users = [];

        $scope.TotalSize = 0;
        $scope.TotalItems = 0;

        function success(returnData) {
            $scope.TotalSize = returnData.TotalPages;
            $scope.TotalItems = returnData.TotalItems;
            $scope.users = returnData.QueueDisplay;
        }

        $scope.pageChangeHandler = function (newPageNumber) {
            $scope.pagingModel.PageNumber = newPageNumber;
            $scope.promise =
                $http.post("http://localhost:8080/api/Account/ReturnAllUsers", $scope.pagingModel).
                    then(function (response) {
                        success(response.data);
                    });
        }

        $scope.createNewUser = function () {
            $http.post("http://localhost:8080/api/Account/Register", $scope.newUserModel).then(
                function (response) {
                    $scope.newUserModel = {
                        IsAdministrator: false
                    }
                })
                .catch(function (error) {

                });
        }

        $scope.hideOptions = function () {
            $scope.showOptions = !$scope.showOptions;
        }

        $scope.onTabSelected = function (tabSelected) {
            $scope.pagingModel.ReturnAll = tabSelected === 0;
            $scope.pageChangeHandler(1);
        }
    }]);