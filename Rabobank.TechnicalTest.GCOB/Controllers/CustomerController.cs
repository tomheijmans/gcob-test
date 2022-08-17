using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rabobank.TechnicalTest.GCOB.Services;
using System;
using System.Threading.Tasks;

namespace Rabobank.TechnicalTest.GCOB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;

        public CustomerController(ILogger<CustomerController> logger
            , ICustomerService customerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }

        [HttpGet]
        [Route("{identifier}")]
        public async Task<Customer> Get(int identifier)
        {
            return await _customerService.GetAsync(identifier);
        }

        [HttpPost]
        public async Task<Customer> Post(Customer customer)
        {
            if (customer is null)
            {
                throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.BadRequest);
            }
            return await _customerService.InsertAsync(customer);
        }
    }
}
