using Rabobank.TechnicalTest.GCOB.Dtos;
using Rabobank.TechnicalTest.GCOB.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rabobank.TechnicalTest.GCOB.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IAddressRepository addressRepository;
        private readonly ICountryRepository countryRepository;

        public CustomerService(ICustomerRepository customerRepository
            , IAddressRepository addressRepository
            , ICountryRepository countryRepository)
        {
            this.customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            this.addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            this.countryRepository = countryRepository ?? throw new ArgumentNullException(nameof(countryRepository));
        }

        public async Task<Customer> GetAsync(int identifier)
        {
            CustomerDto customerDto = await customerRepository.GetAsync(identifier);

            if (customerDto is null)
            {
                return null;
            }

            return await ConstructCustomerFromDtoAsync(customerDto);
        }

        public async Task<Customer> InsertAsync(Customer customer)
        {
            if (customer is null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            IEnumerable<CountryDto> allCountries = await countryRepository.GetAllAsync();

            AddressDto addressDto = new AddressDto
            {
                Id = await addressRepository.GenerateIdentityAsync(),
                City = customer.City,
                Postcode = customer.Postcode,
                Street = customer.Street,
                CountryId = allCountries.Single(x => x.Name == customer.Country).Id  // Expect it does exist for now
            };

            await addressRepository.InsertAsync(addressDto);

            CustomerDto customerDto = new CustomerDto
            {
                Id = await customerRepository.GenerateIdentityAsync(),
                AddressId = addressDto.Id,
                FirstName = customer.FullName.Split(' ').FirstOrDefault(), // Assume everything before the first whitespace is the firstname
                LastName = string.Join(' ', customer.FullName.Split(' ').Skip(1)), // Assume everything after the first whitespace is the lastname
            };

            await customerRepository.InsertAsync(customerDto);

            customer.Id = customerDto.Id;
            return customer;
        }

        private async Task<Customer> ConstructCustomerFromDtoAsync(CustomerDto customerDto)
        {
            if (customerDto is null)
            {
                throw new ArgumentNullException(nameof(customerDto));
            }

            AddressDto addressDto = await addressRepository.GetAsync(customerDto.AddressId);
            CountryDto countryDto = null;

            if (addressDto != null)
            {
                countryDto = await countryRepository.GetAsync(addressDto.CountryId);
            }

            return MapToCustomer(customerDto, addressDto, countryDto);
        }

        private static Customer MapToCustomer(CustomerDto customerDto, AddressDto addressDto, CountryDto countryDto)
        {
            if (customerDto is null)
            {
                throw new ArgumentNullException(nameof(customerDto));
            }

            return new Customer
            {
                City = addressDto?.City,
                Country = countryDto?.Name,
                FullName = $"{customerDto.FirstName} {customerDto.LastName}",
                Id = customerDto.Id,
                Postcode = addressDto?.Postcode,
                Street = addressDto?.Street,
            };
        }
    }
}
