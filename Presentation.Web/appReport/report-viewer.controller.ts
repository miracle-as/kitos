﻿
module Kitos.Reports {
    "use strict";

    export class ReportViewerController {
        viewer: any;
        designer: any;
        report: Kitos.Models.IReport;
        stiReport: any;
        public canDesignReport: boolean;

        emptyreport = { "ReportVersion": "2016.1.28", "ReportGuid": "9ad83767ecc3b68cb62c086805556fce", "ReportName": "Report", "ReportAlias": "Report","ReportAuthor":"","ReportDescription":"", "ReportCreated": "/Date(1472651877000+0200)/", "ReportChanged": "/Date(1472651877000+0200)/", "EngineVersion": "EngineV2", "CalculationMode": "Interpretation", "Dictionary": { "Variables": { "0": { "Name": "CurrentOrganizationName", "DialogInfo": { "DateTimeType": "DateAndTime" }, "Alias": "CurrentOrganizationName", "Type": "System.String", "ReadOnly": true } } }, "Pages": { "0": { "Ident": "StiPage", "Name": "Page1", "Guid": "a4875a4e-cd43-da99-360b-a7560ca0b913", "Interaction": { "Ident": "StiInteraction" }, "Border": ";;2;;;;;solid:Black", "Brush": "solid:Transparent", "Components": { "0": { "Ident": "StiText", "Name": "Text1", "MinSize": "0,0", "MaxSize": "0,0", "ClientRectangle": "0.6,4.8,17.4,4.2", "Interaction": { "Ident": "StiInteraction" }, "Text": { "Value": "Rapporten er tom og venter på at blive designet." }, "HorAlignment": "Center", "Font": "Verdana;28;;", "Border": ";;;;;;;solid:Black", "Brush": "solid:Transparent", "TextBrush": "solid:Black", "TextOptions": { "WordWrap": true }, "Type": "Expression" } }, "PageWidth": 21.01, "PageHeight": 29.69, "Watermark": { "TextBrush": "solid:50,0,0,0" }, "Margins": { "Left": 1, "Right": 1, "Top": 1, "Bottom": 1 } } } }

        public static $inject = ["stimulsoftService", "reportService", "$window", "notify", "userService", "$http", "_"];
        constructor(private stimulsoftService: Kitos.Services.StimulsoftService,
            private reportService: Kitos.Services.ReportService,
            private $window: ng.IWindowService,
            private notify,
            private userService: Services.IUserService,
            private $http,
            private _) {
            let self = this;


            this.userService.getUser()
                .then((user: Services.IUser) => {
                    this.$http.get("odata/GlobalConfigs").then(result => {
                        var globalConfigs = result.data.value;

                        var canGlobalAdminOnlyEditReports = _.find(globalConfigs, function (g:any) {
                            return g.key === "CanGlobalAdminOnlyEditReports";
                        });

                        self.canDesignReport = (canGlobalAdminOnlyEditReports.value === "true") ? user.isGlobalAdmin : user.isGlobalAdmin || user.isLocalAdmin || user.isReportAdmin;
                        // 02/11 MEMA: The translation is far from done. Add back, when translated.
                        //stimulsoftService.setLocalizationFile("./appReport/locales/da-DK.xml")

                        stimulsoftService.setKey("6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkdf+M2tN29K5oDkeIUnpUPgtZSvScjXmSwTPB57ZxmLP59EQ" +
                            "EmxMCtVAUmznuiMrs/aESWbqyJI+0957evRT0kNvq6IOVFnEilK2cCVKuRZZ1rVefpvDkzmHDZPLUf6Yaxrh5iJps7" +
                            "UfuxeCuhr6yIWgrz5jFvdWvkIjnTXKKDQ6xUg5/zpOaC5Xy3AcOC0TaeOTQqGtgR/HNNSDuAjrPIrQyMCD/QCFoG3r" +
                            "S3DXcAaaZq78a8C+YCN4NHPLTHmLWVTyrgiOU4PIXvo8yQ1vLYezpLxyDkvScNFnJf4wmgwtSRnbebI5UAc/766sKY" +
                            "IfzH6WXHo9StXQKR+iuiX8OSpQ6vWH0mJ08QG0gRFLZ6Pd5vF12Mtk0T9s5U8Rt00k5ZsF2aEN8U/NCrw3U4Y7way7" +
                            "4WAB7uSrulVCyjdHB5Kdm6Nfid02/y+Qwf8QX4nRg0s83S9dCcA5EC0UKaZKHtVuVFDsoWYa1UnNhhYOWx1FW3QmMk" + 
                            "iNYu9KP5Gw3YK9Z7tLvRI/BLosV74iVA8Hws");

                        this.viewer = stimulsoftService.getViewer(this.buildViewerOptions(), "Viewer");

                        // Add the design button event
                        this.viewer.onDesignReport = function (e) {
                            this.visible = false;

                            // create designer object
                            this.designer = stimulsoftService.getDesigner(self.buildDesignerOptions(), "designer");
                            // bind events to designer object
                            this.designer.onExit = self.designerOnExit;
                            this.designer.onSaveReport = self.designerSaveReport;
                            // render designer on dom element
                            this.designer.renderHtml("reportDesigner");
                            this.designer.visible = self.canDesignReport;
                            this.designer.report = e.report;
                        };

                        this.viewer.showProcessIndicator();
                        this.viewer.renderHtml("reportViewer");
                        this.loadReport(this.getReportId(), user);
                    });
                })
        }

