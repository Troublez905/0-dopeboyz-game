import math
import os
import random
from PIL import Image, ImageDraw, ImageFont, ImageFilter

def make_dirs():
    os.makedirs("godot/assets/generated", exist_ok=True)
    os.makedirs("godot/assets", exist_ok=True)

# 1. Roller Queen Spritesheet / Sprite
def create_roller_queen():
    size = (1024, 1024)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # 4x4 Grid of animation frames
    cell_w, cell_h = 256, 256
    directions = ["Down", "Up", "Left", "Right"]
    
    for row in range(4):
        for col in range(4):
            cx = col * cell_w + 128
            cy = row * cell_h + 128
            bob = int(math.sin(col * 1.5) * 6)
            
            # Ground shadow
            draw.ellipse([cx - 35, cy + 45, cx + 35, cy + 65], fill=(0, 0, 0, 120))
            
            # Skates
            skate_col = (240, 200, 40, 255)
            draw.ellipse([cx - 22, cy + 40 + bob, cx - 10, cy + 55 + bob], fill=skate_col, outline=(30, 30, 30, 255), width=2)
            draw.ellipse([cx + 10, cy + 40 - bob, cx + 22, cy + 55 - bob], fill=skate_col, outline=(30, 30, 30, 255), width=2)
            
            # Shorts & Legs
            draw.rectangle([cx - 16, cy + 15, cx + 16, cy + 38], fill=(50, 90, 160, 255), outline=(20, 20, 20, 255), width=2)
            
            # Oversized Yellow Hoodie Body
            hoodie_col = (255, 215, 0, 255)
            draw.ellipse([cx - 30, cy - 25 + bob, cx + 30, cy + 22 + bob], fill=hoodie_col, outline=(30, 30, 30, 255), width=3)
            draw.rectangle([cx - 24, cy - 10 + bob, cx + 24, cy + 18 + bob], fill=hoodie_col)
            
            # Head / Hood / Headphones
            draw.ellipse([cx - 22, cy - 50 + bob, cx + 22, cy - 15 + bob], fill=hoodie_col, outline=(30, 30, 30, 255), width=3)
            # Headphone band
            draw.arc([cx - 24, cy - 55 + bob, cx + 24, cy - 25 + bob], 180, 0, fill=(255, 50, 100, 255), width=4)
            draw.ellipse([cx - 26, cy - 38 + bob, cx - 18, cy - 24 + bob], fill=(255, 50, 100, 255))
            draw.ellipse([cx + 18, cy - 38 + bob, cx + 26, cy - 24 + bob], fill=(255, 50, 100, 255))
            
            # Telescopic Paint Roller with Neon Magenta dripping paint
            pole_angle = -0.3 + col * 0.15
            px = cx + math.cos(pole_angle) * 65
            py = cy + math.sin(pole_angle) * 65 - 10 + bob
            draw.line([cx + 15, cy + bob, px, py], fill=(180, 180, 190, 255), width=4)
            # Roller head
            draw.rectangle([px - 8, py - 18, px + 8, py + 18], fill=(255, 40, 130, 255), outline=(20, 20, 20, 255), width=2)
            # Paint drips from roller
            draw.line([px - 4, py + 18, px - 4, py + 30], fill=(255, 40, 130, 220), width=3)
            draw.ellipse([px - 6, py + 28, px - 2, py + 34], fill=(255, 40, 130, 220))
            
    img.save("godot/assets/generated/roller_queen.png")
    img.save("godot/assets/roller_queen.png")
    print("Created roller_queen.png")

