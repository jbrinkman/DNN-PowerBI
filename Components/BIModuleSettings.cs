using DotNetNuke.Entities.Modules.Settings;
using System;

namespace Dnn.Modules.PowerBI.Components
{
    /// <summary>
    /// The settings object for the PowerBI module
    /// </summary>
    /// <remarks>
    /// BIModuleSettings provides a strongly typed list of properties used by 
    /// the PowerBI module.  Settings will automatically be serialized and deserialized
    /// for storage in the underlying settings table.
    /// 
    /// </remarks>
    public class BIModuleSettings
    {
        [ModuleSetting(Prefix = "PBI_")]
        public string WorkspaceName { get; set; } = string.Empty;

        [ModuleSetting(Prefix = "PBI_")]
        public string WorkspaceId { get; set; } = string.Empty;

        [ModuleSetting(Prefix = "PBI_")]
        public string AccessKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// The <see cref="SettingsRepository{T}"/> used for storing and retrieving <see cref="BIModuleSettings"/>
    /// </summary>
    public class BIModuleSettingsRepository : SettingsRepository<BIModuleSettings>
    {
    }
}