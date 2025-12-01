using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Services
{
    public class CustomersManagementService : ICustomersManagementService
    {
        public readonly IRepository _customerRepository;

        public CustomersManagementService(IRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<CustomerModel.CustomerResponse>> GetCustomers()
        {
            var customers = await _customerRepository.GetAll<Customer>();

            return customers.Select(c => new CustomerModel.CustomerResponse(
                     c.Id,
                     c.Name,
                     c.Email,
                     c.UserId
                 ));
        }

        public async Task<CustomerModel.CustomerResponse?> GetCustomerById(Guid id)
        {
            var customer = await _customerRepository.GetById<Customer>(id);
            return customer != null ?
                new CustomerModel.CustomerResponse(
                    customer.Id,
                    customer.Name,
                    customer.Email,
                    customer.UserId
                ) :
                null;
        }

        public async Task<CustomerModel.CustomerResponse> CreateCustomer(CustomerModel.CreateCustomerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("El nombre del cliente no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("El email del cliente no puede estar vacío.");

            var newCustomer = new Customer(
                request.Name,
                request.Email
            );
            newCustomer.UserId = request.UserId;

            var createdCustomer = await _customerRepository.Add<Customer>(newCustomer);

            return new CustomerModel.CustomerResponse(
                createdCustomer.Id,
                createdCustomer.Name,
                createdCustomer.Email,
                createdCustomer.UserId
            );
        }

        public async Task<Customer?> GetCustomerByUserId(string userId)
        {
            return await _customerRepository.First<Customer>(c => c.UserId == userId);
        }


    }
}