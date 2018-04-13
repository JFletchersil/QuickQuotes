/**
 * A collection of directives that manages the upper and lower bounds of text fields if they are actually currency
 * fields
 * @module MinMaxDirective
 */
angular.module("MinMaxDirective", [])
/**
 * Provides a check against the max value such that a string representation of a currency cannot
 * exceed the maximum without breaking validation
 * This is an attribute restricted Directive, it can only be used as an HTML attribute
 * @class ngCurrencyMax
 */
.directive('ngCurrencyMax', function () {
    return {
        restrict: "A",
        require: 'ngModel',
        /**
         * Links the element to the currency directive. This allows the currency
         * directive to function with other directives and still have it's upper bounds controlled
         * @method link
         * @param scope Object The scope of the element that is being worked with
         * @param element Object The element being worked with
         * @param attr Object The attributes present on the element being worked with
         * @param ngModelCtrl Object The controller of the element that has this attribute
         */
        link: function (scope, element, attr, ngModelCtrl) {
            /**
             * Evaluates if value given exceeds the maximum bound or not
             * @method isMax
             * @param ngModelValue string A representation of the current value of the element with this directive attached
             * @example ng-currency-max = 5000
             * @return string A representation of the current value of the element with this directive attached
             */
            function isMax(ngModelValue) {
                if (!isNaN(ngModelValue)) {
                    if (ngModelValue > attr.ngMax) {
                        ngModelCtrl.$setValidity('max', false);
                    } else {
                        ngModelCtrl.$setValidity('max', true);
                    }
                } else {
                    ngModelCtrl.$setValidity('max', true);
                }
                return ngModelValue;
            }
            ngModelCtrl.$parsers.push(isMax);
        }
    };
})
/**
 * Provides a check against the min value such that a string representation of a currency cannot
 * be below the minimum without breaking validation
 * This is an attribute restricted Directive, it can only be used as an HTML attribute
 * @class ngCurrencyMin
 */
.directive('ngCurrencyMin', function () {
    return {
        restrict: "A",
        require: 'ngModel',
        /**
         * Links the element to the currency directive. This allows the currency
         * directive to function with other directives and still have it's lower bounds controlled
         * @method link
         * @param scope Object The scope of the element that is being worked with
         * @param element Object The element being worked with
         * @param attr Object The attributes present on the element being worked with
         * @param ngModelCtrl Object The controller of the element that has this attribute
         */
        link: function (scope, element, attr, ngModelCtrl) {
            /**
             * Evaluates if value given is below the minimum bound or not
             * @method isMin
             * @param ngModelValue string A representation of the current value of the element with this directive attached
             * @example ng-currency-min = 0
             * @return string A representation of the current value of the element with this directive attached
             */
            function isMin(ngModelValue) {
                if (!isNaN(ngModelValue)) {
                    if (ngModelValue < attr.ngMin) {
                        ngModelCtrl.$setValidity('min', false);
                    } else {
                        ngModelCtrl.$setValidity('min', true);
                    }
                } else {
                    ngModelCtrl.$setValidity('min', true);
                }
                return ngModelValue;
            }
            ngModelCtrl.$parsers.push(isMin);
        }
    };
});