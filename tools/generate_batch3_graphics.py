import math
import os
import random
from PIL import Image, ImageDraw, ImageFont, ImageFilter

def make_dirs():
    os.makedirs("godot/assets/generated", exist_ok=True)
    os.makedirs("godot/assets", exist_ok=True)

# 1. Spray Drone Scout
def create_drone_scout():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    draw.polygon([(cx - 80, cy + 100), (cx + 80, cy + 100), (cx + 25, cy + 20), (cx - 25, cy + 20)], fill=(76, 245, 213, 40))
    arm_col = (40, 48, 56, 255)
    draw.line([cx - 70, cy - 70, cx + 70, cy + 70], fill=arm_col, width=8)
    draw.line([cx + 70, cy - 70, cx - 70, cy + 70], fill=arm_col, width=8)
    rotor_positions = [(cx - 70, cy - 70), (cx + 70, cy - 70), (cx - 70, cy + 70), (cx + 70, cy + 70)]
    for rx, ry in rotor_positions:
        draw.ellipse([rx - 32, ry - 32, rx + 32, ry + 32], fill=(255, 255, 255, 40), outline=(76, 245, 213, 200), width=3)
        draw.line([rx - 28, ry, rx + 28, ry], fill=(200, 200, 200, 180), width=3)
        draw.line([rx, ry - 28, rx, ry + 28], fill=(200, 200, 200, 180), width=3)
        draw.ellipse([rx - 8, ry - 8, rx + 8, ry + 8], fill=(20, 20, 20, 255))
    draw.ellipse([cx - 35, cy - 35, cx + 35, cy + 35], fill=(20, 26, 33, 255), outline=(76, 245, 213, 255), width=3)
    draw.ellipse([cx - 18, cy - 18, cx + 18, cy + 18], fill=(13, 17, 23, 255), outline=(255, 224, 109, 255), width=2)
    draw.ellipse([cx - 10, cy - 10, cx + 10, cy + 10], fill=(76, 245, 213, 255))
    draw.ellipse([cx - 74, cy - 74, cx - 66, cy - 66], fill=(0, 255, 100, 255))
    draw.ellipse([cx + 66, cy - 74, cx + 74, cy - 66], fill=(255, 50, 50, 255))
    img.save("godot/assets/generated/drone_scout.png")
    img.save("godot/assets/drone_scout.png")
    print("Created drone_scout.png")

# 2. Vintage Boombox Prop
def create_boombox():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 140
    draw.ellipse([cx - 110, cy - 90, cx + 110, cy + 90], fill=(255, 224, 109, 15), outline=(255, 224, 109, 60), width=3)
    draw.ellipse([cx - 95, cy - 75, cx + 95, cy + 75], fill=(255, 224, 109, 25), outline=(76, 245, 213, 120), width=2)
    draw.rectangle([cx - 85, cy - 40, cx + 85, cy + 45], fill=(30, 36, 44, 255), outline=(150, 160, 170, 255), width=4)
    draw.rectangle([cx - 50, cy - 62, cx + 50, cy - 40], fill=(0, 0, 0, 0), outline=(150, 160, 170, 255), width=5)
    for spk_x in [cx - 46, cx + 46]:
        draw.ellipse([spk_x - 30, cy - 25, spk_x + 30, cy + 35], fill=(15, 18, 22, 255), outline=(76, 245, 213, 255), width=3)
        draw.ellipse([spk_x - 20, cy - 15, spk_x + 20, cy + 25], fill=(45, 55, 68, 255), outline=(255, 59, 119, 255), width=2)
        draw.ellipse([spk_x - 8, cy - 3, spk_x + 8, cy + 13], fill=(255, 224, 109, 255))
    draw.rectangle([cx - 12, cy - 35, cx + 12, cy + 35], fill=(18, 22, 28, 255), outline=(60, 70, 80, 255), width=2)
    draw.rectangle([cx - 10, cy - 30, cx + 10, cy - 10], fill=(255, 59, 119, 200))
    draw.rectangle([cx - 10, cy - 5, cx + 10, cy + 20], fill=(40, 48, 56, 255))
    img.save("godot/assets/generated/boombox_prop.png")
    img.save("godot/assets/boombox_prop.png")
    print("Created boombox_prop.png")

