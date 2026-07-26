import json
import shutil
import os
from datetime import datetime

MASTER_PATH = "/home/becemig/GodotProjects/guardian-interface/master_data/master_study_nodes.json"
SESSION_PATH = "/home/becemig/GodotProjects/guardian-interface/staging_data/session_nodes.json"
BACKUP_DIR = "/home/becemig/GodotProjects/guardian-interface/backups/"

def promote():
    # 1. Ensure backup directory exists
    if not os.path.exists(BACKUP_DIR):
        os.makedirs(BACKUP_DIR)
    
    # 2. Create timestamped backup
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_path = os.path.join(BACKUP_DIR, f"master_backup_{timestamp}.json")
    shutil.copy2(MASTER_PATH, backup_path)
    print(f"Backup created: {backup_path}")

    # 3. Perform Merge
    with open(MASTER_PATH, 'r') as f: master_data = json.load(f)
    with open(SESSION_PATH, 'r') as f: session_data = json.load(f)

    master_nodes = {n['id']: n for n in master_data['nodes']}

    for s_node in session_data['nodes']:
        if 'bridges' in s_node and s_node['id'] in master_nodes:
            m_node = master_nodes[s_node['id']]
            current_targets = [b['target'] for b in m_node.get('bridges', [])]
            for bridge in s_node['bridges']:
                if bridge['target'] not in current_targets:
                    if 'bridges' not in m_node: m_node['bridges'] = []
                    m_node['bridges'].append(bridge)
                    print(f"Promoted: Bridge from {s_node['id']} to {bridge['target']}")

    # 4. Save
    with open(MASTER_PATH, 'w') as f:
        json.dump(master_data, f, indent=4)
    print("--- Promotion Complete: Master Registry updated safely ---")

if __name__ == "__main__":
    promote()
