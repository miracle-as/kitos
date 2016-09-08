module Kitos.Organization.Users {
    "use strict";

    interface IControllerScope extends ng.IScope {

    }

    interface IGridModel extends Models.IUser {
        canEdit: boolean;
    }

    class OrganizationUserController {
        public mainGrid: IKendoGrid<IGridModel>;
        public mainGridOptions: IKendoGridOptions<IGridModel>;

        public static $inject: string[] = ["$scope", "$http", "$timeout", "_", "$", "$state", "$uibModal", "$q", "$stateParams", "notify", "user"];

        constructor(
            private $scope: ng.IScope,
            private $http: ng.IHttpService,
            private $timeout: ng.ITimeoutService,
            private _: ILoDashWithMixins,
            private $: JQueryStatic,
            private $state: ng.ui.IStateService,
            private $uibModal: ng.ui.bootstrap.IModalService,
            private $q: ng.IQService,
            private $stateParams: ng.ui.IStateParamsService,
            private notify,
            private user) {
            this.mainGridOptions = {
                dataSource: {
                    type: "odata-v4",
                    transport: {
                        read: {
                            //url: `/odata/Organizations(${this.user.currentOrganizationId})/Rights?$expand=User,ObjectOwner`,
                            url: `/odata/Users?$filter=OrganizationRights/any(x: x/OrganizationId eq ${this.user.currentOrganizationId})&$expand=ObjectOwner,OrganizationRights($filter=OrganizationId eq ${this.user.currentOrganizationId})`,
                            dataType: "json"
                        },
                        destroy: {
                            url: (entity) => {
                                return `/odata/Users(${entity.Id})/Remove()`;
                            },
                            dataType: "json"
                        },
                        update: {
                            url: (entity) => {
                                return `/odata/Users(${entity.Id})`;
                            },
                            type: "PATCH",
                            dataType: "json",
                            contentType: "application/json"
                        },
                        parameterMap: (options, operation) => {
                            if (operation === "read") {
                                // get kendo to map parameters to an odata url
                                const parameterMap = kendo.data.transports["odata-v4"].parameterMap(options, operation);

                                if (parameterMap.$filter) {
                                    parameterMap.$filter = this.fixNameFilter(parameterMap.$filter, "Name");
                                    parameterMap.$filter = this.fixNameFilter(parameterMap.$filter, "ObjectOwner.Name");
                                }

                                return parameterMap;
                            }
                        }
                    },
                    sort: {
                        field: "Name",
                        dir: "asc"
                    },
                    pageSize: 100,
                    serverPaging: true,
                    serverSorting: true,
                    serverFiltering: true,
                    schema: {
                        model: {
                            id: "Id",
                            fields: {
                                Name: { type: "text" },
                                LastName: { type: "text" },
                                Email: { type: "text" },
                                LastAdvisDate: { type: "date" }
                            }
                        },
                        parse: response => {
                            // iterate each user
                            this._.forEach(response.value, (usr: IGridModel) => {
                                // set if the user can edit
                                if (this.user.isGlobalAdmin || this.user.isLocalAdmin) {
                                    usr.canEdit = true;
                                } else if (this.user.id === usr.Id) {
                                    usr.canEdit = true;
                                } else {
                                    usr.canEdit = false;
                                }

                                // remove the user role
                                this._.remove(usr.OrganizationRights, (right) => right.Role === Models.OrganizationRole.User);
                            });
                            return response;
                        }
                    }
                } as kendo.data.DataSourceOptions,
                toolbar: [
                    { name: "excel", text: "Eksportér til Excel", className: "pull-right" }
                ],
                excel: {
                    fileName: "Brugere.xlsx",
                    filterable: true,
                    allPages: true
                },
                pageable: {
                    refresh: true,
                    pageSizes: [10, 25, 50, 100, 200],
                    buttonCount: 5
                },
                sortable: {
                    mode: "single"
                },
                editable: "popup",
                reorderable: true,
                resizable: true,
                filterable: {
                    mode: "row"
                },
                groupable: false,
                columnMenu: {
                    filterable: false
                },
                detailTemplate: (dataItem) => `<uib-tabset active="0">
                    <uib-tab index="0" heading="Organisation roller"><user-organization-unit-roles user-id="${dataItem.Id}" current-organization-id="${this.user.currentOrganizationId}"></user-organization-unit-roles></uib-tab>
                    <uib-tab index="1" heading="Projekt roller"><user-project-roles user-id="${dataItem.Id}" current-organization-id="${this.user.currentOrganizationId}"></user-project-roles></uib-tab>
                    <uib-tab index="2" heading="System roller"><user-system-roles user-id="${dataItem.Id}" current-organization-id="${this.user.currentOrganizationId}"></user-system-roles></uib-tab>
                    <uib-tab index="3" heading="Kontrakt roller"><user-contract-roles user-id="${dataItem.Id}" current-organization-id="${this.user.currentOrganizationId}"></user-contract-roles></uib-tab>
                </uib-tabset>`,
                //dataBound: this.saveGridOptions,
                //columnResize: this.saveGridOptions,
                //columnHide: this.saveGridOptions,
                //columnShow: this.saveGridOptions,
                //columnReorder: this.saveGridOptions,
                excelExport: this.exportToExcel,
                columns: [
                    {
                        field: "Name", title: "Navn", width: 150,
                        persistId: "fullname", // DON'T YOU DARE RENAME!
                        template: (dataItem) => `${dataItem.Name} ${dataItem.LastName}`,
                        excelTemplate: (dataItem) => `${dataItem.Name} ${dataItem.LastName}`,
                        hidden: false,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "Email", title: "Email", width: 150,
                        persistId: "email", // DON'T YOU DARE RENAME!
                        template: (dataItem) => dataItem.Email,
                        excelTemplate: (dataItem) => dataItem.Email,
                        hidden: false,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "LastAdvisDate", title: "Advis", width: 150,
                        persistId: "advisdate", // DON'T YOU DARE RENAME!
                        template: (dataItem) => `<advis-button data-user="dataItem" data-current-organization-id="${this.user.currentOrganizationId}" data-ng-disabled="${!dataItem.canEdit}"></advis>`,
                        excelTemplate: (dataItem) => dataItem.LastAdvisDate ? dataItem.LastAdvisDate.toDateString() : "",
                        hidden: false,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "ObjectOwner.Name", title: "Oprettet af", width: 150,
                        persistId: "createdby", // DON'T YOU DARE RENAME!
                        template: (dataItem) => dataItem.ObjectOwner ? `${dataItem.ObjectOwner.Name} ${dataItem.ObjectOwner.LastName}` : "",
                        excelTemplate: (dataItem) => dataItem.ObjectOwner ? `${dataItem.ObjectOwner.Name} ${dataItem.ObjectOwner.LastName}` : "",
                        hidden: false,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "OrganizationRights.Role", title: "Rolle", width: 150,
                        persistId: "role", // DON'T YOU DARE RENAME!
                        attributes: { "class": "might-overflow" },
                        template: this.roleTemplate,
                        excelTemplate: this.roleTemplate,
                        hidden: false,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        command: [
                            { text: "Redigér", click: this.onEdit, imageClass: "k-edit", className: "k-custom-edit", iconClass: "k-icon" } /* kendo typedef is missing imageClass and iconClass so casting to any */ as any,
                            { text: "Slet", click: this.onDelete, imageClass: "k-delete", className: "k-custom-delete", iconClass: "k-icon" } /* kendo typedef is missing imageClass and iconClass so casting to any */ as any,
                        ],
                        title: " ", width: "150px",
                        persistId: "command"
                    }
                ]
            };
        }

        private roleTemplate = (dataItem: IGridModel) => {
            var roleNames = this._.pluck(dataItem.OrganizationRights, "Role");
            this._.forEach(roleNames, (roleName, index) => {
                switch (roleName) {
                    case Models.OrganizationRole.LocalAdmin: roleNames[index] = "Lokal Admin"; break;
                    case Models.OrganizationRole.OrganizationModuleAdmin: roleNames[index] = "Organisations Admin"; break;
                    case Models.OrganizationRole.ProjectModuleAdmin: roleNames[index] = "Projekt Admin"; break;
                    case Models.OrganizationRole.SystemModuleAdmin: roleNames[index] = "System Admin"; break;
                    case Models.OrganizationRole.ContractModuleAdmin: roleNames[index] = "Kontrakt Admin"; break;
                }
            });
            return roleNames.join(",");
        }

        private fixNameFilter(filterUrl, column) {
            const pattern = new RegExp(`(\\w+\\()${column}(.*?\\))`, "i");
            return filterUrl.replace(pattern, `$1concat(concat(Name, ' '), LastName)$2`);
        }

        private onEdit = (e: JQueryEventObject) => {
            e.preventDefault();
            var dataItem = this.mainGrid.dataItem(this.$(e.currentTarget).closest("tr"));
            var entityId = dataItem["Id"];
            //this.$state.go("organization.user.edit", { id: entityId, userObj: dataItem });
            dataItem.set("Email", "test@test");
            this.mainGrid.dataSource.sync();
        }

        private onDelete = (e: JQueryEventObject) => {
            e.preventDefault();
            var dataItem = this.mainGrid.dataItem(this.$(e.currentTarget).closest("tr"));
            var entityId = dataItem["Id"];
            console.log("delete", entityId);
        }

        private exportFlag = false;
        private exportToExcel = (e: IKendoGridExcelExportEvent<Models.IOrganizationRight>) => {
            var columns = e.sender.columns;

            if (!this.exportFlag) {
                e.preventDefault();
                this._.forEach(columns, column => {
                    if (column.hidden) {
                        column.tempVisual = true;
                        e.sender.showColumn(column);
                    }
                });
                this.$timeout(() => {
                    this.exportFlag = true;
                    e.sender.saveAsExcel();
                });
            } else {
                this.exportFlag = false;

                // hide columns on visual grid
                this._.forEach(columns, column => {
                    if (column.tempVisual) {
                        delete column.tempVisual;
                        e.sender.hideColumn(column);
                    }
                });

                // render templates
                const sheet = e.workbook.sheets[0];

                // skip header row
                for (let rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
                    const row = sheet.rows[rowIndex];

                    // -1 as sheet has header and dataSource doesn't
                    const dataItem = e.data[rowIndex - 1];

                    for (let columnIndex = 0; columnIndex < row.cells.length; columnIndex++) {
                        if (columns[columnIndex].field === "") continue;
                        const cell = row.cells[columnIndex];
                        const template = this.getTemplateMethod(columns[columnIndex]);

                        cell.value = template(dataItem);
                    }
                }

                // hide loading bar when export is finished
                kendo.ui.progress(this.mainGrid.element, false);
            }
        }

        private getTemplateMethod(column) {
            let template: Function;

            if (column.excelTemplate) {
                template = column.excelTemplate;
            } else if (typeof column.template === "function") {
                template = (column.template as Function);
            } else if (typeof column.template === "string") {
                template = kendo.template(column.template as string);
            } else {
                template = t => t;
            }

            return template;
        }

        //private isLocalAdmin(selectedUser) {
        //    var uId = selectedUser.id;
        //    var oId = this.user.currentOrganizationId;

        //    return this.$http.get("api/OrganizationRight/?roleName=LocalAdmin&userId=" + uId + "&organizationId=" + oId + "&orgRightsForUserWithRole=").success(function (result) {
        //        return result.response;
        //    }).error(function (error) {
        //        this.notify.addErrorMessage("Fejl. Noget gik galt!");
        //    });
        //}

        //private loadUsers() {
        //    var deferred = this.$q.defer();

        //    var url = 'api/user?overview';
        //    url += '&orgId=' + this.user.currentOrganizationId;
        //    url += '&skip=' + this.pagination.skip;
        //    url += '&take=' + this.pagination.take;

        //    if (this.pagination.orderBy) {
        //        url += '&orderBy=' + this.pagination.orderBy;
        //        if (this.pagination.descending) url += '&descending=' + this.pagination.descending;
        //    }

        //    if (this.pagination.search)
        //        url += '&q=' + this.pagination.search;
        //    else
        //        url += "&q=";

        //    this.users = [];
        //    this.$http.get(url).success(function (result, status, headers) {

        //        var paginationHeader = JSON.parse(headers('X-Pagination'));
        //        this.totalCount = paginationHeader.TotalCount;

        //        this.users = result.response;
        //        deferred.resolve();
        //    }).error(function () {
        //        this.notify.addErrorMessage("Kunne ikke hente brugere!");
        //        deferred.reject();
        //    });

        //    return deferred.promise;
        //}

        ////Goes through a collection of users, and for each user sets canBeEdited flag
        ////Returns a flattened promise, that resolves when all users in the collection has been resolved
        //private setCanEdit(userCollection) {
        //    return this.$q.all(_.map(userCollection, function (iteratee: { adminRights; canBeEdited; id; }) {
        //        var deferred = this.$q.defer();

        //        iteratee.adminRights = _.findWhere(iteratee.adminRights, { roleName: "Medarbejder", organizationId: this.user.currentOrganizationId });

        //        setTimeout(function () {
        //            this.$http.get("api/user/" + iteratee.id + "?hasWriteAccess" + '&organizationId=' + this.user.currentOrganizationId)
        //                .success(function (result) {
        //                    iteratee.canBeEdited = result.response;
        //                    deferred.resolve(iteratee);
        //                })
        //                .error(function (result) {
        //                    iteratee.canBeEdited = false;
        //                    deferred.reject(result);
        //                }
        //                );
        //        }, 0);

        //        return deferred.promise;
        //    }));
        //}

        //private updateUser(userToUpdate, successmessage, showNotify) {

        //    var deferred = this.$q.defer();

        //    setTimeout(function () {
        //        if (showNotify)
        //            deferred.notify('Ændrer...');
        //        this.$http({ method: 'PATCH', url: "api/user/" + userToUpdate.id + "?organizationId=" + this.user.currentOrganizationId, data: userToUpdate, handleBusy: true })
        //            .success(function (result) {
        //                deferred.resolve(successmessage);
        //            })
        //            .error(function (result, status) {
        //                if (status === 409) {
        //                    deferred.reject("Fejl! Kan ikke ændre Email for " + userToUpdate.fullName + " da den allerede findes!");
        //                    this.reload();
        //                } else {
        //                    deferred.reject("Fejl! " + userToUpdate.fullName + " kunne ikke ændres!");
        //                }
        //            });
        //    }, 0);

        //    return deferred.promise;
        //}

        //private deleteOrgRoleAction(u) {
        //    var uId = u.id;

        //    var msg = this.notify.addInfoMessage("Arbejder ...", false);

        //    this.$http.delete("api/OrganizationRight/?orgId=" + this.user.currentOrganizationId + "&userId=" + uId + "&byOrganization=").success(function (deleteResult) {
        //        msg.toSuccessMessage(u.name + " " + u.lastName + " er ikke længere tilknyttet organisationen");
        //        this.reload();
        //    }).error(function (deleteResult) {
        //        msg.toErrorMessage("Kunne ikke fjerne " + u.name + " " + u.lastName + " fra organisationen");
        //    });
        //}

        //private reload() {
        //    this.$state.go('.', { lastModule: this.chosenModule }, { reload: true });
        //}

        //public toggleStatus(userToToggle) {
        //    userToToggle.isLocked = !userToToggle.isLocked;
        //    var success = userToToggle.isLocked ? userToToggle.name + " er låst" : userToToggle.name + " er låst op";
        //    this.updateUser(userToToggle, success, null).then( //success
        //        function (successMessage) {
        //            this.notify.addSuccessMessage(successMessage);
        //        },
        //        //failure
        //        function (errorMessage) {
        //            this.notify.addErrorMessage(errorMessage);
        //        },
        //        //update
        //        function (updateMessage) {
        //            this.notify.addInfoMessage(updateMessage);
        //        });
        //}

        ////remove a users organizationRole - thereby removing their readaccess for this organization
        //public deleteOrgRole(u) {
        //    this.isLocalAdmin(u).then(function (response) {
        //        if (response.data.response === true) {
        //            var confirmBox = confirm(u.name + " " + u.lastName + " er også lokaladministrator!\n\nVil du fortsat fjerne tilknytning?");
        //            if (confirmBox) {
        //                this.deleteOrgRoleAction(u);
        //            } else {
        //                this.notify.addInfoMessage("Handling afbrudt!");
        //            }
        //        } else {
        //            this.deleteOrgRoleAction(u);
        //        }
        //    });
        //};

        //public getRightsForModule = function (chosenModule) {
        //    //return a flat promise, that fullfills when all rights have been retrieved
        //    return this.$q.all(_.map(this.users, function (iteratee: { id; rights; }) {
        //        var deferred = this.$q.defer();

        //        setTimeout(function () {
        //            var httpUrl = 'api/';

        //            switch (chosenModule) {
        //                //Choose Modul selected
        //                case '0':
        //                    iteratee.rights = '';
        //                    return deferred.resolve();
        //                //Organisation selected
        //                case '1':
        //                    httpUrl += 'organizationunitright?orgId=' + this.user.currentOrganizationId;
        //                    break;
        //                //ITProjects selected
        //                case '2':
        //                    httpUrl += 'itprojectright?';
        //                    break;
        //                //ITSystems selected
        //                case '3':
        //                    httpUrl += 'itSystemUsageRights?';
        //                    break;
        //                //ITContracts selected
        //                case '4':
        //                    httpUrl += 'itcontractrights?';
        //                    break;
        //            }

        //            httpUrl += '&userId=' + iteratee.id;
        //            return this.$http.get(httpUrl, { handleBusy: true })
        //                .success(function (result) {
        //                    iteratee.rights = result.response;
        //                    deferred.resolve();
        //                })
        //                .error(function (result) {
        //                    deferred.reject();
        //                });
        //        }, 0);

        //        return deferred.promise;
        //    }));
        //}

        //public editUser = function (userToEdit) {
        //    var modal = this.$uibModal.open({
        //        // fade in instead of slide from top, fixes strange cursor placement in IE
        //        // http://stackoverflow.com/questions/25764824/strange-cursor-placement-in-modal-when-using-autofocus-in-internet-explorer
        //        windowClass: 'modal fade in',
        //        templateUrl: 'app/components/org/user/org-user-modal-edit.view.html',
        //        controller: [
        //            '$scope', '$uibModalInstance', 'notify', 'autofocus', function ($modalScope, $modalInstance, modalnotify, autofocus) {
        //                autofocus();
        //                $modalScope.busy = false;
        //                $modalScope.name = userToEdit.name;
        //                $modalScope.email = userToEdit.email;
        //                $modalScope.repeatEmail = userToEdit.email;
        //                $modalScope.lastName = userToEdit.lastName;
        //                $modalScope.phoneNumber = userToEdit.phoneNumber;
        //                $modalScope.ok = function () {
        //                    $modalScope.busy = true;
        //                    userToEdit.name = $modalScope.name;
        //                    userToEdit.email = $modalScope.email;
        //                    userToEdit.lastName = $modalScope.lastName;
        //                    userToEdit.phoneNumber = $modalScope.phoneNumber;
        //                    var msg = this.notify.addInfoMessage("Ændrer");
        //                    this.updateUser(userToEdit, userToEdit.name + " er ændret.", true).then(
        //                        //success
        //                        function (successMessage) {
        //                            msg.toSuccessMessage(successMessage);
        //                            $modalInstance.close();
        //                            this.reload();
        //                        },
        //                        //failure
        //                        function (errorMessage) {
        //                            msg.toErrorMessage(errorMessage);
        //                            $modalInstance.close();
        //                        },
        //                        //update
        //                        function (updateMessage) {
        //                            msg.toInfoMessage(updateMessage);
        //                        });
        //                };
        //                $modalScope.cancel = function () {
        //                    $modalInstance.close();
        //                };
        //            }
        //        ]
        //    });

        //    modal.result.then(function () { });
        //}

        //public sendAdvis = function (userToAdvis, reminder) {
        //    var params: { sendReminder; sendAdvis; organizationId; } = { sendReminder: null, sendAdvis: null, organizationId: null };
        //    var type;

        //    if (reminder) {
        //        params.sendReminder = true;
        //        type = "påmindelse";

        //    } else {
        //        params.sendAdvis = true;
        //        type = "advis";
        //    }
        //    params.organizationId = this.user.currentOrganizationId;

        //    var msg = this.notify.addInfoMessage("Sender " + type + " til " + userToAdvis.email, false);
        //    this.$http.post("api/user", userToAdvis, { handleBusy: true, params: params })
        //        .success(function (result) {
        //            userToAdvis.lastAdvisDate = result.response.lastAdvisDate;
        //            msg.toSuccessMessage("Advis sendt til " + userToAdvis.email);
        //        })
        //        .error(function (result) {
        //            msg.toErrorMessage("Kunne ikke sende " + type + "!");
        //        })
        //        .then(function () { });
        //}
    }

    angular
        .module("app")
        .config(["$stateProvider", ($stateProvider) => {
            $stateProvider.state("organization.user", {
                url: "/user",
                templateUrl: "app/components/org/user/org-user.view.html",
                controller: OrganizationUserController,
                controllerAs: "ctrl",
                resolve: {
                    user: [
                        "userService", userService => userService.getUser()
                    ]
                }
            });
        }
    ]);
}
