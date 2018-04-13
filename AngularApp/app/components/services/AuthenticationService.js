"use strict";
/**
 * A collection of factories designed to make the login system easier and simpler by centralising code into one
 * specific area
 * @module quoteTool.login
 */
angular.module("quoteTool.login")
    /**
     * Provides a centralised Authentication service allowing the application to log into the system
     * using a single access point
     * @class AuthenticationService
     * @param Base64 {Object} Allows for obfuscation and encryption of the login token
     * @param $http {Object} The $HTTP module, this allows us to make HTTP requests
     * @param $cookies {Object} An AngularJS object designed to allow easy interaction with web browser cookies
     * @param $rootScope {Object} The root scope of the AngularJS application
     * @param $timeout {Object} An AngularJS object designed to allow easy checking of the time for timeout purposes
     * @param __env JSON This stores environment values that the application will use
     */
    .factory("AuthenticationService", ["Base64", "$http", "$cookies", "$rootScope", "$timeout", "__env",
        function (Base64, $http, $cookies, $rootScope, $timeout, __env) {
            /**
             * A collection of functions present within the factory
             * @property service
             * @type {{}}
             */
            var service = {};

            /**
             * Allows a user to log in via the standard login service
             * @method Login
             * @param Username {string} The username
             * @param password {string} The password
             * @param callback {Object} How the function should call back to the area that called it
             * @return {Object} A callback allowing the calling function to interact with the function
             */
            service.Login = function (Username, password, callback) {
                $http.post(__env.apiUrl + "/Account/Login", { Email: Username, Password: password })
                    .then(function (response) {
                        callback(response);
                    })
                    .catch(function (error) {
                        console.log(error);
                    });

            };

            /**
             * Sets the credentials of a given user inside the cookie
             * @method SetCredentials
             * @param Username {string} The username
             * @param password {string} The password
             */
            service.SetCredentials = function (Username, password) {
                let authdata = Base64.encode(Username + ":" + password);

                $rootScope.globals = {
                    currentUser: {
                        Username: Username,
                        authdata: authdata
                    }
                };

                $http.defaults.headers.common["Authorization"] = "Basic " + authdata; // jshint ignore:line
                $cookies.putObject("globals", $rootScope.globals, {
                    expires: expireDate,
                    path: "/",
                    withCredentials: true
                });
            };

            /**
             * Checks to see if the rootscope of the application has the user credentials or not
             * @method HasCredentials
             * @return {Object} A bool value showing if the credentials exist within the system or not
             */
            service.HasCredentials = function () {
                return $cookies.getObject("globals");
            };

            /**
             * Removes the user credentials from the rootscope of the application
             * @method ClearCredentials
             */
            service.ClearCredentials = function () {
                $rootScope.globals = {};
                $cookies.remove("globals");
                $http.defaults.headers.common.Authorization = "Basic ";
            };

            return service;
        }])
    /**
     * A factory designed to encode and decode a string to and from a base64 format
     * @class Base64
     */
    .factory("Base64", function () {
        /* jshint ignore:start */
        /**
         * The possible characters that can be used in the string
         * @property keyStr
         * @type {string}
         */
        var keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        return {
            /**
             * Encodes a standard string into a base64 format
             * @method encode
             * @param input {string} A standard non-encoded base 64 string
             * @return {string} A base64 encoded string
             */
            encode: function (input) {
                /// <summary>
                /// s the specified input.
                /// </summary>
                /// <param name="input">The input.</param>
                /// <returns></returns>
                var output = "";
                var chr1, chr2, chr3 = "";
                var enc1, enc2, enc3, enc4 = "";
                var i = 0;

                do {
                    chr1 = input.charCodeAt(i++);
                    chr2 = input.charCodeAt(i++);
                    chr3 = input.charCodeAt(i++);

                    enc1 = chr1 >> 2;
                    enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                    enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                    enc4 = chr3 & 63;

                    if (isNaN(chr2)) {
                        enc3 = enc4 = 64;
                    } else if (isNaN(chr3)) {
                        enc4 = 64;
                    }

                    output = output +
                        keyStr.charAt(enc1) +
                        keyStr.charAt(enc2) +
                        keyStr.charAt(enc3) +
                        keyStr.charAt(enc4);
                    chr1 = chr2 = chr3 = "";
                    enc1 = enc2 = enc3 = enc4 = "";
                } while (i < input.length);

                return output;
            },

            /**
             * Decodes a given string from the base64 encoding back into a standard string
             * @method decode
             * @param input {string} A base64 encoded string
             * @return {string} A string value that been decoded from base64
             */
            decode: function (input) {
                var output = "";
                var chr1, chr2, chr3 = "";
                var enc1, enc2, enc3, enc4 = "";
                var i = 0;

                // remove all characters that are not A-Z, a-z, 0-9, +, /, or =
                var base64test = /[^A-Za-z0-9\+\/\=]/g;
                if (base64test.exec(input)) {
                    window.alert("There were invalid base64 characters in the input text.\n" +
                        "Valid base64 characters are A-Z, a-z, 0-9, '+', '/',and '='\n" +
                        "Expect errors in decoding.");
                }
                input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

                do {
                    enc1 = keyStr.indexOf(input.charAt(i++));
                    enc2 = keyStr.indexOf(input.charAt(i++));
                    enc3 = keyStr.indexOf(input.charAt(i++));
                    enc4 = keyStr.indexOf(input.charAt(i++));

                    chr1 = (enc1 << 2) | (enc2 >> 4);
                    chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                    chr3 = ((enc3 & 3) << 6) | enc4;

                    output = output + String.fromCharCode(chr1);

                    if (enc3 != 64) {
                        output = output + String.fromCharCode(chr2);
                    }
                    if (enc4 != 64) {
                        output = output + String.fromCharCode(chr3);
                    }

                    chr1 = chr2 = chr3 = "";
                    enc1 = enc2 = enc3 = enc4 = "";

                } while (i < input.length);

                return output;
            }
        };

        /* jshint ignore:end */
    });