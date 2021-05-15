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
    public class CreateProductCommand : IRequest<bool>
    {
        [DataMember(EmitDefaultValue = false)]
        public String ProductName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double? UnitPrice { get; set; }
    }

    public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, bool>
    {
        private readonly IMediator mediator = null;

        public CreateProductCommandHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        Task<bool> IRequestHandler<CreateProductCommand, bool>.Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            return mediator.Send(new CreateProductRepository()
            {
                ProductName = request.ProductName,
                UnitPrice = request.UnitPrice
            });
        }
    }
}