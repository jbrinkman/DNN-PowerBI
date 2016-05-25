var PowerBI = PowerBI || {};

PowerBI.quickSettings = function(root, moduleId) {

    var workspaceId = ko.observable('');
    var workspaceName = ko.observable('');
    var accessKey = ko.observable('');

    var isLoading = ko.observable(false);

    // Setup your settings service endpoint
    var service = {
        path: "PowerBI",
        framework: $.ServicesFramework(moduleId)
    }
    service.baseUrl = service.framework.getServiceRoot(service.path) + "Settings/";

    var SaveSettings = function () {
        var settings = {
            WorkspaceId: workspaceId(),
            WorkspaceName: workspaceName(),
            AccessKey: accessKey()
        };

        isLoading(true);
        var jqXHR = $.ajax({
            type: "POST",
            url: service.baseUrl,
            beforeSend: service.framework.setModuleHeaders,
            dataType: "json",
            data: settings
        }).done(function (data) {
            if (data) {
                load(data);
            }
        }).always(function (data) {
            isLoading(false);
        });

        return jqXHR;
    };

    var CancelSettings = function () {
        var deferred = $.Deferred();
        deferred.resolve();
        return deferred.promise();
    };

    var LoadSettings = function () {
        isLoading(true);
        var jqXHR = $.ajax({
            url: service.baseUrl,
            beforeSend: service.framework.setModuleHeaders,
            dataType: "json"
        }).done(function (data) {
            if (data) {
                load(data);
            }
         }).always(function (data) {
            isLoading(false);
        });

    };

    var load = function (data) {
        workspaceId(data.WorkspaceId)
        workspaceName(data.WorkspaceName);
        accessKey(data.AccessKey);
    };

    var init = function () {
        // Wire up the default save and cancel buttons
        $(root).dnnQuickSettings({
            moduleId: moduleId,
            onSave: SaveSettings,
            onCancel: CancelSettings
        });
        LoadSettings();
    }

    return {
        init: init,
        WorkspaceId: workspaceId,
        WorkspaceName: workspaceName,
        AccessKey: accessKey
    }
};
