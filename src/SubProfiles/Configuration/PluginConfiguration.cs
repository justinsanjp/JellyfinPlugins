using System;
using System.Collections.Generic;
using Jellyfin.Plugin.SubProfiles.Models;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.SubProfiles.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public Dictionary<Guid, List<SubProfile>> Profiles { get; set; } = new();
}
