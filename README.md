# TablePlus Command Palette

A PowerToys Command Palette extension that surfaces your TablePlus database
connections so you can launch them directly from CmdPal.

## Features

- Browse all TablePlus connections grouped by **connection group**
- Visual environment tags (Local / Staging / Production)
- Driver tag (PostgreSQL, MySQL, SQLite, …)
- Open a connection directly in TablePlus by selecting it
- Reads connections from the local TablePlus data files — no extra config

## How it works

This extension reads `Connections.plist` and `ConnectionGroups.plist` from
TablePlus' local data directory and renders each connection as a Command
Palette item that launches `tableplus://?id=<connection-id>` when invoked.

## Requirements

- **Windows 10 2004+** or **Windows 11**
- **TablePlus** installed
- Microsoft PowerToys with **Command Palette** support

## Installation

### Option 1: Microsoft Store (recommended)

[![Get from Microsoft Store](https://img.shields.io/badge/Microsoft%20Store-Get-blue?logo=microsoft-store)](https://apps.microsoft.com/detail/9P4L93228V63)

Open the listing: <https://apps.microsoft.com/detail/9P4L93228V63>

### Option 2: WinGet (Microsoft Store source)

```powershell
winget install --source msstore 9P4L93228V63
```

### Build from source

```powershell
dotnet restore
dotnet build TablePlusCommandPalette.sln -c Release -p:Platform=x64
```

See [Development](#development) below for the full build / sideload flow.

## Usage

After installation, open **PowerToys Command Palette** and look for the
**TablePlus** provider. Type to search across your connections, then press
<kbd>Enter</kbd> to open the selected connection in TablePlus.

## Development

### Build from source

```powershell
dotnet restore
dotnet build TablePlusCommandPalette.sln
```

### Publish a self-contained build

Example for x64:

```powershell
dotnet publish .\TablePlusCommandPalette\TablePlusCommandPalette.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  /p:WindowsPackageType=None
```

## Releasing

A new release is cut by triggering the `Release Extension` GitHub Actions
workflow. It builds signed x64 + ARM64 MSIX via `build-msix.ps1`, combines
them into a single `.msixbundle`, and creates a GitHub Release with the
bundle and individual MSIX files attached.

```powershell
gh workflow run release.yml --repo nickknissen/TablePlusCommandPalette `
  -f version=1.0.1 `
  -f release_notes="One-line summary of what changed in this release."
```

When the run finishes:

1. The release appears at
   `https://github.com/nickknissen/TablePlusCommandPalette/releases/tag/<version>`.
2. The `update-winget.yml` workflow fires automatically and submits a
   `wingetcreate` PR to `microsoft/winget-pkgs`.
3. To push the same artifact to the Microsoft Store: download
   `TablePlusCommandPalette_<version>.0.msixbundle` from the release page
   (or `gh release download <version> --pattern *.msixbundle`), then upload
   it in [Partner Center](https://partner.microsoft.com/dashboard/home)
   under your app's **Packages** section. The Store re-signs the package
   during ingestion regardless of the build-time signature.

### Signing prerequisites

The release workflow expects two GitHub repository secrets:

- `SIGNING_PFX_BASE64` — base64-encoded PFX containing the code-signing
  certificate. The cert subject must match the `Publisher` declared in
  `Package.appxmanifest`.
- `SIGNING_PFX_PASSWORD` — PFX password.

The shared CmdPal signing cert (used by this repo and
[TailscaleCommandPalette](https://github.com/nickknissen/TailscaleCommandPalette))
lives in 1Password under `Private/CmdPal Signing Cert`.

### Local build for testing

```powershell
.\TablePlusCommandPalette\build-msix.ps1 -Version 1.0.1 -Platforms @('x64','arm64') -Bundle
```

Produces `TablePlusCommandPalette\bin\Release\msix\TablePlusCommandPalette_1.0.1.0.msixbundle`.
With no `-CertPath`/`-CertPassword`, the package is left unsigned.

To sign locally with the cert from 1Password:

```powershell
.\scripts\sign-local.ps1 -Path .\TablePlusCommandPalette\bin\Release\msix\*.msix*
```

Pass `-Demo` to `build-msix.ps1` to compile with the `DEMO_MODE` define so
the extension surfaces hard-coded demo data instead of reading the local
TablePlus connection files (used for Microsoft Store screenshots).

## Project structure

```text
TablePlusCommandPalette/
├─ Commands/      # Command Palette invokable commands
├─ Models/        # TablePlus connection / group models
├─ Pages/         # Command Palette list pages
├─ Services/      # plist parsing and connection lookup
└─ Assets/        # App and extension icons
```

## Troubleshooting

### The extension shows "No TablePlus connections found"

- Make sure TablePlus is installed and you have at least one saved connection.
- The extension reads from
  `%LocalAppData%\com.tinyapp.TablePlus\data\Connections.plist`. If TablePlus
  stores its data elsewhere on your machine, this extension will not find it.

### Selecting a connection does nothing

The extension launches `tableplus://?id=<id>`. Confirm the URI handler works:

```powershell
start "tableplus://?id=<some-id>"
```

If TablePlus does not open, reinstall TablePlus so the URI handler is
registered.

## License

This project is licensed under the [MIT License](LICENSE).

## Disclaimer

This project is an independent extension for Microsoft PowerToys and is not
affiliated with or endorsed by TablePlus or Microsoft.
