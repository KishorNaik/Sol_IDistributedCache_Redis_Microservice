using Framework.SqlClient.Helper;
using MediatR;
using Product.Command.Api.DTOs;
using Product.Command.Api.Infrastructures.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using Framework.HangFire.MediatR.Extension;
using Product.Command.Api.Infrastructures.Cache;

namespace Product.Command.Api.Infrastructures.Repositories
{
    public class UpdateProductRepository : ProductRequestDTO, IRequest<bool>
    {
    }

    public sealed class UpdateProductRepositoryHandler : ProductRepositoryAbstract, IRequestHandler<UpdateProductRepository, bool>
    {
        private readonly ISqlClientDbProvider sqlClientDbProvider = null;
        private readonly IMediator mediator = null;

        public UpdateProductRepositoryHandler(ISqlClientDbProvider sqlClientDbProvider)
        {
            this.sqlClientDbProvider = sqlClientDbProvider;
        }

        Task<bool> IRequestHandler<UpdateProductRepository, bool>.Handle(UpdateProductRepository request, CancellationToken cancellationToken)
        {
            var dynamicParameterTask = base.SetParameterAsync("Update-Product", request);

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
                        mediator.Enqueue(new UpdateProductCacheRepository()
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