extends Node2D

const Campaign = preload("res://scripts/campaign.gd")
const City = preload("res://scripts/city.gd")
const Actor = preload("res://scripts/actor.gd")
const Interface = preload("res://scripts/interface.gd")
const Sound = preload("res://scripts/audio.gd")
var campaign = Campaign.new()
var audio = Sound.new()
var city: Node2D
var player: CharacterBody2D
var actors: Node2D
var camera: Camera2D
var ui: CanvasLayer
var enemies: Array = []
var crew: Array = []
var bullets: Array[Dictionary] = []
var mode = "menu"
var paint = 100.0
var ammo = 24
var weapon = true
var stamina = 100.0
var heat = 0.0
var cash_clock = 0.0
var save_clock = 0.0
var spawn_clock = 0.0
var penalty = ""
var penalty_time = 0.0
var stash = 0.0
var crew_order = "FOLLOW"
var guard_point = Vector2.ZERO
var combo = 0
var combo_time = 0.0
var contract: Dictionary = {}
var contracts_done = 0
var final_hold = 0.0
var notice = ""
var notice_time = 0.0
var mission_clock = 0.0
var hit_shake = 0.0
var respawn_invulnerability = 0.0
var nearest_wall = -1
var qa_mode = false
var deliveries = 0
var shipment = false
var captains_defeated = 0
var spray_cap = "FAT"
var blast_cooldown = 0.0

# Batch 3 — New Mechanics
var dash_timer = 0.0
var dash_cooldown = 0.0
var dash_dir = Vector2.ZERO
var smoke_cooldown = 0.0
var smoke_bombs = 2
var boombox_pos = Vector2(-1, -1)
var boombox_timer = 0.0
var boombox_cooldown = 0.0
var heist_active = false
var heist_cargo = false
var heist_timer = 0.0
var heist_target = Vector2.ZERO
var flow_tap_timer = 0.0
var flow_tap_window = 0.0
var flow_bonus = false
var flow_bonus_timer = 0.0
var raid_wall = -1
var raid_timer = 0.0
var time_of_day = 0.0
var weather_phase = "MIDNIGHT"
var overdrive_timer = 0.0
var overdrive_kind = ""
var crew_classes: Dictionary = {}
var roadblocks: Array[Dictionary] = []
var black_market_pos = Vector2(-1, -1)
var black_market_timer = 0.0
var neon_puddles: Array[Dictionary] = []
var civilians: Array[Dictionary] = []
var drone_active = false
var drone_timer = 0.0
var drone_pos = Vector2(-1, -1)
var paint_turrets: Array[Dictionary] = []
var paint_mines: Array[Dictionary] = []
var turf_flares: Array[Dictionary] = []
var thermal_foil_timer = 0.0
var news_chopper_pos = Vector2(-1, -1)
var k9_units: Array[Dictionary] = []
var camera_views = [1.85, 1.35, 0.95]
var camera_view_idx = 0
var slowmo_timer = 0.0
var flamethrower_timer = 0.0
var bubble_shield_timer = 0.0
var stencil_stamps = ["CROWN", "SKULL", "CAN", "BOLT", "STAR", "CREST"]
var active_stamp = 0
var neon_underglow = false
var neon_soda_timer = 0.0
var lowrider_hop_cooldown = 0.0
var soundquake_cooldown = 0.0

func _ready() -> void:
	get_tree().auto_accept_quit = false
	qa_mode = "--qa" in OS.get_cmdline_user_args()
	if not qa_mode:
		campaign.load_preferences()
	add_child(campaign)
	add_child(audio)
	audio.config = campaign.settings
	ui = Interface.new()
	ui.game = self
	add_child(ui)
	qa_mode = "--qa" in OS.get_cmdline_user_args()
	if qa_mode:
		campaign.save_path = "user://qa_save.json"
	ui.show_menu()
	if qa_mode:
		call_deferred("run_qa")

func _notification(what: int) -> void:
	if what == NOTIFICATION_WM_CLOSE_REQUEST:
		if is_instance_valid(city) and mode != "menu" and not qa_mode:
			if not save_run(false):
				mode = "pause"
				ui.show_pause()
				return
		get_tree().quit()

func announce(message: String) -> void:
	notice = message
	notice_time = 5.0

func start_run(load_existing: bool = false) -> void:
	if load_existing:
		if not campaign.load_game():
			ui.menu_error(campaign.save_error)
			return
		if not qa_mode:
			campaign.load_preferences()
	else:
		campaign.reset()
	build_world()
	if load_existing and not campaign.snapshot.is_empty():
		restore_world(campaign.snapshot)
	mode = "play"
	audio.active = true
	audio.config = campaign.settings
	apply_settings()
	ui.show_hud()
	if campaign.cleared > campaign.level:
		mode = "upgrade"
		ui.show_upgrades()
	else:
		announce("Welcome to " + campaign.LEVELS[campaign.level].name)

func build_world() -> void:
	if is_instance_valid(city):
		remove_child(city)
		city.queue_free()
	if is_instance_valid(actors):
		remove_child(actors)
		actors.queue_free()
	if is_instance_valid(camera):
		remove_child(camera)
		camera.queue_free()
	city = City.new()
	city.chapter = 1 if campaign.level >= 4 else 0
	add_child(city)
	actors = Node2D.new()
	actors.y_sort_enabled = true
	add_child(actors)
	enemies.clear()
	crew.clear()
	bullets.clear()
	player = create_actor("player", city.hq)
	player.set_outfit(campaign.outfit)
	player.health = campaign.max_health()
	camera = Camera2D.new()
	camera.position = player.position
	camera.position_smoothing_enabled = true
	camera.position_smoothing_speed = 7.0
	camera_view_idx = campaign.settings.get("camera_view", 0)
	var init_zoom = camera_views[camera_view_idx]
	camera.zoom = Vector2(init_zoom, init_zoom)
	camera.limit_left = 0
	camera.limit_top = 0
	camera.limit_right = 2508
	camera.limit_bottom = 2508
	add_child(camera)
	paint = minf(campaign.max_paint(), 100 + campaign.rank_of("hq") * 20)
	ammo = 24 + campaign.rank_of("gear") * 6
	weapon = true
	stamina = 100
	heat = 0
	penalty = ""
	penalty_time = 0
	stash = 0
	combo = 0
	combo_time = 0
	contracts_done = 0
	deliveries = 0
	shipment = false
	captains_defeated = 0
	contract = {}
	final_hold = 0
	mission_clock = 0
	notice = ""
	notice_time = 0
	spawn_clock = 0
	save_clock = 0
	cash_clock = 0
	crew_order = "FOLLOW"
	respawn_invulnerability = 3
	for i in mini(8, 3 + campaign.level):
		spawn_enemy("rival", city.road_point(1 + i % (city.road_x.size() - 1), 1 + i % (city.road_y.size() - 1)))
	for i in 2 + int(campaign.level / 2.0):
		spawn_enemy("cop", city.road_point(city.road_x.size() - 1, i % city.road_y.size()))
	if campaign.level >= 6:
		spawn_enemy("rival", city.road_point(2, 2))
		enemies[-1].badge = "CAPTAIN"
		enemies[-1].health = 320.0
	for i in campaign.level:
		city.walls[city.walls.size() - 1 - i].owner = "rival"
	new_contract()

func create_actor(kind: String, point: Vector2) -> CharacterBody2D:
	var actor = Actor.new()
	actor.kind = kind
	actor.position = point
	actors.add_child(actor)
	return actor

func spawn_enemy(kind: String, point: Vector2) -> void:
	var enemy = create_actor(kind, point)
	enemy.target_wall = randi_range(0, city.walls.size() - 1)
	enemy.ai_timer = randf_range(3, 8)
	enemies.append(enemy)

func recruit() -> bool:
	if crew.size() >= campaign.max_crew():
		announce("Crew full. Upgrade crew training for another slot.")
		return false
	var member = create_actor("crew", player.position + Vector2(0, 18))
	member.set_outfit(campaign.outfit)
	crew.append(member)
	announce("Crew joined. %d / %d slots" % [crew.size(), campaign.max_crew()])
	return true

