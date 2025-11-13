using System;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.SubProfiles;

public class SubProfilesPlugin : BasePlugin<Configuration.SubProfilesConfiguration>, IHasPluginConfiguration
{
    public SubProfilesPlugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    public static SubProfilesPlugin Instance { get; private set; } = null!;

    public override string Name => "Sub Profiles";

    public override string Description =>
        "Adds the ability for a single Jellyfin account to manage multiple named sub-profiles with individual preferences.";

    public override Guid Id => Guid.Parse("d42b1d52-f077-4a0b-b74b-8b82495ddc68");
}
