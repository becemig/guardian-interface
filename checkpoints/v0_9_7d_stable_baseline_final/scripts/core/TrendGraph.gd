extends Control

var hrv_history: Array = []

func update_trend(history_snapshot):
    hrv_history = history_snapshot.get("hrv", [])
    queue_redraw()

func _draw():
    draw_string(ThemeDB.fallback_font, Vector2(20, 30), "TrendGraph: HRV samples " + str(hrv_history.size()))

