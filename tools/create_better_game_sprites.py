import math
import random
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(r"C:\Users\ghost\Desktop\Ideas-Brainstorms\0-dopeboyz game")
OUT_DIRS = [
    ROOT / "godot" / "assets",
    ROOT / "godot" / "assets" / "generated",
    ROOT / "unity" / "Assets" / "Sprites",
    ROOT / "unity" / "Assets" / "Sprites" / "generated"
]

# Palette
CYAN = (76, 245, 213, 255)
CYAN_GLOW = (76, 245, 213, 90)
MAGENTA = (255, 59, 119, 255)
MAGENTA_GLOW = (255, 59, 119, 90)
GOLD = (255, 224, 109, 255)
GOLD_GLOW = (255, 224, 109, 80)
LIME = (118, 255, 3, 255)
PURPLE = (180, 85, 255, 255)
RED = (255, 65, 88, 255)
BLUE = (40, 130, 255, 255)
INK = (8, 8, 12, 255)
CHARCOAL = (33, 36, 43, 255)
GREY = (92, 98, 108, 255)
LIGHT_GREY = (180, 190, 205, 255)
WHITE = (245, 247, 244, 255)

def canvas(w, h, scale=3):
    return Image.new("RGBA", (w * scale, h * scale), (0, 0, 0, 0)), scale

def down(img, w, h):
    return img.resize((w, h), Image.Resampling.LANCZOS)

def poly(draw, pts, fill, outline=INK, width=8):
    draw.polygon(pts, fill=fill)
    if outline and width > 0:
        draw.line(pts + [pts[0]], fill=outline, width=width, joint="curve")

def line(draw, pts, fill, width=6):
    draw.line(pts, fill=fill, width=width, joint="curve")

def ellipse(draw, box, fill, outline=INK, width=6):
    x0, y0, x1, y1 = box
    draw.ellipse((min(x0, x1), min(y0, y1), max(x0, x1), max(y0, y1)), fill=fill, outline=outline, width=width)

def get_font(size):
    try:
        return ImageFont.truetype("arialbd.ttf", size)
    except OSError:
        return ImageFont.load_default()

# 1. Lowrider Car (512x512)
def lowrider():
    img, s = canvas(512, 512)
    d = ImageDraw.Draw(img)
    body = [(78*s, 248*s), (144*s, 156*s), (366*s, 136*s), (444*s, 220*s), (418*s, 330*s), (128*s, 350*s)]
    poly(d, body, MAGENTA, width=12*s)
    roof = [(172*s, 166*s), (252*s, 124*s), (352*s, 142*s), (394*s, 212*s), (214*s, 220*s)]
    poly(d, roof, (232, 35, 115, 255), width=9*s)
    poly(d, [(210*s,178*s),(258*s,150*s),(286*s,210*s),(220*s,210*s)], CYAN, width=5*s)
    poly(d, [(292*s,154*s),(344*s,162*s),(374*s,210*s),(304*s,210*s)], CYAN, width=5*s)
    line(d, [(116*s, 278*s), (410*s, 252*s)], GOLD, 8*s)
    line(d, [(132*s, 306*s), (390*s, 286*s)], WHITE, 5*s)
    for x, y in [(126, 344), (374, 330), (126, 188), (370, 174)]:
        ellipse(d, ((x-38)*s, (y-28)*s, (x+38)*s, (y+28)*s), INK, GOLD, 8*s)
        ellipse(d, ((x-20)*s, (y-14)*s, (x+20)*s, (y+14)*s), (210, 220, 230, 255), WHITE, 4*s)
    line(d, [(104*s, 240*s), (86*s, 268*s), (96*s, 300*s)], (220, 230, 238, 255), 10*s)
    line(d, [(226*s, 238*s), (356*s, 222*s)], PURPLE, 6*s)
    return down(img, 512, 512)

# 2. SWAT Van (512x512)
def swat_van():
    img, s = canvas(512, 512)
    d = ImageDraw.Draw(img)
    body = [(86*s, 158*s), (342*s, 128*s), (440*s, 216*s), (430*s, 360*s), (122*s, 382*s), (72*s, 286*s)]
    poly(d, body, CHARCOAL, width=12*s)
    poly(d, [(132*s,182*s),(260*s,162*s),(300*s,226*s),(150*s,238*s)], (64, 74, 82, 255), width=7*s)
    poly(d, [(312*s,166*s),(392*s,224*s),(388*s,284*s),(326*s,226*s)], (58, 68, 76, 255), width=7*s)
    line(d, [(102*s, 284*s), (430*s, 258*s)], GREY, 7*s)
    d.rectangle((248*s, 244*s, 414*s, 324*s), fill=(20, 22, 26, 255), outline=INK, width=7*s)
    font = get_font(37*s)
    d.text((264*s, 248*s), "SWAT", fill=WHITE, font=font, stroke_width=2*s, stroke_fill=INK)
    d.rounded_rectangle((244*s, 96*s, 350*s, 126*s), radius=8*s, fill=INK, outline=INK, width=5*s)
    d.rectangle((254*s, 100*s, 286*s, 122*s), fill=BLUE)
    d.rectangle((304*s, 100*s, 338*s, 122*s), fill=RED)
    for x, y in [(122, 374), (374, 352), (118, 150), (360, 134)]:
        ellipse(d, ((x-32)*s, (y-24)*s, (x+32)*s, (y+24)*s), INK, (18, 18, 20, 255), 4*s)
    return down(img, 512, 512)

# 3. Roller Queen (1024x1024)
def roller_queen():
    img, s = canvas(1024, 1024)
    d = ImageDraw.Draw(img)
    ellipse(d, (410*s, 120*s, 578*s, 288*s), (151, 83, 50, 255), width=12*s)
    d.arc((344*s, 84*s, 604*s, 326*s), 160, 340, fill=INK, width=34*s)
    poly(d, [(336*s,326*s),(576*s,292*s),(690*s,536*s),(500*s,676*s),(300*s,550*s)], GOLD, width=18*s)
    line(d, [(330*s,390*s),(164*s,558*s),(120*s,690*s)], INK, 24*s)
    line(d, [(332*s,390*s),(164*s,558*s),(120*s,690*s)], PURPLE, 12*s)
    line(d, [(594*s,442*s),(770*s,578*s),(870*s,690*s)], INK, 24*s)
    line(d, [(594*s,442*s),(770*s,578*s),(870*s,690*s)], PURPLE, 12*s)
    line(d, [(430*s,672*s),(340*s,850*s)], INK, 26*s)
    line(d, [(572*s,650*s),(680*s,842*s)], INK, 26*s)
    for x, y in [(310, 862), (696, 858)]:
        d.rounded_rectangle((x*s, y*s, (x+148)*s, (y+58)*s), radius=24*s, fill=MAGENTA, outline=INK, width=12*s)
        for i in range(4):
            ellipse(d, ((x+18+i*32)*s, (y+46)*s, (x+42+i*32)*s, (y+70)*s), CYAN, width=5*s)
    line(d, [(106*s,700*s),(862*s,408*s)], INK, 22*s)
    line(d, [(106*s,700*s),(862*s,408*s)], PURPLE, 10*s)
    d.rounded_rectangle((64*s,690*s,190*s,770*s), radius=20*s, fill=MAGENTA, outline=INK, width=10*s)
    for x, y, c in [(420,206,INK),(516,204,INK),(468,252,RED)]:
        ellipse(d, ((x-10)*s, (y-10)*s, (x+10)*s, (y+10)*s), c, c, 1)
    return down(img, 1024, 1024)

# 4. Wildstyle 404 (512x256)
def wildstyle():
    img, s = canvas(512, 256)
    d = ImageDraw.Draw(img)
    font = get_font(128*s)
    text = "404"
    d.text((38*s, 42*s), text, font=font, fill=INK, stroke_width=20*s, stroke_fill=INK)
    d.text((28*s, 34*s), text, font=font, fill=CYAN, stroke_width=10*s, stroke_fill=MAGENTA)
    line(d, [(42*s,46*s),(8*s,112*s),(62*s,104*s)], PURPLE, 12*s)
    line(d, [(410*s,52*s),(500*s,26*s),(450*s,108*s)], MAGENTA, 12*s)
    line(d, [(98*s,196*s),(240*s,160*s),(370*s,202*s)], WHITE, 7*s)
    for x, y in [(26, 196), (448, 174), (474, 64), (62, 38)]:
        ellipse(d, ((x-12)*s, (y-12)*s, (x+12)*s, (y+12)*s), MAGENTA, MAGENTA, 1)
    return down(img, 512, 256)

