using Dsw2025Tpi.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Interfaces
{
    public interface IProductsManagementService
    {
        Task<ProductModel.ProductResponse?> GetProductById(Guid id);
        Task<ProductModel.PaginationResponse> GetAuthProducts(ProductModel.FilterProductRequest filter);
        Task<ProductModel.PaginationResponse> GetProducts(ProductModel.FilterProductRequest filter);
        Task<ProductModel.ProductResponse> AddProduct(ProductModel.ProductRequest request);
        Task<ProductModel.ProductResponse> UpdateProduct(Guid id, ProductModel.ProductRequest request);
        Task<ProductModel.ProductResponse> DisableProduct(Guid id);
    }
}
