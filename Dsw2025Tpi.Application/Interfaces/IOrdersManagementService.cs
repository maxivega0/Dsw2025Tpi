using Dsw2025Tpi.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Interfaces
{
    public interface IOrdersManagementService
    {
        Task<OrderModel.OrderResponse?> GetOrderById(Guid id);
        Task<OrderModel.CreateResponse> CreateOrder(OrderModel.OrderRequest request);
        Task<OrderModel.PaginationResponse?> GetOrders(OrderModel.FilterOrderRequest request);
        Task<OrderModel.OrderStatusResponse> UpdateOrderStatus(Guid id, OrderModel.OrderStatusRequest request);
    }
}
