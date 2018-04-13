/**
 * The development environment for the front end of the application when it is being actively developed.
 * @module env
 */
(function (window) {
    window.__env = window.__env || {};
    /**
     * The url to access the api
     * @property apiUrl
     * @type {string}
     */    window.__env.apiUrl = "http://localhost:8080/api";
    /**
     * The baseUrl for the application
     * @property baseUrl
     * @type {string}
     */
    window.__env.baseUrl = "/";
    /**
     * If debug mode is enabled
     * @property enableDebug
     * @type {boolean}
     */
    window.__env.enableDebug = true;
    /**
     * The base number of items inside the global menu search
     * @property baseSearch
     * @type {number}
     */
    window.__env.baseSearch = 9;
}(this));