# 5. Boombox (256x256)
def boombox():
    img, s = canvas(256, 256)
    d = ImageDraw.Draw(img)
    # Handle
    poly(d, [(70*s, 50*s), (186*s, 50*s), (186*s, 75*s), (70*s, 75*s)], (45, 50, 60, 255), width=6*s)
    # Main Body
    d.rounded_rectangle((30*s, 70*s, 226*s, 210*s), radius=14*s, fill=CHARCOAL, outline=INK, width=8*s)
    # Dual Speakers
    for cx in [78, 178]:
        ellipse(d, ((cx-36)*s, (140-36)*s, (cx+36)*s, (140+36)*s), INK, CYAN, 6*s)
        ellipse(d, ((cx-24)*s, (140-24)*s, (cx+24)*s, (140+24)*s), (20, 24, 30, 255), MAGENTA, 4*s)
        ellipse(d, ((cx-10)*s, (140-10)*s, (cx+10)*s, (140+10)*s), (210, 220, 235, 255), WHITE, 3*s)
    # Center Cassette deck
    d.rectangle((108*s, 115*s, 148*s, 165*s), fill=(20, 22, 28, 255), outline=INK, width=4*s)
    d.rectangle((114*s, 122*s, 142*s, 142*s), fill=CYAN)
    # Top Equalizer LED bar
    for i in range(7):
        col = LIME if i < 4 else (GOLD if i < 6 else RED)
        d.rectangle(((100 + i*8)*s, 85*s, (105 + i*8)*s, 98*s), fill=col)
    # Antenna
    line(d, [(40*s, 70*s), (15*s, 25*s)], (200, 210, 225, 255), 5*s)
    ellipse(d, (10*s, 20*s, 20*s, 30*s), RED, RED, 1)
    return down(img, 256, 256)

# 6. Black Market Van (512x512)
def black_market_van():
    img, s = canvas(512, 512)
    d = ImageDraw.Draw(img)
    # Underglow shadow/glow
    d.ellipse((80*s, 320*s, 432*s, 440*s), fill=MAGENTA_GLOW)
    # Body
    body = [(90*s, 140*s), (380*s, 110*s), (440*s, 230*s), (420*s, 370*s), (130*s, 390*s), (70*s, 270*s)]
    poly(d, body, (28, 30, 36, 255), outline=INK, width=12*s)
    # Windshield & Windows
    poly(d, [(130*s, 165*s), (250*s, 145*s), (280*s, 215*s), (150*s, 225*s)], (45, 55, 65, 255), width=6*s)
    # Open rear doors revealing glowing contraband
    d.rectangle((100*s, 240*s, 160*s, 360*s), fill=(15, 16, 20, 255), outline=INK, width=6*s)
    # Shelves of spray cans inside
    for y in [265, 295, 325]:
        line(d, [(105*s, y*s), (155*s, y*s)], GREY, 4*s)
        for i, col in enumerate([CYAN, MAGENTA, GOLD]):
            d.rectangle(((110 + i*14)*s, (y-16)*s, (120 + i*14)*s, y*s), fill=col)
    # Slap tags on side
    font = get_font(32*s)
    d.text((220*s, 250*s), "BLACK MARKET", fill=GOLD, font=font, stroke_width=4*s, stroke_fill=INK)
    d.text((250*s, 290*s), "404 RARE", fill=CYAN, font=get_font(24*s))
    # Heavy wheels
    for x, y in [(120, 385), (380, 365), (115, 145), (370, 125)]:
        ellipse(d, ((x-34)*s, (y-24)*s, (x+34)*s, (y+24)*s), INK, GOLD, 6*s)
    return down(img, 512, 512)

# 7. Scrambler Motorcycle (256x256)
def motorcycle():
    img, s = canvas(256, 256)
    d = ImageDraw.Draw(img)
    # Wheels (top-down view)
    ellipse(d, (30*s, 105*s, 80*s, 151*s), INK, CYAN, 6*s)
    ellipse(d, (45*s, 118*s, 65*s, 138*s), (200, 210, 225, 255), INK, 3*s)
    ellipse(d, (176*s, 105*s, 226*s, 151*s), INK, CYAN, 6*s)
    ellipse(d, (191*s, 118*s, 211*s, 138*s), (200, 210, 225, 255), INK, 3*s)
    # Frame & Engine
    poly(d, [(70*s, 110*s), (180*s, 110*s), (160*s, 146*s), (90*s, 146*s)], CHARCOAL, width=8*s)
    # Curved Chrome Exhaust
    line(d, [(100*s, 135*s), (150*s, 150*s), (200*s, 140*s)], (225, 235, 245, 255), 8*s)
    # Leather Seat
    d.rounded_rectangle((100*s, 100*s, 155*s, 122*s), radius=6*s, fill=(110, 60, 30, 255), outline=INK, width=4*s)
    # Handlebars
    line(d, [(60*s, 80*s), (75*s, 115*s), (60*s, 150*s)], (180, 190, 205, 255), 7*s)
    ellipse(d, (56*s, 76*s, 64*s, 84*s), INK, INK, 1)
    ellipse(d, (56*s, 146*s, 64*s, 154*s), INK, INK, 1)
    # Headlight
    ellipse(d, (30*s, 116*s, 44*s, 140*s), GOLD, INK, 3*s)
    return down(img, 256, 256)

# 8. News Broadcast Chopper (512x512)
def news_chopper():
    img, s = canvas(512, 512)
    d = ImageDraw.Draw(img)
    cx, cy = 256*s, 256*s
    # Spinning rotor blur ring
    d.ellipse((cx - 210*s, cy - 210*s, cx + 210*s, cy + 210*s), outline=(255, 255, 255, 70), width=10*s)
    line(d, [(cx - 220*s, cy), (cx + 220*s, cy)], (240, 245, 255, 140), 12*s)
    line(d, [(cx, cy - 220*s), (cx, cy + 220*s)], (240, 245, 255, 140), 12*s)
    # Tail Boom & Fin
    poly(d, [(cx - 24*s, cy + 60*s), (cx + 24*s, cy + 60*s), (cx + 12*s, cy + 220*s), (cx - 12*s, cy + 220*s)], WHITE, width=8*s)
    poly(d, [(cx - 45*s, cy + 200*s), (cx + 45*s, cy + 200*s), (cx, cy + 235*s)], RED, width=6*s)
    # Fuselage Body (Yellow & White)
    ellipse(d, (cx - 75*s, cy - 140*s, cx + 75*s, cy + 80*s), GOLD, INK, 12*s)
    ellipse(d, (cx - 50*s, cy - 120*s, cx + 50*s, cy - 30*s), (40, 55, 75, 255), CYAN, 6*s)
    # News Camera Gimbal (Front)
    ellipse(d, (cx - 22*s, cy - 165*s, cx + 22*s, cy - 125*s), CHARCOAL, INK, 6*s)
    ellipse(d, (cx - 8*s, cy - 160*s, cx + 8*s, cy - 144*s), RED, RED, 1)
    # "NEWS 4" Text
    font = get_font(26*s)
    d.text((cx - 42*s, cy - 10*s), "NEWS 4", fill=INK, font=font)
    return down(img, 512, 512)

# 9. Subway Train Car (512x256)
def subway_train_car():
    img, s = canvas(512, 256)
    d = ImageDraw.Draw(img)
    # Corrugated Stainless Steel Body
    d.rounded_rectangle((30*s, 40*s, 482*s, 216*s), radius=16*s, fill=(195, 205, 218, 255), outline=INK, width=10*s)
    # Ribbed roof ridges
    for rx in range(60, 460, 24):
        line(d, [(rx*s, 50*s), (rx*s, 206*s)], (165, 175, 188, 255), 4*s)
    # Roof vent caps
    for vx in [110, 256, 402]:
        ellipse(d, ((vx-22)*s, 110*s, (vx+22)*s, 146*s), (120, 130, 145, 255), INK, 5*s)
    # Illuminated Passenger Windows
    for wx in [70, 150, 310, 390]:
        d.rounded_rectangle((wx*s, 55*s, (wx+52)*s, 85*s), radius=4*s, fill=CYAN, outline=INK, width=4*s)
        d.rounded_rectangle((wx*s, 171*s, (wx+52)*s, 201*s), radius=4*s, fill=CYAN, outline=INK, width=4*s)
    # Huge Spray Graffiti pieces on side
    font = get_font(52*s)
    d.text((160*s, 95*s), "DOPEBOYZ", fill=MAGENTA, font=font, stroke_width=6*s, stroke_fill=INK)
    d.text((155*s, 90*s), "DOPEBOYZ", fill=GOLD, font=font)
    return down(img, 512, 256)

