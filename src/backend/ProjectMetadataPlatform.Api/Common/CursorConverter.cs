using System;
using System.Text.Json;
using ProjectMetadataPlatform.Application.Helper.Models;

namespace ProjectMetadataPlatform.Api.Common;

public static class CursorConverter
{
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
