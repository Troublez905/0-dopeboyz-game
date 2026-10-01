param(
    [Parameter(Mandatory=$true)][string]$Source,
    [Parameter(Mandatory=$true)][string]$Name,
    [Parameter(Mandatory=$true)][int]$Width,
    [Parameter(Mandatory=$true)][int]$Height
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$root = Split-Path $PSScriptRoot -Parent
$inputImage = [System.Drawing.Image]::FromFile($Source)
$output = New-Object System.Drawing.Bitmap($Width, $Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$graphics = [System.Drawing.Graphics]::FromImage($output)
try {
    $graphics.Clear([System.Drawing.Color]::Transparent)
    $graphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.DrawImage($inputImage, 0, 0, $Width, $Height)
    $target = Join-Path $root "godot/assets/$Name"
    $output.Save($target, [System.Drawing.Imaging.ImageFormat]::Png)
    $unity = Join-Path $root "unity/Assets/Sprites/$Name"
    if (Test-Path (Split-Path $unity -Parent)) { Copy-Item -LiteralPath $target -Destination $unity -Force }
    $transparent = 0
    for ($y = 0; $y -lt $Height; $y += 8) {
        for ($x = 0; $x -lt $Width; $x += 8) {
            if ($output.GetPixel($x, $y).A -eq 0) { $transparent++ }
        }
    }
    if ($transparent -eq 0) { throw "No transparent pixels in $Name" }
    Write-Output "$Name ${Width}x${Height}: exported, transparent samples=$transparent"
} finally {
    $graphics.Dispose()
    $output.Dispose()
    $inputImage.Dispose()
}
