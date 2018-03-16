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
                var searchModel = {
                    SearchText: $scope.searchText,
                    ResultNumber: __env.baseSearch
                }
                return $http.post(__env.apiUrl + "/Queue/ReturnSearchResults", searchModel)
                    .then(function (data) {
                        data.data[data.data.length - 1].hideLast = true;
                        return data.data;
                    })
                    .catch(function (error) {
                        return data.data;
                    });
            };

            $scope.selectedItemChanged = function (item) {
                if (item !== undefined) {
                    if (item.QuoteReference === "ExecuteOrder66") {
                        $location.path("/quotequeue/" + $scope.searchText);
                        $scope.searchText = "";
                    } else {
                        $location.path("/quotequeue/" + item.QuoteReference);
                    }
                }
            }

            $scope.logOut = function () {
                UserService.setItem() = {};
                AuthenticationService.ClearCredentials();
            };
        }]);