func _unhandled_key_input(event: InputEvent) -> void:
	if not event.is_pressed() or event.is_echo():
		return
	var key = event.physical_keycode
	if key in [KEY_ESCAPE, KEY_P]:
		if mode == "play":
			mode = "pause"
			ui.show_pause()
		elif mode in ["pause", "shop", "hq", "black_market", "help"]:
			resume()
		return
	if mode != "play" or not penalty.is_empty():
		return
	match key:
		KEY_E:
			if player.position.distance_to(city.hq) < 65:
				if shipment:
					shipment = false
					deliveries += 1
					campaign.cash += 100
					campaign.score += 75
					announce("Shipment delivered: +$100 and +75 rep.")
					audio.cue("level")
				mode = "hq"
				ui.show_hq()
			elif player.position.distance_to(city.shop) < 65:
				mode = "shop"
				ui.show_shop()
			elif city.black_market_pos != Vector2(-1, -1) and player.position.distance_to(city.black_market_pos) < 65:
				mode = "black_market"
				ui.show_black_market()
			elif is_instance_valid(city) and player.position.distance_to(city.vending_pos) < 75:
				use_vending_machine()
		KEY_Q:
			if campaign.cleared < 1:
				announce("Crew orders unlock after Canal Ink Yard.")
			else:
				var orders = ["FOLLOW", "GUARD", "REGROUP"]
				crew_order = orders[(orders.find(crew_order) + 1) % 3]
				guard_point = player.position
				announce("Crew order: " + crew_order)
		KEY_C:
			if contract.get("state", "") == "available":
				contract.state = "active"
				announce("Contract accepted: " + city.walls[contract.wall].name)
			elif contract.get("state", "") in ["complete", "failed"]:
				new_contract()
		KEY_H:
			if is_instance_valid(city) and player.position.distance_to(city.lowrider_pos) < 140:
				trigger_lowrider_hop()
			else:
				mode = "help"
				ui.show_help()
		KEY_O:
			toggle_neon_underglow()
		KEY_F5:
			save_run()
		KEY_M:
			ui.map_expanded = not ui.map_expanded
		KEY_TAB:
			toggle_cap()
		KEY_B:
			if boombox_pos != Vector2(-1, -1) and player.position.distance_to(boombox_pos) < 140:
				trigger_soundquake()
			else:
				paint_blast()
		KEY_T:
			var track_name = audio.cycle_radio()
			announce("📻 Mixtape: " + track_name)
		KEY_G:
			smoke_grenade()
		KEY_X:
			drop_boombox()
		KEY_Z:
			activate_dash()
		KEY_J:
			try_subway_travel()
		KEY_R:
			try_flow_tap()
		KEY_V:
			deploy_drone()
		KEY_N:
			deploy_turret()
		KEY_Y:
			throw_turf_flare()
		KEY_K:
			plant_paint_mine()
		KEY_F1:
			cycle_camera_view()
		KEY_U:
			activate_bubble_shield()
		KEY_L:
			activate_slowmo()
		KEY_1, KEY_2, KEY_3, KEY_4, KEY_5, KEY_6:
			active_stamp = key - KEY_1
			apply_stencil_stamp()
		KEY_PERIOD:
			start_heist()

func apply_stencil_stamp() -> void:
	if nearest_wall >= 0 and paint >= 10 and penalty == "":
		paint -= 10
		city.walls[nearest_wall].owner = "crew"
		city.walls[nearest_wall].progress = 1.0
		city.add_floating_text("STAMPED! " + stencil_stamps[active_stamp], player.position + Vector2(0, -35), City.GOLD, 1.4)
		announce("🎨 Rapid Stencil Stamp: " + stencil_stamps[active_stamp])

func activate_bubble_shield() -> void:
	if bubble_shield_timer > 0 or penalty != "":
		return
	if paint < 20:
		announce("Need 20 paint for Bubble Shield Barrier!")
		return
	paint -= 20
	bubble_shield_timer = 4.5
	city.add_floating_text("🛡️ BUBBLE SHIELD ACTIVE!", player.position + Vector2(0, -40), City.TURQUOISE, 1.5)
	announce("🛡️ Pressurized Paint Bubble Shield active for 4.5s!")

func activate_slowmo() -> void:
	if slowmo_timer > 0 or penalty != "":
		return
	slowmo_timer = 6.0
	city.add_floating_text("⚡ ADRENALINE SURGE!", player.position + Vector2(0, -40), City.LIME, 1.5)
	announce("⚡ Adrenaline Surge! Slow-motion paint precision enabled for 6s.")

func cycle_camera_view() -> void:
	camera_view_idx = (camera_view_idx + 1) % camera_views.size()
	campaign.settings["camera_view"] = camera_view_idx
	campaign.save_preferences()
	var view_names = ["CLOSE / ACTION VIEW (1.85x)", "STREET MEDIUM VIEW (1.35x)", "TACTICAL OVERVIEW (0.95x)"]
	announce("🎥 Camera View: " + view_names[camera_view_idx])

func trigger_lowrider_hop() -> void:
	if lowrider_hop_cooldown > 0:
		return
	lowrider_hop_cooldown = 1.0
	city.trigger_lowrider_hop()
	audio.cue("blast")
	# Knockback nearby enemies
	for enemy in enemies:
		if is_instance_valid(enemy) and enemy.position.distance_to(city.lowrider_pos) < 220:
			var push = (enemy.position - city.lowrider_pos).normalized() * 180.0
			enemy.position = city.clamp_point(enemy.position + push)
			enemy.take_hit(20, (enemy.position - city.lowrider_pos).normalized())
	city.add_floating_text("🚗 HYDRAULIC HOP!", city.lowrider_pos + Vector2(0, -60), City.MAGENTA, 1.5)
	announce("🚗 Lowrider Hydraulic Bounce Shockwave activated!")

func trigger_soundquake() -> void:
	if soundquake_cooldown > 0:
		return
	soundquake_cooldown = 2.5
	city.trigger_soundquake(boombox_pos)
	audio.cue("blast")
	for enemy in enemies:
		if is_instance_valid(enemy) and enemy.position.distance_to(boombox_pos) < 280:
			enemy.take_hit(30, (enemy.position - boombox_pos).normalized())
	city.add_floating_text("🔊 BASS SOUNDQUAKE!", boombox_pos + Vector2(0, -50), City.GOLD, 1.6)
	announce("🔊 Boombox Soundquake Bass Blast shattered enemy ranks!")

func toggle_neon_underglow() -> void:
	neon_underglow = not neon_underglow
	announce("✨ Neon Ground Underglow: " + ("ENABLED" if neon_underglow else "DISABLED"))

func use_vending_machine() -> void:
	if campaign.cash < 15 and randf() > 0.4:
		announce("Need $15 for Neon Soda, or kick it again!")
		return
	if campaign.cash >= 15:
		campaign.cash -= 15
	stamina = 100.0
	paint = minf(100.0, paint + 25.0)
	neon_soda_timer = 12.0
	city.add_floating_text("🥤 NEON SODA RUSH! +SPEED", player.position + Vector2(0, -45), City.TURQUOISE, 1.5)
	city.burst(player.position, City.TURQUOISE, 20)
	announce("🥤 Neon Soda energy rush! Unlimited sprint & paint refill.")

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventJoypadButton and event.is_pressed():
		if mode != "play" and not mode in ["pause", "shop", "hq", "black_market", "help"]:
			return
		match event.button_index:
			JOY_BUTTON_START:
				if mode == "play":
					mode = "pause"
					ui.show_pause()
				elif mode in ["pause", "shop", "hq", "black_market", "help"]:
					resume()
			JOY_BUTTON_A:
				activate_dash()
			JOY_BUTTON_B:
				paint_blast()
			JOY_BUTTON_X:
				drop_boombox()
			JOY_BUTTON_Y:
				smoke_grenade()
			JOY_BUTTON_RIGHT_SHOULDER:
				toggle_cap()
			JOY_BUTTON_LEFT_SHOULDER:
				if campaign.cleared >= 1:
					var orders = ["FOLLOW", "GUARD", "REGROUP"]
					crew_order = orders[(orders.find(crew_order) + 1) % 3]
					guard_point = player.position
					announce("Crew order: " + crew_order)
			JOY_BUTTON_DPAD_UP:
				deploy_drone()
			JOY_BUTTON_DPAD_DOWN:
				cycle_camera_view()
			JOY_BUTTON_DPAD_LEFT:
				var track_name = audio.cycle_radio()
				announce("📻 Mixtape: " + track_name)
			JOY_BUTTON_DPAD_RIGHT:
				if contract.get("state", "") == "available":
					contract.state = "active"
					announce("Contract accepted: " + city.walls[contract.wall].name)
				elif contract.get("state", "") in ["complete", "failed"]:
					new_contract()
			JOY_BUTTON_BACK:
				ui.map_expanded = not ui.map_expanded

