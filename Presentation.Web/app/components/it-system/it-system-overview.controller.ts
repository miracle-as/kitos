﻿module Kitos.ItSystem.Overview {
    "use strict";

    export interface IOverviewController {
        mainGrid: IKendoGrid<IItSystemUsageOverview>;
        mainGridOptions: kendo.ui.GridOptions;
        roleSelectorOptions: kendo.ui.DropDownListOptions;
        //modal: kendo.ui.Window;
        //usageGrid: kendo.ui.Grid;
        //usageDetailsGrid: kendo.ui.GridOptions;
        //exhibitModal: kendo.ui.Window;
        //exhibitGrid: kendo.ui.Grid;
        //exhibitDetailsGrid: kendo.ui.GridOptions;
    }

    export interface IItSystemUsageOverview extends Models.ItSystemUsage.IItSystemUsage {
        roles: Array<string>;
    }

    // Here be dragons! Thou art forewarned.
    // Or perhaps it's samurais, because it's kendos terrible terrible framework that's the cause...
    export class OverviewController implements IOverviewController {
        private storageKey = "it-system-overview-options";
        private orgUnitStorageKey = "it-system-overview-orgunit";
        private gridState = this.gridStateService.getService(this.storageKey);

        public mainGrid: IKendoGrid<IItSystemUsageOverview>;
        public mainGridOptions: kendo.ui.GridOptions;

        //public usageGrid: kendo.ui.Grid;
        //public modal: kendo.ui.Window;

        //public exhibitGrid: kendo.ui.Grid;
        //public exhibitModal: kendo.ui.Window;

        public static $inject: Array<string> = [
            "$rootScope",
            "$scope",
            "$http",
            "$timeout",
            "$window",
            "$state",
            "$",
            "_",
            "moment",
            "notify",
            "systemRoles",
            "user",
            "gridStateService",
            "orgUnits",
            "$uibModal"
        ];

        constructor(
            private $rootScope: IRootScope,
            private $scope: ng.IScope,
            private $http: ng.IHttpService,
            private $timeout: ng.ITimeoutService,
            private $window: ng.IWindowService,
            private $state: ng.ui.IStateService,
            private $: JQueryStatic,
            private _: ILoDashWithMixins,
            private moment: moment.MomentStatic,
            private notify,
            private systemRoles: Array<any>,
            private user,
            private gridStateService: Services.IGridStateFactory,
            private orgUnits: Array<any>,
            private $modal) {
            $rootScope.page.title = "IT System - Overblik";

            $scope.$on("kendoWidgetCreated", (event, widget) => {
                // the event is emitted for every widget; if we have multiple
                // widgets in this controller, we need to check that the event
                // is for the one we're interested in.
                if (widget === this.mainGrid) {
                    this.loadGridOptions();
                    this.mainGrid.dataSource.read();

                    // show loadingbar when export to excel is clicked
                    // hidden again in method exportToExcel callback
                    $(".k-grid-excel").click(() => {
                        kendo.ui.progress(this.mainGrid.element, true);
                    });
                }
            });

            this.activate();
        }

        public createITSystem() {
            var self = this;

            var modalInstance = this.$modal.open({
                // fade in instead of slide from top, fixes strange cursor placement in IE
                // http://stackoverflow.com/questions/25764824/strange-cursor-placement-in-modal-when-using-autofocus-in-internet-explorer
                windowClass: 'modal fade in',
                templateUrl: 'app/components/it-system/it-system-modal-create.view.html',
                controller: ['$scope', '$uibModalInstance', function ($scope, $modalInstance) {
                    $scope.formData = {};
                    $scope.type = 'IT System';
                    $scope.checkAvailbleUrl = 'api/itSystem/';

                    $scope.submit = function () {
                        var payload = {
                            name: $scope.formData.name,
                            belongsToId: self.user.currentOrganizationId,
                            organizationId: self.user.currentOrganizationId,
                            taskRefIds: [],
                        };

                        var msg = self.notify.addInfoMessage('Opretter system...', false);
                        self.$http.post('api/itsystem', payload)
                            .success(function (result: any) {
                                msg.toSuccessMessage('Et nyt system er oprettet!');
                                var systemId = result.response.id;
                                $modalInstance.close(systemId);
                            }).error(function () {
                                msg.toErrorMessage('Fejl! Kunne ikke oprette et nyt system!');
                            });
                    };
                }]
            });

            modalInstance.result.then(function (id) {
                // modal was closed with OK
                self.$state.go('it-system.edit.interfaces', { id: id });
            });
        };

        // replaces "anything({roleName},'foo')" with "Rights/any(c: anything(concat(concat(c/User/Name, ' '), c/User/LastName),'foo') and c/RoleId eq {roleId})"
        private fixRoleFilter(filterUrl, roleName, roleId) {
            var pattern = new RegExp(`(\\w+\\()${roleName}(.*?\\))`, "i");
            return filterUrl.replace(pattern, `Rights/any(c: $1concat(concat(c/User/Name, ' '), c/User/LastName)$2 and c/RoleId eq ${roleId})`);
        }

        private fixKleIdFilter(filterUrl, column) {
            var pattern = new RegExp(`(\\w+\\()${column}(.*?\\))`, "i");
            return filterUrl.replace(pattern, "ItSystem/TaskRefs/any(c: $1c/TaskKey$2)");
        }

        private fixKleDescFilter(filterUrl, column) {
            var pattern = new RegExp(`(\\w+\\()${column}(.*?\\))`, "i");
            return filterUrl.replace(pattern, "ItSystem/TaskRefs/any(c: $1c/Description$2)");
        }

        // saves grid state to localStorage
        private saveGridOptions = () => {
            this.gridState.saveGridOptions(this.mainGrid);
        }

        // loads kendo grid options from localstorage
        private loadGridOptions() {
            this.gridState.loadGridOptions(this.mainGrid);
        }

        public saveGridProfile() {
            // the stored org unit id must be the current
            var currentOrgUnitId = this.$window.sessionStorage.getItem(this.orgUnitStorageKey);
            this.$window.localStorage.setItem(this.orgUnitStorageKey + "-profile", currentOrgUnitId);

            this.gridState.saveGridProfile(this.mainGrid);
            this.notify.addSuccessMessage("Filtre og sortering gemt");
        }

        public loadGridProfile() {
            this.gridState.loadGridProfile(this.mainGrid);

            var orgUnitId = this.$window.localStorage.getItem(this.orgUnitStorageKey + "-profile");
            // update session
            this.$window.sessionStorage.setItem(this.orgUnitStorageKey, orgUnitId);
            // find the org unit filter row section
            var orgUnitFilterRow = this.$(".k-filter-row [data-field='ResponsibleUsage.OrganizationUnit.Name']");
            // find the kendo widget
            var orgUnitFilterWidget = orgUnitFilterRow.find("input").data("kendoDropDownList");
            orgUnitFilterWidget.select(dataItem => dataItem.Id == orgUnitId);

            this.mainGrid.dataSource.read();
            this.notify.addSuccessMessage("Anvender gemte filtre og sortering");
        }

        public clearGridProfile() {
            this.$window.sessionStorage.removeItem(this.orgUnitStorageKey);
            this.gridState.removeProfile();
            this.gridState.removeSession();
            this.notify.addSuccessMessage("Filtre og sortering slettet");
            this.reload();
        }

        public doesGridProfileExist() {
            return this.gridState.doesGridProfileExist();
        }

        // clears grid filters by removing the localStorageItem and reloading the page
        public clearOptions() {
            this.$window.localStorage.removeItem(this.orgUnitStorageKey + "-profile");
            this.$window.sessionStorage.removeItem(this.orgUnitStorageKey);
            this.gridState.removeProfile();
            this.gridState.removeLocal();
            this.gridState.removeSession();
            this.notify.addSuccessMessage("Sortering, filtering og kolonnevisning, -bredde og –rækkefølge nulstillet");
            // have to reload entire page, as dataSource.read() + grid.refresh() doesn't work :(
            this.reload();
        };

        private reload() {
            this.$state.go(".", null, { reload: true });
        }

        private activate() {
            // overview grid options
            var mainGridOptions: IKendoGridOptions<IItSystemUsageOverview> = {
                autoBind: false, // disable auto fetch, it's done in the kendoRendered event handler
                dataSource: {
                    type: "odata-v4",
                    transport: {
                        read: {
                            url: (options) => {
                                var urlParameters = `?$expand=ItSystem($expand=AppTypeOption,BusinessType,Parent,TaskRefs),Organization,ResponsibleUsage($expand=OrganizationUnit),MainContract($expand=ItContract($expand=Supplier)),Rights($expand=User,Role),ArchiveType,SensitiveDataType,ObjectOwner,LastChangedByUser,ItProjects($select=Name)`;
                                // if orgunit is set then the org unit filter is active
                                var orgUnitId = this.$window.sessionStorage.getItem(this.orgUnitStorageKey);
                                if (orgUnitId === null) {
                                    return `/odata/Organizations(${this.user.currentOrganizationId})/ItSystemUsages` + urlParameters;
                                } else {
                                    return `/odata/Organizations(${this.user.currentOrganizationId})/OrganizationUnits(${orgUnitId})/ItSystemUsages` + urlParameters;
                                }
                            },
                            dataType: "json"
                        },
                        parameterMap: (options, type) => {
                            // get kendo to map parameters to an odata url
                            var parameterMap = kendo.data.transports["odata-v4"].parameterMap(options, type);

                            if (parameterMap.$filter) {
                                this._.forEach(this.systemRoles, role => {
                                    parameterMap.$filter = this.fixRoleFilter(parameterMap.$filter, `role${role.Id}`, role.Id);
                                });

                                parameterMap.$filter = this.fixKleIdFilter(parameterMap.$filter, "ItSystem/TaskRefs/TaskKey");
                                parameterMap.$filter = this.fixKleDescFilter(parameterMap.$filter, "ItSystem/TaskRefs/Description");

                                // replaces "contains(ItSystem/Uuid,'11')" with "contains(CAST(ItSystem/Uuid, 'Edm.String'),'11')"
                                parameterMap.$filter = parameterMap.$filter.replace(/contains\(ItSystem\/Uuid,/, "contains(CAST(ItSystem/Uuid, 'Edm.String'),");
                            }

                            return parameterMap;
                        }
                    },
                    sort: {
                        field: "ItSystem.Name",
                        dir: "asc"
                    },
                    pageSize: 100,
                    serverPaging: true,
                    serverSorting: true,
                    serverFiltering: true,
                    schema: {
                        model: {
                            fields: {
                                LastChanged: { type: "date" }
                            }
                        },
                        parse: response => {
                            // HACK to flattens the Rights on usage so they can be displayed as single columns

                            // iterrate each usage
                            this._.forEach(response.value, usage => {
                                usage.roles = [];
                                // iterrate each right
                                this._.forEach(usage.Rights, right => {
                                    // init an role array to hold users assigned to this role
                                    if (!usage.roles[right.RoleId])
                                        usage.roles[right.RoleId] = [];

                                    // push username to the role array
                                    usage.roles[right.RoleId].push([right.User.Name, right.User.LastName].join(" "));
                                });
                            });
                            return response;
                        }
                    }
                },
                toolbar: [
                    {
                        name: "createITSystem",
                        text: "Opret IT System",
                        template: "<a ng-click='systemOverviewVm.createITSystem()' class='btn btn-success pull-right'>#: text #</a>"
                    },
                    { name: "excel", text: "Eksportér til Excel", className: "pull-right" },
                    {
                        name: "clearFilter",
                        text: "Nulstil",
                        template: "<button type='button' class='k-button k-button-icontext' title='Nulstil sortering, filtering og kolonnevisning, -bredde og –rækkefølge' data-ng-click='systemOverviewVm.clearOptions()'>#: text #</button>"
                    },
                    {
                        name: "saveFilter",
                        text: "Gem filter",
                        template: '<button type="button" class="k-button k-button-icontext" title="Gem filtre og sortering" data-ng-click="systemOverviewVm.saveGridProfile()">#: text #</button>'
                    },
                    {
                        name: "useFilter",
                        text: "Anvend filter",
                        template: '<button type="button" class="k-button k-button-icontext" title="Anvend gemte filtre og sortering" data-ng-click="systemOverviewVm.loadGridProfile()" data-ng-disabled="!systemOverviewVm.doesGridProfileExist()">#: text #</button>'
                    },
                    {
                        name: "deleteFilter",
                        text: "Slet filter",
                        template: "<button type='button' class='k-button k-button-icontext' title='Slet filtre og sortering' data-ng-click='systemOverviewVm.clearGridProfile()' data-ng-disabled='!systemOverviewVm.doesGridProfileExist()'>#: text #</button>"
                    },
                    {
                        template: kendo.template(this.$("#role-selector").html())
                    }
                ],
                excel: {
                    fileName: "IT System Overblik.xlsx",
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
                reorderable: true,
                resizable: true,
                filterable: {
                    mode: "row"
                },
                groupable: false,
                columnMenu: {
                    filterable: false
                },
                dataBound: this.saveGridOptions,
                columnResize: this.saveGridOptions,
                columnHide: this.saveGridOptions,
                columnShow: this.saveGridOptions,
                columnReorder: this.saveGridOptions,
                excelExport: this.exportToExcel,
                columns: [
                    {
                        field: "LocalSystemId", title: "Lokal system ID", width: 150,
                        persistId: "localid", // DON'T YOU DARE RENAME!
                        excelTemplate: dataItem => dataItem && dataItem.LocalSystemId || "",
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "ItSystem.Uuid", title: "UUID", width: 150,
                        persistId: "uuid", // DON'T YOU DARE RENAME!
                        excelTemplate: dataItem => dataItem.ItSystem.Uuid,
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "ItSystem.Parent.Name", title: "Overordnet IT System", width: 150,
                        persistId: "parentsysname", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.ItSystem.Parent ? dataItem.ItSystem.Parent.Name : "",
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "ItSystem.Name", title: "IT System", width: 320,
                        persistId: "sysname", // DON'T YOU DARE RENAME!
                        template: dataItem => `<a data-ui-sref='it-system.usage.interfaces({id: ${dataItem.Id}})'>${dataItem.ItSystem.Name}</a>`,
                        excelTemplate: dataItem => dataItem && dataItem.ItSystem && dataItem.ItSystem.Name || "",
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "Version", title: "Version", width: 150,
                        persistId: "version", // DON'T YOU DARE RENAME!
                        excelTemplate: dataItem => dataItem && dataItem.Version || "",
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "LocalCallName", title: "Lokal kaldenavn", width: 150,
                        persistId: "localname", // DON'T YOU DARE RENAME!
                        excelTemplate: dataItem => dataItem && dataItem.LocalCallName || "",
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "ResponsibleUsage.OrganizationUnit.Name", title: "Ansv. organisationsenhed", width: 190,
                        persistId: "orgunit", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.ResponsibleUsage ? dataItem.ResponsibleUsage.OrganizationUnit.Name : "",
                        filterable: {
                            cell: {
                                showOperators: false,
                                template: this.orgUnitDropDownList
                            }
                        }
                    },
                    {
                        field: "ItSystem.BusinessType.Name", title: "Forretningstype", width: 150,
                        persistId: "busitype", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.ItSystem.BusinessType ? dataItem.ItSystem.BusinessType.Name : "",
                        attributes: { "class": "might-overflow" },
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "ItSystem.AppTypeOption.Name", title: "Applikationstype", width: 150,
                        persistId: "apptype", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.ItSystem.AppTypeOption ? dataItem.ItSystem.AppTypeOption.Name : "",
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "ItSystem.TaskRefs.TaskKey", title: "KLE ID", width: 150,
                        persistId: "taskkey", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.ItSystem.TaskRefs.length > 0 ? this._.map(dataItem.ItSystem.TaskRefs, "TaskKey").join(", ") : "",
                        attributes: { "class": "might-overflow" },
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "startswith"
                            }
                        },
                        sortable: false
                    },
                    {
                        field: "ItSystem.TaskRefs.Description", title: "KLE navn", width: 150,
                        persistId: "klename", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.ItSystem.TaskRefs.length > 0 ? this._.map(dataItem.ItSystem.TaskRefs, "Description").join(", ") : "",
                        attributes: { "class": "might-overflow" },
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        },
                        sortable: false
                    },
                    {
                        field: "EsdhRef", title: "ESDH ref", width: 150,
                        persistId: "esdh", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.EsdhRef ? `<a target="_blank" href="${dataItem.EsdhRef}"><i class="fa fa-link"></a>` : "",
                        excelTemplate: dataItem => dataItem && dataItem.EsdhRef || "",
                        attributes: { "class": "text-center" },
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "DirectoryOrUrlRef", title: "Mappe ref", width: 150,
                        persistId: "folderref", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.DirectoryOrUrlRef ? `<a target="_blank" href="${dataItem.DirectoryOrUrlRef}"><i class="fa fa-link"></i></a>` : "",
                        excelTemplate: dataItem => dataItem && dataItem.DirectoryOrUrlRef || "",
                        attributes: { "class": "text-center" },
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "CmdbRef", title: "CMDB ref", width: 150,
                        persistId: "cmdb", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.CmdbRef ? `<a target="_blank" href="${dataItem.CmdbRef}"><i class="fa fa-link"></i></a>` : "",
                        excelTemplate: dataItem => dataItem && dataItem.CmdbRef || "",
                        attributes: { "class": "text-center" },
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "ArchiveType.Name", title: "Arkivering", width: 150,
                        persistId: "archive", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.ArchiveType ? dataItem.ArchiveType.Name : "",
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "SensitiveDataType.Name", title: "Personfølsom", width: 150,
                        persistId: "sensitive", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.SensitiveDataType ? dataItem.SensitiveDataType.Name : "",
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    //{
                    //    field: "", title: "IT System: Anvendes af", width: 100,
                    //    persistId: "sysusage", // DON'T YOU DARE RENAME!
                    //    template: "TODO",
                    //    filterable: false,
                    //    sortable: false
                    //},
                    //{
                    //    field: "ItSystem.ItInterfaceExhibits", title: "Snitflader: Udstilles ???", width: 95,
                    //    persistId: "exhibit", // DON'T YOU DARE RENAME!
                    //    template: "<a data-ng-click=\"systemOverviewVm.showExposureDetails(#: ItSystem.Id #,'#: ItSystem.Name #')\">#: ItSystem.ItInterfaceExhibits.length #</a>",
                    //    hidden: true,
                    //    filterable: false,
                    //    sortable: false
                    //},
                    //{
                    //    field: "ItSystem.CanUseInterfaces", title: "Snitflader: Anvendes ???", width: 95,
                    //    persistId: "canuse", // DON'T YOU DARE RENAME!
                    //    template: "<a data-ng-click=\"systemOverviewVm.showUsageDetails(#: ItSystem.Id #,'#: ItSystem.Name #')\">#: ItSystem.CanUseInterfaces.length #</a>",
                    //    hidden: true,
                    //    filterable: false,
                    //    sortable: false
                    //},
                    {
                        field: "MainContract", title: "Kontrakt", width: 120,
                        persistId: "contract", // DON'T YOU DARE RENAME!
                        template: dataItem => {
                            if (!dataItem.MainContract || !dataItem.MainContract.ItContract) {
                                return "";
                            }

                            if (this.isContractActive(dataItem.MainContract.ItContract)) {
                                return `<a data-ui-sref="it-system.usage.contracts({id: ${dataItem.Id}})"><span class="fa fa-file text-success" aria-hidden="true"></span></a>`;
                            } else {
                                return `<a data-ui-sref="it-system.usage.contracts({id: ${dataItem.Id}})"><span class="fa fa-file-o text-muted" aria-hidden="true"></span></a>`;
                            }
                        },
                        excelTemplate: dataItem =>
                            dataItem && dataItem.MainContract && dataItem.MainContract.ItContract ? this.isContractActive(dataItem.MainContract.ItContract).toString() : "",
                        attributes: { "class": "text-center" },
                        sortable: false,
                        filterable: {
                            cell: {
                                showOperators: false,
                                template: this.contractFilterDropDownList
                            }
                        }
                    },
                    {
                        field: "MainContract.ItContract.Supplier.Name", title: "Leverandør", width: 175,
                        persistId: "supplier", // DON'T YOU DARE RENAME!
                        template: dataItem =>
                            dataItem.MainContract &&
                            dataItem.MainContract.ItContract &&
                            dataItem.MainContract.ItContract.Supplier &&
                            dataItem.MainContract.ItContract.Supplier.Name || "",
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "ItProjects", title: "IT Projekt", width: 150,
                        persistId: "sysusage", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.ItProjects.length > 0 ? this._.first(dataItem.ItProjects).Name : "",
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "ObjectOwner.Name", title: "Taget i anvendelse af", width: 150,
                        persistId: "ownername", // DON'T YOU DARE RENAME!
                        template: dataItem => `${dataItem.ObjectOwner.Name} ${dataItem.ObjectOwner.LastName}`,
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "LastChangedByUser.Name", title: "Sidst redigeret: Bruger", width: 150,
                        persistId: "lastchangedname", // DON'T YOU DARE RENAME!
                        template: dataItem => `${dataItem.LastChangedByUser.Name} ${dataItem.LastChangedByUser.LastName}`,
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "LastChanged", title: "Sidste redigeret: Dato", format: "{0:dd-MM-yyyy}", width: 150,
                        persistId: "changed", // DON'T YOU DARE RENAME!
                        excelTemplate: dataItem => {
                            if (!dataItem || !dataItem.LastChanged) {
                                return "";
                            }

                            return this.moment(dataItem.LastChanged).format("DD-MM-YYYY");
                        },
                        hidden: true,
                        filterable: {
                            cell: {
                                showOperators: false,
                                operator: "gte"
                            }
                        }
                    }
                ]
            };

            // find the index of column where the role columns should be inserted
            var insertIndex = this._.findIndex(mainGridOptions.columns, { 'persistId': 'orgunit' }) + 1;

            // add a role column for each of the roles
            // note iterating in reverse so we don't have to update the insert index
            this._.forEachRight(this.systemRoles, role => {
                var roleColumn = {
                    field: `role${role.Id}`,
                    title: role.Name,
                    persistId: `role${role.Id}`,
                    template: dataItem => {
                        var roles = "";

                        if (dataItem.roles[role.Id] === undefined)
                            return roles;

                        roles = this.concatRoles(dataItem.roles[role.Id]);

                        var link = `<a data-ui-sref='it-system.usage.roles({id: ${dataItem.Id}})'>${roles}</a>`;

                        return link;
                    },
                    excelTemplate: dataItem => {
                        if (!dataItem || dataItem.roles[role.Id] === undefined) {
                            return "";
                        }

                        return this.concatRoles(dataItem.roles[role.Id]);
                    },
                    width: 145,
                    hidden: !(role.Name === "Systemejer"), // hardcoded role name :(
                    sortable: false,
                    filterable: {
                        cell: {
                            dataSource: [],
                            showOperators: false,
                            operator: "contains"
                        }
                    }
                };

                // insert the generated column at the correct location
                mainGridOptions.columns.splice(insertIndex, 0, roleColumn);
            });

            // assign the generated grid options to the scope value, kendo will do the rest
            this.mainGridOptions = mainGridOptions;
        }

        private concatRoles(roles: Array<any>): string {
            var concatRoles = "";

            // join the first 5 username together
            if (roles.length > 0) {
                concatRoles = roles.slice(0, 4).join(", ");
            }

            // if more than 5 then add an elipsis
            if (roles.length > 5) {
                concatRoles += ", ...";
            }

            return concatRoles;
        }

        private getTemplateMethod(column) {
            var template: Function;

            if (column.excelTemplate) {
                template = column.excelTemplate;
            } else if (typeof column.template === "function") {
                template = <Function>column.template;
            } else if (typeof column.template === "string") {
                template = kendo.template(<string>column.template);
            } else {
                template = t => t;
            }

            return template;
        }

        private isContractActive(dataItem) {
            var today = this.moment();
            var startDate = dataItem.Concluded ? moment(dataItem.Concluded) : today;
            var endDate = dataItem.ExpirationDate ? moment(dataItem.ExpirationDate) : this.moment("9999-12-30");

            if (dataItem.Terminated) {
                var terminationDate = moment(dataItem.Terminated);
                if (dataItem.TerminationDeadline) {
                    terminationDate.add(dataItem.TerminationDeadline.Name, "months");
                }
                // indgået-dato <= dags dato <= opsagt-dato + opsigelsesfrist
                return today >= startDate && today <= terminationDate;
            }

            // indgået-dato <= dags dato <= udløbs-dato
            return today >= startDate && today <= endDate;
        }

        //// show exposureDetailsGrid - takes a itSystemUsageId for data and systemName for modal title
        //public showExposureDetails(usageId, systemName) {
        //    // filter by usageId
        //    this.exhibitGrid.dataSource.filter({ field: "ItSystemId", operator: "eq", value: usageId });
        //    // set title
        //    this.exhibitModal.setOptions({ title: systemName + " udstiller følgende snitflader" });
        //    // open modal
        //    this.exhibitModal.center().open();
        //}

        //public exhibitDetailsGrid = {
        //    dataSource: {
        //        type: "odata-v4",
        //        transport: {
        //            read: {
        //                url: "/odata/ItInterfaceExhibits?$expand=ItInterface",
        //                dataType: "json"
        //            }
        //        },
        //        serverPaging: true,
        //        serverSorting: true,
        //        serverFiltering: true
        //    },
        //    autoBind: false,
        //    columns: [
        //        {
        //            field: "ItInterface.ItInterfaceId", title: "Snitflade ID"
        //        },
        //        {
        //            field: "ItInterface.Name", title: "Snitflade"
        //        }
        //    ],
        //    dataBound: this.exposureDetailsBound
        //};

        //// exposuredetails grid empty-grid handling
        //private exposureDetailsBound(e) {
        //    var grid = e.sender;
        //    if (grid.dataSource.total() == 0) {
        //        var colCount = grid.columns.length;
        //        this.$(e.sender.wrapper)
        //            .find("tbody")
        //            .append(`<tr class="kendo-data-row"><td colspan="${colCount}" class="no-data text-muted">System udstiller ikke nogle snitflader</td></tr>`);
        //    }
        //}

        //// show usageDetailsGrid - takes a itSystemUsageId for data and systemName for modal title
        //private showUsageDetails(systemId, systemName) {
        //    // filter by systemId
        //    this.usageGrid.dataSource.filter({ field: "ItSystemId", operator: "eq", value: systemId });
        //    // set modal title
        //    this.modal.setOptions({ title: `Anvendelse af ${systemName}` });
        //    // open modal
        //    this.modal.center().open();
        //}

        //// usagedetails grid - shows which organizations has a given itsystem in local usage
        //public usageDetailsGrid = {
        //    dataSource: {
        //        type: "odata-v4",
        //        transport:
        //        {
        //            read: {
        //                url: "/odata/ItInterfaceUses/?$expand=ItInterface",
        //                dataType: "json"
        //            },
        //        },
        //        serverPaging: true,
        //        serverSorting: true,
        //        serverFiltering: true
        //    },
        //    autoBind: false,
        //    columns: [
        //        {
        //            field: "ItInterfaceId", title: "Snitflade ID"
        //        },
        //        {
        //            field: "ItInterface.Name", title: "Snitflade"
        //        }
        //    ],
        //    dataBound: this.detailsBound
        //};

        //// usagedetails grid empty-grid handling
        //private detailsBound(e) {
        //    var grid = e.sender;
        //    if (grid.dataSource.total() == 0) {
        //        var colCount = grid.columns.length;
        //        this.$(e.sender.wrapper)
        //            .find("tbody")
        //            .append(`<tr class="kendo-data-row"><td colspan="${colCount}" class="no-data text-muted">System anvendens ikke</td></tr>`);
        //    }
        //};

        private orgUnitDropDownList = (args) => {
            var self = this;

            function indent(dataItem: any) {
                var htmlSpace = "&nbsp;&nbsp;&nbsp;&nbsp;";
                return htmlSpace.repeat(dataItem.$level) + dataItem.Name;
            }

            function setDefaultOrgUnit() {
                var kendoElem = this;
                var idTofind = self.$window.sessionStorage.getItem(self.orgUnitStorageKey);

                if (!idTofind) {
                    // if no id was found then do nothing
                    return;
                }

                // find the index of the org unit that matches the users default org unit
                var index = self._.findIndex(kendoElem.dataItems(), (item: any) => (item.Id == idTofind));

                // -1 = no match
                //  0 = root org unit, which should display all. So remove org unit filter
                if (index > 0) {
                    // select the users default org unit
                    kendoElem.select(index);
                }
            }

            function orgUnitChanged() {
                var kendoElem = this;
                // can't use args.dataSource directly,
                // if we do then the view doesn't update.
                // So have to go through $scope - sadly :(
                var dataSource = self.mainGrid.dataSource;
                var selectedIndex = kendoElem.select();
                var selectedId = self._.parseInt(kendoElem.value());

                if (selectedIndex > 0) {
                    // filter by selected
                    self.$window.sessionStorage.setItem(self.orgUnitStorageKey, selectedId.toString());
                } else {
                    // else clear filter because the 0th element should act like a placeholder
                    self.$window.sessionStorage.removeItem(self.orgUnitStorageKey);
                }
                // setting the above session value will cause the grid to fetch from a different URL
                // see the function part of this http://docs.telerik.com/kendo-ui/api/javascript/data/datasource#configuration-transport.read.url
                // so that's why it works
                dataSource.read();
            }

            // http://dojo.telerik.com/ODuDe/5
            args.element.removeAttr("data-bind");
            args.element.kendoDropDownList({
                dataSource: this.orgUnits,
                dataValueField: "Id",
                dataTextField: "Name",
                template: indent,
                dataBound: setDefaultOrgUnit,
                change: orgUnitChanged
            });
        }

        private contractFilterDropDownList = (args) => {
            var self = this;
            var gridDataSource = args.dataSource;

            function setContractFilter() {
                var kendoElem = this;
                var currentFilter = gridDataSource.filter();
                var contractFilterObj = self._.findKeyDeep(currentFilter, { field: "MainContract" });

                if (contractFilterObj.operator == "neq") {
                    kendoElem.select(1); // index of "Har kontrakt"
                } else if (contractFilterObj.operator == "eq") {
                    kendoElem.select(2); // index of "Ingen kontrakt"
                }
            }

            function filterByContract() {
                var kendoElem = this;
                // can't use args.dataSource directly,
                // if we do then the view doesn't update.
                // So have to go through $scope - sadly :(
                var dataSource = self.mainGrid.dataSource;
                var selectedValue = kendoElem.value();
                var field = "MainContract";
                var currentFilter = dataSource.filter();
                // remove old value first
                var newFilter = self._.removeFiltersForField(currentFilter, field);

                if (selectedValue === "Har kontrakt") {
                    newFilter = self._.addFilter(newFilter, field, "ne", null, "and");
                } else if (selectedValue === "Ingen kontrakt") {
                    newFilter = self._.addFilter(newFilter, field, "eq", null, "and");
                }

                dataSource.filter(newFilter);
            }

            // http://dojo.telerik.com/ODuDe/5
            args.element.removeAttr("data-bind");
            args.element.kendoDropDownList({
                dataSource: ["Har kontrakt", "Ingen kontrakt"],
                optionLabel: "Vælg filter...",
                dataBound: setContractFilter,
                change: filterByContract
            });
        }

        public roleSelectorOptions = () => {
            return {
                autoBind: false,
                dataSource: this.systemRoles,
                dataTextField: "Name",
                dataValueField: "Id",
                optionLabel: "Vælg systemrolle...",
                change: e => {
                    // hide all roles column
                    this._.forEach(this.systemRoles, role => {
                        this.mainGrid.hideColumn(`role${role.Id}`);
                    });

                    var selectedId = e.sender.value();
                    var gridFieldName = `role${selectedId}`;
                    // show only the selected role column
                    this.mainGrid.showColumn(gridFieldName);
                }
            }
        };

        private exportFlag = false;
        private exportToExcel = (e: IKendoGridExcelExportEvent<IItSystemUsageOverview>) => {
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

                // hide coloumns on visual grid
                this._.forEach(columns, column => {
                    if (column.tempVisual) {
                        delete column.tempVisual;
                        e.sender.hideColumn(column);
                    }
                });

                // render templates
                var sheet = e.workbook.sheets[0];

                // skip header row
                for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
                    var row = sheet.rows[rowIndex];

                    // -1 as sheet has header and dataSource doesn't
                    var dataItem = e.data[rowIndex - 1];

                    for (var columnIndex = 0; columnIndex < row.cells.length; columnIndex++) {
                        if (columns[columnIndex].field === "") continue;
                        var cell = row.cells[columnIndex];

                        var template = this.getTemplateMethod(columns[columnIndex]);

                        cell.value = template(dataItem);
                    }
                }

                // hide loadingbar when export is finished
                kendo.ui.progress(this.mainGrid.element, false);
            }
        }
    }

    angular
        .module("app")
        .config([
            "$stateProvider", $stateProvider => {
                $stateProvider.state("it-system.overview", {
                    url: "/overview",
                    templateUrl: "app/components/it-system/it-system-overview.view.html",
                    controller: OverviewController,
                    controllerAs: "systemOverviewVm",
                    resolve: {
                        systemRoles: [
                            "$http", $http => $http.get("odata/ItSystemRoles").then(result => result.data.value)
                        ],
                        user: [
                            "userService", userService => userService.getUser()
                        ],
                        orgUnits: [
                            "$http", "user", "_",
                            ($http, user, _) => $http.get(`/odata/Organizations(${user.currentOrganizationId})/OrganizationUnits`)
                                .then(result => _.addHierarchyLevelOnFlatAndSort(result.data.value, "Id", "ParentId"))
                        ]
                    }
                });
            }
        ]);
}
