﻿module Kitos.GlobalConfig.Directives {
    "use strict";

    function setupDirective(): ng.IDirective {

     
        return {
            scope: {},
            controller: GlobalOptionRoleListDirective,
            controllerAs: "ctrl",
            bindToController: {
                optionsUrl: "@",
                title: "@"
            },
            template: `<h2>{{ ctrl.title }}</h2><div id="mainGrid" data-kendo-grid="{{ ctrl.mainGrid }}" data-k-options="{{ ctrl.mainGridOptions }}"></div>`
        };

    }



    interface IDirectiveScope {
        optionsUrl: string;
        title: string;
    }

    class GlobalOptionRoleListDirective implements IDirectiveScope {
        public optionsUrl: string;
        public title: string;

        public mainGrid: IKendoGrid<Models.IRoleEntity>;
        public mainGridOptions: IKendoGridOptions<Models.IRoleEntity>;

        public static $inject: string[] = ["$http", "$timeout", "_", "$", "$state", "notify"];

        constructor(
            private $http: ng.IHttpService,
            private $timeout: ng.ITimeoutService,
            private _: ILoDashWithMixins,
            private $: JQueryStatic,
            private $state: ng.ui.IStateService,
            private notify) {


            function changePriority() {
                alert("click");
             
                //e.preventDefault();
                //var dataItem = this.mainGrid.dataItem(this.$(e.currentTarget).closest("tr"));
                //var entityId = dataItem["Id"];
                //this.$state.go("organization.user.edit", { id: entityId });
            }
            
            
            this.mainGridOptions = {
                dataSource: {
                    type: "odata-v4",
                    transport: {
                        read: {
                            url: `${this.optionsUrl}?$filter=IsActive eq true`,
                            dataType: "json"
                        },
                        //,destroy: {
                        //    url: (entity) => {
                        //        return `/odata/Organizations(${this.user.currentOrganizationId})/RemoveUser()`;
                        //    },
                        //    dataType: "json",
                        //    contentType: "application/json"
                        //},
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
                            id: "Id"
                        }
                    }
                } as kendo.data.DataSourceOptions,
                toolbar: [
                    {
                        //TODO ng-show='hasWriteAccess'
                        name: "opretRolle",
                        text: "Opret rolle",
                        template: "<a ng-click='ctrl.opretRolle()' class='btn btn-success pull-right'>#: text #</a>"
                    }
                ],
                pageable: {
                    refresh: true,
                    pageSizes: [10, 25, 50, 100, 200],
                    buttonCount: 5
                },
                sortable: {
                    mode: "single",
                    
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
                columns: [
                    {
                        field: "IsActive", title: "Aktiv", width: 112,
                        persistId: "isActive", // DON'T YOU DARE RENAME!
                        attributes: { "class": "text-center" },
                        template: `# if(IsActive) { # <span class="glyphicon glyphicon-check text-success" aria-hidden="true"></span> # } else { # <span class="glyphicon glyphicon-unchecked" aria-hidden="true"></span> # } #`,
                        hidden: false,
                        filterable: false,
                        sortable: false
                    },
                    {
                        /*command: [
                            { text: "Op/Ned", click: "google.com", imageClass: "k-edit", className: "k-custom-edit", iconClass: "k-icon" } /* kendo typedef is missing imageClass and iconClass so casting to any  as any,*/
                        //],
                        template: `<button class='btn btn-link' data-ng-click='ctrl.pushUp($event)'"><i class='fa fa-plus' aria-hidden='true'></i></button>`,
                        title: " ", width: 176,
                        persistId: "command"
                    },
                    {
                        field: "Id", title: "Nr.", width: 230,
                        persistId: "id", // DON'T YOU DARE RENAME!
                        template: (dataItem) => dataItem.Id.toString(),
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
                        field: "Name", title: "Navn", width: 230,
                        persistId: "name", // DON'T YOU DARE RENAME!
                        template: (dataItem) => dataItem.Name,
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
                        field: "HasWriteAccess", title: "Skriv", width: 112,
                        persistId: "hasWriteAccess", // DON'T YOU DARE RENAME!
                        attributes: { "class": "text-center" },
                        template: `# if(HasWriteAccess) { # <span class="glyphicon glyphicon-check text-success" aria-hidden="true"></span> # } else { # <span class="glyphicon glyphicon-unchecked" aria-hidden="true"></span> # } #`,
                        hidden: false,
                        filterable: false,
                        sortable: false
                    },
                    {
                        field: "Description", title: "Beskrivelse", width: 230,
                        persistId: "description", // DON'T YOU DARE RENAME!
                        template: (dataItem) => dataItem.Description,
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
                           ],
                        title: " ", width: 176,
                        persistId: "command"
                    },
                ]
            };
        }

        private onEdit = (e: JQueryEventObject) => {
            e.preventDefault();
            alert("Hey ;-) ");
           // var dataItem = this.mainGrid.dataItem(this.$(e.currentTarget).closest("tr"));
           // var entityId = dataItem["Id"];
           // this.$state.go("organization.user.edit", { id: entityId });
        }
        private pushUp = (e: JQueryEventObject) => {
            e.preventDefault();
            alert("OOOP ;-) ");

            var dataItem = this.mainGrid.dataItem(this.$(e.currentTarget).closest("tr"));
            var entityId = dataItem["Id"];
            alert(entityId);

           /* let payload = {
                Priority: this.
            }
            this.$http.patch(`/odata/reports(${this.reportId})`, payload).then((response) => {
            });*/
        }
        private pushDown = (e: JQueryEventObject) => {
            e.preventDefault();
            alert("NEEEEED ;-) ");
            
        }
    }
    /*
    function up(): void {
        alert("Hey ;-) ");
    }*/
    angular.module("app")
        .directive("globalOptionRoleList", setupDirective);

    
}