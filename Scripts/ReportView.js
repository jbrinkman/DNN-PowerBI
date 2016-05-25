var PowerBI = PowerBI || {};

PowerBI.reportListViewModel = function (moduleId, resx) {
    var service = {
        path: "PowerBI",
        framework: $.ServicesFramework(moduleId)
    }
    service.baseUrl = service.framework.getServiceRoot(service.path) + "Report/";

    var isLoading = ko.observable(false);
    var reportList = ko.observableArray([]);
    var selectedReport = ko.observable('');

    var init = function () {
        getReportList();
    }

    var getReportList = function () {
        isLoading(true);
        var jqXHR = $.ajax({
            url: service.baseUrl,
            beforeSend: service.framework.setModuleHeaders,
            dataType: "json"
        }).done(function (data) {
            if (data) {
                load(data);
            }
            else {
                // No data to load 
                reportList.removeAll();
            }
        }).always(function (data) {
            isLoading(false);
        });
    };

    var load = function (data) {
        reportList.removeAll();
        var underlyingArray = reportList();
        for (var i = 0; i < data.length; i++) {
            var result = data[i];
            var item = new PowerBI.reportViewModel();
            item.load(result);
            underlyingArray.push(item);
        }
        reportList.valueHasMutated();
    };

    var selectReport = function (report, evt) {
        evt.preventDefault();

        selectedReport(report.id);

        // To load a report do the following:
        // 1: Set the url
        // 2: Add a onload handler to submit the auth token
        var iframe = $('#iFrameEmbedReport');
        iframe.load(postActionLoadReport);
        iframe.data("token", report.token());
        iframe.attr("src", report.embedUrl());
    }

    // Post the auth token to the iFrame. 
    var postActionLoadReport = function () {

        var $iframe = $('#iFrameEmbedReport');

        // Get the app token.
        accessToken = $iframe.data("token");

        // Construct the push message structure
        var m = { action: "loadReport", accessToken: accessToken };
        message = JSON.stringify(m);

        // Push the message.
        document.getElementById('iFrameEmbedReport').contentWindow.postMessage(message, "*");
    }


    return {
        init: init,
        reportList: reportList,
        selectReport: selectReport,
        isLoading: isLoading
    }
};

PowerBI.reportViewModel = function () {
    var id = ko.observable('');
    var name = ko.observable('');
    var token = ko.observable('');
    var embedUrl = ko.observable('');

    var load = function (data) {
        id(data.id)
        name(data.name);
        token(data.token);
        embedUrl(data.embedUrl);
    };

    return {
        load: load,
        id: id,
        name: name,
        token: token,
        embedUrl: embedUrl
    }
}
