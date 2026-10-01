extends Node

var failures: Array[String] = []
var checks = 0
var rendered = false

func check(condition: bool, message: String) -> void:
	checks += 1
	if not condition:
		failures.append(message)
		push_error("FAIL: " + message)
	else:
		print("PASS: " + message)

func screenshot(game: Node, name: String) -> void:
	if DisplayServer.get_name() == "headless":
		return
	if is_instance_valid(game.ui.stats):
		var old_mode: String = game.mode
		game.mode = "play"
		game.ui._process(0)
		game.mode = old_mode
	await RenderingServer.frame_post_draw
	var image = game.get_viewport().get_texture().get_image()
	var path = "res://../qa/" + name + ".png"
	DirAccess.make_dir_recursive_absolute("res://../qa")
	check(image.save_png(path) == OK, "Screenshot: " + name)
	var extrema = [1.0, 0.0]
	for y in range(0, image.get_height(), 20):
		for x in range(0, image.get_width(), 20):
			var color = image.get_pixel(x, y)
			extrema[0] = minf(extrema[0], color.get_luminance())
			extrema[1] = maxf(extrema[1], color.get_luminance())
	check(extrema[1] - extrema[0] > 0.2, "Nonblank viewport: " + name)

func run(game: Node) -> void:
	seed(404)
	await get_tree().process_frame
	await screenshot(game, "godot-menu")
	game.start_run()
	game.mode = "qa"
	await get_tree().physics_frame
	check(game.city.walls.size() == 12, "12 reachable tag walls")
	for wall in game.city.walls:
		check(not game.city.blocked(wall.pos + Vector2(0, 30)), "Wall access: " + wall.name)
	check(game.city.blocked(Vector2(450, 380)), "Buildings block movement")
	check(not game.city.clear_line(Vector2(310, 380), Vector2(615, 380)), "Buildings block sight and bullets")
	var route = game.city.path_between(Vector2(310, 380), Vector2(615, 380))
	var prev = Vector2(310, 380)
	var safe_route = true
	for point in route:
		safe_route = safe_route and game.city.clear_line(prev, point)
		prev = point
	check(safe_route and route.size() >= 3, "Godot AStar routes around buildings")
	var advancing_route = game.city.path_between(Vector2(310, 300), Vector2(615, 380))
	check(advancing_route[0].y <= 300, "Replanning continues forward instead of doubling back")
	var cop = game.enemies[-1]
	game.respawn_invulnerability = 0
	game.heat = 80
	cop.position = game.player.position + Vector2(18, 0)
	game.update_enemies(0.016)
	check(game.penalty == "JAIL", "Police physically catch a wanted player")
	game.penalty = ""
	game.penalty_time = 0
	game.heat = 0
	game.paint = 100
	game.ammo = 24
	game.weapon = true
	cop.position = Vector2(2310, 2190)
	var target = game.enemies[0]
	target.position = game.player.position + Vector2(80, 0)
	game.fire_player(false)
	for i in 10:
		game.update_bullets(0.016)
	check(game.ammo == 23 and target.health < 100, "Player fire spends ammo and hits a visible rival")
	target.position = Vector2(1570, 1460)
	var start_paint: float = game.paint
	game.tag_wall(0, 30)
	check(game.city.walls[0].owner == "crew" and game.paint < start_paint, "Tagging consumes paint and claims wall")
	var cash: int = game.campaign.cash
	game.claim_wall(1)
	check(game.combo == 2 and game.campaign.cash > cash, "Tag chains award increasing cash")
	var rep: float = game.campaign.score
	game.update_territory(2)
	check(game.campaign.score > rep, "Territory earns rep over time")
	game.contract = {"state": "active", "wall": 2, "time": 10.0}
	game.claim_wall(2)
	check(game.contracts_done == 1 and game.contract.state == "complete", "Street contract payout")
	game.contract = {"state": "active", "wall": 3, "time": 1.0}
	game.update_contract(2)
	check(game.contract.state == "failed", "Street contracts expire")
	game.campaign.cash = 200
	game.paint = 0
	check(game.purchase("paint") and game.paint == 60 and game.campaign.cash == 180, "Shop purchase updates inventory and money")
	game.campaign.cash = 0
	check(not game.purchase("ammo"), "Shop rejects unaffordable purchases")
	game.transfer_stash(true)
	check(game.stash == 40 and game.paint == 20, "HQ stash deposit")
	game.begin_penalty("JAIL", 11)
	check(game.paint == 0 and game.ammo == 0 and not game.weapon and game.stash == 40, "Jail confiscates inventory but preserves stash")
	var rival = game.enemies[0]
	rival.target_wall = 0
	rival.position = game.city.walls[0].pos + Vector2(0, 30)
	game.update_enemies(14)
	check(game.city.walls[0].owner == "rival", "Rivals repaint while player is jailed")
	game.penalty = ""
	game.transfer_stash(false)
	check(game.paint == 40 and game.stash == 0, "HQ stash withdrawal after jail")
	game.respawn_invulnerability = 0
	game.damage_player(1000)
	check(game.penalty == "HOSPITAL", "Zero health triggers hospital")
	game.penalty = ""
	game.penalty_time = 0
	game.player.health = game.campaign.max_health()
	game.campaign.credits = 3
	check(not game.campaign.buy_upgrade("gear"), "Later upgrade branch stays locked")
	check(game.campaign.buy_upgrade("health") and game.campaign.max_health() == 125, "Chosen upgrade changes real stats")
	game.recruit()
	game.recruit()
	check(game.crew.size() == 2 and not game.recruit(), "Recruitment respects crew capacity")
	game.campaign.cleared = 1
	game.mode = "play"
	var order_key = InputEventKey.new()
	order_key.physical_keycode = KEY_Q
	order_key.pressed = true
	game._unhandled_key_input(order_key)
	check(game.crew_order == "GUARD" and game.guard_point == game.player.position, "Crew guard order anchors to player's location")
	game._unhandled_key_input(order_key)
	check(game.crew_order == "REGROUP", "Crew regroup command")
	game._unhandled_key_input(order_key)
	check(game.crew_order == "FOLLOW", "Crew follow command")
	game.campaign.cleared = 0
	game.mode = "qa"
	game.city.walls[0].owner = "crew"
	game.campaign.score = 200
	game.update_territory(0)
	check(game.campaign.cleared == 1 and game.mode == "upgrade", "First district completion awards upgrade phase")
	var credits: int = game.campaign.credits
	game.update_territory(0)
	check(game.campaign.credits == credits, "No repeated level rewards")
	await screenshot(game, "godot-upgrades")
	game.next_district()
	game.mode = "qa"
	check(game.campaign.level == 1 and game.campaign.score == 0, "Next district resets objectives and keeps upgrades")
	game.campaign.cash = 123
	game.paint = 33
	game.stash = 25
	game.city.walls[1].owner = "crew"
	game.recruit()
	game.penalty = "JAIL"
	game.penalty_time = 4
	check(game.save_run(false), "Atomic save writes successfully")
	game.campaign.cash = 999
	game.start_run(true)
	game.mode = "qa"
	check(game.campaign.cash == 123 and game.paint == 33 and game.stash == 25, "Save restores money, inventory, and stash")
	check(game.city.walls[1].owner == "crew" and game.crew.size() == 1, "Save restores territory and crew")
	check(game.penalty == "JAIL" and game.penalty_time == 4, "Saving cannot skip jail time")
	check(not game.campaign.valid_world({"paint": {"bad": true}}), "Malformed inventory is rejected")
	check(not game.campaign.valid_world({"crew": [{"pos": ["bad", 0]}]}), "Malformed actor positions are rejected")
	game.penalty = ""
	game.penalty_time = 0
	game.campaign.score = 500
	for i in 4:
		game.city.walls[i].owner = "crew"
	game.update_territory(0)
	check(game.campaign.cleared == 1, "Market objective requires contract")
	game.contracts_done = 1
	game.update_territory(0)
	check(game.campaign.cleared == 2, "Market objective completes with contract")
	game.next_district()
	game.mode = "qa"
	game.campaign.score = 800
	for i in 5:
		game.city.walls[i].owner = "crew"
	game.update_territory(0)
	check(game.campaign.cleared == 2, "Railcut requires recruited backup")
	game.recruit()
	game.recruit()
	game.update_territory(0)
	check(game.campaign.cleared == 3, "Railcut crew objective completes")
	game.next_district()
	game.mode = "qa"
	game.campaign.score = 1200
	for i in 7:
		game.city.walls[i].owner = "crew"
	game.update_territory(15)
	check(not game.campaign.completed, "Final district requires a sustained hold")
	game.update_territory(15)
	check(not game.campaign.completed and game.campaign.cleared == 4, "Night chapter opens daytime chapter")
	game.next_district()
	game.mode = "qa"
	check(game.city.chapter == 1 and game.city.map_texture.resource_path.ends_with("city_day.png"), "Second chapter uses the supplied daytime map")
	check(game.city.navigation.get_point_count() == 20, "Daytime map has its own navigation graph")
	check(game.city.blocked(Vector2(380, 650)), "Daytime outer blocks have collision")
	for wall in game.city.walls:
		check(not game.city.blocked(wall.pos + Vector2(0, 30)), "Daytime wall access: " + wall.name)
	game.campaign.score = 500
	for i in 4:
		game.city.walls[i].owner = "crew"
	game.update_territory(0)
	check(game.campaign.cleared == 4, "Cap Alley requires delivery")
	game.take_shipment()
	check(game.shipment, "Supply shop issues shipment")
	game.player.position = game.city.hq
	game.mode = "play"
	var interact = InputEventKey.new()
	interact.physical_keycode = KEY_E
	interact.pressed = true
	game._unhandled_key_input(interact)
	check(game.deliveries == 1 and not game.shipment, "HQ interaction delivers shipment")
	game.mode = "qa"
	game.update_territory(0)
	check(game.campaign.cleared == 5, "Cap Alley delivery objective completes")
	game.next_district()
	game.mode = "qa"
	game.campaign.score = 800
	for i in 6:
		game.city.walls[i].owner = "crew"
	game.contracts_done = 1
	game.update_territory(0)
	check(game.campaign.cleared == 5, "Bass Block requires two contracts")
	game.contracts_done = 2
	game.update_territory(0)
	check(game.campaign.cleared == 6, "Bass Block completes")
	game.next_district()
	game.mode = "qa"
	game.campaign.score = 1100
	for i in 7:
		game.city.walls[i].owner = "crew"
	game.update_territory(0)
	check(game.campaign.cleared == 6, "Sticker Tunnel requires captain defeat")
	for enemy in game.enemies:
		if enemy.badge == "CAPTAIN":
			enemy.health = 0
	game.update_enemies(0)
	game.update_territory(0)
	check(game.captains_defeated == 1 and game.campaign.cleared == 7, "Captain defeat unlocks finale")
	game.next_district()
	game.mode = "qa"
	check(game.campaign.max_crew() >= 3, "Finale cannot be blocked by earlier upgrade choices")
	game.recruit()
	game.recruit()
	game.recruit()
	game.campaign.score = 1500
	for i in 9:
		game.city.walls[i].owner = "crew"
	game.update_territory(44)
	check(not game.campaign.completed, "Finale needs 45 second hold")
	game.update_territory(1)
	check(game.campaign.completed and game.campaign.cleared == 8, "All eight districts can complete")
	game.next_district()
	check(game.campaign.completed and game.campaign.level == 7, "Completion preserves eight-level campaign in free roam")
	game.mode = "qa"
	game.player.position = game.city.shop
	game.camera.position = game.player.position
	game.camera.reset_smoothing()
	game.ui.show_hud()
	game.mode = "play"
	await get_tree().process_frame
	game.mode = "qa"
	await screenshot(game, "godot-daytime")
	game.campaign.reset()
	game.build_world()
	game.mode = "play"
	game.ui.show_hud()
	game.respawn_invulnerability = 100
	var event = InputEventKey.new()
	event.physical_keycode = KEY_D
	event.pressed = true
	Input.parse_input_event(event)
	var start: Vector2 = game.player.position
	for i in 35:
		await get_tree().physics_frame
	event.pressed = false
	Input.parse_input_event(event)
	check(game.player.position.x > start.x + 25, "Real keyboard input moves the player")
	game.mode = "qa"
	await screenshot(game, "godot-gameplay")
	check(game.city.crew_tag != null and game.city.rival_tag != null and game.city.crew_tag != game.city.rival_tag, "Distinct crew and rival graffiti artwork loads")
	game.city.walls[1].owner = "rival"
	game.camera.position = Vector2(610, 520)
	game.camera.reset_smoothing()
	game.tag_wall(0, 1.3)
	check(game.city.walls[0].progress > 0 and game.city.walls[0].owner == "none", "Unfinished graffiti reveals before ownership changes")
	game.city.queue_redraw()
	await screenshot(game, "godot-tag-in-progress")
	game.tag_wall(0, 20)
	check(game.city.walls[0].owner == "crew" and game.city.walls[0].progress == 0, "Finished graffiti remains visible after progress resets")
	game.city.queue_redraw()
	await screenshot(game, "godot-finished-graffiti")
	game.enemies[0].target_wall = 0
	game.enemies[0].position = game.city.walls[0].pos + Vector2(0, 30)
	game.update_enemies(20)
	check(game.city.walls[0].owner == "rival", "Rival repaint replaces completed graffiti")
	game.city.queue_redraw()
	await screenshot(game, "godot-rival-graffiti")
	game.ui.show_shop()
	await screenshot(game, "godot-shop")
	game.ui.show_options()
	await screenshot(game, "godot-options")
	game.ui.show_hq()
	await screenshot(game, "godot-hq")
	game.mode = "play"
	game.ui.show_hud()
	game.player.set_outfit(3)
	game.ui._process(0)
	game.mode = "qa"
	await screenshot(game, "godot-ghost")
	if DisplayServer.get_name() != "headless":
		DisplayServer.window_set_size(Vector2i(960, 540))
		await get_tree().process_frame
		game.ui.show_upgrades()
		await screenshot(game, "godot-compact-upgrades")
	var report = {"checks": checks, "failures": failures, "engine": Engine.get_version_info().string, "renderer": DisplayServer.get_name()}
	DirAccess.make_dir_recursive_absolute("res://../qa")
	var file = FileAccess.open("res://../qa/godot-results.json", FileAccess.WRITE)
	file.store_string(JSON.stringify(report, "\t"))
	file.close()
	DirAccess.remove_absolute(game.campaign.save_path)
	print("BETA CHECKS: %d passed / %d total" % [checks - failures.size(), checks])
	await get_tree().process_frame
	get_tree().quit(0 if failures.is_empty() else 1)
