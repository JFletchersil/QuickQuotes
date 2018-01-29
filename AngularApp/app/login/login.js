'use strict';
angular.module('Authentication', []);
angular.module('User', []);
angular.module('quoteTool.login', ['ui.router', 'ngAnimate'])
    .controller('Login', ['$scope', '$rootScope', '$location', 'AuthenticationService', 'UserService',
        function ($scope, $rootScope, $location, AuthenticationService, UserService) {
            // reset login status
            AuthenticationService.ClearCredentials();
            $scope.submitForm = function () {
                $scope.dataLoading = true;
                AuthenticationService.Login($scope.username, $scope.password, function (response) {
                    if (response.status === 200) {
                        UserService.addItem({ email: $scope.username });
                        AuthenticationService.SetCredentials($scope.username, $scope.password);
                        $location.path('/home');
                    } else {
                        $scope.error = response.message;
                        $scope.dataLoading = false;
                    }
                });
            };
        }]);