[CmdletBinding()]
param(
    [string]$GameDir = 'F:\Steam\steamapps\common\Stonewards',
    [string]$ProfileDir = ''
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$project = Join-Path $PSScriptRoot 'StonewardsGoldRingScaling.csproj'
if (-not (Test-Path -LiteralPath $GameDir -PathType Container)) {
    throw "Game directory not found: $GameDir"
}
$GameDir = (Resolve-Path -LiteralPath $GameDir).Path
$managed = Join-Path $GameDir 'Stonewards_Data\Managed'

$core = Join-Path $GameDir 'BepInEx\core'
if (-not (Test-Path -LiteralPath (Join-Path $core 'BepInEx.dll'))) {
    if (-not $ProfileDir) {
        throw 'BepInEx.dll is missing from the game. Supply -ProfileDir with your mod-manager profile.'
    }
    $core = Join-Path $ProfileDir 'BepInEx\core'
}

$required = @(
    (Join-Path $managed 'Assembly-CSharp.dll'),
    (Join-Path $managed 'UnityEngine.dll'),
    (Join-Path $managed 'UnityEngine.CoreModule.dll'),
    (Join-Path $core 'BepInEx.dll'),
    (Join-Path $core '0Harmony.dll')
)
foreach ($path in $required) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Required reference missing: $path"
    }
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'dotnet CLI is missing. Install .NET SDK (same SDK used to build PartyHUD).'
}

Write-Host 'Building Gold Ring Scaling...'
& dotnet build $project -c Release "-p:GameDir=$GameDir" "-p:BepInExCoreDir=$core"
if ($LASTEXITCODE -ne 0) {
    throw 'Build failed. No files were installed.'
}

$dll = Join-Path $PSScriptRoot 'bin\Release\netstandard2.1\StonewardsGoldRingScaling.dll'
if (-not (Test-Path -LiteralPath $dll -PathType Leaf)) {
    throw "Built DLL not found: $dll"
}

$installRoot = $GameDir
if ($ProfileDir) {
    if (-not (Test-Path -LiteralPath $ProfileDir -PathType Container)) {
        throw "Profile directory not found: $ProfileDir"
    }
    $installRoot = (Resolve-Path -LiteralPath $ProfileDir).Path
}
$destination = Join-Path $installRoot 'BepInEx\plugins\StonewardsGoldRingScaling'
New-Item -ItemType Directory -Force -Path $destination | Out-Null
Copy-Item -LiteralPath $dll -Destination (Join-Path $destination 'StonewardsGoldRingScaling.dll') -Force

Write-Host 'Installed:' -ForegroundColor Green
Write-Host (Join-Path $destination 'StonewardsGoldRingScaling.dll')
Write-Host 'Restart Stonewards. Mod-loader detection / leaderboard restrictions remain unchanged.'