# 10. Alley Dumpster (256x256)
def alley_dumpster():
    img, s = canvas(256, 256)
    d = ImageDraw.Draw(img)
    # Container main box
    d.rounded_rectangle((30*s, 75*s, 226*s, 215*s), radius=10*s, fill=(65, 90, 60, 255), outline=INK, width=8*s)
    # Dual Plastic Lids
    d.rounded_rectangle((24*s, 55*s, 124*s, 82*s), radius=6*s, fill=(40, 44, 48, 255), outline=INK, width=5*s)
    d.rounded_rectangle((132*s, 55*s, 232*s, 82*s), radius=6*s, fill=(40, 44, 48, 255), outline=INK, width=5*s)
    # Side fork lift pockets
    d.rectangle((18*s, 120*s, 32*s, 160*s), fill=CHARCOAL, outline=INK, width=4*s)
    d.rectangle((224*s, 120*s, 238*s, 160*s), fill=CHARCOAL, outline=INK, width=4*s)
    # Street Tags on Front
    font = get_font(32*s)
    d.text((50*s, 110*s), "404", fill=CYAN, font=font, stroke_width=4*s, stroke_fill=INK)
    d.text((115*s, 125*s), "TAG", fill=MAGENTA, font=get_font(26*s), stroke_width=3*s, stroke_fill=INK)
    # Caster wheels
    for wx in [50, 185]:
        ellipse(d, ((wx-12)*s, 212*s, (wx+12)*s, 236*s), CHARCOAL, INK, 4*s)
    return down(img, 256, 256)

# 11. Vending Machine Kiosk (256x256)
def vending_machine():
    img, s = canvas(256, 256)
    d = ImageDraw.Draw(img)
    # Cabinet
    d.rounded_rectangle((40*s, 30*s, 216*s, 230*s), radius=12*s, fill=(35, 40, 52, 255), outline=INK, width=8*s)
    # Illuminated Header Marquee
    d.rounded_rectangle((52*s, 42*s, 204*s, 76*s), radius=6*s, fill=MAGENTA, outline=INK, width=5*s)
    font = get_font(20*s)
    d.text((65*s, 48*s), "404 BOOST", fill=GOLD, font=font)
    # Glass Display Window with illuminated cans
    d.rounded_rectangle((52*s, 84*s, 204*s, 172*s), radius=6*s, fill=(18, 24, 34, 255), outline=CYAN, width=5*s)
    for row in range(2):
        for col in range(4):
            cx = 70 + col * 34
            cy = 100 + row * 38
            col_can = [CYAN, MAGENTA, GOLD, LIME][col % 4]
            d.rounded_rectangle(((cx-10)*s, (cy-12)*s, (cx+10)*s, (cy+12)*s), radius=3*s, fill=col_can, outline=INK, width=2*s)
    # Dispenser hopper door
    d.rounded_rectangle((65*s, 185*s, 191*s, 218*s), radius=6*s, fill=CHARCOAL, outline=INK, width=5*s)
    return down(img, 256, 256)

# 12. 24K Golden Mastery Can (256x256)
def golden_can():
    img, s = canvas(256, 256)
    d = ImageDraw.Draw(img)
    cx, cy = 128*s, 138*s
    # Celestial Gold Halo Rings
    for r in range(100, 35, -8):
        d.ellipse(((cx-r*s), (cy-r*s), (cx+r*s), (cy+r*s)), fill=GOLD_GLOW)
    # Solid 24K Gold Can Body
    d.rounded_rectangle(((cx-44*s), (cy-55*s), (cx+44*s), (cy+75*s)), radius=12*s, fill=(255, 215, 0, 255), outline=INK, width=8*s)
    # Specular Sheen Stripe
    d.rectangle(((cx-25*s), (cy-50*s), (cx-10*s), (cy+70*s)), fill=WHITE)
    # Dome top & Actuator cap
    ellipse(d, ((cx-40*s), (cy-75*s), (cx+40*s), (cy-40*s)), (255, 235, 90, 255), INK, 6*s)
    d.rectangle(((cx-16*s), (cy-95*s), (cx+16*s), (cy-70*s)), fill=(240, 245, 255, 255), outline=INK, width=5*s)
    # Sparkles
    for ang in [0.4, 1.8, 3.5, 5.0]:
        sx = cx + int(math.cos(ang) * 78 * s)
        sy = cy + int(math.sin(ang) * 78 * s)
        line(d, [(sx - 12*s, sy), (sx + 12*s, sy)], WHITE, 4*s)
        line(d, [(sx, sy - 12*s), (sx, sy + 12*s)], WHITE, 4*s)
    font = get_font(36*s)
    d.text((cx - 30*s, cy - 8*s), "24K", fill=(140, 85, 0, 255), font=font)
    return down(img, 256, 256)

# 13. Automated Paint Sentry Turret (256x256)
def paint_turret():
    img, s = canvas(256, 256)
    d = ImageDraw.Draw(img)
    cx, cy = 128*s, 138*s
    # Tripod Legs
    for ang in [0.7, 2.3, 3.9, 5.5]:
        lx = cx + int(math.cos(ang) * 90 * s)
        ly = cy + int(math.sin(ang) * 90 * s)
        line(d, [(cx, cy), (lx, ly)], CHARCOAL, 12*s)
        ellipse(d, ((lx-14*s), (ly-14*s), (lx+14*s), (ly+14*s)), INK, GOLD, 4*s)
    # Turret Base & Swivel Head
    ellipse(d, ((cx-50*s), (cy-50*s), (cx+50*s), (cy+50*s)), (45, 50, 62, 255), INK, 8*s)
    ellipse(d, ((cx-32*s), (cy-32*s), (cx+32*s), (cy+32*s)), MAGENTA, INK, 6*s)
    # Dual Aerosol Cannons (Facing forward/right)
    poly(d, [(cx+10*s, cy-18*s), (cx+75*s, cy-18*s), (cx+75*s, cy-6*s), (cx+10*s, cy-6*s)], (200, 210, 225, 255), width=5*s)
    poly(d, [(cx+10*s, cy+6*s), (cx+75*s, cy+6*s), (cx+75*s, cy+18*s), (cx+10*s, cy+18*s)], (200, 210, 225, 255), width=5*s)
    # Laser Sight Targeting Beam
    line(d, [(cx+75*s, cy), (cx+115*s, cy)], RED, 4*s)
    ellipse(d, ((cx+112*s), (cy-4*s), (cx+120*s), (cy+4*s)), RED, RED, 1)
    return down(img, 256, 256)

# 14. Heist Cargo Crate (256x256)
def heist_crate():
    img, s = canvas(256, 256)
    d = ImageDraw.Draw(img)
    # Heavy Military Contraband Crate
    d.rounded_rectangle((35*s, 45*s, 221*s, 211*s), radius=12*s, fill=(160, 110, 35, 255), outline=INK, width=8*s)
    # Steel Corner Braces
    for bx, by in [(35, 45), (185, 45), (35, 175), (185, 175)]:
        d.rectangle((bx*s, by*s, (bx+36)*s, (by+36)*s), fill=CHARCOAL, outline=INK, width=4*s)
    # Caution Hazard Stripes
    line(d, [(60*s, 128*s), (196*s, 128*s)], GOLD, 16*s)
    line(d, [(60*s, 128*s), (196*s, 128*s)], INK, 6*s)
    # Digital Keycard Lock
    d.rectangle((108*s, 70*s, 148*s, 105*s), fill=(20, 22, 26, 255), outline=INK, width=4*s)
    ellipse(d, (123*s, 82*s, 133*s, 92*s), CYAN, CYAN, 1)
    font = get_font(20*s)
    d.text((65*s, 155*s), "HAZARD 404", fill=WHITE, font=font, stroke_width=2*s, stroke_fill=INK)
    return down(img, 256, 256)

