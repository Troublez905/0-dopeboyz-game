extends Node2D

const ROAD_X = [310.0, 615.0, 855.0, 1245.0, 1570.0, 1890.0, 2310.0]
const ROAD_Y = [250.0, 520.0, 820.0, 1130.0, 1460.0, 1800.0, 2190.0]
const HQ = Vector2(310, 520)
const SHOP = Vector2(855, 820)
const TURQUOISE = Color("4cf5d5")
const MAGENTA = Color("ff3b77")
const GOLD = Color("ffe06d")
const RED = Color("ff4158")
const PURPLE = Color("b455ff")
const LIME = Color("76ff03")

var navigation = AStar2D.new()
var obstacles: Array[PackedVector2Array] = []
var chapter = 0
var road_x = ROAD_X.duplicate()
var road_y = ROAD_Y.duplicate()
var hq = HQ
var shop = SHOP
var road_rows: Array = []
var bounds = Rect2(270, 220, 2080, 2000)
var walls: Array[Dictionary] = []
var pickups: Array[Dictionary] = []
var sparks: Array[Dictionary] = []
var splatters: Array[Dictionary] = []
var mist_particles: Array[Dictionary] = []
var floating_texts: Array[Dictionary] = []
var shockwaves: Array[Dictionary] = []
var time = 0.0
var rain = true
var markers = true
var map_texture: Texture2D
var nearest = -1
var contract_wall = -1
var city_heat = 0.0
var crew_tag: Texture2D
var rival_tag: Texture2D
var subway_tex: Texture2D
var hydrant_tex: Texture2D
var cone_tex: Texture2D
var swat_tex: Texture2D
var motorcycle_tex: Texture2D
var crater_tex: Texture2D
var bullet_tex: Texture2D

var subway_stations: Array[Vector2] = []
var hydrants: Array[Vector2] = []
var streetlights: Array[Vector2] = []
var parked_vehicles: Array[Dictionary] = []
var craters: Array[Dictionary] = []
var bullet_decals: Array[Dictionary] = []

# Batch 3 — New Visual Systems & Textures
var smoke_clouds: Array[Dictionary] = []
var boombox_active: Vector2 = Vector2(-1, -1)
var roadblocks: Array[Dictionary] = []
var black_market_pos: Vector2 = Vector2(-1, -1)
var heist_target: Vector2 = Vector2(-1, -1)
var neon_puddles: Array[Dictionary] = []
var civilians: Array[Dictionary] = []
var weather_phase: String = "MIDNIGHT"
var eq_bars: Array[float] = [0.4, 0.7, 0.9, 0.6, 0.8, 0.5, 0.95, 0.3]
var drone_pos: Vector2 = Vector2(-1, -1)

var drone_tex: Texture2D
var boombox_tex: Texture2D
var skate_trail_tex: Texture2D
var subway_metro_tex: Texture2D
var smoke_grenade_tex: Texture2D
var smoke_cloud_tex: Texture2D
var roadblock_tex: Texture2D
var black_market_van_tex: Texture2D
var audio_spectrum_hud_tex: Texture2D
var civilian_crowd_tex: Texture2D

var paint_turrets: Array[Dictionary] = []
var paint_mines: Array[Dictionary] = []
var turf_flares: Array[Dictionary] = []
var news_chopper_pos: Vector2 = Vector2(-1, -1)
var k9_units: Array[Dictionary] = []

var turret_tex: Texture2D
var manhole_tex: Texture2D
var mine_tex: Texture2D
var news_chopper_tex: Texture2D
var k9_tex: Texture2D
var water_tower_tex: Texture2D
var electrified_puddle_tex: Texture2D
var flare_tex: Texture2D
var zipline_tex: Texture2D
var foil_tex: Texture2D
var stencil_ui_tex: Texture2D
var rival_boss_tex: Texture2D
var air_compressor_tex: Texture2D
var nozzle_caps_tex: Texture2D
var barrel_fire_tex: Texture2D
var storefront_glass_tex: Texture2D
var jetpack_tex: Texture2D
var block_party_tex: Texture2D
var skateboards_tex: Texture2D
var monument_tex: Texture2D
var badges_tex: Texture2D
var cargo_crate_tex: Texture2D
var flow_gauge_tex: Texture2D
var mega_mural_tex: Texture2D

var glider_tex: Texture2D
var swat_van_tex: Texture2D
var subway_train_tex: Texture2D
var slowmo_fx_tex: Texture2D
var grapple_tex: Texture2D
var dj_booth_tex: Texture2D
var flamethrower_tex: Texture2D
var spotter_drone_tex: Texture2D
var stamps_sheet_tex: Texture2D
var lightning_tex: Texture2D
var food_truck_tex: Texture2D
var k9_handler_tex: Texture2D
var jackets_sheet_tex: Texture2D
var helipad_tex: Texture2D
var chameleon_can_tex: Texture2D
var hologram_decoy_tex: Texture2D
var skate_bowl_tex: Texture2D
var cluster_bomb_tex: Texture2D
var watchtower_tex: Texture2D
var tuning_bench_tex: Texture2D
var breakdancer_tex: Texture2D
var bubble_shield_tex: Texture2D
var command_trailer_tex: Texture2D
var roller_derby_tex: Texture2D
var park_fountain_tex: Texture2D

# Batch 6 Graphic Textures (Updates 07)
var lowrider_car_tex: Texture2D
var neon_underglow_tex: Texture2D
var spike_strip_tex: Texture2D
var blimp_tex: Texture2D
var rival_banner_tex: Texture2D
var vending_kiosk_tex: Texture2D
var subway_grate_tex: Texture2D
var riot_shield_tex: Texture2D
var soundquake_tex: Texture2D
var blackbook_tex: Texture2D
var pool_table_tex: Texture2D
var cctv_tex: Texture2D
var exhaust_plume_tex: Texture2D
var metro_tracks_tex: Texture2D
var market_stall_tex: Texture2D
var camo_cloak_tex: Texture2D
var drone_swarm_tex: Texture2D
var dumpster_tex: Texture2D
var radio_jammer_tex: Texture2D
var golden_can_tex: Texture2D

var lowrider_pos: Vector2 = Vector2(615, 820)
var lowrider_bounce: float = 0.0
var vending_pos: Vector2 = Vector2(1245, 1130)
var subway_grate_pos: Vector2 = Vector2(855, 520)
var pool_table_pos: Vector2 = Vector2(310, 820)
var cctv_cameras: Array[Dictionary] = [
	{"pos": Vector2(855, 820), "angle": 0.5, "disabled": false},
	{"pos": Vector2(1570, 1130), "angle": -1.2, "disabled": false}
]
var market_stall_pos: Vector2 = Vector2(1890, 520)
var dumpsters: Array[Vector2] = [Vector2(450, 520), Vector2(1730, 820), Vector2(1100, 1460)]
var radio_jammer_pos: Vector2 = Vector2(1570, 1800)
var blimp_pos: Vector2 = Vector2(400, 300)
var rival_banners: Array[Vector2] = [Vector2(745, 492), Vector2(1730, 1102), Vector2(1080, 1772)]
var golden_can_unlocked: bool = false
var soundquake_waves: Array[Dictionary] = []
var spike_strips: Array[Dictionary] = []

