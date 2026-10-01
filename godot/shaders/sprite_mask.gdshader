shader_type canvas_item;
render_mode unshaded;
uniform vec4 region = vec4(0.0, 0.0, 1.0, 1.0);
uniform bool ghost = false;
uniform bool source_alpha = false;
varying vec4 vertex_color;

void vertex() {
	vertex_color = COLOR;
}

// The supplied sheets have painted backgrounds. Mask each atlas region in UV
// space at render time, preserving the original artwork on disk.
void fragment() {
	vec4 pixel = texture(TEXTURE, UV);
	vec2 local = (UV - region.xy) / region.zw;
	vec3 left_bg = texture(TEXTURE, region.xy + vec2(0.01, clamp(local.y, 0.0, 1.0)) * region.zw).rgb;
	vec3 right_bg = texture(TEXTURE, region.xy + vec2(0.99, clamp(local.y, 0.0, 1.0)) * region.zw).rgb;
	vec3 background = mix(left_bg, right_bg, local.x);
	float difference = length(pixel.rgb - background);
	float matte = smoothstep(0.025, 0.09, difference);
	if (ghost) {
		matte = smoothstep(0.09, 0.24, max(pixel.r, max(pixel.g, pixel.b)));
	}
	float top = smoothstep(0.0, 0.035, local.y);
	float bottom = 1.0 - smoothstep(0.94, 1.0, local.y);
	float edge = smoothstep(0.0, 0.05, local.x) * (1.0 - smoothstep(0.95, 1.0, local.x));
	COLOR = pixel * vertex_color;
	if (!source_alpha) {
		COLOR.a *= top * bottom * edge * matte;
	}
}
