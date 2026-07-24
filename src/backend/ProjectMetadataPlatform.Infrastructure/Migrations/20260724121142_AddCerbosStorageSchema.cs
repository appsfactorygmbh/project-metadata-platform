using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMetadataPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCerbosStorageSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE SCHEMA IF NOT EXISTS cerbos;

CREATE TABLE IF NOT EXISTS cerbos.policy (
    id bigint NOT NULL PRIMARY KEY,
    kind VARCHAR(128) NOT NULL,
    name VARCHAR(1024) NOT NULL,
    version VARCHAR(128) NOT NULL,
    scope VARCHAR(512),
    description TEXT,
    disabled BOOLEAN default false,
    definition BYTEA
);

CREATE TABLE IF NOT EXISTS cerbos.policy_dependency (
    policy_id BIGINT,
    dependency_id BIGINT,
    PRIMARY KEY (policy_id, dependency_id),
    FOREIGN KEY (policy_id) REFERENCES cerbos.policy(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS cerbos.policy_ancestor (
    policy_id BIGINT,
    ancestor_id BIGINT,
    PRIMARY KEY (policy_id, ancestor_id),
    FOREIGN KEY (policy_id) REFERENCES cerbos.policy(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS cerbos.policy_revision (
    revision_id SERIAL PRIMARY KEY,
    action VARCHAR(64),
    id BIGINT,
    kind VARCHAR(128),
    name VARCHAR(1024),
    version VARCHAR(128),
    scope VARCHAR(512),
    description TEXT,
    disabled BOOLEAN,
    definition BYTEA,
    update_timestamp TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS cerbos.attr_schema_defs (
    id VARCHAR(255) PRIMARY KEY,
    definition JSON
);

CREATE OR REPLACE FUNCTION cerbos.process_policy_audit() RETURNS TRIGGER AS $policy_audit$
    BEGIN
        IF (TG_OP = 'DELETE') THEN
            INSERT INTO cerbos.policy_revision(action, id, kind, name, version, scope, description, disabled, definition)
            VALUES('DELETE', OLD.id, OLD.kind, OLD.name, OLD.version, OLD.scope, OLD.description, OLD.disabled, OLD.definition);
        ELSIF (TG_OP = 'UPDATE') THEN
            INSERT INTO cerbos.policy_revision(action, id, kind, name, version, scope, description, disabled, definition)
            VALUES('UPDATE', NEW.id, NEW.kind, NEW.name, NEW.version, NEW.scope, NEW.description, NEW.disabled, NEW.definition);
        ELSIF (TG_OP = 'INSERT') THEN
            INSERT INTO cerbos.policy_revision(action, id, kind, name, version, scope, description, disabled, definition)
            VALUES('INSERT', NEW.id, NEW.kind, NEW.name, NEW.version, NEW.scope, NEW.description, NEW.disabled, NEW.definition);
        END IF;
        RETURN NULL;
    END;
$policy_audit$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER policy_audit
AFTER INSERT OR UPDATE OR DELETE ON cerbos.policy
FOR EACH ROW EXECUTE PROCEDURE cerbos.process_policy_audit();"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP SCHEMA IF EXISTS cerbos CASCADE;");
        }
    }
}
