#!/bin/bash

echo "===================================="
echo "    SHIN EVIDENCE ATLAS DASHBOARD"
echo "===================================="
echo ""

echo "[1] Running full analysis..."
python3 atlas_engine/analyze.py
echo ""

echo "[2] Running delta comparison..."
python3 atlas_engine/delta.py
echo ""

echo "[3] Recent Analysis Report"
echo "------------------------------------"
tail -n 10 reports/atlas_analysis_report.md
echo ""

echo "[4] Recent Delta Report"
echo "------------------------------------"
if [ -f reports/atlas_delta_report.md ]; then
    tail -n 10 reports/atlas_delta_report.md
else
    echo "No delta report yet."
fi

echo ""
echo "===================================="
echo " Dashboard complete."
echo "===================================="
