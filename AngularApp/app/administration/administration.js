"use strict";

angular.module("quoteTool.administration", ["ui.router", "ngAnimate", "ngMaterial", "md.data.table", "ui.bootstrap"])
    .controller("Administration", ["$scope", "$http", "$filter", "UserService", "ModalService", "__env", function ($scope, $http, $filter, UserService, ModalService, __env) {
        $scope.pagingModel = {
            PageNumber: 1,
            PageSize: 5
        };

        $scope.query = {
            order: "UserName",
            limit: $scope.pagingModel.PageSize,
            page: $scope.pagingModel.PageNumber
        };

        $scope.filter = {
            options: {
                debounce: 500
            },
            hasFiltered: false
        };

        $scope.oldItems = [];

        $scope.newUserModel = {
            IsAdministrator: false
        };

        $scope.showOptions = false;

        $scope.isSuperAdmin = UserService.getItem().isSuperAdmin;
        $scope.isAdmin = UserService.getItem().isAdmin;

        $scope.tabs = ($scope.isSuperAdmin) ? [1, 2] : [1];
        $scope.tabLabels = ["Administrators", "Users"];

        $scope.selected = {};
        $scope.TotalSize = 0;
        $scope.TotalItems = 0;

        function success(returnData) {
            $scope.TotalSize = returnData.TotalPages;
            $scope.TotalItems = returnData.TotalItems;
            $scope.users = returnData.QueueDisplay;
        }

        $scope.pageChangeHandler = function (newPageNumber) {
            $scope.pagingModel.PageNumber = newPageNumber;
            $scope.promise =
                $http.post(__env.apiUrl + "/Account/ReturnAllUsers", $scope.pagingModel).
                    then(function (response) {
                        success(response.data);
                    });
        }

        $scope.createNewUser = function () {
            $http.post(__env.apiUrl + "/Account/Register", $scope.newUserModel).then(
                function (response) {
                    $scope.newUserModel = {
                        IsAdministrator: false
                    }
                })
                .catch(function (error) {

                });
        }

        $scope.hideOptions = function (user) {
            $scope.selected = user;
            user.showOptions = !user.showOptions;
        }

        $scope.onTabSelected = function (tabSelected) {
            $scope.pagingModel.ReturnAll = tabSelected === 0;
            $scope.pageChangeHandler(1);
        }

        $scope.onOpenModal = function (title, infoMessage) {
            return ModalService.showModal({
                templateUrl: "../administration/modals/adminmodal.html",
                controller: "AdminModal",
                preClose: (modal) => { modal.element.modal("hide"); },
                inputs: {
                    title: title,
                    infoMessage: infoMessage
                }
            });
        }

        $scope.onChangeUserType = function () {
            $http.post(__env.apiUrl + "/Account/ChangeUserType", {
                Guid: $scope.selected.Guid,
                IsAdmin: $scope.selected.IsAdmin
            })
                .then(function (response) {
                })
                .catch(function (error) {

                });
        };

        $scope.onEditUserDetails = function () {
            $scope.onOpenModal("Edit User", "Please enter the details you wish to change").then(function (modal) {
                modal.element.modal();
                modal.closed.then(function (result) {
                    if (result.UserName === undefined) {
                        alert("No changes saved");
                    } else {
                        $http.post(__env.apiUrl + "/Account/AlterAccountLoginDetails",
                            {
                                Guid: $scope.selected.Guid,
                                UserName: result.UserName,
                                EmailAddress: result.EmailAddress,
                                Password: result.Password,
                                ConfirmPassword: result.ConfirmPassword,
                                PhoneNumber: result.PhoneNumber
                            })
                            .then(function (response) {
                            })
                            .catch(function (error) {

                            });
                    }
                });
            });
        };

        $scope.onDeleteUser = function () {
            $http.post("http://localhost:8080/api/Account/DeleteUser", { Guid: $scope.selected.Guid, IsDeleting: true }).then(
                function (response) {
                })
                .catch(function (error) {

                });
        };

        $scope.onCloneUser = function () {
            $scope.onOpenModal("Clone User", "Please enter the details you wish to set the cloned user with").then(function (modal) {
                modal.element.modal();
                modal.closed.then(function (result) {
                    if (result.UserName === undefined || result.EmailAddress === undefined || result.Password === undefined || result.ConfirmPassword === undefined) {
                        alert("No changes saved");
                    } else {
                        $http.post("http://localhost:8080/api/Account/CloneUser", {
                            Guid: $scope.selected.Guid, UserName: result.UserName, EmailAddress: result.EmailAddress,
                            Password: result.Password, ConfirmPassword: result.ConfirmPassword
                        })
                            .then(function (response) {
                            })
                            .catch(function (error) {

                            });
                    }
                });
            });
        };

        $scope.removeFilter = function () {
            if ($scope.filter.hasFiltered) {
                $scope.filter.show = false;
                $scope.query.filter = '';

                if ($scope.filter.form.$dirty) {
                    $scope.filter.form.$setPristine();
                }
                $scope.users = $scope.oldItems;
                $scope.oldItems = [];
            } else {
                $scope.filter.show = false;
            }
        };

        $scope.filterItems = function () {
            if ($scope.oldItems.length === 0) {
                $scope.oldItems = $scope.users;
                $scope.users = $filter('filter')($scope.users, $scope.query.filter)
                $scope.filter.hasFiltered = true;
            } else {
                $scope.users = $scope.oldItems;
                $scope.users = $filter('filter')($scope.users, $scope.query.filter)
                $scope.filter.hasFiltered = true;
            }
        };
    }]);