# 3. Skate Dash Trail
def create_skate_trail():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    for y in range(40, 220, 25):
        alpha = int(255 * (1.0 - (y / 256.0)))
        draw.line([(20, y), (460, y)], fill=(76, 245, 213, alpha), width=6)
        draw.line([(60, y + 6), (420, y + 6)], fill=(255, 59, 119, alpha // 2), width=3)
    for _ in range(35):
        sx = random.randint(30, 480)
        sy = random.randint(20, 230)
        sr = random.randint(2, 6)
        col = random.choice([(255, 224, 109, 255), (76, 245, 213, 255), (255, 255, 255, 255)])
        draw.ellipse([sx - sr, sy - sr, sx + sr, sy + sr], fill=col)
    img.save("godot/assets/generated/skate_trail.png")
    img.save("godot/assets/skate_trail.png")
    print("Created skate_trail.png")

# 4. Subway Entrance
def create_subway_entrance():
    size = (512, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 256
    draw.rectangle([cx - 180, cy - 140, cx + 180, cy + 160], fill=(25, 30, 38, 255), outline=(100, 110, 120, 255), width=6)
    for i in range(8):
        sy = cy - 100 + i * 28
        sw = 320 - i * 26
        draw.rectangle([cx - sw // 2, sy, cx + sw // 2, sy + 20], fill=(12 + i * 6, 15 + i * 7, 20 + i * 8, 255), outline=(60, 70, 80, 255), width=2)
    draw.line([cx - 170, cy - 160, cx - 170, cy + 150], fill=(180, 190, 200, 255), width=8)
    draw.line([cx + 170, cy - 160, cx + 170, cy + 150], fill=(180, 190, 200, 255), width=8)
    draw.line([cx - 180, cy - 140, cx + 180, cy - 140], fill=(180, 190, 200, 255), width=8)
    draw.rectangle([cx - 160, cy - 220, cx + 160, cy - 150], fill=(10, 15, 20, 255), outline=(76, 245, 213, 255), width=4)
    draw.text((cx - 110, cy - 206), "METRO 404", fill=(76, 245, 213, 255))
    for lx in [cx - 170, cx + 170]:
        draw.ellipse([lx - 22, cy - 240, lx + 22, cy - 196], fill=(255, 224, 109, 255), outline=(255, 255, 255, 255), width=3)
    img.save("godot/assets/generated/subway_entrance_metro.png")
    img.save("godot/assets/subway_entrance_metro.png")
    print("Created subway_entrance_metro.png")

# 5. Smoke Grenade
def create_smoke_assets():
    img1 = Image.new("RGBA", (256, 256), (0, 0, 0, 0))
    draw1 = ImageDraw.Draw(img1)
    cx, cy = 128, 128
    draw1.rectangle([cx - 30, cy - 45, cx + 30, cy + 50], fill=(40, 48, 56, 255), outline=(76, 245, 213, 255), width=4)
    draw1.rectangle([cx - 30, cy - 15, cx + 30, cy + 15], fill=(255, 59, 119, 255))
    draw1.rectangle([cx - 12, cy - 65, cx + 12, cy - 45], fill=(180, 190, 200, 255), outline=(20, 20, 20, 255), width=2)
    draw1.ellipse([cx - 24, cy - 80, cx - 8, cy - 64], fill=(0, 0, 0, 0), outline=(255, 224, 109, 255), width=4)
    img1.save("godot/assets/generated/smoke_grenade_prop.png")
    img1.save("godot/assets/smoke_grenade_prop.png")
    
    img2 = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
    draw2 = ImageDraw.Draw(img2)
    for _ in range(60):
        px = random.randint(120, 392)
        py = random.randint(120, 392)
        pr = random.randint(50, 110)
        col = random.choice([(76, 245, 213, 70), (255, 59, 119, 60), (192, 132, 252, 65), (255, 255, 255, 50)])
        draw2.ellipse([px - pr, py - pr, px + pr, py + pr], fill=col)
    img2 = img2.filter(ImageFilter.GaussianBlur(radius=12))
    img2.save("godot/assets/generated/smoke_particle_cloud.png")
    img2.save("godot/assets/smoke_particle_cloud.png")
    print("Created smoke_grenade_prop.png and smoke_particle_cloud.png")

# 6. Flow Rhythm Meter Gauge
def create_flow_rhythm_gauge():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 128
    draw.arc([cx - 180, cy - 70, cx + 180, cy + 70], 180, 0, fill=(40, 48, 56, 255), width=24)
    # Green Sweet Spot Zone
    draw.arc([cx - 180, cy - 70, cx + 180, cy + 70], 230, 310, fill=(76, 245, 213, 255), width=24)
    # Gold Starburst Center
    draw.text((cx - 80, cy - 20), "PERFECT FLOW", fill=(255, 224, 109, 255))
    img.save("godot/assets/generated/flow_rhythm_gauge.png")
    img.save("godot/assets/flow_rhythm_gauge.png")
    print("Created flow_rhythm_gauge.png")

# 7. Police Roadblock
def create_police_roadblock():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    for lx in [60, 452]:
        draw.polygon([(lx - 25, 220), (lx + 25, 220), (lx + 10, 40), (lx - 10, 40)], fill=(40, 48, 56, 255), outline=(15, 18, 22, 255), width=3)
    draw.rectangle([40, 60, 472, 140], fill=(240, 200, 30, 255), outline=(20, 20, 20, 255), width=4)
    for sx in range(40, 460, 40):
        draw.polygon([(sx, 60), (sx + 25, 60), (sx + 5, 140), (sx - 15, 140)], fill=(20, 25, 30, 255))
    draw.rectangle([100, 25, 136, 60], fill=(255, 140, 0, 255), outline=(255, 255, 255, 255), width=2)
    draw.rectangle([376, 25, 412, 60], fill=(0, 140, 255, 255), outline=(255, 255, 255, 255), width=2)
    img.save("godot/assets/generated/police_roadblock.png")
    img.save("godot/assets/police_roadblock.png")
    print("Created police_roadblock.png")

# 8. Black Market Van
def create_black_market_van():
    size = (512, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 256
    draw.ellipse([cx - 200, cy + 120, cx + 200, cy + 200], fill=(255, 59, 119, 100))
    draw.rectangle([cx - 160, cy - 120, cx + 160, cy + 130], fill=(18, 22, 28, 255), outline=(76, 245, 213, 255), width=5)
    draw.rectangle([cx - 130, cy - 90, cx + 130, cy - 45], fill=(10, 12, 16, 255), outline=(255, 224, 109, 255), width=3)
    draw.text((cx - 95, cy - 76), "BLACK MARKET 404", fill=(255, 224, 109, 255))
    img.save("godot/assets/generated/black_market_van.png")
    img.save("godot/assets/black_market_van.png")
    print("Created black_market_van.png")

# 9. 6 Tag Styles
def create_tag_style_sprites():
    styles = [
        ("bubble", (76, 245, 213, 255), "BUBBLE 404"),
        ("blockbuster", (255, 224, 109, 255), "BLOCKBUSTER"),
        ("wildstyle", (255, 59, 119, 255), "WILDSTYLE 3D"),
        ("stencil", (192, 132, 252, 255), "STENCIL CREW"),
        ("throwup", (89, 183, 255, 255), "THROW-UP 404"),
        ("chrome", (230, 240, 250, 255), "CHROME BURNER")
    ]
    for name, col, text in styles:
        img = Image.new("RGBA", (512, 256), (0, 0, 0, 0))
        draw = ImageDraw.Draw(img)
        cx, cy = 256, 128
        draw.ellipse([cx - 210, cy - 80, cx + 210, cy + 80], fill=(col[0], col[1], col[2], 50))
        for ox in [-4, 0, 4]:
            for oy in [-4, 0, 4]:
                draw.text((cx - 150 + ox, cy - 25 + oy), text, fill=(10, 12, 16, 255))
        draw.text((cx - 150, cy - 25), text, fill=col)
        img.save(f"godot/assets/generated/tag_style_{name}.png")
        img.save(f"godot/assets/tag_style_{name}.png")

# 10. Audio Spectrum HUD
def create_audio_spectrum_hud():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rectangle([20, 20, 492, 236], fill=(15, 18, 24, 230), outline=(76, 245, 213, 255), width=4)
    draw.text((50, 48), "📻 STREET RADIO 99.4 FM   |   TRACK 03: WILDSTYLE BASS", fill=(255, 224, 109, 255))
    bar_heights = [45, 80, 110, 65, 130, 95, 140, 75, 105, 125, 60, 90]
    for i, h in enumerate(bar_heights):
        bx = 50 + i * 36
        by = 210
        for seg in range(0, h, 10):
            seg_col = (76, 245, 213, 255) if seg < 60 else (255, 224, 109, 255) if seg < 100 else (255, 59, 119, 255)
            draw.rectangle([bx, by - seg - 8, bx + 26, by - seg], fill=seg_col)
    img.save("godot/assets/generated/audio_spectrum_hud.png")
    img.save("godot/assets/audio_spectrum_hud.png")
    print("Created audio_spectrum_hud.png")

# 11. Civilian Crowd
def create_civilian_crowd():
    size = (1024, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cell_w = 256
    civilians = [
        ("Skater Kid", (255, 59, 119, 255), (76, 245, 213, 255)),
        ("Hoodie Fan", (255, 224, 109, 255), (40, 48, 56, 255)),
        ("City Reporter", (192, 132, 252, 255), (200, 200, 200, 255)),
        ("Street DJ", (89, 183, 255, 255), (255, 140, 0, 255))
    ]
    for i, (name, shirt_col, accent_col) in enumerate(civilians):
        cx = i * cell_w + 128
        cy = 140
        draw.ellipse([cx - 30, cy + 50, cx + 30, cy + 68], fill=(0, 0, 0, 120))
        draw.rectangle([cx - 24, cy - 25, cx + 24, cy + 18], fill=shirt_col, outline=(15, 18, 22, 255), width=3)
        draw.ellipse([cx - 18, cy - 58, cx + 18, cy - 24], fill=(220, 170, 140, 255), outline=(15, 18, 22, 255), width=2)
    img.save("godot/assets/generated/civilian_crowd.png")
    img.save("godot/assets/civilian_crowd.png")
    print("Created civilian_crowd.png")

# 12. Crew Classes Badges (Scout, Bomber, Enforcer)
def create_crew_classes_badges():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    badges = [
        ("SCOUT", (76, 245, 213, 255), "⚡"),
        ("BOMBER", (255, 59, 119, 255), "🎨"),
        ("ENFORCER", (255, 224, 109, 255), "🛡️")
    ]
    for i, (name, col, symbol) in enumerate(badges):
        cx = i * 160 + 96
        cy = 128
        draw.ellipse([cx - 45, cy - 45, cx + 45, cy + 45], fill=(30, 36, 44, 255), outline=col, width=4)
        draw.text((cx - 30, cy - 15), name, fill=col)
    img.save("godot/assets/generated/crew_classes_badges.png")
    img.save("godot/assets/crew_classes_badges.png")
    print("Created crew_classes_badges.png")

# 13. Heist Cargo Duffel Crate
def create_heist_cargo_crate():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    # Crate Duffel Bag
    draw.rectangle([cx - 50, cy - 40, cx + 50, cy + 40], fill=(200, 140, 30, 255), outline=(20, 20, 20, 255), width=4)
    draw.line([cx - 50, cy - 10, cx + 50, cy - 10], fill=(40, 40, 40, 255), width=4)
    draw.text((cx - 35, cy - 5), "HAZARD 404", fill=(255, 255, 255, 255))
    img.save("godot/assets/generated/heist_cargo_crate.png")
    img.save("godot/assets/heist_cargo_crate.png")
    print("Created heist_cargo_crate.png")

# 14. Rooftop Billboard Mega-Mural
def create_billboard_mural():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rectangle([20, 20, 492, 236], fill=(20, 25, 32, 255), outline=(255, 224, 109, 255), width=5)
    draw.ellipse([80, 40, 432, 216], fill=(76, 245, 213, 80))
    draw.text((120, 100), "MEGA MURAL KING", fill=(255, 59, 119, 255))
    img.save("godot/assets/generated/billboard_mega_mural.png")
    img.save("godot/assets/billboard_mega_mural.png")
    print("Created billboard_mega_mural.png")

if __name__ == "__main__":
    make_dirs()
    create_drone_scout()
    create_boombox()
    create_skate_trail()
    create_subway_entrance()
    create_smoke_assets()
    create_flow_rhythm_gauge()
    create_police_roadblock()
    create_black_market_van()
    create_tag_style_sprites()
    create_audio_spectrum_hud()
    create_civilian_crowd()
    create_crew_classes_badges()
    create_heist_cargo_crate()
    create_billboard_mural()
    print("All Batch 3 graphics generated successfully!")
