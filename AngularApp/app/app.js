"use strict";
// Declare app level module which depends on views, and components
var expireDate = new Date();
// this will set the expiration to 1 day;
expireDate.setDate(expireDate.getDate() + 1);
//angular.module('Authentication', []);
angular.module("quoteTool", [
    "ui.router",
    "ngStorage",
    "ngAnimate",
    "ngMaterial",
    "md.data.table",
    "ngCookies",
    "angularModalService",
    "GInputDirective",
    "User",
    //'Authentication',
    "quoteTool.login",
    "quoteTool.mainmenu",
    "quoteTool.home",
    "quoteTool.quotequeue",
    "quoteTool.quoteselection",
    "quoteTool.quotegeneration",
    "quoteTool.version",
    "quoteTool.administration",
    "quoteTool.adminmodal",
    "quoteTool.Userdetails"
])
    .config(["$locationProvider", "$stateProvider", "$urlRouterProvider", function ($locationProvider, $stateProvider, $urlRouterProvider) {
        $locationProvider.hashPrefix("!");
        $urlRouterProvider.otherwise("/login");
        $stateProvider
            .state("login", {
                url: "/login",
                templateUrl: "../login/login.html",
                controller: "Login"
            })
            .state("mainmenu", {
                abstract: true,
                url: "/home",
                templateUrl: "../mainmenu/mainmenu.html",
                controller: "MainMenu",
                redirectTo: "mainmenu.home"
            })
            .state("mainmenu.home", {
                url: "",
                templateUrl: "../home/home.html",
                controller: "Home"
            })
            .state("mainmenu.quotequeue", {
                url: "^/quotequeue",
                templateUrl: "../quotequeue/quotequeue.html",
                controller: "QuoteQueue"
            })
            .state("mainmenu.quoteselection", {
                url: "^/quoteselection",
                templateUrl: "../quoteselection/quoteselection.html",
                controller: "QuoteSelection"
            })
            .state("mainmenu.quotegeneration", {
                url: "^/hirepurchase/{parentID}/{quoteID}",
                templateUrl: "../quotegeneration/quotegeneration.html",
                controller: "QuoteGeneration"
            })
            .state("mainmenu.quoteedit",
            {
                url: "^/hirepurchase/{quoteReference}",
                templateUrl: "../quotegeneration/quotegeneration.html",
                controller: "QuoteGeneration"
            })
            .state("mainmenu.userdetails",
            {
                url: "^/userdetails",
                templateUrl: "../userdetails/userdetails.html",
                controller: "UserDetails"
            })
            .state("mainmenu.administration",
            {
                url: "^/administration",
                templateUrl: "../administration/administration.html",
                controller: "Administration"
            });
    }])
    .run(["$rootScope", "$location", "$cookies", "$http",
        function ($rootScope, $location, $cookies, $http) {
            // keep User logged in after page refresh
            $rootScope.globals = $cookies.getObject("globals") || {};
            if ($rootScope.globals.currentUser) {
                $http.defaults.headers.common["Authorization"] = "Basic " + $rootScope.globals.currentUser.authdata; // jshint ignore:line
                $rootScope.hasValidated = true;
            }

            $rootScope.$on("$locationChangeStart", function (event, next, current) {
                // redirect to login page if not logged in
                if ($location.path() !== "/login" && !($rootScope.globals.currentUser || $rootScope.hasValidated)) {
                    $location.path("/login");
                }
            });
        }]);