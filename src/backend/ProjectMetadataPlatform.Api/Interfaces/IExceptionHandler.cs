using Microsoft.AspNetCore.Mvc;

namespace ProjectMetadataPlatform.Api.Interfaces;

/// <summary>
/// Defines a handler for exceptions of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of exception to handle.</typeparam>
public interface IExceptionHandler<in T>
{
    /// <summary>
    /// Handles the specified exception and returns an appropriate <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of handling the exception.</returns>
    public IActionResult? Handle(T exception);
}
