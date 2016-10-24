﻿module Kitos.GlobalConfig.Directives {
    "use strict";

    function setupDirective(): ng.IDirective {
        return {
            scope: {
                editState: "@state",
                dirId: "@",
                optionType: "@"
            },
            controller: GlobalOptionListDirective,
            controllerAs: "ctrl",
            bindToController: {
                title: "@",
                optionsUrl: "@"
            },
            template: `<h2>{{ ctrl.title }}</h2><div id="{{ctrl.dirId}}" data-kendo-grid="{{ ctrl.mainGrid }}" data-k-options="{{ ctrl.mainGridOptions }}"></div>`
        };
    }

    interface IDirectiveScope {
        title: string;
        editState: string;
        optionsUrl: string;
        optionId: string;
        optionType: string;
        dirId: string;
    }

    class GlobalOptionListDirective implements IDirectiveScope {
        public optionsUrl: string;
        public title: string;
        public editState: string;
        public optionId: string;
        public dirId: string;
        public optionType: string;
        public mainGrid: IKendoGrid<Models.IOptionEntity>;
        public mainGridOptions: IKendoGridOptions<Models.IOptionEntity>;

        public static $inject: string[] = ["$http", "$timeout", "_", "$", "$state", "notify", "$scope"];

        constructor(
            private $http: ng.IHttpService,
            private $timeout: ng.ITimeoutService,
            private _: ILoDashWithMixins,
            private $: JQueryStatic,
            private $state: ng.ui.IStateService,
            private notify,
            private $scope) {

            this.$scope.$state = $state;
            this.editState = $scope.editState;
            this.dirId = $scope.dirId;
            this.optionType = $scope.optionType;

            this.mainGridOptions = {
                dataSource: {
                    type: "odata-v4",
                    transport: {
                        read: {
                            url: `${this.optionsUrl}`,
                            dataType: "json"
                        }
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
                            id: "Id",
                            fields: {
                                IsActive: { type: "boolean" },
                                IsObligatory: { type: "boolean" },
                                Name: { type: "string" },
                                Description: { type: "string" }
                            }
                        }
                    }
                } as kendo.data.DataSourceOptions,
                toolbar: [
                    {
                        //TODO ng-show='hasWriteAccess'
                        name: "opretType",
                        text: "Opret type",
                        template: "<a ng-click='ctrl.createOption()' class='btn btn-success pull-right'>#: text #</a>"
                    }
                ],
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
                columns: [
                    {
                        field: "IsActive",
                        title: "Aktiv",
                        width: 112,
                        persistId: "isActive", // DON'T YOU DARE RENAME!
                        attributes: { "class": "text-center" },
                        template: `# if(IsActive) { # <span class="glyphicon glyphicon-check text-success" aria-hidden="true"></span> # } else { # <span class="glyphicon glyphicon-unchecked" aria-hidden="true"></span> # } #`,
                        hidden: false,
                        filterable: false,
                        sortable: false
                    },
                    {
                        field: "IsObligatory",
                        title: "Obligatorisk",
                        width: 112,
                        persistId: "IsObligatory", // DON'T YOU DARE RENAME!
                        attributes: { "class": "text-center" },
                        template: `# if(IsObligatory) { # <span class="glyphicon glyphicon-check text-success" aria-hidden="true"></span> # } else { # <span class="glyphicon glyphicon-unchecked" aria-hidden="true"></span> # } #`,
                        hidden: false,
                        filterable: false,
                        sortable: false
                    },
                    //{
                    //    command: [
                    //        { text: "Op/Ned", click: this.onEdit, imageClass: "k-edit", className: "k-custom-edit", iconClass: "k-icon" } /* kendo typedef is missing imageClass and iconClass so casting to any */ as any,
                    //    ],
                    //    title: " ", width: 176,
                    //    persistId: "command"
                    //},
                    {
                        field: "Id",
                        title: "Nr.",
                        width: 230,
                        persistId: "id", // DON'T YOU DARE RENAME!
                        template: (dataItem) => dataItem.Id.toString(),
                        hidden: false,
                        filterable: false
                    },
                    {
                        field: "Name",
                        title: "Navn",
                        width: 230,
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
                        field: "Description",
                        title: "Beskrivelse",
                        width: 230,
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
                        name: "editOption",
                        text: "Redigér",
                        template: "<button type='button' class='btn btn-link' title='Redigér type' ng-click='ctrl.editOption($event)'><i class='fa fa-pencil' aria-hidden='true'></i></button>",
                        title: " ",
                        width: 176
                    } as any
                ]
            };
        }

        public createOption = () => {
            this.$scope.$state.go(this.editState, { id: 0, optionsUrl: this.optionsUrl, optionType: this.optionType });
        };

        public editOption = (e: JQueryEventObject) => {
            e.preventDefault();
            var entityGrid = this.$(`#${this.dirId}`).data("kendoGrid");
            var selectedItem = entityGrid.dataItem(this.$(e.currentTarget).closest("tr"));
            this.optionId = selectedItem.get("id");
            this.$scope.$state.go(this.editState, { id: this.optionId, optionsUrl: this.optionsUrl, optionType: this.optionType });
        };

        public deactivateOption = () => {
            //TODO OS2KITOS-258
        };
    }
    angular.module("app")
        .directive("globalOptionList", setupDirective);
}
