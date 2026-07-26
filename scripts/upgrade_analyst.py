import os
import re
from collections import Counter

def get_godot_log_path(project_name="GuardianInterface"):
    """Dynamically locates the Godot user data path for the project."""
    home = os.path.expanduser("~")
    return os.path.join(
        home, 
        ".local/share/godot/app_userdata", 
        project_name, 
        "research_notes/failure_log.txt"
    )

def generate_upgrade_report(project_name="GuardianInterface"):
    log_path = get_godot_log_path(project_name)
    print(f"--- Analyzing Log: {log_path} ---")
    
    if not os.path.exists(log_path):
        print(f"Error: Log file not found at {log_path}.")
        print("Ensure the simulation has run at least once to generate the file.")
        return

    with open(log_path, 'r') as f:
        lines = f.readlines()

    # Regex to extract the FailureMode from the log entry
    failures = []
    for line in lines:
        match = re.search(r"Mode: (\w+)", line)
        if match:
            failures.append(match.group(1))

    # Analysis and Reporting
    counts = Counter(failures)
    if not counts:
        print("Log is empty. Run more simulations in the Lab Bay!")
        return

    for mode, count in counts.items():
        print(f"\n[Trend Detected: {mode} occurred {count} times]")
        if mode == "PowerStarvation":
            print(">> Suggestion: Upgrade to High-Density Solid-State Battery array.")
        elif mode == "StructuralBuckling":
            print(">> Suggestion: Upgrade to Carbon-Nanotube Reinforced Alloy joints.")
        elif mode == "PrincipleConflict":
            print(">> Suggestion: Review 'ActionState' logic; tighten Principle Coherence thresholds.")

if __name__ == "__main__":
    # Ensure this matches the project name set in your project.godot file
    generate_upgrade_report("GuardianInterface")
