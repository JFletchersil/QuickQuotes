angular.module('User')
    .factory('UserService', function () {
        var dict = {};

        return {
            addItem: addItem,
            getItem: getItem
        };

        function addItem(item) {
            dict = item;
        }

        function getItem() {
            return dict;
        }
    });