func deploy_drone() -> void:
	if drone_active or penalty != "":
		return
	if campaign.cash < 40:
		announce("Need $40 to launch Aerial Spray Drone Scout.")
		return
	campaign.cash -= 40
	drone_active = true
	drone_timer = 25.0
	drone_pos = player.position
	city.add_floating_text("🚁 SPRAY DRONE DEPLOYED!", player.position + Vector2(0, -40), City.TURQUOISE, 1.5)
	announce("🚁 Aerial Spray Drone deployed! Scouting rooftops and tagging targets.")

func deploy_turret() -> void:
	if penalty != "":
		return
	if campaign.cash < 50:
		announce("Need $50 for Deployable Paint Turret.")
		return
	campaign.cash -= 50
	paint_turrets.append({"pos": player.position, "life": 35.0, "timer": 0.0})
	city.paint_turrets = paint_turrets
	city.add_floating_text("⚙️ PAINT TURRET ONLINE!", player.position + Vector2(0, -40), City.TURQUOISE, 1.5)
	announce("⚙️ Automated Paint Turret deployed! Auto-spraying and defending area.")

func throw_turf_flare() -> void:
	if penalty != "":
		return
	if campaign.cash < 30:
		announce("Need $30 for Turf War Flare Marker.")
		return
	campaign.cash -= 30
	turf_flares.append({"pos": player.position, "life": 25.0})
	city.turf_flares = turf_flares
	recruit()
	recruit()
	city.add_floating_text("🔥 TURF FLARE DEPLOYED!", player.position + Vector2(0, -40), City.GOLD, 1.5)
	announce("🔥 Turf War Flare deployed! Crew backup summoned.")

func plant_paint_mine() -> void:
	if penalty != "":
		return
	if campaign.cash < 25:
		announce("Need $25 for Paint Splatter Mine Trap.")
		return
	campaign.cash -= 25
	paint_mines.append({"pos": player.position, "active": true})
	city.paint_mines = paint_mines
	city.add_floating_text("💣 PAINT MINE ARMED!", player.position + Vector2(0, -40), City.MAGENTA, 1.5)
	announce("💣 Paint Splatter Landmine armed!")

func road_rand_pos() -> Vector2:
	var rx = city.road_x[randi() % city.road_x.size()]
	var ry = city.road_y[randi() % city.road_y.size()]
	return Vector2(rx + randf_range(-60, 60), ry + randf_range(-60, 60))

func toggle_cap() -> void:
	spray_cap = "SKINNY" if spray_cap == "FAT" else "FAT"
	audio.cue("spray_shake")
	announce("Spray Cap: %s NOZZLE (%s)" % [spray_cap, "Fast & Wide" if spray_cap == "FAT" else "Precision & Low Heat"])

func smoke_grenade() -> void:
	if smoke_cooldown > 0 or penalty != "":
		return
	if smoke_bombs <= 0:
		announce("Out of smoke bombs! Buy more at the shop.")
		return
	smoke_bombs -= 1
	smoke_cooldown = 3.0
	heat = maxf(0, heat - 30.0)
	city.add_smoke_cloud(player.position, 5.5)
	audio.cue("blast")
	# Break line-of-sight — cops can't chain-arrest through smoke
	for enemy in enemies:
		if player.position.distance_to(enemy.position) < 200:
			enemy.ai_timer = 3.5
	announce("💨 SMOKE GRENADE! Heat -30, pursuit broken.")

func drop_boombox() -> void:
	if boombox_cooldown > 0 or boombox_timer > 0 or penalty != "":
		return
	boombox_pos = player.position
	boombox_timer = 22.0
	boombox_cooldown = 35.0
	city.boombox_active = boombox_pos
	city.add_floating_text("🎵 BOOMBOX DROPPED!", boombox_pos + Vector2(0, -30), City.GOLD, 1.3)
	announce("🎵 Boombox live! Enemies distracted — crew speed boosted.")

func activate_dash() -> void:
	if dash_cooldown > 0 or stamina < 20 or penalty != "":
		return
	dash_dir = player.facing if player.facing != Vector2.ZERO else Vector2.RIGHT
	dash_timer = 0.22
	dash_cooldown = 1.8
	stamina = maxf(0, stamina - 20)
	hit_shake = maxf(hit_shake, 1.2)
	city.burst(player.position, City.TURQUOISE, 12)

func try_subway_travel() -> void:
	if penalty != "":
		return
	var nearest_sub = Vector2(-1, -1)
	var nearest_dist = 80.0
	for sub_pos in city.subway_stations:
		var d = player.position.distance_to(sub_pos)
		if d < nearest_dist:
			nearest_dist = d
			nearest_sub = sub_pos
	if nearest_sub == Vector2(-1, -1):
		announce("No subway entrance nearby. Approach a METRO 404 entrance.")
		return
	# Find another station to teleport to
	var destinations = city.subway_stations.filter(func(sp): return sp != nearest_sub)
	if destinations.is_empty():
		return
	var dest: Vector2 = destinations.pick_random()
	player.position = dest
	for member in crew:
		member.position = dest + Vector2.from_angle(randf() * TAU) * 30
	hit_shake = 2.0
	city.burst(dest, City.TURQUOISE, 30)
	city.add_floating_text("🚇 METRO TRANSIT!", dest + Vector2(0, -40), City.TURQUOISE, 1.4)
	announce("🚇 Subway fast travel! Arrived at " + str(dest))

func try_flow_tap() -> void:
	if not player.painting or nearest_wall < 0:
		return
	# Flow meter: tap R in a 0.4–0.8 second window after starting to spray
	flow_tap_timer = 0.5
	if flow_tap_window > 0.1 and flow_tap_window < 0.8:
		flow_bonus = true
		flow_bonus_timer = 8.0
		city.burst(player.position, City.GOLD, 20)
		city.add_floating_text("⚡ PERFECT FLOW!", player.position + Vector2(0, -40), City.GOLD, 1.5)
		announce("⚡ PERFECT FLOW! Paint speed x1.5 for 8s.")

func start_heist() -> void:
	if heist_active or penalty != "" or campaign.cleared < 2:
		if campaign.cleared < 2:
			announce("Heist contracts unlock after District 2.")
		return
	heist_active = true
	heist_cargo = false
	# Spawn a rival paint delivery van somewhere on the map
	heist_target = road_rand_pos()
	heist_timer = 60.0
	city.add_floating_text("🎒 HEIST STARTED!", player.position + Vector2(0, -40), City.GOLD, 1.4)
	announce("🎒 Rival paint van spotted! Reach it within 60s and bring the cargo to HQ.")

func kick_hydrant() -> void:
	for hyd_pos in city.hydrants:
		if player.position.distance_to(hyd_pos) < 55:
			# Burst water on nearby enemies
			for enemy in enemies:
				if hyd_pos.distance_to(enemy.position) < 140:
					var push_dir = hyd_pos.direction_to(enemy.position)
					enemy.position = city.clamp_point(enemy.position + push_dir * 120)
					enemy.hurt_time = 0.3
			heat = maxf(0, heat - 20)
			city.add_floating_text("💧 HYDRANT BURST! Heat -20", hyd_pos + Vector2(0, -30), Color("59b7ff"), 1.3)
			city.burst(hyd_pos, Color(0.4, 0.8, 1.0), 30)
			audio.cue("blast")
			announce("💧 Fire hydrant burst! Enemies pushed back, heat reduced.")
			return
	announce("No fire hydrant nearby to kick!")

