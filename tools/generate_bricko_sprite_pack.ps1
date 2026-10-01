param(
  [string]$CatalogPath = ".\new-graphics-generated01.md",
  [string[]]$OutputDirs = @(".\godot\assets", ".\unity\Assets\Sprites")
)

Add-Type -AssemblyName System.Drawing

$ErrorActionPreference = "Stop"

$palette = @(
  [Drawing.Color]::FromArgb(255, 76, 245, 213),
  [Drawing.Color]::FromArgb(255, 255, 59, 119),
  [Drawing.Color]::FromArgb(255, 255, 224, 109),
  [Drawing.Color]::FromArgb(255, 118, 255, 3),
  [Drawing.Color]::FromArgb(255, 180, 85, 255),
  [Drawing.Color]::FromArgb(255, 255, 65, 88)
)

function New-Color([int]$i, [int]$a = 255) {
  $c = $palette[$i % $palette.Count]
  [Drawing.Color]::FromArgb($a, $c.R, $c.G, $c.B)
}

function New-Pen([Drawing.Color]$c, [float]$w) {
  $p = [Drawing.Pen]::new($c, $w)
  $p.StartCap = [Drawing.Drawing2D.LineCap]::Round
  $p.EndCap = [Drawing.Drawing2D.LineCap]::Round
  $p.LineJoin = [Drawing.Drawing2D.LineJoin]::Round
  $p
}

function Fill-RoundedRect($g, $brush, [float]$x, [float]$y, [float]$w, [float]$h, [float]$r) {
  $path = [Drawing.Drawing2D.GraphicsPath]::new()
  $d = $r * 2
  $path.AddArc($x, $y, $d, $d, 180, 90)
  $path.AddArc($x + $w - $d, $y, $d, $d, 270, 90)
  $path.AddArc($x + $w - $d, $y + $h - $d, $d, $d, 0, 90)
  $path.AddArc($x, $y + $h - $d, $d, $d, 90, 90)
  $path.CloseFigure()
  $g.FillPath($brush, $path)
  $path.Dispose()
}

function Draw-InkShape($g, [string]$kind, [int]$w, [int]$h, [int]$seed) {
  $rng = [Random]::new($seed)
  $ink = New-Pen ([Drawing.Color]::FromArgb(245, 8, 8, 10)) ([Math]::Max(5, $w / 70))
  $accent = New-Pen (New-Color $seed) ([Math]::Max(4, $w / 95))
  $accent2 = New-Pen (New-Color ($seed + 2)) ([Math]::Max(3, $w / 120))
  $fill = [Drawing.SolidBrush]::new((New-Color ($seed + 1) 218))
  $fill2 = [Drawing.SolidBrush]::new((New-Color ($seed + 3) 190))
  $dark = [Drawing.SolidBrush]::new([Drawing.Color]::FromArgb(230, 34, 36, 42))
  $cx = $w / 2; $cy = $h / 2

  if ($kind -match "car|van|truck|chopper|blimp|motorcycle|train|trailer") {
    Fill-RoundedRect $g $dark ($w*.18) ($h*.28) ($w*.64) ($h*.42) ($w*.05)
    Fill-RoundedRect $g $fill ($w*.24) ($h*.34) ($w*.52) ($h*.28) ($w*.04)
    $g.DrawRectangle($ink, [int]($w*.18), [int]($h*.28), [int]($w*.64), [int]($h*.42))
    0..3 | ForEach-Object {
      $x = if ($_ % 2 -eq 0) { $w*.21 } else { $w*.69 }
      $y = if ($_ -lt 2) { $h*.22 } else { $h*.64 }
      $g.FillEllipse($dark, $x, $y, $w*.10, $h*.16)
      $g.DrawEllipse($accent, $x, $y, $w*.10, $h*.16)
    }
    $g.DrawLine($accent2, $w*.28, $h*.39, $w*.72, $h*.57)
  } elseif ($kind -match "queen|enforcer|officer|boss|handler|performer|pursuit|crowd|k9") {
    $g.FillEllipse($fill, $cx - $w*.09, $h*.13, $w*.18, $h*.18)
    $g.DrawEllipse($ink, $cx - $w*.09, $h*.13, $w*.18, $h*.18)
    Fill-RoundedRect $g $dark ($cx - $w*.13) ($h*.31) ($w*.26) ($h*.34) ($w*.04)
    $g.DrawLine($accent, $cx - $w*.16, $h*.39, $cx - $w*.34, $h*.56)
    $g.DrawLine($accent, $cx + $w*.16, $h*.39, $cx + $w*.34, $h*.56)
    $g.DrawLine($ink, $cx - $w*.09, $h*.63, $cx - $w*.20, $h*.86)
    $g.DrawLine($ink, $cx + $w*.09, $h*.63, $cx + $w*.20, $h*.86)
    0..5 | ForEach-Object { $g.FillEllipse($fill2, $rng.Next($w*.18,$w*.78), $rng.Next($h*.20,$h*.82), $w*.04, $h*.04) }
  } elseif ($kind -match "fx|fire|shield|puddle|lightning|smoke|crater|splatters|trail|cone|geyser|flare|underglow|soundquake|slowmo") {
    0..5 | ForEach-Object {
      $r = ($_.GetHashCode()+1) * [Math]::Min($w,$h) / 13
      $pen = New-Pen (New-Color ($seed + $_) (220 - $_*20)) ([Math]::Max(4, $w / 80))
      $g.DrawEllipse($pen, $cx - $r, $cy - $r*.72, $r*2, $r*1.44)
      $pen.Dispose()
    }
    0..10 | ForEach-Object {
      $g.DrawLine($accent2, $cx, $cy, $rng.Next($w*.12,$w*.88), $rng.Next($h*.12,$h*.88))
    }
  } elseif ($kind -match "graffiti|bubble|wildstyle|chrome|stencil|throwup|blockbuster|banner|billboard") {
    $font = [Drawing.Font]::new("Arial Black", [Math]::Max(34, [Math]::Min($w, $h) / 3), [Drawing.FontStyle]::Bold, [Drawing.GraphicsUnit]::Pixel)
    $txt = if ($kind -match "billboard") { "404" } else { "TAG" }
    $sf = [Drawing.StringFormat]::new()
    $sf.Alignment = "Center"; $sf.LineAlignment = "Center"
    $g.DrawString($txt, $font, [Drawing.SolidBrush]::new([Drawing.Color]::Black), [Drawing.RectangleF]::new($w*.08+5,$h*.1+5,$w*.84,$h*.8), $sf)
    $g.DrawString($txt, $font, $fill, [Drawing.RectangleF]::new($w*.08,$h*.1,$w*.84,$h*.8), $sf)
    0..7 | ForEach-Object { $g.DrawLine($accent2, $rng.Next($w*.1,$w*.9), $rng.Next($h*.2,$h*.8), $rng.Next($w*.1,$w*.9), $rng.Next($h*.2,$h*.8)) }
    $font.Dispose(); $sf.Dispose()
  } else {
    Fill-RoundedRect $g $dark ($w*.25) ($h*.20) ($w*.50) ($h*.56) ($w*.06)
    Fill-RoundedRect $g $fill ($w*.31) ($h*.27) ($w*.38) ($h*.42) ($w*.04)
    $g.DrawEllipse($accent, $w*.18, $h*.15, $w*.64, $h*.70)
    0..8 | ForEach-Object { $g.FillEllipse($fill2, $rng.Next($w*.18,$w*.78), $rng.Next($h*.16,$h*.76), $w*.05, $h*.05) }
  }

  $ink.Dispose(); $accent.Dispose(); $accent2.Dispose()
  $fill.Dispose(); $fill2.Dispose(); $dark.Dispose()
}

