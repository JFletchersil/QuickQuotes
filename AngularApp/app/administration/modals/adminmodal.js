"use strict";

angular.module("quoteTool.adminmodal", ["ui.router", "ngAnimate", "ngMaterial", "md.data.table"])
    .controller("AdminModal", ["$scope", "$element", "title", "infoMessage", "close", function ($scope, $element, title, infoMessage, close) {
        $scope.title = title;
        $scope.infoMessage = infoMessage;
        $scope.newUserModel = {};
        //  This close function doesn't need to use jQuery or bootstrap, because
        //  the button has the 'data-dismiss' attribute.
        $scope.close = function () {
            //  Manually hide the modal.
            $element.modal("hide");
            close({
                UserName: $scope.newUserModel.UserName,
                EmailAddress: $scope.newUserModel.Email,
                IsAdministrator: $scope.newUserModel.IsAdministrator,
                Password: $scope.newUserModel.Password,
                ConfirmPassword: $scope.newUserModel.ConfirmPassword,
                PhoneNumber: $scope.newUserModel.PhoneNumber
            }, 500); // close, but give 500ms for bootstrap to animate
        };

        //  This cancel function must use the bootstrap, 'modal' function because
        //  the doesn't have the 'data-dismiss' attribute.
        $scope.cancel = function () {

            //  Manually hide the modal.
            $element.modal("hide");

            //  Now call close, returning control to the caller.
            close({
                UserName: $scope.newUserModel.UserName,
                EmailAddress: $scope.newUserModel.Email,
                IsAdministrator: $scope.newUserModel.IsAdministrator,
                Password: $scope.newUserModel.Password,
                ConfirmPassword: $scope.newUserModel.ConfirmPassword,
                PhoneNumber: $scope.newUserModel.PhoneNumber
            }, 500); // close, but give 500ms for bootstrap to animate
        };
    }]);