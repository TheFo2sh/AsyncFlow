using AsyncFlow.Responses;

namespace AsyncFlow.Interfaces;


/// <summary>
/// Represents an asynchronous flow that processes a request of type <typeparamref name="TRequest"/> and returns a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the input request.</typeparam>
/// <typeparam name="TResult">The type of the result returned after processing the request.</typeparam>
public interface IAsyncFlow<in TRequest, TResult>
{
    /// <summary>
    /// Processes the specified request asynchronously, optionally reports progress, and returns the result.
    /// </summary>
    /// <param name="request">The request of type <typeparamref name="TRequest"/> to process.</param>
    /// <param name="progress">An optional progress reporter that can report updates about the processing progress.</param>
    /// <param name="cancellationToken">An  token to observe for cancellation requests. It allows the operation to be cancelled if requested.</param>
    /// <returns>A task that represents the asynchronous operation, containing the processed result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> ProcessAsync(TRequest request, IProgress<ProgressData> progress , CancellationToken cancellationToken );
}
