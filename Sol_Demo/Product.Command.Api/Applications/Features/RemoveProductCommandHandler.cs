using MediatR;
using Product.Command.Api.Infrastructures.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Product.Command.Api.Applications.Features
{
    [DataContract]
    public class RemoveProductCommand : IRequest<bool>
    {
        [DataMember(EmitDefaultValue = false)]
        public Guid? ProductIdentity { get; set; }
    }

    public sealed class RemoveProductCommandHandler : IRequestHandler<RemoveProductCommand, bool>
    {
        private readonly IMediator mediator = null;

        public RemoveProductCommandHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        Task<bool> IRequestHandler<RemoveProductCommand, bool>.Handle(RemoveProductCommand request, CancellationToken cancellationToken)
        {
            return mediator.Send<bool>(new RemoveProductRepository()
            {
                ProductIdentity = request.ProductIdentity
            });
        }
    }
}