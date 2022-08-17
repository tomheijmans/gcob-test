using System.Threading.Tasks;

namespace Rabobank.TechnicalTest.GCOB.Services
{
    public interface ICustomerService
    {
        Task<Customer> GetAsync(int identifier);
        Task<Customer> InsertAsync(Customer customer);
    }
}