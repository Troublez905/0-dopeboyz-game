import os
from pathlib import Path
from PIL import Image

p = Path(r"C:\Users\ghost\.codex\generated_images\01a0e796-662e-7ce1-b496-8ebec14bf8dc")
unmapped = [
    "exec-3d2651c1-dabc-4343-9d69-c5150f570ad0.png",
    "exec-479839a2-fb5a-4262-b6cc-15db77868c80.png",
    "exec-4cd1b6fd-6b56-4fdc-83a2-c50bdba2af8a.png",
    "exec-c44d01be-0d1b-4f18-9847-fd64a158e206.png",
    "exec-dd23bed1-0f5e-467e-afc6-cbef3aebad77.png",
    "exec-e479d9fc-aa0c-459c-81bd-b89f43d5b561.png",
    "exec-e7d31d79-b98e-48e1-ba7f-e2b530ded99a.png",
    "exec-efda0a7a-7bb3-4523-a40b-b195af03bab1.png"
]

print("Analyzing unmapped images...")
for u in unmapped:
    fp = p / u
    im = Image.open(fp)
    # Check bounding box of non-zero alpha
    bbox = im.getbbox()
    print(f"{u}: size={im.size}, aspect={im.width/im.height:.2f}, bbox={bbox}")
