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
    // Actual server index mapping (verified from frame data):
    // Right side: ankle=0, knee=1, hip=2, shoulder=3, elbow=4, wrist=5
    // Left side:  wrist=6, elbow=7, shoulder=8, hip=9, knee=10, ankle=11
    int SH = (side < 0.0) ? 8 : 3;  // shoulder
    int EL = (side < 0.0) ? 7 : 4;  // elbow
    int WR = (side < 0.0) ? 6 : 5;  // wrist
    int HI = (side < 0.0) ? 9 : 2;  // hip
    int KN = (side < 0.0) ? 10 : 1; // knee
    int AN = (side < 0.0) ? 11 : 0; // ankle

    float blend = 0.12; // blend radius in meters

    if (h > 1.4 + blend) {
        return bone_colors[SH];
    } else if (h > 1.4 - blend) {
        float t = (h - (1.4 - blend)) / (2.0 * blend);
        return mix(bone_colors[EL], bone_colors[SH], t);
    } else if (h > 1.1 + blend) {
        return bone_colors[EL];
    } else if (h > 1.1 - blend) {
        float t = (h - (1.1 - blend)) / (2.0 * blend);
        return mix(bone_colors[WR], bone_colors[EL], t);
    } else if (h > 0.85 + blend) {
        return bone_colors[WR];
    } else if (h > 0.85 - blend) {
        float t = (h - (0.85 - blend)) / (2.0 * blend);
        return mix(bone_colors[HI], bone_colors[WR], t);
    } else if (h > 0.7 + blend) {
        return bone_colors[HI];
    } else if (h > 0.7 - blend) {
        float t = (h - (0.7 - blend)) / (2.0 * blend);
        return mix(bone_colors[KN], bone_colors[HI], t);
    } else if (h > 0.4 + blend) {
        return bone_colors[KN];
    } else if (h > 0.4 - blend) {
        float t = (h - (0.4 - blend)) / (2.0 * blend);
        return mix(bone_colors[AN], bone_colors[KN], t);
    } else {
        return bone_colors[AN];
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
