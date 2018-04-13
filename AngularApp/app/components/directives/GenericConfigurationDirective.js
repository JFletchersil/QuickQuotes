/**
 * A collection of directives that allows the application to generate generic configuration HTML elements
 * @module GConfigDirective
 */
angular.module("GConfigDirective", [])
/**
 * Allows for the creation of a general configuration element
 * This a generic element directive, it allows the associated HTML template to
 * take many forms and still remain properly bound to the controller that is using the directive
 * The scope bindings are very important, they should not be altered without good reason
 * If altered, you may break the two way binding of elements, and this will result in data loss
 * @class GConfigDirective
 * @param $filter {Object} The $filter module, this allows filters to be placed on text
 */
    .directive("dbconfig", function ($filter) {
        return {
            scope: {
                elementHtmlName: "<?",
                binding: "=?",
                inputType: "<?",
                configData: "=?",
                hasChanged: "=",
                index: "="
            },
            priority: 1001,
            restrict: "E",
            templateUrl: "components/templates/genericconfigurationtemplate.html",
            replace: true,
            /**
             * Links the HTML template to the configuration directive. This allows the configuration
             * directive to manage the presentation and operation of the HTML template
             * @method link
             * @param $scope {Object} The scope of the element that is being worked with
             * @param element {Object} A copy of the HTML template being worked with
             * @param attrs {Object} The attributes present on the element being worked with
             * @param ctrls {Object} The controls placed on the HTML element
             */
            link: function ($scope, element, attrs, ctrls) {
                /**
                 * Automatically resizes the size of the textbox to fit the size of the text
                 * @method resize
                 * @param $event {Object} The $event, could be anything from on blur, to input
                 */
                $scope.resize = function ($event) {
                    $event.target.style.height = $event.target.scrollHeight + "px";
                };
                try {
                    $scope.configData[$scope.elementHtmlName] = JSON.stringify(JSON.parse($scope.configData[$scope.elementHtmlName]), null, "    ");
                } catch (err) {
                    try {
                        $scope.configData[$scope.elementHtmlName] = $filter("prettyXml")($scope.configData[$scope.elementHtmlName]);
                    } catch (err) {
                        $scope.configData[$scope.elementHtmlName] = $scope.configData[$scope.elementHtmlName];
                    }
                }
            }
        }
    });