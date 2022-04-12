using Rabobank.TechnicalTest.GCOB.Dtos;
using System.Threading.Tasks;

namespace Rabobank.TechnicalTest.GCOB.Repositories
{
    public interface ICustomerRepository
    {
        Task<int> GenerateIdentityAsync();
        Task InsertAsync(CustomerDto customer);
        Task<CustomerDto> GetAsync(int identity);
        Task UpdateAsync(CustomerDto customer);
    }
}
