using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dsw2025Tpi.Application.Dtos.ProductModel;

namespace Dsw2025Tpi.Application.Dtos
{
    public static class OrderModel
    {
        public record OrderRequest(string ClientUsername, string? ShippingAddress, string? BillingAddress, ICollection<OrderItemModel.OrderItemRequest> OrderItems);
        public record CreateResponse(Guid Id, Guid CustomerId/*string? ShippingAddress, string? BillingAddress,*/, decimal TotalAmount, DateTime? Date, OrderStatus Status, ICollection<OrderItem> OrderItems);
        public record OrderResponse(Guid Id, Guid CustomerId, string CustomerName /*string? ShippingAddress, string? BillingAddress,*/, decimal TotalAmount, DateTime? Date, string Status, bool IsActive, ICollection<OrderItemModel.Response> OrderItems);
        public record PaginationResponse(List<OrderResponse> Orders, int Total);
        public record FilterOrderRequest(string? Status, string? Search, int PageNumber, int? PageSize);
        public record OrderStatusRequest(OrderStatus Status);
        public record OrderStatusResponse(Guid Id, Guid CustomerId, string Status);
    }
}
