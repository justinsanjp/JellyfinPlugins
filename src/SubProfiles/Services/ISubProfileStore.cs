using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.SubProfiles.Models;

namespace Jellyfin.Plugin.SubProfiles.Services;

public interface ISubProfileStore
{
    Task<IReadOnlyList<SubProfileModel>> GetProfilesAsync(Guid userId, CancellationToken cancellationToken);

    Task<SubProfileModel> UpsertProfileAsync(Guid userId, SubProfileModel profile, CancellationToken cancellationToken);

    Task DeleteProfileAsync(Guid userId, Guid profileId, CancellationToken cancellationToken);
}
