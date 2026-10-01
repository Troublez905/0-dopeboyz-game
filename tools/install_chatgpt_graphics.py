import json
import os
import shutil
from pathlib import Path
from PIL import Image

ROOT = Path(r"C:\Users\ghost\Desktop\Ideas-Brainstorms\0-dopeboyz game")
PROMPTS_DIR = ROOT / "qa" / "graphics-prompts"

DEST_DIRS = [
    ROOT / "godot" / "assets",
    ROOT / "godot" / "assets" / "generated",
    ROOT / "unity" / "Assets" / "Sprites",
    ROOT / "unity" / "Assets" / "Sprites" / "generated"
]

for d in DEST_DIRS:
    d.mkdir(parents=True, exist_ok=True)

ALIAS_MAP = {
    "boombox_prop.png": ["boombox.png"],
    "police_k9_unit.png": ["k9_handler_officer.png"],
    "throwup.png": ["tag_style_throwup.png"],
    "bubble.png": ["tag_style_bubble.png"],
    "blockbuster.png": ["tag_style_blockbuster.png"],
    "chrome.png": ["tag_style_chrome.png"],
    "stencil.png": ["tag_style_stencil.png"],
    "wildstyle.png": ["tag_style_wildstyle.png"],
}

installed = 0
failed = 0
skipped = 0

print(f"Scanning {PROMPTS_DIR} for ChatGPT generated graphics...")

for jf in sorted(PROMPTS_DIR.glob("*.json")):
    try:
        data = json.loads(jf.read_text(encoding="utf-8"))
        name = data.get("name")
        w = data.get("w")
        h = data.get("h")
        src = data.get("source")

        if not name:
            name = jf.stem
            if not name.endswith(".png"):
                name += ".png"

        if not src or not os.path.exists(src):
            print(f"[-] SKIPPED: {name} (source file not found: {src})")
            skipped += 1
            continue

        im = Image.open(src)
        if im.mode != "RGBA":
            im = im.convert("RGBA")

        # Resize to target w, h if specified and different
        if w and h and (im.width != w or im.height != h):
            im_resized = im.resize((w, h), Image.Resampling.LANCZOS)
        else:
            im_resized = im

        target_names = [name]
        if name in ALIAS_MAP:
            for alias in ALIAS_MAP[name]:
                if alias not in target_names:
                    target_names.append(alias)

        for out_name in target_names:
            for dest_dir in DEST_DIRS:
                out_path = dest_dir / out_name
                im_resized.save(out_path, "PNG")
            
        print(f"[+] INSTALLED: {name} -> {target_names} ({im_resized.width}x{im_resized.height}) from {Path(src).name}")
        installed += 1

    except Exception as e:
        print(f"[!] ERROR processing {jf.name}: {e}")
        failed += 1

print(f"\n==========================================")
print(f"INSTALLATION COMPLETE:")
print(f"Installed: {installed} graphics")
print(f"Skipped:   {skipped}")
print(f"Failed:    {failed}")
print(f"Destinations: {len(DEST_DIRS)} directories synchronized")
print(f"==========================================")
