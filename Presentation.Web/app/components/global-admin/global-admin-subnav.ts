﻿(function (ng, app) {

    app.config(["$stateProvider", $stateProvider => {
        $stateProvider.state("global-admin", {
            url: "/global-admin",
            abstract: true,
            controller: "globalAdminConfig",
            template: "<ui-view autoscroll='false' />"
        });
    }]);

    app.controller("globalAdminConfig", ["$rootScope", $rootScope => {
        var subnav = [
            { state: "global-admin.organizations", text: "Organisationer" },
            { state: "global-admin.global-users", text: "Globale administratorer" },
            { state: "global-admin.local-users", text: "Lokale administratorer" },
            { state: "global-admin.org", text: "Organisation" },
            { state: "global-admin.project", text: "IT Projekt" },
            { state: "global-admin.system", text: "IT System" },
            { state: "global-admin.contract", text: "IT Kontrakt" },
            { state: "global-admin.misc", text: "Andet" }
        ];

        $rootScope.page.title = "Global admin";
        $rootScope.page.subnav = subnav;

    }]);

})(angular, app);
