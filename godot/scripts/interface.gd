extends CanvasLayer

const INK = Color("0d1117")
const PAPER = Color("eff4ed")
const MINT = Color("4cf5d5")
const MAGENTA = Color("ff3b77")
const GOLD = Color("ffe06d")
const PURPLE = Color("c084fc")

var game: Node2D
var root: Control
var column: VBoxContainer
var stats: Label
var mission: Label
var status: Label
var hint: Label
var toast: Label
var combo_banner: Label
var combo_bar: ProgressBar
var contract_label: Label
var error_label: Label
var map_expanded = false
var radar: Control
var health_bar: ProgressBar
var paint_bar: ProgressBar
var stamina_bar: ProgressBar
var heat_bar: ProgressBar
var menu_background: TextureRect
var crests_tex: Texture2D
var time = 0.0

func _ready() -> void:
	layer = 10
	if ResourceLoader.exists("res://assets/district_crests.png"):
		crests_tex = load("res://assets/district_crests.png")
	root = Control.new()
	root.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	root.mouse_filter = Control.MOUSE_FILTER_IGNORE
	add_child(root)
	
	var theme = Theme.new()
	theme.default_font_size = 18
	theme.set_color("font_color", "Label", PAPER)
	theme.set_color("font_color", "Button", PAPER)
	theme.set_color("font_hover_color", "Button", MINT)
	theme.set_color("font_focus_color", "Button", MINT)
	theme.set_color("font_disabled_color", "Button", Color("48525b"))
	theme.set_stylebox("normal", "Button", panel_style(Color("161b22"), Color("30363d"), 4))
	theme.set_stylebox("hover", "Button", panel_style(Color("1f2937"), MINT, 4))
	theme.set_stylebox("focus", "Button", panel_style(Color("1f2937"), MINT, 4))
	theme.set_stylebox("pressed", "Button", panel_style(Color("0f3d35"), MINT, 4))
	theme.set_stylebox("disabled", "Button", panel_style(Color("0f1115"), Color("21262d"), 4))
	root.theme = theme

func panel_style(color: Color, border: Color = Color.TRANSPARENT, radius: int = 4) -> StyleBoxFlat:
	var style = StyleBoxFlat.new()
	style.bg_color = color
	style.set_border_width_all(2)
	style.border_color = border
	style.set_corner_radius_all(radius)
	style.content_margin_left = 18
	style.content_margin_right = 18
	style.content_margin_top = 10
	style.content_margin_bottom = 10
	return style

func clear() -> void:
	for child in root.get_children():
		root.remove_child(child)
		child.queue_free()
	stats = null
	mission = null
	status = null
	hint = null
	toast = null
	combo_banner = null
	combo_bar = null
	contract_label = null
	error_label = null
	radar = null

func label(text: String, size: int = 18, color: Color = PAPER) -> Label:
	var node = Label.new()
	node.text = text
	node.add_theme_font_size_override("font_size", size)
	node.add_theme_color_override("font_color", color)
	node.add_theme_color_override("font_shadow_color", Color(0, 0, 0, 0.8))
	node.add_theme_constant_override("shadow_offset_x", 1)
	node.add_theme_constant_override("shadow_offset_y", 1)
	node.mouse_filter = Control.MOUSE_FILTER_IGNORE
	return node

func button(text: String, action: Callable, disabled: bool = false) -> Button:
	var node = Button.new()
	node.text = text
	node.custom_minimum_size.y = 46
	node.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	node.disabled = disabled
	node.pressed.connect(func():
		if is_instance_valid(game) and is_instance_valid(game.audio):
			game.audio.cue("spray_shake")
		action.call())
	return node

func add_text(text: String, size: int = 18, color: Color = PAPER) -> Label:
	var node = label(text, size, color)
	node.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	column.add_child(node)
	return node

