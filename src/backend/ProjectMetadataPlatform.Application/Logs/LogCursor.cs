using System;
using ProjectMetadataPlatform.Application.Helper.Models;

namespace ProjectMetadataPlatform.Application.Logs;

/// <summary>
/// Cursor record pointing towards a Log Resource.
/// </summary>
/// <param name="TimeStamp">Timestamp of the Log Entry.</param>
/// <param name="Id">Id of the Log Entry.</param>
public record LogCursor(DateTimeOffset TimeStamp, int Id) : Cursor;
