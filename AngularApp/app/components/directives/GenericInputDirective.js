angular.module("GInputDirective", [])
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
            link: function ($scope, element, attrs, ctrls) {
                $scope.formName = ctrls.form.$name;
                if (attrs.required !== undefined) {
                    $scope.required = true;
                }
            }
        }
    });