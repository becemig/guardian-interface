shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_lambert, specular_schlick_ggx;

uniform vec4 bone_colors[12];
uniform float emit_strength : hint_range(0.0, 2.0) = 0.18;
uniform float roughness : hint_range(0.0, 1.0) = 0.72;
uniform float kappa_threshold : hint_range(0.0, 1.0) = 0.5;

// Naturalistic skin -- warm desaturated neutral
const vec3 SKIN_BASE = vec3(0.71, 0.58, 0.49);
const vec3 SKIN_SHADOW = vec3(0.52, 0.40, 0.34);

varying vec3 world_pos;
varying vec3 world_normal;

void vertex() {
    world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
    world_normal = normalize((MODEL_MATRIX * vec4(NORMAL, 0.0)).xyz);
}

vec4 zone_color(float h, float side) {
    int SH = (side < 0.0) ? 8 : 3;
    int EL = (side < 0.0) ? 7 : 4;
    int WR = (side < 0.0) ? 6 : 5;
    int HI = (side < 0.0) ? 9 : 2;
    int KN = (side < 0.0) ? 10 : 1;
    int AN = (side < 0.0) ? 11 : 0;
    float blend = 0.10;
    if (h > 1.4 + blend) return bone_colors[SH];
    else if (h > 1.4 - blend) {
        float t = (h-(1.4-blend))/(2.0*blend);
        return mix(bone_colors[EL],bone_colors[SH],t);
    } else if (h > 1.1 + blend) return bone_colors[EL];
    else if (h > 1.1 - blend) {
        float t = (h-(1.1-blend))/(2.0*blend);
        return mix(bone_colors[WR],bone_colors[EL],t);
    } else if (h > 0.85 + blend) return bone_colors[WR];
    else if (h > 0.85 - blend) {
        float t = (h-(0.85-blend))/(2.0*blend);
        return mix(bone_colors[HI],bone_colors[WR],t);
    } else if (h > 0.7 + blend) return bone_colors[HI];
    else if (h > 0.7 - blend) {
        float t = (h-(0.7-blend))/(2.0*blend);
        return mix(bone_colors[KN],bone_colors[HI],t);
    } else if (h > 0.4 + blend) return bone_colors[KN];
    else if (h > 0.4 - blend) {
        float t = (h-(0.4-blend))/(2.0*blend);
        return mix(bone_colors[AN],bone_colors[KN],t);
    } else return bone_colors[AN];
}

void fragment() {
    float h = world_pos.y;
    float side = world_pos.x;
    vec4 kappa_col = zone_color(h, side);
    // Kappa intensity from alpha channel (set by thermography ramp in C#)
    float kappa_intensity = kappa_col.a;
    float ndotl = clamp(dot(world_normal,normalize(vec3(0.4,1.0,0.6))),0.15,1.0);
    vec3 skin = mix(SKIN_SHADOW, SKIN_BASE, ndotl);
    // Very gentle blend -- skin dominates, heat tints subtly
    float blend_t = smoothstep(0.1, 0.75, kappa_intensity);
    vec3 final_col = mix(skin, mix(skin, kappa_col.rgb, 0.55), blend_t);
    ALBEDO = final_col;
    ROUGHNESS = roughness;
    METALLIC = 0.0;
    // Emission only at high kappa, very restrained
    EMISSION = kappa_col.rgb * emit_strength * smoothstep(0.5, 0.9, kappa_intensity);
}
