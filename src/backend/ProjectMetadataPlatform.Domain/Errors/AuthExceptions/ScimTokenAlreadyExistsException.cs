using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

/// <summary>
/// Exception thrown when a Token of the type SCIM already exists in the Project Metadata Platform.
/// </summary>
public class ScimTokenAlreadyExistsException()
    : EntityAlreadyExistsException(
        "There already exists a Api-Token of the type SCIM. Either use the existing Token or delete it to create a new one."
    );
