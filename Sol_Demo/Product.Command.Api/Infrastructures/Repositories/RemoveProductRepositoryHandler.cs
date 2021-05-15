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
    public class RemoveProductRepository : ProductRequestDTO, IRequest<bool>
    {
    }

    public sealed class RemoveProductRepositoryHandler : ProductRepositoryAbstract, IRequestHandler<RemoveProductRepository, bool>
    {
        private readonly ISqlClientDbProvider sqlClientDbProvider = null;
        private readonly IMediator mediator = null;

        public RemoveProductRepositoryHandler(ISqlClientDbProvider sqlClientDbProvider, IMediator mediator)
        {
            this.sqlClientDbProvider = sqlClientDbProvider;
            this.mediator = mediator;
        }

        Task<bool> IRequestHandler<RemoveProductRepository, bool>.Handle(RemoveProductRepository request, CancellationToken cancellationToken)
        {
            var dynamicParameterTask = base.SetParameterAsync("Remove-Product", request);

            var result =
                sqlClientDbProvider
                ?.DapperBuilder
                ?.OpenConnection(sqlClientDbProvider.GetConnection())
                ?.Parameter(async () => await dynamicParameterTask)
                ?.Command(async (dbConnection, dynamicParameter) =>
                {
                    var noOfRowsAffected =
                    await
                    dbConnection
                    ?.ExecuteAsync(sql: "uspSetProduct", param: dynamicParameter, commandType: CommandType.StoredProcedure);

                    if (noOfRowsAffected >= 1)
                    {
                        mediator.Enqueue(new RemoveProductCacheRepository()
                        {
                            ProductIdentity = request.ProductIdentity,
                        });

                        return true;
                    }

                    return false;
                })
                ?.ResultAsync<bool>();

            return result;
        }
    }
}