namespace Models;

/// <summary>
/// A tokenize response, wrapper around the
/// response's raw <paramref name="Content"/> value.
/// </summary>
/// <param name="Content">
/// The content provided for the given response.
/// </param>
public record class TokenizedResponse(string Content);
