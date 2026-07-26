#!/bin/bash

# 1. Initialize Log Directory
echo "[Launch] Initializing Research Data Directories..."
mkdir -p /home/becemig/.local/share/godot/app_userdata/GuardianInterface/research_notes/

# 2. Start the Guardian Suit Simulation
echo "[Launch] Booting Guardian Interface in Godot..."
# Replace with the path to your godot executable if needed
godot --path /home/becemig/GodotProjects/guardian-interface/ &

# 3. Start the Real-time Monitor (Automation Suite)
echo "[Launch] Starting Automation Suite for Telemetry and Hot-Reloading..."
python3 /home/becemig/GodotProjects/guardian-interface/scripts/automation_suite.py &

# 4. Success Message
echo "[Launch] Systems Online. Lab Bay is active."
echo "[Launch] Results will be logged to the research directory."
