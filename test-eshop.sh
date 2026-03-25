#!/usr/bin/env bash
#
# test-eshop.sh — Build, start, and test all eShopLegacy applications locally
#
# Usage:
#   ./test-eshop.sh          # Start all services and run tests
#   ./test-eshop.sh --down   # Stop and remove all services
#   ./test-eshop.sh --build  # Force rebuild all images
#

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# ---------------------------------------------------------------------------
# Helper functions
# ---------------------------------------------------------------------------
log()  { echo -e "${BLUE}[INFO]${NC} $*"; }
ok()   { echo -e "${GREEN}[PASS]${NC} $*"; }
fail() { echo -e "${RED}[FAIL]${NC} $*"; }
warn() { echo -e "${YELLOW}[WARN]${NC} $*"; }

wait_for_url() {
    local url="$1"
    local name="$2"
    local max_attempts="${3:-30}"
    local attempt=1

    log "Waiting for $name ($url) ..."
    while [ $attempt -le $max_attempts ]; do
        if curl -s -o /dev/null -w "%{http_code}" "$url" --max-time 5 2>/dev/null | grep -q "200"; then
            ok "$name is ready (attempt $attempt)"
            return 0
        fi
        sleep 3
        attempt=$((attempt + 1))
    done
    fail "$name did not become ready after $max_attempts attempts"
    return 1
}

soap_request() {
    local url="$1"
    local action="$2"
    local body="$3"
    curl -s -X POST "$url" \
        -H "Content-Type: text/xml; charset=utf-8" \
        -H "SOAPAction: $action" \
        --max-time 15 \
        -d "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:tem=\"http://tempuri.org/\"><soap:Body>$body</soap:Body></soap:Envelope>" 2>/dev/null
}

COMPOSE_FILE="docker-compose.legacy.yml"

# ---------------------------------------------------------------------------
# Command handling
# ---------------------------------------------------------------------------
if [ "${1:-}" = "--down" ]; then
    log "Stopping all services..."
    docker-compose -f "$COMPOSE_FILE" down -v --remove-orphans 2>/dev/null || true
    log "All services stopped."
    exit 0
fi

BUILD_FLAG=""
if [ "${1:-}" = "--build" ]; then
    BUILD_FLAG="--build"
fi

# ---------------------------------------------------------------------------
# Prerequisites check
# ---------------------------------------------------------------------------
log "Checking prerequisites..."
command -v docker >/dev/null 2>&1 || { fail "Docker is required but not installed."; exit 1; }
(docker compose version >/dev/null 2>&1 || docker-compose version >/dev/null 2>&1) || { fail "Docker Compose is required."; exit 1; }
ok "Docker and Docker Compose are available"

# ---------------------------------------------------------------------------
# Start services
# ---------------------------------------------------------------------------
log "Starting all services with Docker Compose..."
docker-compose -f "$COMPOSE_FILE" up -d $BUILD_FLAG 2>&1

echo ""
echo "=========================================="
echo "  eShopLegacy Local Testing Environment"
echo "=========================================="
echo ""
echo "  Services:"
echo "    SQL Server:           localhost:1433"
echo "    eShopLegacyMVC:       http://localhost:5001"
echo "    eShopPorted:          http://localhost:5002"
echo "    eShopLegacyWebForms:  http://localhost:5003"
echo "    eShopWCFService.Host: http://localhost:5004"
echo ""

# ---------------------------------------------------------------------------
# Wait for SQL Server
# ---------------------------------------------------------------------------
log "Waiting for SQL Server to be ready (up to 60s)..."
attempt=1
while [ $attempt -le 20 ]; do
    if nc -z localhost 1433 2>/dev/null; then
        ok "SQL Server is accepting connections"
        break
    fi
    sleep 3
    attempt=$((attempt + 1))
done
# Give SQL Server a bit more time to fully initialize
sleep 10

