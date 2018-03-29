angular.module("AuthModule", ["auth0.auth0"])
    .service("AuthService", ["$state", "angularAuth0", "$timeout", function ($state, angularAuth0, $timeout) {
        var userProfile;

        function login() {
            angularAuth0.authorize();
        }

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
                    //console.log(err);
                    //alert('Error: ' + err.error + '. Check the console for further details.');
                }
            });
        }

        function getProfile(cb) {
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

        function setUserProfile(profile) {
            userProfile = profile;
        }

        function getCachedProfile() {
            return userProfile;
        }

        function setSession(authResult) {
            // Set the time that the access token will expire at
            let expiresAt = JSON.stringify((authResult.expiresIn * 1000) + new Date().getTime());
            localStorage.setItem('access_token', authResult.accessToken);
            localStorage.setItem('id_token', authResult.idToken);
            localStorage.setItem('expires_at', expiresAt);
        }

        function logout() {
            // Remove tokens and expiry time from localStorage
            localStorage.removeItem('access_token');
            localStorage.removeItem('id_token');
            localStorage.removeItem('expires_at');
            $state.go('home');
        }

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