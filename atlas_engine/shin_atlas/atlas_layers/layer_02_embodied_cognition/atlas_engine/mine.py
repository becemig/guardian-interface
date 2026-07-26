import json
import sys

def ingest_batch(input_file, target_file="master_data/evidence_nodes.json"):
    with open(input_file, 'r') as f:
        new_nodes = json.load(f)
    with open(target_file, 'r') as f:
        atlas = json.load(f)
    atlas.extend(new_nodes)
    with open(target_file, 'w') as f:
        json.dump(atlas, f, indent=2)
    print(f"Ingested {len(new_nodes)} papers. Total now: {len(atlas)}")

if __name__ == "__main__":
    file_to_ingest = sys.argv[1] if len(sys.argv) > 1 else 'batch_input.json'
    ingest_batch(file_to_ingest)
