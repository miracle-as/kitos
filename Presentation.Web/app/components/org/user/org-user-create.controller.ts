module Kitos.Organization.Users {
    "use strict";

    class CreateOrganizationUserController {
        public checkAvailbleUrl: string;
        public checkOrgUserUrl: string;
        public busy: boolean;
        public name: string;
        public email: string;
        public lastName: string;
        public phoneNumber: string;

        public static $inject: string[] = ["$uibModalInstance", "$http", "notify", "autofocus", "user"];

        constructor(private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance, private $http: IHttpServiceWithCustomConfig, private notify, private autofocus, private user) {
            if (!user.currentOrganizationId) {
                notify.addErrorMessage("Fejl! Kunne ikke oprette bruger.", true);
                return;
            }

            this.checkAvailbleUrl = "api/user";
            this.checkOrgUserUrl = "api/user";

            autofocus();
            this.busy = false;
        }

        public cancel() {
            this.$uibModalInstance.close();
        }

        public create(sendMail) {
            this.busy = true;
            var newUser = {
                name: this.name,
                email: this.email,
                lastName: this.lastName,
                phoneNumber: this.phoneNumber
            };

            const params: { organizationId; sendMailOnCreation; } = { organizationId: this.user.currentOrganizationId, sendMailOnCreation: null };
            // set params if sendMail is true
            if (sendMail) {
                params.sendMailOnCreation = sendMail;
            }

            var msg = this.notify.addInfoMessage("Opretter bruger", false);

            this.$http.post<API.Models.IApiWrapper<any>>("api/user", newUser, { handleBusy: true, params: params }).then((result) => {
                var userResult = result.data.response;
                var oId = this.user.currentOrganizationId;

                var data = {
                    userId: userResult.id,
                    role: API.Models.OrganizationRole.User,
                };

                this.$http.post(`api/OrganizationRight/?rightByOrganizationRight&organizationId=${oId}&userId=${this.user.id}`, data, { handleBusy: true }).then(() => {
                    msg.toSuccessMessage(`${userResult.fullName} er oprettet i KITOS`);
                }, () => {
                    msg.toErrorMessage(`Kunne ikke tilknytte ${this.user.fullName} til organisationen!`);
                });

                this.$uibModalInstance.close(userResult);
            }, () => {
                msg.toErrorMessage(`Fejl! Noget gik galt ved oprettelsen af ${newUser.name}!`);
                this.$uibModalInstance.close();
            });
        }
    }

    angular
        .module("app")
        .config(["$stateProvider", ($stateProvider: ng.ui.IStateProvider) => {
            $stateProvider.state("organization.user.create", {
                url: "/create",
                onEnter: [
                    "$state", "$stateParams", "$uibModal",
                    ($state: ng.ui.IStateService, $stateParams: ng.ui.IStateParamsService, $uibModal: ng.ui.bootstrap.IModalService) => {
                        $uibModal.open({
                            templateUrl: "app/components/org/user/org-user-modal-create.view.html",
                            // fade in instead of slide from top, fixes strange cursor placement in IE
                            // http://stackoverflow.com/questions/25764824/strange-cursor-placement-in-modal-when-using-autofocus-in-internet-explorer
                            windowClass: "modal fade in",
                            controller: CreateOrganizationUserController,
                            controllerAs: "ctrl",
                            resolve: {
                                user: ["userService", userService => userService.getUser()]
                            }
                        }).result.then(() => {
                            // OK
                            // GOTO parent state and reload
                            $state.go("^", null, { reload: true });
                        },
                        () => {
                            // Cancel
                            // GOTO parent state
                            $state.go("^");
                        });
                    }
                ]
            });
        }]);
}
