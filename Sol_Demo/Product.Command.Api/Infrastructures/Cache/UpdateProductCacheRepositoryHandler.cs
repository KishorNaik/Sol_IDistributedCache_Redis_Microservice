using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Product.Command.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Product.Command.Api.Infrastructures.Cache
{
    public class UpdateProductCacheRepository : ProductRequestDTO, INotification
    {
    }

    public sealed class UpdateProductCacheRepositoryHandler : INotificationHandler<UpdateProductCacheRepository>
    {
        private readonly IDistributedCache distributedCache = null;
        private static readonly object CacheLockObject = new object();

        public UpdateProductCacheRepositoryHandler(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        async Task INotificationHandler<UpdateProductCacheRepository>.Handle(UpdateProductCacheRepository notification, CancellationToken cancellationToken)
        {
            List<ProductResponseDTO> productResponsesList = null;
            var cacheList = await distributedCache?.GetStringAsync("Product-List");

            if (cacheList != null)
            {
                lock (CacheLockObject)
                {
                    cacheList = distributedCache?.GetString("Product-List");

                    if (cacheList != null)
                    {
                        productResponsesList = JsonConvert.DeserializeObject<List<ProductResponseDTO>>(cacheList);
                        // Update
                        var productResponseObj = productResponsesList.FirstOrDefault((productResponse) => productResponse.ProductIdentity == notification.ProductIdentity);
                        productResponseObj.ProductName = notification.ProductName;
                        productResponseObj.UnitPrice = notification.UnitPrice;

                        distributedCache.Remove("Product-List");
                        distributedCache.SetString("Product-List", JsonConvert.SerializeObject(productResponsesList));
                    }
                }
            }
        }
    }
}