func show_menu() -> void:
	clear()
	menu_background = TextureRect.new()
	menu_background.texture = load("res://assets/title.png")
	menu_background.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	menu_background.stretch_mode = TextureRect.STRETCH_KEEP_ASPECT_COVERED
	menu_background.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	root.add_child(menu_background)
	
	var band = PanelContainer.new()
	band.set_anchors_and_offsets_preset(Control.PRESET_BOTTOM_WIDE)
	band.offset_top = -140
	band.add_theme_stylebox_override("panel", panel_style(Color(0.04, 0.05, 0.07, 0.96), Color("30363d"), 6))
	root.add_child(band)
	
	var menu = VBoxContainer.new()
	menu.add_theme_constant_override("separation", 10)
	band.add_child(menu)
	menu.add_child(label("⚡ STREET SHOOTERS ⚡  /  TURF WARS EXPANSION", 15, MINT))
	
	var row = HBoxContainer.new()
	row.add_theme_constant_override("separation", 14)
	menu.add_child(row)
	row.add_child(button("▶ NEW RUN", func():
		if FileAccess.file_exists(game.campaign.save_path):
			show_new_confirmation()
		else:
			game.start_run()))
	row.add_child(button("↺ CONTINUE", func(): game.start_run(true), not FileAccess.file_exists(game.campaign.save_path)))
	row.add_child(button("⚙ OPTIONS", func(): show_options(true)))
	row.add_child(button("? CONTROLS", func(): show_help(true)))
	row.add_child(button("✕ QUIT", func(): game.get_tree().quit()))
	
	error_label = label("", 14, Color("ff8494"))
	menu.add_child(error_label)

func menu_error(message: String) -> void:
	if is_instance_valid(error_label):
		error_label.text = message

func modal(title: String, subtitle: String = "") -> void:
	clear()
	var shade = ColorRect.new()
	shade.color = Color(0.01, 0.02, 0.03, 0.88)
	shade.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	root.add_child(shade)
	
	var center = CenterContainer.new()
	center.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	root.add_child(center)
	
	var panel = PanelContainer.new()
	panel.custom_minimum_size = Vector2(740, 0)
	panel.add_theme_stylebox_override("panel", panel_style(INK, MINT, 6))
	center.add_child(panel)
	
	column = VBoxContainer.new()
	column.add_theme_constant_override("separation", 12)
	panel.add_child(column)
	
	add_text(title, 28, MINT)
	if not subtitle.is_empty():
		add_text(subtitle, 16, Color("b2bbb9"))

func show_new_confirmation() -> void:
	modal("START A NEW RUN?", "This replaces the saved campaign. Your options are kept.")
	column.add_child(button("CONFIRM & START NEW RUN", func(): game.start_run()))
	column.add_child(button("BACK", show_menu))

