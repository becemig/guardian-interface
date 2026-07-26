extends Node

# The "Brain" that evaluates interactions
func process_interaction(source_concept, target_system):
    var props = source_concept.get("properties", {})
    var rules = props.get("interaction_rules", "")
    
    # Example Rule Parser:
    # This evaluates logic based on the string definitions in your JSON
    if "If sleep_debt > 0.5" in rules:
        if get_node("/root/CognitiveState").bio_state.sleep_debt > 0.5:
            apply_penalties(target_system, props.get("influence_type"))

func apply_penalties(target, type):
    if type == "multiplier":
        # Access the target node and modify its values
        if target.has_method("set_precision"):
            target.set_precision(target.get_precision() * 0.8)
    print("Interaction Applied: Concept influence modified target system.")
