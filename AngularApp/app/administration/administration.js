"use strict";
/**
 * A collection of controllers designed to run and operate the administration screen
 * @module quoteTool.administration
 * @param ui.router Object The router object that controls how the system handles navigation requests
 * @param ngAnimate Object The animation object which provides Jquery animations in AngularJS Fashion
 * @param ngMaterial Object The object that allows interaction with the Material Design of the page in an AngularJS Fashion
 * @param md.data.table Object The object that allows JQuery DataTables to be constructed within the scope of the module
 * @param ui.bootstrap Object The object that allows interaction with the Bootstrap elements of the page in an AngularJS Fashion
 */
angular.module("quoteTool.administration", ["ui.router", "ngAnimate", "ngMaterial", "md.data.table", "ui.bootstrap"])
    /**
     * This controller is designed to allow administrators to manage users.
     * This includes the ability to add new users, delete users, change their roles
     * and edit specific details about the user.
     * @class Administration
     * @param $scope Object The $scope
     * @param $http Object The $HTTP module, this allows us to make HTTP requests
     * @param $filter Object The $filter module, this allows filters to be placed on text
     * @param UserService Object The user service, this allows us to save and retrieve data from the local storage
     * @param ModalService Object The modal service, this allows us to create and use modals as required
     * @param __env JSON This stores environment values that the application will use
     */
    .controller("Administration", ["$scope", "$http", "$filter", "UserService", "ModalService", "__env",
        function ($scope, $http, $filter, UserService, ModalService, __env) {
            /**
             * The paging model used to page the table
             * @property $scope.pagingModel
             * @type {{PageNumber: number, PageSize: number}}
             */
            $scope.pagingModel = {
                PageNumber: 1,
                PageSize: 5
            };

            /**
             * The query sent to the database when querying for users
             * @property $scope.query
             * @type {{order: string, limit: number, page: number}}
             */
            $scope.query = {
                order: "UserName",
                limit: $scope.pagingModel.PageSize,
                page: $scope.pagingModel.PageNumber
            };

            /**
             * The filter for the table
             * @property $scope.filter
             * @type {{options: {debounce: number}, hasFiltered: boolean}}
             */
            $scope.filter = {
                options: {
                    debounce: 500
                },
                hasFiltered: false
            };

            /**
             * The old items prior to starting a search via the search filter
             * @property $scope.oldItems
             * @type {Array}
             */
            $scope.oldItems = [];

            /**
             * The model responsible for managing the new user model
             * @property $scope.newUserModel
             * @type {{IsAdministrator: boolean}}
             */
            $scope.newUserModel = {
                IsAdministrator: false
            };

            /**
             * Determines if a specific user is having the actions that can be performed against them shown or not
             * @property $scope.showOptions
             * @type {boolean}
             */
            $scope.showOptions = false;

            /**
             * Determines if the user working on the administration screen is a super admin or not
             * @property $scope.isSuperAdmin
             * @type {string}
             */
            $scope.isSuperAdmin = UserService.getItem().isSuperAdmin;

            /**
             * Determines if the user working on the administration screen is an admin or not
             * @property $scope.isAdmin
             * @type {string}
             */
            $scope.isAdmin = UserService.getItem().isAdmin;

            /**
             * The tabs that will be shown to the user, determined by if they are a super admin or not
             * @property $scope.tabs
             * @type {*[]}
             */
            $scope.tabs = ($scope.isSuperAdmin) ? [{Disabled: false}, {Disabled: false}] : [{Disabled: false}];

            /**
             * The labels for the number of tabs within the system
             * @property $scope.tabLabels
             * @type {string[]}
             */
            $scope.tabLabels = ["Administrators", "Users"];

            /**
             * The current selected user
             * @property $scope.selected
             * @type {{}}
             */
            $scope.selected = {};

            /**
             * The total number of pages within the database at the current paging size
             * @property $scope.TotalSize
             * @type {number}
             */
            $scope.TotalSize = 0;

            /**
             * The total number of users within the database
             * @property $scope.TotalItems
             * @type {number}
             */
            $scope.TotalItems = 0;

            /**
             * Successes the specified return data wrapped inside a promise.
             * This is used when making requests to the database, and is used to evaluate
             * how long it will take before a promise resolves.
             * @method success
             * @return {*|Object} Updates the total number of users, the total number of paged users, and provides a list of users
             * @param returnData Object The return data.
             */
            function success(returnData) {
                $scope.TotalSize = returnData.TotalPages;
                $scope.TotalItems = returnData.TotalItems;
                $scope.users = returnData.QueueDisplay;
            }

            /**
             * Performs a search within the database for the given search term
             * @method generalSearch
             * @param searchTerm string The search term
             * @return {*|Object} A list of users who match the search term
             */
            $scope.generalSearch = function (searchTerm) {
                let searchModel = {
                    FilterTerm: searchTerm,
                    ReturnAll: $scope.pagingModel.ReturnAll
                };
                $http.post(__env.apiUrl + "/Account/ReturnAllUsersAtLevelForSearch", searchModel).then(function (response) {
                    $scope.users = response.data;
                });
            };

            /**
             * Changes the current database page that the user of the application is using
             * @method pageChangeHandler
             * @param newPageNumber number The new page number that the user wishes to view
             * @return {*|Object} A new list of users who would be on the current page
             */
            $scope.pageChangeHandler = function (newPageNumber) {
                if (isNaN(newPageNumber))
                    $scope.pagingModel.OrderBy = newPageNumber;
                $scope.promise =
                    $http.post(__env.apiUrl + "/Account/ReturnAllUsers", $scope.pagingModel).then(function (response) {
                        success(response.data);
                    });
            };

            /**
             * Creates a new user within the database
             * @method createNewUser
             * @return {*|Object} A 200 or 500 response depending on if adding a new user was successful or not
             */
            $scope.createNewUser = function () {
                $http.post(__env.apiUrl + "/Account/Register", $scope.newUserModel).then(
                    function (response) {
                        $scope.newUserModel = {
                            IsAdministrator: false
                        }
                    })
                    .catch(function (error) {

                    });
            };

            /**
             * Hides the operations that can be performed on a given user
             * Options in this case being the various different actions that can be performed
             * upon a user
             * @method hideOptions
             * @param user Object The user object whose options are being hidden
             */
            $scope.hideOptions = function (user) {
                $scope.selected = user;
                user.showOptions = !user.showOptions;
            };

            /**
             * Allows the user to change the tab, if the user has multiple tabs in which
             * they can move between
             * @method onTabSelected
             * @param tabSelected number The tab number that is being moved towards
             */
            $scope.onTabSelected = function (tabSelected) {
                $scope.tabNumber = tabSelected;
                $scope.pagingModel.ReturnAll = tabSelected === 0;
                $scope.pageChangeHandler(1);
            };

            /**
             * Opens a modal for the user to view
             * @method onOpenModal
             * @param title string The title of the modal
             * @param infoMessage string The message displayed inside the modal
             * @return {*|void} Closes the modal when closed
             */
            $scope.onOpenModal = function (title, infoMessage) {
                return ModalService.showModal({
                    templateUrl: "../administration/modals/adminmodal.html",
                    controller: "AdminModal",
                    preClose: (modal) => {
                        modal.element.modal("hide");
                    },
                    inputs: {
                        title: title,
                        infoMessage: infoMessage
                    }
                });
            };

            /**
             * Changes the user type from Admin to User and back again
             * @method onChangeUserType
             */
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

            /**
             * Opens a model to allow the administrator to edit the details of a user
             * @method onEditUserDetails
             * @return {*|Object} A 200 or 500 response depending on if the action to edit the user was successful or not
             */
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

            /**
             * Deletes a user from the user database
             * @method onDeleteUser
             * @return {*|Object} A true or false response depending on if the action to delete the user was successful or not
             */
            $scope.onDeleteUser = function () {
                $http.post(__env.apiUrl + "/Account/DeleteUser", {Guid: $scope.selected.Guid, IsDeleting: true}).then(
                    function (response) {
                    })
                    .catch(function (error) {

                    });
            };

            /**
             * Clones a users permissions and roles onto a new user
             * A modal will display if information was missed during the clone process, it will state
             * the user must fill in all information into the application in order for the application
             * @method onCloneUser
             * @return {*|Object} A 200 or 500 response depending on if the action to edit the user was successful or not
             */
            $scope.onCloneUser = function () {
                $scope.onOpenModal("Clone User", "Please enter the details you wish to set the cloned user with").then(function (modal) {
                    modal.element.modal();
                    modal.closed.then(function (result) {
                        if (result.UserName === undefined || result.EmailAddress === undefined || result.Password === undefined || result.ConfirmPassword === undefined) {
                            alert("Information is missing from the application, please enter all details");
                        } else {
                            $http.post(__env.apiUrl + "/Account/CloneUser", {
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

            /**
             * Resets the user list, and removes the filter that the user was searching for
             * within the application
             * @method removeFilter
             * @return {*|Object} The normal state of the application prior to searching for users
             */
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

                if ($scope.isSuperAdmin || $scope.isAdmin) {
                    if ($scope.tabNumber === 0) {
                        $scope.tabs[1].Disabled = false;
                    } else {
                        $scope.tabs[0].Disabled = false;
                    }
                }

                $scope.filter.hasFiltered = false;
            };

            /**
             * Filters out the current list of users for the filter search text
             * If it cannot find it present within the current list of users, it will
             * search the database
             * @method filterItems
             * @return {*|Object} A list of users that matches the current filter search text
             */
            $scope.filterItems = function () {
                if ($scope.oldItems.length === 0) {
                    $scope.oldItems = $scope.users;
                    $scope.users = $filter('filter')($scope.users, $scope.query.filter)
                } else {
                    $scope.users = $scope.oldItems;
                    $scope.users = $filter('filter')($scope.users, $scope.query.filter)
                }

                if ($scope.users.length === 0) {
                    $scope.generalSearch($scope.query.filter);
                }
            };

            /**
             * Shows the filter location for the user to enter his search text
             * Takes into account which tab is currently being searched
             * @method activateFilter
             * @return {*|Object} A visible search box for the user to use
             */
            $scope.activateFilter = function () {
                $scope.filter.show = true;
                if ($scope.isSuperAdmin || $scope.isAdmin) {
                    if ($scope.tabNumber === 0) {
                        $scope.tabs[1].Disabled = true;
                    } else {
                        $scope.tabs[0].Disabled = true;
                    }
                }
                $scope.filter.hasFiltered = true;
            }
        }]);