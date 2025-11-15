using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Plugin.SubProfiles.Configuration;
using Jellyfin.Plugin.SubProfiles.Models;
using MediaBrowser.Controller.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.SubProfiles.Controllers;

[ApiController]
[Route("SubProfiles")]
[Authorize]
public class SubProfilesController : ControllerBase
{
    private static readonly StringComparer NameComparer = StringComparer.OrdinalIgnoreCase;

    private readonly IUserManager _userManager;

    public SubProfilesController(IUserManager userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("{userId}")]
    public ActionResult<IEnumerable<SubProfile>> Get(Guid userId)
    {
        if (_userManager.GetUserById(userId) == null)
        {
            return NotFound($"User {userId} was not found.");
        }

        var profiles = new List<SubProfile>();

        lock (Plugin.ConfigurationSyncRoot)
        {
            var configuration = GetConfiguration();
            if (configuration.Profiles.TryGetValue(userId, out var storedProfiles))
            {
                profiles = storedProfiles
                    .OrderBy(p => p.Name, NameComparer)
                    .Select(CloneProfile)
                    .ToList();
            }
        }

        return Ok(profiles);
    }

    [HttpPost("{userId}")]
    public ActionResult<SubProfile> Create(Guid userId, [FromBody] CreateSubProfileRequest request)
    {
        if (_userManager.GetUserById(userId) == null)
        {
            return NotFound($"User {userId} was not found.");
        }

        var trimmedName = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            return BadRequest("Name is required.");
        }

        SubProfile profile;

        lock (Plugin.ConfigurationSyncRoot)
        {
            var configuration = GetConfiguration();

            if (!configuration.Profiles.TryGetValue(userId, out var profiles))
            {
                profiles = new List<SubProfile>();
                configuration.Profiles[userId] = profiles;
            }

            if (profiles.Any(p => NameComparer.Equals(p.Name, trimmedName)))
            {
                return Conflict($"A sub profile with the name '{trimmedName}' already exists for this user.");
            }

            profile = new SubProfile
            {
                Name = trimmedName!,
                AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl,
                Pin = string.IsNullOrWhiteSpace(request.Pin) ? null : request.Pin,
                CustomData = CloneCustomData(request.CustomData)
            };

            profiles.Add(profile);
            SaveConfiguration(configuration);
        }

        return CreatedAtAction(nameof(Get), new { userId }, CloneProfile(profile));
    }

    [HttpPut("{userId}/{profileId}")]
    public ActionResult<SubProfile> Update(Guid userId, Guid profileId, [FromBody] UpdateSubProfileRequest request)
    {
        if (_userManager.GetUserById(userId) == null)
        {
            return NotFound($"User {userId} was not found.");
        }

        var trimmedName = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            return BadRequest("Name is required.");
        }

        SubProfile updatedProfile;

        lock (Plugin.ConfigurationSyncRoot)
        {
            var configuration = GetConfiguration();
            if (!configuration.Profiles.TryGetValue(userId, out var profiles))
            {
                return NotFound("User has no sub profiles.");
            }

            var profile = profiles.FirstOrDefault(p => p.Id == profileId);
            if (profile is null)
            {
                return NotFound("Sub profile not found.");
            }

            if (!NameComparer.Equals(profile.Name, trimmedName) &&
                profiles.Any(p => NameComparer.Equals(p.Name, trimmedName)))
            {
                return Conflict($"A sub profile with the name '{trimmedName}' already exists for this user.");
            }

            profile.Name = trimmedName!;
            profile.AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl;
            profile.Pin = string.IsNullOrWhiteSpace(request.Pin) ? null : request.Pin;
            if (request.CustomData is not null)
            {
                profile.CustomData = CloneCustomData(request.CustomData);
            }
            profile.LastUsedAt = DateTimeOffset.UtcNow;

            SaveConfiguration(configuration);
            updatedProfile = CloneProfile(profile);
        }

        return Ok(updatedProfile);
    }

    [HttpDelete("{userId}/{profileId}")]
    public IActionResult Delete(Guid userId, Guid profileId)
    {
        if (_userManager.GetUserById(userId) == null)
        {
            return NotFound($"User {userId} was not found.");
        }

        lock (Plugin.ConfigurationSyncRoot)
        {
            var configuration = GetConfiguration();
            if (!configuration.Profiles.TryGetValue(userId, out var profiles))
            {
                return NotFound("User has no sub profiles.");
            }

            var removed = profiles.RemoveAll(p => p.Id == profileId) > 0;
            if (!removed)
            {
                return NotFound("Sub profile not found.");
            }

            SaveConfiguration(configuration);
        }

        return NoContent();
    }

    private static PluginConfiguration GetConfiguration()
    {
        if (Plugin.Instance is null)
        {
            throw new InvalidOperationException("Plugin instance is not initialized.");
        }

        return Plugin.Instance.Configuration;
    }

    private static void SaveConfiguration(PluginConfiguration configuration)
    {
        Plugin.Instance!.UpdateConfiguration(configuration);
    }

    private static SubProfile CloneProfile(SubProfile profile)
        => new()
        {
            Id = profile.Id,
            Name = profile.Name,
            AvatarUrl = profile.AvatarUrl,
            Pin = profile.Pin,
            CreatedAt = profile.CreatedAt,
            LastUsedAt = profile.LastUsedAt,
            CustomData = CloneCustomData(profile.CustomData)
        };

    private static Dictionary<string, string> CloneCustomData(Dictionary<string, string>? source)
        => source is null
            ? new Dictionary<string, string>()
            : new Dictionary<string, string>(source, StringComparer.Ordinal);
}