func show_hud() -> void:
	clear()
	
	# Top HUD Container
	var top = PanelContainer.new()
	top.set_anchors_and_offsets_preset(Control.PRESET_TOP_WIDE)
	top.offset_bottom = 92
	top.add_theme_stylebox_override("panel", panel_style(Color(0.03, 0.04, 0.05, 0.95), Color("30363d"), 0))
	root.add_child(top)
	
	var box = VBoxContainer.new()
	top.add_child(box)
	
	stats = label("", 17, PAPER)
	box.add_child(stats)
	
	var row = HBoxContainer.new()
	row.add_theme_constant_override("separation", 16)
	box.add_child(row)
	
	health_bar = stat_bar(row, "♥ HEALTH", MAGENTA)
	paint_bar = stat_bar(row, "⚡ PAINT", MINT)
	stamina_bar = stat_bar(row, "▲ STAMINA", GOLD)
	heat_bar = stat_bar(row, "☠ HEAT", Color("59b7ff"))
	
	# Mission Objectives
	mission = label("", 16, GOLD)
	mission.position = Vector2(20, 102)
	root.add_child(mission)
	
	contract_label = label("", 15, PAPER)
	contract_label.position = Vector2(20, 156)
	root.add_child(contract_label)
	
	# Combo Banner
	combo_banner = label("", 22, GOLD)
	combo_banner.position = Vector2(20, 206)
	root.add_child(combo_banner)
	
	combo_bar = ProgressBar.new()
	combo_bar.custom_minimum_size = Vector2(180, 5)
	combo_bar.position = Vector2(20, 238)
	combo_bar.show_percentage = false
	combo_bar.max_value = 40.0
	combo_bar.add_theme_stylebox_override("background", panel_style(Color("101216"), Color.TRANSPARENT, 2))
	combo_bar.add_theme_stylebox_override("fill", panel_style(GOLD, Color.TRANSPARENT, 2))
	root.add_child(combo_bar)
	
	status = label("", 22, Color("ff4158"))
	status.position = Vector2(20, 252)
	root.add_child(status)
	
	# Central / Bottom Notifications
	toast = label("", 19, MINT)
	toast.set_anchors_and_offsets_preset(Control.PRESET_BOTTOM_WIDE)
	toast.offset_top = -104
	toast.offset_bottom = -68
	toast.offset_left = 20
	toast.add_theme_color_override("font_shadow_color", Color.BLACK)
	toast.add_theme_constant_override("shadow_offset_x", 2)
	toast.add_theme_constant_override("shadow_offset_y", 2)
	root.add_child(toast)
	
	# Bottom Action Bar
	var bottom = PanelContainer.new()
	bottom.set_anchors_and_offsets_preset(Control.PRESET_BOTTOM_WIDE)
	bottom.offset_top = -58
	bottom.add_theme_stylebox_override("panel", panel_style(Color(0.03, 0.04, 0.05, 0.95), Color("30363d"), 0))
	root.add_child(bottom)
	
	hint = label("", 16, PAPER)
	bottom.add_child(hint)
	
	# Mini-Radar
	radar = Control.new()
	radar.set_anchors_and_offsets_preset(Control.PRESET_TOP_RIGHT)
	radar.position = Vector2(-192, 106)
	radar.size = Vector2(174, 174)
	radar.mouse_filter = Control.MOUSE_FILTER_IGNORE
	radar.draw.connect(draw_radar)
	root.add_child(radar)

func stat_bar(row: HBoxContainer, title: String, color: Color) -> ProgressBar:
	var box = HBoxContainer.new()
	box.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	row.add_child(box)
	box.add_child(label(title, 13, color))
	
	var bar = ProgressBar.new()
	bar.custom_minimum_size = Vector2(120, 12)
	bar.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	bar.size_flags_vertical = Control.SIZE_SHRINK_CENTER
	bar.show_percentage = false
	bar.add_theme_stylebox_override("background", panel_style(Color("101216"), Color("30363d"), 3))
	bar.add_theme_stylebox_override("fill", panel_style(color, Color.TRANSPARENT, 3))
	box.add_child(bar)
	return bar

