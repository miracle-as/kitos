var paths = require("./paths.config.js");

module.exports = {
    // library script bundle
    // not minified!
    librarySrc: [
        bower("/lodash/lodash.min.js"),
        bower("/jquery/dist/jquery.min.js"),
        bower("/select2/select2.min.js"),
        bower("/select2/select2_locale_da.js"),
        bower("/moment/min/moment.min.js"),
        bower("/bootstrap/dist/js/bootstrap.min.js"),
        bower("/jsonfn-bower/jsonfn.min.js")
    ],
    libraryBundle: "library-bundle.min.js",

    libraryStylesSrc: [
        bower("/bootstrap/dist/css/bootstrap.min.css"),
        bower("/font-awesome/css/font-awesome.min.css"),
        bower("/select2/select2.css"),
        bower("/select2-bootstrap-css/select2-bootstrap.min.css"),
        bower("/angular-loading-bar/build/loading-bar.min.css")
    ],

    // angular script bundle
    // not minified
    angularSrc: [
        bower("/angular/angular.min.js"),
        bower("/angular-i18n/angular-locale_da-dk.js"),
        bower("/angular-animate/angular-animate.min.js"),
        bower("/angular-sanitize/angular-sanitize.min.js"),
        bower("/angular-ui-router/release/angular-ui-router.min.js"),
        bower("/angular-bootstrap/ui-bootstrap-tpls.min.js"),
        bower("/angular-ui-select2/src/select2.js"),
        bower("/angular-loading-bar/build/loading-bar.min.js"),
        script("/notify/*.js"),
    ],
    angularBundle: "angular-bundle.min.js",

    // app script bundle
    appSrc: paths.allJavaScriptNoTests,
    appBundle: "app-bundle.min.js",

    // font bundle
    fontSrc: [
        bower("/bootstrap/dist/fonts/*.*"),
        bower("/font-awesome/fonts/*.*")
    ],

    // assets
    assetsSrc: [
        bower("/select2/*.png"),
        bower("/select2/*.gif")
    ],

    // custom style bundle
    customCssSrc: [
        content("/notify/notify.css"),
        content("/kitos.css")
    ],
    cssBundle: "app.css",
    cssBundleMin: "app.min.css",

    fontDest: content("/fonts"),
    cssDest: content("/css"),
    maps: "maps",

    script: script,
    content: content,
    bower: bower
};

// path helper functions
function script(file) {
    return paths.sourceScript + "/" + file;
}

function content(file) {
    return paths.source + "/Content" + file;
}

function bower(file) {
    return paths.bowerComponents + "/" + file;
}