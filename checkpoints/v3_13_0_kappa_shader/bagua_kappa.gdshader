shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_lambert, specular_schlick_ggx;

uniform vec4 bone_colors[12];
uniform float emit_strength : hint_range(0.0, 4.0) = 2.5;
uniform float roughness : hint_range(0.0, 1.0) = 0.4;

varying vec3 world_pos;

void vertex() {
    world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment() {
    float h = world_pos.y;
    float side = world_pos.x;

    vec4 col;
    if (h > 1.4) {
        col = (side < 0.0) ? bone_colors[0] : bone_colors[1];
    } else if (h > 1.1) {
        col = (side < 0.0) ? bone_colors[2] : bone_colors[3];
    } else if (h > 0.85) {
        col = (side < 0.0) ? bone_colors[4] : bone_colors[5];
    } else if (h > 0.7) {
        col = (side < 0.0) ? bone_colors[6] : bone_colors[7];
    } else if (h > 0.4) {
        col = (side < 0.0) ? bone_colors[8] : bone_colors[9];
    } else {
        col = (side < 0.0) ? bone_colors[10] : bone_colors[11];
    }

    ALBEDO = col.rgb;
    ROUGHNESS = roughness;
    METALLIC = 0.0;
    EMISSION = col.rgb * emit_strength;
}
