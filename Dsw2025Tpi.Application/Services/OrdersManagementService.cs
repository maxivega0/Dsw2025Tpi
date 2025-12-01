using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Services
{
    public class OrdersManagementService : IOrdersManagementService
    {
        public readonly IRepository _orderRepository;
        private readonly ICustomersManagementService _customersManagementService;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersManagementService(IRepository orderRepository, 
            ICustomersManagementService customersManagementService, 
            UserManager<IdentityUser> userManager)
        {
            _orderRepository = orderRepository;
            _customersManagementService = customersManagementService;
            _userManager = userManager;
        }

        public async Task<OrderModel.PaginationResponse?> GetOrders(OrderModel.FilterOrderRequest request)
        {
            var isActive = request.Status == "enabled"
                ? (bool?)true
                : request.Status == "disabled"
                ? (bool?)false
                : null;

            var orders = await _orderRepository.GetFiltered<Order>(o => (
                (isActive == null || o.IsActive == isActive)), $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}");

            if (orders == null)
                return null;

            var customers = await _customersManagementService.GetCustomers();

            // Combinar datos + filtrar en memoria
            var filtered = orders
                .Select(o =>
                {
                    var customer = customers.FirstOrDefault(c => c.Id == o.CustomerId);

                    return new
                    {
                        Order = o,
                        CustomerName = customer?.Name ?? ""
                    };
                })
                .Where(x =>
                    string.IsNullOrEmpty(request.Search)
                    || x.CustomerName.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                );

            var finalList = filtered
             .Select(x => new OrderModel.OrderResponse(
                 x.Order.Id,
                 x.Order.CustomerId,
                 x.CustomerName,
                 x.Order.TotalAmount,
                 x.Order.Date,
                 x.Order.Status.ToString(),
                 x.Order.IsActive,
                 x.Order.OrderItems.Select(item => new OrderItemModel.Response(
                     item.ProductId,
                     item.Product.Name,
                     item.Quantity,
                     item.UnitPrice,
                     item.Subtotal))
                 .ToList()
             ));

            var paged = finalList.OrderBy(o => o.Id)
            .Skip((request.PageNumber - 1) * (request.PageSize ?? 20))
            .Take(request.PageSize ?? 20)
            .ToList();

            return new OrderModel.PaginationResponse(paged, finalList.Count());
        }

        public async Task<OrderModel.OrderResponse?> GetOrderById(Guid id)
        {
            var order = await _orderRepository.GetById<Order>(id, $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}");
            var customer = await _customersManagementService.GetCustomerById(order.CustomerId);
            
            return order != null ?
                new OrderModel.OrderResponse(
                    order.Id,
                    order.CustomerId,
                    customer?.Name,
                    //order.ShippingAddress,
                    //order.BillingAddress,
                    order.TotalAmount,
                    order.Date,
                    order.Status.ToString(),
                    order.IsActive,
                    order.OrderItems.Select(item => new OrderItemModel.Response(
                        item.ProductId,
                        item.Product.Name,
                        item.Quantity,
                        item.UnitPrice,
                        item.Subtotal
                    )).ToList()
                ) :
                null;
        }

        public async Task<OrderModel.CreateResponse?> CreateOrder(OrderModel.OrderRequest request)
        {
            //if (string.IsNullOrWhiteSpace(request.ShippingAddress)) throw new ArgumentException("La dirección de envío no puede estar vacía.");
            //if (string.IsNullOrWhiteSpace(request.BillingAddress)) throw new ArgumentException("La dirección de facturación no puede estar vacía.");
            if (request.OrderItems == null || !request.OrderItems.Any()) throw new ArgumentException("La orden debe contener al menos un producto.");

            var user = await _userManager.FindByNameAsync(request.ClientUsername);

            var customer = await _customersManagementService.GetCustomerByUserId(user.Id);
            if (customer == null) throw new EntityNotFoundException($"No existe un cliente con el ID {customer.Id}");

            var duplicateProductIds = request.OrderItems.GroupBy(x => x.ProductId).Where(g => g.Count() > 1).Select(g => g.Key);
            if (duplicateProductIds.Any()) throw new DuplicatedEntityException("La orden contiene productos duplicados");

            var productIds = request.OrderItems.Select(x => x.ProductId).Distinct().ToList();
            var products = await _orderRepository.GetFiltered<Product>(p => productIds.Contains(p.Id));

            foreach (var item in request.OrderItems)
            {
                var product = products?.FirstOrDefault(p => p.Id == item.ProductId);

                if (product is null) throw new EntityNotFoundException($"Producto con ID {item.ProductId} no fue encontrado.");

                if (item.Quantity <= 0) throw new ArgumentException($"La cantidad para '{product.Name}' debe ser mayor que 0.");

                if (item.Quantity > product.StockQuantity) throw new ArgumentException($"No hay stock suficiente para '{product.Name}'. Solicitado: {item.Quantity}, disponible: {product.StockQuantity}");

                if (item.UnitPrice != product.CurrentUnitPrice) throw new ArgumentException($"El precio de '{product.Name}' no coincide con el actual. Esperado: {product.CurrentUnitPrice}, recibido: {item.UnitPrice}");

            }

            foreach (var item in request.OrderItems)
            {
                var product = products?.FirstOrDefault(p => p.Id == item.ProductId);
                product.StockQuantity -= item.Quantity;
                await _orderRepository.Update(product);
            }

            var order = new Order(customer.Id/*, request.ShippingAddress, request.BillingAddress*/);
            order.OrderItems = request.OrderItems.Select(item => new OrderItem(item.ProductId, item.Quantity, item.UnitPrice)).ToList();
            var createdOrder = await _orderRepository.Add(order);
            return new OrderModel.CreateResponse(
                createdOrder.Id,
                createdOrder.CustomerId,
                //createdOrder.ShippingAddress,
                //createdOrder.BillingAddress,
                createdOrder.TotalAmount,
                createdOrder.Date,
                createdOrder.Status,
                createdOrder.OrderItems);
        }

    }
}
