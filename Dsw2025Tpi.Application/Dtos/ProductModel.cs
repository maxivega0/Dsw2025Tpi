using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public static class ProductModel
    {
        public record ProductRequest(string Sku, string InternalCode, string Name, string? Description, decimal CurrentUnitPrice, int StockQuantity, string Image);
        public record ProductResponse(Guid Id, string Sku, string InternalCode, string Name, string? Description, decimal CurrentUnitPrice, int StockQuantity, string Image, bool isActive);
        public record PaginationResponse(List<ProductResponse> ProductItems, int Total); 
        public record FilterProductRequest(string? Status, string? Search, int? PageNumber, int? PageSize);

    }
}
