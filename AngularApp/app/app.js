'use strict';

// Declare app level module which depends on views, and components
angular.module('quoteTool', [
    'ui.router',
    'ngAnimate',
    'ngMaterial',
    'Authentication',
    'User',
    'ngCookies',
    'quoteTool.login',
    'quoteTool.mainmenu',
    'quoteTool.home',
    'quoteTool.quotequeue',
    'quoteTool.quoteselection',
    'quoteTool.quotegeneration',
    'quoteTool.version',
    'quoteTool.administration',
    'quoteTool.userdetails'
])
.config(['$locationProvider', '$stateProvider', '$urlRouterProvider', function ($locationProvider, $stateProvider, $urlRouterProvider) {
    $locationProvider.hashPrefix('!');
    $urlRouterProvider.otherwise('/login');
    $stateProvider
        .state('login', {
            url: '/login',
            templateUrl: '../login/login.html',
            controller: 'Login'
        })
        .state('mainmenu', {
            abstract: true,
            url: '/home',
            templateUrl: '../mainmenu/mainmenu.html',
            controller: 'MainMenu',
            redirectTo: "mainmenu.home"
        })
        .state('mainmenu.home', {
            url: '',
            templateUrl: '../home/home.html',
            controller: 'Home'
        })
        .state('mainmenu.quotequeue', {
            url: '^/quotequeue',
            templateUrl: '../quotequeue/quotequeue.html',
            controller: 'QuoteQueue'
        });
}]);