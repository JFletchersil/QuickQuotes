"use strict";

/**
 * A collection of controllers designed to manage the admin modals
 * @module quoteTool.adminmodal
 * @param ui.router {Object} The router object that controls how the system handles navigation requests
 * @param ngAnimate {Object} The animation object which provides Jquery animations in AngularJS Fashion
 * @param ngMaterial {Object} The object that allows interaction with the Material Design of the page in an AngularJS Fashion
 * @param md.data.table {Object} The object that allows JQuery DataTables to be constructed within the scope of the module
 */
angular.module("quoteTool.adminmodal", ["ui.router", "ngAnimate", "ngMaterial", "md.data.table"])
    /**
     * This controller is designed to provide a modal that allows an admin level user to edit a users details.
     * This includes the ability to change user names, passwords (only to force a new password) as well as Phone Numbers.
     * @class AdminModal
     * @param $scope {Object} The $scope
     * @param $element {Object} The $HTTP module, this allows us to make HTTP requests
     * @param title {string} The title of the modal
     * @param infoMessage {string} The message presented inside the modal
     * @param close {Object} How the modal should close
     */
    .controller("AdminModal", ["$scope", "$element", "title", "infoMessage", "close",
        function ($scope, $element, title, infoMessage, close) {
            /**
             * The title of the modal
             * @property $scope.title
             * @type {string}
             */
            $scope.title = title;
            /**
             * The infoMessage of the modal
             * @property $scope.infoMessage
             * @type {string}
             */
            $scope.infoMessage = infoMessage;
            /**
             * The newUserModel of the modal
             * @property $scope.newUserModel
             * @type {Object}
             */
            $scope.newUserModel = {};
            //  This close function doesn't need to use jQuery or bootstrap, because
            //  the button has the 'data-dismiss' attribute.

            /**
             * Handles how the modal should shut if the attribute 'data-dismiss' is present
             * on the modal close.
             * @method close
             */
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

            /**
             * Handles how the modal should shut if the attribute 'data-dismiss' is not present
             * on the modal close.
             * @method cancel
             */
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