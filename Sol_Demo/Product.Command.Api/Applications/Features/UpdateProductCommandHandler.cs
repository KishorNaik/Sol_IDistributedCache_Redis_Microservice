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
    public class UpdateProductCommand : IRequest<bool>
    {
        [DataMember(EmitDefaultValue = false)]
        public Guid? ProductIdentity { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public String ProductName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double? UnitPrice { get; set; }
    }

    public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IMediator mediator = null;

        public UpdateProductCommandHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        Task<bool> IRequestHandler<UpdateProductCommand, bool>.Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            return mediator.Send<bool>(new UpdateProductRepository()
            {
                ProductIdentity = request.ProductIdentity,
                ProductName = request.ProductName,
                UnitPrice = request.UnitPrice
            });
        }
    }
}