# 15. Police Roadblock Barricade (512x128)
def police_roadblock():
    img, s = canvas(512, 128)
    d = ImageDraw.Draw(img)
    # A-Frame Leg Supports
    for lx in [55, 457]:
        poly(d, [(lx*s, 25*s), ((lx-25)*s, 115*s), ((lx+25)*s, 115*s)], CHARCOAL, width=7*s)
    # Main Horizontal Barricade Beam
    d.rounded_rectangle((30*s, 35*s, 482*s, 95*s), radius=8*s, fill=WHITE, outline=INK, width=8*s)
    # Diagonal Hazard Orange/Black Stripes
    for sx in range(40, 480, 40):
        poly(d, [(sx*s, 35*s), ((sx+25)*s, 35*s), ((sx+5)*s, 95*s), ((sx-20)*s, 95*s)], RED, outline=None, width=0)
    line(d, [(30*s, 35*s), (482*s, 35*s)], INK, 8*s)
    line(d, [(30*s, 95*s), (482*s, 95*s)], INK, 8*s)
    # Top Warning Beacon Lights
    ellipse(d, (110*s, 12*s, 140*s, 38*s), RED, INK, 4*s)
    ellipse(d, (372*s, 12*s, 402*s, 38*s), BLUE, INK, 4*s)
    return down(img, 512, 128)

# 16. DJ Turntable Booth (512x256)
def dj_booth():
    img, s = canvas(512, 256)
    d = ImageDraw.Draw(img)
    # Tabletop Surface
    d.rounded_rectangle((30*s, 40*s, 482*s, 216*s), radius=16*s, fill=(24, 28, 36, 255), outline=INK, width=10*s)
    # Dual 1200 Turntables
    for px in [120, 392]:
        ellipse(d, ((px-65)*s, 60*s, (px+65)*s, 190*s), INK, GREY, 8*s)
        ellipse(d, ((px-48)*s, 77*s, (px+48)*s, 173*s), (15, 16, 20, 255), (60, 65, 75, 255), 4*s)
        # Vinyl grooves & Slipmat label
        ellipse(d, ((px-20)*s, 105*s, (px+20)*s, 145*s), MAGENTA if px < 250 else CYAN, INK, 4*s)
        # Tonearm
        line(d, [((px+55)*s, 70*s), ((px+45)*s, 125*s)], (210, 220, 235, 255), 6*s)
    # Battle Mixer in Center
    d.rectangle((216*s, 60*s, 296*s, 190*s), fill=(38, 42, 52, 255), outline=INK, width=6*s)
    # Crossfader & Knobs
    line(d, [(230*s, 165*s), (282*s, 165*s)], INK, 6*s)
    d.rectangle((250*s, 155*s, 262*s, 175*s), fill=GOLD, outline=INK, width=3*s)
    # Glowing VU Level Meters
    for i in range(6):
        d.rectangle(((236 + i*8)*s, 75*s, (241 + i*8)*s, 115*s), fill=LIME if i < 4 else RED)
    return down(img, 512, 256)

# 17. Hydrant Geyser (256x256)
def hydrant_geyser():
    img, s = canvas(256, 256)
    d = ImageDraw.Draw(img)
    cx = 128*s
    # Upward erupting water plume
    for r in range(75, 15, -10):
        d.ellipse(((cx-r*s), (10*s), (cx+r*s), (180*s)), fill=(76, 245, 213, 80))
    # Water spray splash lines
    for _ in range(16):
        sx = cx + random.randint(-60*s, 60*s)
        sy = random.randint(20*s, 170*s)
        line(d, [(cx, 160*s), (sx, sy)], WHITE, 4*s)
    # Cast Iron Red Fire Hydrant at base
    d.rounded_rectangle(((cx-35*s), 170*s, (cx+35*s), 236*s), radius=8*s, fill=RED, outline=INK, width=8*s)
    # Side caps
    ellipse(d, ((cx-50*s), 190*s, (cx-30*s), 214*s), GOLD, INK, 4*s)
    ellipse(d, ((cx+30*s), 190*s, (cx+50*s), 214*s), GOLD, INK, 4*s)
    return down(img, 256, 256)

# 18. Water Tower Mural (512x512)
def water_tower():
    img, s = canvas(512, 512)
    d = ImageDraw.Draw(img)
    cx = 256*s
    # Steel Legs / Framework
    line(d, [(cx-140*s, 460*s), (cx-90*s, 280*s)], CHARCOAL, 14*s)
    line(d, [(cx+140*s, 460*s), (cx+90*s, 280*s)], CHARCOAL, 14*s)
    line(d, [(cx-140*s, 460*s), (cx+90*s, 280*s)], GREY, 8*s)
    line(d, [(cx+140*s, 460*s), (cx-90*s, 280*s)], GREY, 8*s)
    # Conical Wooden Roof
    poly(d, [(cx-160*s, 160*s), (cx+160*s, 160*s), (cx, 40*s)], (140, 85, 45, 255), width=10*s)
    # Cylindrical Tank Body
    d.rectangle((cx-150*s, 160*s, cx+150*s, 340*s), fill=(185, 130, 80, 255), outline=INK, width=12*s)
    # Black Iron Hoop Bands
    for hy in [190, 250, 310]:
        line(d, [(cx-150*s, hy*s), (cx+150*s, hy*s)], INK, 8*s)
    # Massive Graffiti Burner across tank
    font = get_font(52*s)
    d.text((cx-130*s, 210*s), "DOPEBOYZ", fill=CYAN, font=font, stroke_width=8*s, stroke_fill=INK)
    d.text((cx-126*s, 206*s), "DOPEBOYZ", fill=MAGENTA, font=font)
    return down(img, 512, 512)

# 19. Market Vendor Stall (256x256)
def market_stall():
    img, s = canvas(256, 256)
    d = ImageDraw.Draw(img)
    # Wooden Display Counter
    d.rounded_rectangle((30*s, 120*s, 226*s, 215*s), radius=8*s, fill=(145, 95, 55, 255), outline=INK, width=8*s)
    # Displayed contraband goods
    for cx in [60, 110, 160, 200]:
        d.rectangle(((cx-10)*s, 95*s, (cx+10)*s, 120*s), fill=random.choice([CYAN, MAGENTA, GOLD]), outline=INK, width=3*s)
    # Striped Awning Canopy
    poly(d, [(20*s, 50*s), (236*s, 50*s), (216*s, 115*s), (40*s, 115*s)], MAGENTA, width=8*s)
    for sx in range(40, 220, 36):
        poly(d, [(sx*s, 50*s), ((sx+18)*s, 50*s), ((sx+10)*s, 115*s), ((sx-8)*s, 115*s)], GOLD, outline=None, width=0)
    line(d, [(20*s, 50*s), (236*s, 50*s)], INK, 8*s)
    line(d, [(40*s, 115*s), (216*s, 115*s)], INK, 8*s)
    font = get_font(20*s)
    d.text((50*s, 150*s), "STREET MARKET", fill=WHITE, font=font, stroke_width=3*s, stroke_fill=INK)
    return down(img, 256, 256)

# 20. Nozzle Tuning Bench (256x256)
def tuning_bench():
    img, s = canvas(256, 256)
    d = ImageDraw.Draw(img)
    # Heavy Workbench Table
    d.rounded_rectangle((30*s, 90*s, 226*s, 205*s), radius=8*s, fill=(70, 78, 92, 255), outline=INK, width=8*s)
    # Tabletop Vice Clamp
    d.rectangle((45*s, 65*s, 85*s, 105*s), fill=CHARCOAL, outline=INK, width=5*s)
    line(d, [(65*s, 50*s), (65*s, 80*s)], (200, 210, 225, 255), 6*s)
    # Sorted Spray Can Nozzle Caps (Fat, Skinny, Needle)
    caps = [(110, 115, CYAN), (135, 115, MAGENTA), (160, 115, GOLD), (185, 115, LIME),
            (110, 145, RED), (135, 145, WHITE), (160, 145, (230, 120, 255, 255)), (185, 145, (255, 180, 50, 255))]
    for cx, cy, col in caps:
        ellipse(d, ((cx-9)*s, (cy-9)*s, (cx+9)*s, (cy+9)*s), col, INK, 3*s)
        ellipse(d, ((cx-3)*s, (cy-3)*s, (cx+3)*s, (cy+3)*s), INK, INK, 1)
    # Solvent Cleaning Jar
    d.rounded_rectangle((120*s, 168*s, 170*s, 198*s), radius=4*s, fill=(76, 245, 213, 160), outline=INK, width=4*s)
    # Table Legs
    for lx in [45, 205]:
        line(d, [(lx*s, 205*s), (lx*s, 235*s)], CHARCOAL, 10*s)
    return down(img, 256, 256)

