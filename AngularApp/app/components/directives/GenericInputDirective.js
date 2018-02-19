angular.module("GInputDirective", [])
    .directive("dbinput",
    function () {
        return {
            scope: {
                elementHtmlName: "<",
                label: "<",
                form: "=",
                binding: "=",
                inputType: "<"
            },
            require: {
                form: "^form"
            },
            restrict: "E",
            templateUrl: "components/templates/genericinputtemplate.html",
            replace: true,
            link: function ($scope, element, attrs, ctrls) {
                debugger;
                console.debug($scope);
                $scope.elementHtmlName = attrs.elementHtmlName;
                $scope.label = attrs.label;
                $scope.form = ctrls.form;
                $scope.formName = ctrls.form.$name;
                if (attrs.required !== undefined) {
                    $scope.required = true;
                }
            }
        }
    });