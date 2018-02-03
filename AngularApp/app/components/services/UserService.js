angular.module("User", ["ngStorage"])
    .factory("UserService", ["$localStorage", function ($localStorage) {
        $storage = $localStorage;
        var user = undefined;

        return {
            addItem: addItem,
            getItem: getItem
        };

        function addItem(item) {
            if (user === undefined)
                user = {}
            Object.keys(item).forEach(function (key) {
                user[key] = item[key];
            });
            $storage.user = user;
        }

        function getItem() {
            if (!user) {
                user = $storage.user;
            }
            return user;
        }
    }]);