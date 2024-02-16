using Microsoft.Extensions.DependencyInjection;
using Reineur.Share;
using System.Text.Json.Serialization;

namespace Reineur.Share.MediatKO
{
    public interface IMediator
    {
        Task<ActionResponse> SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : Request;
    }

    public abstract class Request
    {
        [JsonIgnore]
        public virtual Guid Id { get; set; }
        public virtual ActionResponse Validate() => new();
    }
    public interface IRequestHandler<TRequest> where TRequest : Request
    {
        Task<ActionResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
    }


    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<ActionResponse> SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : Request
        {
            var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();
            return handler.HandleAsync(request, cancellationToken);
        }
    }

}