# 21. High-Pressure Air Compressor Station (256x256)
def air_compressor():
    img, s = canvas(256, 256)
    d = ImageDraw.Draw(img)
    cx, cy = 128*s, 138*s
    # Cylindrical Air Pressure Tank
    d.rounded_rectangle(((cx-65*s), (cy-40*s), (cx+65*s), (cy+40*s)), radius=16*s, fill=RED, outline=INK, width=8*s)
    # Electric Motor Top Housing
    d.rounded_rectangle(((cx-35*s), (cy-75*s), (cx+35*s), (cy-38*s)), radius=6*s, fill=CHARCOAL, outline=INK, width=6*s)
    # Dual Brass Pressure Dial Gauges
    for gx in [cx - 20*s, cx + 20*s]:
        ellipse(d, ((gx-16*s), (cy-105*s), (gx+16*s), (cy-73*s)), WHITE, INK, 4*s)
        line(d, [(gx, cy-89*s), (gx + 8*s, cy-97*s)], RED, 3*s)
    # Wheels
    for wx in [cx - 50*s, cx + 50*s]:
        ellipse(d, ((wx-16*s), (cy+32*s), (wx+16*s), (cy+64*s)), INK, GREY, 4*s)
    # Coiled Air Hose
    for i in range(4):
        d.arc(((cx+50*s + i*6*s), (cy-20*s), (cx+80*s + i*6*s), (cy+20*s)), 0, 360, fill=GOLD, width=6*s)
    return down(img, 256, 256)

# 22. Rival Boss Captain (512x512)
def rival_captain():
    img, s = canvas(512, 512)
    d = ImageDraw.Draw(img)
    cx, cy = 256*s, 240*s

    # Shadow / Underglow
    d.ellipse((cx - 150*s, 440*s, cx + 150*s, 485*s), fill=MAGENTA_GLOW)

    # Legs & Sneaker Boots
    for lx in [cx - 65*s, cx + 65*s]:
        # Baggy Cargo Pants
        poly(d, [(lx - 35*s, 310*s), (lx + 35*s, 310*s), (lx + 25*s, 420*s), (lx - 25*s, 420*s)], (45, 30, 55, 255), width=8*s)
        # Heavy High-Top Street Sneaker
        d.rounded_rectangle((lx - 38*s, 420*s, lx + 38*s, 465*s), radius=12*s, fill=GOLD, outline=INK, width=7*s)
        d.rectangle((lx - 38*s, 450*s, lx + 38*s, 465*s), fill=WHITE, outline=INK, width=4*s)

    # Torso (Studded Leather Biker Vest over Purple Hoodie)
    poly(d, [(cx - 95*s, 190*s), (cx + 95*s, 190*s), (cx + 80*s, 330*s), (cx - 80*s, 330*s)], PURPLE, width=10*s)
    poly(d, [(cx - 75*s, 200*s), (cx + 75*s, 200*s), (cx + 65*s, 325*s), (cx - 65*s, 325*s)], CHARCOAL, width=8*s)
    # Silver studs on lapels
    for sy in [220, 250, 280, 310]:
        ellipse(d, (cx - 55*s, sy*s - 6*s, cx - 43*s, sy*s + 6*s), (230, 235, 245, 255), INK, 3*s)
        ellipse(d, (cx + 43*s, sy*s - 6*s, cx + 55*s, sy*s + 6*s), (230, 235, 245, 255), INK, 3*s)

    # Heavy Gold Chain Medallion
    d.arc((cx - 45*s, 220*s, cx + 45*s, 290*s), 0, 180, fill=GOLD, width=10*s)
    ellipse(d, (cx - 28*s, 285*s, cx + 28*s, 341*s), GOLD, INK, 6*s)
    font = get_font(24*s)
    d.text((cx - 18*s, 300*s), "404", fill=INK, font=font)

    # Arms (Muscular with Sleeve Tattoos & Spiked Wristbands)
    for side, ax in [(-1, cx - 110*s), (1, cx + 110*s)]:
        poly(d, [(cx + side*80*s, 200*s), (ax, 280*s), (ax + side*20*s, 330*s), (cx + side*65*s, 270*s)], (195, 135, 95, 255), width=8*s)
        # Flame tattoo
        line(d, [(ax - 10*s, 240*s), (ax + 10*s, 270*s)], CYAN, 6*s)
        # Spiked wristband
        d.rounded_rectangle((ax - 18*s, 315*s, ax + 18*s, 335*s), radius=4*s, fill=CHARCOAL, outline=INK, width=4*s)

    # Dual Chrome Aerosol Pistols (In Hands)
    for side, hx, hy in [(-1, cx - 140*s, 330*s), (1, cx + 140*s, 330*s)]:
        # Gun body
        d.rounded_rectangle((hx - 20*s, hy - 40*s, hx + 20*s, hy + 20*s), radius=6*s, fill=(225, 235, 245, 255), outline=INK, width=5*s)
        # Extended high-pressure paint nozzle
        line(d, [(hx, hy - 40*s), (hx + side*25*s, hy - 65*s)], RED, 8*s)
        ellipse(d, (hx + side*22*s, hy - 68*s, hx + side*34*s, hy - 56*s), GOLD, INK, 3*s)
        # Fingerless Glove
        ellipse(d, (hx - 15*s, hy - 5*s, hx + 15*s, hy + 25*s), CHARCOAL, INK, 5*s)

    # Head & Face
    # Neck
    d.rectangle((cx - 25*s, 160*s, cx + 25*s, 195*s), fill=(185, 125, 85, 255), outline=INK, width=6*s)
    # Head block
    d.rounded_rectangle((cx - 55*s, 85*s, cx + 55*s, 175*s), radius=16*s, fill=(195, 135, 95, 255), outline=INK, width=8*s)
    # Neon Pink Respirator Mask
    poly(d, [(cx - 45*s, 135*s), (cx + 45*s, 135*s), (cx + 30*s, 185*s), (cx - 30*s, 185*s)], MAGENTA, width=6*s)
    ellipse(d, (cx - 30*s, 145*s, cx - 10*s, 165*s), CHARCOAL, CYAN, 3*s)
    ellipse(d, (cx + 10*s, 145*s, cx + 30*s, 165*s), CHARCOAL, CYAN, 3*s)

    # Cyan Aviator Sunglasses
    poly(d, [(cx - 50*s, 105*s), (cx - 8*s, 105*s), (cx - 15*s, 132*s), (cx - 45*s, 132*s)], CYAN, width=5*s)
    poly(d, [(cx + 8*s, 105*s), (cx + 50*s, 105*s), (cx + 45*s, 132*s), (cx + 15*s, 132*s)], CYAN, width=5*s)
    line(d, [(cx - 8*s, 112*s), (cx + 8*s, 112*s)], INK, 5*s)

    # Backward Red Baseball Cap
    d.arc((cx - 58*s, 60*s, cx + 58*s, 130*s), 180, 360, fill=RED, width=16*s)
    d.rounded_rectangle((cx - 58*s, 72*s, cx + 58*s, 102*s), radius=10*s, fill=RED, outline=INK, width=7*s)
    # Backward Brim
    poly(d, [(cx - 35*s, 72*s), (cx + 35*s, 72*s), (cx + 45*s, 50*s), (cx - 45*s, 50*s)], (180, 25, 45, 255), width=6*s)
    return down(img, 512, 512)

