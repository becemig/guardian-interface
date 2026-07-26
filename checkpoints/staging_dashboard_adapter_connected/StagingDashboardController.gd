extends Node

var adapter: Node = null

func _ready():
    print("StagingDashboardController active.")

    adapter = preload("res://scripts/core/DashboardBridgeAdapter.gd").new()
    add_child(adapter)

    adapter.dashboard_payload_updated.connect(_on_dashboard_payload)

func _on_dashboard_payload(payload):
    print("Dashboard Staging Payload:")
    print(payload)
