using System;

namespace ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

/// <summary>
/// Represents an exception that occurs while accessing the database.
/// </summary>
public class DatabaseException(Exception innerException)
    : PmpException("An Error occured while accessing the database.", innerException);
