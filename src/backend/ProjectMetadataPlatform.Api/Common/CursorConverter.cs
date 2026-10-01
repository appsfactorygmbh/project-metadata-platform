using System;
using System.Text.Json;
using ProjectMetadataPlatform.Application.Helper.Models;

namespace ProjectMetadataPlatform.Api.Common;

/// <summary>
/// Helper Class for Decoding and Encoding Pagination Cursors.
/// </summary>
public static class CursorConverter
{
    /// <summary>
    /// Encodes a Pagination Cursor as a base64 string.
    /// </summary>
    /// <typeparam name="T">Type of Cursor.</typeparam>
    /// <param name="cursor">Cursor to be encoded.</param>
    /// <returns>Base64 encoded cursor string or null.</returns>
    public static string? Encode<T>(T? cursor)
        where T : Cursor
    {
        if (cursor == null)
        {
            return default;
        }
        var cursorBytes = JsonSerializer.SerializeToUtf8Bytes(cursor);
        return Convert.ToBase64String(cursorBytes);
    }

    /// <summary>
    /// Decodes a base64 string to a Cursor object.
    /// </summary>
    /// <typeparam name="T">Type of cursor to be decoded.</typeparam>
    /// <param name="cursor">Encoded Cursor string.</param>
    /// <returns>Cursor Object or null.</returns>
    public static T? Decode<T>(string? cursor)
        where T : Cursor
    {
        if (string.IsNullOrWhiteSpace(cursor))
        {
            return default;
        }

        var cursorBytes = Convert.FromBase64String(cursor);
        return JsonSerializer.Deserialize<T>(cursorBytes);
    }
}