func _ready() -> void:
	if ResourceLoader.exists("res://assets/tags/crew.svg"):
		crew_tag = load("res://assets/tags/crew.svg")
	if ResourceLoader.exists("res://assets/tags/rival.svg"):
		rival_tag = load("res://assets/tags/rival.svg")
	if crew_tag == null:
		crew_tag = _create_tag_texture(TURQUOISE)
	if rival_tag == null:
		rival_tag = _create_tag_texture(RED)
		
	# Batch 2 Textures
	if ResourceLoader.exists("res://assets/subway_entrance.png"):
		subway_tex = load("res://assets/subway_entrance.png")
	if ResourceLoader.exists("res://assets/hydrant_geyser.png"):
		hydrant_tex = load("res://assets/hydrant_geyser.png")
	if ResourceLoader.exists("res://assets/streetlight_cone.png"):
		cone_tex = load("res://assets/streetlight_cone.png")
	if ResourceLoader.exists("res://assets/swat_truck.png"):
		swat_tex = load("res://assets/swat_truck.png")
	if ResourceLoader.exists("res://assets/motorcycle.png"):
		motorcycle_tex = load("res://assets/motorcycle.png")
	if ResourceLoader.exists("res://assets/paint_crater.png"):
		crater_tex = load("res://assets/paint_crater.png")
	if ResourceLoader.exists("res://assets/bullet_splatters.png"):
		bullet_tex = load("res://assets/bullet_splatters.png")
		
	# Batch 3 Graphic Textures
	if ResourceLoader.exists("res://assets/drone_scout.png"):
		drone_tex = load("res://assets/drone_scout.png")
	if ResourceLoader.exists("res://assets/boombox_prop.png"):
		boombox_tex = load("res://assets/boombox_prop.png")
	if ResourceLoader.exists("res://assets/skate_trail.png"):
		skate_trail_tex = load("res://assets/skate_trail.png")
	if ResourceLoader.exists("res://assets/subway_entrance_metro.png"):
		subway_metro_tex = load("res://assets/subway_entrance_metro.png")
	if ResourceLoader.exists("res://assets/smoke_grenade_prop.png"):
		smoke_grenade_tex = load("res://assets/smoke_grenade_prop.png")
	if ResourceLoader.exists("res://assets/smoke_particle_cloud.png"):
		smoke_cloud_tex = load("res://assets/smoke_particle_cloud.png")
	if ResourceLoader.exists("res://assets/police_roadblock.png"):
		roadblock_tex = load("res://assets/police_roadblock.png")
	if ResourceLoader.exists("res://assets/black_market_van.png"):
		black_market_van_tex = load("res://assets/black_market_van.png")
	if ResourceLoader.exists("res://assets/audio_spectrum_hud.png"):
		audio_spectrum_hud_tex = load("res://assets/audio_spectrum_hud.png")
	if ResourceLoader.exists("res://assets/civilian_crowd.png"):
		civilian_crowd_tex = load("res://assets/civilian_crowd.png")
	if ResourceLoader.exists("res://assets/crew_classes_badges.png"):
		badges_tex = load("res://assets/crew_classes_badges.png")
	if ResourceLoader.exists("res://assets/heist_cargo_crate.png"):
		cargo_crate_tex = load("res://assets/heist_cargo_crate.png")
	if ResourceLoader.exists("res://assets/flow_rhythm_gauge.png"):
		flow_gauge_tex = load("res://assets/flow_rhythm_gauge.png")
	if ResourceLoader.exists("res://assets/billboard_mega_mural.png"):
		mega_mural_tex = load("res://assets/billboard_mega_mural.png")
		
	# Batch 4 Graphic Textures
	if ResourceLoader.exists("res://assets/paint_turret_prop.png"):
		turret_tex = load("res://assets/paint_turret_prop.png")
	if ResourceLoader.exists("res://assets/manhole_sewer_passage.png"):
		manhole_tex = load("res://assets/manhole_sewer_passage.png")
	if ResourceLoader.exists("res://assets/paint_mine_trap.png"):
		mine_tex = load("res://assets/paint_mine_trap.png")
	if ResourceLoader.exists("res://assets/news_chopper.png"):
		news_chopper_tex = load("res://assets/news_chopper.png")
	if ResourceLoader.exists("res://assets/police_k9_unit.png"):
		k9_tex = load("res://assets/police_k9_unit.png")
	if ResourceLoader.exists("res://assets/water_tower_mural.png"):
		water_tower_tex = load("res://assets/water_tower_mural.png")
	if ResourceLoader.exists("res://assets/electrified_puddle.png"):
		electrified_puddle_tex = load("res://assets/electrified_puddle.png")
	if ResourceLoader.exists("res://assets/turf_flare_marker.png"):
		flare_tex = load("res://assets/turf_flare_marker.png")
	if ResourceLoader.exists("res://assets/zipline_grind_cable.png"):
		zipline_tex = load("res://assets/zipline_grind_cable.png")
	if ResourceLoader.exists("res://assets/thermal_foil_stealth.png"):
		foil_tex = load("res://assets/thermal_foil_stealth.png")
	if ResourceLoader.exists("res://assets/stencil_workshop_ui.png"):
		stencil_ui_tex = load("res://assets/stencil_workshop_ui.png")
	if ResourceLoader.exists("res://assets/rival_boss_captain.png"):
		rival_boss_tex = load("res://assets/rival_boss_captain.png")
	if ResourceLoader.exists("res://assets/air_compressor_station.png"):
		air_compressor_tex = load("res://assets/air_compressor_station.png")
	if ResourceLoader.exists("res://assets/nozzle_caps_sheet.png"):
		nozzle_caps_tex = load("res://assets/nozzle_caps_sheet.png")
	if ResourceLoader.exists("res://assets/barrel_fire_warmup.png"):
		barrel_fire_tex = load("res://assets/barrel_fire_warmup.png")
	if ResourceLoader.exists("res://assets/storefront_glass.png"):
		storefront_glass_tex = load("res://assets/storefront_glass.png")
	if ResourceLoader.exists("res://assets/jetpack_thruster.png"):
		jetpack_tex = load("res://assets/jetpack_thruster.png")
	if ResourceLoader.exists("res://assets/block_party_crowd.png"):
		block_party_tex = load("res://assets/block_party_crowd.png")
	if ResourceLoader.exists("res://assets/skateboards_sheet.png"):
		skateboards_tex = load("res://assets/skateboards_sheet.png")
	if ResourceLoader.exists("res://assets/city_hall_monument.png"):
		monument_tex = load("res://assets/city_hall_monument.png")
		
	# Batch 5 Graphic Textures
	if ResourceLoader.exists("res://assets/glider_wingsuit.png"):
		glider_tex = load("res://assets/glider_wingsuit.png")
	if ResourceLoader.exists("res://assets/swat_van_breaching.png"):
		swat_van_tex = load("res://assets/swat_van_breaching.png")
	if ResourceLoader.exists("res://assets/subway_train_car.png"):
		subway_train_tex = load("res://assets/subway_train_car.png")
	if ResourceLoader.exists("res://assets/slowmo_adrenaline_fx.png"):
		slowmo_fx_tex = load("res://assets/slowmo_adrenaline_fx.png")
	if ResourceLoader.exists("res://assets/grappling_hook_launcher.png"):
		grapple_tex = load("res://assets/grappling_hook_launcher.png")
	if ResourceLoader.exists("res://assets/dj_turntable_booth.png"):
		dj_booth_tex = load("res://assets/dj_turntable_booth.png")
	if ResourceLoader.exists("res://assets/aerosol_flamethrower_fire.png"):
		flamethrower_tex = load("res://assets/aerosol_flamethrower_fire.png")
	if ResourceLoader.exists("res://assets/recon_spotter_drone.png"):
		spotter_drone_tex = load("res://assets/recon_spotter_drone.png")
	if ResourceLoader.exists("res://assets/stencil_stamps_sheet.png"):
		stamps_sheet_tex = load("res://assets/stencil_stamps_sheet.png")
	if ResourceLoader.exists("res://assets/thunderstorm_lightning.png"):
		lightning_tex = load("res://assets/thunderstorm_lightning.png")
	if ResourceLoader.exists("res://assets/food_truck_station.png"):
		food_truck_tex = load("res://assets/food_truck_station.png")
	if ResourceLoader.exists("res://assets/k9_handler_officer.png"):
		k9_handler_tex = load("res://assets/k9_handler_officer.png")
	if ResourceLoader.exists("res://assets/crew_jackets_sheet.png"):
		jackets_sheet_tex = load("res://assets/crew_jackets_sheet.png")
	if ResourceLoader.exists("res://assets/rooftop_helipad.png"):
		helipad_tex = load("res://assets/rooftop_helipad.png")
	if ResourceLoader.exists("res://assets/chameleon_rainbow_can.png"):
		chameleon_can_tex = load("res://assets/chameleon_rainbow_can.png")
	if ResourceLoader.exists("res://assets/decoy_hologram_emitter.png"):
		hologram_decoy_tex = load("res://assets/decoy_hologram_emitter.png")
	if ResourceLoader.exists("res://assets/skate_park_bowl.png"):
		skate_bowl_tex = load("res://assets/skate_park_bowl.png")
	if ResourceLoader.exists("res://assets/cluster_paint_bomb.png"):
		cluster_bomb_tex = load("res://assets/cluster_paint_bomb.png")
	if ResourceLoader.exists("res://assets/spotlight_watchtower.png"):
		watchtower_tex = load("res://assets/spotlight_watchtower.png")
	if ResourceLoader.exists("res://assets/nozzle_tuning_bench.png"):
		tuning_bench_tex = load("res://assets/nozzle_tuning_bench.png")
	if ResourceLoader.exists("res://assets/breakdancer_performer.png"):
		breakdancer_tex = load("res://assets/breakdancer_performer.png")
	if ResourceLoader.exists("res://assets/bubble_shield_barrier.png"):
		bubble_shield_tex = load("res://assets/bubble_shield_barrier.png")
	if ResourceLoader.exists("res://assets/rival_command_trailer.png"):
		command_trailer_tex = load("res://assets/rival_command_trailer.png")
	if ResourceLoader.exists("res://assets/roller_derby_pursuit.png"):
		roller_derby_tex = load("res://assets/roller_derby_pursuit.png")
	if ResourceLoader.exists("res://assets/park_fountain_takeover.png"):
		park_fountain_tex = load("res://assets/park_fountain_takeover.png")

	# Batch 6 Graphic Textures (Updates 07)
	if ResourceLoader.exists("res://assets/lowrider_hydraulic_car.png"):
		lowrider_car_tex = load("res://assets/lowrider_hydraulic_car.png")
	if ResourceLoader.exists("res://assets/neon_underglow_fx.png"):
		neon_underglow_tex = load("res://assets/neon_underglow_fx.png")
	if ResourceLoader.exists("res://assets/police_spike_strip.png"):
		spike_strip_tex = load("res://assets/police_spike_strip.png")
	if ResourceLoader.exists("res://assets/surveillance_blimp_airship.png"):
		blimp_tex = load("res://assets/surveillance_blimp_airship.png")
	if ResourceLoader.exists("res://assets/rival_turf_banner.png"):
		rival_banner_tex = load("res://assets/rival_turf_banner.png")
	if ResourceLoader.exists("res://assets/vending_machine_kiosk.png"):
		vending_kiosk_tex = load("res://assets/vending_machine_kiosk.png")
	if ResourceLoader.exists("res://assets/subway_grate_updraft.png"):
		subway_grate_tex = load("res://assets/subway_grate_updraft.png")
	if ResourceLoader.exists("res://assets/riot_shield_officer.png"):
		riot_shield_tex = load("res://assets/riot_shield_officer.png")
	if ResourceLoader.exists("res://assets/soundquake_bass_blast.png"):
		soundquake_tex = load("res://assets/soundquake_bass_blast.png")
	if ResourceLoader.exists("res://assets/blackbook_sticker_album.png"):
		blackbook_tex = load("res://assets/blackbook_sticker_album.png")
	if ResourceLoader.exists("res://assets/rooftop_lounge_props.png"):
		pool_table_tex = load("res://assets/rooftop_lounge_props.png")
	if ResourceLoader.exists("res://assets/cctv_security_camera.png"):
		cctv_tex = load("res://assets/cctv_security_camera.png")
	if ResourceLoader.exists("res://assets/paint_exhaust_plume.png"):
		exhaust_plume_tex = load("res://assets/paint_exhaust_plume.png")
	if ResourceLoader.exists("res://assets/metro_rail_tracks.png"):
		metro_tracks_tex = load("res://assets/metro_rail_tracks.png")
	if ResourceLoader.exists("res://assets/market_vendor_stall.png"):
		market_stall_tex = load("res://assets/market_vendor_stall.png")
	if ResourceLoader.exists("res://assets/camo_stealth_cloak.png"):
		camo_cloak_tex = load("res://assets/camo_stealth_cloak.png")
	if ResourceLoader.exists("res://assets/micro_drone_swarm.png"):
		drone_swarm_tex = load("res://assets/micro_drone_swarm.png")
	if ResourceLoader.exists("res://assets/alley_dumpster_prop.png"):
		dumpster_tex = load("res://assets/alley_dumpster_prop.png")
	if ResourceLoader.exists("res://assets/police_radio_jammer.png"):
		radio_jammer_tex = load("res://assets/police_radio_jammer.png")
	if ResourceLoader.exists("res://assets/golden_mastery_can.png"):
		golden_can_tex = load("res://assets/golden_mastery_can.png")
		
	subway_stations = [Vector2(855, 1130), Vector2(1570, 520)]
	hydrants = [Vector2(615, 820), Vector2(1245, 1460), Vector2(1890, 820)]
	streetlights = [
		Vector2(310, 250), Vector2(855, 250), Vector2(1570, 250), Vector2(2310, 250),
		Vector2(615, 820), Vector2(1245, 1130), Vector2(1890, 1460),
		Vector2(615, 1800), Vector2(1570, 1800), Vector2(2310, 2190)
	]
	parked_vehicles = [
		{"pos": Vector2(1890, 1130), "type": "swat", "rot": -0.2},
		{"pos": Vector2(615, 520), "type": "motorcycle", "rot": 0.4}
	]
	# Seed neon puddles on road intersections
	var puddle_spots = [Vector2(615, 520), Vector2(1245, 820), Vector2(855, 1130),
		Vector2(1570, 1130), Vector2(615, 1460), Vector2(1890, 1460),
		Vector2(1245, 1800), Vector2(2070, 820)]
	for ps in puddle_spots:
		neon_puddles.append({"pos": ps + Vector2(randf_range(-30, 30), randf_range(-20, 20)),
			"radius": randf_range(18.0, 38.0), "color": [TURQUOISE, MAGENTA, GOLD, PURPLE].pick_random(),
			"ripple": randf() * TAU})
	if chapter == 1:
		road_x = [530.0, 990.0, 1515.0, 2000.0]
		road_y = [470.0, 904.0, 1335.0, 1775.0, 2250.0]
		road_rows = [[530.0, 990.0, 1515.0, 2000.0], [490.0, 980.0, 1520.0, 2010.0], [465.0, 965.0, 1520.0, 2025.0], [440.0, 960.0, 1520.0, 2050.0], [400.0, 950.0, 1530.0, 2080.0]]
		hq = Vector2(530, 470)
		shop = Vector2(980, 904)
		bounds = Rect2(350, 435, 1790, 1855)
	else:
		for y in road_y.size():
			road_rows.append(road_x.duplicate())
	map_texture = load("res://assets/city_day.png" if chapter == 1 else "res://assets/city.png")
	for y in road_y.size():
		for x in road_x.size():
			var id = y * road_x.size() + x
			navigation.add_point(id, road_point(x, y))
			if x > 0:
				navigation.connect_points(id - 1, id)
			if y > 0:
				navigation.connect_points(id - road_x.size(), id)
	for y in road_y.size() - 1:
		for x in road_x.size() - 1:
			var polygon = PackedVector2Array([road_point(x, y) + Vector2(40, 40), road_point(x + 1, y) + Vector2(-40, 40), road_point(x + 1, y + 1) + Vector2(-40, -40), road_point(x, y + 1) + Vector2(40, -40)])
			obstacles.append(polygon)
			var body = StaticBody2D.new()
			var collision = CollisionPolygon2D.new()
			collision.polygon = polygon
			body.add_child(collision)
			add_child(body)
	if chapter == 1:
		for y in road_y.size() - 1:
			for side in [0, road_x.size() - 1]:
				var edge = 0.0 if side == 0 else 2508.0
				var margin = -40.0 if side == 0 else 40.0
				var polygon = PackedVector2Array([Vector2(edge, road_y[y]), road_point(side, y) + Vector2(margin, 0), road_point(side, y + 1) + Vector2(margin, 0), Vector2(edge, road_y[y + 1])])
				obstacles.append(polygon)
				var body = StaticBody2D.new()
				var collision = CollisionPolygon2D.new()
				collision.polygon = polygon
				body.add_child(collision)
				add_child(body)
	var spots = [Vector2(450, 492), Vector2(745, 492), Vector2(1100, 792), Vector2(1400, 492), Vector2(1730, 1102), Vector2(2070, 792), Vector2(450, 1432), Vector2(1080, 1772), Vector2(1730, 1772), Vector2(2070, 2162), Vector2(1100, 2162), Vector2(2070, 1432)]
	var names = ["CANAL CORNER", "INK ALLEY", "MARKET SHUTTERS", "404 CROSSING", "COURT WALL", "STATIC YARD", "ROOTZ MURAL", "RAILCUT", "BASS BLOCK", "ROLLER HEIGHTS", "CROWN WALL", "NIGHT MARKET"]
	if chapter == 1:
		spots.clear()
		names = ["CAP ALLEY", "BASS BLOCK", "GLITCH BORN", "FOUNTAIN ROW", "STICKER TUNNEL", "CROWN CROSSING", "ROLLER HEIGHTS", "ROOTZ YARD", "WHITE SECTOR", "STAY UP", "BURN COLORS", "MAKE YOUR MARK"]
		for y in 4:
			for x in 3:
				spots.append(road_point(x, y).lerp(road_point(x + 1, y), 0.5) + Vector2(0, -28))
	for i in spots.size():
		walls.append({"pos": spots[i], "owner": "none", "progress": 0.0, "enemy_progress": 0.0, "size": 1 + i % 3, "name": names[i], "drips": _generate_drips(1 + i % 3)})
	for i in 24:
		var x = i % (road_x.size() - 1)
		var y = int(i / float(road_x.size() - 1)) % road_y.size()
		var fraction = 0.25 if i < (road_x.size() - 1) * road_y.size() else 0.75
		pickups.append({"pos": road_point(x, y).lerp(road_point(x + 1, y), fraction), "kind": ["paint", "ammo", "health", "paint", "recruit", "weapon"][i % 6], "timer": 0.0})

