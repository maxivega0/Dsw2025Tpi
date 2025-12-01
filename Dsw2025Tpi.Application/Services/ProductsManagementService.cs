using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Helpers;
using Dsw2025Tpi.Application.Interfaces;

namespace Dsw2025Tpi.Application.Services
{
    public class ProductsManagementService : IProductsManagementService
    {
        private readonly IRepository _productRepository;
        public ProductsManagementService(IRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductModel.ProductResponse?> GetProductById(Guid id)
        {
            var product = await _productRepository.GetById<Product>(id);

            return product != null && product.IsActive ?
                new ProductModel.ProductResponse(
                    product.Id, 
                    product.Sku, 
                    product.InternalCode, 
                    product.Name, 
                    product.Description, 
                    product.CurrentUnitPrice, 
                    product.StockQuantity,
                    product.IsActive
                    ) :
                null;
        }

        public async Task<ProductModel.PaginationResponse> GetProducts(ProductModel.FilterProductRequest request)
        {
            var isActive = true;

            var activeProducts = await _productRepository.GetFiltered<Product>(p => (
                (isActive == null || p.IsActive == isActive)
                && (string.IsNullOrEmpty(request.Search) || p.Name.Contains(request.Search))
                ));


            var products = activeProducts
                 .Select(p => new ProductModel.ProductResponse(
                     p.Id,
                     p.Sku,
                     p.InternalCode,
                     p.Name,
                     p.Description,
                     p.CurrentUnitPrice,
                     p.StockQuantity,
                     p.IsActive
                 ))
                 .OrderBy(p => p.Sku)
                 .Skip((request.PageNumber - 1) * request.PageSize ?? 0)
                 .Take(request.PageSize ?? activeProducts.Count());


            return new ProductModel.PaginationResponse(products.ToList(), activeProducts.Count());

        }

        public async Task<ProductModel.PaginationResponse> GetAuthProducts(ProductModel.FilterProductRequest request)
        {
            var isActive = request.Status == "enabled" 
                ? (bool?)true 
                : request.Status == "disabled" 
                ? (bool?)false 
                : null;

            var activeProducts = await _productRepository.GetFiltered<Product>(p =>(
                (isActive == null || p.IsActive == isActive)
                && (string.IsNullOrEmpty(request.Search) || p.Name.Contains(request.Search))
                ));

            var products = activeProducts
                 .Select(p => new ProductModel.ProductResponse(
                     p.Id,
                     p.Sku,
                     p.InternalCode,
                     p.Name,
                     p.Description,
                     p.CurrentUnitPrice,
                     p.StockQuantity,
                     p.IsActive
                 ))
                 .OrderBy(p => p.Sku)
                 .Skip((request.PageNumber - 1) * request.PageSize ?? 0)
                 .Take(request.PageSize ?? activeProducts.Count());
                 

            return new ProductModel.PaginationResponse(products.ToList(), activeProducts.Count());

        }

        public async Task<ProductModel.ProductResponse> AddProduct(ProductModel.ProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Sku)) throw new ArgumentException("El Sku no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(request.InternalCode)) throw new ArgumentException("El código interno no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("El Name no puede estar vacío.");
            if (request.CurrentUnitPrice <= 0) throw new ArgumentException("El precio debe ser mayor que 0.");
            if (request.StockQuantity < 0) throw new ArgumentException("La cantidad de stock no puede ser negativa.");

            var exist = await _productRepository.First<Product>(p => p.Sku == request.Sku || p.InternalCode == request.InternalCode);
            if (exist != null) throw new DuplicatedEntityException("Ya existe un producto con el mismo SKU o código interno");
            var product = new Product(request.Sku, request.InternalCode, request.Name, request.Description, request.CurrentUnitPrice, request.StockQuantity);
            await _productRepository.Add(product);
            return new ProductModel.ProductResponse(
                product.Id, 
                product.Sku, 
                product.InternalCode, 
                product.Name, 
                product.Description, 
                product.CurrentUnitPrice, 
                product.StockQuantity,
                product.IsActive
                );
        }

        public async Task<ProductModel.ProductResponse> UpdateProduct(Guid id, ProductModel.ProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Sku)) throw new ArgumentException("El Sku no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(request.InternalCode)) throw new ArgumentException("El código interno no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("El Name no puede estar vacío.");
            if (request.CurrentUnitPrice <= 0) throw new ArgumentException("El precio debe ser mayor que 0.");
            if (request.StockQuantity < 0) throw new ArgumentException("La cantidad de stock no puede ser negativa.");

            var product = await _productRepository.GetById<Product>(id);
            if (product == null)  throw new EntityNotFoundException("No existe un producto con el Id especificado");
            
            if (ProductComparer.HasChanges(product, request))
            {
                product.Sku = request.Sku;
                product.InternalCode = request.InternalCode;
                product.Name = request.Name;
                product.Description = request.Description;
                product.CurrentUnitPrice = request.CurrentUnitPrice;
                product.StockQuantity = request.StockQuantity;
                await _productRepository.Update(product);
                return new ProductModel.ProductResponse(
                    product.Id, 
                    product.Sku, 
                    product.InternalCode, 
                    product.Name, 
                    product.Description, 
                    product.CurrentUnitPrice, 
                    product.StockQuantity,
                    product.IsActive
                    );
            }
            throw new ArgumentException("No se han modificado los valores del producto");

        }

        public async Task<ProductModel.ProductResponse> DisableProduct(Guid id)
        {
            var product = await _productRepository.GetById<Product>(id);
            if (product == null || !product.IsActive) throw new EntityNotFoundException("No existe un producto con el ID especificado");
            product.IsActive = false;
            await _productRepository.Update(product);
            return new ProductModel.ProductResponse(
                product.Id, 
                product.Sku,
                product.InternalCode, 
                product.Name, 
                product.Description, 
                product.CurrentUnitPrice, 
                product.StockQuantity,
                product.IsActive
                );

        }
    }
}
