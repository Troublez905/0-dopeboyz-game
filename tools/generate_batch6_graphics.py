import math
import os
import random
from PIL import Image, ImageDraw, ImageFont, ImageFilter

def make_dirs():
    os.makedirs("godot/assets/generated", exist_ok=True)
    os.makedirs("godot/assets", exist_ok=True)

# 1. Lowrider Hydraulic Car (512x256)
def create_lowrider_car():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 128
    # Shadow
    draw.ellipse([cx - 210, cy - 80, cx + 210, cy + 90], fill=(0, 0, 0, 110))
    # Car body (Candy Magenta)
    draw.rounded_rectangle([cx - 200, cy - 65, cx + 200, cy + 65], radius=22, fill=(220, 20, 95, 255), outline=(255, 120, 180, 255), width=4)
    # Hood & trunk lines
    draw.line([cx - 90, cy - 60, cx - 90, cy + 60], fill=(160, 10, 65, 255), width=3)
    draw.line([cx + 90, cy - 60, cx + 90, cy + 60], fill=(160, 10, 65, 255), width=3)
    # Roof / Cabin
    draw.rounded_rectangle([cx - 75, cy - 45, cx + 75, cy + 45], radius=14, fill=(40, 48, 60, 255), outline=(76, 245, 213, 255), width=3)
    # Windshield & rear glass
    draw.rounded_rectangle([cx - 65, cy - 38, cx - 25, cy + 38], radius=6, fill=(120, 210, 240, 200))
    draw.rounded_rectangle([cx + 25, cy - 38, cx + 65, cy + 38], radius=6, fill=(120, 210, 240, 200))
    # Chrome bumpers
    draw.rounded_rectangle([cx - 208, cy - 50, cx - 198, cy + 50], radius=5, fill=(240, 245, 255, 255))
    draw.rounded_rectangle([cx + 198, cy - 50, cx + 208, cy + 50], radius=5, fill=(240, 245, 255, 255))
    # Chrome spoke wheels
    for wx, wy in [(cx - 130, cy - 72), (cx + 130, cy - 72), (cx - 130, cy + 72), (cx + 130, cy + 72)]:
        draw.rounded_rectangle([wx - 25, wy - 10, wx + 25, wy + 10], radius=5, fill=(20, 20, 24, 255))
        draw.ellipse([wx - 10, wy - 6, wx + 10, wy + 6], fill=(240, 240, 250, 255))
    # Turquoise pinstripe detail
    draw.line([cx - 180, cy - 25, cx + 180, cy - 25], fill=(76, 245, 213, 230), width=2)
    draw.line([cx - 180, cy + 25, cx + 180, cy + 25], fill=(76, 245, 213, 230), width=2)
    # Headlights & Taillights
    draw.ellipse([cx - 202, cy - 40, cx - 192, cy - 20], fill=(255, 250, 180, 255))
    draw.ellipse([cx - 202, cy + 20, cx - 192, cy + 40], fill=(255, 250, 180, 255))
    draw.ellipse([cx + 192, cy - 40, cx + 202, cy - 20], fill=(255, 40, 40, 255))
    draw.ellipse([cx + 192, cy + 20, cx + 202, cy + 40], fill=(255, 40, 40, 255))
    img.save("godot/assets/generated/lowrider_hydraulic_car.png")
    img.save("godot/assets/lowrider_hydraulic_car.png")
    print("Created lowrider_hydraulic_car.png")

# 2. Neon Underglow FX (256x256)
def create_neon_underglow():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    for r in range(110, 10, -5):
        alpha = int(140 * (1.0 - (r / 110.0)))
        draw.ellipse([cx - r * 1.1, cy - r * 0.7, cx + r * 1.1, cy + r * 0.7], fill=(76, 245, 213, alpha))
    for r in range(60, 5, -5):
        alpha = int(180 * (1.0 - (r / 60.0)))
        draw.ellipse([cx - r * 1.0, cy - r * 0.6, cx + r * 1.0, cy + r * 0.6], fill=(255, 59, 119, alpha))
    img.save("godot/assets/generated/neon_underglow_fx.png")
    img.save("godot/assets/neon_underglow_fx.png")
    print("Created neon_underglow_fx.png")

