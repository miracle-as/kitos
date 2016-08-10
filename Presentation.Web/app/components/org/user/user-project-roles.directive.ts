module Kitos.Organization.Users {
    "use strict";

    interface IDirectiveScope extends ng.IScope {
        name: string;
    }

    class UserProjectRoles implements ng.IDirective {
        public templateUrl = "app/components/org/user/user-project-roles.view.html";
        public scope = {
            name: "="
        };

        public static directiveName = "userProjectRoles";
        public static $inject: string[] = ["$http"];

        constructor(private $http: ng.IHttpService) {
        }

        public link(scope: IDirectiveScope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes) {

        }
    }

    angular.module("app")
        .directive(UserProjectRoles.directiveName, DirectiveFactory.getFactoryFor(UserProjectRoles));
}
