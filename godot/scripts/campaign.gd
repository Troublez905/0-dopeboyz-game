extends Node

const SAVE_PATH = "user://street_shooters_v1.json"
const LEVELS = [
	{"name": "CANAL INK YARD", "walls": 3, "score": 200, "task": "Claim 3 walls and earn 200 rep", "credits": 3},
	{"name": "NEON MARKET STRIP", "walls": 4, "score": 450, "task": "Hold 4 walls and finish a street contract", "credits": 3},
	{"name": "RAILCUT BACKLOTS", "walls": 5, "score": 750, "task": "Hold 5 walls with 2 crew members", "credits": 4},
	{"name": "DOWNTOWN CROWN", "walls": 7, "score": 1100, "task": "Hold 7 walls for 30 seconds at 1100 rep", "credits": 5},
	{"name": "CAP ALLEY ARRIVAL", "walls": 4, "score": 450, "task": "Hold 4 walls and deliver a shipment to HQ", "credits": 3},
	{"name": "BASS BLOCK TAKEOVER", "walls": 6, "score": 750, "task": "Hold 6 walls and complete 2 contracts", "credits": 4},
	{"name": "STICKER TUNNEL", "walls": 7, "score": 1000, "task": "Hold 7 walls and defeat the rival captain", "credits": 4},
	{"name": "ROLLER HEIGHTS FINALE", "walls": 9, "score": 1400, "task": "Hold 9 walls with 3 crew for 45 seconds", "credits": 5}
]
const UPGRADES = {
	"speed": {"name": "Runner", "detail": "+18 move speed per rank", "unlock": 0},
	"paint": {"name": "Paint rig", "detail": "+30 paint capacity, +12% tagging speed", "unlock": 0},
	"health": {"name": "Resilience", "detail": "+25 maximum health per rank", "unlock": 0},
	"crew": {"name": "Crew training", "detail": "+1 crew slot, +8 crew damage per rank", "unlock": 1},
	"hq": {"name": "Headquarters", "detail": "+50 stash capacity, +20 starting paint", "unlock": 1},
	"gear": {"name": "Armor & gear", "detail": "+6 weapon damage, 8% less damage per rank", "unlock": 2},
	"lookout": {"name": "Lookout network", "detail": "+4 heat before police pursuit per rank", "unlock": 4},
	"influence": {"name": "Street influence", "detail": "+15% passive reputation per rank", "unlock": 4}
}
const OUTFITS = ["Purple Hoodie", "Runner", "Heavy Hoodie", "Ghost", "Subway Bomber"]
var level = 0
var cleared = 0
var credits = 0
var cash = 60
var score = 0.0
var outfit = 0
var upgrades: Dictionary = {}
var completed = false
var settings = {"music": 0.35, "effects": 0.65, "rain": true, "shake": true, "fullscreen": false}
var snapshot: Dictionary = {}
var save_error = ""
var save_path = SAVE_PATH

func save_preferences() -> bool:
	var config = ConfigFile.new()
	for key in settings:
		config.set_value("options", key, settings[key])
	var result = config.save("user://options.cfg")
	if result != OK:
		save_error = "Options could not be saved: " + error_string(result)
	return result == OK

func load_preferences() -> void:
	var config = ConfigFile.new()
	if not FileAccess.file_exists("user://options.cfg"):
		return
	if config.load("user://options.cfg") != OK:
		push_warning("Street Shooters options file is invalid. Using default options.")
		return
	for key in settings:
		var value = config.get_value("options", key, settings[key])
		if key in ["music", "effects"] and (value is float or value is int):
			settings[key] = clampf(float(value), 0, 1)
		elif key in ["rain", "shake", "fullscreen"] and value is bool:
			settings[key] = value

func _init() -> void:
	reset()

func reset() -> void:
	level = 0
	cleared = 0
	credits = 0
	cash = 60
	score = 0.0
	completed = false
	snapshot = {}
	upgrades = {"speed": 0, "paint": 0, "health": 0, "crew": 0, "hq": 0, "gear": 0, "lookout": 0, "influence": 0}
	outfit = 0

func rank_of(key: String) -> int:
	return int(upgrades.get(key, 0))

func max_health() -> float:
	return 100.0 + rank_of("health") * 25.0

func max_paint() -> float:
	return 100.0 + rank_of("paint") * 30.0

func max_crew() -> int:
	return 2 + rank_of("crew") + (1 if cleared >= 6 else 0)

func can_upgrade(key: String) -> bool:
	return UPGRADES.has(key) and credits > 0 and cleared >= int(UPGRADES[key].unlock) and rank_of(key) < 5

func buy_upgrade(key: String) -> bool:
	if not can_upgrade(key):
		return false
	credits -= 1
	upgrades[key] = rank_of(key) + 1
	return true

