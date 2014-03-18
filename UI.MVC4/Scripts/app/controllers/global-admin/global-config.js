﻿(function(ng, app) {
    var subnav = [
        { state: "config-org", text: "Organisation" },
        { state: "config-project", text: "IT Projekt" },
        { state: "config-system", text: "IT System" },
        { state: "config-contract", text: "IT Kontrakt" }
    ];

    app.config(['$stateProvider', '$urlRouterProvider', function($stateProvider, $urlRouterProvider) {

        $stateProvider.state('config-org', {
            url: '/global-config/org',
            templateUrl: 'partials/global-config/org.html',
            controller: 'globalConfig.OrgCtrl',
            authRoles: ['GlobalAdmin']
        }).state('config-project', {
            url: '/global-config/project',
            templateUrl: 'partials/global-config/project.html',
            controller: 'globalConfig.ProjectCtrl',
            authRoles: ['GlobalAdmin']
        }).state('config-system', {
            url: '/global-config/system',
            templateUrl: 'partials/global-config/system.html',
            controller: 'globalConfig.SystemCtrl',
            authRoles: ['GlobalAdmin']
        }).state('config-contract', {
            url: '/global-config/contract',
            templateUrl: 'partials/global-config/contract.html',
            controller: 'globalConfig.ContractCtrl',
            authRoles: ['GlobalAdmin']
        });
    }]);

    app.controller('globalConfig.OrgCtrl', ['$rootScope', '$scope', 'Restangular', 'growl', function($rootScope, $scope, Restangular, growl) {
        $rootScope.page.title = 'Global konfiguration';
        $rootScope.page.subnav = subnav;

        $scope.patchRole = function (data, i) {
            var item = $scope.roles[i];
            var result = Restangular.one('DepartmentRole', item.Id).patch({ IsActive: data }).then(function () {
                updateRoles();
            });
            return result;
        };

        $scope.patchSuggestion = function (data, i) {
            var item = $scope.roleSuggestions[i];
            var result = Restangular.one('DepartmentRole', item.Id).patch({ IsSuggestion: data }).then(function () {
                updateRoles();
            });
            return result;
        };

        var baseRoles = Restangular.all('DepartmentRole');
        function updateRoles() {
            baseRoles.getList({ nonsuggestions: true }).then(function (roles) {
                $scope.roles = roles;
            });

            baseRoles.getList({ suggestions: true }).then(function (roles) {
                $scope.roleSuggestions = roles;
            });
        };
        updateRoles();
    }]);

    app.controller('globalConfig.ProjectCtrl', ['$rootScope', '$scope', 'Restangular', 'growl', function ($rootScope, $scope, Restangular, growl) {
        $rootScope.page.title = 'Global konfiguration';
        $rootScope.page.subnav = subnav;
        
        var baseCategories = Restangular.all('projectCategory');
        baseCategories.getList({nonsuggestions: true}).then(function (categories) {
            $scope.categories = categories;
        });
        
        $scope.patchProjectCategory = function (data, i) {
            var item = $scope.categories[i];
            return Restangular.one('projectCategory', item.Id).patch({IsActive: data});
        };
        
        var basePhases = Restangular.all('ProjectPhase');
        basePhases.getList({ nonsuggestions: true }).then(function (phases) {
            $scope.phases = phases;
        });
        
        $scope.patchPhase = function (data, i) {
            var item = $scope.phases[i];
            return Restangular.one('ProjectPhase', item.Id).patch({ Name: data });
        };

        var baseRefs = Restangular.all('extReferenceType');
        baseRefs.getList({ nonsuggestions: true }).then(function (refs) {
            $scope.refs = refs;
        });
        
        $scope.patchRef = function (data, i) {
            var item = $scope.refs[i];
            return Restangular.one('extReferenceType', item.Id).patch({ Name: data });
        };
               
        $scope.patchRole = function (data, i) {
            var item = $scope.roles[i];
            var result = Restangular.one('ItProjectRole', item.Id).patch({ IsActive: data }).then(function() {
                updateRoles();
            });
            return result;
        };
        
        $scope.patchSuggestion = function (data, i) {
            var item = $scope.roleSuggestions[i];
            var result = Restangular.one('ItProjectRole', item.Id).patch({ IsSuggestion: data }).then(function() {
                updateRoles();
            });
            return result;
        };

        var baseRoles = Restangular.all('ItProjectRole');
        function updateRoles() {
            baseRoles.getList({ nonsuggestions: true }).then(function(roles) {
                $scope.roles = roles;
            });
            
            baseRoles.getList({ suggestions: true }).then(function (roles) {
                $scope.roleSuggestions = roles;
            });
        };
        updateRoles();
    }]);

    app.controller('globalConfig.SystemCtrl', ['$rootScope', '$scope', 'Restangular', 'growl', function($rootScope, $scope, Restangular, growl) {
        $rootScope.page.title = 'Global konfiguration';
        $rootScope.page.subnav = subnav;
        
        var baseRefs = Restangular.all('extReferenceType');
        baseRefs.getList({ nonsuggestions: true }).then(function (refs) {
            $scope.refs = refs;
        });

        $scope.patchRef = function (data, i) {
            var item = $scope.refs[i];
            return Restangular.one('extReferenceType', item.Id).patch({ Name: data });
        };

        $scope.patchRole = function (data, i) {
            var item = $scope.roles[i];
            var result = Restangular.one('ItSystemRole', item.Id).patch({ IsActive: data }).then(function () {
                updateRoles();
            });
            return result;
        };

        $scope.patchRoleSuggestion = function (data, i) {
            var item = $scope.roleSuggestions[i];
            var result = Restangular.one('ItSystemRole', item.Id).patch({ IsSuggestion: data }).then(function () {
                updateRoles();
            });
            return result;
        };

        var baseRoles = Restangular.all('ItSystemRole');
        function updateRoles() {
            baseRoles.getList({ nonsuggestions: true }).then(function (list) {
                $scope.roles = list;
            });

            baseRoles.getList({ suggestions: true }).then(function (list) {
                $scope.roleSuggestions = list;
            });
        };
        updateRoles();
        
        $scope.patchType = function (data, i) {
            var item = $scope.roles[i];
            var result = Restangular.one('SystemType', item.Id).patch({ IsActive: data }).then(function () {
                updateTypes();
            });
            return result;
        };

        $scope.patchTypeSuggestion = function (data, i) {
            var item = $scope.roleSuggestions[i];
            var result = Restangular.one('SystemType', item.Id).patch({ IsSuggestion: data }).then(function () {
                updateTypes();
            });
            return result;
        };

        var baseTypes = Restangular.all('SystemType');
        function updateTypes() {
            baseTypes.getList({ nonsuggestions: true }).then(function (list) {
                $scope.types = list;
            });

            baseTypes.getList({ suggestions: true }).then(function (list) {
                $scope.typeSuggestions = list;
            });
        };
        updateTypes();
        
        $scope.patchInterface = function (data, i) {
            var item = $scope.roles[i];
            var result = Restangular.one('InterfaceType', item.Id).patch({ IsActive: data }).then(function () {
                updateInterfaces();
            });
            return result;
        };

        $scope.patchInterfaceSuggestion = function (data, i) {
            var item = $scope.roleSuggestions[i];
            var result = Restangular.one('InterfaceType', item.Id).patch({ IsSuggestion: data }).then(function () {
                updateInterfaces();
            });
            return result;
        };

        var baseInterfaces = Restangular.all('InterfaceType');
        function updateInterfaces() {
            baseInterfaces.getList({ nonsuggestions: true }).then(function (list) {
                $scope.interfaces = list;
            });

            baseInterfaces.getList({ suggestions: true }).then(function (list) {
                $scope.interfacesSuggestions = list;
            });
        };
        updateInterfaces();
        
        $scope.patchProtocol = function (data, i) {
            var item = $scope.roles[i];
            var result = Restangular.one('ProtocolType', item.Id).patch({ IsActive: data }).then(function () {
                updateProtocols();
            });
            return result;
        };

        $scope.patchProtocolSuggestion = function (data, i) {
            var item = $scope.roleSuggestions[i];
            var result = Restangular.one('ProtocolType', item.Id).patch({ IsSuggestion: data }).then(function () {
                updateProtocols();
            });
            return result;
        };

        var baseProtocols = Restangular.all('ProtocolType');
        function updateProtocols() {
            baseProtocols.getList({ nonsuggestions: true }).then(function (list) {
                $scope.protocols = list;
            });

            baseProtocols.getList({ suggestions: true }).then(function (list) {
                $scope.protocolsSuggestions = list;
            });
        };
        updateProtocols();
        
        $scope.patchMethod = function (data, i) {
            var item = $scope.roles[i];
            var result = Restangular.one('Method', item.Id).patch({ IsActive: data }).then(function () {
                updateMethods();
            });
            return result;
        };

        $scope.patchProtocolSuggestion = function (data, i) {
            var item = $scope.roleSuggestions[i];
            var result = Restangular.one('Method', item.Id).patch({ IsSuggestion: data }).then(function () {
                updateMethods();
            });
            return result;
        };

        var baseMethods = Restangular.all('Method');
        function updateMethods() {
            baseMethods.getList({ nonsuggestions: true }).then(function (list) {
                $scope.methods = list;
            });

            baseMethods.getList({ suggestions: true }).then(function (list) {
                $scope.methodsSuggestions = list;
            });
        };
        updateMethods();
    }]);
})(angular, app);