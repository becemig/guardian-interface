import json
import os

# Define the 4 domains
DOMAINS = ["Earth Systems", "Life Systems", "Human Systems", "Cosmic Systems"]

def migrate_node(raw_content, domain):
    return {
        "id": raw_content.get("label", "unknown").lower().replace(" ", "_"),
        "label": raw_content.get("label"),
        "domain": domain,
        "links": raw_content.get("links", []),
        "metadata": raw_content.get("details", {})
    }

# Example: Process your extracted PDF content into the Atlas
def run_migration():
    # You can add logic here to loop through your extracted JSON files
    print("--- Guardian Atlas: Migration Initialized ---")
    # Placeholder for actual file parsing logic
    print("--- Migration Complete: Nodes mapped to domains ---")

if __name__ == "__main__":
    run_migration()
