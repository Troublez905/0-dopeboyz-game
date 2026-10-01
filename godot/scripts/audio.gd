extends Node

var music: AudioStreamPlayer
var effects: AudioStreamPlayer
var spray_player: AudioStreamPlayer
var beat_clock = 0.0
var step = 0
var active = false
var config: Dictionary = {"music": 0.7, "effects": 0.8}
var is_spraying = false
var radio_track = 0
const TRACKS = ["BOOM-BAP CYPHER", "CYBER DRIFT", "MIDNIGHT 808", "UNDERGROUND HEAT"]

# Pre-synthesized audio cache for crisp performance
var sfx_cache: Dictionary = {}

func _ready() -> void:
	music = AudioStreamPlayer.new()
	effects = AudioStreamPlayer.new()
	spray_player = AudioStreamPlayer.new()
	add_child(music)
	add_child(effects)
	add_child(spray_player)
	_build_sfx_cache()

func _build_sfx_cache() -> void:
	sfx_cache["spray_shake"] = _generate_spray_shake()
	sfx_cache["tag"] = _generate_tag_finish()
	sfx_cache["pickup"] = _generate_pickup()
	sfx_cache["shot"] = _generate_gunshot()
	sfx_cache["hit"] = _generate_hit_splat()
	sfx_cache["level"] = _generate_fanfare()
	sfx_cache["buy"] = _generate_cash_ching()
	sfx_cache["alarm"] = _generate_siren()
	sfx_cache["combo"] = _generate_scratch()
	sfx_cache["dash"] = _generate_dash()
	sfx_cache["blast"] = _generate_paint_blast()

func cycle_radio() -> String:
	radio_track = (radio_track + 1) % TRACKS.size()
	cue("combo")
	return TRACKS[radio_track]

func set_spraying(spraying: bool) -> void:
	if spraying == is_spraying:
		return
	is_spraying = spraying
	if is_spraying:
		cue("spray_shake")
		spray_player.volume_db = linear_to_db(maxf(0.0001, float(config.get("effects", 0.8)) * 0.75))
		spray_player.stream = _generate_spray_hiss(1.2)
		spray_player.play()
	else:
		spray_player.stop()

func cue(kind: String) -> void:
	if config.is_empty():
		return
	var vol = float(config.get("effects", 0.8))
	effects.volume_db = linear_to_db(maxf(0.0001, vol))
	if sfx_cache.has(kind):
		effects.stream = sfx_cache[kind]
	else:
		effects.stream = tone(440.0, 0.2, 0.3)
	effects.play()

func tone(frequency: float, length: float, volume: float, drum: bool = false) -> AudioStreamWAV:
	var wave = AudioStreamWAV.new()
	wave.format = AudioStreamWAV.FORMAT_16_BITS
	wave.mix_rate = 22050
	var count = int(length * 22050)
	var bytes = PackedByteArray()
	bytes.resize(count * 2)
	for i in count:
		var t = float(i) / 22050.0
		var envelope = minf(t * 220.0, 1.0) * pow(1.0 - float(i) / count, 2.2)
		var f = frequency + (110.0 * exp(-t * 40.0) if drum else 0.0)
		var value = (sin(TAU * f * t) + 0.25 * sin(TAU * f * 2.0 * t) + 0.1 * sin(TAU * f * 3.0 * t)) * envelope * volume
		bytes.encode_s16(i * 2, int(clampf(value, -1.0, 1.0) * 32760))
	wave.data = bytes
	return wave

func _generate_spray_hiss(length: float = 1.0) -> AudioStreamWAV:
	var wave = AudioStreamWAV.new()
	wave.format = AudioStreamWAV.FORMAT_16_BITS
	wave.mix_rate = 22050
	wave.loop_mode = AudioStreamWAV.LOOP_FORWARD
	wave.loop_begin = 1000
	wave.loop_end = int(length * 22050) - 1000
	var count = int(length * 22050)
	var bytes = PackedByteArray()
	bytes.resize(count * 2)
	var last = 0.0
	for i in count:
		var white = randf_range(-1.0, 1.0)
		# Bandpass filtered white noise for authentic aerosol spray can hiss
		last = last * 0.72 + white * 0.28
		var mod = 0.8 + 0.2 * sin(TAU * 14.0 * (float(i) / 22050.0))
		var value = last * mod * 0.38
		bytes.encode_s16(i * 2, int(clampf(value, -1.0, 1.0) * 32760))
	wave.data = bytes
	return wave

