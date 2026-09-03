namespace ProjectMetadataPlatform.Domain.Errors.BillingExceptions;

/// <summary>
/// Thrown when someone tries to add / update billing information notes that are longer than 280 characters.
/// </summary>
/// <param name="notesLength">length of the notes.</param>
public class PluginBillingNotesSizeException(int notesLength)
    : BillingException(
        "The billing notes are " + notesLength + " chars long. Maximum allowed is 280 chars."
    );
