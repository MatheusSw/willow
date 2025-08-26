using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Serilog;

namespace admin_application.Utilities;

public static class ResultHelpers
{
    /// <summary>
    /// Checks whether the provided result has failed. If it has, logs the provided message and
    /// returns true, outputting the provided failureMessage via out parameter so callers can
    /// return a typed failure (e.g., Result.Fail&lt;T&gt;(message)).
    /// </summary>
    public static bool TryGetFailure<TSource, TResult>(Result<TSource>? result,
        string failureMessage, [MaybeNullWhen(false)] out Result<TResult> failureResult)
    {
        if (result == null || result.IsFailed || result.Value == null)
        {
            failureResult = Result.Fail<TResult>(failureMessage);

            return true;
        }

        failureResult = null;

        return false;
    }
}