# 23. Heavy Rival Enforcer (512x512)
def heavy_enforcer():
    img, s = canvas(512, 512)
    d = ImageDraw.Draw(img)
    cx, cy = 256*s, 240*s

    # Shadow
    d.ellipse((cx - 160*s, 445*s, cx + 160*s, 490*s), fill=CYAN_GLOW)

    # Huge Chunky Legs & Steel-Toed Boots
    for lx in [cx - 75*s, cx + 75*s]:
        d.rounded_rectangle((lx - 45*s, 310*s, lx + 45*s, 430*s), radius=12*s, fill=(40, 48, 60, 255), outline=INK, width=9*s)
        d.rounded_rectangle((lx - 50*s, 430*s, lx + 50*s, 475*s), radius=10*s, fill=CHARCOAL, outline=INK, width=8*s)
        d.rectangle((lx - 35*s, 435*s, lx + 35*s, 455*s), fill=(210, 220, 235, 255), outline=INK, width=4*s)

    # Massive Armor Torso
    poly(d, [(cx - 140*s, 160*s), (cx + 140*s, 160*s), (cx + 110*s, 330*s), (cx - 110*s, 330*s)], (35, 40, 48, 255), width=12*s)

    # Chained Street Sign Chest Plate ("STOP" sign)
    poly(d, [(cx - 60*s, 180*s), (cx + 60*s, 180*s), (cx + 85*s, 225*s), (cx + 85*s, 265*s),
             (cx + 60*s, 310*s), (cx - 60*s, 310*s), (cx - 85*s, 265*s), (cx - 85*s, 225*s)], RED, width=8*s)
    font = get_font(38*s)
    d.text((cx - 52*s, 222*s), "STOP", fill=WHITE, font=font, stroke_width=4*s, stroke_fill=INK)
    # Chains wrapping chest
    for cy_chain in [195, 290]:
        line(d, [(cx - 120*s, cy_chain*s), (cx + 120*s, cy_chain*s)], (180, 190, 205, 255), 7*s)

    # Massive Brawny Arms with Spiked Brawler Knuckles
    for side, ax in [(-1, cx - 150*s), (1, cx + 150*s)]:
        poly(d, [(cx + side*110*s, 170*s), (ax, 280*s), (ax + side*15*s, 360*s), (cx + side*85*s, 300*s)], (180, 120, 80, 255), width=9*s)
        # Spiked Knuckle Fist
        d.rounded_rectangle((ax - 30*s, 340*s, ax + 30*s, 390*s), radius=10*s, fill=CHARCOAL, outline=INK, width=6*s)
        for kx in [-15, 0, 15]:
            poly(d, [(ax + kx*s - 5*s, 340*s), (ax + kx*s + 5*s, 340*s), (ax + kx*s, 320*s)], GOLD, width=3*s)

    # Welder's Skull Mask Head
    d.rounded_rectangle((cx - 55*s, 70*s, cx + 55*s, 165*s), radius=14*s, fill=CHARCOAL, outline=INK, width=9*s)
    # Glowing Orange Horizontal Visor Slit
    d.rounded_rectangle((cx - 40*s, 105*s, cx + 40*s, 125*s), radius=5*s, fill=(255, 140, 0, 255), outline=INK, width=4*s)
    # Stencil skull teeth
    for tx in range(-25, 35, 12):
        d.rectangle((cx + tx*s, 138*s, cx + (tx+6)*s, 152*s), fill=WHITE)
    return down(img, 512, 512)

# 24. SWAT Riot Shield Officer (512x512)
def riot_shield_officer():
    img, s = canvas(512, 512)
    d = ImageDraw.Draw(img)
    cx, cy = 256*s, 240*s

    # Tactical Body Behind Shield
    poly(d, [(cx - 80*s, 140*s), (cx + 80*s, 140*s), (cx + 65*s, 360*s), (cx - 65*s, 360*s)], (30, 42, 58, 255), width=10*s)

    # Ballistic Tactical Helmet
    d.rounded_rectangle((cx - 52*s, 60*s, cx + 52*s, 145*s), radius=18*s, fill=(22, 28, 38, 255), outline=INK, width=8*s)
    # Reflective Blue Visor
    d.rounded_rectangle((cx - 42*s, 88*s, cx + 42*s, 122*s), radius=8*s, fill=BLUE, outline=CYAN, width=4*s)

    # Massive Clear Polycarbonate Riot Shield in Front
    shield_box = (cx - 130*s, 130*s, cx + 130*s, 440*s)
    d.rounded_rectangle(shield_box, radius=24*s, fill=(210, 235, 255, 160), outline=INK, width=12*s)
    # Reinforcement border frame
    d.rounded_rectangle((cx - 120*s, 140*s, cx + 120*s, 430*s), radius=16*s, outline=(240, 248, 255, 200), width=6*s)

    # Stenciled "POLICE"
    font = get_font(42*s)
    d.text((cx - 85*s, 160*s), "POLICE", fill=WHITE, font=font, stroke_width=4*s, stroke_fill=INK)

    # Splatters of Neon Pink & Cyan Graffiti on the Shield
    for sx, sy, rad, col in [
        (cx - 50, 260, 38, MAGENTA),
        (cx + 40, 320, 45, CYAN),
        (cx - 20, 360, 28, LIME),
        (cx + 70, 230, 24, GOLD)
    ]:
        ellipse(d, ((sx-rad)*s, (sy-rad)*s, (sx+rad)*s, (sy+rad)*s), col, INK, 3*s)
        # Drip running down
        line(d, [(sx*s, (sy+rad)*s), (sx*s, (sy+rad+28)*s)], col, 6*s)

    # Right Hand holding Shock Baton
    line(d, [(cx + 125*s, 220*s), (cx + 185*s, 140*s)], (200, 210, 225, 255), 10*s)
    ellipse(d, (cx + 175*s, 130*s, cx + 195*s, 150*s), CYAN, INK, 3*s)
    return down(img, 512, 512)

# 25. Police Cyber K-9 Unit (512x512)
def police_k9():
    img, s = canvas(512, 512)
    d = ImageDraw.Draw(img)
    cx, cy = 256*s, 260*s

    # Shadow
    d.ellipse((cx - 160*s, 410*s, cx + 160*s, 455*s), fill=(0, 0, 0, 90))

    # Muscular Dog Body (German Shepherd / Cyber Doberman)
    poly(d, [(cx - 130*s, 220*s), (cx + 60*s, 200*s), (cx + 120*s, 260*s), (cx - 40*s, 310*s), (cx - 140*s, 270*s)], (60, 42, 30, 255), width=10*s)

    # Carbon Fiber Tactical Vest
    poly(d, [(cx - 80*s, 210*s), (cx + 40*s, 200*s), (cx + 50*s, 285*s), (cx - 60*s, 295*s)], CHARCOAL, width=8*s)
    font = get_font(20*s)
    d.text((cx - 40*s, 235*s), "K-9", fill=WHITE, font=font)

    # Front Paws (Athletic forward reach)
    line(d, [(cx + 60*s, 270*s), (cx + 110*s, 410*s)], (60, 42, 30, 255), 18*s)
    line(d, [(cx + 30*s, 275*s), (cx + 70*s, 415*s)], (45, 32, 22, 255), 16*s)
    ellipse(d, (cx + 95*s, 400*s, cx + 125*s, 425*s), INK, INK, 1)
    ellipse(d, (cx + 55*s, 405*s, cx + 85*s, 430*s), INK, INK, 1)

    # Cybernetic Rear Leg (Piston & Chrome Tread)
    line(d, [(cx - 110*s, 260*s), (cx - 150*s, 350*s)], (190, 200, 215, 255), 16*s)
    line(d, [(cx - 150*s, 350*s), (cx - 110*s, 420*s)], (160, 170, 185, 255), 14*s)
    ellipse(d, (cx - 125*s, 410*s, cx - 95*s, 430*s), CHARCOAL, CYAN, 4*s)

    # Head & Pointed Ears
    poly(d, [(cx + 60*s, 190*s), (cx + 160*s, 170*s), (cx + 175*s, 210*s), (cx + 110*s, 240*s)], (75, 52, 38, 255), width=8*s)
    # Pointed Ears
    poly(d, [(cx + 75*s, 185*s), (cx + 90*s, 110*s), (cx + 115*s, 175*s)], (45, 32, 22, 255), width=6*s)
    # Glowing Cyber Snout & Cyan Tracking Collar
    d.rounded_rectangle((cx + 55*s, 195*s, cx + 80*s, 240*s), radius=4*s, fill=CYAN, outline=INK, width=4*s)
    ellipse(d, (cx + 145*s, 180*s, cx + 158*s, 193*s), RED, RED, 1)
    return down(img, 512, 512)

