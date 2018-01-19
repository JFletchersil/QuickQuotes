'use strict';

angular.module('quoteTool.mainmenu', ['ui.router', 'ngAnimate'])

    //.config(['$routeProvider', function ($routeProvider) {
    //    $routeProvider.when('/mainmenu', {
    //        templateUrl: 'mainmenu/mainmenu.html',
    //        controller: 'MainMenu'
    //    });
    //}])
    //.config(['$locationProvider', '$stateProvider', '$urlRouterProvider', function ($locationProvider, $stateProvider, $urlRouterProvider) {
    //    $locationProvider.hashPrefix('!');
    //    //$urlRouterProvider.otherwise('/login');

    //    //$routeProvider.otherwise({ redirectTo: '/login' });
    //    //var loginState = {
    //    //    name: 'login',
    //    //    url: '/login',
    //    //    template: 'quoteTool.login'
    //    //}
    //    //    $routeProvider.when('/quotequeue', {
    //    //        templateUrl: 'quotequeue/quotequeue.html',
    //    //        controller: 'QuoteQueue'
    //    $stateProvider
    //        .state('mainmenu.home', {
    //            url: '/home',
    //            templateUrl: '../home/home.html',
    //            controller: 'Home'
    //        })
    //        .state('mainmenu.quotequeue', {
    //            url: '/quotequeue',
    //            templateUrl: '../quotequeue/quotequeue.html',
    //            controller: 'QuoteQueue'
    //        });
    //}])
    .controller('MainMenu', ['$scope', function ($scope) {
        $scope.user = {
            "username": "Joshua Fletcher",
            "shortname": ""
        };
    }]);