        getReportId = () => {
            var searchObject = this.$window.location.search;
            var regex = /([0-9]+)/g;
            var matches = searchObject.match(regex);
            let id = matches[0];
            if (matches.length === 1) {
                return parseInt(id);
            }
        };

        buildViewerOptions = () => {
            let options = this.stimulsoftService.getViewerOptions();
            options.appearance.scrollbarsMode = true;
            options.toolbar.showDesignButton = this.canDesignReport;
            options.appearance.fullScreenMode = true;
            return options;
        };

        buildDesignerOptions = () => {
            // set designer options TODO load from db
            let options = this.stimulsoftService.getDesignerOptions();
            options.appearance.fullScreenMode = true;
            options.appearance.showSaveDialog = false;
            return options;
        };

        loadReport = (id: number, user: Services.IUser) => {
            this.reportService.GetById(id)
                .then((result) => {
                    this.report = result.data;

                    this.stiReport = this.stimulsoftService.getReport();

                    var reportDef = this.emptyreport;

                    if (this.report.Definition && this.report.Definition.length > 0) {
                        //  Load reports from JSON object
                        reportDef = JSON.parse(this.report.Definition);
                    }

                    // Set standard values for report
                    reportDef.ReportName = this.report.Name;
                    reportDef.ReportAlias = this.report.Name;
                    reportDef.ReportAuthor = user.fullName;
                    reportDef.ReportDescription = this.report.Description;
                    reportDef.ReportCreated = new Date().toString();

                    this.stiReport.load(reportDef);

                    var addKitos = true;
                    var i;
                    var conString = window.location.origin + "/odata"; //+";AddressBearer=" + window.location.origin + "/api/authorize/gettoken";

                    
                    for (i = 0; i < this.stiReport.dictionary.databases.count; i++) {
                        if (this.stiReport.dictionary.databases.getByIndex(i).name === "Kitos") {
                            addKitos = false;
                            this.stiReport.dictionary.databases.getByIndex(i).connectionString = conString
                        }
                    }

                    if (addKitos) {
                        var odata = this.stimulsoftService.getODataDatabase();
                        odata.name = "Kitos";
                        odata.alias = "Kitos";
                        odata.connectionString = conString
                        this.stiReport.dictionary.databases.add(odata);
                        this.stiReport.dictionary.synchronize();
                    }

                    // Add default variables to report
                    var currentOrgName = this.stimulsoftService.getVariable();
                    currentOrgName.name = "CurrentOrganizationName";
                    currentOrgName.alias = "CurrentOrganizationName";
                    currentOrgName.value = user.currentOrganizationName;
                    currentOrgName.description = "Auto generated and injected variable";
                    currentOrgName.readOnly = true;
                    currentOrgName.requestFromUser = false;
                    currentOrgName.allowUseAsSqlParameter = false;
                    currentOrgName.typeT = typeof (String);
                    currentOrgName.category = "";
                    this.stiReport.dictionary.variables.add(currentOrgName);

                    var currentOrgId = this.stimulsoftService.getVariable();
                    currentOrgId.name = 'CurrentOrganizationId';
                    currentOrgId.alias = 'CurrentOrganizationId';
                    currentOrgId.value = user.currentOrganizationId.toString();
                    currentOrgId.description = "Auto generated and injected variable";
                    currentOrgId.readOnly = true;
                    currentOrgId.requestFromUser = false;
                    currentOrgId.allowUseAsSqlParameter = false;
                    currentOrgId.typeT = typeof (String);
                    currentOrgId.category = "";
                    this.stiReport.dictionary.variables.add(currentOrgId);

                    var currentUserName = this.stimulsoftService.getVariable();
                    currentUserName.name = 'CurrentUserName';
                    currentUserName.alias = 'CurrentUserName';
                    currentUserName.value = user.fullName;
                    currentUserName.description = "Auto generated and injected variable";
                    currentUserName.readOnly = true;
                    currentUserName.requestFromUser = false;
                    currentUserName.allowUseAsSqlParameter = false;
                    currentUserName.typeT = typeof (String);
                    currentUserName.category = "";
                    this.stiReport.dictionary.variables.add(currentUserName);

                    var currentUserId = this.stimulsoftService.getVariable();
                    currentUserId.name = 'CurrentUserId';
                    currentUserId.alias = 'CurrentUserId';
                    currentUserId.value = user.id.toString();
                    currentUserId.description = "Auto generated and injected variable";
                    currentUserId.readOnly = true;
                    currentUserId.requestFromUser = false;
                    currentUserId.allowUseAsSqlParameter = false;
                    currentUserId.typeT = typeof (String);
                    currentUserId.category = "";
                    this.stiReport.dictionary.variables.add(currentUserId);

                    //Assign the report to the viewer
                    this.viewer.report = this.stiReport;
                })
                .catch((result => {
                    console.log(result);
                    alert("Ingen adgang til rapporten!");
                }));
        }


        designerSaveReport = (saveEvent) => {
            this.report.Definition = saveEvent.report.saveToJsonString();
            this.reportService.saveReport(this.report);
            this.viewer.report = saveEvent.report;
            this.notify.addSuccessMessage("Rapporten er gemt", true);
        };

        designerOnExit = (exitEvent) => {
            this.designer.visible = false;
            this.viewer.report = exitEvent.report;
            this.viewer.visible = true;
        }
    }

    angular.module("reportApp").controller("reportViewerController", Kitos.Reports.ReportViewerController);
}
