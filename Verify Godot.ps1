param([switch]$Render)
$ErrorActionPreference = 'Continue'
$engine = Join-Path $PSScriptRoot 'tools\godot\Godot_v4.6.2-stable_win64_console.exe'
$project = Join-Path $PSScriptRoot 'godot'
$qa = Join-Path $PSScriptRoot 'qa'
New-Item -ItemType Directory -Force -Path $qa | Out-Null
$arguments = @('--path', $project)
if (-not $Render) { $arguments += '--headless' }
$arguments += @('--', '--qa')
& $engine @arguments 2>&1 | Tee-Object -FilePath (Join-Path $qa 'verification.log')
$exitCode = $LASTEXITCODE
if ($exitCode -ne 0) { throw "Godot checks failed with exit code $exitCode" }
if (Select-String -LiteralPath (Join-Path $qa 'verification.log') -Pattern 'SCRIPT ERROR:|^ERROR:|^FAIL:' -Quiet) {
    throw 'Godot reported a runtime error; inspect qa/verification.log.'
}
Write-Host 'Verification complete. See qa/godot-results.json.'