func _generate_drips(wall_size: int) -> Array[Dictionary]:
	var result: Array[Dictionary] = []
	var count = 4 + wall_size * 2
	for k in count:
		result.append({
			"offset_x": randf_range(-0.45, 0.45),
			"length": randf_range(8.0, 24.0 + float(wall_size) * 6.0),
			"width": randf_range(2.0, 4.0),
			"delay": randf_range(0.0, 0.6)
		})
	return result

func road_point(x: int, y: int) -> Vector2:
	return Vector2(road_rows[y][x], road_y[y])

func clamp_point(point: Vector2) -> Vector2:
	return point.clamp(bounds.position, bounds.end)

func blocked(point: Vector2) -> bool:
	for polygon in obstacles:
		if Geometry2D.is_point_in_polygon(point, polygon):
			return true
	return false

func clear_line(a: Vector2, b: Vector2) -> bool:
	for polygon in obstacles:
		if Geometry2D.is_point_in_polygon(a, polygon) or Geometry2D.is_point_in_polygon(b, polygon):
			return false
		for i in polygon.size():
			if Geometry2D.segment_intersects_segment(a, b, polygon[i], polygon[(i + 1) % polygon.size()]) != null:
				return false
	return true

func path_between(a: Vector2, b: Vector2) -> PackedVector2Array:
	if clear_line(a, b):
		return PackedVector2Array([b])
	var end = visible_node(b)
	var best_path = PackedVector2Array()
	var best_cost = INF
	for start in navigation.get_point_ids():
		var position = navigation.get_point_position(start)
		if not clear_line(a, position):
			continue
		var candidate = navigation.get_point_path(start, end)
		var cost = a.distance_to(position)
		for i in candidate.size() - 1:
			cost += candidate[i].distance_to(candidate[i + 1])
		if cost < best_cost:
			best_cost = cost
			best_path = candidate
	best_path.append(b)
	return best_path

func visible_node(point: Vector2) -> int:
	var best = navigation.get_closest_point(point)
	var distance = INF
	for id in navigation.get_point_ids():
		var p = navigation.get_point_position(id)
		var d = point.distance_squared_to(p)
		if d < distance and clear_line(point, p):
			distance = d
			best = id
	return best

