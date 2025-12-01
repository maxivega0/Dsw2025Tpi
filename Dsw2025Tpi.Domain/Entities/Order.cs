using Dsw2025Tpi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Order : EntityBase
    {
        
        public Order(Guid customerId /*string shippingAddress, string billingAddress, string? notes = null ) */)
        {
            CustomerId = customerId;
            //ShippingAddress = shippingAddress;
            //BillingAddress = billingAddress;
            //Notes = notes;
            IsActive = true;
        }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string? ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
        public string? Notes { get; set; }
        public decimal TotalAmount => OrderItems?.Sum(item => item.Subtotal) ?? 0;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public bool IsActive { get; set; }
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