# 3. Police Spike Strip (256x128)
def create_spike_strip():
    size = (256, 128)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Steel frame
    draw.rectangle([10, 45, 246, 83], fill=(35, 40, 48, 255), outline=(100, 110, 120, 255), width=3)
    # Hazard stripes
    for i in range(15, 240, 22):
        draw.polygon([(i, 47), (i + 12, 47), (i + 6, 81), (i - 6, 81)], fill=(255, 220, 40, 255))
    # Sharp hollow puncture spikes
    for sx in range(20, 240, 14):
        draw.polygon([(sx - 5, 45), (sx + 5, 45), (sx, 16)], fill=(220, 230, 240, 255), outline=(80, 90, 100, 255), width=1)
        draw.polygon([(sx - 5, 83), (sx + 5, 83), (sx, 112)], fill=(220, 230, 240, 255), outline=(80, 90, 100, 255), width=1)
    img.save("godot/assets/generated/police_spike_strip.png")
    img.save("godot/assets/police_spike_strip.png")
    print("Created police_spike_strip.png")

# 4. Surveillance Blimp Airship (512x256)
def create_surveillance_blimp():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 120
    # Shadow
    draw.ellipse([cx - 210, cy + 20, cx + 210, cy + 120], fill=(0, 0, 0, 80))
    # Main dirigible envelope
    draw.ellipse([cx - 230, cy - 80, cx + 230, cy + 80], fill=(30, 38, 52, 255), outline=(76, 110, 160, 255), width=5)
    # Stabilizer tail fins
    draw.polygon([(cx - 240, cy - 30), (cx - 180, cy - 75), (cx - 180, cy - 20)], fill=(20, 26, 36, 255), outline=(76, 245, 213, 255), width=2)
    draw.polygon([(cx - 240, cy + 30), (cx - 180, cy + 75), (cx - 180, cy + 20)], fill=(20, 26, 36, 255), outline=(76, 245, 213, 255), width=2)
    # Side digital billboard screen
    draw.rounded_rectangle([cx - 110, cy - 35, cx + 110, cy + 35], radius=6, fill=(10, 14, 20, 255), outline=(76, 245, 213, 255), width=2)
    draw.text((cx - 85, cy - 12), "★ POLICE AIR 404 ★", fill=(76, 245, 213, 255))
    # Gondola cabin
    draw.rounded_rectangle([cx - 50, cy + 65, cx + 50, cy + 95], radius=6, fill=(200, 210, 225, 255), outline=(30, 40, 50, 255), width=3)
    # Halogen searchlight beam emitter
    draw.ellipse([cx + 35, cy + 75, cx + 55, cy + 95], fill=(255, 250, 180, 255))
    img.save("godot/assets/generated/surveillance_blimp_airship.png")
    img.save("godot/assets/surveillance_blimp_airship.png")
    print("Created surveillance_blimp_airship.png")

