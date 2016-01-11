﻿module Kitos.ItProject.Edit {
    interface IRolesController {
        rights: Array<any>;
        activeItProjectRoles: Array<any>;
        newRole: string;
        selectedUser: any;
        orgId: number;

        rightSortChange(sort: string): void;
        submitRight(): void;
        deleteRight(right): void;
        updateRight(right): void;
    }

    class RolesController implements IRolesController {
        rights: Array<any>;
        activeItProjectRoles: Array<any>;
        newRole: string;
        selectedUser: any;
        orgId: number;
        rightSortBy: string;
        rightSortReverse: boolean;

        projectId: number;

        static $inject: Array<string> = [
            "$rootScope",
            "$scope",
            "$http",
            "notify",
            "project",
            "itProjectRights",
            "itProjectRoles",
            "user"
        ];

        constructor(
            $rootScope: ng.IScope,
            $scope: ng.IScope,
            private $http: ng.IHttpService,
            private notify,
            project,
            itProjectRights,
            private itProjectRoles,
            private user) {

            this.projectId = project.id;

            this.orgId = this.user.currentOrganizationId;
            this.activeItProjectRoles = _.where(this.itProjectRoles, { isActive: true });
            this.newRole = "1";

            this.rights = [];
            _.each(itProjectRights, (right: { role; roleId; show; userForSelect; roleForSelect; user; }) => {
                right.role = _.findWhere(this.itProjectRoles, { id: right.roleId });
                right.show = true;

                right.userForSelect = { id: right.user.id, text: right.user.fullName };
                right.roleForSelect = right.roleId;

                this.rights.push(right);
            });

            $scope.$watch(() => this.selectedUser, () => this.submitRight());

            this.rightSortBy = "roleName";
            this.rightSortReverse = false;
        }

        rightSortChange(val): void {
            if (this.rightSortBy === val) {
                this.rightSortReverse = !this.rightSortReverse;
            } else {
                this.rightSortReverse = false;
            }

            this.rightSortBy = val;
        }

        rightSort(right): string {
            switch (this.rightSortBy) {
                case "roleName":
                    return right.role.name;
                case "userName":
                    return right.user.fullName;
                case "userEmail":
                    return right.user.email;
                default:
                    return right.role.name;
            }
        }

        updateRight(right): void {
            if (!right.roleForSelect || !right.userForSelect) {
                return;
            }

            // old values
            var rIdOld = right.roleId;
            var uIdOld = right.userId;

            // new values
            var rIdNew = right.roleForSelect;
            var uIdNew = right.userForSelect.id;

            // if nothing was changed, just exit edit-mode
            if (rIdOld === rIdNew && uIdOld === uIdNew) {
                right.edit = false;
            }

            // otherwise, we should delete the old entry, then add a new one
            this.$http.delete(`api/itprojectright/${this.projectId}?rId=${rIdOld}&uId=${uIdOld}&organizationId=${this.user.currentOrganizationId}`)
                .then(
                    successResult => {
                        var data = {
                            "roleId": rIdNew,
                            "userId": uIdNew
                        };

                        this.$http.post(`api/itprojectright/${this.projectId}?organizationId=${this.user.currentOrganizationId}`, data)
                            .then(
                                (result: ng.IHttpPromiseCallbackArg<IApiResponse<any>>) => {
                                    right.roleId = result.data.response.roleId;
                                    right.user = result.data.response.user;
                                    right.userId = result.data.response.userId;

                                    right.role = _.findWhere(this.itProjectRoles, { id: right.roleId }),

                                    right.edit = false;

                                    this.notify.addSuccessMessage(right.user.fullName + " er knyttet i rollen");
                                },
                                result => {
                                    // we successfully deleted the old entry, but didn't add a new one
                                    // fuck
                                    right.show = false;

                                    this.notify.addErrorMessage("Fejl!");
                                }
                            );
                    },
                    errorResult => {
                        // couldn't delete the old entry, just reset select options
                        right.userForSelect = { id: right.user.id, text: right.user.fullName };
                        right.roleForSelect = right.roleId;

                        this.notify.addErrorMessage("Fejl!");
                    }
                );
        }

        deleteRight(right): void {
            var rId = right.roleId;
            var uId = right.userId;

            this.$http.delete(`api/itprojectright/${this.projectId}?rId=${rId}&uId=${uId}&organizationId=${this.user.currentOrganizationId}`)
                .then(
                    deleteResult => {
                        right.show = false;
                        this.notify.addSuccessMessage("Rollen er slettet!");
                    },
                    deleteResult => {
                        this.notify.addErrorMessage("Kunne ikke slette rollen!");
                    }
                );
        }

        submitRight(): void {
            if (!this.selectedUser || !this.newRole) {
                return;
            }

            var oId = this.projectId;
            var rId = parseInt(this.newRole, 10);
            var uId = this.selectedUser.id;

            if (!oId || !rId || !uId) {
                return;
            }

            var data = {
                "roleId": rId,
                "userId": uId
            };

            this.$http.post(`api/itprojectright/${oId}?organizationId=${this.user.currentOrganizationId}`, data)
                .then(
                    (result: ng.IHttpPromiseCallbackArg<IApiResponse<any>>) => {
                        this.notify.addSuccessMessage(result.data.response.user.fullName + " er knyttet i rollen");

                        this.rights.push({
                            objectId: result.data.response.objectId,
                            roleId: result.data.response.roleId,
                            userId: result.data.response.userId,
                            user: result.data.response.user,
                            userForSelect: { id: result.data.response.userId, text: result.data.response.user.fullName },
                            roleForSelect: result.data.response.roleId,
                            role: _.findWhere(this.itProjectRoles, { id: result.data.response.roleId }),
                            show: true
                        });

                        this.newRole = "1";
                        this.selectedUser = "";
                    },
                    result => this.notify.addErrorMessage("Fejl!")
                );
        }
    }

    angular
        .module("app")
        .config([
            "$stateProvider", ($stateProvider) => {
                $stateProvider.state("it-project.edit.roles", {
                    url: "/roles",
                    templateUrl: "app/components/it-project/tabs/it-project-tab-roles.view.html",
                controller: RolesController,
                    controllerAs: "projectRolesVm",
                resolve: {
                    // re-resolve data from parent cause changes here wont cascade to it
                        project: [
                            "$http", "$stateParams",
                            ($http, $stateParams) => $http.get(`api/itproject/${$stateParams.id}`)
                            .then(result => result.data.response)
                    ],
                        itProjectRights: [
                            "$http", "$stateParams",
                            ($http, $stateParams) => $http.get(`api/itprojectright/${$stateParams.id}`)
                            .then(result => result.data.response)
                    ],
                        itProjectRoles: [
                            "$http",
                            $http => $http.get("api/itprojectrole/?nonsuggestions=")
                            .then(result => result.data.response)
                    ],
                        user: [
                            "userService",
                        userService => userService.getUser()
                            .then(user => user)
                    ]
                }
            });
            }
        ]);
}