# 2. Heavy Enforcer Spritesheet
def create_heavy_enforcer():
    size = (1024, 1024)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cell_w, cell_h = 256, 256
    
    for row in range(4):
        for col in range(4):
            cx = col * cell_w + 128
            cy = row * cell_h + 128
            bob = int(math.sin(col * 1.5) * 4)
            
            # Heavy shadow
            draw.ellipse([cx - 45, cy + 50, cx + 45, cy + 72], fill=(0, 0, 0, 140))
            # Heavy Boots
            draw.ellipse([cx - 28, cy + 42, cx - 12, cy + 62], fill=(60, 40, 30, 255), outline=(20, 20, 20, 255), width=2)
            draw.ellipse([cx + 12, cy + 42, cx + 28, cy + 62], fill=(60, 40, 30, 255), outline=(20, 20, 20, 255), width=2)
            
            # Reinforced Overalls
            draw.rectangle([cx - 36, cy - 20 + bob, cx + 36, cy + 44 + bob], fill=(45, 55, 70, 255), outline=(20, 20, 20, 255), width=3)
            # Straps
            draw.line([cx - 22, cy - 35 + bob, cx - 22, cy + 5 + bob], fill=(200, 140, 40, 255), width=6)
            draw.line([cx + 22, cy - 35 + bob, cx + 22, cy + 5 + bob], fill=(200, 140, 40, 255), width=6)
            
            # Broad Shoulders & Muscular Arms
            draw.ellipse([cx - 48, cy - 32 + bob, cx - 28, cy + 5 + bob], fill=(210, 160, 130, 255), outline=(30, 30, 30, 255), width=2)
            draw.ellipse([cx + 28, cy - 32 + bob, cx + 48, cy + 5 + bob], fill=(210, 160, 130, 255), outline=(30, 30, 30, 255), width=2)
            
            # Head / Backward Cap
            draw.ellipse([cx - 20, cy - 58 + bob, cx + 20, cy - 26 + bob], fill=(210, 160, 130, 255), outline=(30, 30, 30, 255), width=2)
            draw.arc([cx - 24, cy - 64 + bob, cx + 24, cy - 38 + bob], 180, 0, fill=(180, 40, 40, 255), width=8)
            draw.rectangle([cx - 10, cy - 68 + bob, cx + 10, cy - 58 + bob], fill=(180, 40, 40, 255))
            
            # Converted Fire Extinguisher Spray Blaster
            draw.rectangle([cx + 22, cy - 10 + bob, cx + 44, cy + 36 + bob], fill=(220, 30, 30, 255), outline=(20, 20, 20, 255), width=2)
            draw.ellipse([cx + 22, cy - 16 + bob, cx + 44, cy - 4 + bob], fill=(220, 30, 30, 255))
            # Nozzle & Hose
            draw.line([cx + 33, cy - 14 + bob, cx + 15, cy - 35 + bob], fill=(40, 40, 40, 255), width=5)
            # Pressure Gauge
            draw.circle([cx + 33, cy + 10 + bob], 6, fill=(240, 240, 240, 255), outline=(20, 20, 20, 255))
            
    img.save("godot/assets/generated/heavy_enforcer.png")
    img.save("godot/assets/heavy_enforcer.png")
    print("Created heavy_enforcer.png")

