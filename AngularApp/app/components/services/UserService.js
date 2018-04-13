/**
 * A collection of factories that helps the application manage information the user needs to save
 * This includes items such as the product types, quote types, and the configuration of certain quote types
 * @module User
 * @param ngStorage object Allows easy interaction with the localStorage in an AngularJS fashion
 */
angular.module("User", ["ngStorage"])
    /**
     * A service designed to store data within the localStorage of the browser
     * Processes and un-processes data such that it is always in the form of a JSON Object for easier interaction
     * @class UserService
     * @param $localStorage object An AngularJS module designed to systematise and simplify working with localStorage
     */
    .factory("UserService", ["$localStorage", function ($localStorage) {
        /**
         * Sets the $storage property to be equal to the $localStorage property
         * @property $storage
         * @type {$localStorage|*}
         */
        $storage = $localStorage;

        /**
         * The current user and the user's data
         * @property user
         * @type {undefined}
         */
        var user = undefined;

        return {
            addItem: addItem,
            getItem: getItem
        };

        /**
         * Adds a given item to the localStorage
         * Adds the item with regards to if the key exists or not, ensuring there are no duplicate keys within the object
         * Ensures that the object is always formed in the context of being the users details
         * @method addItem
         * @param item JSON The object being saved to localStorage
         */
        function addItem(item) {
            if (user === undefined)
                user = {};
            Object.keys(item).forEach(function (key) {
                user[key] = item[key];
            });
            $storage.user = user;
        }

        /**
         * Returns the user object and all of it's associated data
         * @method getItem
         * @return {JSON} The user object containing all of the current users information
         */
        function getItem() {
            if (!user) {
                user = $storage.user;
            }
            return user;
        }
    }]);