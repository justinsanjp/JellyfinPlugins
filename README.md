# JellyfinPlugins

A repository with custom Jellyfin plugins targeting Jellyfin Server **10.11.1**.

## Sub Profiles Plugin

The `Jellyfin.Plugin.SubProfiles` project adds support for managing multiple sub-profiles under a single Jellyfin user account. Each sub-profile can store its own playback language, subtitle preferences, and arbitrary key/value settings.

### Building

1. Install the .NET 9.0 SDK (the `dotnet-install.sh` script or Microsoft packages can be used). Jellyfin 10.11.x plugins are built against .NET 9.
2. Restore dependencies and build the solution:

   ```bash
   dotnet restore
   dotnet build
   ```

3. The compiled plugin library is generated at `src/SubProfiles/bin/Debug/net9.0/` alongside the `plugin.json` manifest. Bundle these artifacts for deployment to a Jellyfin server.

### API Overview

Once deployed, the plugin exposes the following authenticated REST endpoints:

- `GET /SubProfiles/{userId}` – Retrieve all sub-profiles for a user.
- `POST /SubProfiles/{userId}` – Create or update a sub-profile (pass `id` to update).
- `DELETE /SubProfiles/{userId}/{profileId}` – Remove a sub-profile.

All sub-profile data is stored within the plugin configuration, ensuring portability with the Jellyfin server configuration backup.
