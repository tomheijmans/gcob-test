using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabobank.TechnicalTest.GCOB.Controllers;
using Rabobank.TechnicalTest.GCOB.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Rabobank.TechnicalTest.GCOB.Tests.Services
{
    [TestClass]
    public class CustomerControllerTest
    {
        private ICustomerService customerServiceMock;
        private CustomerController subjectUnderTest;

        [TestInitialize]
        public void Initialize()
        {
            customerServiceMock = A.Fake<ICustomerService>();
            subjectUnderTest = new CustomerController(A.Fake<ILogger<CustomerController>>(), customerServiceMock);
        }

        [TestMethod]
        public async Task GivenHaveACustomer_AndICallAServiceToGetTheCustomer_ThenTheCustomerIsReturned()
        {
            // Arrange
            Customer expectedResult = new Customer
            {
                City = "Amsterdam",
                Country = "Nederland",
                FullName = "Chuck Norris",
                Id = 2,
                Postcode = "1234 AB",
                Street = "Leidseplein",
            };
            A.CallTo(() => customerServiceMock.GetAsync(5))
                .Returns(expectedResult);

            // Act
            Customer actualResult = await subjectUnderTest.Get(5);

            // Assert
            actualResult.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task GivenNullCustomer_WhenPost_ThenHttpResponseExceptionIsThrownIndicatingABadRequest()
        {
            // Arrange
            
            // Act
            Func<Task<Customer>> act = () => subjectUnderTest.Post(null);

            // Assert
            (await act.Should().ThrowAsync<HttpResponseException>())
                .And.Response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task GivenCustomer_WhenPost_ThenCustomerIsInsertedInCustomerService()
        {
            // Arrange
            var customerPassedIn = new Customer();

            // Act
            await subjectUnderTest.Post(customerPassedIn);

            // Assert
            A.CallTo(() => customerServiceMock.InsertAsync(customerPassedIn))
                .MustHaveHappenedOnceExactly();
        }
    }
}