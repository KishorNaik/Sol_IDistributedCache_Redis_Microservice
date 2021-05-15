using Hangfire;
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
    public class RemoveProductCacheRepository : ProductRequestDTO, INotification
    {
    }

    public sealed class RemoveProductCacheRepositoryHandler : INotificationHandler<RemoveProductCacheRepository>
    {
        private readonly IDistributedCache distributedCache = null;
        private static readonly object CacheLockObject = new object();

        public RemoveProductCacheRepositoryHandler(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        //[AutomaticRetry(Attempts = 0)]
        async Task INotificationHandler<RemoveProductCacheRepository>.Handle(RemoveProductCacheRepository notification, CancellationToken cancellationToken)
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
                        // Remove
                        var productResponseObj = productResponsesList.FirstOrDefault((productResponse) => productResponse.ProductIdentity == notification.ProductIdentity);
                        productResponsesList.Remove(productResponseObj);

                        distributedCache.Remove("Product-List");
                        distributedCache.SetString("Product-List", JsonConvert.SerializeObject(productResponsesList));
                    }
                }
            }
        }
    }
}