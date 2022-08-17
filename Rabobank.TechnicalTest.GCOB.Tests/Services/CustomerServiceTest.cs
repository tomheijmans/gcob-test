using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabobank.TechnicalTest.GCOB.Dtos;
using Rabobank.TechnicalTest.GCOB.Repositories;
using Rabobank.TechnicalTest.GCOB.Services;
using System.Threading.Tasks;

namespace Rabobank.TechnicalTest.GCOB.Tests.Services
{
    [TestClass]
    public class CustomerServiceTest
    {
        private ICustomerRepository customerRepositoryMock;
        private IAddressRepository addressRepositoryMock;
        private ICountryRepository countryRepositoryMock;
        private CustomerService subjectUnderTest;

        [TestInitialize]
        public void Initialize()
        {
            customerRepositoryMock = A.Fake<ICustomerRepository>();
            addressRepositoryMock = A.Fake<IAddressRepository>();
            countryRepositoryMock = A.Fake<ICountryRepository>();

            subjectUnderTest = new CustomerService(customerRepositoryMock, addressRepositoryMock, countryRepositoryMock);
        }

        [TestMethod]
        public async Task GivenHaveACustomer_AndICallAServiceToGetTheCustomer_ThenTheCustomerIsReturned()
        {
            // Arrange 
            A.CallTo(() => customerRepositoryMock.GetAsync(2))
                .Returns(new CustomerDto
                {
                    Id = 2,
                    AddressId = 5,
                    FirstName = "Chuck",
                    LastName = "Norris"
                });

            A.CallTo(() => addressRepositoryMock.GetAsync(5))
                .Returns(new AddressDto
                {
                    Id = 5,
                    City = "Amsterdam",
                    CountryId = 31,
                    Postcode = "1234 AB",
                    Street = "Leidseplein"
                });

            A.CallTo(() => countryRepositoryMock.GetAsync(31))
               .Returns(new CountryDto
               {
                   Id = 31,
                   Name = "Nederland"
               });

            // Act
            Customer actualResult = await subjectUnderTest.GetAsync(2);

            // Assert
            actualResult.Should().BeEquivalentTo(new Customer
            {
                City = "Amsterdam",
                Country = "Nederland",
                FullName = "Chuck Norris",
                Id = 2,
                Postcode = "1234 AB",
                Street = "Leidseplein",
            });
        }

        [TestMethod]
        public async Task GivenCustomerWithoutAddress_WhenGetAsync_ThenCustomerIsReturned()
        {
            // Arrange 
            A.CallTo(() => customerRepositoryMock.GetAsync(2))
                .Returns(new CustomerDto
                {
                    Id = 2,
                    AddressId = 5,
                    FirstName = "Chuck",
                    LastName = "Norris"
                });

            A.CallTo(() => addressRepositoryMock.GetAsync(5)).Returns((AddressDto)null);

            // Act
            Customer actualResult = await subjectUnderTest.GetAsync(2);

            // Assert
            actualResult.Should().BeEquivalentTo(new Customer
            {
                City = null,
                Country = null,
                FullName = "Chuck Norris",
                Id = 2,
                Postcode = null,
                Street = null,
            });
        }


        [TestMethod]
        public async Task GivenNoCustomerForIdentifier_WhenGetAsync_ThenReturnNull()
        {
            // Arrange
            A.CallTo(() => customerRepositoryMock.GetAsync(2))
               .Returns((CustomerDto)null);

            // Act
            Customer actualResult = await subjectUnderTest.GetAsync(2);

            // assert
            actualResult.Should().BeNull();
        }

        [TestMethod]
        public async Task GivenInsertACustomer_AndICallAServiceToGetTheCustomer_ThenTheCustomerIsInserted_AndTheCustomerIsReturned()
        {
            // Given the title it should be a component test (or component integration test) where the repository is not a mock. 
            // Don't add save and return behaviour to a mock. Implemented this test is on the repository tests.
        }

        [TestMethod]
        public async Task GivenInsertACustomer_ThenTheCustomerIsInserted_AndTheCustomerIsReturned()
        {
            // Arrange
            A.CallTo(() => customerRepositoryMock.GenerateIdentityAsync())
                .Returns(7);
            A.CallTo(() => addressRepositoryMock.GenerateIdentityAsync())
                .Returns(3);
            A.CallTo(() => countryRepositoryMock.GetAllAsync())
                .Returns(new[] {
                    new CountryDto { Id = 31, Name = "Nederland"},
                    new CountryDto { Id = 13, Name= "Hawai" },
                });

            // Act
            Customer result = await subjectUnderTest.InsertAsync(new Customer
            {
                City = "Amsterdam",
                Country = "Nederland",
                FullName = "Chuck Norris",
                Postcode = "1234 AB",
                Street = "Leidseplein",
            });

            // Assert
            result.Id.Should().Be(7);
            A.CallTo(() => customerRepositoryMock.InsertAsync(
                A<CustomerDto>.That.Matches(actual =>
                    actual.Id == 7 &&
                    actual.AddressId == 3 &&
                    actual.FirstName == "Chuck" &&
                    actual.LastName == "Norris"
                )))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => addressRepositoryMock.InsertAsync(
                A<AddressDto>.That.Matches(actual =>
                    actual.City == "Amsterdam" &&
                    actual.CountryId == 31 &&
                    actual.Id == 3 &&
                    actual.Postcode == "1234 AB" &&
                    actual.Street == "Leidseplein"
                )))
                .MustHaveHappenedOnceExactly();
        }
    }
}