import math
import os
import random
from PIL import Image, ImageDraw, ImageFont, ImageFilter

def make_dirs():
    os.makedirs("godot/assets/generated", exist_ok=True)
    os.makedirs("godot/assets", exist_ok=True)

# 1. Deployable Paint Cannon Turret
def create_paint_turret():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 140
    for angle in [-0.8, 0, 0.8]:
        tx = cx + math.sin(angle) * 70
        ty = cy + math.cos(angle) * 60 + 20
        draw.line([cx, cy, tx, ty], fill=(40, 48, 56, 255), width=7)
        draw.ellipse([tx - 8, ty - 4, tx + 8, ty + 8], fill=(20, 24, 30, 255))
    for tx in [cx - 28, cx + 28]:
        draw.rectangle([tx - 14, cy - 40, tx + 14, cy + 10], fill=(76, 245, 213, 255), outline=(20, 24, 30, 255), width=3)
        draw.ellipse([tx - 14, cy - 50, tx + 14, cy - 35], fill=(255, 59, 119, 255))
    draw.ellipse([cx - 32, cy - 35, cx + 32, cy + 15], fill=(25, 30, 38, 255), outline=(255, 224, 109, 255), width=3)
    draw.rectangle([cx - 12, cy - 75, cx - 4, cy - 20], fill=(180, 190, 200, 255), outline=(10, 12, 16, 255), width=2)
    draw.rectangle([cx + 4, cy - 75, cx + 12, cy - 20], fill=(180, 190, 200, 255), outline=(10, 12, 16, 255), width=2)
    draw.ellipse([cx - 14, cy - 82, cx - 2, cy - 72], fill=(76, 245, 213, 255))
    draw.ellipse([cx + 2, cy - 82, cx + 14, cy - 72], fill=(76, 245, 213, 255))
    img.save("godot/assets/generated/paint_turret_prop.png")
    img.save("godot/assets/paint_turret_prop.png")
    print("Created paint_turret_prop.png")

# 2. Sewer Manhole Passage
def create_sewer_manhole():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    draw.ellipse([cx - 85, cy - 85, cx + 85, cy + 85], fill=(30, 36, 44, 255), outline=(90, 100, 110, 255), width=6)
    draw.ellipse([cx - 70, cy - 70, cx + 70, cy + 70], fill=(15, 20, 25, 255))
    draw.ellipse([cx - 60, cy - 60, cx + 60, cy + 60], fill=(76, 245, 213, 40))
    for r in range(-50, 60, 22):
        draw.line([cx - 65, cy + r, cx + 65, cy + r], fill=(70, 80, 90, 255), width=4)
        draw.line([cx + r, cy - 65, cx + r, cy + 65], fill=(70, 80, 90, 255), width=4)
    draw.ellipse([cx - 28, cy - 28, cx + 28, cy + 28], fill=(45, 55, 68, 255), outline=(255, 224, 109, 255), width=3)
    draw.text((cx - 18, cy - 8), "404", fill=(255, 224, 109, 255))
    img.save("godot/assets/generated/manhole_sewer_passage.png")
    img.save("godot/assets/manhole_sewer_passage.png")
    print("Created manhole_sewer_passage.png")

# 3. Zipline Cable & Pulley
def create_zipline_cable():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.line([(10, 80), (502, 80)], fill=(180, 190, 200, 255), width=6)
    draw.line([(10, 86), (502, 86)], fill=(76, 245, 213, 160), width=3)
    # Pulley Trolley Wheel
    cx, cy = 256, 80
    draw.ellipse([cx - 24, cy - 24, cx + 24, cy + 24], fill=(40, 48, 56, 255), outline=(255, 224, 109, 255), width=3)
    draw.rectangle([cx - 14, cy, cx + 14, cy + 50], fill=(60, 70, 80, 255), outline=(20, 20, 20, 255), width=2)
    # Grind Sparks
    for _ in range(20):
        sx = random.randint(180, 330)
        sy = random.randint(70, 110)
        draw.ellipse([sx - 3, sy - 3, sx + 3, sy + 3], fill=(255, 224, 109, 255))
    img.save("godot/assets/generated/zipline_grind_cable.png")
    img.save("godot/assets/zipline_grind_cable.png")
    print("Created zipline_grind_cable.png")

