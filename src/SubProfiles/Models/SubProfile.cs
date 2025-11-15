using System;
using System.Collections.Generic;

namespace Jellyfin.Plugin.SubProfiles.Models;

public class SubProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public string? Pin { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? LastUsedAt { get; set; }

    public Dictionary<string, string> CustomData { get; set; } = new();
}
