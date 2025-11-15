using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jellyfin.Plugin.SubProfiles.Models;

public class SubProfileRequest
{
    public Guid? Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Language { get; set; }

    public string? SubtitleMode { get; set; }

    public Dictionary<string, string> Preferences { get; set; } = new();
}