function Get-AssetSize([string]$catalog, [string]$name) {
  $needle = "``" + $name + "``"
  $idx = $catalog.IndexOf($needle)
  if ($idx -lt 0) { return @(512, 512) }
  $nextHeading = $catalog.IndexOf("### ", $idx + $needle.Length)
  if ($nextHeading -lt 0) { $nextHeading = $catalog.Length }
  $section = $catalog.Substring($idx, $nextHeading - $idx)
  $m = [regex]::Match($section, "Target Size\*\*:\s*(\d+)x(\d+)")
  if ($m.Success) { return @([int]$m.Groups[1].Value, [int]$m.Groups[2].Value) }
  @(512, 512)
}

$catalog = Get-Content -LiteralPath $CatalogPath -Raw
$names = [regex]::Matches($catalog, '`([^`]+\.png)`') |
  ForEach-Object { $_.Groups[1].Value } |
  Where-Object { $_ -notmatch "\*" } |
  Sort-Object -Unique

foreach ($dir in $OutputDirs) {
  New-Item -ItemType Directory -Force -Path $dir | Out-Null
}

$generated = @()
foreach ($name in $names) {
  $size = Get-AssetSize $catalog $name
  $w = $size[0]; $h = $size[1]
  $bmp = [Drawing.Bitmap]::new($w, $h, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
  $g = [Drawing.Graphics]::FromImage($bmp)
  $g.SmoothingMode = [Drawing.Drawing2D.SmoothingMode]::AntiAlias
  $g.Clear([Drawing.Color]::Transparent)

  $seed = [Math]::Abs($name.GetHashCode())
  Draw-InkShape $g $name $w $h $seed

  foreach ($dir in $OutputDirs) {
    $out = Join-Path $dir $name
    $bmp.Save($out, [Drawing.Imaging.ImageFormat]::Png)
  }

  $generated += [pscustomobject]@{ file = $name; width = $w; height = $h }
  $g.Dispose()
  $bmp.Dispose()
}

$manifestPath = Join-Path (Split-Path -Parent $CatalogPath) "generated-graphics-manifest.csv"
$generated | Sort-Object file | Export-Csv -NoTypeInformation -Path $manifestPath
Write-Host "Generated $($generated.Count) transparent PNG assets into:"
$OutputDirs | ForEach-Object { Write-Host " - $((Resolve-Path $_).Path)" }
Write-Host "Manifest: $manifestPath"
