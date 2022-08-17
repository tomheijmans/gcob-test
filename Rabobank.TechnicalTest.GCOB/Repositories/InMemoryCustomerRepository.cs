using Microsoft.Extensions.Logging;
using Rabobank.TechnicalTest.GCOB.Dtos;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Rabobank.TechnicalTest.GCOB.Repositories
{
    public class InMemoryCustomerRepository : ICustomerRepository
    {
        private ConcurrentDictionary<int, CustomerDto> Customers { get; } = new ConcurrentDictionary<int, CustomerDto>();
        private ILogger _logger;

        public InMemoryCustomerRepository(ILogger<InMemoryCustomerRepository> logger)
        {
            _logger = logger;
        }

        public Task<int> GenerateIdentityAsync()
        {
            _logger.LogDebug("Generating Customer identity");
            return Task.Run(() =>
            {
                if (Customers.Count == 0) return 1;

                var x = Customers.Values.Max(c => c.Id);
                return ++x;
            });
        }

        public Task InsertAsync(CustomerDto customer)
        {
            if (Customers.ContainsKey(customer.Id))
            {
                throw new Exception(
                    $"Cannot insert customer with identity '{customer.Id}' " +
                    "as it already exists in the collection");
            }

            Customers.TryAdd(customer.Id, customer);
            _logger.LogDebug($"New customer inserted [ID:{customer.Id}]. " +
                          $"There are now {Customers.Count} legal entities in the store.");
            return Task.FromResult(customer);
        }

        public Task<CustomerDto> GetAsync(int identity)
        {
            _logger.LogDebug($"FindMany Customers with identity {identity}");

            if (!Customers.ContainsKey(identity)) throw new Exception(identity.ToString());
            _logger.LogDebug($"Found Customer with identity {identity}");
            return Task.FromResult(Customers[identity]);
        }

        public Task UpdateAsync(CustomerDto customer)
        {
            if (!Customers.ContainsKey(customer.Id))
            {
                throw new Exception(
                    $"Cannot update customer with identity '{customer.Id}' " +
                    "as it doesn't exist");
            }

            Customers[customer.Id] = customer;
            _logger.LogDebug($"New customer updated [ID:{customer.Id}].");

            return Task.FromResult(customer);
        }
    }
}
