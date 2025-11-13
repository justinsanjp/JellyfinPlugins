using System;
using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.SubProfiles.Configuration;

public class SubProfilesConfiguration : BasePluginConfiguration
{
    public Dictionary<string, List<SubProfileDefinition>> Profiles { get; set; } = new();
}

public class SubProfileDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string? Language { get; set; }

    public string? SubtitleMode { get; set; }

    public Dictionary<string, string> Preferences { get; set; } = new();
}
