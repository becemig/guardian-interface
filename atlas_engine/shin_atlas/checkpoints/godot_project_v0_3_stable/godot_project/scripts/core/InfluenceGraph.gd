extends Control

var active_signal = "HRV"
var nodes = []

func _ready():
    if TelemetryController:
        TelemetryController.telemetry_payload_updated.connect(_on_data_received)

func _on_data_received(payload):
    active_signal = str(payload.get("active_signal", "HRV")).to_upper()
    nodes = payload.get("top_node_labels", [])
    queue_redraw()

func _draw():
    var center = get_viewport_rect().size / 2
    draw_circle(center, 32, Color.WHITE)
    draw_string(ThemeDB.fallback_font, center + Vector2(-20, 5), active_signal)
    if nodes.size() == 0: return
    for i in range(nodes.size()):
        var angle = (float(i) / nodes.size()) * TAU
        var pos = center + Vector2(cos(angle), sin(angle)) * 220
        var label = str(nodes[i])
        var node_color = _get_node_color(label)
        var dynamic_radius = max(8.0, 22.0 - (float(i) * 2.0))
        
        draw_line(center, pos, Color.DIM_GRAY, 2.0)
        draw_circle(pos, dynamic_radius, node_color)
        draw_string(ThemeDB.fallback_font, pos + Vector2(dynamic_radius + 5, 5), _short_label(label))
func _get_node_color(label):
    var text = label.to_lower()
    if "stress" in text or "strain" in text: return Color.ORANGE_RED
    if "sleep" in text or "rem" in text or "wave" in text or "recovery" in text: return Color.CORNFLOWER_BLUE
    if "autonomic" in text or "vagus" in text or "regulation" in text: return Color.MEDIUM_SEA_GREEN
    return Color.CYAN

func _short_label(label):
    var clean = label.replace("[Shin-ISU] ", "")
    if clean.length() > 34: return clean.substr(0, 34) + "..."
    return clean
