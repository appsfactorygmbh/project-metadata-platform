using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;

/// <summary>
/// Exception thrown when a project has notes larger than 500 chars..
/// </summary>
/// <param name="notesLength">Length of the project Notes.</param>
public class ProjectNotesSizeException(int notesLength)
    : ProjectException(
        "The project notes are " + notesLength + " chars long. Maximum allowed is 500 chars."
    );