func _generate_spray_shake() -> AudioStreamWAV:
	var wave = AudioStreamWAV.new()
	wave.format = AudioStreamWAV.FORMAT_16_BITS
	wave.mix_rate = 22050
	var count = int(0.22 * 22050)
	var bytes = PackedByteArray()
	bytes.resize(count * 2)
	for i in count:
		var t = float(i) / 22050.0
		var value = 0.0
		# Three quick metallic marble clicks
		for click_time in [0.02, 0.08, 0.15]:
			if t >= click_time and t < click_time + 0.03:
				var dt = t - click_time
				var env = exp(-dt * 200.0)
				value += (sin(TAU * 1850.0 * dt) + 0.6 * sin(TAU * 3400.0 * dt)) * env * 0.5
		bytes.encode_s16(i * 2, int(clampf(value, -1.0, 1.0) * 32760))
	wave.data = bytes
	return wave

func _generate_tag_finish() -> AudioStreamWAV:
	var wave = AudioStreamWAV.new()
	wave.format = AudioStreamWAV.FORMAT_16_BITS
	wave.mix_rate = 22050
	var count = int(0.55 * 22050)
	var bytes = PackedByteArray()
	bytes.resize(count * 2)
	for i in count:
		var t = float(i) / 22050.0
		var sub_env = exp(-t * 5.0)
		var sub = sin(TAU * (95.0 - t * 45.0) * t) * sub_env * 0.6
		var chime_env = exp(-t * 8.0)
		var chime = (sin(TAU * 880.0 * t) + 0.5 * sin(TAU * 1320.0 * t)) * chime_env * 0.35
		var value = sub + chime
		bytes.encode_s16(i * 2, int(clampf(value, -1.0, 1.0) * 32760))
	wave.data = bytes
	return wave

func _generate_pickup() -> AudioStreamWAV:
	var wave = AudioStreamWAV.new()
	wave.format = AudioStreamWAV.FORMAT_16_BITS
	wave.mix_rate = 22050
	var count = int(0.32 * 22050)
	var bytes = PackedByteArray()
	bytes.resize(count * 2)
	for i in count:
		var t = float(i) / 22050.0
		var note_idx = int(t * 12.0)
		var freq = [523.25, 659.25, 783.99, 1046.50][mini(note_idx, 3)]
		var env = exp(-fmod(t, 0.08) * 45.0) * (1.0 - t / 0.32)
		var value = (sin(TAU * freq * t) + 0.3 * sin(TAU * freq * 2.0 * t)) * env * 0.4
		bytes.encode_s16(i * 2, int(clampf(value, -1.0, 1.0) * 32760))
	wave.data = bytes
	return wave

func _generate_gunshot() -> AudioStreamWAV:
	var wave = AudioStreamWAV.new()
	wave.format = AudioStreamWAV.FORMAT_16_BITS
	wave.mix_rate = 22050
	var count = int(0.28 * 22050)
	var bytes = PackedByteArray()
	bytes.resize(count * 2)
	for i in count:
		var t = float(i) / 22050.0
		var crack = randf_range(-1.0, 1.0) * exp(-t * 70.0) * 0.6
		var body = sin(TAU * (160.0 * exp(-t * 35.0)) * t) * exp(-t * 18.0) * 0.55
		var value = crack + body
		bytes.encode_s16(i * 2, int(clampf(value, -1.0, 1.0) * 32760))
	wave.data = bytes
	return wave

func _generate_hit_splat() -> AudioStreamWAV:
	var wave = AudioStreamWAV.new()
	wave.format = AudioStreamWAV.FORMAT_16_BITS
	wave.mix_rate = 22050
	var count = int(0.24 * 22050)
	var bytes = PackedByteArray()
	bytes.resize(count * 2)
	for i in count:
		var t = float(i) / 22050.0
		var splat = (randf_range(-0.8, 0.8) + sin(TAU * 220.0 * t)) * exp(-t * 35.0) * 0.45
		var thud = sin(TAU * (90.0 - t * 50.0) * t) * exp(-t * 20.0) * 0.5
		var value = splat + thud
		bytes.encode_s16(i * 2, int(clampf(value, -1.0, 1.0) * 32760))
	wave.data = bytes
	return wave

