angular.module("CurrencyDirective", [])
.directive("blurToCurrency", function ($filter) {
    return {
        scope: {
            amount: "="
        },
        link: function (scope, el, attrs) {
            el.val($filter("currency")(scope.amount, "\u00A3"));

            el.bind("focus", function () {
                el.val(scope.amount);
            });

            el.bind("input", function () {
                scope.amount = el.val();
                scope.$apply();
            });

            el.bind("blur", function () {
                var filterVal = scope.amount.toString();
                if (filterVal.charAt(0) === "\u00A3") {
                    filterVal = scope.amount.substr(1);
                }
                el.val($filter("currency")(filterVal, "\u00A3"));
            });
        }
    }
});