# 26. Breakdancer Performer (512x512)
def breakdancer():
    img, s = canvas(512, 512)
    d = ImageDraw.Draw(img)
    cx, cy = 256*s, 256*s

    # Dynamic B-Boy Freeze Pose (Handstand / Windmill Freeze)
    # Supporting Arm down to ground
    line(d, [(cx, cy + 40*s), (cx - 40*s, cy + 180*s)], INK, 24*s)
    line(d, [(cx, cy + 40*s), (cx - 40*s, cy + 180*s)], (195, 135, 95, 255), 14*s)
    # Hand on floor
    d.rounded_rectangle((cx - 65*s, cy + 175*s, cx - 15*s, cy + 200*s), radius=8*s, fill=CHARCOAL, outline=INK, width=6*s)

    # Inverted Torso (Windbreaker Jacket: Magenta & Cyan)
    poly(d, [(cx - 70*s, cy - 20*s), (cx + 70*s, cy - 40*s), (cx + 40*s, cy + 60*s), (cx - 50*s, cy + 70*s)], MAGENTA, width=10*s)
    poly(d, [(cx - 20*s, cy - 30*s), (cx + 60*s, cy - 40*s), (cx + 35*s, cy + 20*s), (cx - 10*s, cy + 20*s)], CYAN, width=6*s)

    # Head with Beanie & Headphones
    ellipse(d, (cx - 35*s, cy + 70*s, cx + 35*s, cy + 140*s), (195, 135, 95, 255), INK, 7*s)
    d.rounded_rectangle((cx - 38*s, cy + 95*s, cx + 38*s, cy + 148*s), radius=10*s, fill=GOLD, outline=INK, width=6*s)
    # Headphones over beanie
    d.arc((cx - 44*s, cy + 75*s, cx + 44*s, cy + 135*s), 0, 180, fill=WHITE, width=8*s)
    ellipse(d, (cx - 48*s, cy + 95*s, cx - 32*s, cy + 125*s), CHARCOAL, CYAN, 4*s)
    ellipse(d, (cx + 32*s, cy + 95*s, cx + 48*s, cy + 125*s), CHARCOAL, CYAN, 4*s)

    # Legs Kicked Dynamically in Air
    # Leg 1: Kicked high left
    line(d, [(cx - 45*s, cy - 20*s), (cx - 160*s, cy - 140*s)], INK, 26*s)
    line(d, [(cx - 45*s, cy - 20*s), (cx - 160*s, cy - 140*s)], (35, 40, 52, 255), 16*s)
    # Sneaker 1
    d.rounded_rectangle((cx - 195*s, cy - 180*s, cx - 135*s, cy - 125*s), radius=10*s, fill=CYAN, outline=INK, width=6*s)
    # Leg 2: Bent flare right
    line(d, [(cx + 50*s, cy - 35*s), (cx + 140*s, cy - 80*s)], INK, 26*s)
    line(d, [(cx + 140*s, cy - 80*s), (cx + 175*s, cy - 160*s)], (35, 40, 52, 255), 16*s)
    # Sneaker 2
    d.rounded_rectangle((cx + 150*s, cy - 200*s, cx + 205*s, cy - 145*s), radius=10*s, fill=MAGENTA, outline=INK, width=6*s)

    # Spray Can in Free Hand
    ellipse(d, (cx + 80*s, cy + 30*s, cx + 110*s, cy + 60*s), (195, 135, 95, 255), INK, 4*s)
    d.rounded_rectangle((cx + 105*s, cy + 15*s, cx + 130*s, cy + 65*s), radius=4*s, fill=GOLD, outline=INK, width=4*s)
    return down(img, 512, 512)

# 27. Surveillance Blimp Airship (512x256)
def surveillance_blimp():
    img, s = canvas(512, 256)
    d = ImageDraw.Draw(img)
    cx, cy = 256*s, 110*s

    # Searchlight Cone (Translucent volumetric projection)
    poly(d, [(cx - 20*s, cy + 60*s), (cx + 20*s, cy + 60*s), (cx + 160*s, 250*s), (cx - 160*s, 250*s)], (255, 240, 100, 50), outline=None, width=0)

    # Massive Aerostat Hull (Navy / Charcoal with Yellow stripe)
    ellipse(d, (30*s, 30*s, 482*s, 190*s), (28, 34, 46, 255), INK, 10*s)
    # Speed cheatline stripe
    line(d, [(55*s, 110*s), (457*s, 110*s)], GOLD, 12*s)

    # Tail Fins
    poly(d, [(40*s, 110*s), (10*s, 50*s), (65*s, 65*s)], RED, width=6*s)
    poly(d, [(40*s, 110*s), (10*s, 170*s), (65*s, 155*s)], RED, width=6*s)

    # Twin Vector Thruster Ducted Fans
    for fx in [130, 380]:
        ellipse(d, ((fx-24)*s, 150*s, (fx+24)*s, 195*s), CHARCOAL, INK, 6*s)
        line(d, [(fx*s, 155*s), (fx*s, 190*s)], CYAN, 5*s)

    # Observation Gondola & Cockpit
    d.rounded_rectangle((170*s, 160*s, 342*s, 215*s), radius=12*s, fill=(45, 52, 65, 255), outline=INK, width=7*s)
    for wx in [185, 225, 265, 305]:
        d.rounded_rectangle((wx*s, 172*s, (wx+28)*s, 198*s), radius=4*s, fill=CYAN, outline=INK, width=3*s)

    # Electronic Ticker Display on Hull
    d.rounded_rectangle((150*s, 72*s, 362*s, 102*s), radius=6*s, fill=(12, 14, 18, 255), outline=INK, width=4*s)
    font = get_font(20*s)
    d.text((165*s, 76*s), "POLICE 404 // AIR PATROL", fill=RED, font=font)
    return down(img, 512, 256)

# 28. Food Truck Station (512x512)
def food_truck():
    img, s = canvas(512, 512)
    d = ImageDraw.Draw(img)

    # Neon Underglow
    d.ellipse((60*s, 390*s, 452*s, 470*s), fill=PURPLE)

    # Step-Van Truck Body
    body = [(60*s, 140*s), (380*s, 130*s), (450*s, 220*s), (440*s, 410*s), (70*s, 420*s)]
    poly(d, body, (245, 140, 30, 255), outline=INK, width=12*s)

    # Slanted Front Windshield
    poly(d, [(375*s, 150*s), (435*s, 225*s), (380*s, 235*s), (345*s, 160*s)], CYAN, width=6*s)

    # Open Service Serving Hatch
    d.rectangle((100*s, 180*s, 310*s, 320*s), fill=(25, 28, 36, 255), outline=INK, width=8*s)
    # Warm Interior Counter
    d.rectangle((100*s, 270*s, 310*s, 320*s), fill=(255, 230, 160, 255), outline=INK, width=4*s)

    # Striped Canvas Awning (Over serving window)
    poly(d, [(80*s, 150*s), (330*s, 150*s), (310*s, 195*s), (100*s, 195*s)], RED, width=7*s)
    for sx in range(100, 320, 36):
        poly(d, [(sx*s, 150*s), ((sx+18)*s, 150*s), ((sx+10)*s, 195*s), ((sx-8)*s, 195*s)], WHITE, outline=None, width=0)

    # Slap Sticker & Graffiti Tags on Truck Body
    font = get_font(32*s)
    d.text((115*s, 345*s), "404 TACOS", fill=GOLD, font=font, stroke_width=4*s, stroke_fill=INK)
    d.text((290*s, 345*s), "STREET EATS", fill=CYAN, font=get_font(22*s), stroke_width=3*s, stroke_fill=INK)

    # Heavy Chrome Wheels
    for wx in [140, 370]:
        ellipse(d, ((wx-40)*s, 390*s, (wx+40)*s, 450*s), INK, GOLD, 8*s)
        ellipse(d, ((wx-20)*s, 405*s, (wx+20)*s, 435*s), (220, 230, 245, 255), WHITE, 4*s)
    return down(img, 512, 512)

# 29. Street Barrel Fire Warmup (256x256)
def barrel_fire():
    img, s = canvas(256, 256)
    d = ImageDraw.Draw(img)
    cx, cy = 128*s, 175*s

    # Fire Flames Licking into Air (Layered Red, Orange, Yellow)
    poly(d, [(cx-45*s, cy-20*s), (cx-60*s, cy-90*s), (cx-20*s, cy-65*s), (cx, cy-125*s),
             (cx+25*s, cy-75*s), (cx+55*s, cy-100*s), (cx+45*s, cy-20*s)], RED, width=0)
    poly(d, [(cx-35*s, cy-20*s), (cx-40*s, cy-75*s), (cx-10*s, cy-50*s), (cx, cy-95*s),
             (cx+18*s, cy-60*s), (cx+38*s, cy-80*s), (cx+35*s, cy-20*s)], (255, 140, 0, 255), width=0)
    poly(d, [(cx-20*s, cy-20*s), (cx-15*s, cy-55*s), (cx, cy-70*s), (cx+15*s, cy-50*s), (cx+20*s, cy-20*s)], GOLD, width=0)

    # Sparks & Embers
    for ex, ey, col in [(cx-40, cy-110, GOLD), (cx+35, cy-120, (255, 160, 0, 255)), (cx-10, cy-140, GOLD), (cx+20, cy-150, WHITE)]:
        ellipse(d, ((ex-3)*s, (ey-3)*s, (ex+3)*s, (ey+3)*s), col, col, 1)

    # Steel 55-Gallon Drum Barrel
    d.rounded_rectangle(((cx-52*s), (cy-30*s), (cx+52*s), (cy+70*s)), radius=8*s, fill=(70, 75, 85, 255), outline=INK, width=8*s)
    # Barrel Ribs
    for ry in [cy - 5, cy + 30]:
        line(d, [((cx-52)*s, ry*s), ((cx+52)*s, ry*s)], INK, 6*s)

    # Perforated Ventilation Holes Glowing Red/Orange
    for hx in [-32, -10, 12, 32]:
        for hy in [cy + 10, cy + 45]:
            ellipse(d, ((cx+hx-5)*s, (hy-5)*s, (cx+hx+5)*s, (hy+5)*s), (255, 100, 0, 255), INK, 2*s)
    return down(img, 256, 256)