func burst(point: Vector2, color: Color, count: int = 18) -> void:
	for i in count:
		sparks.append({"pos": point, "vel": Vector2.from_angle(randf() * TAU) * randf_range(20, 160), "life": randf_range(0.25, 0.85), "color": color, "size": randf_range(2.0, 4.5)})

func spray_cloud(from_pos: Vector2, to_pos: Vector2, color: Color, count: int = 4) -> void:
	var dir = from_pos.direction_to(to_pos)
	for i in count:
		var spread = dir.rotated(randf_range(-0.45, 0.45))
		mist_particles.append({
			"pos": from_pos + dir * randf_range(5, 15),
			"vel": spread * randf_range(40, 120),
			"life": randf_range(0.25, 0.55),
			"max_life": 0.55,
			"color": color,
			"radius": randf_range(4.0, 9.0)
		})

func add_splatter(pos: Vector2, color: Color, count: int = 3) -> void:
	for i in count:
		splatters.append({
			"pos": pos + Vector2(randf_range(-14, 14), randf_range(-10, 10)),
			"color": Color(color, randf_range(0.35, 0.7)),
			"radius": randf_range(3.0, 8.0),
			"life": 30.0
		})
	if splatters.size() > 80:
		splatters.pop_front()

func add_floating_text(text: String, pos: Vector2, color: Color = Color.WHITE, scale: float = 1.0) -> void:
	floating_texts.append({
		"text": text,
		"pos": pos,
		"vel": Vector2(randf_range(-15, 15), -45.0),
		"color": color,
		"scale": scale,
		"life": 1.2,
		"max_life": 1.2
	})

func add_bullet_impact(pos: Vector2) -> void:
	bullet_decals.append({"pos": pos, "life": 25.0, "rot": randf_range(0, TAU)})
	if bullet_decals.size() > 40:
		bullet_decals.pop_front()
	burst(pos, GOLD, 6)

func add_smoke_cloud(pos: Vector2, duration: float = 4.0) -> void:
	smoke_clouds.append({
		"pos": pos, "life": duration, "max_life": duration,
		"radius": 80.0, "target_radius": 140.0
	})
	for i in 20:
		var angle = (float(i) / 20.0) * TAU
		mist_particles.append({
			"pos": pos, "vel": Vector2.from_angle(angle) * randf_range(30, 90),
			"life": duration * 0.7, "max_life": duration * 0.7,
			"color": Color(TURQUOISE, 0.7), "radius": randf_range(14.0, 28.0)
		})
	burst(pos, MAGENTA, 20)

func add_paint_bomb(pos: Vector2, color: Color) -> void:
	shockwaves.append({"pos": pos, "radius": 10.0, "max_radius": 180.0, "life": 0.45, "max_life": 0.45, "color": color})
	craters.append({"pos": pos, "life": 45.0, "color": color})
	if craters.size() > 20:
		craters.pop_front()
	burst(pos, color, 32)
	add_splatter(pos, color, 8)
	for i in 12:
		var angle = (float(i) / 12.0) * TAU
		var dir = Vector2.from_angle(angle)
		mist_particles.append({
			"pos": pos,
			"vel": dir * randf_range(120, 220),
			"life": 0.6,
			"max_life": 0.6,
			"color": color,
			"radius": randf_range(8.0, 16.0)
		})

func tick(delta: float) -> void:
	time += delta
	for pickup in pickups:
		pickup.timer = maxf(0, pickup.timer - delta)
	for i in range(sparks.size() - 1, -1, -1):
		sparks[i].life -= delta
		sparks[i].pos += sparks[i].vel * delta
		sparks[i].vel *= 0.94
		if sparks[i].life <= 0:
			sparks.remove_at(i)
	for i in range(mist_particles.size() - 1, -1, -1):
		mist_particles[i].life -= delta
		mist_particles[i].pos += mist_particles[i].vel * delta
		mist_particles[i].vel *= 0.92
		if mist_particles[i].life <= 0:
			mist_particles.remove_at(i)
	for i in range(shockwaves.size() - 1, -1, -1):
		shockwaves[i].life -= delta
		var progress = 1.0 - (shockwaves[i].life / shockwaves[i].max_life)
		shockwaves[i].radius = lerpf(10.0, shockwaves[i].max_radius, progress)
		if shockwaves[i].life <= 0:
			shockwaves.remove_at(i)
	for i in range(floating_texts.size() - 1, -1, -1):
		floating_texts[i].life -= delta
		floating_texts[i].pos += floating_texts[i].vel * delta
		floating_texts[i].vel.y *= 0.96
		if floating_texts[i].life <= 0:
			floating_texts.remove_at(i)
	for i in range(splatters.size() - 1, -1, -1):
		splatters[i].life -= delta
		if splatters[i].life <= 0:
			splatters.remove_at(i)
	for i in range(craters.size() - 1, -1, -1):
		craters[i].life -= delta
		if craters[i].life <= 0:
			craters.remove_at(i)
	for i in range(bullet_decals.size() - 1, -1, -1):
		bullet_decals[i].life -= delta
		if bullet_decals[i].life <= 0:
			bullet_decals.remove_at(i)
	# Smoke clouds
	for i in range(smoke_clouds.size() - 1, -1, -1):
		smoke_clouds[i].life -= delta
		smoke_clouds[i].radius = lerpf(smoke_clouds[i].radius, smoke_clouds[i].target_radius, delta * 1.4)
		if smoke_clouds[i].life <= 0:
			smoke_clouds.remove_at(i)
	# EQ bar animation
	for i in eq_bars.size():
		eq_bars[i] = clampf(eq_bars[i] + randf_range(-0.25, 0.25) * delta * 8.0, 0.1, 1.0)
		
	# Batch 6 Tick Updates
	blimp_pos.x += delta * 20.0
	if blimp_pos.x > bounds.end.x + 200:
		blimp_pos.x = bounds.position.x - 200
	lowrider_bounce = maxf(0.0, lowrider_bounce - delta * 3.0)
	for cam in cctv_cameras:
		if not cam.get("disabled", false):
			cam.angle = sin(time * 1.5 + cam.pos.x) * 0.8
	for i in range(soundquake_waves.size() - 1, -1, -1):
		soundquake_waves[i].life -= delta
		var progress = 1.0 - (soundquake_waves[i].life / soundquake_waves[i].max_life)
		soundquake_waves[i].radius = lerpf(20.0, soundquake_waves[i].max_radius, progress)
		if soundquake_waves[i].life <= 0:
			soundquake_waves.remove_at(i)
	queue_redraw()

func trigger_lowrider_hop() -> void:
	lowrider_bounce = 1.0
	shockwaves.append({"pos": lowrider_pos, "radius": 15.0, "max_radius": 220.0, "life": 0.5, "max_life": 0.5, "color": MAGENTA})
	burst(lowrider_pos, MAGENTA, 30)

func trigger_soundquake(pos: Vector2) -> void:
	soundquake_waves.append({"pos": pos, "radius": 20.0, "max_radius": 280.0, "life": 0.6, "max_life": 0.6})
	burst(pos, GOLD, 32)
	burst(pos, TURQUOISE, 20)

