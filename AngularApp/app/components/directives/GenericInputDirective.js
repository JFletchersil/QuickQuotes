/**
 * A collection of directives that allows the application to generate generic quotation HTML elements
 * @module GInputDirective
 */
angular.module("GInputDirective", [])
/**
 * Allows for the creation of a generic input element.
 * This a generic element directive, it allows the associated HTML template to
 * take many forms and still remain properly bound to the controller that is using the directive
 * The scope bindings are very important, they should not be altered without good reason
 * If altered, you may break the two way binding of elements, and this will result in data loss
 * It is vital that this directive remain unchanged, otherwise the entire quote page will break
 * @class dbinput
 */
    .directive("dbinput",
    function () {
        return {
            scope: {
                elementHtmlName: "<?",
                label: "<?",
                form: "=",
                binding: "=?",
                inputType: "<?",
                warningLabel: "<?",
                description: "<?"
            },
            require: {
                form: "^form"
            },
            priority: 1001,
            restrict: "E",
            templateUrl: "components/templates/genericinputtemplate.html",
            replace: true,
            /**
             * This links the element with controlling template
             * This also ensures that the correct forms are linked together in order to make sure
             * that the validation works as a developer would expect
             * @method link
             * @param $scope Object The scope of the element that is being worked with
             * @param element Object A copy of the HTML template being worked with
             * @param attrs Object The attributes present on the element being worked with
             * @param ctrls Object The controls placed on the HTML element
             */
            link: function ($scope, element, attrs, ctrls) {
                $scope.formName = ctrls.form.$name;
                if (attrs.required !== undefined) {
                    $scope.required = true;
                }
            }
        }
    });