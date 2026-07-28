shader_type spatial;
render_mode blend_add, depth_draw_never, cull_back, unshaded;

uniform vec4 sphere_color : source_color = vec4(0.1, 0.3, 0.45, 0.4);
uniform float glow_intensity : hint_range(0.0, 3.0) = 1.2;

void fragment() {
    // Radial soft fade from center -- rim darkens to transparent
    vec2 uv = UV * 2.0 - 1.0;
    float dist = length(uv);
    float fade = 1.0 - smoothstep(0.3, 1.0, dist);
    float core = 1.0 - smoothstep(0.0, 0.45, dist);
    float glow = fade * 0.7 + core * 0.3;
    ALBEDO = sphere_color.rgb * glow_intensity;
    ALPHA = sphere_color.a * glow;
    EMISSION = sphere_color.rgb * glow * glow_intensity;
}
