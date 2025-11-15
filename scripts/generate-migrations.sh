#!/usr/bin/env bash
set -euo pipefail

# Usage: scripts/generate-migrations.sh <MigrationName>
# Reads connection strings from environment variables or falls back to defaults below.
#   SQLITE_DB_CONN_STR      default: sephirah.db
#   MYSQL_DB_CONN_STR       default: Server=localhost;Port=3306;Database=librarian;User=root;Password=pass;
#   POSTGRESQL_DB_CONN_STR  default: Host=localhost;Port=5432;Database=librarian;Username=postgres;Password=pass
#
# Example:
#   SQLITE_DB_CONN_STR="sephirah.db" \
#   MYSQL_DB_CONN_STR="Server=127.0.0.1;Port=3306;Database=librarian;User=root;Password=***;" \
#   POSTGRESQL_DB_CONN_STR="Host=127.0.0.1;Port=5432;Database=librarian;Username=postgres;Password=***" \
#   scripts/generate-migrations.sh Init

if [[ $# -lt 1 ]]; then
  echo "Usage: $0 <MigrationName>"
  exit 1
fi
MIGRATION_NAME="$1"

# Defaults
SQLITE_DB_CONN_STR_DEFAULT="sephirah.db"
MYSQL_DB_CONN_STR_DEFAULT="Server=localhost;Port=3306;Database=librarian;User=root;Password=pass;"
POSTGRESQL_DB_CONN_STR_DEFAULT="Host=localhost;Port=5432;Database=librarian;Username=postgres;Password=pass;"

# Read env or default
SQLITE_DB_CONN_STR="${SQLITE_DB_CONN_STR:-$SQLITE_DB_CONN_STR_DEFAULT}"
MYSQL_DB_CONN_STR="${MYSQL_DB_CONN_STR:-$MYSQL_DB_CONN_STR_DEFAULT}"
POSTGRESQL_DB_CONN_STR="${POSTGRESQL_DB_CONN_STR:-$POSTGRESQL_DB_CONN_STR_DEFAULT}"

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

if ! command -v dotnet-ef >/dev/null 2>&1; then
  echo "dotnet-ef not found. Install with:"
  echo "  dotnet tool update --global dotnet-ef"
  exit 1
fi

run_migration () {
  local provider="$1"
  local project="$2"
  local dbtype="$3"
  local conn="$4"

  echo ""
  echo "==> Generating migration for ${provider}: ${MIGRATION_NAME}"
  dotnet ef migrations add "${MIGRATION_NAME}" \
    --project "${project}" \
    --startup-project "Librarian.Sephirah.Server" \
    --context "ApplicationDbContext" \
    --output-dir "Migrations" \
    -- \
    --DbType="${dbtype}" \
    --DbConnStr="${conn}"
}

run_migration "SQLite"     "Librarian.Common.Migrations.SQLite"      "sqlite"      "${SQLITE_DB_CONN_STR}"
run_migration "MySQL"      "Librarian.Common.Migrations.MySql"       "mysql"       "${MYSQL_DB_CONN_STR}"
run_migration "PostgreSQL" "Librarian.Common.Migrations.PostgreSQL"  "postgresql"  "${POSTGRESQL_DB_CONN_STR}"

echo ""
echo "All migrations generated."


