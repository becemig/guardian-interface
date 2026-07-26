extends Control

var hrv_history: Array = []

func update_trend(history_snapshot):
    hrv_history = history_snapshot.get("hrv", [])
    queue_redraw()

func _draw():
    var sample_count = hrv_history.size()

    draw_string(
        ThemeDB.fallback_font,
        Vector2(20, 30),
        "TrendGraph: HRV samples " + str(sample_count)
    )

    if sample_count < 2:
        return

    var rect = Rect2(20, 60, size.x - 40, size.y - 100)

    if rect.size.x <= 0 or rect.size.y <= 0:
        return

    var min_value = float(hrv_history[0])
    var max_value = float(hrv_history[0])

    for value in hrv_history:
        var v = float(value)

        if v < min_value:
            min_value = v

        if v > max_value:
            max_value = v

    var value_range = max_value - min_value

    if value_range == 0:
        value_range = 1.0

    var previous_point = Vector2.ZERO

    for i in range(sample_count):
        var value = float(hrv_history[i])

        var x = rect.position.x

        if sample_count > 1:
            x += (float(i) / float(sample_count - 1)) * rect.size.x

        var normalized = (value - min_value) / value_range
        var y = rect.position.y + rect.size.y - (normalized * rect.size.y)

        var point = Vector2(x, y)

        draw_circle(point, 4, Color.WHITE)

        if i > 0:
            draw_line(previous_point, point, Color.WHITE, 2.0)

        previous_point = point

    draw_string(
        ThemeDB.fallback_font,
        Vector2(20, size.y - 40),
        "HRV min/max: " + str(min_value) + " / " + str(max_value)
    )

