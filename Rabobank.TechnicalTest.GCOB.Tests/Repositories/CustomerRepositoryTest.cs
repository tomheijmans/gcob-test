using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabobank.TechnicalTest.GCOB.Dtos;
using Rabobank.TechnicalTest.GCOB.Repositories;
using System;
using System.Threading.Tasks;

namespace Rabobank.TechnicalTest.GCOB.Tests.Services
{
    [TestClass]
    public class CustomerRepositoryTest
    {
        private InMemoryCustomerRepository subjectUnderTest;

        [TestInitialize]
        public void Initialize()
        {
            ILogger<InMemoryCustomerRepository> loggerMock = A.Fake<ILogger<InMemoryCustomerRepository>>();
            subjectUnderTest = new InMemoryCustomerRepository(loggerMock);
        }

        [TestMethod]
        public async Task GivenHaveACustomer_AndIGetTheCustomerFromTheDB_ThenTheCustomerIsRetrieved()
        {
            // Arrange
            CustomerDto expectedResult = new CustomerDto { Id = 2 };
            await subjectUnderTest.InsertAsync(new CustomerDto { Id = 0 });
            await subjectUnderTest.InsertAsync(new CustomerDto { Id = 1 });
            await subjectUnderTest.InsertAsync(expectedResult);

            // Act
            CustomerDto actualResult = await subjectUnderTest.GetAsync(2);

            // Assert
            actualResult.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task GivenDoesntHaveExpectedCustomer_AndIGetTheCustomerFromTheDB_ThenExceptionIsThrown()
        {
            // Arrange
            // Act
            Func<Task<CustomerDto>> act = () => subjectUnderTest.GetAsync(2);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("2");
        }

        [TestMethod]
        public async Task GivenInsertACustomer_AndICallAServiceToGetTheCustomer_ThenTheCustomerIsInserted_AndTheCustomerIsReturned()
        {
            // Arrange
            var customerToInsert = new CustomerDto { Id = 2, AddressId = 5, FirstName = "Chuck", LastName = "Norris" };

            // Act
            await subjectUnderTest.InsertAsync(customerToInsert);
            var customerGetResult = await subjectUnderTest.GetAsync(2);

            // Assert
            customerGetResult.Should().BeEquivalentTo(customerToInsert);
        }

        [TestMethod]
        public async Task GivenCustomerWithDuplicateKey_WhenInsertAsync_ThenThrowsException()
        {
            // Arrange
            await subjectUnderTest.InsertAsync(new CustomerDto { Id = 10 });

            // Act
            Func<Task> act = () => subjectUnderTest.InsertAsync(new CustomerDto { Id = 10 });

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Cannot insert customer with identity '10' as it already exists in the collection");
        }
    }
}
