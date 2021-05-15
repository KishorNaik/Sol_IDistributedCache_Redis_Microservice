using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Product.Command.Api.DTOs;

namespace Product.Command.Api.Infrastructures.Abstracts
{
    public abstract class ProductRepositoryAbstract
    {
        protected Task<DynamicParameters> SetParameterAsync(string command, ProductRequestDTO productRequest)
        {
            return Task.Run(() =>
            {
                DynamicParameters dynamicParameters = new();

                dynamicParameters.Add("@Command", command, DbType.String, direction: ParameterDirection.Input);
                dynamicParameters.Add("@ProductIdentity", productRequest?.ProductIdentity, DbType.Guid, direction: ParameterDirection.Input);
                dynamicParameters.Add("@ProductName", productRequest?.ProductName, DbType.String, direction: ParameterDirection.Input);
                dynamicParameters.Add("@UnitPrice", productRequest?.UnitPrice, DbType.String, direction: ParameterDirection.Input);

                return dynamicParameters;
            });
        }
    }
}