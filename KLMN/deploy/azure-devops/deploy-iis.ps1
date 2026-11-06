[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][ValidateSet('Api', 'Web')][string]$Kind,
    [Parameter(Mandatory = $true)][string]$PackagePath,
    [Parameter(Mandatory = $true)][string]$SiteName,
    [Parameter(Mandatory = $true)][string]$AppPoolName,
    [Parameter(Mandatory = $true)][string]$TargetPath
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
if (-not [System.Environment]::OSVersion.Platform.ToString().StartsWith('Win')) {
    throw 'This release script requires a Windows IIS deployment agent.'
}
if (-not (Test-Path -LiteralPath $PackagePath -PathType Leaf)) {
    throw "Build artifact not found: $PackagePath"
}
Import-Module WebAdministration -ErrorAction Stop
$site = Get-Item -Path ("IIS:\Sites\" + $SiteName) -ErrorAction Stop
if (-not (Test-Path -Path ("IIS:\AppPools\" + $AppPoolName))) {
    throw "IIS application pool not found: $AppPoolName"
}
$destination = [System.IO.Path]::GetFullPath([System.Environment]::ExpandEnvironmentVariables($TargetPath)).TrimEnd('\')
$actual = [System.IO.Path]::GetFullPath(
    [System.Environment]::ExpandEnvironmentVariables([string]$site.physicalPath)).TrimEnd('\')
if (-not [string]::Equals($destination, $actual, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Refusing to deploy: IIS site physical path does not equal the configured target path."
}
if ($destination -eq [System.IO.Path]::GetPathRoot($destination).TrimEnd('\')) {
    throw 'Refusing to deploy into a drive root.'
}
if ($Kind -eq 'Api' -and -not (Test-Path -LiteralPath (Join-Path $destination 'appsettings.json') -PathType Leaf)) {
    throw 'First provision a server-specific appsettings.json OUTSIDE source control at the API target path.'
}

$parent = Split-Path -Path $destination -Parent
$folder = Split-Path -Path $destination -Leaf
$staging = Join-Path $parent ($folder + '.incoming.' + [guid]::NewGuid().ToString('N'))
$backup = Join-Path $parent ($folder + '.backup.' + (Get-Date -Format 'yyyyMMddHHmmss'))
$oldMoved = $false
$newMoved = $false
$websiteStopped = $false
$poolStopped = $false

try {
    New-Item -ItemType Directory -Path $staging -Force | Out-Null
    Expand-Archive -LiteralPath $PackagePath -DestinationPath $staging -Force
    if ($Kind -eq 'Api') {
        if (-not (Test-Path -LiteralPath (Join-Path $staging 'KLMN.Api.dll'))) {
            throw 'Invalid API artifact: KLMN.Api.dll missing.'
        }
        # The build artifact must NEVER overwrite deployment secrets.
        if (Get-ChildItem -Path $staging -File -Filter 'appsettings*.json') {
            throw 'API artifact unexpectedly contains an appsettings JSON file.'
        }
        Copy-Item -LiteralPath (Join-Path $destination 'appsettings.json') -Destination (Join-Path $staging 'appsettings.json') -Force
        $extra = Join-Path $destination 'appsettings.Production.json'
        if (Test-Path -LiteralPath $extra -PathType Leaf) {
            Copy-Item -LiteralPath $extra -Destination (Join-Path $staging 'appsettings.Production.json') -Force
        }
    }
    else {
        foreach ($name in @('index.html', 'web.config')) {
            if (-not (Test-Path -LiteralPath (Join-Path $staging $name) -PathType Leaf)) {
                throw "Invalid WEB artifact: $name missing."
            }
        }
    }

    Stop-Website -Name $SiteName
    $websiteStopped = $true
    Stop-WebAppPool -Name $AppPoolName
    $poolStopped = $true

    if (Test-Path -LiteralPath $destination) {
        Move-Item -LiteralPath $destination -Destination $backup
        $oldMoved = $true
    }
    Move-Item -LiteralPath $staging -Destination $destination
    $newMoved = $true

    Start-WebAppPool -Name $AppPoolName
    $poolStopped = $false
    Start-Website -Name $SiteName
    $websiteStopped = $false
    Write-Host "Deployment completed: $Kind, IIS site $SiteName. Previous directory: $backup"
}
catch {
    $failure = $_
    Write-Warning 'Deployment failed. Restoring the previous website files where possible.'
    try {
        # Stop any partially started worker before attempting to roll back files.
        try {
            Stop-Website -Name $SiteName -ErrorAction SilentlyContinue
            Stop-WebAppPool -Name $AppPoolName -ErrorAction SilentlyContinue
        } catch { Write-Warning "Could not stop IIS during rollback: $_" }
        if ($newMoved -and (Test-Path -LiteralPath $destination)) {
            Remove-Item -LiteralPath $destination -Recurse -Force
        }
        if ($oldMoved -and (Test-Path -LiteralPath $backup)) {
            Move-Item -LiteralPath $backup -Destination $destination
        }
    } catch { Write-Error "Rollback of site files failed: $_" -ErrorAction Continue }
    try {
        if ($poolStopped) { Start-WebAppPool -Name $AppPoolName }
        if ($websiteStopped) { Start-Website -Name $SiteName }
    } catch { Write-Error "IIS restart after rollback failed: $_" -ErrorAction Continue }
    throw $failure
}
finally {
    if (Test-Path -LiteralPath $staging) {
        Remove-Item -LiteralPath $staging -Recurse -Force
    }
}
