angular.module("GConfigDirective", [])
    .directive("dbconfig",
    function ($filter) {
        return {
            scope: {
                elementHtmlName: "<?",
                binding: "=?",
                inputType: "<?",
                configData: "=?"
            },
            priority: 1001,
            restrict: "E",
            templateUrl: "components/templates/genericconfigurationtemplate.html",
            replace: true,
            link: function ($scope, element, attrs, ctrls) {
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