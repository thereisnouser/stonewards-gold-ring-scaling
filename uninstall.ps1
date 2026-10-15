[CmdletBinding()]
param(
    [string]$GameDir = 'F:\Steam\steamapps\common\Stonewards',
    [string]$ProfileDir = ''
)
$ErrorActionPreference = 'Stop'
$installRoot = if ($ProfileDir) { $ProfileDir } else { $GameDir }
$destination = Join-Path $installRoot 'BepInEx\plugins\StonewardsGoldRingScaling\StonewardsGoldRingScaling.dll'
if (Test-Path -LiteralPath $destination -PathType Leaf) {
    Remove-Item -LiteralPath $destination -Force
    Write-Host "Removed: $destination" -ForegroundColor Green
} else {
    Write-Host "Already absent: $destination"
}
Write-Host 'Restart the game to reset cached ring effects.'
