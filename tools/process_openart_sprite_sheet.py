from pathlib import Path
from PIL import Image


ROOT = Path(r"C:\Users\ghost\Desktop\Ideas-Brainstorms\0-dopeboyz game")
SOURCE = ROOT / "godot" / "assets" / "openart_source" / "openart_sprite_sheet_W6ukAyqpG8b7S1Psxlwh.webp"
GODOT = ROOT / "godot" / "assets"
UNITY = ROOT / "unity" / "Assets" / "Sprites"

TARGETS = {
    "lowrider_hydraulic_car.png": ((0, 0, 512, 512), (512, 512)),
    "swat_van_breaching.png": ((512, 0, 1024, 512), (512, 512)),
    "roller_queen.png": ((0, 512, 512, 1024), (1024, 1024)),
    "wildstyle.png": ((512, 512, 1024, 1024), (512, 256)),
}


def trim_and_pad(img: Image.Image, target_size: tuple[int, int]) -> Image.Image:
    """Trim near-empty edges, preserve transparency, then fit inside target canvas."""
    img = img.convert("RGBA")
    alpha = img.getchannel("A")
    bbox = alpha.getbbox()
    if bbox:
        img = img.crop(bbox)

    target_w, target_h = target_size
    scale = min(target_w / img.width, target_h / img.height) * 0.92
    new_size = (max(1, int(img.width * scale)), max(1, int(img.height * scale)))
    resample = getattr(Image.Resampling, "LANCZOS", Image.LANCZOS)
    img = img.resize(new_size, resample)

    canvas = Image.new("RGBA", target_size, (0, 0, 0, 0))
    canvas.alpha_composite(img, ((target_w - img.width) // 2, (target_h - img.height) // 2))
    return canvas


def main() -> None:
    sheet = Image.open(SOURCE).convert("RGBA")
    if sheet.size != (1024, 1024):
        sheet = sheet.resize((1024, 1024), getattr(Image.Resampling, "LANCZOS", Image.LANCZOS))

    converted = SOURCE.with_suffix(".png")
    sheet.save(converted)

    for filename, (box, target_size) in TARGETS.items():
        sprite = trim_and_pad(sheet.crop(box), target_size)
        for folder in (GODOT, UNITY):
            folder.mkdir(parents=True, exist_ok=True)
            sprite.save(folder / filename)
        print(f"{filename}: {target_size[0]}x{target_size[1]}")

    print(f"source_png: {converted}")


if __name__ == "__main__":
    main()
