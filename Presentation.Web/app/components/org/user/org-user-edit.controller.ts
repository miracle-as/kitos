module Kitos.Organization.Users {
    "use strict";

    class EditOrganizationUserController {
        public checkAvailbleUrl: string;
        public checkOrgUserUrl: string;
        public busy: boolean;
        public name: string;
        public email: string;
        public lastName: string;
        public phoneNumber: string;

        private userId: number;

        public static $inject: string[] = ["$uibModalInstance", "$stateParams", "$http", "notify"];

        constructor(private $uibModalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private $stateParams: ng.ui.IStateParamsService,
            private $http: IHttpServiceWithCustomConfig,
            private notify,
            private user) {
            this.userId = $stateParams["id"];
            var userObj: Models.IUser = $stateParams["userObj"];
            this.email = userObj.Email;
            this.name = userObj.Name;
            this.lastName = userObj.LastName;
            this.phoneNumber = userObj.PhoneNumber;
        }

        public ok() {
            var payload = {
                name: this.name,
                lastName: this.lastName,
                phoneNumber: this.phoneNumber,
                email: this.email
            };
            this.$http.patch(`/odata/Users(${this.userId})`, payload).then(() => {
                this.$uibModalInstance.close();
            });
        }

        public cancel() {
            this.$uibModalInstance.dismiss();
        }
    }

    angular
        .module("app")
        .config(["$stateProvider", ($stateProvider: ng.ui.IStateProvider) => {
            $stateProvider.state("organization.user.edit", {
                url: "/:id/edit",
                params: {
                    userObj: null // default value
                },
                onEnter: [
                    "$state", "$stateParams", "$uibModal",
                    ($state: ng.ui.IStateService, $stateParams: ng.ui.IStateParamsService, $uibModal: ng.ui.bootstrap.IModalService) => {
                        $uibModal.open({
                            templateUrl: "app/components/org/user/org-user-modal-edit.view.html",
                            // fade in instead of slide from top, fixes strange cursor placement in IE
                            // http://stackoverflow.com/questions/25764824/strange-cursor-placement-in-modal-when-using-autofocus-in-internet-explorer
                            windowClass: "modal fade in",
                            controller: EditOrganizationUserController,
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