func _process(delta: float) -> void:
	time += delta
	if game.mode != "play" or not is_instance_valid(stats):
		return
	var c = game.campaign
	stats.text = "★ %s   |   $%d   |   REP %d   |   AMMO %d   |   CREW %d [%s]   |   CAP: %s   |   x%d" % [c.LEVELS[c.level].name, c.cash, c.score, game.ammo, game.crew.size(), game.crew_order, game.spray_cap, maxi(1, game.combo)]
	
	health_bar.max_value = c.max_health()
	health_bar.value = game.player.health
	paint_bar.max_value = c.max_paint()
	paint_bar.value = game.paint
	stamina_bar.value = game.stamina
	heat_bar.value = game.heat
	
	mission.text = ("👑 CITY CROWNED / FREE ROAM" if c.completed else "DISTRICT %d / %d: %s" % [c.level + 1, c.LEVELS.size(), c.LEVELS[c.level].task]) + "\nWALLS %d / %d   REP %d / %d" % [game.owned_walls(), c.LEVELS[c.level].walls, c.score, c.LEVELS[c.level].score]
	if c.level in [3, 7] and not c.completed:
		mission.text += "   HOLD %ds / %ds" % [game.final_hold, 30 if c.level == 3 else 45]
	if c.level >= 4:
		mission.text += "   SHIPMENTS %d  CONTRACTS %d" % [game.deliveries, game.contracts_done]
		
	var contract = game.contract
	var state = contract.get("state", "")
	if state in ["active", "available"]:
		contract_label.text = "🎯 CONTRACT: %s  |  %s" % [game.city.walls[contract.wall].name, "%ds remaining" % contract.time if state == "active" else "Press C to accept (+$90)"]
	elif state == "unavailable":
		contract_label.text = "🎯 CONTRACTS: All walls currently claimed."
	else:
		contract_label.text = "🎯 CONTRACT %s  |  Press C for next lead" % state.to_upper()
		
	# Combo Banner & Decay
	if game.combo > 1 and game.combo_time > 0:
		var combo_titles = ["", "", "★ FRESH! x2 ★", "🔥 WILDSTYLE x3 🔥", "👑 BOMBING RUN x4 👑", "⚡ KING OF THE CITY x5 ⚡"]
		var title_idx = mini(game.combo, combo_titles.size() - 1)
		combo_banner.text = combo_titles[title_idx]
		combo_banner.visible = true
		combo_bar.visible = true
		combo_bar.value = game.combo_time
	else:
		combo_banner.visible = false
		combo_bar.visible = false
		
	status.text = "%s (%ds)\nRivals repainting exposed territory!" % [game.penalty, ceili(game.penalty_time)] if not game.penalty.is_empty() else ""
	toast.text = game.notice if game.notice_time > 0 else ""
	
	if game.shipment:
		hint.text = "📦 SHIPMENT ON BOARD   |   RETURN TO HQ (E)   |   AVOID POLICE / ARREST"
	elif game.player.position.distance_to(game.city.hq) < 65:
		hint.text = "[E] 404 HEADQUARTERS   |   [TAB] CAP NOZZLE   |   [T] RADIO   |   [P] PAUSE"
	elif game.player.position.distance_to(game.city.shop) < 65:
		hint.text = "[E] NIGHT SUPPLY SHOP   |   [TAB] CAP NOZZLE   |   [T] RADIO   |   [P] PAUSE"
	elif game.city.black_market_pos != Vector2(-1, -1) and game.player.position.distance_to(game.city.black_market_pos) < 65:
		hint.text = "[E] BLACK MARKET VAN   |   ILLEGAL GEAR & OVERDRIVE BOOSTS   |   [P] PAUSE"
	elif game.nearest_wall >= 0:
		var wall: Dictionary = game.city.walls[game.nearest_wall]
		hint.text = "%s   |   %s   |   PAINT %d   |   [TAB] %s CAP   |   [R] FLOW TAP" % [wall.name, "OWNED BY 404 CREW" if wall.owner == "crew" else "HOLD [SPACE] TO SPRAY PAINT", game.paint, game.spray_cap]
	else:
		hint.text = "WASD: MOVE  SHIFT: SPRINT  SPACE: TAG  Z: DASH  G: SMOKE  X: BOOMBOX  J: SUBWAY  R: HYDRANT/FLOW  TAB: CAP  P: PAUSE"
		
	if is_instance_valid(radar):
		radar.queue_redraw()