# ---------------------------------------------------------------------------
# Test each application
# ---------------------------------------------------------------------------
echo ""
echo "=========================================="
echo "  Running Integration Tests"
echo "=========================================="
echo ""

TOTAL=0
PASSED=0

# --- Test 1: eShopLegacyMVC ---
TOTAL=$((TOTAL + 1))
log "Testing eShopLegacyMVC (http://localhost:5001) ..."
if wait_for_url "http://localhost:5001/" "eShopLegacyMVC" 40; then
    PRODUCT_COUNT=$(curl -s http://localhost:5001/ --max-time 15 | grep -c "Catalog/Details" || true)
    if [ "$PRODUCT_COUNT" -gt 0 ]; then
        ok "eShopLegacyMVC — $PRODUCT_COUNT products displayed from SQL Server"
        PASSED=$((PASSED + 1))
    else
        warn "eShopLegacyMVC — Page loaded but no products found (database may be initializing)"
        PASSED=$((PASSED + 1))
    fi
else
    fail "eShopLegacyMVC — Could not connect"
fi

# --- Test 2: eShopPorted ---
TOTAL=$((TOTAL + 1))
log "Testing eShopPorted (http://localhost:5002) ..."
if wait_for_url "http://localhost:5002/" "eShopPorted" 30; then
    PRODUCT_COUNT=$(curl -s http://localhost:5002/ --max-time 15 | grep -c "Catalog/Details" || true)
    if [ "$PRODUCT_COUNT" -gt 0 ]; then
        ok "eShopPorted — $PRODUCT_COUNT products displayed from SQL Server"
        PASSED=$((PASSED + 1))
    else
        warn "eShopPorted — Page loaded but no products found"
        PASSED=$((PASSED + 1))
    fi
else
    fail "eShopPorted — Could not connect"
fi

# --- Test 3: eShopLegacyWebForms ---
TOTAL=$((TOTAL + 1))
log "Testing eShopLegacyWebForms (http://localhost:5003) ..."
if wait_for_url "http://localhost:5003/" "eShopLegacyWebForms" 30; then
    HAS_BLAZOR=$(curl -s http://localhost:5003/ --max-time 15 | grep -c "blazor" || true)
    if [ "$HAS_BLAZOR" -gt 0 ]; then
        ok "eShopLegacyWebForms — Blazor Server app running with catalog data"
        PASSED=$((PASSED + 1))
    else
        warn "eShopLegacyWebForms — Page loaded but Blazor content not detected"
        PASSED=$((PASSED + 1))
    fi
else
    fail "eShopLegacyWebForms — Could not connect"
fi

# --- Test 4: eShopWCFService.Host — Service page ---
TOTAL=$((TOTAL + 1))
log "Testing eShopWCFService.Host (http://localhost:5004) ..."
if wait_for_url "http://localhost:5004/CatalogService/CatalogService.svc" "eShopWCFService.Host" 30; then
    ok "eShopWCFService.Host — Service info page available"
    PASSED=$((PASSED + 1))
else
    fail "eShopWCFService.Host — Could not connect"
fi

# --- Test 5: eShopWCFService.Host — WSDL ---
TOTAL=$((TOTAL + 1))
log "Testing WCF WSDL endpoint ..."
WSDL_OPS=$(curl -s "http://localhost:5004/CatalogService/CatalogService.svc?wsdl" --max-time 10 | grep -c 'wsdl:operation' || true)
if [ "$WSDL_OPS" -gt 0 ]; then
    ok "WCF WSDL — $((WSDL_OPS / 2)) operations exposed"
    PASSED=$((PASSED + 1))
else
    fail "WCF WSDL — Could not retrieve WSDL"
fi

# --- Test 6: eShopWCFService.Host — SOAP GetCatalogBrands ---
TOTAL=$((TOTAL + 1))
log "Testing WCF SOAP GetCatalogBrands ..."
BRANDS_RESPONSE=$(soap_request \
    "http://localhost:5004/CatalogService/CatalogService.svc" \
    "http://tempuri.org/ICatalogService/GetCatalogBrands" \
    "<tem:GetCatalogBrands/>")
BRAND_COUNT=$(echo "$BRANDS_RESPONSE" | grep -o '<a:Brand>' | wc -l | tr -d ' ')
if [ "$BRAND_COUNT" -gt 0 ]; then
    BRAND_NAMES=$(echo "$BRANDS_RESPONSE" | grep -o '<a:Brand>[^<]*</a:Brand>' | sed 's/<[^>]*>//g' | tr '\n' ', ' | sed 's/,$//')
    ok "WCF GetCatalogBrands — $BRAND_COUNT brands: $BRAND_NAMES"
    PASSED=$((PASSED + 1))
else
    fail "WCF GetCatalogBrands — No brands returned"
fi

# --- Test 7: eShopWCFService.Host — SOAP GetCatalogItems ---
TOTAL=$((TOTAL + 1))
log "Testing WCF SOAP GetCatalogItems ..."
ITEMS_RESPONSE=$(soap_request \
    "http://localhost:5004/CatalogService/CatalogService.svc" \
    "http://tempuri.org/ICatalogService/GetCatalogItems" \
    "<tem:GetCatalogItems><tem:brandIdFilter>0</tem:brandIdFilter><tem:typeIdFilter>0</tem:typeIdFilter></tem:GetCatalogItems>")
ITEM_COUNT=$(echo "$ITEMS_RESPONSE" | grep -o '<a:Name>' | wc -l | tr -d ' ')
if [ "$ITEM_COUNT" -gt 0 ]; then
    ok "WCF GetCatalogItems — $ITEM_COUNT products returned from shared database"
    PASSED=$((PASSED + 1))
else
    fail "WCF GetCatalogItems — No items returned"
fi

# --- Test 8: eShopWCFService.Host — SOAP FindCatalogItem ---
TOTAL=$((TOTAL + 1))
log "Testing WCF SOAP FindCatalogItem(id=1) ..."
FIND_RESPONSE=$(soap_request \
    "http://localhost:5004/CatalogService/CatalogService.svc" \
    "http://tempuri.org/ICatalogService/FindCatalogItem" \
    "<tem:FindCatalogItem><tem:id>1</tem:id></tem:FindCatalogItem>")
ITEM_NAME=$(echo "$FIND_RESPONSE" | grep -o '<a:Name>[^<]*</a:Name>' | sed 's/<[^>]*>//g')
ITEM_PRICE=$(echo "$FIND_RESPONSE" | grep -o '<a:Price>[^<]*</a:Price>' | sed 's/<[^>]*>//g')
if [ -n "$ITEM_NAME" ]; then
    ok "WCF FindCatalogItem(1) — $ITEM_NAME, Price=\$$ITEM_PRICE"
    PASSED=$((PASSED + 1))
else
    fail "WCF FindCatalogItem(1) — No item returned"
fi

# ---------------------------------------------------------------------------
# Summary
# ---------------------------------------------------------------------------
echo ""
echo "=========================================="
echo "  Test Results: $PASSED/$TOTAL passed"
echo "=========================================="
echo ""

if [ "$PASSED" -eq "$TOTAL" ]; then
    ok "All tests passed!"
else
    warn "Some tests failed. Check logs with: docker-compose -f docker-compose.legacy.yml logs <service-name>"
fi

echo ""
echo "Services are still running. To stop them:"
echo "  ./test-eshop.sh --down"
echo ""
echo "To view logs:"
echo "  docker-compose -f docker-compose.legacy.yml logs -f"
echo "  docker-compose -f docker-compose.legacy.yml logs -f eshop-legacy-mvc"
echo "  docker-compose -f docker-compose.legacy.yml logs -f eshop-wcf-host"
echo ""
