# Guardian Interface Specification

## Specification ID

**v1.0.3 — Archive Validator**

**Status:** Approved for Implementation

**Checkpoint Target:** `v1_0_3_archive_validator`

---

# Purpose

The Archive Validator verifies that every research document in the Guardian Interface archive meets the minimum structural requirements before the Knowledge Graph Builder is executed.

The validator performs analysis only. It never modifies files.

Its purpose is to identify problems early, produce a clear validation report, and prevent malformed data from silently entering the Knowledge Graph.

---

# Input

```
master_research_archive/**/*.md
```

All Markdown research documents contained within the research archive.

---

# Output

A validation report displayed in the terminal.

Example:

```
Guardian Interface Archive Validator
====================================

Scanning archive...

Documents Found: 2

PASS
✓ UID
✓ Title
✓ Category
✓ Associated Somatic Nodes

PASS
✓ Duplicate UID Check

PASS
✓ Front Matter

Warnings: 0

Errors: 0

Archive Status:
PASS
```

---

# Required Metadata

Every document should contain:

* uid
* title
* category
* associated_somatic_nodes

Recommended fields:

* sub_category
* source_type
* status
* target_meridians
* telemetry_triggers
* tags
* citations

Missing recommended fields generate warnings but do not fail validation.

---

# Validation Checks

## Required Metadata

Verify required fields exist.

---

## UID Validation

* UID present
* UID non-empty
* UID unique across archive

Duplicate UIDs produce an error.

---

## Title Validation

Verify title exists and is not empty.

---

## Category Validation

Verify category exists.

---

## Somatic Node Validation

Verify:

* field exists
* field is a list
* each value is an integer
* duplicate node IDs within the same document are reported as warnings

---

## Front Matter Validation

Verify:

* opening delimiter exists
* closing delimiter exists
* front matter parses successfully

Malformed front matter produces an error.

---

# Validation Summary

At completion, display:

* Documents scanned
* Passes
* Warnings
* Errors

---

# Exit Status

Exit code 0

Validation successful.

Exit code 1

Validation failed because one or more errors were detected.

---

# Scope

The validator performs validation only.

It does not:

* build the Knowledge Graph
* modify Markdown files
* rewrite metadata
* generate JSON

---

# Future Enhancements

Future versions may include:

* citation validation
* broken cross-reference detection
* orphan document detection
* tag consistency checking
* telemetry trigger validation
* meridian validation
* schema version compatibility
* PDF attachment validation
* image asset validation

---

# Success Criteria

The validator is considered complete when:

* It scans the archive successfully.
* It detects malformed documents.
* It reports warnings and errors clearly.
* It exits with the appropriate status code.
* It can be run before every Knowledge Graph build.

---

# Development Philosophy

The Archive Validator is the first quality assurance tool for the Guardian Interface Research Operating System.

Its responsibility is to ensure that every document entering the Knowledge Graph meets a consistent structural standard while remaining easy to maintain and extend.

