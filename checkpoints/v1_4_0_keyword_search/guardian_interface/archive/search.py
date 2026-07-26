"""
guardian_interface.archive.search
Keyword search across the knowledge graph index.
No external dependencies - stdlib only.
"""


def search_nodes(index_data: dict, query: str) -> list:
    """
    Search nodes where query appears in title, summary, or tags.
    Case-insensitive. Returns list of matching node dicts.
    """
    q = query.lower().strip()
    if not q:
        return []

    results = []
    for node in index_data.get("nodes", []):
        title = node.get("title", "").lower()
        summary = node.get("summary", "").lower()
        tags = [str(t).lower() for t in node.get("tags", [])]
        category = node.get("category", "").lower()

        if q in title or q in summary or q in category or any(q in t for t in tags):
            results.append(node)

    return results
