//angular.module("Storage", []);
//angular.module("Storage")
//    .factory('StorageService', ['$rootScope', function ($rootScope) {

//    var service = {

//        model: {
//            name: '',
//            email: ''
//        },

//        SaveState: function () {
//            sessionStorage.UserService = angular.toJson(service.model);
//        },

//        RestoreState: function () {
//            service.model = angular.fromJson(sessionStorage.UserService);
//        }
//    }

//    $rootScope.$on("savestate", service.SaveState);
//    $rootScope.$on("restorestate", service.RestoreState);

//    return service;
//}]);