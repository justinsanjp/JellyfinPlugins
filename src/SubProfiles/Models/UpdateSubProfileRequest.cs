using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jellyfin.Plugin.SubProfiles.Models;

public class UpdateSubProfileRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public string? Pin { get; set; }

    public Dictionary<string, string>? CustomData { get; set; }
}
