shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_lambert, specular_schlick_ggx;

uniform vec4 bone_colors[12];
uniform float emit_strength : hint_range(0.0, 4.0) = 2.5;
uniform float roughness : hint_range(0.0, 1.0) = 0.4;

varying vec3 world_pos;

void vertex() {
    world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

vec4 zone_color(float h, float side) {
    // Left = even indices, Right = odd indices
    // Zones by height with smooth blend radius
    int L = (side < 0.0) ? 0 : 1;  // shoulder
    int LE = (side < 0.0) ? 2 : 3; // elbow
    int LW = (side < 0.0) ? 4 : 5; // wrist
    int LH = (side < 0.0) ? 6 : 7; // hip
    int LK = (side < 0.0) ? 8 : 9; // knee
    int LA = (side < 0.0) ? 10 : 11; // ankle

    float blend = 0.12; // blend radius in meters

    if (h > 1.4 + blend) {
        return bone_colors[L];
    } else if (h > 1.4 - blend) {
        float t = (h - (1.4 - blend)) / (2.0 * blend);
        return mix(bone_colors[LE], bone_colors[L], t);
    } else if (h > 1.1 + blend) {
        return bone_colors[LE];
    } else if (h > 1.1 - blend) {
        float t = (h - (1.1 - blend)) / (2.0 * blend);
        return mix(bone_colors[LW], bone_colors[LE], t);
    } else if (h > 0.85 + blend) {
        return bone_colors[LW];
    } else if (h > 0.85 - blend) {
        float t = (h - (0.85 - blend)) / (2.0 * blend);
        return mix(bone_colors[LH], bone_colors[LW], t);
    } else if (h > 0.7 + blend) {
        return bone_colors[LH];
    } else if (h > 0.7 - blend) {
        float t = (h - (0.7 - blend)) / (2.0 * blend);
        return mix(bone_colors[LK], bone_colors[LH], t);
    } else if (h > 0.4 + blend) {
        return bone_colors[LK];
    } else if (h > 0.4 - blend) {
        float t = (h - (0.4 - blend)) / (2.0 * blend);
        return mix(bone_colors[LA], bone_colors[LK], t);
    } else {
        return bone_colors[LA];
    }
}

void fragment() {
    float h = world_pos.y;
    float side = world_pos.x;

    vec4 col = zone_color(h, side);

    ALBEDO = col.rgb;
    ROUGHNESS = roughness;
    METALLIC = 0.0;
    EMISSION = col.rgb * emit_strength;
}
