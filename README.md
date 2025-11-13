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

3. The compiled plugin library is generated at `src/SubProfiles/bin/Release/net9.0/` alongside the `plugin.json` manifest. Use the packaging step below to create a distributable archive for Jellyfin.

### Packaging

Run the helper script to compile the plugin and place the packaged archive under `dist/`:

```bash
scripts/package-subprofiles.sh
```

The script prints the SHA256 checksum of the resulting archive so you can copy it into `manifest.json` when releasing a new
version. It requires `python3`, the .NET SDK, and the standard `zip` utility to be available on your PATH.

### Plugin Manifest & Self-Hosting

`manifest.json` enumerates every plugin bundle provided by this repository. Jellyfin servers can subscribe to it by exposing the repository over HTTP. For quick local testing you can serve the repo from the project root with:

```bash
python -m http.server 8096
```

Then add `http://<your-host>:8096/manifest.json` as a custom repository in the Jellyfin admin dashboard. The manifest references plugin archives in the `dist/` folder with relative URLs, so make sure the generated `.zip` files exist alongside the manifest before starting the server. The archives are ignored by git, allowing you to host them without committing binary files.

### API Overview

Once deployed, the plugin exposes the following authenticated REST endpoints:

- `GET /SubProfiles/{userId}` – Retrieve all sub-profiles for a user.
- `POST /SubProfiles/{userId}` – Create or update a sub-profile (pass `id` to update).
- `DELETE /SubProfiles/{userId}/{profileId}` – Remove a sub-profile.

All sub-profile data is stored within the plugin configuration, ensuring portability with the Jellyfin server configuration backup.