func _draw() -> void:
	if map_texture == null:
		return
	
	# Weather phase ambient tint overlay
	var weather_col: Color
	match weather_phase:
		"MIDNIGHT": weather_col = Color(0.05, 0.05, 0.15, 0.18)
		"DAWN": weather_col = Color(0.05, 0.12, 0.2, 0.12)
		"DUSK": weather_col = Color(0.28, 0.12, 0.04, 0.14)
		_: weather_col = Color(0, 0, 0, 0)
	draw_texture_rect(map_texture, Rect2(0, 0, 2508, 2508), false)
	draw_rect(Rect2(0, 0, 2508, 2508), weather_col)
	
	# Neon puddle reflections
	for pd in neon_puddles:
		var ripple_r = pd.radius * (1.0 + 0.12 * sin(pd.ripple))
		draw_set_transform(pd.pos, 0.0, Vector2(1.0, 0.3))
		draw_circle(Vector2.ZERO, ripple_r, Color(pd.color.r, pd.color.g, pd.color.b, 0.22))
		draw_arc(Vector2.ZERO, ripple_r * 0.7, 0, TAU, 24, Color(pd.color, 0.45), 1.8, true)
		draw_set_transform(Vector2.ZERO, 0.0, Vector2.ONE)
	
	# Paint Craters
	for cr in craters:
		var alpha = clampf(cr.life / 45.0, 0.0, 1.0)
		if crater_tex != null:
			var c_size = Vector2(160, 160)
			draw_texture_rect(crater_tex, Rect2(cr.pos - c_size * 0.5, c_size), false, Color(1, 1, 1, alpha * 0.85))
		else:
			draw_circle(cr.pos, 50, Color(cr.color.r, cr.color.g, cr.color.b, alpha * 0.3))
	
	# Ground Splatters
	for s in splatters:
		draw_circle(s.pos, s.radius, s.color)
		draw_circle(s.pos + Vector2(s.radius * 0.4, -s.radius * 0.3), s.radius * 0.35, s.color)
		
	# Bullet Decals on Pavement
	for bd in bullet_decals:
		var alpha = clampf(bd.life / 25.0, 0.0, 1.0)
		if bullet_tex != null:
			var b_size = Vector2(36, 36)
			draw_set_transform(bd.pos, bd.rot, Vector2.ONE)
			draw_texture_rect(bullet_tex, Rect2(-b_size * 0.5, b_size), false, Color(1, 1, 1, alpha * 0.8))
			draw_set_transform(Vector2.ZERO, 0.0, Vector2.ONE)
		else:
			draw_circle(bd.pos, 3, Color(0.1, 0.1, 0.1, alpha * 0.9))
			
	# Subway Entrance Stations
	for sub_pos in subway_stations:
		if subway_tex != null:
			var s_size = Vector2(96, 72)
			draw_texture_rect(subway_tex, Rect2(sub_pos - s_size * 0.5, s_size), false)
		else:
			draw_rect(Rect2(sub_pos - Vector2(36, 24), Vector2(72, 48)), Color("161b22"))
			draw_rect(Rect2(sub_pos - Vector2(36, 24), Vector2(72, 48)), TURQUOISE, false, 2.0)
		draw_string(ThemeDB.fallback_font, sub_pos + Vector2(-36, 44), "METRO 404", HORIZONTAL_ALIGNMENT_CENTER, -1, 11, TURQUOISE)
		
	# Fire Hydrants
	for hyd_pos in hydrants:
		if hydrant_tex != null:
			var h_size = Vector2(40, 52)
			draw_texture_rect(hydrant_tex, Rect2(hyd_pos - h_size * 0.5, h_size), false)
		else:
			draw_circle(hyd_pos, 8, RED)
			draw_circle(hyd_pos, 4, GOLD)
		# Occasional water spray mist
		if randf() < 0.08:
			burst(hyd_pos + Vector2(0, -10), Color(0.6, 0.85, 1.0, 0.6), 2)
			
	# Parked City Vehicles (SWAT Truck & Motorcycle)
	for veh in parked_vehicles:
		var v_pos: Vector2 = veh.pos
		var v_type: String = veh.type
		var v_rot: float = veh.rot
		draw_set_transform(v_pos, v_rot, Vector2.ONE)
		if v_type == "swat" and swat_tex != null:
			var swat_size = Vector2(110, 60)
			draw_texture_rect(swat_tex, Rect2(-swat_size * 0.5, swat_size), false)
		elif v_type == "motorcycle" and motorcycle_tex != null:
			var moto_size = Vector2(56, 32)
			draw_texture_rect(motorcycle_tex, Rect2(-moto_size * 0.5, moto_size), false)
		draw_set_transform(Vector2.ZERO, 0.0, Vector2.ONE)
	
	# Walls and Graffiti Pieces
	for i in walls.size():
		var wall = walls[i]
		var point: Vector2 = wall.pos
		var color = TURQUOISE if wall.owner == "crew" else RED if wall.owner == "rival" else GOLD
		var rect = tag_rect(wall)
		
		# Wall concrete base plaque
		draw_rect(rect.grow(5), Color(0.04, 0.05, 0.06, 0.92), true)
		draw_rect(rect.grow(5), Color(color, 0.4), false, 1.5)
		
		if wall.owner != "none":
			draw_graffiti_piece(wall, wall.owner, 1.0)
		else:
			# Unclaimed Wall: Stencil markers, crosshairs, and number label
			for corner in [rect.position, Vector2(rect.end.x, rect.position.y), rect.end, Vector2(rect.position.x, rect.end.y)]:
				var inward = (rect.get_center() - corner).sign()
				draw_line(corner, corner + Vector2(inward.x * 12, 0), Color(GOLD, 0.6), 2.0, true)
				draw_line(corner, corner + Vector2(0, inward.y * 10), Color(GOLD, 0.6), 2.0, true)
			draw_string(ThemeDB.fallback_font, rect.get_center() + Vector2(-28, 5), "[SPOT %d]" % (i + 1), HORIZONTAL_ALIGNMENT_CENTER, -1, 14, Color(GOLD, 0.85))
			draw_string(ThemeDB.fallback_font, point + Vector2(-36, -rect.size.y * 0.5 - 6), wall.name, HORIZONTAL_ALIGNMENT_CENTER, -1, 12, Color(1, 1, 1, 0.75))
		
		# In-progress spraying overlay
		if wall.progress > 0:
			draw_graffiti_piece(wall, "crew", wall.progress)
		if wall.enemy_progress > 0:
			draw_graffiti_piece(wall, "rival", wall.enemy_progress)
			
		# Spray progress bars
		if wall.progress > 0 or wall.enemy_progress > 0:
			var bar_rect = Rect2(point + Vector2(-rect.size.x * 0.48, rect.size.y * 0.5 + 4), Vector2(rect.size.x * 0.96, 6))
			draw_rect(bar_rect, Color("101216"))
			draw_rect(bar_rect, Color(0.3, 0.3, 0.3), false, 1)
			if wall.progress > 0:
				draw_rect(Rect2(bar_rect.position, Vector2(bar_rect.size.x * wall.progress, 6)), TURQUOISE)
			if wall.enemy_progress > 0:
				draw_rect(Rect2(bar_rect.position + Vector2(0, 7), Vector2(bar_rect.size.x * wall.enemy_progress, 4)), RED)
				
		if i == nearest:
			var pulse = sin(time * 6.0) * 4.0
			draw_arc(point, rect.size.x * 0.58 + pulse, 0, TAU, 36, Color(color, 0.75), 2.0, true)
		if i == contract_wall:
			var target_pulse = sin(time * 8.0) * 5.0
			draw_arc(point, rect.size.x * 0.65 + target_pulse, 0, TAU, 36, Color(GOLD, 0.9), 3.0, true)
			draw_string(ThemeDB.fallback_font, point + Vector2(-30, rect.size.y * 0.5 + 24), "★ TARGET ★", HORIZONTAL_ALIGNMENT_CENTER, -1, 12, GOLD)
			
	# Pickups with urban neon glow & badges
	for pickup in pickups:
		if pickup.timer > 0:
			continue
		var p: Vector2 = pickup.pos + Vector2(0, sin(time * 3.5 + pickup.pos.x) * 4.0)
		var colors = {"paint": TURQUOISE, "health": MAGENTA, "ammo": GOLD, "weapon": Color.WHITE, "recruit": PURPLE}
		var color: Color = colors[pickup.kind]
		
		# Glowing aura
		draw_circle(p, 20 + sin(time * 4.0) * 2.0, Color(color, 0.16))
		draw_arc(p, 16, 0, TAU, 24, Color(color, 0.8), 2.0, true)
		match pickup.kind:
			"paint":
				draw_style_box(_can_style(color), Rect2(p - Vector2(6, 9), Vector2(12, 18)))
				draw_rect(Rect2(p + Vector2(-3, -13), Vector2(6, 4)), color)
				draw_circle(p + Vector2(0, -1), 3.5, Color.WHITE)
			"health":
				draw_rect(Rect2(p - Vector2(8, 3), Vector2(16, 6)), color)
				draw_rect(Rect2(p - Vector2(3, 8), Vector2(6, 16)), color)
			"ammo":
				for dx in [-6, 0, 6]:
					draw_line(p + Vector2(dx, -7), p + Vector2(dx, 7), color, 3.5)
					draw_line(p + Vector2(dx, -7), p + Vector2(dx, -9), Color.WHITE, 3.0)
			"recruit":
				draw_circle(p + Vector2(0, -6), 5, color)
				draw_arc(p + Vector2(0, 7), 8, PI, TAU, 14, color, 4.5)
			"weapon":
				draw_line(p + Vector2(-10, -4), p + Vector2(10, -4), color, 5)
				draw_line(p + Vector2(-6, -3), p + Vector2(-6, 7), color, 4)
				draw_line(p + Vector2(5, -4), p + Vector2(5, -1), Color.WHITE, 2)
				
	# HQ and Supplies Landmarks
	for pair in [[hq, "404 HQ", TURQUOISE], [shop, "SUPPLY SHOP", GOLD]]:
		var pos = pair[0]
		var name = pair[1]
		var col = pair[2]
		var halo_size = 46.0 + sin(time * 3.0) * 3.0
		draw_circle(pos, halo_size, Color(col, 0.12))
		draw_arc(pos, halo_size, 0, TAU, 36, Color(col, 0.85), 2.5, true)
		draw_rect(Rect2(pos + Vector2(-42, -58), Vector2(84, 20)), Color(0.02, 0.03, 0.04, 0.88))
		draw_rect(Rect2(pos + Vector2(-42, -58), Vector2(84, 20)), Color(col, 0.6), false, 1.5)
		draw_string(ThemeDB.fallback_font, pos + Vector2(-36, -43), name, HORIZONTAL_ALIGNMENT_CENTER, -1, 13, col)
		
	# Streetlight Atmospheric Lighting Cones
	for sl_pos in streetlights:
		if cone_tex != null:
			var cone_sz = Vector2(140, 180)
			draw_texture_rect(cone_tex, Rect2(sl_pos - Vector2(cone_sz.x * 0.5, 10), cone_sz), false, Color(1, 1, 0.85, 0.35 + 0.05 * sin(time * 3.0 + sl_pos.x)))
		else:
			draw_circle(sl_pos, 45, Color(1, 0.95, 0.7, 0.15))
		# Streetlight post icon
		draw_circle(sl_pos, 4.0, Color.WHITE)
		draw_circle(sl_pos, 7.0, Color(1, 0.9, 0.4, 0.5))

	# Aerosol Mist Particles
	for m in mist_particles:
		var alpha = clampf(m.life / m.max_life, 0.0, 1.0)
		draw_circle(m.pos, m.radius * (1.2 - alpha * 0.2), Color(m.color.r, m.color.g, m.color.b, alpha * 0.45))
		
	# Paint Shockwaves
	for sw in shockwaves:
		var alpha = clampf(sw.life / sw.max_life, 0.0, 1.0)
		draw_arc(sw.pos, sw.radius, 0, TAU, 36, Color(sw.color, alpha * 0.8), 4.0 * alpha, true)
		draw_circle(sw.pos, sw.radius * 0.5, Color(sw.color.r, sw.color.g, sw.color.b, alpha * 0.15))
		
	# Sparks & Flecks
	for spark in sparks:
		var life_ratio = minf(1.0, spark.life * 2.0)
		draw_circle(spark.pos, spark.get("size", 2.5), Color(spark.color, life_ratio))
		
	# Floating Graffiti Text Popups
	for ft in floating_texts:
		var ratio = ft.life / ft.max_life
		var alpha = minf(1.0, ratio * 2.5)
		var text_col = Color(ft.color, alpha)
		var shadow_col = Color(0, 0, 0, alpha * 0.9)
		var f_size = int(18.0 * ft.scale)
		# Drop shadow
		draw_string(ThemeDB.fallback_font, ft.pos + Vector2(2, 2), ft.text, HORIZONTAL_ALIGNMENT_CENTER, -1, f_size, shadow_col)
		# Fore text
		draw_string(ThemeDB.fallback_font, ft.pos, ft.text, HORIZONTAL_ALIGNMENT_CENTER, -1, f_size, text_col)
		
	# Police Siren Atmosphere / Night Lighting when heat is rising
	if city_heat > 15.0:
		var siren_phase = fmod(time * 3.5, 1.0)
		var siren_col = RED if siren_phase < 0.5 else Color("3388ff")
		var siren_intensity = clampf((city_heat - 15.0) / 85.0, 0.0, 0.4) * (0.6 + 0.4 * sin(time * 12.0))
		# Sweep siren searchlights across the screen
		var sweep_center = Vector2(1254, 1254) + Vector2.from_angle(time * 2.0) * 600.0
		draw_circle(sweep_center, 380, Color(siren_col, siren_intensity * 0.35))
		
	# Atmospheric Rain
	if rain:
		for i in 220:
			var p = Vector2(fposmod(i * 173.0 - time * 65.0, 2508), fposmod(i * 311.0 + time * 480, 2508))
			draw_line(p, p + Vector2(-5, 15), Color(0.65, 0.85, 1.0, 0.18), 1.2)

	# Smoke Clouds
	for sc in smoke_clouds:
		var alpha = clampf(sc.life / sc.max_life, 0.0, 1.0)
		if smoke_cloud_tex != null:
			draw_texture_rect(smoke_cloud_tex, Rect2(sc.pos - Vector2(sc.radius, sc.radius), Vector2(sc.radius * 2, sc.radius * 2)), false, Color(1, 1, 1, alpha * 0.7))
		else:
			draw_circle(sc.pos, sc.radius, Color(0.35, 0.92, 0.85, alpha * 0.28))
			draw_circle(sc.pos, sc.radius * 0.6, Color(0.95, 0.35, 0.7, alpha * 0.18))
			draw_arc(sc.pos, sc.radius, 0, TAU, 24, Color(TURQUOISE, alpha * 0.55), 2.5, true)

	# Civilian & Skater Crowd
	for civ in civilians:
		var civ_cols = [Color("59b7ff"), GOLD, LIME]
		var cc = civ_cols[civ.kind % civ_cols.size()]
		if civilian_crowd_tex != null:
			var cell_w = civilian_crowd_tex.get_width() / 4.0
			var src_rect = Rect2((civ.kind % 4) * cell_w, 0, cell_w, civilian_crowd_tex.get_height())
			draw_texture_rect_region(civilian_crowd_tex, Rect2(civ.pos - Vector2(18, 30), Vector2(36, 36)), src_rect)
		else:
			draw_set_transform(civ.pos, 0.0, Vector2(1.0, 0.45))
			draw_circle(Vector2.ZERO, 7, Color(0, 0, 0, 0.4))
			draw_set_transform(Vector2.ZERO, 0.0, Vector2.ONE)
			draw_circle(civ.pos + Vector2(0, -12), 5, cc)
			draw_arc(civ.pos + Vector2(0, -2), 8, PI, TAU, 10, cc, 3.5)
			if civ.kind == 0:
				draw_line(civ.pos + Vector2(-5, 4), civ.pos + Vector2(5, 4), Color(0.3, 0.3, 0.3), 2)
		if civ.speech != "" and civ.speech_timer > 0:
			var bubble_alpha = minf(1.0, civ.speech_timer)
			draw_rect(Rect2(civ.pos + Vector2(-28, -38), Vector2(56, 16)), Color(0.05, 0.06, 0.08, bubble_alpha * 0.92))
			draw_rect(Rect2(civ.pos + Vector2(-28, -38), Vector2(56, 16)), Color(cc, bubble_alpha * 0.8), false, 1.0)
			draw_string(ThemeDB.fallback_font, civ.pos + Vector2(-24, -25), civ.speech, HORIZONTAL_ALIGNMENT_LEFT, -1, 10, Color(cc, bubble_alpha))

	# Boombox Prop
	if boombox_active != Vector2(-1, -1):
		var bp = boombox_active
		var pulse = 0.9 + 0.1 * sin(time * 12.0)
		if boombox_tex != null:
			draw_texture_rect(boombox_tex, Rect2(bp - Vector2(28, 28) * pulse, Vector2(56, 56) * pulse), false)
		else:
			draw_rect(Rect2(bp - Vector2(18, 10), Vector2(36, 20)), Color("1a1a2e"))
			draw_rect(Rect2(bp - Vector2(18, 10), Vector2(36, 20)), GOLD, false, 1.5)
			draw_circle(bp + Vector2(-9, 0), 6 * pulse, Color(GOLD, 0.8))
			draw_circle(bp + Vector2(9, 0), 6 * pulse, Color(GOLD, 0.8))
		# Sound wave rings
		for r in 3:
			var ring_r = (r + 1) * 18.0 * pulse
			draw_arc(bp, ring_r, -PI * 0.4, PI * 0.4, 12, Color(GOLD, 0.35 - r * 0.1), 1.5, true)
		draw_string(ThemeDB.fallback_font, bp + Vector2(-14, -28), "808 LIVE", HORIZONTAL_ALIGNMENT_LEFT, -1, 10, GOLD)

	# Police Riot Roadblocks
	for rb in roadblocks:
		if roadblock_tex != null:
			draw_texture_rect(roadblock_tex, Rect2(rb.pos - Vector2(40, 20), Vector2(80, 40)), false)
		else:
			var rb_life = clampf(rb.life / 28.0, 0.0, 1.0)
			draw_set_transform(rb.pos, rb.angle, Vector2.ONE)
			draw_rect(Rect2(Vector2(-35, -10), Vector2(70, 20)), Color(0.85, 0.85, 0.85))
			for s in 4:
				var sx = -30.0 + s * 18.0
				draw_rect(Rect2(Vector2(sx, -10), Vector2(9, 20)), Color("f5a623") if s % 2 == 0 else Color("1a1a1a"))
			if fmod(time * 4.0, 1.0) < 0.5:
				draw_circle(Vector2(0, -16), 6, Color("f5a623"))
				draw_circle(Vector2(0, -16), 9, Color(1.0, 0.65, 0.14, 0.4))
			draw_set_transform(Vector2.ZERO, 0.0, Vector2.ONE)

	# Black Market Van
	if black_market_pos != Vector2(-1, -1):
		var bm = black_market_pos
		if black_market_van_tex != null:
			draw_texture_rect(black_market_van_tex, Rect2(bm - Vector2(45, 45), Vector2(90, 90)), false)
		else:
			draw_rect(Rect2(bm - Vector2(28, 18), Vector2(56, 36)), Color("0d1117"))
			draw_rect(Rect2(bm - Vector2(28, 18), Vector2(56, 36)), PURPLE, false, 2.0)
			var glow_pulse = 0.5 + 0.5 * sin(time * 5.0)
			draw_circle(bm + Vector2(16, 0), 10 * glow_pulse, Color(PURPLE, 0.4))
		draw_string(ThemeDB.fallback_font, bm + Vector2(-24, -28), "\uD83D\uDECD BLACK MKT", HORIZONTAL_ALIGNMENT_LEFT, -1, 11, PURPLE)
		draw_arc(bm, 52, 0, TAU, 20, Color(PURPLE, 0.25 + 0.15 * sin(time * 3.0)), 1.5, true)

	# Deployable Spray Drone Scout
	if drone_pos != Vector2(-1, -1):
		var dp = drone_pos
		if drone_tex != null:
			draw_texture_rect(drone_tex, Rect2(dp - Vector2(30, 30), Vector2(60, 60)), false)
		else:
			draw_circle(dp, 14, TURQUOISE)
			draw_circle(dp, 28, Color(TURQUOISE, 0.2))
		var spotlight_pts = PackedVector2Array([dp, dp + Vector2(-40, 90), dp + Vector2(40, 90)])
		draw_colored_polygon(spotlight_pts, Color(TURQUOISE, 0.15))
		draw_string(ThemeDB.fallback_font, dp + Vector2(-22, -32), "DRONE 404", HORIZONTAL_ALIGNMENT_LEFT, -1, 10, TURQUOISE)

	# Batch 4 Rendering (Turrets, Mines, Flares, News Chopper, K9, Cargo)
	if heist_target != Vector2(-1, -1):
		if cargo_crate_tex != null:
			draw_texture_rect(cargo_crate_tex, Rect2(heist_target - Vector2(25, 25), Vector2(50, 50)), false)
		else:
			var pulse_r = 18.0 + sin(time * 6.0) * 5.0
			draw_arc(heist_target, pulse_r, 0, TAU, 24, Color(GOLD, 0.9), 3.0, true)
			draw_circle(heist_target, 8, GOLD)
		draw_string(ThemeDB.fallback_font, heist_target + Vector2(-20, -28), "CARGO", HORIZONTAL_ALIGNMENT_LEFT, -1, 12, GOLD)
	for pt in paint_turrets:
		if turret_tex != null:
			draw_texture_rect(turret_tex, Rect2(pt.pos - Vector2(30, 30), Vector2(60, 60)), false)
		else:
			draw_circle(pt.pos, 16, TURQUOISE)
			draw_circle(pt.pos, 24, Color(TURQUOISE, 0.3))

	for pm in paint_mines:
		if mine_tex != null:
			draw_texture_rect(mine_tex, Rect2(pm.pos - Vector2(20, 20), Vector2(40, 40)), false)
		else:
			draw_circle(pm.pos, 10, MAGENTA)

	for tf in turf_flares:
		if flare_tex != null:
			draw_texture_rect(flare_tex, Rect2(tf.pos - Vector2(25, 25), Vector2(50, 50)), false)
		else:
			draw_circle(tf.pos, 14, GOLD)

	if news_chopper_pos != Vector2(-1, -1):
		if news_chopper_tex != null:
			draw_texture_rect(news_chopper_tex, Rect2(news_chopper_pos - Vector2(60, 30), Vector2(120, 60)), false)
		else:
			draw_circle(news_chopper_pos, 22, Color.WHITE)

	for k9 in k9_units:
		if k9_tex != null:
			draw_texture_rect(k9_tex, Rect2(k9.pos - Vector2(20, 20), Vector2(40, 40)), false)
		else:
			draw_circle(k9.pos, 12, Color("a52a2a"))

	# Batch 5 — Interactive City Landmarks & Visual Features
	# 1. Food Truck Station
	var ft_pos = Vector2(1245, 520)
	if food_truck_tex != null:
		draw_texture_rect(food_truck_tex, Rect2(ft_pos - Vector2(50, 40), Vector2(100, 80)), false)
	else:
		draw_rect(Rect2(ft_pos - Vector2(40, 25), Vector2(80, 50)), GOLD)
	draw_string(ThemeDB.fallback_font, ft_pos + Vector2(-36, -45), "TACO TRUCK", HORIZONTAL_ALIGNMENT_CENTER, -1, 11, GOLD)

	# 2. DJ Turntable Booth
	var dj_pos = Vector2(1570, 1460)
	if dj_booth_tex != null:
		draw_texture_rect(dj_booth_tex, Rect2(dj_pos - Vector2(40, 40), Vector2(80, 80)), false)
	else:
		draw_rect(Rect2(dj_pos - Vector2(30, 20), Vector2(60, 40)), PURPLE)
	draw_string(ThemeDB.fallback_font, dj_pos + Vector2(-30, -45), "BEAT BOOTH", HORIZONTAL_ALIGNMENT_CENTER, -1, 11, PURPLE)

	# 3. Skate Park Bowl
	var bowl_pos = Vector2(1890, 1800)
	if skate_bowl_tex != null:
		draw_texture_rect(skate_bowl_tex, Rect2(bowl_pos - Vector2(70, 70), Vector2(140, 140)), false)
	else:
		draw_circle(bowl_pos, 55, Color(0.2, 0.2, 0.25))

	# 4. Breakdancer Performance
	var dancer_pos = Vector2(615, 1130) + Vector2(sin(time * 6.0) * 15.0, 0)
	if breakdancer_tex != null:
		draw_texture_rect(breakdancer_tex, Rect2(dancer_pos - Vector2(25, 25), Vector2(50, 50)), false)

	# 5. Rooftop Helipad
	var heli_pos = Vector2(2310, 520)
	if helipad_tex != null:
		draw_texture_rect(helipad_tex, Rect2(heli_pos - Vector2(65, 65), Vector2(130, 130)), false)

	# 6. Park Fountain Takeover
	var fountain_pos = Vector2(1570, 820)
	if park_fountain_tex != null:
		draw_texture_rect(park_fountain_tex, Rect2(fountain_pos - Vector2(55, 55), Vector2(110, 110)), false)

	# 7. Rival Command Trailer
	var trailer_pos = Vector2(2310, 1800)
	if command_trailer_tex != null:
		draw_texture_rect(command_trailer_tex, Rect2(trailer_pos - Vector2(60, 35), Vector2(120, 70)), false)

	# 8. Spotlight Watchtower
	var tower_pos = Vector2(310, 1460)
	if watchtower_tex != null:
		draw_texture_rect(watchtower_tex, Rect2(tower_pos - Vector2(40, 60), Vector2(80, 120)), false)
		var sweep_angle = time * 1.5
		var sweep_dir = Vector2.from_angle(sweep_angle)
		var cone_pts = PackedVector2Array([tower_pos, tower_pos + (sweep_dir.rotated(-0.35) * 220), tower_pos + (sweep_dir.rotated(0.35) * 220)])
		draw_colored_polygon(cone_pts, Color(1, 0.9, 0.4, 0.18))

	# 9. Nozzle Tuning Bench
	var bench_pos = Vector2(855, 1800)
	if tuning_bench_tex != null:
		draw_texture_rect(tuning_bench_tex, Rect2(bench_pos - Vector2(30, 30), Vector2(60, 60)), false)

	# EQ Audio Visualizer (Corner HUD in-world)
	var eq_origin = Vector2(320, 2380)
	if audio_spectrum_hud_tex != null:
		draw_texture_rect(audio_spectrum_hud_tex, Rect2(eq_origin - Vector2(4, 4), Vector2(180, 90)), false)
	else:
		draw_rect(Rect2(eq_origin - Vector2(4, 4), Vector2(8 * 14 + 8, 48)), Color(0.04, 0.05, 0.07, 0.88))
		draw_rect(Rect2(eq_origin - Vector2(4, 4), Vector2(8 * 14 + 8, 48)), GOLD, false, 1.0)
		for b in eq_bars.size():
			var bar_h = eq_bars[b] * 36.0
			var bar_col = TURQUOISE if b < 4 else MAGENTA
			draw_rect(Rect2(eq_origin + Vector2(b * 14, 36 - bar_h), Vector2(10, bar_h)), bar_col)

	# Batch 6 — Interactive Features & Urban Props (Updates 07)
	# 1. Lowrider Hydraulic Bounce Car
	var lr_hop = Vector2(0, -sin(lowrider_bounce * PI) * 22.0)
	var lr_draw_pos = lowrider_pos + lr_hop
	if lowrider_car_tex != null:
		draw_texture_rect(lowrider_car_tex, Rect2(lr_draw_pos - Vector2(75, 40), Vector2(150, 80)), false)
	else:
		draw_rect(Rect2(lr_draw_pos - Vector2(60, 30), Vector2(120, 60)), MAGENTA)
	draw_string(ThemeDB.fallback_font, lowrider_pos + Vector2(-45, 48), "LOWRIDER [H]", HORIZONTAL_ALIGNMENT_CENTER, -1, 11, MAGENTA)

	# 2. Vending Machine Kiosk
	if vending_kiosk_tex != null:
		draw_texture_rect(vending_kiosk_tex, Rect2(vending_pos - Vector2(30, 50), Vector2(60, 100)), false)
	else:
		draw_rect(Rect2(vending_pos - Vector2(25, 40), Vector2(50, 80)), TURQUOISE)
	draw_string(ThemeDB.fallback_font, vending_pos + Vector2(-45, -55), "NEON SODA [E]", HORIZONTAL_ALIGNMENT_CENTER, -1, 11, TURQUOISE)

	# 3. Subway Grate Updraft Vent
	if subway_grate_tex != null:
		draw_texture_rect(subway_grate_tex, Rect2(subway_grate_pos - Vector2(45, 45), Vector2(90, 90)), false)
	else:
		draw_rect(Rect2(subway_grate_pos - Vector2(35, 35), Vector2(70, 70)), Color(0.2, 0.25, 0.3))
	# Billowing hot steam mist particles
	if randf() < 0.25:
		spray_cloud(subway_grate_pos, subway_grate_pos + Vector2(0, -50), Color(0.9, 0.95, 1.0, 0.5), 3)

	# 4. Rooftop Chill Lounge & Neon Pool Table
	if pool_table_tex != null:
		draw_texture_rect(pool_table_tex, Rect2(pool_table_pos - Vector2(50, 50), Vector2(100, 100)), false)
	else:
		draw_rect(Rect2(pool_table_pos - Vector2(40, 40), Vector2(80, 80)), LIME)
	draw_string(ThemeDB.fallback_font, pool_table_pos + Vector2(-40, -55), "CHILL LOUNGE", HORIZONTAL_ALIGNMENT_CENTER, -1, 11, LIME)

	# 5. Police CCTV Security Cameras
	for cam in cctv_cameras:
		var cp: Vector2 = cam.pos
		var c_angle: float = cam.angle
		var is_dis: bool = cam.get("disabled", false)
		if cctv_tex != null:
			draw_texture_rect(cctv_tex, Rect2(cp - Vector2(20, 20), Vector2(40, 40)), false)
		else:
			draw_circle(cp, 10, Color.WHITE)
		if not is_dis:
			var c_dir = Vector2.from_angle(c_angle + PI * 0.5)
			var fov_pts = PackedVector2Array([cp, cp + (c_dir.rotated(-0.45) * 140), cp + (c_dir.rotated(0.45) * 140)])
			draw_colored_polygon(fov_pts, Color(1, 0.2, 0.2, 0.2))

	# 6. Contraband Market Vendor Stall
	if market_stall_tex != null:
		draw_texture_rect(market_stall_tex, Rect2(market_stall_pos - Vector2(50, 50), Vector2(100, 100)), false)
	draw_string(ThemeDB.fallback_font, market_stall_pos + Vector2(-45, -55), "ALLEY BAZAAR", HORIZONTAL_ALIGNMENT_CENTER, -1, 11, GOLD)

	# 7. Alley Parkour Dumpsters
	for dp in dumpsters:
		if dumpster_tex != null:
			draw_texture_rect(dumpster_tex, Rect2(dp - Vector2(35, 35), Vector2(70, 70)), false)
		else:
			draw_rect(Rect2(dp - Vector2(25, 25), Vector2(50, 50)), Color(0.25, 0.35, 0.2))

	# 8. Police Radio Jammer
	if radio_jammer_tex != null:
		draw_texture_rect(radio_jammer_tex, Rect2(radio_jammer_pos - Vector2(45, 45), Vector2(90, 90)), false)

	# 9. Contested Rival Turf Banners
	for bp in rival_banners:
		if rival_banner_tex != null:
			draw_texture_rect(rival_banner_tex, Rect2(bp - Vector2(20, 40), Vector2(40, 80)), false)

	# 10. Elevated Metro Rail Tracks (North Border)
	if metro_tracks_tex != null:
		draw_texture_rect(metro_tracks_tex, Rect2(Vector2(1245, 230) - Vector2(80, 40), Vector2(160, 80)), false)

	# 11. Police Surveillance Airship Blimp (Sky Layer)
	if blimp_tex != null:
		draw_texture_rect(blimp_tex, Rect2(blimp_pos - Vector2(120, 60), Vector2(240, 120)), false)
		# Massive sweeping searchlight
		var b_light_dir = Vector2(sin(time * 0.8) * 0.4, 1.0).normalized()
		var bl_pts = PackedVector2Array([blimp_pos, blimp_pos + (b_light_dir.rotated(-0.35) * 320), blimp_pos + (b_light_dir.rotated(0.35) * 320)])
		draw_colored_polygon(bl_pts, Color(0.4, 0.7, 1.0, 0.16))

	# 12. Soundquake Bass Blast Waves
	for sq in soundquake_waves:
		var sq_alpha = clampf(sq.life / sq.max_life, 0.0, 1.0)
		draw_arc(sq.pos, sq.radius, 0, TAU, 36, Color(GOLD, sq_alpha * 0.9), 5.0 * sq_alpha, true)
		draw_arc(sq.pos, sq.radius * 0.75, 0, TAU, 28, Color(TURQUOISE, sq_alpha * 0.8), 3.0 * sq_alpha, true)

	# 13. 24K Golden Mastery Can Trophy
	if golden_can_unlocked:
		var gold_pos = hq + Vector2(0, -90 + sin(time * 3.0) * 8.0)
		if golden_can_tex != null:
			draw_texture_rect(golden_can_tex, Rect2(gold_pos - Vector2(35, 35), Vector2(70, 70)), false)
		draw_arc(gold_pos, 42.0 + sin(time * 5.0) * 4.0, 0, TAU, 30, Color(GOLD, 0.8), 2.5, true)
		draw_string(ThemeDB.fallback_font, gold_pos + Vector2(-45, -42), "★ 24K MASTER ★", HORIZONTAL_ALIGNMENT_CENTER, -1, 12, GOLD)