func paint_blast() -> void:
	if blast_cooldown > 0 or penalty != "":
		return
	if paint < 25.0:
		announce("Need at least 25 paint for Paint Blast!")
		return
	paint -= 25.0
	blast_cooldown = 1.8
	audio.cue("blast")
	city.add_paint_bomb(player.position, City.TURQUOISE)
	hit_shake = maxf(hit_shake, 3.5)
	heat = maxf(0, heat - 15.0)
	
	# Blast radius stun & knockback
	for enemy in enemies:
		var d = player.position.distance_to(enemy.position)
		if d < 185.0:
			enemy.health -= 35.0
			enemy.hurt_time = 0.45
			var knock_dir = player.position.direction_to(enemy.position)
			enemy.position = city.clamp_point(enemy.position + knock_dir * 85.0)
			city.add_floating_text("BLINDED! -35", enemy.position + Vector2(0, -28), City.TURQUOISE, 1.2)
	announce("💥 PAINT BOMB DETONATED! Enemies stunned.")

func resume() -> void:
	mode = "play"
	ui.show_hud()

func _physics_process(delta: float) -> void:
	if mode != "play":
		return
	mission_clock += delta
	notice_time = maxf(0, notice_time - delta)
	respawn_invulnerability = maxf(0, respawn_invulnerability - delta)
	blast_cooldown = maxf(0, blast_cooldown - delta)
	smoke_cooldown = maxf(0, smoke_cooldown - delta)
	boombox_cooldown = maxf(0, boombox_cooldown - delta)
	dash_cooldown = maxf(0, dash_cooldown - delta)
	flow_tap_timer = maxf(0, flow_tap_timer - delta)
	flow_bonus_timer = maxf(0, flow_bonus_timer - delta)
	if flow_bonus_timer <= 0:
		flow_bonus = false
	overdrive_timer = maxf(0, overdrive_timer - delta)
	if overdrive_timer <= 0:
		overdrive_kind = ""
	neon_soda_timer = maxf(0, neon_soda_timer - delta)
	lowrider_hop_cooldown = maxf(0, lowrider_hop_cooldown - delta)
	soundquake_cooldown = maxf(0, soundquake_cooldown - delta)
	
	# Subway updraft steam vent launch
	if is_instance_valid(city) and player.position.distance_to(city.subway_grate_pos) < 55:
		player.position += Vector2(0, -40)
		city.burst(player.position, Color(0.9, 0.95, 1.0, 0.8), 8)
	
	# Boombox decay
	if boombox_timer > 0:
		boombox_timer -= delta
		city.boombox_active = boombox_pos
		# Boombox pulls enemies away from boombox pos not player
		if boombox_timer <= 0:
			boombox_pos = Vector2(-1, -1)
			city.boombox_active = Vector2(-1, -1)
	else:
		city.boombox_active = Vector2(-1, -1)
	
	# Active dash movement override
	if dash_timer > 0:
		dash_timer -= delta
		player.velocity = dash_dir * 580
		player.move_and_slide()
		player.position = city.clamp_point(player.position)
		# Dash shoves nearby enemies
		for enemy in enemies:
			if player.position.distance_to(enemy.position) < 45:
				enemy.position = city.clamp_point(enemy.position + dash_dir * 90)
				enemy.health -= 8
				enemy.hurt_time = 0.2
		city.burst(player.position, City.TURQUOISE, 4)
	
	# Aerial Spray Drone Scout physics
	if drone_active:
		drone_timer -= delta
		drone_pos += Vector2.from_angle(mission_clock * 1.5) * 160.0 * delta
		city.drone_pos = drone_pos
		var target_w = city.nearest
		if target_w >= 0 and city.walls[target_w].owner != "crew":
			city.walls[target_w].progress = minf(1.0, city.walls[target_w].progress + delta * 0.4)
			if city.walls[target_w].progress >= 1.0:
				tag_wall(target_w, delta)
		if drone_timer <= 0:
			drone_active = false
			city.drone_pos = Vector2(-1, -1)
			announce("🚁 Drone battery depleted — returned to base.")
	else:
		city.drone_pos = Vector2(-1, -1)

	# Batch 4 — Turrets, Mines, News Chopper & K9
	for i in range(paint_turrets.size() - 1, -1, -1):
		var pt = paint_turrets[i]
		pt.life -= delta
		pt.timer += delta
		if pt.life <= 0:
			paint_turrets.remove_at(i)
			continue
		if pt.timer > 0.3:
			pt.timer = 0.0
			city.burst(pt.pos, City.TURQUOISE, 3)
			for enemy in enemies:
				if pt.pos.distance_to(enemy.position) < 220:
					enemy.health -= 6
					enemy.hurt_time = 0.2
	city.paint_turrets = paint_turrets

	for i in range(paint_mines.size() - 1, -1, -1):
		var pm = paint_mines[i]
		for enemy in enemies:
			if pm.pos.distance_to(enemy.position) < 35:
				enemy.health -= 45
				enemy.hurt_time = 0.5
				city.burst(pm.pos, City.MAGENTA, 25)
				city.add_floating_text("MINE DETONATED! -45", pm.pos + Vector2(0, -30), City.MAGENTA, 1.4)
				paint_mines.remove_at(i)
				break
	city.paint_mines = paint_mines

	for i in range(turf_flares.size() - 1, -1, -1):
		turf_flares[i].life -= delta
		if turf_flares[i].life <= 0:
			turf_flares.remove_at(i)
	city.turf_flares = turf_flares

	if heat > 50:
		news_chopper_pos = player.position + Vector2.from_angle(mission_clock * 0.8) * 140 + Vector2(0, -180)
		city.news_chopper_pos = news_chopper_pos
	else:
		city.news_chopper_pos = Vector2(-1, -1)

	if heat > 70 and k9_units.size() < 2:
		if randf() < delta * 0.1:
			var k9_p = road_rand_pos()
			k9_units.append({"pos": k9_p, "health": 40.0})
	for i in range(k9_units.size() - 1, -1, -1):
		var k9 = k9_units[i]
		var dir = k9.pos.direction_to(player.position)
		k9.pos += dir * 180 * delta
		if k9.pos.distance_to(player.position) < 25:
			player.health -= delta * 15
			hit_shake = maxf(hit_shake, 1.5)
	city.k9_units = k9_units

	# Batch 5 Timers (Slowmo, Bubble Shield, Flamethrower)
	slowmo_timer = maxf(0, slowmo_timer - delta)
	Engine.time_scale = 0.5 if slowmo_timer > 0 else 1.0
	bubble_shield_timer = maxf(0, bubble_shield_timer - delta)
	if bubble_shield_timer > 0:
		for enemy in enemies:
			if player.position.distance_to(enemy.position) < 65:
				var push_dir = player.position.direction_to(enemy.position)
				enemy.position = city.clamp_point(enemy.position + push_dir * 120 * delta)
	
	# Time of day cycle
	time_of_day = fmod(time_of_day + delta * 0.004, 1.0)
	if time_of_day < 0.33:
		weather_phase = "MIDNIGHT"
	elif time_of_day < 0.66:
		weather_phase = "DAWN"
	else:
		weather_phase = "DUSK"
	city.weather_phase = weather_phase
	
	# Raid defense alert
	if raid_timer > 0:
		raid_timer -= delta
		if raid_timer <= 0 and raid_wall >= 0 and raid_wall < city.walls.size():
			var rw = city.walls[raid_wall]
			if rw.owner == "crew":
				campaign.cash += 120
				campaign.score += 80
				city.add_floating_text("RAID DEFENDED! +$120", rw.pos, City.GOLD, 1.5)
				announce("Sector defended! +$120 bonus.")
			raid_wall = -1
	
	# Heist cargo escort
	if heist_active and heist_cargo:
		heist_timer -= delta
		if heist_timer <= 0:
			heist_cargo = false
			heist_active = false
			announce("Heist failed — cargo expired!")
			city.add_floating_text("HEIST FAILED!", player.position + Vector2(0, -40), City.RED, 1.4)
	if heist_cargo and player.position.distance_to(city.hq) < 65:
		heist_cargo = false
		heist_active = false
		campaign.cash += 200
		campaign.score += 120
		announce("Cargo delivered to HQ! +$200 payday.")
		city.burst(city.hq, City.GOLD, 50)
		city.add_floating_text("CARGO DELIVERED! +$200", city.hq + Vector2(0, -50), City.GOLD, 1.6)
	audio.set_spraying(false)
	
	# Neon puddle ripple update
	for i in range(city.neon_puddles.size() - 1, -1, -1):
		city.neon_puddles[i].ripple = fmod(city.neon_puddles[i].ripple + delta * 2.0, TAU)
	
	# Spawn civilian crowds periodically
	if randf() < delta * 0.3 and city.civilians.size() < 12:
		var cx = road_rand_pos()
		city.civilians.append({"pos": cx, "vel": Vector2.from_angle(randf() * TAU) * randf_range(20, 45), "kind": randi() % 3, "speech": "", "speech_timer": 0.0})
	for i in range(city.civilians.size() - 1, -1, -1):
		var civ = city.civilians[i]
		civ.pos += civ.vel * delta
		civ.speech_timer = maxf(0, civ.speech_timer - delta)
		if not city.bounds.has_point(civ.pos):
			city.civilians.remove_at(i)
			continue
		# Civilians flee shooting
		if heat > 40 and civ.speech_timer <= 0 and player.position.distance_to(civ.pos) < 180:
			civ.vel = civ.pos.direction_to(player.position + Vector2(randf_range(-200, 200), randf_range(-200, 200))) * -60
			civ.speech = ["LOOK OUT!", "SCATTER!", "HEAT'S ON!"].pick_random()
			civ.speech_timer = 3.5
		# Civilians cheer combo
		elif combo >= 3 and civ.speech_timer <= 0 and player.position.distance_to(civ.pos) < 220:
			civ.speech = ["FRESH!", "404 UP!", "WILDSTYLE!", "KING!"].pick_random()
			civ.speech_timer = 2.5
			if randf() < 0.15:
				campaign.cash += 2
	
	# Police roadblocks when heat is critical
	if heat > 60 and city.roadblocks.size() < 3:
		if randf() < delta * 0.08:
			var rp = road_rand_pos()
			city.roadblocks.append({"pos": rp, "life": 28.0, "angle": randf() * TAU})
	for i in range(city.roadblocks.size() - 1, -1, -1):
		city.roadblocks[i].life -= delta
		if city.roadblocks[i].life <= 0 or heat < 20:
			city.roadblocks.remove_at(i)
			continue
		# Slow player if they drive through roadblock
		if player.position.distance_to(city.roadblocks[i].pos) < 32:
			heat = minf(100, heat + delta * 6)
	
	# Black market van random appearance
	black_market_timer -= delta
	if black_market_timer <= 0:
		if city.black_market_pos == Vector2(-1, -1):
			# Spawn van in a secluded spot periodically
			if randf() < 0.06:
				city.black_market_pos = road_rand_pos()
				black_market_timer = 45.0
				announce("🚐 Black Market van spotted in the alley. Approach to browse.")
		else:
			city.black_market_pos = Vector2(-1, -1)
			black_market_timer = randf_range(30, 90)
	
	city.rain = bool(campaign.settings.rain)
	city.city_heat = heat
	city.tick(delta)
	if not penalty.is_empty():
		penalty_time -= delta
		player.velocity = Vector2.ZERO
		audio.set_spraying(false)
		if penalty_time <= 0:
			player.position = city.hq
			player.health = campaign.max_health()
			penalty = ""
			heat = 0
			respawn_invulnerability = 6
			announce("Back at HQ. Your stash survived. Supplies are nearby.")
	else:
		if dash_timer <= 0:
			update_player(delta)
	audio.set_spraying(player.painting and penalty.is_empty())
	update_enemies(delta)
	update_crew(delta)
	update_bullets(delta)
	update_territory(delta)
	update_contract(delta)
	combo_time = maxf(0, combo_time - delta)
	if combo_time <= 0:
		combo = 0
	for actor in enemies + crew + [player]:
		if is_instance_valid(actor):
			actor.tick_visual(delta)
	
	# Overdrive perk visuals
	if overdrive_kind != "":
		city.burst(player.position, City.TURQUOISE if overdrive_kind == "WILDSTYLE" else City.GOLD, 3)
	
	# Smooth camera look-ahead
	var look_target = player.position + player.facing * 32.0
	camera.position = camera.position.lerp(look_target, delta * 8.0)
	var target_zoom_val = camera_views[camera_view_idx]
	camera.zoom = camera.zoom.lerp(Vector2(target_zoom_val, target_zoom_val), delta * 6.0)
	hit_shake = maxf(0, hit_shake - delta * 18)
	camera.offset = Vector2(randf_range(-hit_shake, hit_shake), randf_range(-hit_shake, hit_shake)) if campaign.settings.shake else Vector2.ZERO
	spawn_clock += delta
	if spawn_clock > 18 and enemies.size() < 5 + campaign.level:
		spawn_clock = 0
		spawn_enemy("rival", city.road_point(city.road_x.size() - 1, city.road_y.size() - 1))
	save_clock += delta
	if save_clock > 20:
		save_clock = 0
		save_run(false)
	queue_redraw()