# 3. Subway Station Entrance
def create_subway_entrance():
    size = (512, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # Concrete Sidewalk Foundation
    draw.rectangle([30, 30, 482, 482], fill=(55, 60, 65, 255), outline=(30, 30, 35, 255), width=4)
    
    # Stairwell Pit (Gradient downward into darkness)
    for i in range(12):
        y1 = 90 + i * 26
        y2 = y1 + 26
        shade = max(5, 45 - i * 4)
        draw.rectangle([70, y1, 442, y2], fill=(shade, shade + 3, shade + 6, 255))
        # Step edge
        draw.line([70, y1, 442, y1], fill=(80 - i * 4, 85 - i * 4, 90 - i * 4, 255), width=2)
        
    # Cast Iron Green Railings
    rail_col = (20, 80, 50, 255)
    rail_thick = 6
    draw.rectangle([60, 80, 452, 410], outline=rail_col, width=rail_thick)
    # Railing bars
    for x in range(80, 440, 24):
        draw.line([x, 80, x, 410], fill=rail_col, width=3)
        
    # Subway Globe Lamps (Glowing spherical green entrance lights)
    for lx in [60, 452]:
        # Light glow aura
        draw.ellipse([lx - 25, 75 - 25, lx + 25, 75 + 25], fill=(80, 255, 160, 80))
        # Lamp globe
        draw.ellipse([lx - 15, 75 - 15, lx + 15, 75 + 15], fill=(120, 255, 180, 255), outline=(10, 50, 30, 255), width=3)
        draw.ellipse([lx - 6, 75 - 10, lx + 2, 75 - 3], fill=(255, 255, 255, 220))
        
    # Subway Name Header Banner ("404 SUBWAY - DOWNTOWN")
    draw.rectangle([110, 50, 402, 85], fill=(20, 25, 30, 255), outline=rail_col, width=3)
    # Green subway circle line badge
    draw.ellipse([120, 56, 146, 80], fill=(0, 160, 80, 255), outline=(255, 255, 255, 255), width=2)
    
    img.save("godot/assets/generated/subway_entrance.png")
    img.save("godot/assets/subway_entrance.png")
    print("Created subway_entrance.png")

# 4. Burst Fire Hydrant Paint Geyser
def create_hydrant_geyser():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # Ground water/paint pool ripples
    for r in [90, 70, 50, 35]:
        draw.ellipse([128 - r, 128 - r * 0.55 + 20, 128 + r, 128 + r * 0.55 + 20], fill=(76, 245, 213, int(40 - r * 0.3)), outline=(76, 245, 213, 140), width=2)
        
    # Paint geyser spray arcs
    for i in range(16):
        angle = -math.pi * 0.5 + (random.random() - 0.5) * 1.2
        speed = random.uniform(40, 100)
        ex = 128 + math.cos(angle) * speed
        ey = 120 + math.sin(angle) * speed
        col = (76, 245, 213, 220) if i % 2 == 0 else (255, 224, 109, 220)
        draw.line([128, 120, ex, ey], fill=col, width=random.randint(2, 5))
        draw.circle([ex, ey], random.randint(3, 7), fill=col)
        
    # Cast Iron Red Fire Hydrant (Top-down view)
    draw.circle([128, 128], 26, fill=(210, 35, 45, 255), outline=(30, 30, 30, 255), width=3)
    # Hydrant Top Nut
    draw.rectangle([123, 123, 133, 133], fill=(240, 210, 40, 255), outline=(30, 30, 30, 255), width=2)
    # Side Valves
    draw.rectangle([98, 124, 108, 132], fill=(180, 25, 35, 255), outline=(30, 30, 30, 255), width=2)
    draw.rectangle([148, 124, 158, 132], fill=(180, 25, 35, 255), outline=(30, 30, 30, 255), width=2)
    draw.rectangle([124, 148, 132, 158], fill=(180, 25, 35, 255), outline=(30, 30, 30, 255), width=2)
    
    img.save("godot/assets/generated/hydrant_geyser.png")
    img.save("godot/assets/hydrant_geyser.png")
    print("Created hydrant_geyser.png")

# 5. Streetlight Cone Lightmap Overlay
def create_streetlight_cone():
    size = (512, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # Smooth radial glow from center
    cx, cy = 256, 256
    max_r = 240
    for r in range(max_r, 0, -2):
        alpha = int(pow(1.0 - (r / max_r), 1.6) * 190)
        # Warm amber / tungsten light
        draw.ellipse([cx - r, cy - r, cx + r, cy + r], fill=(255, 230, 160, alpha))
        
    # Hot center core
    for r in range(45, 0, -2):
        alpha = int(pow(1.0 - (r / 45.0), 1.2) * 220)
        draw.ellipse([cx - r, cy - r, cx + r, cy + r], fill=(255, 255, 230, alpha))
        
    img.save("godot/assets/generated/streetlight_cone.png")
    img.save("godot/assets/streetlight_cone.png")
    print("Created streetlight_cone.png")

# 6. SWAT Armored Truck (Top-Down)
def create_swat_truck():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # Drop shadow
    draw.rectangle([45, 35, 475, 235], fill=(0, 0, 0, 120))
    
    # 6 Heavy Tires
    tire_col = (25, 25, 28, 255)
    for tx in [95, 256, 410]:
        draw.rectangle([tx - 28, 18, tx + 28, 48], fill=tire_col, outline=(10, 10, 10, 255), width=2)
        draw.rectangle([tx - 28, 208, tx + 28, 238], fill=tire_col, outline=(10, 10, 10, 255), width=2)
        
    # Armored Chassis (Dark Navy steel)
    body_col = (25, 35, 50, 255)
    draw.rounded_rectangle([50, 38, 470, 218], radius=14, fill=body_col, outline=(10, 15, 25, 255), width=4)
    
    # Front Heavy Steel Riot Plow
    draw.polygon([[38, 50], [55, 38], [55, 218], [38, 206], [28, 128]], fill=(60, 65, 75, 255), outline=(15, 15, 20, 255))
    
    # Windshield / Armor-plated slitted glass
    draw.rectangle([110, 60, 160, 196], fill=(15, 20, 30, 255), outline=(60, 75, 95, 255), width=3)
    # Rooftop Turret / Steerable Searchlight
    draw.circle([270, 128], 38, fill=(35, 45, 60, 255), outline=(60, 75, 95, 255), width=3)
    # Halogen Searchlight bulb
    draw.ellipse([275, 110, 305, 146], fill=(255, 255, 200, 255), outline=(20, 20, 20, 255), width=2)
    
    # Rooftop Lightbar (Flashing Blue & Red LEDs)
    draw.rectangle([180, 52, 205, 204], fill=(20, 20, 25, 255), outline=(50, 50, 60, 255), width=2)
    draw.rectangle([184, 60, 201, 120], fill=(255, 40, 50, 255))
    draw.rectangle([184, 136, 201, 196], fill=(50, 130, 255, 255))
    
    img.save("godot/assets/generated/swat_truck.png")
    img.save("godot/assets/swat_truck.png")
    print("Created swat_truck.png")

# 7. Street Getaway Dirt Bike (Top-Down)
def create_motorcycle():
    size = (256, 128)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # Drop shadow
    draw.ellipse([20, 52, 236, 76], fill=(0, 0, 0, 110))
    
    # Front & Rear Knobby Tires
    draw.rectangle([25, 54, 75, 74], fill=(30, 30, 35, 255), outline=(10, 10, 10, 255), width=2)
    draw.rectangle([180, 54, 230, 74], fill=(30, 30, 35, 255), outline=(10, 10, 10, 255), width=2)
    
    # Frame & Engine (Matte Grey + Teal Accents)
    draw.rounded_rectangle([70, 48, 185, 80], radius=8, fill=(70, 75, 80, 255), outline=(20, 20, 20, 255), width=2)
    draw.rectangle([90, 52, 140, 76], fill=(76, 245, 213, 255)) # Neon teal fuel tank
    
    # Handlebars
    draw.line([60, 28, 60, 100], fill=(40, 40, 45, 255), width=5)
    draw.circle([60, 28], 5, fill=(76, 245, 213, 255))
    draw.circle([60, 100], 5, fill=(76, 245, 213, 255))
    
    # Spray Can Side Holsters with mounted cans
    draw.rectangle([145, 32, 175, 48], fill=(255, 59, 119, 255), outline=(10, 10, 10, 255), width=2)
    draw.rectangle([145, 80, 175, 96], fill=(76, 245, 213, 255), outline=(10, 10, 10, 255), width=2)
    
    img.save("godot/assets/generated/motorcycle.png")
    img.save("godot/assets/motorcycle.png")
    print("Created motorcycle.png")

# 8. Paint Bomb Crater Decal
def create_paint_crater():
    size = (512, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    cx, cy = 256, 256
    # Outer radial paint splatter arms
    for i in range(48):
        angle = random.random() * math.tau
        dist = random.uniform(80, 230)
        px = cx + math.cos(angle) * dist
        py = cy + math.sin(angle) * dist
        col = (76, 245, 213, 200) if i % 3 == 0 else (255, 59, 119, 200) if i % 3 == 1 else (180, 85, 255, 200)
        draw.line([cx, cy, px, py], fill=col, width=random.randint(3, 9))
        draw.circle([px, py], random.randint(4, 14), fill=col)
        
    # Dense core blast ring
    draw.ellipse([cx - 85, cy - 85, cx + 85, cy + 85], fill=(76, 245, 213, 220), outline=(255, 255, 255, 255), width=4)
    draw.ellipse([cx - 50, cy - 50, cx + 50, cy + 50], fill=(255, 59, 119, 240), outline=(255, 224, 109, 255), width=3)
    draw.ellipse([cx - 20, cy - 20, cx + 20, cy + 20], fill=(255, 255, 255, 255))
    
    img.save("godot/assets/generated/paint_crater.png")
    img.save("godot/assets/paint_crater.png")
    print("Created paint_crater.png")

# 9. Bullet Splatters Decals
def create_bullet_splatters():
    size = (512, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # 6 Decal spots in 3x2 grid
    positions = [
        (100, 130), (256, 130), (412, 130),
        (100, 380), (256, 380), (412, 380)
    ]
    colors = [
        (76, 245, 213, 240),
        (255, 59, 119, 240),
        (255, 224, 109, 240),
        (118, 255, 3, 240),
        (180, 85, 255, 240),
        (255, 65, 88, 240)
    ]
    
    for (bx, by), col in zip(positions, colors):
        # Splatter star
        for a in range(12):
            ang = a * (math.tau / 12.0) + random.uniform(-0.2, 0.2)
            d = random.uniform(25, 55)
            draw.line([bx, by, bx + math.cos(ang) * d, by + math.sin(ang) * d], fill=col, width=random.randint(3, 6))
            draw.circle([bx + math.cos(ang) * d, by + math.sin(ang) * d], random.randint(3, 7), fill=col)
        # Core paint splash
        draw.ellipse([bx - 22, by - 22, bx + 22, by + 22], fill=col)
        # Bullet crater hole
        draw.ellipse([bx - 10, by - 10, bx + 10, by + 10], fill=(15, 15, 20, 255), outline=(80, 80, 90, 255), width=2)
        # Concrete cracks
        for c_ang in [0.4, 1.8, 3.2, 4.7]:
            draw.line([bx + math.cos(c_ang) * 10, by + math.sin(c_ang) * 10, bx + math.cos(c_ang) * 35, by + math.sin(c_ang) * 35], fill=(30, 30, 35, 255), width=2)
            
    img.save("godot/assets/generated/bullet_splatters.png")
    img.save("godot/assets/bullet_splatters.png")
    print("Created bullet_splatters.png")

# 10. District Championship Crests
def create_district_crests():
    size = (1024, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    names = ["CANAL", "MARKET", "RAILCUT", "DOWNTOWN", "CAP ALLEY", "BASS BLOCK", "TUNNEL", "ROLLER"]
    
    for i in range(8):
        col = i % 4
        row = i // 4
        cx = col * 256 + 128
        cy = row * 256 + 128
        
        # Shield / Hexagon Outer Crest
        gold = (255, 215, 0, 255)
        bronze = (30, 35, 45, 255)
        neon = [(76, 245, 213, 255), (255, 59, 119, 255), (118, 255, 3, 255), (255, 224, 109, 255)][i % 4]
        
        # Hexagon points
        pts = [
            (cx, cy - 80),
            (cx + 68, cy - 40),
            (cx + 68, cy + 40),
            (cx, cy + 85),
            (cx - 68, cy + 40),
            (cx - 68, cy - 40)
        ]
        draw.polygon(pts, fill=bronze, outline=gold, width=4)
        
        # Inner Neon Ring
        draw.ellipse([cx - 48, cy - 48, cx + 48, cy + 48], outline=neon, width=3)
        
        # Crown icon on top
        crown_pts = [
            (cx - 30, cy - 10),
            (cx - 36, cy - 36),
            (cx - 15, cy - 22),
            (cx, cy - 44),
            (cx + 15, cy - 22),
            (cx + 36, cy - 36),
            (cx + 30, cy - 10)
        ]
        draw.polygon(crown_pts, fill=gold, outline=(255, 255, 255, 255), width=2)
        
        # Crossed spray cans
        draw.line([cx - 25, cy + 30, cx + 25, cy - 10], fill=(220, 220, 230, 255), width=4)
        draw.line([cx + 25, cy + 30, cx - 25, cy - 10], fill=(220, 220, 230, 255), width=4)
        
        # Star badge
        draw.polygon([(cx, cy + 18), (cx + 8, cy + 38), (cx + 28, cy + 38), (cx + 12, cy + 50), (cx + 18, cy + 70), (cx, cy + 58), (cx - 18, cy + 70), (cx - 12, cy + 50), (cx - 28, cy + 38), (cx - 8, cy + 38)], fill=neon)
        
    img.save("godot/assets/generated/district_crests.png")
    img.save("godot/assets/district_crests.png")
    print("Created district_crests.png")

if __name__ == "__main__":
    make_dirs()
    create_roller_queen()
    create_heavy_enforcer()
    create_subway_entrance()
    create_hydrant_geyser()
    create_streetlight_cone()
    create_swat_truck()
    create_motorcycle()
    create_paint_crater()
    create_bullet_splatters()
    create_district_crests()
    print("All Batch 2 graphics generated successfully!")