# 4. Thermal Stealth Foil
def create_thermal_foil():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    draw.ellipse([cx - 95, cy - 95, cx + 95, cy + 95], fill=(230, 240, 250, 60), outline=(76, 245, 213, 200), width=4)
    # Shiny reflective foil triangles
    for i in range(12):
        p1 = (cx, cy)
        a1 = i * (math.pi / 6)
        a2 = (i + 1) * (math.pi / 6)
        p2 = (cx + math.cos(a1) * 85, cy + math.sin(a1) * 85)
        p3 = (cx + math.cos(a2) * 85, cy + math.sin(a2) * 85)
        col = (220, 235, 255, 140) if i % 2 == 0 else (180, 210, 240, 180)
        draw.polygon([p1, p2, p3], fill=col)
    img.save("godot/assets/generated/thermal_foil_stealth.png")
    img.save("godot/assets/thermal_foil_stealth.png")
    print("Created thermal_foil_stealth.png")

# 5. Paint Mine Trap
def create_paint_mine():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    draw.ellipse([cx - 90, cy - 90, cx + 90, cy + 90], fill=(255, 59, 119, 15), outline=(255, 59, 119, 120), width=3)
    draw.ellipse([cx - 45, cy - 45, cx + 45, cy + 45], fill=(20, 25, 32, 255), outline=(255, 224, 109, 255), width=4)
    for a in range(0, 360, 60):
        rad = math.radians(a)
        vx = cx + math.cos(rad) * 32
        vy = cy + math.sin(rad) * 32
        draw.ellipse([vx - 6, vy - 6, vx + 6, vy + 6], fill=(76, 245, 213, 255))
    draw.ellipse([cx - 14, cy - 14, cx + 14, cy + 14], fill=(255, 30, 60, 255), outline=(255, 255, 255, 255), width=2)
    img.save("godot/assets/generated/paint_mine_trap.png")
    img.save("godot/assets/paint_mine_trap.png")
    print("Created paint_mine_trap.png")

# 6. Stencil Workshop UI
def create_stencil_workshop():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rectangle([20, 20, 492, 236], fill=(18, 22, 28, 240), outline=(76, 245, 213, 255), width=4)
    draw.rectangle([40, 40, 260, 216], fill=(30, 36, 44, 255), outline=(60, 70, 80, 255), width=2)
    # Stencil grid pattern
    for x in range(50, 250, 20):
        draw.line([(x, 40), (x, 216)], fill=(50, 60, 70, 255), width=1)
    for y in range(50, 210, 20):
        draw.line([(40, y), (260, y)], fill=(50, 60, 70, 255), width=1)
    draw.text((280, 50), "STENCIL WORKSHOP", fill=(255, 224, 109, 255))
    draw.text((280, 90), "LAYER 01: OUTLINE", fill=(76, 245, 213, 255))
    draw.text((280, 120), "LAYER 02: FILL", fill=(255, 59, 119, 255))
    draw.text((280, 150), "PERK: +20% CASH", fill=(76, 245, 213, 255))
    img.save("godot/assets/generated/stencil_workshop_ui.png")
    img.save("godot/assets/stencil_workshop_ui.png")
    print("Created stencil_workshop_ui.png")

