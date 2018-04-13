/**
 * A collection of directives that make it possible to turn a text element into a currency element
 * @module CurrencyDirective
 */
angular.module("CurrencyDirective", [])
/**
 * On blur, this will force the addition of a currency symbol to a text box
 * This is an attribute restricted Directive, it can only be used as an HTML attribute
 * @class blurToCurrency
 * @param $filter Object The $filter module, this allows filters to be placed on text
 */
    .directive("blurToCurrency", function ($filter) {
        return {
            restrict: "A",
            scope: {
                amount: "="
            },
            /**
             * Links the element to the currency directive. This allows the currency
             * directive to function with other directives
             * @method link
             * @param scope Object The scope of the element that is being worked with
             * @param el Object The element being worked with
             * @param attrs Object The attributes present on the element being worked with
             */
            link: function (scope, el, attrs) {
                el.val($filter("currency")(scope.amount, "\u00A3"));
                /**
                 * On focus events, set the value of the element to match the value
                 * that the scope says it has
                 * @event focus
                 */
                el.bind("focus", function () {
                    el.val(scope.amount);
                });

                /**
                 * Inside the scope of the element, it sets the value to match the input and
                 * then applies the scope change
                 * @event input
                 */
                el.bind("input", function () {
                    scope.amount = el.val();
                    scope.$apply();
                });

                /**
                 * On blur events, set the amount to remain the same, but place the
                 * currency symbol at the front of the amount
                 * @event blur
                 */
                el.bind("blur", function () {
                    let filterVal = scope.amount.toString();
                    if (filterVal.charAt(0) === "\u00A3") {
                        filterVal = scope.amount.substr(1);
                    }
                    el.val($filter("currency")(filterVal, "\u00A3"));
                });
            }
        }
    });