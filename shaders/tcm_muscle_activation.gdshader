shader_type spatial;
render_mode cull_back, diffuse_burley;

uniform vec4 element_color : source_color = vec4(0.10, 0.75, 0.35, 1.0);
uniform float activation : hint_range(0.0, 1.0) = 0.0;
uniform float emission_strength : hint_range(0.0, 8.0) = 1.8;
uniform float pulse_phase : hint_range(0.0, 6.283185) = 0.0;
uniform float pulse_amount : hint_range(0.0, 0.35) = 0.08;
uniform float opacity : hint_range(0.0, 1.0) = 0.82;

void fragment() {
    float pulse = 1.0 + pulse_amount * sin(TIME * 2.0 + pulse_phase);
    float intensity = clamp(activation, 0.0, 1.0);

    ALBEDO = mix(element_color.rgb * 0.18, element_color.rgb, intensity);
    EMISSION = element_color.rgb * intensity * emission_strength * pulse;
    ROUGHNESS = 0.38;
    METALLIC = 0.0;
    ALPHA = opacity;
}
