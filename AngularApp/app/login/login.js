'use strict';
angular.module('Authentication', []);
angular.module('quoteTool.login', ['ui.router', 'ngAnimate'])

    //.config(['$routeProvider', function ($routeProvider) {
    //    //$routeProvider.when('/login', {
    //    //    templateUrl: 'login/login.html',
    //    //    controller: 'Login'
    //    //});
    //}])

    .controller('Login', ['$scope', '$rootScope', '$location', 'AuthenticationService',
        function ($scope, $rootScope, $location, AuthenticationService) {
        // reset login status
        AuthenticationService.ClearCredentials();
        $scope.submitForm = function () {
            $scope.dataLoading = true;
            AuthenticationService.Login($scope.username, $scope.password, function (response) {
                if (response.success) {
                    AuthenticationService.SetCredentials($scope.username, $scope.password);
                    $location.path('/home');
                } else {
                    $scope.error = response.message;
                    $scope.dataLoading = false;
                }
            });
        };
    }]);