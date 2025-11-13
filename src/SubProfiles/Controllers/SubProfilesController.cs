using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.SubProfiles.Models;
using Jellyfin.Plugin.SubProfiles.Services;
using MediaBrowser.Controller.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.SubProfiles.Controllers;

[ApiController]
[Authorize]
[Route("SubProfiles")]
public class SubProfilesController : ControllerBase
{
    private readonly ISubProfileStore _store;
    private readonly IUserManager _userManager;

    public SubProfilesController(ISubProfileStore store, IUserManager userManager)
    {
        _store = store;
        _userManager = userManager;
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<SubProfileModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<SubProfileModel>>> GetProfiles(Guid userId, CancellationToken cancellationToken)
    {
        var user = _userManager.GetUserById(userId);
        if (user is null)
        {
            return NotFound();
        }

        var profiles = await _store.GetProfilesAsync(userId, cancellationToken).ConfigureAwait(false);
        return Ok(profiles);
    }

    [HttpPost("{userId:guid}")]
    [ProducesResponseType(typeof(SubProfileModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubProfileModel>> UpsertProfile(Guid userId, [FromBody] SubProfileRequest request, CancellationToken cancellationToken)
    {
        var user = _userManager.GetUserById(userId);
        if (user is null)
        {
            return NotFound();
        }

        var profile = new SubProfileModel(request.Id ?? Guid.NewGuid(), request.Name, request.Language, request.SubtitleMode, request.Preferences);
        var result = await _store.UpsertProfileAsync(userId, profile, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpDelete("{userId:guid}/{profileId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProfile(Guid userId, Guid profileId, CancellationToken cancellationToken)
    {
        var user = _userManager.GetUserById(userId);
        if (user is null)
        {
            return NotFound();
        }

        await _store.DeleteProfileAsync(userId, profileId, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }
}
