﻿var app = angular.module('app', ['ui.router', 'ui.select2', 'ngAnimate', 'angular-growl', 'xeditable', 'restangular']);

app.config(['$urlRouterProvider', function ($urlRouterProvider) {
    $urlRouterProvider.otherwise('/');
}]);

app.config(['growlProvider', 'RestangularProvider', function (growlProvider, RestangularProvider) {
    growlProvider.globalTimeToLive(5000);
    growlProvider.onlyUniqueMessages(false);
    
    //Restangular config
    RestangularProvider.setBaseUrl('/api');
    RestangularProvider.setResponseExtractor(function (response, operation) {
        return response.Response;
    });
}]);

app.run(['$rootScope', '$http', '$state', 'editableOptions', function ($rootScope, $http, $state, editableOptions) {
    //init info
    $rootScope.page = {
        title: 'Index',
        subnav: []
    };

    $rootScope.user = {};

    //x-editable config
    editableOptions.theme = 'bs3'; // bootstrap3 theme.

    //logout function for top navigation bar
    $rootScope.logout = function () {
        $http.post('api/authorize?logout').success(function (result) {
            $rootScope.user = {};
            $state.go('index');
        });
    };

    $rootScope.saveUser = function (result) {
        $rootScope.user = {
            name: result.Response.Name,
            email: result.Response.Email,
            municipality: result.Response.Municipality_Id,
            authStatus: 'authorized',
            role: result.Response.RoleName
        };
    };

    var hasInitUser = false;
    var initUser = $http.get('api/authorize').success($rootScope.saveUser).finally(function() {
        hasInitUser = true;
    });
    
    function auth(toState, toParams) {
        var user = $rootScope.user;
        var authRoles = toState.authRoles;

        return (user.authStatus == 'authorized' && (!authRoles || _.indexOf(authRoles, user.role) != -1));
    }

    $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
        if (toState.noAuth) return; //no need to auth

        //need to auth, but first when the initUser call is finished.
        if (!hasInitUser) {

            //initUser is not yet loaded - wait for it
            event.preventDefault();
            
            initUser.finally(function() {
                var user = $rootScope.user;
                var userRole = user.role;
                var authRoles = toState.authRoles;

                if (!auth(toState, toParams)) {
                    $state.go('login', { to: toState.name, toParams: toParams });
                } else {
                    $state.go(toState, toParams);
                }
            });
        } else {
            //initUSer has loaded, just run auth()
            if (!auth(toState, toParams)) {
                event.preventDefault();
                $state.go('login', { to: toState.name, toParams: toParams });
            }
        }

    });
}]);