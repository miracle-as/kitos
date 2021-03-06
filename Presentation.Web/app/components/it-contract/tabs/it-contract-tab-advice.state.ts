﻿(function (ng, app) {
    app.config(['$stateProvider', function ($stateProvider) {
        $stateProvider.state('it-contract.edit.advice-generic', {
            url: '/advice',
            templateUrl: 'app/components/it-advice/it-advice.view.html',
            controller: 'object.EditAdviceCtrl',
            controllerAs: 'Vm',
            resolve: {
                Roles: ['$http', function ($http) {
                    return $http.get("odata/LocalItContractRoles?$filter=IsLocallyAvailable eq true or IsObligatory&$orderby=Priority desc")
                        .then(function (result) {
                            return result.data.value;
                        });
                }],
                advices: ['$http', '$stateParams', function ($http, $stateParams) {
                    return $http.get('api/itcontract/' + $stateParams.id).then(function (result) {
                        return result.data.response.advices;
                    });
                }],
                object: ['contract', function (contract) {
                    return contract;
                }],
                type: [function () {
                    return "itContract";
                }],
                advicename: [
                    '$http', '$stateParams', function ($http, $stateParams) {
                        return $http.get('api/itcontract/' + $stateParams.id).then(function (result) {
                            return result.data.response;
                        });
                    }
                ],
                hasWriteAccess: [
                    '$http', '$stateParams', 'user', function ($http, $stateParams, user) {
                        return $http.get("api/itcontract/" + $stateParams.id + "?hasWriteAccess=true&organizationId=" + user.currentOrganizationId)
                            .then(function (result) {
                                return result.data.response;
                            });
                    }]
            }
        });
    }]);
})(angular, app);
