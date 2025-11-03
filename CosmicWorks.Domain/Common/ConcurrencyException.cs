namespace CosmicWorks.Domain.Common;

/// <summary>
/// Exception thrown when an optimistic concurrency conflict is detected
/// while persisting domain entities (e.g., ETag/If-Match mismatch).
/// </summary>
public sealed class ConcurrencyException : Exception
{
    public ConcurrencyException(string message, Exception? inner = null) : base(message, inner) { }
}