func update_player(delta: float) -> void:
	var kb_dir = Vector2(float(Input.is_physical_key_pressed(KEY_D) or Input.is_physical_key_pressed(KEY_RIGHT)) - float(Input.is_physical_key_pressed(KEY_A) or Input.is_physical_key_pressed(KEY_LEFT)), float(Input.is_physical_key_pressed(KEY_S) or Input.is_physical_key_pressed(KEY_DOWN)) - float(Input.is_physical_key_pressed(KEY_W) or Input.is_physical_key_pressed(KEY_UP)))
	var joy_dir = Vector2(Input.get_joy_axis(0, JOY_AXIS_LEFT_X), Input.get_joy_axis(0, JOY_AXIS_LEFT_Y))
	var direction = kb_dir if kb_dir != Vector2.ZERO else (joy_dir if joy_dir.length() > 0.25 else Vector2.ZERO)
	if direction != Vector2.ZERO:
		direction = direction.normalized()
		
	var joy_aim = Vector2(Input.get_joy_axis(0, JOY_AXIS_RIGHT_X), Input.get_joy_axis(0, JOY_AXIS_RIGHT_Y))
	if joy_aim.length() > 0.3:
		player.facing = joy_aim.normalized()

	var rt_pressed = Input.get_joy_axis(0, JOY_AXIS_TRIGGER_RIGHT) > 0.3
	var lt_pressed = Input.get_joy_axis(0, JOY_AXIS_TRIGGER_LEFT) > 0.3
	var sprinting = (Input.is_physical_key_pressed(KEY_SHIFT) or Input.is_joy_button_pressed(0, JOY_BUTTON_LEFT_STICK)) and stamina > 1 and direction != Vector2.ZERO
	stamina = clampf(stamina + (-29 if sprinting else 21) * delta, 0, 100)
	nearest_wall = find_nearest_wall()
	city.nearest = nearest_wall
	player.painting = (Input.is_physical_key_pressed(KEY_SPACE) or rt_pressed) and nearest_wall >= 0 and paint > 0 and city.walls[nearest_wall].owner != "crew"
	if player.painting:
		direction = Vector2.ZERO
		tag_wall(nearest_wall, delta)
		flow_tap_window += delta
		if Input.is_physical_key_pressed(KEY_R):
			try_flow_tap()
	else:
		flow_tap_window = 0.0
	
	# Overdrive speed boost
	var overdrive_speed = 1.4 if overdrive_kind == "BOMBING" else 1.0
	var boombox_boost = 1.3 if boombox_timer > 0 and player.position.distance_to(boombox_pos) < 220 else 1.0
	var speed = (170.0 + campaign.rank_of("speed") * 18) * (1.55 if sprinting else 1.0) * (0.88 if shipment else 1.0) * overdrive_speed * boombox_boost
	player.velocity = player.velocity.move_toward(direction * speed, 1600 * delta)
	player.move_and_slide()
	player.position = city.clamp_point(player.position)
	if Input.is_mouse_button_pressed(MOUSE_BUTTON_RIGHT):
		paint_blast()
	if lt_pressed or Input.is_physical_key_pressed(KEY_F) or Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT):
		fire_player(Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT) or lt_pressed)
	if Input.is_physical_key_pressed(KEY_E):
		kick_hydrant()
	# Heist cargo pickup
	if heist_active and not heist_cargo and player.position.distance_to(heist_target) < 60:
		heist_cargo = true
		heist_timer = 55.0
		city.add_floating_text("🎒 CARGO GRABBED!", player.position + Vector2(0, -40), City.GOLD, 1.5)
		announce("Cargo grabbed! Rush back to HQ!")
	# Black market interaction
	if city.black_market_pos != Vector2(-1, -1) and player.position.distance_to(city.black_market_pos) < 55:
		if not mode in ["shop", "menu"]:
			mode = "shop"
			ui.show_black_market()
	if not player.painting:
		heat = maxf(0, heat - delta * 4.0)
	# Overdrive loot vacuum (King mode)
	if overdrive_kind == "KING":
		for pickup in city.pickups:
			if pickup.timer <= 0 and player.position.distance_to(pickup.pos) < 120:
				collect_pickup(pickup)
	for pickup in city.pickups:
		if pickup.timer <= 0 and player.position.distance_to(pickup.pos) < 27:
			collect_pickup(pickup)