# 30. Authentic Throwup Graffiti (512x256)
def graffiti_throwup():
    img, s = canvas(512, 256)
    d = ImageDraw.Draw(img)
    font = get_font(120*s)
    text = "DOPE"

    # Deep Magenta Drop Shadow
    d.text((45*s, 50*s), text, font=font, fill=MAGENTA, stroke_width=18*s, stroke_fill=INK)
    # Electric Yellow Fill
    d.text((32*s, 35*s), text, font=font, fill=GOLD, stroke_width=12*s, stroke_fill=INK)

    # 5 Long Dripping Paint Trails Running Down
    for dx, length in [(90, 65), (170, 85), (260, 50), (350, 75), (430, 60)]:
        line(d, [(dx*s, 160*s), (dx*s, (160+length)*s)], GOLD, 8*s)
        ellipse(d, ((dx-7)*s, (160+length-4)*s, (dx+7)*s, (160+length+10)*s), GOLD, INK, 3*s)
    return down(img, 512, 256)

# 31. Authentic Bubble Graffiti (512x256)
def graffiti_bubble():
    img, s = canvas(512, 256)
    d = ImageDraw.Draw(img)
    font = get_font(120*s)
    text = "BOYZ"

    # Deep Purple 3D Shadow
    d.text((42*s, 52*s), text, font=font, fill=PURPLE, stroke_width=18*s, stroke_fill=INK)
    # Acid Lime Fill
    d.text((28*s, 36*s), text, font=font, fill=LIME, stroke_width=12*s, stroke_fill=INK)

    # Wet Glossy Reflection Highlights (White pills on top curves)
    for hx, hy in [(70, 55), (175, 55), (280, 55), (380, 55)]:
        d.rounded_rectangle(((hx-18)*s, (hy-6)*s, (hx+18)*s, (hy+6)*s), radius=4*s, fill=WHITE)
    return down(img, 512, 256)

# 32. Authentic Blockbuster Graffiti (512x256)
def graffiti_blockbuster():
    img, s = canvas(512, 256)
    d = ImageDraw.Draw(img)
    font = get_font(125*s)
    text = "CITY"

    # 45-degree Heavy 3D Extrusion
    for off in range(25, 0, -5):
        d.text(((25 + off)*s, (35 + off)*s), text, font=font, fill=CHARCOAL, stroke_width=14*s, stroke_fill=INK)

    # Crisp Crimson Border & Silver Face
    d.text((25*s, 35*s), text, font=font, fill=RED, stroke_width=16*s, stroke_fill=INK)
    d.text((25*s, 35*s), text, font=font, fill=(225, 235, 245, 255), stroke_width=6*s, stroke_fill=WHITE)
    return down(img, 512, 256)

# 33. Authentic Chrome Masterpiece Graffiti (512x256)
def graffiti_chrome():
    img, s = canvas(512, 256)
    d = ImageDraw.Draw(img)
    font = get_font(110*s)
    text = "STREET"

    # Heavy Black Inking Outline
    d.text((25*s, 40*s), text, font=font, fill=INK, stroke_width=18*s, stroke_fill=INK)
    # Sky-Blue Top Half / Bronze Bottom Half Horizon Reflection
    d.text((20*s, 32*s), text, font=font, fill=CYAN, stroke_width=8*s, stroke_fill=BLUE)
    d.text((20*s, 32*s), text, font=font, fill=WHITE)

    # Twinkling 4-Point Star Gleams
    for sx, sy in [(50, 45), (190, 30), (320, 40), (450, 35)]:
        line(d, [(sx*s - 16*s, sy*s), (sx*s + 16*s, sy*s)], WHITE, 5*s)
        line(d, [(sx*s, sy*s - 16*s), (sx*s, sy*s + 16*s)], WHITE, 5*s)
        ellipse(d, ((sx-4)*s, (sy-4)*s, (sx+4)*s, (sy+4)*s), GOLD, GOLD, 1)
    return down(img, 512, 256)

# 34. Multi-Layer Stencil Graffiti (512x256)
def graffiti_stencil():
    img, s = canvas(512, 256)
    d = ImageDraw.Draw(img)
    font = get_font(105*s)
    text = "REBEL 404"

    # Soft Spray Mist Halo
    for r in range(16, 2, -3):
        d.text((25*s, 45*s), text, font=font, fill=(76, 245, 213, 35), stroke_width=(12 + r)*s, stroke_fill=(76, 245, 213, 25))

    # Crisp Stencil Body
    d.text((25*s, 45*s), text, font=font, fill=WHITE, stroke_width=8*s, stroke_fill=INK)

    # Cutout Stencil Bridge Gaps (Black bars slicing through letters)
    for bx in [95, 195, 290, 390]:
        line(d, [(bx*s, 40*s), (bx*s, 160*s)], (0, 0, 0, 0), 10*s)
    return down(img, 512, 256)


def save_all():
    print("Generating Bricko 3D graphics matching set...")
    sprites = {
        # Core 5 original high-res assets
        "lowrider_hydraulic_car.png": lowrider(),
        "swat_van_breaching.png": swat_van(),
        "roller_queen.png": roller_queen(),
        "wildstyle.png": wildstyle(),
        "boombox_prop.png": boombox(),
        "boombox.png": boombox(),

        # The 16 expanded vehicle, prop, equipment, and graffiti assets
        "black_market_van.png": black_market_van(),
        "motorcycle.png": motorcycle(),
        "news_chopper.png": news_chopper(),
        "subway_train_car.png": subway_train_car(),
        "alley_dumpster_prop.png": alley_dumpster(),
        "vending_machine_kiosk.png": vending_machine(),
        "golden_mastery_can.png": golden_can(),
        "paint_turret_prop.png": paint_turret(),
        "heist_cargo_crate.png": heist_crate(),
        "police_roadblock.png": police_roadblock(),
        "dj_turntable_booth.png": dj_booth(),
        "hydrant_geyser.png": hydrant_geyser(),
        "water_tower_mural.png": water_tower(),
        "market_vendor_stall.png": market_stall(),
        "nozzle_tuning_bench.png": tuning_bench(),
        "air_compressor_station.png": air_compressor(),

        # Squads, Bosses, and Street Characters
        "rival_boss_captain.png": rival_captain(),
        "heavy_enforcer.png": heavy_enforcer(),
        "riot_shield_officer.png": riot_shield_officer(),
        "police_k9_unit.png": police_k9(),
        "k9_handler_officer.png": police_k9(),
        "breakdancer_performer.png": breakdancer(),

        # Massive Urban Landmarks & Props
        "surveillance_blimp_airship.png": surveillance_blimp(),
        "food_truck_station.png": food_truck(),
        "barrel_fire_warmup.png": barrel_fire(),

        # Authentic Street Graffiti Burners & Tags
        "throwup.png": graffiti_throwup(),
        "tag_style_throwup.png": graffiti_throwup(),
        "bubble.png": graffiti_bubble(),
        "tag_style_bubble.png": graffiti_bubble(),
        "blockbuster.png": graffiti_blockbuster(),
        "tag_style_blockbuster.png": graffiti_blockbuster(),
        "chrome.png": graffiti_chrome(),
        "tag_style_chrome.png": graffiti_chrome(),
        "stencil.png": graffiti_stencil(),
        "tag_style_stencil.png": graffiti_stencil(),
    }

    for folder in OUT_DIRS:
        folder.mkdir(parents=True, exist_ok=True)
        for name, img in sprites.items():
            target_path = folder / name
            img.save(target_path, "PNG")
            print(f"Saved: {target_path} ({img.width}x{img.height})")

    print(f"\nSuccessfully generated and synchronized all {len(sprites)} graphics across Godot and Unity!")


if __name__ == "__main__":
    save_all()
