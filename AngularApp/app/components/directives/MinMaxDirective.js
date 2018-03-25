angular.module("MinMaxDirective", [])
.directive('ngCurrencyMax', function () {
    return {
        restrict: "A",
        require: 'ngModel',
        link: function (scope, element, attr, ngModelCtrl) {
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
.directive('ngCurrencyMin', function () {
    return {
        restrict: "A",
        require: 'ngModel',
        link: function (scope, element, attr, ngModelCtrl) {
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