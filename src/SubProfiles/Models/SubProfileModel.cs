using System;
using System.Collections.Generic;

namespace Jellyfin.Plugin.SubProfiles.Models;

public record SubProfileModel(Guid Id, string Name, string? Language, string? SubtitleMode, Dictionary<string, string> Preferences);
