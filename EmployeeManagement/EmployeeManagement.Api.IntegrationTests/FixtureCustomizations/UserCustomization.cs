using AutoFixture;
using AutoFixture.Kernel;
using Bogus;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Internship.EmployeeManagement.Api.IntegrationTests.FixtureCustomizations
{
    internal class UserCustomization(Faker faker) : ICustomization
    {
        private readonly Faker _faker = faker;

        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Insert(0, new IgnoreRegexBuilder(_faker));
        }

        private class IgnoreRegexBuilder(Faker faker) : ISpecimenBuilder
        {
            private readonly Faker _faker = faker;

            public object Create(object request, ISpecimenContext context)
            {
                if (request is RegularExpressionRequest)
                {
                    return "123456Aa$";
                }
                else if (request is PropertyInfo prop && prop.Name.Equals("Email") )
                {
                  
                    return _faker.Internet.Email();
                    
                }
                return new NoSpecimen();
            }
        }
    }
}