func find_nearest_wall() -> int:
	var best = -1
	var distance = 64.0
	for i in city.walls.size():
		var d = player.position.distance_to(city.walls[i].pos)
		if d < distance:
			distance = d
			best = i
	return best

func tag_wall(index: int, delta: float) -> void:
	var wall: Dictionary = city.walls[index]
	if wall.owner == "crew":
		return
	# Wildstyle overdrive: infinite paint
	if overdrive_kind == "WILDSTYLE":
		paint = maxf(paint, 30)
	if paint <= 0:
		return
	var cap_speed_mult = 1.35 if spray_cap == "FAT" else 0.85
	var cap_cost_mult = 1.15 if spray_cap == "FAT" else 0.70
	var cap_heat_mult = 1.1 if spray_cap == "FAT" else 0.5
	var flow_mult = 1.5 if flow_bonus else 1.0
	
	var rate = ((1.0 + campaign.rank_of("paint") * 0.12) / (2.0 + float(wall.size) * 1.3)) * cap_speed_mult * flow_mult
	var cost_mod = (0.6 if flow_bonus else 1.0)
	var portion = minf(delta * rate, minf(1.0 - wall.progress, paint / (12.0 * wall.size * cap_cost_mult * cost_mod)))
	wall.progress += portion
	paint = maxf(0, paint - portion * 12.0 * wall.size * cap_cost_mult * cost_mod)
	heat = minf(100, heat + delta * 8 * cap_heat_mult)
	
	# Rich aerosol mist and paint flecks
	var mist_count = 3 if spray_cap == "FAT" else 1
	city.spray_cloud(player.position, wall.pos, City.TURQUOISE, mist_count)
	if randf() < 0.35:
		city.burst(wall.pos, City.TURQUOISE, 2)
		city.add_splatter(wall.pos, City.TURQUOISE, 1)
	if wall.progress >= 0.9999:
		claim_wall(index)

func claim_wall(index: int) -> void:
	var wall: Dictionary = city.walls[index]
	if wall.owner == "crew":
		return
	wall.owner = "crew"
	wall.progress = 0.0
	wall.enemy_progress = 0.0
	wall.style = randi() % 6
	combo = mini(combo + 1, 5) if combo_time > 0 else 1
	combo_time = 40.0
	var reward = (12 + int(wall.size) * 4) * combo
	campaign.cash += reward
	campaign.score += 15 * combo
	city.burst(wall.pos, City.TURQUOISE, 40)
	city.add_splatter(wall.pos, City.TURQUOISE, 5)
	city.add_floating_text("+ $" + str(reward) + ("  x" + str(combo) if combo > 1 else ""), wall.pos + Vector2(0, -28), City.TURQUOISE, 1.3)
	
	if combo > 1:
		audio.cue("combo")
	else:
		audio.cue("tag")
	announce("%s claimed  |  x%d chain  |  +$%d" % [wall.name, combo, reward])
	
	# Check 100% district dominance for 24K Golden Mastery Can
	if is_instance_valid(city) and not city.golden_can_unlocked:
		var all_claimed = true
		for w in city.walls:
			if w.owner != "crew":
				all_claimed = false
				break
		if all_claimed:
			city.golden_can_unlocked = true
			campaign.cash += 500
			campaign.score += 300
			city.add_floating_text("🏆 100% DISTRICT DOMINANCE! +$500", player.position + Vector2(0, -60), City.GOLD, 1.8)
			announce("🏆 100% DOMINANCE! 24K Golden Mastery Can Awarded!")
	
	# Overdrive Perks by combo tier
	if combo == 3:
		overdrive_kind = "WILDSTYLE"
		overdrive_timer = 8.0
		city.add_floating_text("⚡ WILDSTYLE! Infinite paint 8s!", wall.pos + Vector2(0, -55), City.TURQUOISE, 1.6)
	elif combo == 4:
		overdrive_kind = "BOMBING"
		overdrive_timer = 10.0
		city.add_floating_text("🔥 BOMBING RUN! Speed x1.4!", wall.pos + Vector2(0, -55), City.GOLD, 1.6)
	elif combo >= 5:
		overdrive_kind = "KING"
		overdrive_timer = 12.0
		city.add_floating_text("👑 KING OF THE CITY!", wall.pos + Vector2(0, -55), City.GOLD, 1.8)
		city.burst(wall.pos, City.GOLD, 80)
	
	# Trigger raid defense on your newly claimed wall (chance-based)
	if raid_wall < 0 and combo >= 2 and randf() < 0.35:
		raid_wall = index
		raid_timer = 60.0
		city.add_floating_text("🚨 RAID INCOMING!", wall.pos + Vector2(0, -70), City.RED, 1.5)
		announce("🚨 Rival raid on " + wall.name + "! Defend for 60s for bonus!")
		# Spawn extra rivals heading for this wall
		for _i in 2:
			spawn_enemy("rival", city.road_point(city.road_x.size() - 1, city.road_y.size() - 1))
			if not enemies.is_empty():
				enemies.back().target_wall = index
	
	if contract.get("state", "") == "active" and int(contract.wall) == index:
		contract.state = "complete"
		contracts_done += 1
		campaign.cash += 90
		campaign.score += 60
		city.add_floating_text("CONTRACT COMPLETE! +$90", wall.pos + Vector2(0, -48), City.GOLD, 1.4)
		announce("Contract complete! +$90 and +60 rep.")

func collect_pickup(pickup: Dictionary) -> void:
	match pickup.kind:
		"paint":
			if paint >= campaign.max_paint():
				return
			paint = minf(campaign.max_paint(), paint + 45)
		"health":
			if player.health >= campaign.max_health():
				return
			player.health = minf(campaign.max_health(), player.health + 45)
		"ammo":
			if ammo >= 180:
				return
			ammo = mini(180, ammo + 18)
		"weapon":
			if weapon and ammo >= 180:
				return
			weapon = true
			ammo = mini(180, ammo + 12)
		"recruit":
			if not recruit():
				return
	pickup.timer = 28.0 if pickup.kind != "recruit" else 60.0
	audio.cue("pickup")
	city.burst(pickup.pos, Color("ffe06d"), 12)
	city.add_floating_text("+" + pickup.kind.to_upper(), pickup.pos, Color("ffe06d"), 1.1)

func fire_player(mouse_aim: bool) -> void:
	if player.cooldown > 0 or not weapon or ammo <= 0 or player.painting:
		return
	var direction: Vector2 = player.facing
	if mouse_aim:
		direction = player.position.direction_to(get_global_mouse_position())
	else:
		var closest: Node2D = null
		var distance = 330.0
		for enemy in enemies:
			var d = player.position.distance_to(enemy.position)
			if d < distance and city.clear_line(player.position, enemy.position):
				distance = d
				closest = enemy
		if closest != null:
			direction = player.position.direction_to(closest.position)
	player.facing = direction
	player.cooldown = 0.30
	player.trigger_muzzle_flash()
	hit_shake = maxf(hit_shake, 1.5)
	ammo -= 1
	heat = minf(100, heat + 12)
	bullets.append({"pos": player.position, "vel": direction * 780, "life": 0.65, "damage": 24 + campaign.rank_of("gear") * 6})
	audio.cue("shot")
	city.burst(player.position + direction * 20, Color("ffe06d"), 5)