# 7. News Broadcast Chopper
def create_news_chopper():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 128
    draw.ellipse([cx - 200, cy - 65, cx + 200, cy - 45], fill=(255, 255, 255, 40), outline=(200, 220, 240, 160), width=3)
    draw.line([cx - 190, cy - 55, cx + 190, cy - 55], fill=(220, 230, 240, 200), width=4)
    draw.ellipse([cx - 110, cy - 45, cx + 60, cy + 45], fill=(240, 245, 250, 255), outline=(20, 25, 30, 255), width=4)
    draw.rectangle([cx - 40, cy - 20, cx + 60, cy + 40], fill=(0, 100, 200, 255))
    draw.text((cx - 20, cy - 5), "NEWS 404", fill=(255, 255, 255, 255))
    draw.ellipse([cx - 120, cy - 35, cx - 60, cy + 25], fill=(76, 245, 213, 160), outline=(255, 255, 255, 255), width=2)
    draw.rectangle([cx + 50, cy - 10, cx + 190, cy + 10], fill=(240, 245, 250, 255), outline=(20, 25, 30, 255), width=3)
    draw.ellipse([cx + 175, cy - 30, cx + 205, cy + 30], fill=(255, 255, 255, 60), outline=(255, 59, 119, 255), width=2)
    draw.polygon([(cx - 80, cy + 40), (cx - 180, cy + 120), (cx + 20, cy + 120)], fill=(255, 224, 109, 45))
    img.save("godot/assets/generated/news_chopper.png")
    img.save("godot/assets/news_chopper.png")
    print("Created news_chopper.png")

