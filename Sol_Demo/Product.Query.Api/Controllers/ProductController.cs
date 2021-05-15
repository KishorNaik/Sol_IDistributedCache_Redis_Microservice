using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Product.Query.Api.Applications.Features;
using Product.Query.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Query.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/product-query")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator mediator = null;

        public ProductController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("get-all-products")]
        public async Task<IActionResult> GetAllProductList()
        {
            var productList = await mediator.Send<IReadOnlyList<ProductResponseDTO>>(new GetAllProductQuery());

            if (productList == null) base.Content("No Records Found");

            return base.Ok(productList);
        }
    }
}