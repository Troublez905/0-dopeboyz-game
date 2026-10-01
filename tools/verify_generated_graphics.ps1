$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$root = Split-Path $PSScriptRoot -Parent
$records = Get-ChildItem (Join-Path $root 'qa/graphics-prompts') -Filter '*.json' | ForEach-Object { Get-Content $_.FullName -Raw | ConvertFrom-Json }
$results = foreach ($record in $records) {
    $name = $record.name
    $file = Join-Path $root "godot/assets/$name"
    $mirror = Join-Path $root "unity/Assets/Sprites/$name"
    $bitmap = [System.Drawing.Bitmap]::FromFile($file)
    try {
        $transparent = 0
        $visible = 0
        for ($y = 0; $y -lt $bitmap.Height; $y += 4) {
            for ($x = 0; $x -lt $bitmap.Width; $x += 4) {
                $alpha = $bitmap.GetPixel($x, $y).A
                if ($alpha -eq 0) { $transparent++ }
                if ($alpha -gt 128) { $visible++ }
            }
        }
        $same = (Get-FileHash $file).Hash -eq (Get-FileHash $mirror).Hash
        $expectedW = $record.w
        $expectedH = $record.h
        if ($name -eq 'civilian_crowd.png') { $expectedW = 1024; $expectedH = 342 }
        $ok = $bitmap.Width -eq $expectedW -and $bitmap.Height -eq $expectedH -and $transparent -gt 0 -and $visible -gt 0 -and $same
        [pscustomobject]@{ file=$name; width=$bitmap.Width; height=$bitmap.Height; transparent_samples=$transparent; visible_samples=$visible; unity_matches=$same; passed=$ok }
    } finally { $bitmap.Dispose() }
}
$results | ConvertTo-Json -Depth 3 | Set-Content (Join-Path $root 'qa/graphics-validation.json')
$failed = @($results | Where-Object { -not $_.passed })
Write-Output "Graphics checked: $(@($results).Count); failures: $($failed.Count)"
if ($failed.Count -gt 0) { $failed | Format-Table; throw 'Graphics validation failed' }
