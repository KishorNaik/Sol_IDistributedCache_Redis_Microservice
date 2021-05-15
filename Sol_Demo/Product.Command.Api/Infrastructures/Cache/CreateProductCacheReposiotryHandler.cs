using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Product.Command.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Product.Command.Api.Infrastructures.Cache
{
    public class CreateProductCacheRepository : ProductRequestDTO, INotification
    {
    }

    public sealed class CreateProductCacheReposiotryHandler : INotificationHandler<CreateProductCacheRepository>
    {
        private readonly IDistributedCache distributedCache = null;
        private static readonly object CacheLockObject = new object();

        public CreateProductCacheReposiotryHandler(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        async Task INotificationHandler<CreateProductCacheRepository>.Handle(CreateProductCacheRepository notification, CancellationToken cancellationToken)
        {
            List<ProductResponseDTO> productResponsesList = null;
            var cacheList = await distributedCache?.GetStringAsync("Product-List");

            if (cacheList == null)
            {
                productResponsesList = new List<ProductResponseDTO>();
                productResponsesList.Add(Mapping(notification));
                await distributedCache.SetStringAsync("Product-List", JsonConvert.SerializeObject(productResponsesList));
            }
            else
            {
                lock (CacheLockObject)
                {
                    cacheList = distributedCache?.GetString("Product-List");

                    if (cacheList != null)
                    {
                        productResponsesList = JsonConvert.DeserializeObject<List<ProductResponseDTO>>(cacheList);
                        productResponsesList.Add(Mapping(notification));
                        distributedCache.Remove("Product-List");
                        distributedCache.SetString("Product-List", JsonConvert.SerializeObject(productResponsesList));
                    }
                }
            }

            ProductResponseDTO Mapping(CreateProductCacheRepository notification)
            {
                return new ProductResponseDTO()
                {
                    ProductIdentity = notification.ProductIdentity,
                    ProductName = notification.ProductName,
                    UnitPrice = notification.UnitPrice
                };
            }
        }
    }
}