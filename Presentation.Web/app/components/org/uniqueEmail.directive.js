(function (ng, app) {
    'use strict';
    app.directive('uniqueEmail', [
        '$http', 'userService', '_', function ($http, userService, _) {
            return {
                require: 'ngModel',
                link: function (scope, element, attrs, ctrl) {
                    var user;
                    userService.getUser().then(function (result) {
                        user = result;
                    });
                    var validateAsync = _.debounce(function (viewValue) {
                        $http.get("/odata/Users/IsEmailAvailable(email='" + viewValue + "')")
                            .then(function (response) {
                            if (response.data.value) {
                                // email is available
                                scope.emailExists = false;
                                ctrl.$setValidity('available', true);
                                ctrl.$setValidity('lookup', true);
                            }
                            else {
                                // email is in use
                                scope.emailExists = true;
                            }
                        }, function () {
                            // something went wrong
                            ctrl.$setValidity('lookup', false);
                        });
                    }, 500);
                    ctrl.$parsers.unshift(function (viewValue) {
                        validateAsync(viewValue);
                        // async returns breaks the setting of $modelValue so just returning
                        return viewValue;
                    });
                }
            };
        }
    ]);
})(angular, app);
//# sourceMappingURL=uniqueEmail.directive.js.map