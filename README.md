# Jellyfin Plugins

This repository hosts custom Jellyfin plugins. Each plugin lives under `src/` with its own project file and manifest so it can be built and packaged individually.

## Sub Profiles Plugin

The first plugin included here adds Netflix-style sub profiles. It stores multiple profiles per Jellyfin user and exposes a small REST API so clients can create, update, and remove sub profiles. The project references the Jellyfin 10.11.1 server packages so it can be loaded on that server release.

### Building

1. Install the .NET SDK 8.0 or newer.
2. Restore packages and build the project:

   ```bash
   dotnet build src/SubProfiles/Jellyfin.Plugins.SubProfiles.csproj
   ```

3. Package the plugin (creates the ZIP expected by Jellyfin 10.11.1):

   ```bash
   dotnet publish src/SubProfiles/Jellyfin.Plugins.SubProfiles.csproj -c Release -o bin/publish
   ```

   The output folder will contain `manifest.json` and the compiled DLL, which can be zipped and installed in Jellyfin.

### API Overview

The plugin adds authenticated endpoints under `/SubProfiles`:

- `GET /SubProfiles/{userId}` – List sub profiles for a user.
- `POST /SubProfiles/{userId}` – Create a new sub profile.
- `PUT /SubProfiles/{userId}/{profileId}` – Update an existing sub profile.
- `DELETE /SubProfiles/{userId}/{profileId}` – Remove a sub profile.

Requests accept and return JSON representations of sub profiles, which include optional avatar URLs, PINs, and arbitrary custom data fields.
