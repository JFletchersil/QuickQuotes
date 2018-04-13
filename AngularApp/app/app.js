"use strict";
var env = {};
if (window) {
    Object.assign(env, window.__env);
}
// Declare app level module which depends on views, and components
var expireDate = new Date();
// this will set the expiration to 1 day;
expireDate.setDate(expireDate.getDate() + 1);
/**
 * Defines the main module for the entire angular app, all other modules are loaded in here
 * There are 23 different modules present within the application, the majority of them are of my own creation
 * and have been documented elsewhere
 * @module quoteTool
 */
angular.module("quoteTool", [
    "ui.router",
    "ngStorage",
    "ngAnimate",
    "ngMaterial",
    "md.data.table",
    "ngCookies",
    "angularModalService",
    "prettyXml",
    "monospaced.elastic",
    "GInputDirective",
    "GConfigDirective",
    "MinMaxDirective",
    "User",
    "quoteTool.login",
    "quoteTool.mainmenu",
    "quoteTool.home",
    "quoteTool.quotequeue",
    "quoteTool.quoteselection",
    "quoteTool.quotegeneration",
    "quoteTool.version",
    "quoteTool.administration",
    "quoteTool.adminmodal",
    "quoteTool.userdetails",
    "quoteTool.applicationconfiguration"
])
    .constant("__env", env)
    /**
     * Configures the state of the application, as well setting the basic hash prefix
     * There are multiple different states possible, some pages have multiple states they can be in,
     * others only have one state
     * These states have been documented within their associated modules, please see the other modules for the
     * documentation
     * @class config
     */
    .config(["$locationProvider", "$stateProvider", "$urlRouterProvider",
        function ($locationProvider, $stateProvider, $urlRouterProvider) {
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
                .state("mainmenu.quotesearch", {
                    url: "^/quotequeue/{quoteReference}",
                    templateUrl: "../quotequeue/quotequeue.html",
                    controller: "QuoteQueue"
                })
                .state("mainmenu.quoteselection", {
                    url: "^/quoteselection",
                    templateUrl: "../quoteselection/quoteselection.html",
                    controller: "QuoteSelection"
                })
                .state("mainmenu.quotegeneration", {
                    url: "^/{parentID}/{quoteID}",
                    templateUrl: "../quotegeneration/quotegeneration.html",
                    controller: "QuoteGeneration"
                })
                .state("mainmenu.quoteedit", {
                    url: "^/quoteretrieval/{quoteType}/{quoteReference}",
                    templateUrl: "../quotegeneration/quotegeneration.html",
                    controller: "QuoteGeneration"
                })
                .state("mainmenu.userdetails", {
                    url: "^/userdetails",
                    templateUrl: "../userdetails/userdetails.html",
                    controller: "UserDetails"
                })
                .state("mainmenu.administration", {
                    url: "^/administration",
                    templateUrl: "../administration/administration.html",
                    controller: "Administration"
                })
                .state("mainmenu.applicationconfiguration", {
                    url: "^/configuration",
                    templateUrl: "../config/config.html",
                    controller: "AppConfiguration"
                });
        }])
    /**
     * Configures the basic operation of the application
     * These are items that must be run when the application starts, this can be thought of as AngularJS's main
     * method, and should be kept as clean as possible
     * The majority of this code is meant to evaluate if the user has a valid login token, if they have a valid
     * token then they will be allowed into the application and automatically redirected
     * If they are not authorised to enter the app, they will be refused entry and be redirected back to the login screen
     * @class run
     * @param $rootScope Object The $rootScope of the application as a whole
     * @param $location Object Allows the login form to redirect users if their login process was successful
     * @param $cookies Object An AngularJS object designed to allow easy interaction with web browser cookies
     * @param $http Object The $HTTP module, this allows us to make HTTP requests
     */
    .run(["$rootScope", "$location", "$cookies", "$http",
        function ($rootScope, $location, $cookies, $http) {
            // keep User logged in after page refresh
            $rootScope.globals = $cookies.getObject("globals") || {};
            if ($rootScope.globals.currentUser) {
                $http.defaults.headers.common["Authorization"] = "Basic "
                    + $rootScope.globals.currentUser.authdata; // jshint ignore:line
                $rootScope.hasValidated = true;
            }

            $rootScope.$on("$locationChangeStart", function (event, next, current) {
                // redirect to login page if not logged in
                if ($location.path() !== "/login" && !($rootScope.globals.currentUser || $rootScope.hasValidated)) {
                    $location.path("/login");
                }
            });
        }]);