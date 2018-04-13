/**
 * A collection of modules designed to make it easier and simpler to log in via the Auth0 Service
 * This is heavily based on sample code provided by the Auth0 team in order to underline how one would
 * log into their system in an AngularJS context
 * @module AuthModule
 * @param auth0.auth0 The Auth0 module required to properly interact with the Auth0 services
 */
angular.module("AuthModule", ["auth0.auth0"])
    /**
     * The standard service designed to allow a user to log into the Auth0 system as well as retrieve context
     * information about that user
     * Also stores the access token information such that it can be examined and used when communicating with the
     * Auth0 API as well as other APIs
     * @class AuthService
     * @param $state Object The state of the calling controller
     * @param angularAuth0 Object The Auth0 wrapper providing functions that make it possible to interact with the Auth0 API
     * @param $timeout Object An AngularJS object designed to allow easy checking of the time for timeout purposes
     */
    .service("AuthService", ["$state", "angularAuth0", "$timeout", function ($state, angularAuth0, $timeout) {
        /**
         * The current user profile of the user logged in
         * @type {Object}
         * @property userProfile
         */
        var userProfile;

        /**
         * Moves the user to the authorisation screen
         * @method login
         */
        function login() {
            angularAuth0.authorize();
        }

        /**
         * Handles the returning Authentication redirect, parsing it to see if the authorisation token is valid or not
         * Sets the auth result and returns the user to the home screen if the auth result is valid
         * Send the user back to the home screen if the auth result is not valid
         * @method handleAuthentication
         */
        function handleAuthentication() {
            var options = {};
            options.hash = window.location.hash.split("#!")[1];
            angularAuth0.parseHash(options, function (err, authResult) {
                if (authResult && authResult.accessToken && authResult.idToken) {
                    setSession(authResult);
                    $state.go('mainmenu.home');
                } else if (err) {
                    $timeout(function () {
                        $state.go('mainmenu.home');
                    });
                }
            });
        }

        /**
         * Using the access token, it attempts to retrieve and set the user profile
         * If no access token exists, then it returns a new error for the application to catch and handle
         * @method getProfile
         */
        function getProfile() {
            var accessToken = localStorage.getItem('access_token');
            if (!accessToken) {
                throw new Error('Access Token must exist to fetch profile');
            }
            angularAuth0.client.userInfo(accessToken, function (err, profile) {
                if (profile) {
                    setUserProfile(profile);
                }
                //cb(err, profile);
            });
        }

        /**
         * Saves the user profile to the profile of the user who just logged into the system
         * @method setUserProfile
         * @param profile JSON The profile of the current user
         */
        function setUserProfile(profile) {
            userProfile = profile;
        }

        /**
         * Returns the profile of the current user from the saved profile within this file
         * @method getCachedProfile
         * @return {*} JSON The profile of the current user
         */
        function getCachedProfile() {
            return userProfile;
        }

        /**
         * Sets the result of the authentication result to be stored inside local storage
         * Sets when the result will expire as well
         * Will only be called if the authResult was valid, otherwise the result will not be saved to local storage
         * @method authResult
         * @param authResult JSON The valid auth result of the authentication request
         */
        function setSession(authResult) {
            // Set the time that the access token will expire at
            let expiresAt = JSON.stringify((authResult.expiresIn * 1000) + new Date().getTime());
            localStorage.setItem('access_token', authResult.accessToken);
            localStorage.setItem('id_token', authResult.idToken);
            localStorage.setItem('expires_at', expiresAt);
        }

        /**
         * Logs a user out of the Auth0 local authentication
         * Removes all items from local storage and sends the user back to the home state
         * @method logout
         */
        function logout() {
            // Remove tokens and expiry time from localStorage
            localStorage.removeItem('access_token');
            localStorage.removeItem('id_token');
            localStorage.removeItem('expires_at');
            $state.go('home');
        }

        /**
         * Checks to see if the Authentication result, if it exists, is still valid
         * @method isAuthenticated
         * @return {boolean} A true or false value that indicates if the token is still valid at the present time
         */
        function isAuthenticated() {
            // Check whether the current time is past the 
            // access token's expiry time
            let expiresAt = JSON.parse(localStorage.getItem('expires_at'));
            return new Date().getTime() < expiresAt;
        }

        return {
            login: login,
            getProfile: getProfile,
            getCachedProfile: getCachedProfile,
            handleAuthentication: handleAuthentication,
            logout: logout,
            isAuthenticated: isAuthenticated
        }
    }]);