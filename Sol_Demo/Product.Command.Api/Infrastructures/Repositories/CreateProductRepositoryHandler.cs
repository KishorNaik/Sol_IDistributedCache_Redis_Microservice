using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using MediatR;
using System.Threading;
using Framework.SqlClient.Helper;
using Product.Command.Api.DTOs;
using Product.Command.Api.Infrastructures.Abstracts;
using Framework.HangFire.MediatR.Extension;
using Product.Command.Api.Infrastructures.Cache;

namespace Product.Command.Api.Infrastructures.Repositories
{
    public class CreateProductRepository : ProductRequestDTO, IRequest<bool>
    {
    }

    public sealed class CreateProductRepositoryHandler : ProductRepositoryAbstract, IRequestHandler<CreateProductRepository, bool>
    {
        private readonly ISqlClientDbProvider sqlClientDbProvider = null;
        private readonly IMediator mediator = null;

        public CreateProductRepositoryHandler(ISqlClientDbProvider sqlClientDbProvider, IMediator mediator)
        {
            this.sqlClientDbProvider = sqlClientDbProvider;
            this.mediator = mediator;
        }

        Task<bool> IRequestHandler<CreateProductRepository, bool>.Handle(CreateProductRepository request, CancellationToken cancellationToken)
        {
            var dynamicParameterTask = base.SetParameterAsync("Create-Product", request);

            var result =
                sqlClientDbProvider
                ?.DapperBuilder
                ?.OpenConnection(sqlClientDbProvider.GetConnection())
                ?.Parameter(async () => await dynamicParameterTask)
                ?.Command(async (dbConnection, dynamicParameter) =>
                {
                    var productResponse =
                       await
                       dbConnection
                       ?.QueryFirstAsync<ProductResponseDTO>(sql: "uspSetProduct", param: dynamicParameter, commandType: CommandType.StoredProcedure);

                    if (productResponse != null)
                    {
                        mediator.Enqueue(new CreateProductCacheRepository()
                        {
                            ProductIdentity = productResponse.ProductIdentity,
                            ProductName = productResponse.ProductName,
                            UnitPrice = productResponse.UnitPrice
                        });
                    }

                    return (productResponse != null) ? true : false;
                })
                ?.ResultAsync<bool>();

            return result;
        }
    }
}