import json
import os
from pathlib import Path
from PIL import Image

prompt_dir = Path("qa/graphics-prompts")
for jf in sorted(prompt_dir.glob("*.json"))[:10]:
    try:
        data = json.loads(jf.read_text(encoding="utf-8"))
        name = data.get("name")
        w = data.get("w")
        h = data.get("h")
        src = data.get("source")
        if src and os.path.exists(src):
            im = Image.open(src)
            alpha_info = "No Alpha"
            if im.mode == "RGBA":
                ext = im.getextrema()
                alpha_info = f"Alpha min={ext[3][0]}, max={ext[3][1]}"
            print(f"{name:30} target=({w}x{h}) actual={im.size} mode={im.mode} {alpha_info}")
        else:
            print(f"{name:30} MISSING SOURCE: {src}")
    except Exception as e:
        print(f"Error reading {jf.name}: {e}")