# 5. Rival Turf Banner (128x256)
def create_rival_banner():
    size = (128, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Top wooden / brass pole
    draw.rounded_rectangle([10, 12, 118, 26], radius=4, fill=(180, 130, 40, 255), outline=(60, 40, 10, 255), width=2)
    # Hanging cords
    draw.line([20, 24, 28, 38], fill=(240, 240, 240, 255), width=2)
    draw.line([108, 24, 100, 38], fill=(240, 240, 240, 255), width=2)
    # Ripped fabric banner
    banner_pts = [(20, 35), (108, 35), (108, 210), (88, 235), (64, 215), (40, 240), (20, 215)]
    draw.polygon(banner_pts, fill=(190, 25, 45, 255), outline=(240, 80, 60, 255), width=3)
    # Rival gang skull insignia
    cx, cy = 64, 110
    draw.ellipse([cx - 24, cy - 24, cx + 24, cy + 18], fill=(240, 240, 240, 255))
    draw.rectangle([cx - 16, cy + 12, cx + 16, cy + 28], fill=(240, 240, 240, 255))
    draw.ellipse([cx - 15, cy - 6, cx - 5, cy + 6], fill=(190, 25, 45, 255))
    draw.ellipse([cx + 5, cy - 6, cx + 15, cy + 6], fill=(190, 25, 45, 255))
    draw.text((cx - 28, cy + 45), "RIVALS", fill=(255, 220, 60, 255))
    img.save("godot/assets/generated/rival_turf_banner.png")
    img.save("godot/assets/rival_turf_banner.png")
    print("Created rival_turf_banner.png")

# 6. Vending Machine Kiosk (128x256)
def create_vending_machine():
    size = (128, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Outer cabinet
    draw.rounded_rectangle([12, 14, 116, 244], radius=10, fill=(24, 30, 42, 255), outline=(76, 245, 213, 255), width=4)
    # Marquee top
    draw.rounded_rectangle([20, 22, 108, 52], radius=4, fill=(15, 20, 28, 255), outline=(255, 59, 119, 255), width=2)
    draw.text((26, 30), "NEON SODA", fill=(255, 59, 119, 255))
    # Glass showcase
    draw.rectangle([20, 58, 108, 160], fill=(10, 16, 26, 255), outline=(60, 80, 100, 255), width=2)
    # Soda cans display rows
    for row in [72, 104, 136]:
        for col in [30, 52, 74, 96]:
            color = random.choice([(76, 245, 213), (255, 59, 119), (255, 224, 109), (118, 255, 3)])
            draw.rectangle([col - 7, row - 10, col + 7, row + 10], fill=color)
            draw.line([col - 7, row + 12, col + 7, row + 12], fill=(200, 200, 200, 255), width=1)
    # Coin / Card slot & change return
    draw.rectangle([22, 172, 50, 182], fill=(60, 70, 85, 255))
    draw.ellipse([70, 175, 82, 187], fill=(76, 245, 213, 255))
    # Delivery dispenser door
    draw.rounded_rectangle([26, 196, 102, 232], radius=4, fill=(12, 15, 20, 255), outline=(50, 65, 80, 255), width=2)
    img.save("godot/assets/generated/vending_machine_kiosk.png")
    img.save("godot/assets/vending_machine_kiosk.png")
    print("Created vending_machine_kiosk.png")

# 7. Subway Grate Updraft Vent (256x256)
def create_subway_grate():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Iron street frame
    draw.rounded_rectangle([25, 25, 231, 231], radius=8, fill=(35, 40, 48, 255), outline=(70, 80, 95, 255), width=5)
    # Inner dark pit
    draw.rectangle([35, 35, 221, 221], fill=(12, 15, 20, 255))
    # Grate iron slots
    for gy in range(45, 215, 14):
        draw.line([38, gy, 218, gy], fill=(75, 85, 100, 255), width=6)
    # Steam clouds rising
    for _ in range(25):
        sx = random.randint(50, 206)
        sy = random.randint(40, 210)
        sr = random.randint(15, 45)
        draw.ellipse([sx - sr, sy - sr, sx + sr, sy + sr], fill=(220, 240, 255, random.randint(30, 80)))
    img.save("godot/assets/generated/subway_grate_updraft.png")
    img.save("godot/assets/subway_grate_updraft.png")
    print("Created subway_grate_updraft.png")

# 8. Riot Shield Officer (256x256)
def create_riot_shield_officer():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 140
    # SWAT Officer helmet & shoulders
    draw.ellipse([cx - 40, cy - 25, cx + 40, cy + 30], fill=(25, 30, 40, 255))
    draw.ellipse([cx - 20, cy - 20, cx + 20, cy + 18], fill=(15, 18, 24, 255), outline=(50, 65, 80, 255), width=3)
    # Visor
    draw.arc([cx - 15, cy - 16, cx + 15, cy + 14], -0.2, 3.3, fill=(120, 190, 240, 255), width=4)
    # Heavy rectangular polycarbonate riot shield in front
    draw.rounded_rectangle([cx - 65, cy - 75, cx + 65, cy - 35], radius=6, fill=(160, 210, 245, 160), outline=(220, 235, 250, 240), width=4)
    draw.text((cx - 26, cy - 62), "POLICE", fill=(20, 30, 45, 230))
    # Paint splatter on shield
    draw.ellipse([cx + 10, cy - 65, cx + 35, cy - 45], fill=(255, 59, 119, 210))
    img.save("godot/assets/generated/riot_shield_officer.png")
    img.save("godot/assets/riot_shield_officer.png")
    print("Created riot_shield_officer.png")

# 9. Soundquake Bass Blast (256x256)
def create_soundquake_blast():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    for r, col, w in [(115, (255, 224, 109, 120), 3), (95, (76, 245, 213, 160), 4), (70, (255, 59, 119, 200), 5), (45, (255, 255, 255, 240), 6)]:
        draw.arc([cx - r, cy - r, cx + r, cy + r], 0, math.tau, fill=col, width=w)
    # Sonic spikes
    for i in range(16):
        ang = (i / 16.0) * math.tau
        x1 = cx + math.cos(ang) * 40
        y1 = cy + math.sin(ang) * 40
        x2 = cx + math.cos(ang) * 115
        y2 = cy + math.sin(ang) * 115
        draw.line([x1, y1, x2, y2], fill=(255, 224, 109, 180), width=3)
    img.save("godot/assets/generated/soundquake_bass_blast.png")
    img.save("godot/assets/soundquake_bass_blast.png")
    print("Created soundquake_bass_blast.png")

# 10. Blackbook Sticker Album (256x256)
def create_blackbook():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Leather cover
    draw.rounded_rectangle([15, 25, 241, 231], radius=12, fill=(28, 22, 18, 255), outline=(140, 100, 60, 255), width=4)
    # Open paper pages
    draw.rounded_rectangle([25, 35, 124, 221], radius=4, fill=(245, 238, 220, 255))
    draw.rounded_rectangle([132, 35, 231, 221], radius=4, fill=(240, 232, 215, 255))
    # Binding spine
    draw.rectangle([124, 30, 132, 226], fill=(50, 40, 30, 255))
    # Stickers on left page
    draw.rounded_rectangle([35, 45, 75, 80], radius=4, fill=(76, 245, 213, 255), outline=(20, 20, 20, 255), width=2)
    draw.text((42, 55), "404", fill=(10, 10, 10, 255))
    draw.ellipse([80, 50, 115, 85], fill=(255, 59, 119, 255))
    draw.text((85, 62), "BOYZ", fill=(255, 255, 255, 255))
    # Sketches on right page
    draw.text((142, 50), "WILDSTYLE", fill=(40, 40, 40, 255))
    draw.line([145, 90, 215, 90], fill=(220, 30, 70, 255), width=5)
    draw.line([150, 110, 220, 110], fill=(20, 180, 220, 255), width=5)
    img.save("godot/assets/generated/blackbook_sticker_album.png")
    img.save("godot/assets/blackbook_sticker_album.png")
    print("Created blackbook_sticker_album.png")

# 11. Rooftop Lounge & Pool Table (256x256)
def create_pool_table():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Mahogany wood frame
    draw.rounded_rectangle([25, 45, 231, 211], radius=14, fill=(65, 35, 20, 255), outline=(130, 85, 50, 255), width=5)
    # Neon green felt
    draw.rounded_rectangle([42, 62, 214, 194], radius=6, fill=(16, 185, 90, 255), outline=(10, 120, 60, 255), width=2)
    # 6 Pockets
    for px, py in [(42, 62), (128, 62), (214, 62), (42, 194), (128, 194), (214, 194)]:
        draw.ellipse([px - 9, py - 9, px + 9, py + 9], fill=(15, 15, 18, 255), outline=(180, 140, 50, 255), width=2)
    # Triangle rack of balls
    for bx, by in [(160, 128), (175, 120), (175, 136), (190, 112), (190, 128), (190, 144)]:
        draw.ellipse([bx - 5, by - 5, bx + 5, by + 5], fill=random.choice([(255, 220, 40), (220, 30, 40), (30, 120, 230), (180, 40, 200)]))
    # Cue ball
    draw.ellipse([80 - 6, 128 - 6, 80 + 6, 128 + 6], fill=(250, 250, 255, 255))
    img.save("godot/assets/generated/rooftop_lounge_props.png")
    img.save("godot/assets/rooftop_lounge_props.png")
    print("Created rooftop_lounge_props.png")

# 12. CCTV Security Camera (128x128)
def create_cctv_camera():
    size = (128, 128)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 64, 64
    # Wall bracket
    draw.rectangle([10, 50, 30, 78], fill=(60, 70, 85, 255))
    draw.line([30, 64, 52, 64], fill=(120, 130, 145, 255), width=6)
    # Camera dome housing
    draw.ellipse([50, 36, 106, 92], fill=(235, 240, 245, 255), outline=(40, 50, 65, 255), width=3)
    # Smoked dark lens
    draw.ellipse([70, 50, 102, 78], fill=(18, 22, 30, 255))
    # Glowing red sensor LED
    draw.ellipse([82, 60, 90, 68], fill=(255, 30, 30, 255))
    img.save("godot/assets/generated/cctv_security_camera.png")
    img.save("godot/assets/cctv_security_camera.png")
    print("Created cctv_security_camera.png")

# 13. Paint Exhaust Plume (256x256)
def create_exhaust_plume():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 40
    for i in range(45):
        px = cx + random.randint(-40, 40)
        py = cy + random.randint(20, 200)
        pr = random.randint(10, 35)
        col = (76, 245, 213, random.randint(60, 180)) if i % 2 == 0 else (255, 59, 119, random.randint(60, 180))
        draw.ellipse([px - pr, py - pr, px + pr, py + pr], fill=col)
    img.save("godot/assets/generated/paint_exhaust_plume.png")
    img.save("godot/assets/paint_exhaust_plume.png")
    print("Created paint_exhaust_plume.png")

# 14. Metro Rail Tracks (256x256)
def create_metro_tracks():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Timber cross-ties
    for ty in range(20, 245, 28):
        draw.rectangle([30, ty, 226, ty + 16], fill=(55, 42, 32, 255), outline=(25, 18, 12, 255), width=2)
    # Outer steel rails
    draw.line([60, 0, 60, 256], fill=(190, 205, 220, 255), width=8)
    draw.line([196, 0, 196, 256], fill=(190, 205, 220, 255), width=8)
    # Electrified center third rail
    draw.line([128, 0, 128, 256], fill=(255, 224, 60, 255), width=6)
    # Electric sparks
    for _ in range(8):
        sy = random.randint(10, 246)
        draw.line([128, sy, 128 + random.randint(-15, 15), sy + random.randint(-10, 10)], fill=(120, 230, 255, 255), width=2)
    img.save("godot/assets/generated/metro_rail_tracks.png")
    img.save("godot/assets/metro_rail_tracks.png")
    print("Created metro_rail_tracks.png")

# 15. Market Vendor Stall (256x256)
def create_market_stall():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Striped awning canopy
    draw.polygon([(25, 45), (231, 45), (215, 110), (41, 110)], fill=(76, 245, 213, 255), outline=(20, 25, 30, 255), width=3)
    for sx in range(45, 215, 34):
        draw.polygon([(sx, 45), (sx + 17, 45), (sx + 10, 110), (sx - 7, 110)], fill=(255, 59, 119, 255))
    # Wooden counter
    draw.rectangle([35, 110, 221, 195], fill=(140, 95, 55, 255), outline=(60, 40, 20, 255), width=4)
    # Paint cans on counter
    for cx in range(55, 205, 30):
        draw.rectangle([cx - 8, 85, cx + 8, 110], fill=random.choice([(255, 224, 109), (76, 245, 213), (255, 59, 119)]))
    draw.text((70, 140), "BLACK MARKET", fill=(255, 255, 255, 255))
    img.save("godot/assets/generated/market_vendor_stall.png")
    img.save("godot/assets/market_vendor_stall.png")
    print("Created market_vendor_stall.png")

# 16. Chameleon Camo Cloak FX (256x256)
def create_camo_cloak():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Hexagonal digital camo pattern
    cx, cy = 128, 128
    for y in range(20, 240, 28):
        for x in range(20, 240, 32):
            draw.regular_polygon((x, y, 14), 6, fill=(76, 245, 213, random.randint(40, 130)), outline=(120, 255, 235, 160))
    draw.text((cx - 45, cy - 10), "[CAMO ACTIVE]", fill=(255, 255, 255, 200))
    img.save("godot/assets/generated/camo_stealth_cloak.png")
    img.save("godot/assets/camo_stealth_cloak.png")
    print("Created camo_stealth_cloak.png")

# 17. Micro-Drone Swarm (256x256)
def create_drone_swarm():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Trio of drones in triangle
    drones = [(128, 60), (70, 175), (186, 175)]
    for dx, dy in drones:
        draw.ellipse([dx - 22, dy - 22, dx + 22, dy + 22], fill=(25, 30, 40, 255), outline=(76, 245, 213, 255), width=2)
        draw.ellipse([dx - 8, dy - 8, dx + 8, dy + 8], fill=(76, 245, 213, 255))
        # Rotor circles
        for rx, ry in [(dx - 18, dy - 18), (dx + 18, dy - 18), (dx - 18, dy + 18), (dx + 18, dy + 18)]:
            draw.ellipse([rx - 6, ry - 6, rx + 6, ry + 6], fill=(120, 235, 255, 140))
    img.save("godot/assets/generated/micro_drone_swarm.png")
    img.save("godot/assets/micro_drone_swarm.png")
    print("Created micro_drone_swarm.png")

# 18. Alley Dumpster Prop (256x256)
def create_dumpster():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Olive-green metal container
    draw.rounded_rectangle([30, 75, 226, 215], radius=8, fill=(65, 85, 55, 255), outline=(30, 45, 25, 255), width=5)
    # Split plastic lids
    draw.rounded_rectangle([25, 55, 126, 80], radius=4, fill=(35, 38, 42, 255))
    draw.rounded_rectangle([130, 55, 231, 80], radius=4, fill=(35, 38, 42, 255))
    # Spray tags on front
    draw.text((45, 120), "404 CREW", fill=(76, 245, 213, 255))
    draw.text((50, 155), "TAG THE CITY", fill=(255, 59, 119, 255))
    # Caster wheels
    draw.rectangle([45, 215, 65, 235], fill=(20, 20, 24, 255))
    draw.rectangle([190, 215, 210, 235], fill=(20, 20, 24, 255))
    img.save("godot/assets/generated/alley_dumpster_prop.png")
    img.save("godot/assets/alley_dumpster_prop.png")
    print("Created alley_dumpster_prop.png")

# 19. Police Radio Jammer (256x256)
def create_radio_jammer():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    # Tripod steel legs
    draw.line([cx, cy, cx - 60, cy + 95], fill=(80, 90, 105, 255), width=6)
    draw.line([cx, cy, cx + 60, cy + 95], fill=(80, 90, 105, 255), width=6)
    draw.line([cx, cy, cx, cy + 105], fill=(80, 90, 105, 255), width=6)
    # Parabolic dish
    draw.ellipse([cx - 55, cy - 80, cx + 55, cy + 20], fill=(210, 220, 230, 255), outline=(50, 60, 75, 255), width=4)
    # Feed horn antenna
    draw.line([cx, cy - 30, cx, cy - 70], fill=(40, 50, 60, 255), width=5)
    # Red blinking beacon
    draw.ellipse([cx - 9, cy - 85, cx + 9, cy - 67], fill=(255, 30, 30, 255))
    # Jammer pulse rings
    for r in [65, 85, 105]:
        draw.arc([cx - r, cy - 76 - r, cx + r, cy - 76 + r], -1.0, 1.0, fill=(255, 60, 60, 160), width=2)
    img.save("godot/assets/generated/police_radio_jammer.png")
    img.save("godot/assets/police_radio_jammer.png")
    print("Created police_radio_jammer.png")

# 20. 24K Golden Mastery Can Trophy (256x256)
def create_golden_can():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 135
    # Golden celestial halo
    for r in range(95, 30, -6):
        draw.ellipse([cx - r, cy - r, cx + r, cy + r], fill=(255, 224, 109, int(90 * (1.0 - r / 95.0))))
    # Solid 24K gold aerosol cylinder
    draw.rounded_rectangle([cx - 40, cy - 55, cx + 40, cy + 75], radius=10, fill=(255, 215, 0, 255), outline=(255, 245, 160, 255), width=4)
    # Gold dome top & collar
    draw.ellipse([cx - 38, cy - 72, cx + 38, cy - 42], fill=(255, 230, 80, 255))
    # Diamond encrusted actuator cap
    draw.rectangle([cx - 15, cy - 92, cx + 15, cy - 68], fill=(230, 245, 255, 255), outline=(180, 220, 250, 255), width=2)
    # Sparkle rays
    for ang in [0.2, 1.1, 2.3, 3.7, 4.8, 5.7]:
        sx = cx + math.cos(ang) * 75
        sy = cy + math.sin(ang) * 75
        draw.line([sx - 8, sy, sx + 8, sy], fill=(255, 255, 255, 255), width=3)
        draw.line([sx, sy - 8, sx, sy + 8], fill=(255, 255, 255, 255), width=3)
    draw.text((cx - 22, cy), "24K", fill=(140, 90, 0, 255))
    img.save("godot/assets/generated/golden_mastery_can.png")
    img.save("godot/assets/golden_mastery_can.png")
    print("Created golden_mastery_can.png")

def main():
    make_dirs()
    create_lowrider_car()
    create_neon_underglow()
    create_spike_strip()
    create_surveillance_blimp()
    create_rival_banner()
    create_vending_machine()
    create_subway_grate()
    create_riot_shield_officer()
    create_soundquake_blast()
    create_blackbook()
    create_pool_table()
    create_cctv_camera()
    create_exhaust_plume()
    create_metro_tracks()
    create_market_stall()
    create_camo_cloak()
    create_drone_swarm()
    create_dumpster()
    create_radio_jammer()
    create_golden_can()
    print("ALL 20 BATCH 6 GRAPHICS SUCCESSFULLY CREATED!")

if __name__ == "__main__":
    main()
