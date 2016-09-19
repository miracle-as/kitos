﻿module Kitos.ItContract.OverviewPlan {
    "use strict";

    export interface IOverviewPlanController {
        mainGrid: Kitos.IKendoGrid<IItContractPlan>;
        mainGridOptions: kendo.ui.GridOptions;
        roleSelectorOptions: kendo.ui.DropDownListOptions;

        saveGridProfile(): void;
        loadGridProfile(): void;
        clearGridProfile(): void;
        doesGridProfileExist(): void;
        clearOptions(): void;
    }

    export interface IItContractPlan extends Models.ItContract.IItContract {
        roles: Array<any>;
    }

    export class OverviewPlanController implements IOverviewPlanController {
        private storageKey = "it-contract-plan-options";
        private orgUnitStorageKey = "it-contract-plan-orgunit";
        private gridState = this.gridStateService.getService(this.storageKey);
        private roleSelectorDataSource;
        public mainGrid: Kitos.IKendoGrid<IItContractPlan>;
        public mainGridOptions: kendo.ui.GridOptions;

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
            "user",
            "gridStateService",
            "itContractRoles",
            "orgUnits"
        ];

        constructor(
            private $rootScope: Kitos.IRootScope,
            private $scope: ng.IScope,
            private $http: ng.IHttpService,
            private $timeout: ng.ITimeoutService,
            private $window: ng.IWindowService,
            private $state: ng.ui.IStateService,
            private $: JQueryStatic,
            private _: Kitos.ILoDashWithMixins,
            private moment: moment.MomentStatic,
            private notify,
            private user,
            private gridStateService: Kitos.Services.IGridStateFactory,
            private itContractRoles: Array<any>,
            private orgUnits: Array<any>) {
            this.$rootScope.page.title = "IT Kontrakt - Tid";

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

        // saves grid state to localStorage
        private saveGridOptions = () => {
            this.gridState.saveGridOptions(this.mainGrid);
        }

        // replaces "anything({roleName},'foo')" with "Rights/any(c: anything(concat(concat(c/User/Name, ' '), c/User/LastName),'foo') and c/RoleId eq {roleId})"
        private fixRoleFilter(filterUrl, roleName, roleId) {
            var pattern = new RegExp(`(\\w+\\()${roleName}(.*?\\))`, "i");
            return filterUrl.replace(pattern, `Rights/any(c: $1concat(concat(c/User/Name, ' '), c/User/LastName)$2 and c/RoleId eq ${roleId})`);
        }

        private fixProcurmentFilter(filterUrl) {
            return filterUrl.replace(/ProcurementPlanYear/i, "cast($&, Edm.String)");
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
            var orgUnitFilterRow = this.$(".k-filter-row [data-field='ResponsibleOrganizationUnit.Name']");
            // find the access modifier kendo widget
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
        }

        private reload() {
            this.$state.go(".", null, { reload: true });
        }

        private activate() {
            var clonedItContractRoles = this._.cloneDeep(this.itContractRoles);
            this._.forEach(clonedItContractRoles, n => n.Id = `role${n.Id}`);
            clonedItContractRoles.push({ Id: "ContractSigner.Name", Name: "Kontraktunderskriver" });
            this.roleSelectorDataSource = clonedItContractRoles;

            var mainGridOptions: Kitos.IKendoGridOptions<IItContractPlan> = {
                autoBind: false, // disable auto fetch, it's done in the kendoRendered event handler
                dataSource: {
                    type: "odata-v4",
                    transport: {
                        read: {
                            url: (options) => {
                                var urlParameters = `?$expand=Parent,ResponsibleOrganizationUnit,Rights($expand=User,Role),Supplier,ContractTemplate,ContractType,PurchaseForm,OptionExtend,TerminationDeadline,ProcurementStrategy,Advices,ContractSigner`;
                                // if orgunit is set then the org unit filter is active
                                var orgUnitId = this.$window.sessionStorage.getItem(this.orgUnitStorageKey);
                                if (orgUnitId === null) {
                                    return `/odata/Organizations(${this.user.currentOrganizationId})/ItContracts` + urlParameters;
                                } else {
                                    return `/odata/Organizations(${this.user.currentOrganizationId})/OrganizationUnits(${orgUnitId})/ItContracts` + urlParameters;
                                }
                            },
                            dataType: "json"
                        },
                        parameterMap: (options, type) => {
                            // get kendo to map parameters to an odata url
                            var parameterMap = kendo.data.transports["odata-v4"].parameterMap(options, type);

                            if (parameterMap.$orderby) {
                                if (parameterMap.$orderby === "ProcurementPlanYear") {
                                    parameterMap.$orderby = "ProcurementPlanYear,ProcurementPlanHalf";
                                }
                                if (parameterMap.$orderby === "ProcurementPlanYear desc") {
                                    parameterMap.$orderby = "ProcurementPlanYear desc,ProcurementPlanHalf desc";
                                }
                            }

                            if (parameterMap.$filter) {
                                this._.forEach(this.itContractRoles, role => {
                                    parameterMap.$filter = this.fixRoleFilter(parameterMap.$filter, `role${role.Id}`, role.Id);
                                });

                                parameterMap.$filter = this.fixProcurmentFilter(parameterMap.$filter);
                            }

                            return parameterMap;
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
                            fields: {
                                OperationRemunerationBegun: { type: "date" },
                                LastChanged: { type: "date" },
                                Concluded: { type: "date" },
                                ExpirationDate: { type: "date" },
                                IrrevocableTo: { type: "date" },
                                Terminated: { type: "date" },
                                Duration: { type: "number" }
                            }
                        },
                        parse: response => {
                            // iterrate each contract
                            this._.forEach(response.value, contract => {
                                // HACK to flattens the Rights on usage so they can be displayed as single columns
                                contract.roles = [];
                                // iterrate each right
                                this._.forEach(contract.Rights, right => {
                                    // init an role array to hold users assigned to this role
                                    if (!contract.roles[right.RoleId])
                                        contract.roles[right.RoleId] = [];

                                    // push username to the role array
                                    contract.roles[right.RoleId].push([right.User.Name, right.User.LastName].join(" "));
                                });
                            });
                            return response;
                        }
                    }
                },
                toolbar: [
                    { name: "excel", text: "Eksportér til Excel", className: "pull-right" },
                    {
                        name: "clearFilter",
                        text: "Nulstil",
                        template: "<button type='button' class='k-button k-button-icontext' title='Nulstil sortering, filtering og kolonnevisning, -bredde og –rækkefølge' data-ng-click='contractOverviewPlanVm.clearOptions()'>#: text #</button>"
                    },
                    {
                        name: "saveFilter",
                        text: "Gem filter",
                        template: '<button type="button" class="k-button k-button-icontext" title="Gem filtre og sortering" data-ng-click="contractOverviewPlanVm.saveGridProfile()">#: text #</button>'
                    },
                    {
                        name: "useFilter",
                        text: "Anvend filter",
                        template: '<button type="button" class="k-button k-button-icontext" title="Anvend gemte filtre og sortering" data-ng-click="contractOverviewPlanVm.loadGridProfile()" data-ng-disabled="!contractOverviewPlanVm.doesGridProfileExist()">#: text #</button>'
                    },
                    {
                        name: "deleteFilter",
                        text: "Slet filter",
                        template: "<button type='button' class='k-button k-button-icontext' title='Slet filtre og sortering' data-ng-click='contractOverviewPlanVm.clearGridProfile()' data-ng-disabled='!contractOverviewPlanVm.doesGridProfileExist()'>#: text #</button>"
                    },
                    {
                        template: kendo.template(this.$("#role-selector").html())
                    }
                ],
                excel: {
                    fileName: "IT Kontrakt Overblik.xlsx",
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
                        field: "Active", title: "Aktiv", width: 45,
                        persistId: "active", // DON'T YOU DARE RENAME!
                        template: dataItem => {
                            var isActive = this.isContractActive(dataItem);

                            if (isActive) {
                                return '<span class="fa fa-file text-success" aria-hidden="true"></span>';
                            }
                            return '<span class="fa fa-file-o text-muted" aria-hidden="true"></span>';
                        },
                        excelTemplate: dataItem => {
                            var isActive = this.isContractActive(dataItem);
                            return isActive.toString();
                        },
                        attributes: { "class": "text-center" },
                        sortable: false,
                        filterable: false,
                    },
                    {
                        field: "ItContractId", title: "KontraktID", width: 150,
                        persistId: "contractid", // DON'T YOU DARE RENAME!
                        excelTemplate: dataItem => dataItem && dataItem.ItContractId || "",
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains",
                            }
                        }
                    },
                    {
                        field: "Parent.Name", title: "Overordnet kontrakt", width: 150,
                        persistId: "parentname", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.Parent ? `<a data-ui-sref="it-contract.edit.systems({id:${dataItem.Parent.Id}})">${dataItem.Parent.Name}</a>` : "",
                        excelTemplate: dataItem => dataItem && dataItem.Parent && dataItem.Parent.Name || "",
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
                        field: "Name", title: "IT Kontrakt", width: 265,
                        persistId: "name", // DON'T YOU DARE RENAME!
                        template: dataItem => `<a data-ui-sref='it-contract.edit.systems({id: ${dataItem.Id}})'>${dataItem.Name}</a>`,
                        excelTemplate: dataItem => dataItem && dataItem.Name || "",
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "ResponsibleOrganizationUnit.Name", title: "Ansv. organisationsenhed", width: 245,
                        persistId: "orgunit", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.ResponsibleOrganizationUnit ? dataItem.ResponsibleOrganizationUnit.Name : "",
                        hidden: true,
                        filterable: {
                            cell: {
                                showOperators: false,
                                template: this.orgUnitDropDownList
                            }
                        }
                    },
                    {
                        field: "Supplier.Name", title: "Leverandør", width: 200,
                        persistId: "suppliername", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.Supplier ? dataItem.Supplier.Name : "",
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "Esdh", title: "ESDH ref", width: 150,
                        persistId: "esdh", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.Esdh ? `<a target="_blank" href="${dataItem.Esdh}"><i class="fa fa-link"></a>` : "",
                        excelTemplate: dataItem => dataItem && dataItem.Esdh || "",
                        attributes: { "class": "text-center" },
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains",
                            }
                        }
                    },
                    {
                        field: "Folder", title: "Mappe ref", width: 150,
                        persistId: "folderref", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.Folder ? `<a target="_blank" href="${dataItem.Folder}"><i class="fa fa-link"></i></a>` : "",
                        excelTemplate: dataItem => dataItem && dataItem.Folder || "",
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
                        field: "ContractType.Name", title: "Kontrakttype", width: 120,
                        persistId: "contracttype", // DON'T YOU DARE RENAME!
                        template: "#: ContractType ? ContractType.Name : '' #",
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "ContractTemplate.Name", title: "Kontraktskabelon", width: 145,
                        persistId: "contracttmpl", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.ContractTemplate ? dataItem.ContractTemplate.Name : "",
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "PurchaseForm.Name", title: "Indkøbsform", width: 115,
                        persistId: "purchaseform", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.PurchaseForm ? dataItem.PurchaseForm.Name : "",
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "Concluded", title: "Indgået", format: "{0:dd-MM-yyyy}", width: 90,
                        persistId: "concluded", // DON'T YOU DARE RENAME!
                        excelTemplate: dataItem => {
                            if (!dataItem.Concluded) {
                                return "";
                            }

                            return this.moment(dataItem.Concluded).format("DD-MM-YYYY");
                        },
                        filterable: {
                            cell: {
                                showOperators: false,
                                operator: "gte"
                            }
                        }
                    },
                    {
                        field: "Duration", title: "Varighed", width: 115,
                        persistId: "duration", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.Duration ? `${dataItem.Duration} md` : "",
                        hidden: true,
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "eq"
                            }
                        }
                    },
                    {
                        field: "ExpirationDate", title: "Udløbs dato", format: "{0:dd-MM-yyyy}", width: 90,
                        persistId: "expirationDate", // DON'T YOU DARE RENAME!
                        excelTemplate: dataItem => {
                            if (!dataItem.ExpirationDate) {
                                return "";
                            }

                            return this.moment(dataItem.ExpirationDate).format("DD-MM-YYYY");
                        },
                        filterable: {
                            cell: {
                                showOperators: false,
                                operator: "gte"
                            }
                        }
                    },
                    {
                        field: "OptionExtend", title: "Option", width: 150,
                        persistId: "option", // DON'T YOU DARE RENAME!
                        hidden: true,
                        template: dataItem => {
                            var value = "";

                            if (dataItem.OptionExtend) {
                                value += dataItem.OptionExtend.Name;
                            }

                            if (dataItem.OptionExtend) {
                                value += ` (${dataItem.ExtendMultiplier})`;
                            }

                            return value;
                        },
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "TerminationDeadline.Name", title: "Opsigelse", width: 100,
                        persistId: "terminationDeadline", // DON'T YOU DARE RENAME!
                        template: dataItem => dataItem.TerminationDeadline ? `${dataItem.TerminationDeadline.Name} md` : "",
                        hidden: true,
                        filterable: {
                            cell: {
                                showOperators: false,
                                operator: "gte"
                            }
                        }
                    },
                    {
                        field: "IrrevocableTo", title: "Uopsigelig til", format: "{0:dd-MM-yyyy}", width: 150,
                        headerTemplate: '<div style="word-wrap: break-word;">Uopsigelig til</div>',
                        persistId: "irrevocableTo", // DON'T YOU DARE RENAME!
                        excelTemplate: dataItem => {
                            if (!dataItem.IrrevocableTo) {
                                return "";
                            }

                            return this.moment(dataItem.IrrevocableTo).format("DD-MM-YYYY");
                        },
                        hidden: true,
                        filterable: {
                            cell: {
                                showOperators: false,
                                operator: "gte"
                            }
                        }
                    },
                    {
                        field: "Terminated", title: "Opsagt", format: "{0:dd-MM-yyyy}", width: 150,
                        persistId: "terminated", // DON'T YOU DARE RENAME!
                        excelTemplate: dataItem => {
                            if (!dataItem.Terminated) {
                                return "";
                            }

                            return this.moment(dataItem.Terminated).format("DD-MM-YYYY");
                        },
                        hidden: true,
                        filterable: {
                            cell: {
                                showOperators: false,
                                operator: "gte"
                            }
                        }
                    },
                    {
                        field: "ProcurementStrategy", title: "Udbudsstrategi", width: 150,
                        persistId: "procurementStrategy", // DON'T YOU DARE RENAME!
                        hidden: true,
                        template: dataItem => dataItem.ProcurementStrategy ? dataItem.ProcurementStrategy.Name : "",
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "ProcurementPlanYear", title: "Udbuds plan", width: 90,
                        persistId: "procurementPlan", // DON'T YOU DARE RENAME!
                        attributes: { "class": "text-center" },
                        template: dataItem =>
                            dataItem.ProcurementPlanHalf && dataItem.ProcurementPlanYear ? `${dataItem.ProcurementPlanYear} | ${dataItem.ProcurementPlanHalf}` : "",
                        filterable: {
                            cell: {
                                dataSource: [],
                                showOperators: false,
                                operator: "contains"
                            }
                        }
                    },
                    {
                        field: "Advices.AlarmDate", title: "Dato for næste advis", width: 90,
                        persistId: "nextadvis", // DON'T YOU DARE RENAME!
                        template: dataItem => {
                            if (dataItem.Advices.length <= 0) {
                                return "";
                            }
                            var date = this._.first(this._.sortBy(dataItem.Advices, ["AlarmDate"])).AlarmDate;
                            return this.moment(date).format("DD-MM-YYYY");
                        },
                        sortable: false,
                        filterable: false
                    }
                ]
            };

            // find the index of column where the role columns should be inserted
            var insertIndex = this._.findIndex(mainGridOptions.columns, { 'persistId': 'orgunit' }) + 1;

            // add special contract signer role
            var signerRole = {
                field: "ContractSigner.Name",
                title: "Kontraktunderskriver",
                persistId: "roleSigner",
                template: dataItem => dataItem.ContractSigner ? `${dataItem.ContractSigner.Name} ${dataItem.ContractSigner.LastName}` : "",
                width: 130,
                hidden: true,
                sortable: true,
                filterable: {
                    cell: {
                        dataSource: [],
                        showOperators: false,
                        operator: "contains"
                    }
                }
            };
            mainGridOptions.columns.splice(insertIndex, 0, signerRole);

            // add a role column for each of the roles
            // note iterating in reverse so we don't have to update the insert index
            this._.forEachRight(this.itContractRoles, role => {
                var roleColumn: IKendoGridColumn<IItContractPlan> = {
                    field: `role${role.Id}`,
                    title: role.Name,
                    persistId: `role${role.Id}`,
                    template: dataItem => {
                        var roles = "";

                        if (dataItem.roles[role.Id] === undefined)
                            return roles;

                        roles = this.concatRoles(dataItem.roles[role.Id]);

                        var link = `<a data-ui-sref='it-contract.edit.roles({id: ${dataItem.Id}})'>${roles}</a>`;

                        return link;
                    },
                    excelTemplate: dataItem => {
                        var roles = "";

                        if (dataItem.roles[role.Id] === undefined)
                            return roles;

                        return this.concatRoles(dataItem.roles[role.Id]);
                    },
                    width: 130,
                    hidden: true,
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

        private exportFlag = false;
        private exportToExcel = (e: IKendoGridExcelExportEvent<IItContractPlan>) => {
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

        private isContractActive(dataItem) {
            var today = this.moment();
            var startDate = dataItem.Concluded ? this.moment(dataItem.Concluded) : today;
            var endDate = dataItem.ExpirationDate ? this.moment(dataItem.ExpirationDate) : this.moment("9999-12-30");

            if (dataItem.Terminated) {
                var terminationDate = this.moment(dataItem.Terminated);
                if (dataItem.TerminationDeadline) {
                    terminationDate.add(dataItem.TerminationDeadline.Name, "months");
                }
                // indgået-dato <= dags dato <= opsagt-dato + opsigelsesfrist
                return today >= startDate && today <= terminationDate;
            }

            // indgået-dato <= dags dato <= udløbs-dato
            return today >= startDate && today <= endDate;
        }

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
                var index = self._.findIndex(kendoElem.dataItems(), (item: any) => item.Id == idTofind);

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

       public roleSelectorOptions = (): kendo.ui.DropDownListOptions => {
            return {
                autoBind: false,
                dataSource: this.roleSelectorDataSource,
                dataTextField: "Name",
                dataValueField: "Id",
                optionLabel: "Vælg kontraktrolle...",
                change: e => {
                    // hide all roles column
                    this.mainGrid.hideColumn("ContractSigner.Name");
                    this._.forEach(this.itContractRoles, role => this.mainGrid.hideColumn(`role${role.Id}`));

                    var selectedId = e.sender.value();

                    // show only the selected role column
                    this.mainGrid.showColumn(selectedId);
                }
            }
        }
    }

    angular
        .module("app")
        .config(["$stateProvider", $stateProvider => {
            $stateProvider.state("it-contract.plan", {
                url: "/plan",
                templateUrl: "app/components/it-contract/it-contract-plan.view.html",
                controller: OverviewPlanController,
                controllerAs: "contractOverviewPlanVm",
                resolve: {
                    user: ["userService", userService => userService.getUser()],
                    itContractRoles: [
                        "$http", $http => $http.get("/odata/ItContractRoles").then(result => result.data.value)
                    ],
                    orgUnits: [
                        "$http", "user", "_", ($http, user, _) => $http.get(`/odata/Organizations(${user.currentOrganizationId})/OrganizationUnits`).then(result => _.addHierarchyLevelOnFlatAndSort(result.data.value, "Id", "ParentId"))
                    ]
                }
            });
        }]);
}