func draw_radar() -> void:
	var side = 430.0 if map_expanded else 174.0
	var offset = Vector2(174 - side, 0)
	
	# Radar background & glow
	radar.draw_rect(Rect2(offset - Vector2(4, 4), Vector2(side + 8, side + 8)), Color(0.02, 0.03, 0.04, 0.96))
	radar.draw_rect(Rect2(offset - Vector2(4, 4), Vector2(side + 8, side + 8)), MINT, false, 1.5)
	radar.draw_texture_rect(game.city.map_texture, Rect2(offset, Vector2(side, side)), false, Color(0.75, 0.75, 0.75))
	
	# Radar sweep line
	var sweep_angle = fmod(time * 2.5, TAU)
	var sweep_center = offset + Vector2(side * 0.5, side * 0.5)
	radar.draw_line(sweep_center, sweep_center + Vector2.from_angle(sweep_angle) * (side * 0.5), Color(MINT, 0.25), 1.5)
	
	var scale = side / 2508.0
	for wall in game.city.walls:
		var color = MINT if wall.owner == "crew" else Color("ff4158") if wall.owner == "rival" else GOLD
		radar.draw_circle(offset + wall.pos * scale, 3.5 if map_expanded else 2.5, color)
		
	for point in [game.city.hq, game.city.shop]:
		radar.draw_rect(Rect2(offset + point * scale - Vector2(3.5, 3.5), Vector2(7, 7)), GOLD)
		
	for enemy in game.enemies:
		var e_col = Color("59b7ff") if enemy.kind == "cop" else Color("ff4158")
		radar.draw_circle(offset + enemy.position * scale, 2.5, e_col)
		
	for member in game.crew:
		radar.draw_circle(offset + member.position * scale, 2.5, GOLD)
		
	# Player position & facing arrow
	var p_pos = offset + game.player.position * scale
	radar.draw_circle(p_pos, 4.5, Color.WHITE)
	radar.draw_line(p_pos, p_pos + game.player.facing * 7.0, MINT, 2.0)
	
	if game.city.contract_wall >= 0:
		radar.draw_arc(offset + game.city.walls[game.city.contract_wall].pos * scale, 7, 0, TAU, 16, GOLD, 2.0)

func show_pause() -> void:
	modal("PAUSED  /  TURF REPORT", "The city simulation is held.")
	column.add_child(button("▶ RESUME RUN", game.resume))
	column.add_child(button("💾 SAVE PROGRESS", func():
		game.save_run()
		show_pause()))
	column.add_child(button("⚙ OPTIONS & AUDIO", func(): show_options(false)))
	column.add_child(button("? CONTROLS & MANUAL", show_help))
	column.add_child(button("✕ SAVE & EXIT TO MENU", func():
		if game.save_run():
			game.mode = "menu"
			show_menu()))
	add_text(game.notice if game.notice_time > 0 else "", 14, GOLD)

func show_options(from_menu: bool = false) -> void:
	modal("GAMEPLAY & AUDIO OPTIONS")
	for key in ["music", "effects"]:
		var row = HBoxContainer.new()
		column.add_child(row)
		var name_label = label(key.to_upper() + " VOLUME", 16)
		name_label.custom_minimum_size.x = 160
		row.add_child(name_label)
		var slider = HSlider.new()
		slider.min_value = 0
		slider.max_value = 1
		slider.step = 0.05
		slider.value = float(game.campaign.settings[key])
		slider.size_flags_horizontal = Control.SIZE_EXPAND_FILL
		slider.value_changed.connect(func(value):
			game.campaign.settings[key] = value
			game.apply_settings())
		row.add_child(slider)
		
	for key in ["rain", "shake", "fullscreen"]:
		var toggle = CheckButton.new()
		toggle.text = {"rain": "Atmospheric Rain & Weather", "shake": "Combat Screen Shake", "fullscreen": "Fullscreen Window Mode"}[key]
		toggle.button_pressed = game.campaign.settings[key]
		toggle.toggled.connect(func(value):
			game.campaign.settings[key] = value
			game.apply_settings())
		column.add_child(toggle)
		
	add_text("CAMERA VIEW ZOOM", 15, GOLD)
	var cam_select = OptionButton.new()
	cam_select.custom_minimum_size.y = 40
	cam_select.add_item("🔍 CLOSE / ACTION VIEW (1.85x) [RECOMMENDED]")
	cam_select.add_item("🎥 STREET MEDIUM VIEW (1.35x)")
	cam_select.add_item("🗺️ TACTICAL OVERVIEW (0.95x)")
	cam_select.select(game.campaign.settings.get("camera_view", 0))
	cam_select.item_selected.connect(func(index):
		game.campaign.settings["camera_view"] = index
		game.camera_view_idx = index
		game.campaign.save_preferences())
	column.add_child(cam_select)
		
	column.add_child(button("← BACK", func():
		if from_menu:
			show_menu()
		else:
			game.save_run(false)
			show_pause()))

