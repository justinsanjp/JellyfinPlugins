using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.SubProfiles.Configuration;
using Jellyfin.Plugin.SubProfiles.Models;

namespace Jellyfin.Plugin.SubProfiles.Services;

public class ConfigurationSubProfileStore : ISubProfileStore
{
    public Task<IReadOnlyList<SubProfileModel>> GetProfilesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var configuration = SubProfilesPlugin.Instance.Configuration;
        var key = userId.ToString("N");

        if (!configuration.Profiles.TryGetValue(key, out var profiles))
        {
            return Task.FromResult<IReadOnlyList<SubProfileModel>>(Array.Empty<SubProfileModel>());
        }

        var result = profiles
            .Select(p => new SubProfileModel(p.Id, p.Name, p.Language, p.SubtitleMode, new Dictionary<string, string>(p.Preferences)))
            .ToList();

        return Task.FromResult<IReadOnlyList<SubProfileModel>>(result);
    }

    public Task<SubProfileModel> UpsertProfileAsync(Guid userId, SubProfileModel profile, CancellationToken cancellationToken)
    {
        var configuration = SubProfilesPlugin.Instance.Configuration;
        var key = userId.ToString("N");

        if (!configuration.Profiles.TryGetValue(key, out var profiles))
        {
            profiles = new List<SubProfileDefinition>();
            configuration.Profiles[key] = profiles;
        }

        var existing = profiles.FirstOrDefault(p => p.Id == profile.Id);
        if (existing is null)
        {
            existing = new SubProfileDefinition
            {
                Id = profile.Id == Guid.Empty ? Guid.NewGuid() : profile.Id,
            };
            profiles.Add(existing);
        }

        existing.Name = profile.Name;
        existing.Language = profile.Language;
        existing.SubtitleMode = profile.SubtitleMode;
        existing.Preferences = new Dictionary<string, string>(profile.Preferences);

        SubProfilesPlugin.Instance.UpdateConfiguration(configuration);

        var saved = new SubProfileModel(existing.Id, existing.Name, existing.Language, existing.SubtitleMode, existing.Preferences);
        return Task.FromResult(saved);
    }

    public Task DeleteProfileAsync(Guid userId, Guid profileId, CancellationToken cancellationToken)
    {
        var configuration = SubProfilesPlugin.Instance.Configuration;
        var key = userId.ToString("N");

        if (configuration.Profiles.TryGetValue(key, out var profiles))
        {
            profiles.RemoveAll(p => p.Id == profileId);
            if (profiles.Count == 0)
            {
                configuration.Profiles.Remove(key);
            }

            SubProfilesPlugin.Instance.UpdateConfiguration(configuration);
        }

        return Task.CompletedTask;
    }
}
