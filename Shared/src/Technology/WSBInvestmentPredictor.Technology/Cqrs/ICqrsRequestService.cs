namespace WSBInvestmentPredictor.Technology.Cqrs;

public interface ICqrsRequestService
{
    Task<TResult> Handle<TRequest, TResult>(TRequest request)
        where TRequest : class;

    Task Handle<TRequest>(TRequest request)
        where TRequest : class;
}
