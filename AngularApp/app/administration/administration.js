"use strict";

angular.module("quoteTool.administration", ["ui.router", "ngAnimate"])
    .controller("Administration", ["$scope", function ($scope) {
        $scope.showOptions = false;
        $scope.hideOptions = function () {
            $scope.showOptions = !$scope.showOptions;
        }
    }]);