using Microsoft.Extensions.Logging;
using Rabobank.TechnicalTest.GCOB.Dtos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rabobank.TechnicalTest.GCOB.Repositories
{
    public class InMemoryCountryRepository : ICountryRepository
    {
        private ConcurrentDictionary<int, CountryDto> Countries { get; } = new ConcurrentDictionary<int, CountryDto>();
        private ILogger _logger;

        public InMemoryCountryRepository(ILogger logger)
        {
            _logger = logger;
            Countries.TryAdd(1, new CountryDto { Id = 1, Name = "Netherlands" });
            Countries.TryAdd(1, new CountryDto { Id = 2, Name = "Poland" });
            Countries.TryAdd(1, new CountryDto { Id = 3, Name = "Ireland" });
            Countries.TryAdd(1, new CountryDto { Id = 4, Name = "South Afrcia" });
            Countries.TryAdd(1, new CountryDto { Id = 5, Name = "India" });
        }

        public Task<CountryDto> GetAsync(int identity)
        {
            _logger.LogDebug($"Get Country with identity {identity}");

            if (!Countries.ContainsKey(identity)) throw new Exception(identity.ToString());
            _logger.LogDebug($"Found Country with identity {identity}");
            return Task.FromResult(Countries[identity]);
        }

        public Task<IEnumerable<CountryDto>> GetAllAsync()
        {
            _logger.LogDebug($"Get all Countries");

            return Task.FromResult(Countries.Select(x => x.Value));
        }

    }

}
