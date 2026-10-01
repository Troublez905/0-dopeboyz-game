extends CharacterBody2D

var kind = "player"
var health = 100.0
var cooldown = 0.0
var facing = Vector2.DOWN
var frame_time = 0.0
var hurt_time = 0.0
var flash_time = 0.0
var route: PackedVector2Array = []
var route_timer = 0.0
var target_wall = 0
var ai_timer = 0.0
var painting = false
var body_sprite: Sprite2D
var mask_material: ShaderMaterial
var texture_sheet: Texture2D
var outfit = 0
var last_frame = -1
var badge = ""
var ghost_trails: Array[Dictionary] = []

const SHEETS = ["hoodie2", "hoodie1", "hoodie3", "ghost1"]

func _ready() -> void:
	collision_layer = 2
	collision_mask = 1
	var collider = CollisionShape2D.new()
	var circle = CircleShape2D.new()
	circle.radius = 10.0
	collider.shape = circle
	add_child(collider)
	body_sprite = Sprite2D.new()
	body_sprite.position.y = -20
	mask_material = ShaderMaterial.new()
	mask_material.shader = preload("res://shaders/sprite_mask.gdshader")
	body_sprite.material = mask_material
	add_child(body_sprite)
	set_outfit(outfit)

func set_outfit(value: int) -> void:
	outfit = value
	if (badge == "CAPTAIN" or badge == "ENFORCER") and ResourceLoader.exists("res://assets/heavy_enforcer.png"):
		texture_sheet = load("res://assets/heavy_enforcer.png")
	elif kind == "crew" and ResourceLoader.exists("res://assets/roller_queen.png"):
		texture_sheet = load("res://assets/roller_queen.png")
	elif kind in ["rival", "cop"]:
		texture_sheet = load("res://assets/rival.png")
	elif outfit == 4 and ResourceLoader.exists("res://assets/roller_queen.png"):
		texture_sheet = load("res://assets/roller_queen.png")
	else:
		texture_sheet = load("res://assets/" + SHEETS[clampi(outfit, 0, 3)] + ".png")
	last_frame = -1
	if is_instance_valid(body_sprite):
		update_art()

func update_art() -> void:
	var speed = velocity.length()
	var moving = speed > 5
	var is_custom_batch2 = texture_sheet != null and (texture_sheet.resource_path.ends_with("roller_queen.png") or texture_sheet.resource_path.ends_with("heavy_enforcer.png"))
	mask_material.set_shader_parameter("source_alpha", is_custom_batch2)
	var index = int(frame_time * 8.0) % 4 if moving else 0
	if not is_custom_batch2:
		if kind in ["cop", "rival"]:
			index = (6 + int(frame_time * 8.0) % 4) if moving else 0
		elif facing.y < -0.3 or absf(facing.x) > 0.3:
			index += 12
	if index != last_frame:
		last_frame = index
		var region: Rect2
		if is_custom_batch2:
			var frame_w = float(texture_sheet.get_width()) / 4.0
			var frame_h = float(texture_sheet.get_height())
			region = Rect2(index * frame_w, 0, frame_w, frame_h)
		elif kind in ["cop", "rival"]:
			region = Rect2(70 + (index % 6) * 239, 53 + int(index / 6.0) * 226, 196, 201)
		elif outfit == 3:
			region = Rect2(112 + (index % 4) * 114, 762 if moving else 283, 101, 100)
		else:
			region = Rect2(228 + (index % 4) * 278, 56 + int(index / 4.0) * 236, 164, 190)
		var atlas = AtlasTexture.new()
		atlas.atlas = texture_sheet
		atlas.region = region
		body_sprite.texture = atlas
		mask_material.set_shader_parameter("region", Vector4(region.position.x / texture_sheet.get_width(), region.position.y / texture_sheet.get_height(), region.size.x / texture_sheet.get_width(), region.size.y / texture_sheet.get_height()))
		mask_material.set_shader_parameter("ghost", outfit == 3 and kind not in ["cop", "rival"] and not is_custom_batch2)
		body_sprite.scale = Vector2(54.0 / region.size.x, 62.0 / region.size.y)
	body_sprite.flip_h = facing.x < -0.1
	body_sprite.modulate = Color(0.65, 0.85, 1.2) if kind == "cop" else Color.WHITE
	if hurt_time > 0:
		body_sprite.modulate = Color(2.5, 0.5, 0.5)
	
	# Leaning into movement and bobbing
	var tilt = clampf(velocity.x * 0.0008, -0.18, 0.18)
	body_sprite.rotation = tilt
	body_sprite.position.y = -27 + (sin(frame_time * 16.0) * 2.0 if moving else 0.0)

