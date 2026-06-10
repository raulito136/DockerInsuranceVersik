#!/bin/bash

# Start all port-forwards in background with original ports

echo "Starting port-forwards for InsuranceApp services..."

# Get WSL IP address dynamically, fallback to localhost
WSL_IP=$(hostname -I | awk '{print $1}')
WSL_IP=${WSL_IP:-localhost}

echo "Detected WSL IP: $WSL_IP"

# Kill any existing port-forwards on these ports
pkill -f "port-forward.*4200" 2>/dev/null || true
pkill -f "port-forward.*4201" 2>/dev/null || true
pkill -f "port-forward.*4202" 2>/dev/null || true
pkill -f "port-forward.*4203" 2>/dev/null || true
pkill -f "port-forward.*5001" 2>/dev/null || true
pkill -f "port-forward.*5002" 2>/dev/null || true
pkill -f "port-forward.*5003" 2>/dev/null || true
pkill -f "port-forward.*1433" 2>/dev/null || true

sleep 1

# Start all port-forwards in background on all interfaces (0.0.0.0)
kubectl port-forward -n backend svc/frontend 4200:4200 --address 0.0.0.0 > /tmp/frontend-pf.log 2>&1 &
FE_PID=$!
echo "✓ Frontend Shell: http://$WSL_IP:4200 (PID: $FE_PID)"

# Forward MFE ports (inside the frontend container)
kubectl port-forward -n backend svc/frontend 4201:4201 --address 0.0.0.0 > /tmp/mfe-refdata-pf.log 2>&1 &
MFE_RD_PID=$!
echo "✓ MFE Reference Data: http://$WSL_IP:4201 (PID: $MFE_RD_PID)"

kubectl port-forward -n backend svc/frontend 4202:4202 --address 0.0.0.0 > /tmp/mfe-policies-pf.log 2>&1 &
MFE_PO_PID=$!
echo "✓ MFE Policies: http://$WSL_IP:4202 (PID: $MFE_PO_PID)"

kubectl port-forward -n backend svc/frontend 4203:4203 --address 0.0.0.0 > /tmp/mfe-claims-pf.log 2>&1 &
MFE_CL_PID=$!
echo "✓ MFE Claims: http://$WSL_IP:4203 (PID: $MFE_CL_PID)"

# Backend APIs
kubectl port-forward -n backend svc/referencedata-api 5001:8080 --address 0.0.0.0 > /tmp/refdata-pf.log 2>&1 &
RD_PID=$!
echo "✓ Reference Data API: http://$WSL_IP:5001 (PID: $RD_PID)"

kubectl port-forward -n backend svc/policies-api 5002:8080 --address 0.0.0.0 > /tmp/policies-pf.log 2>&1 &
PO_PID=$!
echo "✓ Policies API: http://$WSL_IP:5002 (PID: $PO_PID)"

kubectl port-forward -n backend svc/claims-api 5003:8080 --address 0.0.0.0 > /tmp/claims-pf.log 2>&1 &
CL_PID=$!
echo "✓ Claims API: http://$WSL_IP:5003 (PID: $CL_PID)"

kubectl port-forward -n backend svc/sqlserver 1433:1433 --address 0.0.0.0 > /tmp/sqlserver-pf.log 2>&1 &
SQL_PID=$!
echo "✓ SQL Server: $WSL_IP:1433 (PID: $SQL_PID)"

sleep 2

echo ""
echo "============================================"
echo "All services are ready!"
echo "============================================"
echo ""
echo "Access your services from Windows:"
echo "  Frontend Shell:      http://$WSL_IP:4200"
echo "  MFE Reference Data:  http://$WSL_IP:4201"
echo "  MFE Policies:        http://$WSL_IP:4202"
echo "  MFE Claims:          http://$WSL_IP:4203"
echo "  Reference Data API:  http://$WSL_IP:5001/swagger"
echo "  Policies API:        http://$WSL_IP:5002/swagger"
echo "  Claims API:          http://$WSL_IP:5003/swagger"
echo "  SQL Server:          $WSL_IP:1433"
echo ""
echo "To stop all port-forwards, run:"
echo "  pkill -f 'port-forward -n backend'"
echo ""
echo "Logs:"
echo "  Frontend Shell:         tail -f /tmp/frontend-pf.log"
echo "  MFE Reference Data:     tail -f /tmp/mfe-refdata-pf.log"
echo "  MFE Policies:           tail -f /tmp/mfe-policies-pf.log"
echo "  MFE Claims:             tail -f /tmp/mfe-claims-pf.log"
echo "  Reference Data API:     tail -f /tmp/refdata-pf.log"
echo "  Policies API:           tail -f /tmp/policies-pf.log"
echo "  Claims API:             tail -f /tmp/claims-pf.log"
echo "  SQL Server:             tail -f /tmp/sqlserver-pf.log"
echo ""

# Keep script running so port-forwards stay active
wait