func _generate_cash_ching() -> AudioStreamWAV:
	var wave = AudioStreamWAV.new()
	wave.format = AudioStreamWAV.FORMAT_16_BITS
	wave.mix_rate = 22050
	var count = int(0.38 * 22050)
	var bytes = PackedByteArray()
	bytes.resize(count * 2)
	for i in count:
		var t = float(i) / 22050.0
		var ding1 = sin(TAU * 1318.5 * t) * exp(-t * 12.0) * 0.35
		var ding2 = 0.0
		if t > 0.07:
			ding2 = sin(TAU * 1760.0 * (t - 0.07)) * exp(-(t - 0.07) * 10.0) * 0.45
		var value = ding1 + ding2
		bytes.encode_s16(i * 2, int(clampf(value, -1.0, 1.0) * 32760))
	wave.data = bytes
	return wave

func _generate_siren() -> AudioStreamWAV:
	var wave = AudioStreamWAV.new()
	wave.format = AudioStreamWAV.FORMAT_16_BITS
	wave.mix_rate = 22050
	var count = int(0.65 * 22050)
	var bytes = PackedByteArray()
	bytes.resize(count * 2)
	for i in count:
		var t = float(i) / 22050.0
		var sweep = 550.0 + 250.0 * sin(TAU * 2.8 * t)
		var env = minf(t * 15.0, 1.0) * minf((0.65 - t) * 8.0, 1.0)
		var value = (sin(TAU * sweep * t) + 0.3 * sin(TAU * sweep * 2.0 * t)) * env * 0.4
		bytes.encode_s16(i * 2, int(clampf(value, -1.0, 1.0) * 32760))
	wave.data = bytes
	return wave

func _generate_scratch() -> AudioStreamWAV:
	var wave = AudioStreamWAV.new()
	wave.format = AudioStreamWAV.FORMAT_16_BITS
	wave.mix_rate = 22050
	var count = int(0.28 * 22050)
	var bytes = PackedByteArray()
	bytes.resize(count * 2)
	for i in count:
		var t = float(i) / 22050.0
		var mod_freq = 300.0 + 700.0 * sin(TAU * 8.0 * t)
		var noise = randf_range(-0.3, 0.3)
		var env = minf(t * 40.0, 1.0) * (1.0 - t / 0.28)
		var value = (sin(TAU * mod_freq * t) + noise) * env * 0.45
		bytes.encode_s16(i * 2, int(clampf(value, -1.0, 1.0) * 32760))
	wave.data = bytes
	return wave

func _generate_dash() -> AudioStreamWAV:
	var wave = AudioStreamWAV.new()
	wave.format = AudioStreamWAV.FORMAT_16_BITS
	wave.mix_rate = 22050
	var count = int(0.20 * 22050)
	var bytes = PackedByteArray()
	bytes.resize(count * 2)
	for i in count:
		var t = float(i) / 22050.0
		var whoosh = randf_range(-0.7, 0.7) * sin(PI * (t / 0.20)) * 0.4
		var sweep = sin(TAU * (350.0 - t * 900.0) * t) * (1.0 - t / 0.20) * 0.3
		var value = whoosh + sweep
		bytes.encode_s16(i * 2, int(clampf(value, -1.0, 1.0) * 32760))
	wave.data = bytes
	return wave

func _generate_fanfare() -> AudioStreamWAV:
	var wave = AudioStreamWAV.new()
	wave.format = AudioStreamWAV.FORMAT_16_BITS
	wave.mix_rate = 22050
	var count = int(0.9 * 22050)
	var bytes = PackedByteArray()
	bytes.resize(count * 2)
	var chords = [523.25, 659.25, 783.99, 1046.50]
	for i in count:
		var t = float(i) / 22050.0
		var segment = int(t * 4.5)
		var root_f = chords[mini(segment, 3)]
		var env = exp(-fmod(t, 0.22) * 8.0) * (1.0 - t / 0.9)
		var value = (sin(TAU * root_f * t) + 0.4 * sin(TAU * root_f * 1.5 * t) + 0.2 * sin(TAU * root_f * 2.0 * t)) * env * 0.4
		bytes.encode_s16(i * 2, int(clampf(value, -1.0, 1.0) * 32760))
	wave.data = bytes
	return wave

func _generate_paint_blast() -> AudioStreamWAV:
	var wave = AudioStreamWAV.new()
	wave.format = AudioStreamWAV.FORMAT_16_BITS
	wave.mix_rate = 22050
	var count = int(0.55 * 22050)
	var bytes = PackedByteArray()
	bytes.resize(count * 2)
	for i in count:
		var t = float(i) / 22050.0
		var whoosh = randf_range(-1.0, 1.0) * exp(-t * 14.0) * 0.7
		var sub_boom = sin(TAU * (85.0 * exp(-t * 22.0) + 38.0) * t) * exp(-t * 6.0) * 0.8
		var hiss = randf_range(-0.5, 0.5) * exp(-t * 8.0) * 0.4
		var value = whoosh + sub_boom + hiss
		bytes.encode_s16(i * 2, int(clampf(value, -1.0, 1.0) * 32760))
	wave.data = bytes
	return wave

