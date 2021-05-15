using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Product.Query.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Product.Query.Api.Applications.Features
{
    public class GetAllProductQuery : IRequest<IReadOnlyList<ProductResponseDTO>>
    {
    }

    public sealed class GetAllProductsQueryHandler : IRequestHandler<GetAllProductQuery, IReadOnlyList<ProductResponseDTO>>
    {
        private readonly IDistributedCache distributedCache = null;

        public GetAllProductsQueryHandler(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        async Task<IReadOnlyList<ProductResponseDTO>> IRequestHandler<GetAllProductQuery, IReadOnlyList<ProductResponseDTO>>.Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var productCacheList = await distributedCache?.GetStringAsync("Product-List");

            if (productCacheList != null)
            {
                var productList = JsonConvert.DeserializeObject<List<ProductResponseDTO>>(productCacheList);
                return productList.AsReadOnly();
            }

            return null;
        }
    }
}