# 8. Rival Boss Captain
def create_rival_boss_captain():
    size = (512, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 256
    # Aura
    draw.ellipse([cx - 140, cy - 140, cx + 140, cy + 140], fill=(255, 59, 119, 40))
    # Body Spiked Leather Jacket
    draw.rectangle([cx - 80, cy - 60, cx + 80, cy + 100], fill=(30, 20, 25, 255), outline=(255, 59, 119, 255), width=5)
    # Spikes on shoulders
    for sx in [-80, 80]:
        draw.polygon([(sx + cx, cy - 60), (sx + cx - 15, cy - 90), (sx + cx + 15, cy - 90)], fill=(200, 200, 210, 255))
    # Head & Respirator Mask
    draw.ellipse([cx - 50, cy - 150, cx + 50, cy - 50], fill=(40, 30, 35, 255), outline=(255, 224, 109, 255), width=4)
    draw.rectangle([cx - 30, cy - 90, cx + 30, cy - 50], fill=(20, 20, 20, 255), outline=(255, 59, 119, 255), width=3)
    draw.text((cx - 45, cy - 20), "BOSS CAPTAIN", fill=(255, 59, 119, 255))
    img.save("godot/assets/generated/rival_boss_captain.png")
    img.save("godot/assets/rival_boss_captain.png")
    print("Created rival_boss_captain.png")

# 9. Industrial Air Compressor Station
def create_air_compressor():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 140
    # Red Tank Body
    draw.rectangle([cx - 50, cy - 50, cx + 50, cy + 50], fill=(220, 40, 40, 255), outline=(20, 20, 20, 255), width=4)
    draw.ellipse([cx - 50, cy - 70, cx + 50, cy - 30], fill=(220, 40, 40, 255))
    # Pressure Gauge Dial
    draw.ellipse([cx - 18, cy - 40, cx + 18, cy - 4], fill=(240, 245, 250, 255), outline=(20, 20, 20, 255), width=2)
    draw.line([(cx, cy - 22), (cx + 10, cy - 28)], fill=(220, 30, 30, 255), width=3)
    # Hose
    draw.arc([cx + 30, cy - 20, cx + 70, cy + 40], 270, 90, fill=(30, 35, 40, 255), width=6)
    img.save("godot/assets/generated/air_compressor_station.png")
    img.save("godot/assets/air_compressor_station.png")
    print("Created air_compressor_station.png")

# 10. 6 Nozzle Caps Sheet
def create_nozzle_caps():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    caps = [
        ("Monster Fat", (76, 245, 213, 255)),
        ("Needle Tip", (255, 224, 109, 255)),
        ("Chisel", (255, 59, 119, 255)),
        ("Soft Fan", (192, 132, 252, 255)),
        ("Splatter", (89, 183, 255, 255)),
        ("Flare Nozzle", (255, 140, 0, 255))
    ]
    for i, (name, col) in enumerate(caps):
        cx = (i % 3) * 160 + 80
        cy = (i // 3) * 110 + 65
        draw.ellipse([cx - 25, cy - 25, cx + 25, cy + 25], fill=(30, 36, 44, 255), outline=col, width=3)
        draw.ellipse([cx - 10, cy - 10, cx + 10, cy + 10], fill=col)
        draw.text((cx - 30, cy + 30), name, fill=(240, 245, 250, 255))
    img.save("godot/assets/generated/nozzle_caps_sheet.png")
    img.save("godot/assets/nozzle_caps_sheet.png")
    print("Created nozzle_caps_sheet.png")

# 11. Trash Can Fire Warmup Zone
def create_barrel_fire():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 160
    # Warm Halo
    draw.ellipse([cx - 100, cy - 60, cx + 100, cy + 60], fill=(255, 140, 0, 50))
    # Metal Barrel
    draw.rectangle([cx - 40, cy - 40, cx + 40, cy + 50], fill=(50, 55, 65, 255), outline=(20, 20, 20, 255), width=3)
    # Rising Flames
    for _ in range(15):
        fx = cx + random.randint(-25, 25)
        fy = cy - 40 - random.randint(10, 60)
        fr = random.randint(8, 20)
        draw.ellipse([fx - fr, fy - fr, fx + fr, fy + fr], fill=(255, random.randint(100, 220), 0, 180))
    img.save("godot/assets/generated/barrel_fire_warmup.png")
    img.save("godot/assets/barrel_fire_warmup.png")
    print("Created barrel_fire_warmup.png")

# 12. Police K9 Unit
def create_police_k9():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 140
    draw.ellipse([cx - 50, cy + 40, cx + 50, cy + 62], fill=(0, 0, 0, 120))
    for lx in [-30, -15, 15, 30]:
        draw.rectangle([cx + lx - 6, cy + 10, cx + lx + 6, cy + 50], fill=(60, 45, 30, 255), outline=(20, 20, 20, 255), width=2)
    draw.ellipse([cx - 45, cy - 25, cx + 35, cy + 25], fill=(70, 50, 32, 255), outline=(20, 20, 20, 255), width=3)
    draw.rectangle([cx - 25, cy - 22, cx + 15, cy + 22], fill=(25, 35, 50, 255), outline=(255, 224, 109, 255), width=2)
    draw.text((cx - 18, cy - 8), "K9", fill=(255, 224, 109, 255))
    draw.ellipse([cx - 55, cy - 45, cx - 25, cy - 10], fill=(80, 55, 35, 255), outline=(20, 20, 20, 255), width=2)
    draw.rectangle([cx - 75, cy - 32, cx - 50, cy - 12], fill=(30, 22, 16, 255))
    draw.polygon([(cx - 48, cy - 45), (cx - 40, cy - 68), (cx - 32, cy - 45)], fill=(60, 40, 25, 255))
    draw.polygon([(cx - 35, cy - 45), (cx - 28, cy - 68), (cx - 20, cy - 45)], fill=(60, 40, 25, 255))
    draw.rectangle([cx - 50, cy - 20, cx - 40, cy - 10], fill=(255, 30, 30, 255))
    img.save("godot/assets/generated/police_k9_unit.png")
    img.save("godot/assets/police_k9_unit.png")
    print("Created police_k9_unit.png")

# 13. Water Tower Mega-Mural
def create_water_tower():
    size = (512, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 260
    for lx in [cx - 120, cx - 40, cx + 40, cx + 120]:
        draw.line([lx, cy + 50, lx * 0.8 + cx * 0.2, cy + 220], fill=(70, 50, 35, 255), width=8)
    draw.line([cx - 130, cy + 140, cx + 130, cy + 140], fill=(50, 35, 25, 255), width=6)
    draw.rectangle([cx - 140, cy - 130, cx + 140, cy + 60], fill=(90, 65, 45, 255), outline=(30, 20, 15, 255), width=5)
    for hy in [-80, -20, 30]:
        draw.line([cx - 140, cy + hy, cx + 140, cy + hy], fill=(160, 170, 180, 255), width=6)
    draw.polygon([(cx - 160, cy - 130), (cx + 160, cy - 130), (cx, cy - 220)], fill=(120, 40, 40, 255), outline=(30, 20, 15, 255), width=4)
    draw.ellipse([cx - 110, cy - 100, cx + 110, cy + 30], fill=(76, 245, 213, 80))
    draw.text((cx - 90, cy - 45), "404 CITY KING", fill=(255, 224, 109, 255))
    draw.text((cx - 90, cy - 48), "404 CITY KING", fill=(255, 59, 119, 255))
    img.save("godot/assets/generated/water_tower_mural.png")
    img.save("godot/assets/water_tower_mural.png")
    print("Created water_tower_mural.png")

# 14. Storefront Glass Window
def create_storefront_glass():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rectangle([20, 20, 492, 236], fill=(30, 40, 55, 180), outline=(76, 245, 213, 255), width=4)
    # Glass cracks
    for _ in range(8):
        x1 = random.randint(40, 470)
        y1 = random.randint(40, 210)
        x2 = x1 + random.randint(-60, 60)
        y2 = y1 + random.randint(-40, 40)
        draw.line([(x1, y1), (x2, y2)], fill=(255, 255, 255, 220), width=2)
    draw.text((180, 110), "NEON STORE", fill=(255, 59, 119, 255))
    img.save("godot/assets/generated/storefront_glass.png")
    img.save("godot/assets/storefront_glass.png")
    print("Created storefront_glass.png")

# 15. Jetpack Thrusters
def create_jetpack_thrusters():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 100
    for tx in [cx - 30, cx + 30]:
        draw.rectangle([tx - 15, cy - 40, tx + 15, cy + 20], fill=(40, 48, 56, 255), outline=(76, 245, 213, 255), width=3)
        # Flame plume
        draw.polygon([(tx - 12, cy + 20), (tx + 12, cy + 20), (tx, cy + 100)], fill=(76, 245, 213, 220))
        draw.polygon([(tx - 6, cy + 20), (tx + 6, cy + 20), (tx, cy + 70)], fill=(255, 224, 109, 255))
    img.save("godot/assets/generated/jetpack_thruster.png")
    img.save("godot/assets/jetpack_thruster.png")
    print("Created jetpack_thruster.png")

# 16. Block Party Scene
def create_block_party():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rectangle([20, 20, 492, 236], fill=(15, 18, 25, 220), outline=(255, 224, 109, 255), width=4)
    for i in range(8):
        cx = 50 + i * 55
        draw.ellipse([cx - 15, 120, cx + 15, 150], fill=(76, 245, 213, 255))
        draw.ellipse([cx - 10, 80, cx + 10, 115], fill=(255, 59, 119, 255))
    draw.text((160, 40), "🎉 STREET BLOCK PARTY 🎉", fill=(255, 224, 109, 255))
    img.save("godot/assets/generated/block_party_crowd.png")
    img.save("godot/assets/block_party_crowd.png")
    print("Created block_party_crowd.png")

# 17. Electrified Puddle
def create_electrified_puddle():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    draw.ellipse([cx - 100, cy - 60, cx + 100, cy + 60], fill=(30, 90, 160, 180), outline=(76, 245, 213, 255), width=3)
    for _ in range(12):
        x1 = cx + random.randint(-80, 80)
        y1 = cy + random.randint(-40, 40)
        x2 = x1 + random.randint(-30, 30)
        y2 = y1 + random.randint(-20, 20)
        draw.line([(x1, y1), (x2, y2)], fill=(255, 255, 255, 255), width=3)
        draw.line([(x1, y1), (x2, y2)], fill=(89, 183, 255, 255), width=6)
    img.save("godot/assets/generated/electrified_puddle.png")
    img.save("godot/assets/electrified_puddle.png")
    print("Created electrified_puddle.png")

# 18. Skateboard Decks Sheet
def create_skateboards_sheet():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    decks = [
        ("Cyber Neon", (76, 245, 213, 255)),
        ("Wildstyle", (255, 59, 119, 255)),
        ("Midnight", (192, 132, 252, 255)),
        ("Gold Flame", (255, 224, 109, 255))
    ]
    for i, (name, col) in enumerate(decks):
        dx = i * 125 + 60
        draw.rectangle([dx - 20, 30, dx + 20, 200], fill=(25, 30, 38, 255), outline=col, width=3)
        draw.ellipse([dx - 20, 15, dx + 20, 45], fill=col)
        draw.ellipse([dx - 20, 185, dx + 20, 215], fill=col)
        draw.text((dx - 25, 220), name, fill=(240, 245, 250, 255))
    img.save("godot/assets/generated/skateboards_sheet.png")
    img.save("godot/assets/skateboards_sheet.png")
    print("Created skateboards_sheet.png")

# 19. Turf Flare Marker
def create_turf_flare():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 160
    draw.ellipse([cx - 90, cy - 50, cx + 90, cy + 50], fill=(255, 59, 119, 60))
    draw.rectangle([cx - 8, cy - 20, cx + 8, cy + 40], fill=(220, 40, 40, 255), outline=(20, 20, 20, 255), width=2)
    draw.polygon([(cx - 18, cy - 20), (cx + 18, cy - 20), (cx, cy - 80)], fill=(255, 224, 109, 255))
    draw.ellipse([cx - 30, cy - 110, cx + 30, cy - 50], fill=(255, 59, 119, 140))
    img.save("godot/assets/generated/turf_flare_marker.png")
    img.save("godot/assets/turf_flare_marker.png")
    print("Created turf_flare_marker.png")

# 20. City Hall Monument
def create_city_hall_monument():
    size = (512, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 260
    draw.rectangle([cx - 160, cy + 100, cx + 160, cy + 180], fill=(200, 205, 210, 255), outline=(30, 35, 40, 255), width=4)
    draw.rectangle([cx - 120, cy - 80, cx + 120, cy + 100], fill=(220, 225, 230, 255), outline=(30, 35, 40, 255), width=4)
    # Brass Crown
    draw.polygon([(cx - 70, cy - 80), (cx - 80, cy - 150), (cx - 35, cy - 110), (cx, cy - 170), (cx + 35, cy - 110), (cx + 80, cy - 150), (cx + 70, cy - 80)], fill=(255, 215, 0, 255), outline=(20, 20, 20, 255), width=3)
    draw.text((cx - 100, cy), "404 KINGDOM", fill=(255, 59, 119, 255))
    img.save("godot/assets/generated/city_hall_monument.png")
    img.save("godot/assets/city_hall_monument.png")
    print("Created city_hall_monument.png")

if __name__ == "__main__":
    make_dirs()
    create_paint_turret()
    create_sewer_manhole()
    create_zipline_cable()
    create_thermal_foil()
    create_paint_mine()
    create_stencil_workshop()
    create_news_chopper()
    create_rival_boss_captain()
    create_air_compressor()
    create_nozzle_caps()
    create_barrel_fire()
    create_police_k9()
    create_water_tower()
    create_storefront_glass()
    create_jetpack_thrusters()
    create_block_party()
    create_electrified_puddle()
    create_skateboards_sheet()
    create_turf_flare()
    create_city_hall_monument()
    print("All 20 Batch 4 graphics generated successfully!")