func finish_level() -> bool:
	if completed or cleared > level:
		return false
	credits += int(LEVELS[level].credits)
	cleared = level + 1
	if cleared == LEVELS.size():
		completed = true
	return true

func advance() -> void:
	if not completed and cleared > level:
		level += 1
		score = 0.0
		snapshot = {}

func save_game(world_state: Dictionary) -> bool:
	snapshot = world_state.duplicate(true)
	var payload = {"version": 1, "level": level, "cleared": cleared, "credits": credits, "cash": cash, "score": score, "outfit": outfit, "upgrades": upgrades, "completed": completed, "settings": settings, "world": snapshot}
	var file = FileAccess.open(save_path + ".tmp", FileAccess.WRITE)
	if file == null:
		save_error = "Cannot write save: " + error_string(FileAccess.get_open_error())
		return false
	file.store_string(JSON.stringify(payload, "\t"))
	file.close()
	var result = DirAccess.rename_absolute(save_path + ".tmp", save_path)
	if result != OK:
		save_error = "Cannot replace save: " + error_string(result)
		return false
	save_error = ""
	return true

func load_game() -> bool:
	if not FileAccess.file_exists(save_path):
		save_error = "No saved run yet."
		return false
	var file = FileAccess.open(save_path, FileAccess.READ)
	if file == null:
		save_error = "Save could not be opened."
		return false
	var parser = JSON.new()
	if parser.parse(file.get_as_text()) != OK or not parser.data is Dictionary:
		save_error = "Save is invalid; start a new run to replace it."
		return false
	var data: Dictionary = parser.data
	if data.get("version", 0) != 1 or not data.get("upgrades") is Dictionary or not data.get("world") is Dictionary:
		save_error = "Unsupported or incomplete save."
		return false
	if not valid_world(data.world):
		save_error = "Save has invalid world data. Your file has not been replaced."
		return false
	for key in ["level", "cleared", "credits", "cash", "score", "outfit"]:
		if not data.get(key) is float and not data.get(key) is int:
			save_error = "Save has an invalid " + key
			return false
	for key in UPGRADES:
		if key in ["lookout", "influence"] and not data.upgrades.has(key):
			data.upgrades[key] = 0
		if not data.upgrades.get(key) is float and not data.upgrades.get(key) is int:
			save_error = "Save has an invalid upgrade."
			return false
	level = clampi(int(data.level), 0, LEVELS.size() - 1)
	cleared = clampi(int(data.cleared), level, LEVELS.size())
	credits = clampi(int(data.credits), 0, 100)
	cash = clampi(int(data.cash), 0, 1000000)
	score = clampf(float(data.score), 0, 1000000)
	outfit = clampi(int(data.outfit), 0, mini(cleared, 3))
	for key in UPGRADES:
		upgrades[key] = clampi(int(data.upgrades[key]), 0, 5)
	completed = cleared == LEVELS.size()
	if data.get("settings") is Dictionary:
		for key in ["music", "effects"]:
			if data.settings.get(key) is float or data.settings.get(key) is int:
				settings[key] = clampf(float(data.settings[key]), 0, 1)
		for key in ["rain", "shake", "fullscreen"]:
			if data.settings.get(key) is bool:
				settings[key] = data.settings[key]
	snapshot = data.world
	save_error = ""
	return true

func valid_world(world: Dictionary) -> bool:
	for key in ["health", "paint", "ammo", "stamina", "heat", "stash", "combo", "combo_time", "contracts_done", "final_hold", "penalty_time", "deliveries", "captains_defeated"]:
		if world.has(key) and not numeric(world[key]):
			return false
	for key in ["weapon", "shipment"]:
		if world.has(key) and not world[key] is bool:
			return false
	for key in ["pos", "guard"]:
		if world.has(key) and not valid_position(world[key]):
			return false
	for key in ["walls", "crew", "enemies", "pickups"]:
		if world.has(key) and not world[key] is Array:
			return false
	for value in world.get("pickups", []):
		if not numeric(value):
			return false
	for key in ["walls", "crew", "enemies"]:
		for value in world.get(key, []):
			if not value is Dictionary:
				return false
			for field in ["progress", "enemy_progress", "health", "target"]:
				if value.has(field) and not numeric(value[field]):
					return false
			if value.has("pos") and not valid_position(value.pos):
				return false
	if world.has("contract"):
		if not world.contract is Dictionary:
			return false
		for key in ["wall", "time"]:
			if world.contract.has(key) and not numeric(world.contract[key]):
				return false
	return true

func numeric(value: Variant) -> bool:
	return (value is int or value is float) and is_finite(float(value))

func valid_position(value: Variant) -> bool:
	return value is Array and value.size() == 2 and numeric(value[0]) and numeric(value[1])
