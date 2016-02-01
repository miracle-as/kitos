﻿import mock = require("protractor-http-mock");
import Helper = require("../../../helper");
import PageObject = require("../../../../app/components/it-system/usage/tabs/it-system-usage-tab-roles.po");

describe("system usage tab roles", () => {
    var mockHelper: Helper.Mock;
    var browserHelper: Helper.Browser;
    var pageObject: PageObject;
    var mockDependencies: Array<string> = [
        "itSystemUsage",
        "businesstype",
        "archivetype",
        "sensitivedatatype",
        "itSystemRole",
        "organization",
        "itSystemUsageRights"
    ];

    beforeEach(() => {
        browser.driver.manage().window().maximize();

        mockHelper = new Helper.Mock();
        browserHelper = new Helper.Browser(browser);
        pageObject = new PageObject();
    });

    afterEach(() => {
        mock.teardown();
    });

    describe("with no write access", () => {
        beforeEach(done => {
            mock(["itSystemUsageNoWriteAccess"].concat(mockDependencies));
            pageObject.getPage();

            // clear initial requests
            mock.clearRequests()
                .then(() => done());
        });

        it("should hide new right fields", () => {
            // arrange

            // act

            // assert
            expect(pageObject.addRightRoleSelector.element).not.toBeVisible();
            expect(pageObject.addRightUserSelector.element).not.toBeVisible();
        });

        it("should hide edit and delete buttons on rights", () => {
            // arrange
            var editButton = element.all(pageObject.rightEditButtonLocator)
                .first();
            var deleteButton = element.all(pageObject.rightEditDeleteLocator)
                .first();

            // act

            // assert
            expect(editButton).not.toBeVisible();
            expect(deleteButton).not.toBeVisible();
        });
    });

    describe("with write access", () => {
        beforeEach(done => {
            mock(["itSystemUsageWriteAccess"].concat(mockDependencies));
            pageObject.getPage();

            // clear initial requests
            mock.clearRequests()
                .then(() => done());
        });

        it("should init roles selector", () => {
            // arrange
            var element = pageObject.addRightRoleSelector;

            // act
            element.element.click();

            // assert
            expect(element.options.count()).toBeGreaterThan(0, "No options in selector");
        });

        it("should get users when typing", () => {
            // arrange
            var element = pageObject.addRightUserSelector;

            // act
            element.selectFirst("t");

            // assert
            expect(mock.requestsMade()).toMatchInRequests({ method: "GET", url: "api/organization" });
        });

        it("should save right when user is selected", () => {
            // arrange
            var element = pageObject.addRightUserSelector;

            // act
            element.selectFirst("t");

            // assert
            expect(mock.requestsMade()).toMatchInRequests({ method: "POST", url: "api/itSystemUsageRights/" });
        });

        it("should repeat rights", () => {
            // arrange

            // act

            // assert
            // two rows are created per right, one for display one for edit
            var count = pageObject.rightsRepeater.selectFirst(pageObject.rightRowLocator).count();
            expect(count).toBe(2);
        });

        it("should display edit field for rights when edit is clicked", () => {
            // arrange

            // act
            pageObject.rightsRepeater
                .selectFirst(pageObject.rightEditButtonLocator)
                .click();

            // assert
            var count = pageObject.rightsRepeater.selectFirst(pageObject.rightEditRoleInputLocator).count();
            expect(count).toBe(1);
        });

        it("should save edited field for rights when save is clicked", () => {
            // arrange
            pageObject.rightsRepeater
                .selectFirst(pageObject.rightEditButtonLocator)
                .click();
            mock.clearRequests();

            // act
            pageObject.rightsRepeater
                .selectFirst(pageObject.rightEditSaveButtonLocator)
                .click();

            // assert
            expect(mock.requestsMade()).toMatchInRequests({ method: "DELETE", url: "api/itSystemUsageRights" });
            expect(mock.requestsMade()).toMatchInRequests({ method: "POST", url: "api/itSystemUsageRights" });
        });

        it("should delete right when delete confirmed", () => {
            // arrange
            mock.clearRequests();
            pageObject.rightsRepeater
                .selectFirst(pageObject.rightEditDeleteLocator)
                .click();

            // act
            browserHelper.acceptAlert();

            // assert
            expect(mock.requestsMade()).toMatchInRequests({ method: "DELETE", url: "api/itSystemUsageRights" });
        });

        it("should not delete right when delete dismissed", () => {
            // arrange
            pageObject.rightsRepeater
                .selectFirst(pageObject.rightEditDeleteLocator)
                .click();

            // act
            browserHelper.dismissAlert();

            // assert
            expect(mock.requestsMade()).not.toMatchInRequests({ method: "DELETE", url: "api/itSystemUsageRights" });
        });
    });
});