func show_help(from_menu: bool = false) -> void:
	modal("STREET MANUAL & CONTROLS")
	add_text("KEYBOARD & MOUSE CONTROLS:", 16, MINT)
	add_text("WASD / Arrows: Move     Shift: Sprint     Z: Skate Dash     N: Paint Cannon Turret ($50)     F1: Cycle Camera View\nSpace: Hold to Spray Paint     R: Rhythm Flow Tap / Fire Hydrant Blast     K: Paint Mine ($25)\nMouse + Left Click: Aim & Fire Weapon     F: Auto-aim fire nearest enemy     Y: Turf Flare ($30)\nRight Click / B: Paint Bomb Blast (360° Stun)     G: Smoke Grenade (Drop Heat)\nX: Deploy Boombox Decoy     V: Deploy Spray Drone Scout ($40)     J: Metro Subway Teleport\nTab: Switch Spray Cap     T: Change Hip-Hop Mixtape     Q: Cycle Crew Orders (Follow, Guard, Regroup)\nE: Interact HQ / Shop / Black Market Van / Heist Delivery     C: Accept Contract     M: Toggle Radar     P / Esc: Pause", 14)
	add_text("🎮 GAMEPAD / XBOX CONTROLLER SUPPORT (Razer Wolverine TE Ready):", 16, MINT)
	add_text("Left Analog Stick: Move     Right Analog Stick: Aim     D-Pad Down / F1: Cycle Camera Zoom (1.85x / 1.35x / 0.95x)\nRight Trigger (RT): Hold to Spray Paint     Left Trigger (LT): Fire Weapon\nButton A: Skate Dash     Button B: Paint Bomb Blast     Button X: Deploy Boombox     Button Y: Smoke Grenade\nRight Bumper (RB): Switch Cap Nozzle     Left Bumper (LB): Cycle Crew Orders\nD-Pad Up: Spray Drone     Start: Pause Game     Select / Back: Toggle Radar Map", 14)
	add_text("High heat triggers Police pursuits & K9 tracking. Getting busted sends you to jail (HQ stash stays protected!).", 15, GOLD)
	column.add_child(button("← BACK", show_menu if from_menu else game.resume))

func show_upgrades() -> void:
	var c = game.campaign
	modal("👑 CITY CROWNED" if c.completed else "★ DISTRICT COMPLETE ★", "%d upgrade credits available. Spend now or bank for later." % c.credits)
	
	if crests_tex != null:
		var crest_box = CenterContainer.new()
		var crest_rect = TextureRect.new()
		var atlas = AtlasTexture.new()
		atlas.atlas = crests_tex
		var crest_idx = mini(c.cleared, 3)
		var frame_w = float(crests_tex.get_width()) / 4.0
		var frame_h = float(crests_tex.get_height())
		atlas.region = Rect2(crest_idx * frame_w, 0, frame_w, frame_h)
		crest_rect.texture = atlas
		crest_rect.custom_minimum_size = Vector2(90, 90)
		crest_rect.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
		crest_rect.stretch_mode = TextureRect.STRETCH_KEEP_ASPECT_CENTERED
		crest_box.add_child(crest_rect)
		column.add_child(crest_box)
		
	var grid = GridContainer.new()
	grid.columns = 2
	grid.add_theme_constant_override("h_separation", 12)
	grid.add_theme_constant_override("v_separation", 10)
	column.add_child(grid)
	
	for key in c.UPGRADES:
		var spec: Dictionary = c.UPGRADES[key]
		var text = "⚡ %s  [%d / 5]\n%s" % [spec.name, c.rank_of(key), spec.detail]
		if c.cleared < spec.unlock:
			text += "\n(Unlocks after District %d)" % spec.unlock
		var upgrade_button = button(text, func():
			if c.buy_upgrade(key):
				game.save_run(false)
				game.audio.cue("buy")
				show_upgrades(), not c.can_upgrade(key))
		upgrade_button.add_theme_font_size_override("font_size", 15)
		upgrade_button.custom_minimum_size = Vector2(345, 72)
		grid.add_child(upgrade_button)
		
	add_outfits()
	column.add_child(button("ENTER " + ("FREE ROAM" if c.completed else c.LEVELS[mini(c.level + 1, c.LEVELS.size() - 1)].name), game.next_district))

