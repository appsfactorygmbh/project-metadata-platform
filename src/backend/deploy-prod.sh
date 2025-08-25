#!/bin/bash
SECRET_PATH="AF-IT/data/ProjectMetadataPlatformBackend"

set -e # Exit immediately if a command fails
set -o pipefail # Exit if any command in a pipeline fails

# --- Pre-flight Checks ---
if ! command -v jq &> /dev/null; then
    echo "Error: jq is not installed. Please install it to parse Vault's JSON response."
    exit 1
fi

if [ -z "$VAULT_ADDR" ] || [ -z "$VAULT_TOKEN" ]; then
    echo "Error: VAULT_ADDR and VAULT_TOKEN environment variables must be set."
    exit 1
fi

# --- Fetch Secrets from Vault ---
echo "--> Fetching secrets from Vault at path: $SECRET_PATH"

SECRETS_JSON=$(curl -s -L \
    -H "X-Vault-Token: $VAULT_TOKEN" \
    "$VAULT_ADDR/v1/$SECRET_PATH")

echo "Response: $SECRETS_JSON"

if [ -z "$SECRETS_JSON" ] || ! echo "$SECRETS_JSON" | jq -e '.data.data' > /dev/null; then
    echo "Error: Failed to retrieve secrets from Vault. Check path, token, and permissions."
    exit 1
fi

# --- Parse and Export Secrets ---
export PMP_DB_USER=$(echo "$SECRETS_JSON" | jq -r '.data.data.PMP_DB_USER')
export PMP_DB_PASSWORD=$(echo "$SECRETS_JSON" | jq -r '.data.data.PMP_DB_PASSWORD')
export PMP_DB_NAME=$(echo "$SECRETS_JSON" | jq -r '.data.data.PMP_DB_NAME')
export JWT_ISSUER_SIGNING_KEY=$(echo "$SECRETS_JSON" | jq -r '.data.data.JWT_ISSUER_SIGNING_KEY')
export PMP_ADMIN_PASSWORD=$(echo "$SECRETS_JSON" | jq -r '.data.data.PMP_ADMIN_PASSWORD')

# --- Final Validation Step ---
# Check if all critical variables were successfully parsed from Vault.
echo "--> Validating secrets..."
if [ -z "$PMP_DB_USER" ] || \
   [ -z "$PMP_DB_PASSWORD" ] || \
   [ -z "$PMP_DB_NAME" ] || \
   [ -z "$JWT_ISSUER_SIGNING_KEY" ] || \
   [ -z "$PMP_ADMIN_PASSWORD" ]; then
    echo "Error: One or more secrets could not be parsed from Vault."
    echo "Please ensure all required keys (PMP_DB_USER, PMP_DB_PASSWORD, etc.) exist in the secret at '$SECRET_PATH'."
    exit 1
fi

echo "--> Secrets loaded and validated successfully."

# --- Deploy with Docker Compose ---
echo "--> Starting deployment with Docker Compose..."

docker compose -f ~/pmp-backend/docker-compose-production.yml pull
docker compose -f ~/pmp-backend/docker-compose-production.yml up -d

echo "--> Deployment complete."