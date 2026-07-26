def process_update(self):
    with open(self.source_file, 'r') as f:
        data = json.load(f)
    
    # 1. Enrich (Heavy)
    enriched = self.engine.process_telemetry(data)
    data.update(enriched)
    
    # Write to master record
    with open(self.output_file, 'w') as f:
        json.dump(data, f)
        
    # 2. Extract (Lightweight)
    # Filter only what Godot needs to stay performant
    bridge_payload = {
        "guardian_state": data.get("guardian_state"),
        "confidence": data.get("confidence"),
        "respiration_rate": data.get("respiration_rate")
    }
    
    with open("godot_bridge_payload.json", 'w') as f:
        json.dump(bridge_payload, f)

