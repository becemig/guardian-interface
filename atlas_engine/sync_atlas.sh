#!/bin/bash
# Guardian Interface Master Sync Script

echo "[1/4] Validating Dependencies..."
python3 /home/becemig/GodotProjects/guardian-interface/atlas_engine/validate_deps.py

echo "[2/4] Updating Knowledge Registry..."
python3 /home/becemig/GodotProjects/guardian-interface/atlas_engine/atlas_controller.py auto_link

echo "[3/4] Calculating Mastery & Corridors..."
python3 -c "from atlas_controller import assign_corridors; assign_corridors()"

echo "[4/4] Exporting to Godot Visual Garden..."
python3 -c "from atlas_controller import export_for_godot; export_for_godot()"

echo "Sync Complete. System ready."
