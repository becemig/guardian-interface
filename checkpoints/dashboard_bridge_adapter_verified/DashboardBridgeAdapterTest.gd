extends Node

var adapter: Node = null

func _ready():
    print("DashboardBridgeAdapterTest initialized.")

    adapter = preload("res://scripts/core/DashboardBridgeAdapter.gd").new()
    add_child(adapter)

    adapter.dashboard_payload_updated.connect(_on_dashboard_update)

func _on_dashboard_update(payload):
    print("--------------------------------")
    print("Dashboard payload received")
    print(payload)
