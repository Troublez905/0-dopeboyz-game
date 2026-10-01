import math
import os
import random
from PIL import Image, ImageDraw, ImageFont, ImageFilter

def make_dirs():
    os.makedirs("godot/assets/generated", exist_ok=True)
    os.makedirs("godot/assets", exist_ok=True)

# 1. Glider Wingsuit
def create_glider_wingsuit():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 128
    draw.polygon([(cx - 220, cy + 40), (cx, cy - 80), (cx + 220, cy + 40)], fill=(20, 25, 32, 240), outline=(76, 245, 213, 255), width=5)
    for x in [-120, 120]:
        draw.ellipse([cx + x - 12, cy + 20, cx + x + 12, cy + 50], fill=(255, 59, 119, 255))
    img.save("godot/assets/generated/glider_wingsuit.png")
    img.save("godot/assets/glider_wingsuit.png")
    print("Created glider_wingsuit.png")

# 2. SWAT Van
def create_swat_van():
    size = (512, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 256
    draw.rectangle([cx - 180, cy - 100, cx + 180, cy + 120], fill=(15, 18, 22, 255), outline=(50, 60, 75, 255), width=6)
    draw.rectangle([cx - 160, cy - 80, cx + 160, cy - 30], fill=(40, 50, 65, 255))
    draw.text((cx - 60, cy - 65), "POLICE SWAT", fill=(255, 255, 255, 255))
    img.save("godot/assets/generated/swat_van_breaching.png")
    img.save("godot/assets/swat_van_breaching.png")
    print("Created swat_van_breaching.png")

# 3. Subway Train Car
def create_subway_train():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rectangle([20, 40, 492, 200], fill=(200, 210, 220, 255), outline=(40, 48, 56, 255), width=5)
    for wx in range(60, 450, 90):
        draw.rectangle([wx, 65, wx + 60, 120], fill=(76, 245, 213, 160), outline=(20, 20, 20, 255), width=2)
    draw.ellipse([80, 130, 432, 190], fill=(255, 59, 119, 90))
    img.save("godot/assets/generated/subway_train_car.png")
    img.save("godot/assets/subway_train_car.png")
    print("Created subway_train_car.png")

# 4. Slow-Mo Adrenaline FX
def create_slowmo_fx():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    draw.ellipse([cx - 110, cy - 110, cx + 110, cy + 110], fill=(76, 245, 213, 20), outline=(76, 245, 213, 180), width=4)
    draw.text((cx - 50, cy - 12), "SLOW-MO", fill=(76, 245, 213, 255))
    img.save("godot/assets/generated/slowmo_adrenaline_fx.png")
    img.save("godot/assets/slowmo_adrenaline_fx.png")
    print("Created slowmo_adrenaline_fx.png")

# 5. Grappling Hook Launcher
def create_grappling_hook():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 140
    draw.rectangle([cx - 15, cy - 40, cx + 15, cy + 40], fill=(50, 60, 70, 255), outline=(20, 20, 20, 255), width=3)
    draw.polygon([(cx - 30, cy - 40), (cx + 30, cy - 40), (cx, cy - 90)], fill=(200, 210, 220, 255))
    img.save("godot/assets/generated/grappling_hook_launcher.png")
    img.save("godot/assets/grappling_hook_launcher.png")
    print("Created grappling_hook_launcher.png")

# 6. DJ Turntable Booth
def create_dj_booth():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rectangle([30, 60, 482, 220], fill=(20, 25, 32, 255), outline=(255, 224, 109, 255), width=4)
    for tx in [130, 380]:
        draw.ellipse([tx - 55, 90, tx + 55, 200], fill=(10, 10, 12, 255), outline=(200, 200, 200, 255), width=3)
        draw.ellipse([tx - 18, 130, tx + 18, 160], fill=(255, 59, 119, 255))
    img.save("godot/assets/generated/dj_turntable_booth.png")
    img.save("godot/assets/dj_turntable_booth.png")
    print("Created dj_turntable_booth.png")

# 7. Aerosol Flamethrower
def create_flamethrower():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    for _ in range(40):
        fx = random.randint(100, 480)
        fy = random.randint(40, 210)
        fr = random.randint(15, 45)
        draw.ellipse([fx - fr, fy - fr, fx + fr, fy + fr], fill=(255, random.randint(60, 180), 0, 160))
    img.save("godot/assets/generated/aerosol_flamethrower_fire.png")
    img.save("godot/assets/aerosol_flamethrower_fire.png")
    print("Created aerosol_flamethrower_fire.png")

# 8. Recon Spotter Drone
def create_spotter_drone():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    draw.ellipse([cx - 40, cy - 40, cx + 40, cy + 40], fill=(30, 15, 20, 255), outline=(255, 40, 60, 255), width=4)
    draw.ellipse([cx - 15, cy - 15, cx + 15, cy + 15], fill=(255, 30, 50, 255))
    img.save("godot/assets/generated/recon_spotter_drone.png")
    img.save("godot/assets/recon_spotter_drone.png")
    print("Created recon_spotter_drone.png")

# 9. Stencil Stamps Sheet
def create_stencil_stamps():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    stamps = ["👑 CROWN", "💀 SKULL", "🎨 CAN", "⚡ BOLT", "⭐ STAR", "🛡️ CREST"]
    for i, s in enumerate(stamps):
        cx = (i % 3) * 160 + 80
        cy = (i // 3) * 110 + 65
        draw.ellipse([cx - 35, cy - 35, cx + 35, cy + 35], fill=(25, 30, 38, 255), outline=(76, 245, 213, 255), width=3)
        draw.text((cx - 30, cy - 8), s, fill=(255, 224, 109, 255))
    img.save("godot/assets/generated/stencil_stamps_sheet.png")
    img.save("godot/assets/stencil_stamps_sheet.png")
    print("Created stencil_stamps_sheet.png")

# 10. Thunderstorm Lightning
def create_lightning():
    size = (256, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.polygon([(128, 10), (160, 220), (120, 220), (150, 490), (80, 260), (120, 260)], fill=(255, 255, 255, 255))
    img.save("godot/assets/generated/thunderstorm_lightning.png")
    img.save("godot/assets/thunderstorm_lightning.png")
    print("Created thunderstorm_lightning.png")

# 11. Food Truck
def create_food_truck():
    size = (512, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 256
    draw.rectangle([cx - 160, cy - 100, cx + 160, cy + 120], fill=(240, 180, 40, 255), outline=(20, 20, 20, 255), width=5)
    draw.rectangle([cx - 140, cy - 60, cx + 140, cy], fill=(30, 35, 45, 255))
    draw.text((cx - 80, cy - 40), "TACO 404 STREET", fill=(255, 255, 255, 255))
    img.save("godot/assets/generated/food_truck_station.png")
    img.save("godot/assets/food_truck_station.png")
    print("Created food_truck_station.png")

# 12. K9 Handler Officer
def create_k9_handler():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    draw.rectangle([cx - 25, cy - 40, cx + 25, cy + 50], fill=(25, 35, 55, 255), outline=(20, 20, 20, 255), width=3)
    draw.ellipse([cx - 18, cy - 70, cx + 18, cy - 35], fill=(200, 160, 130, 255))
    draw.text((cx - 22, cy - 10), "POLICE", fill=(255, 255, 255, 255))
    img.save("godot/assets/generated/k9_handler_officer.png")
    img.save("godot/assets/k9_handler_officer.png")
    print("Created k9_handler_officer.png")

# 13. Crew Jackets Sheet
def create_jackets_sheet():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    jackets = [("Spiked Denim", (40, 80, 150, 255)), ("Neon Leather", (255, 59, 119, 255)), ("Tactical Plate", (40, 50, 60, 255)), ("Graffiti Parka", (255, 224, 109, 255))]
    for i, (name, col) in enumerate(jackets):
        cx = i * 125 + 60
        draw.rectangle([cx - 30, 40, cx + 30, 180], fill=col, outline=(20, 20, 20, 255), width=3)
        draw.text((cx - 35, 195), name, fill=(240, 245, 250, 255))
    img.save("godot/assets/generated/crew_jackets_sheet.png")
    img.save("godot/assets/crew_jackets_sheet.png")
    print("Created crew_jackets_sheet.png")

# 14. Helipad
def create_helipad():
    size = (512, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 256
    draw.ellipse([cx - 180, cy - 180, cx + 180, cy + 180], fill=(40, 45, 55, 255), outline=(240, 200, 30, 255), width=8)
    draw.text((cx - 40, cy - 60), "H", fill=(255, 255, 255, 255))
    img.save("godot/assets/generated/rooftop_helipad.png")
    img.save("godot/assets/rooftop_helipad.png")
    print("Created rooftop_helipad.png")

# 15. Chameleon Rainbow Can
def create_chameleon_can():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    draw.rectangle([cx - 30, cy - 50, cx + 30, cy + 50], fill=(255, 224, 109, 255), outline=(76, 245, 213, 255), width=4)
    draw.ellipse([cx - 40, cy - 40, cx + 40, cy + 40], fill=(255, 59, 119, 100))
    img.save("godot/assets/generated/chameleon_rainbow_can.png")
    img.save("godot/assets/chameleon_rainbow_can.png")
    print("Created chameleon_rainbow_can.png")

# 16. Hologram Decoy Emitter
def create_hologram_emitter():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 160
    draw.ellipse([cx - 50, cy - 20, cx + 50, cy + 20], fill=(20, 25, 35, 255), outline=(255, 59, 119, 255), width=3)
    draw.polygon([(cx - 30, cy), (cx + 30, cy), (cx, cy - 120)], fill=(255, 59, 119, 60))
    img.save("godot/assets/generated/decoy_hologram_emitter.png")
    img.save("godot/assets/decoy_hologram_emitter.png")
    print("Created decoy_hologram_emitter.png")

# 17. Skate Park Bowl
def create_skate_bowl():
    size = (512, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 256
    draw.ellipse([cx - 200, cy - 200, cx + 200, cy + 200], fill=(180, 185, 190, 255), outline=(76, 245, 213, 255), width=8)
    draw.ellipse([cx - 140, cy - 140, cx + 140, cy + 140], fill=(120, 125, 130, 255))
    img.save("godot/assets/generated/skate_park_bowl.png")
    img.save("godot/assets/skate_park_bowl.png")
    print("Created skate_park_bowl.png")

# 18. Cluster Paint Bomb
def create_cluster_bomb():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    draw.ellipse([cx - 40, cy - 40, cx + 40, cy + 40], fill=(255, 59, 119, 255), outline=(255, 224, 109, 255), width=4)
    for a in range(0, 360, 72):
        rad = math.radians(a)
        bx = cx + math.cos(rad) * 35
        by = cy + math.sin(rad) * 35
        draw.ellipse([bx - 12, by - 12, bx + 12, by + 12], fill=(76, 245, 213, 255))
    img.save("godot/assets/generated/cluster_paint_bomb.png")
    img.save("godot/assets/cluster_paint_bomb.png")
    print("Created cluster_paint_bomb.png")

# 19. Watchtower
def create_watchtower():
    size = (256, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx = 128
    draw.line([(40, 480), (80, 120)], fill=(50, 60, 70, 255), width=8)
    draw.line([(216, 480), (176, 120)], fill=(50, 60, 70, 255), width=8)
    draw.rectangle([60, 60, 196, 130], fill=(30, 35, 45, 255), outline=(240, 200, 30, 255), width=4)
    img.save("godot/assets/generated/spotlight_watchtower.png")
    img.save("godot/assets/spotlight_watchtower.png")
    print("Created spotlight_watchtower.png")

# 20. Tuning Bench
def create_tuning_bench():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rectangle([20, 40, 492, 210], fill=(80, 55, 35, 255), outline=(20, 20, 20, 255), width=5)
    draw.text((160, 60), "WORKBENCH NOZZLE TUNING", fill=(255, 224, 109, 255))
    img.save("godot/assets/generated/nozzle_tuning_bench.png")
    img.save("godot/assets/nozzle_tuning_bench.png")
    print("Created nozzle_tuning_bench.png")

# 21. Breakdancer
def create_breakdancer():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    draw.ellipse([cx - 50, cy - 50, cx + 50, cy + 50], fill=(255, 224, 109, 255), outline=(255, 59, 119, 255), width=4)
    img.save("godot/assets/generated/breakdancer_performer.png")
    img.save("godot/assets/breakdancer_performer.png")
    print("Created breakdancer_performer.png")

# 22. Bubble Shield
def create_bubble_shield():
    size = (256, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 128, 128
    draw.ellipse([cx - 100, cy - 100, cx + 100, cy + 100], fill=(76, 245, 213, 80), outline=(76, 245, 213, 240), width=6)
    img.save("godot/assets/generated/bubble_shield_barrier.png")
    img.save("godot/assets/bubble_shield_barrier.png")
    print("Created bubble_shield_barrier.png")

# 23. Command Trailer
def create_command_trailer():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rectangle([20, 60, 492, 210], fill=(40, 30, 35, 255), outline=(255, 59, 119, 255), width=5)
    draw.text((140, 100), "RIVAL OUTPOST TRAILER", fill=(255, 59, 119, 255))
    img.save("godot/assets/generated/rival_command_trailer.png")
    img.save("godot/assets/rival_command_trailer.png")
    print("Created rival_command_trailer.png")

# 24. Roller Derby Banner
def create_roller_derby():
    size = (512, 256)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rectangle([20, 80, 492, 180], fill=(76, 245, 213, 220), outline=(20, 20, 20, 255), width=4)
    draw.text((150, 110), "🏁 ROLLER DERBY FINISH 🏁", fill=(20, 20, 20, 255))
    img.save("godot/assets/generated/roller_derby_pursuit.png")
    img.save("godot/assets/roller_derby_pursuit.png")
    print("Created roller_derby_pursuit.png")

# 25. Park Fountain
def create_park_fountain():
    size = (512, 512)
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx, cy = 256, 256
    draw.ellipse([cx - 180, cy - 180, cx + 180, cy + 180], fill=(210, 215, 220, 255), outline=(76, 245, 213, 255), width=8)
    draw.ellipse([cx - 100, cy - 100, cx + 100, cy + 100], fill=(76, 245, 213, 160))
    draw.text((cx - 90, cy - 10), "PARK FOUNTAIN", fill=(255, 224, 109, 255))
    img.save("godot/assets/generated/park_fountain_takeover.png")
    img.save("godot/assets/park_fountain_takeover.png")
    print("Created park_fountain_takeover.png")

if __name__ == "__main__":
    make_dirs()
    create_glider_wingsuit()
    create_swat_van()
    create_subway_train()
    create_slowmo_fx()
    create_grappling_hook()
    create_dj_booth()
    create_flamethrower()
    create_spotter_drone()
    create_stencil_stamps()
    create_lightning()
    create_food_truck()
    create_k9_handler()
    create_jackets_sheet()
    create_helipad()
    create_chameleon_can()
    create_hologram_emitter()
    create_skate_bowl()
    create_cluster_bomb()
    create_watchtower()
    create_tuning_bench()
    create_breakdancer()
    create_bubble_shield()
    create_command_trailer()
    create_roller_derby()
    create_park_fountain()
    print("All 25 Batch 5 graphics generated successfully!")