func tick_visual(delta: float) -> void:
	frame_time += delta
	cooldown = maxf(0, cooldown - delta)
	hurt_time = maxf(0, hurt_time - delta)
	flash_time = maxf(0, flash_time - delta)
	if velocity.length() > 1:
		facing = velocity.normalized()
	
	# Ghost afterimages when moving fast
	if velocity.length() > 220:
		if ghost_trails.size() < 4:
			ghost_trails.append({"pos": position, "alpha": 0.5})
	for i in range(ghost_trails.size() - 1, -1, -1):
		ghost_trails[i].alpha -= delta * 3.5
		if ghost_trails[i].alpha <= 0:
			ghost_trails.remove_at(i)
			
	update_art()
	queue_redraw()

func navigate(goal: Vector2, speed: float, delta: float, city: Node2D) -> void:
	route_timer -= delta
	if route_timer <= 0:
		route = city.path_between(position, goal)
		route_timer = 0.55
	while not route.is_empty() and position.distance_to(route[0]) < 14:
		route.remove_at(0)
	var target = goal if route.is_empty() else route[0]
	velocity = position.direction_to(target) * speed if position.distance_to(target) > 8 else Vector2.ZERO
	move_and_slide()
	position = city.clamp_point(position)

func trigger_muzzle_flash() -> void:
	flash_time = 0.08

func _draw() -> void:
	# Ghost afterimages
	for trail in ghost_trails:
		var trail_offset = trail.pos - position
		draw_set_transform(trail_offset, 0.0, Vector2(1.0, 0.45))
		draw_circle(Vector2.ZERO, 15, Color(0.3, 0.9, 0.8, trail.alpha * 0.3))
		draw_set_transform(Vector2.ZERO)
		
	# Ground Shadow
	draw_set_transform(Vector2.ZERO, 0.0, Vector2(1.0, 0.45))
	draw_circle(Vector2.ZERO, 18, Color(0, 0, 0, 0.55))
	# Neon Underglow Kit (Batch 6)
	if kind == "player" and get_parent() != null and get_parent().get_parent() != null and get_parent().get_parent().get("neon_underglow"):
		draw_circle(Vector2.ZERO, 34, Color(0.3, 0.95, 0.85, 0.45))
		draw_circle(Vector2.ZERO, 20, Color(1.0, 0.25, 0.5, 0.6))
	draw_set_transform(Vector2.ZERO)
	
	var ring = Color("4cf5d5")
	if kind == "rival":
		ring = Color("ff4158")
	elif kind == "cop":
		ring = Color("59b7ff")
	elif kind == "crew":
		ring = Color("ffe06d")
		
	# Stylized Ground Selection Ring
	draw_arc(Vector2(0, 1), 17, 0, TAU, 28, Color(ring, 0.85), 2.0, true)
	if kind == "player":
		draw_arc(Vector2(0, 1), 21, 0, TAU, 28, Color(ring, 0.35), 1.0, true)
		
	# Spray can in hand when painting
	if painting:
		var can_pos = Vector2(facing.x * 16, -20 + facing.y * 8)
		draw_rect(Rect2(can_pos - Vector2(3, 5), Vector2(6, 10)), Color("4cf5d5"))
		draw_rect(Rect2(can_pos - Vector2(1.5, 7.5), Vector2(3, 2.5)), Color.WHITE)
		draw_circle(can_pos + facing * 8, 4.0, Color(0.3, 0.96, 0.84, 0.8))
		
	# Gun Muzzle Flash
	if flash_time > 0:
		var flash_pos = Vector2(facing.x * 22, -22 + facing.y * 10)
		draw_circle(flash_pos, 7.0, Color("ffe06d"))
		draw_circle(flash_pos, 3.5, Color.WHITE)
		
	# Badge & Health Bars for NPCs
	if kind != "player":
		var tag_title = badge if not badge.is_empty() else kind.to_upper()
		if badge == "CAPTAIN":
			# Boss Horns / Skull Glow
			draw_arc(Vector2(0, -66), 14, 0, TAU, 16, Color(1, 0.2, 0.2, 0.8), 2.0, true)
			draw_string(ThemeDB.fallback_font, Vector2(-28, -62), "👑 CAPTAIN 👑", HORIZONTAL_ALIGNMENT_CENTER, -1, 11, Color("ff4158"))
		else:
			draw_string(ThemeDB.fallback_font, Vector2(-24, -57), tag_title, HORIZONTAL_ALIGNMENT_CENTER, -1, 10, ring)
			
		if health < 100 or badge == "CAPTAIN":
			var bar_w = 42.0 if badge == "CAPTAIN" else 32.0
			var max_h = 320.0 if badge == "CAPTAIN" else 100.0
			draw_rect(Rect2(-bar_w * 0.5, -50, bar_w, 4), Color(0.05, 0.05, 0.05, 0.85))
			draw_rect(Rect2(-bar_w * 0.5, -50, bar_w * clampf(health / max_h, 0.0, 1.0), 4), ring)
