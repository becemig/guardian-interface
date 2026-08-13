#!/usr/bin/env bash
set -u

LOG="$HOME/.local/share/godot/app_userdata/guardian-interface/logs/godot.log"

echo "Watching: $LOG"
echo "Press Ctrl+C to stop."

tail -n 0 -F "$LOG" | while IFS= read -r line; do
    case "$line" in
        *"SERVER RESPONSE"*)
            printf '\033[1;31m[ALERT: full Atlas payload] %s\033[0m\n' "$line"
            ;;
        *"[AtlasBridge]"*)
            printf '\033[1;36m%s\033[0m\n' "$line"
            ;;
        *"[SessionRecorder]"*)
            printf '\033[1;32m%s\033[0m\n' "$line"
            ;;
        *"ObjectDB"*|*"leaked at exit"*)
            printf '\033[1;33m[LEAK WARNING] %s\033[0m\n' "$line"
            ;;
        *)
            printf '%s\n' "$line"
            ;;
    esac
done