func update_bullets(delta: float) -> void:
	for i in range(bullets.size() - 1, -1, -1):
		var bullet = bullets[i]
		var old: Vector2 = bullet.pos
		bullet.pos += bullet.vel * delta
		bullet.life -= delta
		var consumed = not city.clear_line(old, bullet.pos)
		if not consumed:
			for enemy in enemies:
				if Geometry2D.get_closest_point_to_segment(enemy.position, old, bullet.pos).distance_to(enemy.position) < 17:
					enemy.health -= bullet.damage
					enemy.hurt_time = 0.15
					consumed = true
					audio.cue("hit")
					city.burst(enemy.position, Color("ff4158"), 8)
					city.add_floating_text("-" + str(int(bullet.damage)), enemy.position + Vector2(0, -24), Color("ff4158"), 1.0)
					city.add_bullet_impact(enemy.position)
					break
		else:
			city.add_bullet_impact(bullet.pos)
		if consumed or bullet.life <= 0:
			bullets.remove_at(i)

func update_enemies(delta: float) -> void:
	for i in range(enemies.size() - 1, -1, -1):
		var enemy = enemies[i]
		if enemy.health <= 0:
			if enemy.badge == "CAPTAIN":
				captains_defeated += 1
				campaign.cash += 150
				announce("Rival captain defeated. +$150")
			campaign.cash += 10
			enemy.queue_free()
			enemies.remove_at(i)
			continue
		var distance: float = enemy.position.distance_to(player.position)
		var sees_player: bool = penalty.is_empty() and respawn_invulnerability <= 0 and distance < (330 if enemy.kind == "cop" else 190) and city.clear_line(enemy.position, player.position)
		var chase: bool = sees_player and (enemy.kind == "rival" or heat > 18 + campaign.rank_of("lookout") * 4)
		if chase:
			enemy.navigate(player.position, (157 if enemy.kind == "cop" else 130) + campaign.level * 7, delta, city)
			if enemy.kind == "cop" and distance < 23:
				begin_penalty("JAIL", 11.0)
			elif enemy.kind == "rival" and distance < 35 and enemy.cooldown <= 0:
				damage_player(13)
				enemy.cooldown = 1.0
		else:
			var wall: Dictionary = city.walls[enemy.target_wall]
			var target: Vector2 = wall.pos + Vector2(0, 30)
			enemy.navigate(target, 74 + campaign.level * 7, delta, city)
			if enemy.position.distance_to(target) < 25:
				if enemy.kind == "rival" and wall.owner != "rival":
					wall.enemy_progress += delta / (13.0 + campaign.rank_of("hq") * 2.0)
					if wall.enemy_progress >= 1:
						wall.owner = "rival"
						wall.progress = 0
						wall.enemy_progress = 0
						announce("Rivals repainted " + wall.name)
				else:
					enemy.target_wall = randi_range(0, city.walls.size() - 1)
			for member in crew:
				if enemy.position.distance_to(member.position) < 35 and enemy.cooldown <= 0:
					member.health -= 10
					member.hurt_time = 0.12
					enemy.cooldown = 1.1

func update_crew(delta: float) -> void:
	for i in range(crew.size() - 1, -1, -1):
		var member = crew[i]
		if member.health <= 0:
			member.queue_free()
			crew.remove_at(i)
			announce("A crew member is down. Recruit new backup.")
			continue
		var base: Vector2 = guard_point if crew_order == "GUARD" else player.position
		var target: Vector2 = base + Vector2.from_angle(i * 2.4 + 1.5) * 37
		if city.blocked(target):
			target = base
		var opponent: Node2D = null
		if crew_order != "REGROUP" and penalty.is_empty():
			for enemy in enemies:
				if enemy.kind == "rival" and member.position.distance_to(enemy.position) < 155 and city.clear_line(member.position, enemy.position):
					opponent = enemy
					break
		if opponent != null:
			target = opponent.position
			if member.position.distance_to(target) < 120 and member.cooldown <= 0:
				opponent.health -= 16 + campaign.rank_of("crew") * 8
				opponent.hurt_time = 0.15
				member.cooldown = 0.85
				city.burst(target, Color("ffe06d"), 3)
		member.navigate(target, 190 + campaign.rank_of("crew") * 8, delta, city)

func damage_player(amount: float) -> void:
	if not penalty.is_empty() or respawn_invulnerability > 0:
		return
	player.health -= amount * (1.0 - campaign.rank_of("gear") * 0.08)
	player.hurt_time = 0.2
	hit_shake = 3
	audio.cue("hit")
	if player.health <= 0:
		begin_penalty("HOSPITAL", 8.0)

func begin_penalty(kind: String, duration: float) -> void:
	if not penalty.is_empty():
		return
	penalty = kind
	shipment = false
	penalty_time = duration
	combo = 0
	combo_time = 0
	player.painting = false
	player.velocity = Vector2.ZERO
	if kind == "JAIL":
		paint = 0
		ammo = 0
		weapon = false
	else:
		campaign.cash = maxi(0, campaign.cash - 35)
	heat = 0
	audio.cue("alarm")
	announce("%s: the city keeps moving." % kind)
	save_run(false)

func owned_walls() -> int:
	var count = 0
	for wall in city.walls:
		if wall.owner == "crew":
			count += 1
	return count

func update_territory(delta: float) -> void:
	var owned = owned_walls()
	campaign.score += owned * delta * 1.5 * (1.0 + campaign.rank_of("influence") * 0.15)
	cash_clock += delta
	if cash_clock >= 8:
		cash_clock -= 8
		campaign.cash += owned * 2
	var level: Dictionary = campaign.LEVELS[campaign.level]
	var meets = owned >= int(level.walls) and campaign.score >= float(level.score)
	if campaign.level == 1:
		meets = meets and contracts_done >= 1
	elif campaign.level == 2:
		meets = meets and crew.size() >= 2
	elif campaign.level == 3:
		final_hold = final_hold + delta if meets else 0.0
		meets = meets and final_hold >= 30
	elif campaign.level == 4:
		meets = meets and deliveries >= 1
	elif campaign.level == 5:
		meets = meets and contracts_done >= 2
	elif campaign.level == 6:
		meets = meets and captains_defeated >= 1
	elif campaign.level == 7:
		meets = meets and crew.size() >= 3
		final_hold = final_hold + delta if meets else 0.0
		meets = meets and final_hold >= 45
	if meets and penalty.is_empty() and campaign.finish_level():
		mode = "upgrade"
		player.velocity = Vector2.ZERO
		audio.cue("level")
		save_run(false)
		ui.show_upgrades()

func new_contract() -> void:
	var choices: Array[int] = []
	for i in city.walls.size():
		if city.walls[i].owner != "crew":
			choices.append(i)
	if choices.is_empty():
		contract = {"state": "unavailable"}
		return
	contract = {"state": "available", "wall": choices.pick_random(), "time": 150.0}

func update_contract(delta: float) -> void:
	city.contract_wall = int(contract.get("wall", -1)) if contract.get("state") in ["active", "available"] else -1
	if contract.get("state", "") == "active":
		contract.time = maxf(0, contract.time - delta)
		if contract.time <= 0:
			contract.state = "failed"
			announce("Contract expired. Press C for another lead.")
	elif contract.get("state", "") == "available" and city.walls[contract.wall].owner == "crew":
		new_contract()

func next_district() -> void:
	if campaign.completed:
		mode = "play"
		ui.show_hud()
		announce("Campaign complete. Free roam unlocked; all upgrades remain yours.")
	else:
		var saved_stash = stash
		campaign.advance()
		build_world()
		stash = saved_stash
		mode = "play"
		ui.show_hud()
		save_run(false)

func take_shipment() -> void:
	if campaign.cleared < 4 or shipment:
		return
	shipment = true
	announce("Shipment collected. Return to HQ; carrying it slows movement.")
	save_run(false)

func purchase(key: String) -> bool:
	var costs = {"paint": 20, "ammo": 25, "health": 30, "weapon": 45, "recruit": 70}
	if not costs.has(key) or campaign.cash < costs[key]:
		announce("Not enough cash.")
		return false
	if (key == "paint" and paint >= campaign.max_paint()) or (key == "health" and player.health >= campaign.max_health()) or (key == "ammo" and ammo >= 180) or (key == "weapon" and weapon):
		announce("Already full or equipped.")
		return false
	if key == "recruit" and not recruit():
		return false
	match key:
		"paint": paint = minf(campaign.max_paint(), paint + 60)
		"ammo": ammo = mini(180, ammo + 24)
		"health": player.health = minf(campaign.max_health(), player.health + 60)
		"weapon": weapon = true
	campaign.cash -= costs[key]
	audio.cue("buy")
	save_run(false)
	return true

