using System;
using System.Collections.Generic;
using Jellyfin.Plugin.SubProfiles.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.SubProfiles;

public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    public const string PluginName = "Sub Profiles";

    internal static object ConfigurationSyncRoot { get; } = new();

    private readonly Guid _pluginId = new("f91dc702-4fca-4e5c-a44e-45b6fe85eb9c");

    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    public static Plugin? Instance { get; private set; }

    public override string Name => PluginName;

    public override Guid Id => _pluginId;

    public override string Description => "Adds support for multiple sub profiles per Jellyfin user account.";

    public IEnumerable<PluginPageInfo> GetPages()
    {
        yield return new PluginPageInfo
        {
            Name = "subprofiles",
            EmbeddedResourcePath = GetType().Namespace + ".Web.configPage.html"
        };
    }
}