func tag_rect(wall: Dictionary) -> Rect2:
	var width = 94.0 + float(wall.size) * 24.0
	var height = width * 0.5
	return Rect2(wall.pos - Vector2(width * 0.5, height - 12), Vector2(width, height))

# Procedural Multi-Layer Graffiti Renderer
func draw_graffiti_piece(wall: Dictionary, owner_kind: String, reveal: float) -> void:
	var rect = tag_rect(wall)
	var is_crew = owner_kind == "crew"
	var primary_col = TURQUOISE if is_crew else RED
	var secondary_col = MAGENTA if is_crew else Color("ff8800")
	var highlight_col = LIME if is_crew else GOLD
	var tag_title = "404 CREW" if is_crew else "RIVALS"
	
	# Clipping width based on spray progress
	var width_cut = rect.size.x * clampf(reveal, 0.0, 1.0)
	var draw_w = width_cut
	
	# 1. Spray paint background halo / overspray cloud
	var center = rect.get_center()
	for offset_pass in [Vector2(-3, 0), Vector2(3, 0), Vector2(0, -2), Vector2(0, 2)]:
		draw_circle(center + offset_pass, (rect.size.y * 0.6) * reveal, Color(primary_col, 0.15))
		
	# 2. Dripping paint trails
	if wall.has("drips"):
		for drip in wall.drips:
			if drip.delay <= reveal:
				var dx = center.x + drip.offset_x * draw_w
				if dx >= rect.position.x and dx <= rect.position.x + draw_w:
					var dy = rect.end.y - 4
					var d_len = drip.length * clampf((reveal - drip.delay) / 0.4, 0.0, 1.0)
					draw_line(Vector2(dx, dy), Vector2(dx, dy + d_len), primary_col, drip.width, true)
					draw_circle(Vector2(dx, dy + d_len), drip.width * 0.7, primary_col)
					
	# 3. 3D Drop Shadow Extrusion
	var shadow_offset = Vector2(4, 4)
	draw_string(ThemeDB.fallback_font, center + shadow_offset + Vector2(-draw_w * 0.42, 8), tag_title, HORIZONTAL_ALIGNMENT_LEFT, int(draw_w * 0.95), int(rect.size.y * 0.45), Color(0.01, 0.02, 0.03, 0.95))
	
	# 4. Bold Graffiti Outline (Thick Stroke)
	var stroke_col = Color(0.05, 0.08, 0.1, 1.0)
	for ox in [-2, 0, 2]:
		for oy in [-2, 0, 2]:
			if ox != 0 or oy != 0:
				draw_string(ThemeDB.fallback_font, center + Vector2(ox, oy) + Vector2(-draw_w * 0.42, 8), tag_title, HORIZONTAL_ALIGNMENT_LEFT, int(draw_w * 0.95), int(rect.size.y * 0.45), stroke_col)
				
	# 5. Dual-tone Graffiti Fill
	draw_string(ThemeDB.fallback_font, center + Vector2(-draw_w * 0.42, 8), tag_title, HORIZONTAL_ALIGNMENT_LEFT, int(draw_w * 0.95), int(rect.size.y * 0.45), primary_col)
	
	# 6. Upper Spray Highlights / Shines
	if reveal >= 0.8:
		draw_string(ThemeDB.fallback_font, center + Vector2(-draw_w * 0.42, 6), tag_title, HORIZONTAL_ALIGNMENT_LEFT, int(draw_w * 0.95), int(rect.size.y * 0.45), Color(highlight_col, 0.4))
		# Street Crown / Halo for full pieces
		var crown_pos = Vector2(center.x, rect.position.y - 2)
		if is_crew:
			draw_crown(crown_pos, GOLD)
		else:
			draw_horns(crown_pos, RED)
			
	# Spray nozzle tip flash while actively painting
	if reveal > 0 and reveal < 1.0:
		var tip_pos = rect.position + Vector2(draw_w, rect.size.y * 0.5)
		draw_circle(tip_pos, 8.0, Color(primary_col, 0.7))
		draw_circle(tip_pos, 4.0, Color.WHITE)

