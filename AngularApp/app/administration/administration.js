'use strict';

angular.module('quoteTool.administration', ['ui.router', 'ngAnimate'])

    //.config(['$routeProvider', function ($routeProvider) {
    //    $routeProvider.when('/administration', {
    //        templateUrl: 'administration/administration.html',
    //        controller: 'Administration'
    //    });
    //}])

    .controller('Administration', ['$scope', function ($scope) {
        $scope.showOptions = false;
        $scope.hideOptions = function () {
            $scope.showOptions = !$scope.showOptions;
        }
    }]);