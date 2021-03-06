﻿(function(ng, app) {
    "use strict";

    // match two input fields against each other
    // usage <input ng-model="firstModel" match="otherModel">
    app.directive("match", [
        function() {
            return {
                restrict: "A",
                scope: true,
                require: "ngModel",
                link: function(scope, elem, attrs, control) {
                    var checker = function() {

                        // get the value of the first password
                        var e1 = scope.$eval(attrs.ngModel);

                        // get the value of the other password
                        var e2 = scope.$eval(attrs.match);
                        return e1 == e2;
                    };
                    scope.$watch(checker, function(n) {

                        // set the form control to valid if both
                        // passwords are the same, else invalid
                        control.$setValidity("unique", n);
                    });
                }
            };
        }
    ]);
})(angular, app);