func add_outfits() -> void:
	var c = game.campaign
	add_text("CREW STYLE & OUTFIT", 15, GOLD)
	var select = OptionButton.new()
	select.custom_minimum_size.y = 40
	for i in c.OUTFITS.size():
		select.add_item(c.OUTFITS[i] + (" (Locked)" if i > c.cleared else ""))
		select.set_item_disabled(i, i > c.cleared)
	select.select(c.outfit)
	select.item_selected.connect(func(index):
		c.outfit = index
		game.player.set_outfit(index)
		for member in game.crew:
			member.set_outfit(index)
		game.save_run(false))
	column.add_child(select)

func show_hq() -> void:
	modal("404 CREW HEADQUARTERS", "Stash: %d / %d Paint   |   Carried: %d / %d" % [game.stash, 50 + game.campaign.rank_of("hq") * 50, game.paint, game.campaign.max_paint()])
	column.add_child(button("📥 DEPOSIT 40 PAINT TO STASH", func():
		game.transfer_stash(true)
		show_hq(), game.paint <= 0 or game.stash >= 50 + game.campaign.rank_of("hq") * 50))
	column.add_child(button("📤 WITHDRAW 40 PAINT FROM STASH", func():
		game.transfer_stash(false)
		show_hq(), game.stash <= 0 or game.paint >= game.campaign.max_paint()))
	add_outfits()
	column.add_child(button("⚡ SPEND BANKED CREDITS", show_hq_upgrades, game.campaign.credits <= 0))
	column.add_child(button("← BACK TO THE STREETS", game.resume))

func show_hq_upgrades() -> void:
	show_upgrades()
	var last = column.get_child(column.get_child_count() - 1)
	column.remove_child(last)
	last.queue_free()
	column.add_child(button("← BACK TO HQ", show_hq))

func show_shop() -> void:
	modal("NIGHT SUPPLY DEPOT", "Available Cash: $%d" % game.campaign.cash)
	var offers = {
		"paint": "🎨 60 SPRAY PAINT  /  $20",
		"ammo": "🎯 24 ROUNDS AMMO  /  $25",
		"health": "♥ MEDICAL KIT  /  $30",
		"weapon": "🔫 REPLACEMENT WEAPON  /  $45",
		"recruit": "⚡ RECRUIT CREW BACKUP  /  $70"
	}
	for key in offers:
		column.add_child(button(offers[key], func():
			game.purchase(key)
			show_shop()))
	if game.campaign.cleared >= 4:
		column.add_child(button("📦 COLLECT HQ SHIPMENT (FREE)", func():
			game.take_shipment()
			show_shop(), game.shipment))
	add_text(game.notice if game.notice_time > 0 else "", 14, GOLD)
	column.add_child(button("← BACK TO THE STREETS", game.resume))

func show_black_market() -> void:
	modal("🚐 ALLEYWAY BLACK MARKET VAN", "Illegal Gear & Overdrive Enhancements  |  Cash: $%d" % game.campaign.cash)
	var offers = {
		"smoke": "💨 SMOKE BOMBS (+3)  /  $35",
		"overdrive_speed": "⚡ NITRO OVERDRIVE (Speed x1.4 for 30s)  /  $50",
		"overdrive_paint": "🎨 INFINITE PAINT FLOW (15s)  /  $65",
		"night_vision": "🕶️ NIGHT VISION GOGGLES (Clear Heat -40)  /  $40",
		"stamina_surge": "▲ STAMINA SURGE (Full Restore)  /  $80"
	}
	for key in offers:
		column.add_child(button(offers[key], func():
			game.purchase_black_market(key)
			show_black_market()))
	add_text(game.notice if game.notice_time > 0 else "", 14, GOLD)
	column.add_child(button("← BACK TO THE STREETS", game.resume))
