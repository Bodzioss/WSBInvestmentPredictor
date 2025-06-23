namespace WSBInvestmentPredictor.Technology.Cqrs;

/// <summary>
/// Defines a service for handling CQRS (Command Query Responsibility Segregation) requests.
/// This interface provides methods for sending requests to the backend and receiving responses.
/// </summary>
public interface ICqrsRequestService
{
    /// <summary>
    /// Handles a CQRS request and returns a response of the specified type.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object.</typeparam>
    /// <typeparam name="TResult">The type of the response object.</typeparam>
    /// <param name="request">The request object to send.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response object.</returns>
    Task<TResult> Handle<TRequest, TResult>(TRequest request)
        where TRequest : class;

    /// <summary>
    /// Handles a CQRS request that does not return a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object.</typeparam>
    /// <param name="request">The request object to send.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Handle<TRequest>(TRequest request)
        where TRequest : class;
}