using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        public readonly ILogger<ICustomersManagementService> _logger;

        public CustomersManagementService(IRepository customerRepository,
            ILogger<ICustomersManagementService> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CustomerModel.CustomerResponse>> GetCustomers()
        {
            _logger.LogInformation("Consulta de customers");
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
            _logger.LogInformation("Consulta de customer por id: {id}",id);
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
            _logger.LogInformation("Creacion de customer");

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
            _logger.LogInformation("Consulta de customer por UserId: {userId}", userId);

            return await _customerRepository.First<Customer>(c => c.UserId == userId);
        }


    }
}