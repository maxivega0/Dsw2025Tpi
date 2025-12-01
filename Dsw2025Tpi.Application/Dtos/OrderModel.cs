using Dsw2025Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public static class OrderModel
    {
        public record OrderRequest(string ClientUsername, string? ShippingAddress, string? BillingAddress, ICollection<OrderItemModel.OrderItemRequest> OrderItems);
        public record AddResponse(Guid Id, Guid CustomerId, /*string? ShippingAddress, string? BillingAddress,*/ decimal TotalAmount, DateTime? Date, ICollection<OrderItem> OrderItems);
        public record GetResponse(Guid Id, Guid CustomerId, /*string? ShippingAddress, string? BillingAddress,*/ decimal TotalAmount, DateTime? Date, ICollection<OrderItemModel.Response> OrderItems);

    }
}