# Urban Hip-Hop Beat Synthesizer (Kick, Snare, Hi-Hat, 808 Bass, Synth Keys)
func _process(delta: float) -> void:
	if not active or config.is_empty():
		return
	beat_clock -= delta
	var tempo_rates = [0.165, 0.145, 0.185, 0.155]
	var rate = tempo_rates[radio_track % tempo_rates.size()]
	if beat_clock <= 0:
		beat_clock += rate
		_play_groove_step(step % 16)
		step += 1

func _play_groove_step(s: int) -> void:
	var vol = float(config.get("music", 0.7))
	if vol <= 0.001:
		return
	music.volume_db = linear_to_db(maxf(0.0001, vol))
	
	var wave = AudioStreamWAV.new()
	wave.format = AudioStreamWAV.FORMAT_16_BITS
	wave.mix_rate = 22050
	var length = 0.24
	var count = int(length * 22050)
	var bytes = PackedByteArray()
	bytes.resize(count * 2)
	
	var bar = int(step / 16.0) % 4
	var prog_table = [
		[65.41, 51.91, 43.65, 49.00], # Track 0: Boom-Bap Cypher (C - G# - F - G)
		[58.27, 65.41, 77.78, 69.30], # Track 1: Cyber Drift (A# - C - D# - D)
		[43.65, 43.65, 51.91, 49.00], # Track 2: Midnight 808 (F - F - G# - G)
		[51.91, 58.27, 65.41, 49.00]  # Track 3: Underground Heat (G# - A# - C - G)
	]
	var current_prog = prog_table[radio_track % prog_table.size()]
	var bass_f = current_prog[bar]
	
	var has_kick = s in [0, 6, 10] if radio_track != 2 else s in [0, 4, 8, 11, 14]
	var has_snare = s in [4, 12]
	var has_hat = (s % 2 == 0) or (s == 15) or (radio_track == 1 and s % 4 == 3)
	var is_open_hat = s in [2, 10]
	
	for i in count:
		var t = float(i) / 22050.0
		var mixed = 0.0
		
		# 1. 808 Sub Bass
		var bass_decay = 6.5 if radio_track != 2 else 4.0
		var bass_env = exp(-t * bass_decay)
		var bass_val = sin(TAU * bass_f * t) + 0.3 * sin(TAU * bass_f * 2.0 * t)
		mixed += clampf(bass_val * 1.3, -1.0, 1.0) * bass_env * 0.45
		
		# 2. Kick Drum
		if has_kick:
			var k_env = exp(-t * 24.0)
			var k_pitch = 145.0 * exp(-t * 40.0) + 48.0
			var k_click = (randf_range(-0.3, 0.3) * exp(-t * 120.0))
			mixed += (sin(TAU * k_pitch * t) + k_click) * k_env * 0.65
			
		# 3. Snare / Clap
		if has_snare:
			var sn_env = exp(-t * 15.0)
			var sn_noise = randf_range(-0.7, 0.7) * exp(-t * 18.0)
			var sn_tone = sin(TAU * 190.0 * t) * exp(-t * 28.0)
			mixed += (sn_noise * 0.7 + sn_tone * 0.4) * sn_env * 0.55
			
		# 4. Hi-Hat
		if has_hat:
			var h_decay = 35.0 if is_open_hat else 95.0
			var h_env = exp(-t * h_decay)
			var h_noise = (randf_range(-0.5, 0.5) - randf_range(-0.5, 0.5)) * h_env * (0.28 if is_open_hat else 0.18)
			mixed += h_noise
			
		# 5. Urban Melody / Brass Stabs (on bar beats)
		if s in [0, 4, 8, 12]:
			var m_freq = bass_f * (4.0 if radio_track != 1 else 3.0)
			var m_env = exp(-t * 9.0)
			var m_val = (sin(TAU * m_freq * t) + 0.4 * sin(TAU * m_freq * 1.5 * t)) * m_env * 0.18
			mixed += m_val
			
		bytes.encode_s16(i * 2, int(clampf(mixed * 0.85, -1.0, 1.0) * 32760))
		
	wave.data = bytes
	music.stream = wave
	music.play()
