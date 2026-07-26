class_name KnowledgeAtlasBridge
extends Node
# KnowledgeAtlasBridge.gd
# Loads knowledge_graph_index.json and exposes query methods.
# Schema: knowledge_graph_index.v2
# Keys: nodes, somatic_node_index, topic_index, category_index, evidence_index

var is_loaded: bool = false

var _nodes: Array = []
var _nodes_by_uid: Dictionary = {}
var _somatic_node_index: Dictionary = {}
var _topic_index: Dictionary = {}
var _category_index: Dictionary = {}
var _evidence_index: Dictionary = {}


func load_atlas(path: String = "res://master_research_archive/knowledge_graph_index.json") -> bool:
	if not FileAccess.file_exists(path):
		push_warning("[KnowledgeAtlasBridge] Index not found: " + path)
		is_loaded = false
		return false

	var file := FileAccess.open(path, FileAccess.READ)
	var raw := file.get_as_text()
	file.close()

	var json := JSON.new()
	var err = json.parse(raw)
	if err != OK:
		push_error("[KnowledgeAtlasBridge] JSON parse error at line: " + str(json.get_error_line()))
		is_loaded = false
		return false

	var data: Dictionary = json.get_data()

	_nodes = data.get("nodes", [])
	_somatic_node_index = data.get("somatic_node_index", {})
	_topic_index = data.get("topic_index", {})
	_category_index = data.get("category_index", {})
	_evidence_index = data.get("evidence_index", {})

	_nodes_by_uid.clear()
	for node in _nodes:
		_nodes_by_uid[node["uid"]] = node

	is_loaded = true
	print("[KnowledgeAtlasBridge] Loaded %d knowledge nodes." % _nodes.size())
	return true


var _bus: Node = null

func set_bus(bus: Node) -> void:
	_bus = bus

func get_uids_for_somatic_node(node_id: int) -> Array:
	var uids = _somatic_node_index.get(str(node_id), [])
	if _bus and _bus.has_signal("somatic_node_queried"):
		_bus.emit_somatic_query(node_id, uids)
	return uids


func get_uids_for_tag(tag: String) -> Array:
	return _topic_index.get(tag, [])


func get_uids_for_category(category: String) -> Array:
	return _category_index.get(category, [])


func get_uids_for_evidence(author_key: String) -> Array:
	var results: Array = []
	for key in _evidence_index.keys():
		if author_key.to_lower() in key.to_lower():
			results.append_array(_evidence_index[key])
	return results


func get_node(uid: String) -> Dictionary:
	return _nodes_by_uid.get(uid, {})


func get_all_nodes() -> Array:
	return _nodes


func get_node_count() -> int:
	return _nodes.size()
