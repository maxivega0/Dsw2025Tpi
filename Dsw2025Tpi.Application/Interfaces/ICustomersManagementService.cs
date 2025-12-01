using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Interfaces
{
    public interface ICustomersManagementService
    {
        Task<CustomerModel.CustomerResponse?> GetCustomerById(Guid id);
        Task<CustomerModel.CustomerResponse> CreateCustomer(CustomerModel.CreateCustomerRequest request);
        Task<Customer?> GetCustomerByUserId(string userId);
    }
}