func purchase_black_market(key: String) -> bool:
	var costs = {
		"smoke": 35,
		"overdrive_speed": 50,
		"overdrive_paint": 65,
		"night_vision": 40,
		"stamina_surge": 80
	}
	if not costs.has(key) or campaign.cash < costs[key]:
		announce("Not enough cash for Black Market deal.")
		return false
	match key:
		"smoke":
			smoke_bombs += 3
			announce("💨 Acquired 3 Smoke Bombs!")
		"overdrive_speed":
			overdrive_kind = "NITRO"
			overdrive_timer = 30.0
			announce("⚡ Nitro Overdrive active for 30s!")
		"overdrive_paint":
			overdrive_kind = "FLOW"
			overdrive_timer = 15.0
			announce("🎨 Infinite Paint Flow active for 15s!")
		"night_vision":
			heat = maxf(0, heat - 40)
			announce("🕶️ Night Vision Goggles equipped! Cops evaded, heat lowered.")
		"stamina_surge":
			stamina = 100.0
			announce("▲ Stamina Surge! Fully restored.")
	campaign.cash -= costs[key]
	audio.cue("buy")
	save_run(false)
	return true


func transfer_stash(deposit: bool) -> void:
	if deposit:
		var amount = minf(40, minf(paint, 50 + campaign.rank_of("hq") * 50 - stash))
		paint -= amount
		stash += amount
	else:
		var amount = minf(40, minf(stash, campaign.max_paint() - paint))
		paint += amount
		stash -= amount
	save_run(false)

func save_run(feedback: bool = true) -> bool:
	if not is_instance_valid(city):
		return false
	var success: bool = campaign.save_game(snapshot_world())
	if not success:
		announce(campaign.save_error)
	elif feedback:
		announce("Progress saved.")
	return success

func snapshot_world() -> Dictionary:
	var wall_states = []
	for wall in city.walls:
		wall_states.append({"owner": wall.owner, "progress": wall.progress, "enemy_progress": wall.enemy_progress})
	var enemy_states = []
	for enemy in enemies:
		enemy_states.append({"kind": enemy.kind, "pos": [enemy.position.x, enemy.position.y], "health": enemy.health, "target": enemy.target_wall, "badge": enemy.badge})
	var crew_states = []
	for member in crew:
		crew_states.append({"pos": [member.position.x, member.position.y], "health": member.health})
	var timers = []
	for pickup in city.pickups:
		timers.append(pickup.timer)
	return {"pos": [player.position.x, player.position.y], "health": player.health, "paint": paint, "ammo": ammo, "weapon": weapon, "stamina": stamina, "heat": heat, "walls": wall_states, "crew": crew_states, "enemies": enemy_states, "pickups": timers, "stash": stash, "order": crew_order, "guard": [guard_point.x, guard_point.y], "combo": combo, "combo_time": combo_time, "contract": contract, "contracts_done": contracts_done, "final_hold": final_hold, "penalty": penalty, "penalty_time": penalty_time, "deliveries": deliveries, "shipment": shipment, "captains_defeated": captains_defeated}

func safe_position(value: Variant, fallback: Vector2 = City.HQ) -> Vector2:
	if fallback == City.HQ:
		fallback = city.hq
	if not value is Array or value.size() != 2 or not (value[0] is float or value[0] is int) or not (value[1] is float or value[1] is int):
		return fallback
	var point = city.clamp_point(Vector2(float(value[0]), float(value[1])))
	return fallback if city.blocked(point) else point

func restore_world(data: Dictionary) -> void:
	player.position = safe_position(data.get("pos"))
	player.health = clampf(float(data.get("health", 100)), 0, campaign.max_health())
	paint = clampf(float(data.get("paint", 100)), 0, campaign.max_paint())
	ammo = clampi(int(data.get("ammo", 24)), 0, 180)
	weapon = bool(data.get("weapon", true))
	stamina = clampf(float(data.get("stamina", 100)), 0, 100)
	heat = clampf(float(data.get("heat", 0)), 0, 100)
	stash = clampf(float(data.get("stash", 0)), 0, 50 + campaign.rank_of("hq") * 50)
	combo = clampi(int(data.get("combo", 0)), 0, 5)
	combo_time = clampf(float(data.get("combo_time", 0)), 0, 40)
	contracts_done = maxi(0, int(data.get("contracts_done", 0)))
	final_hold = clampf(float(data.get("final_hold", 0)), 0, 45)
	deliveries = maxi(0, int(data.get("deliveries", 0)))
	shipment = bool(data.get("shipment", false))
	captains_defeated = maxi(0, int(data.get("captains_defeated", 0)))
	crew_order = str(data.get("order", "FOLLOW"))
	if not crew_order in ["FOLLOW", "GUARD", "REGROUP"]:
		crew_order = "FOLLOW"
	guard_point = safe_position(data.get("guard"), player.position)
	penalty = str(data.get("penalty", ""))
	if not penalty in ["", "JAIL", "HOSPITAL"]:
		penalty = ""
	penalty_time = clampf(float(data.get("penalty_time", 0)), 0, 11)
	var walls_saved = data.get("walls", [])
	if walls_saved is Array:
		for i in mini(walls_saved.size(), city.walls.size()):
			if walls_saved[i] is Dictionary:
				var owner = str(walls_saved[i].get("owner", "none"))
				city.walls[i].owner = owner if owner in ["crew", "rival", "none"] else "none"
				city.walls[i].progress = clampf(float(walls_saved[i].get("progress", 0)), 0, 1)
				city.walls[i].enemy_progress = clampf(float(walls_saved[i].get("enemy_progress", 0)), 0, 1)
	if data.get("contract") is Dictionary:
		var saved_contract: Dictionary = data.contract
		if saved_contract.get("state") in ["available", "active", "complete", "failed"] and (saved_contract.get("wall", -1) is float or saved_contract.get("wall", -1) is int):
			var index = int(saved_contract.get("wall", -1))
			if index >= 0 and index < city.walls.size():
				contract = {"state": str(saved_contract.state), "wall": index, "time": clampf(float(saved_contract.get("time", 150)), 0, 150)}
	if data.get("crew") is Array:
		for saved in data.crew.slice(0, campaign.max_crew()):
			if saved is Dictionary:
				var member = create_actor("crew", safe_position(saved.get("pos")))
				member.health = clampf(float(saved.get("health", 100)), 1, 100)
				member.set_outfit(campaign.outfit)
				crew.append(member)
	if data.get("enemies") is Array:
		for enemy in enemies:
			enemy.queue_free()
		enemies.clear()
		for saved in data.enemies.slice(0, 20):
			if saved is Dictionary and saved.get("kind") in ["cop", "rival"]:
				spawn_enemy(saved.kind, safe_position(saved.get("pos"), Vector2(2310, 2190)))
				enemies[-1].badge = "CAPTAIN" if saved.get("badge", "") == "CAPTAIN" else ""
				enemies[-1].health = clampf(float(saved.get("health", 100)), 1, 320 if enemies[-1].badge == "CAPTAIN" else 100)
				enemies[-1].target_wall = clampi(int(saved.get("target", 0)), 0, city.walls.size() - 1)
	if data.get("pickups") is Array:
		for i in mini(data.pickups.size(), city.pickups.size()):
			city.pickups[i].timer = clampf(float(data.pickups[i]), 0, 60)

func apply_settings() -> void:
	audio.config = campaign.settings
	if not qa_mode:
		DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN if campaign.settings.fullscreen else DisplayServer.WINDOW_MODE_WINDOWED)
		if not campaign.save_preferences():
			announce(campaign.save_error)

func _draw() -> void:
	for bullet in bullets:
		var dir = bullet.vel.normalized()
		draw_line(bullet.pos, bullet.pos - dir * 18, Color(1.0, 0.88, 0.4, 0.45), 4.5, true)
		draw_line(bullet.pos, bullet.pos - dir * 14, Color("ffe9a1"), 2.5, true)
		draw_circle(bullet.pos, 3.0, Color.WHITE)

func run_qa() -> void:
	var test = load("res://tests/beta_checks.gd").new()
	add_child(test)
	await test.run(self)
