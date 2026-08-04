namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Represents a Request.
/// </summary>
/// <typeparam name="TResult">Type of the Request Response.</typeparam>
public interface IRequest<TResult> { }

/// <summary>
/// Represents a Request without Response.
/// </summary>
public interface IRequest : IRequest<Unit> { }
