shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_lambert, specular_schlick_ggx;

uniform vec4 sphere_color : source_color = vec4(0.196, 0.506, 0.51, 1.0);

void fragment() {
    ALBEDO = sphere_color.rgb;
    ROUGHNESS = 0.2;
    METALLIC = 0.3;
    EMISSION = sphere_color.rgb * 3.0;
}
