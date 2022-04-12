using Rabobank.TechnicalTest.GCOB.Dtos;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Rabobank.TechnicalTest.GCOB.Repositories
{
    public class InMemoryAddressRepository : IAddressRepository
    {
        private ConcurrentDictionary<int, AddressDto> Addresses { get; } = new ConcurrentDictionary<int, AddressDto>();

        public Task<AddressDto> GetAsync(int identity)
        {
            throw new System.NotImplementedException();
        }
    }
}
