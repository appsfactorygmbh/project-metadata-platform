# How to use:
# pws ./dotnet_ef.ps1 <my parameters>
# Example:
# pws ./dotnet_ef.ps1 migrations list
pwsh -CommandWithArgs '$env:PMP_DB_URL="localhost";$env:PMP_DB_PORT = "5432";$env:PMP_DB_NAME = "pmp_db_local";$env:PMP_DB_USER = "postgres";$env:PMP_DB_PASSWORD = "postgres";$env:JWT_VALID_ISSUER = "validIssuer";$env:JWT_VALID_AUDIENCE = "validAudience";$env:JWT_ISSUER_SIGNING_KEY = "superSecretKeyThatIsAtLeast257BitLong@345";$env:REFRESH_TOKEN_EXPIRATION_HOURS= "6";$env:ACCESS_TOKEN_EXPIRATION_MINUTES= "15";dotnet ef -p ..\ProjectMetadataPlatform.Infrastructure\ -s . $args' $args