func draw_crown(pos: Vector2, color: Color) -> void:
	var pts = PackedVector2Array([
		pos + Vector2(-12, 0),
		pos + Vector2(-14, -8),
		pos + Vector2(-6, -4),
		pos + Vector2(0, -11),
		pos + Vector2(6, -4),
		pos + Vector2(14, -8),
		pos + Vector2(12, 0)
	])
	draw_colored_polygon(pts, color)
	draw_polyline(pts, Color.WHITE, 1.2)

func draw_horns(pos: Vector2, color: Color) -> void:
	draw_line(pos + Vector2(-8, 2), pos + Vector2(-13, -8), color, 2.5)
	draw_line(pos + Vector2(8, 2), pos + Vector2(13, -8), color, 2.5)

func _can_style(color: Color) -> StyleBoxFlat:
	var style = StyleBoxFlat.new()
	style.bg_color = color
	style.corner_radius_top_left = 3
	style.corner_radius_top_right = 3
	return style

func _create_tag_texture(color: Color) -> ImageTexture:
	var img = Image.create(128, 64, false, Image.FORMAT_RGBA8)
	img.fill(Color(0, 0, 0, 0))
	for y in range(8, 56):
		for x in range(8, 120):
			var edge = (x < 12 or x > 115 or y < 12 or y > 51)
			img.set_pixel(x, y, Color.BLACK if edge else color)
	return ImageTexture.create_from_image(img)
