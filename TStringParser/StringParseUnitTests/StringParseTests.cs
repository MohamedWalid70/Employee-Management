using FluentAssertions;
using Internship.StringParser;

namespace Internship.StringParseUnitTests
{
    public class StringParseTests
    {
        [Theory]
        [InlineData("Name,Age,Title\r\nEmployee1,27, Manager\r\nEmployee2,30, SeniorManager\r\nEmployee3,35, SeniorManager2")]
        public void Parse_WithEmployeeObjectString_ReturnsListOfEmployees(string input)
        {
            var expectedElement = new Employee() { Name = "Employee1", Age = 27, Title = " Manager" };
            var result = StringParser<Employee>.Parse(input);

            result.Should().BeOfType<List<Employee>>();
            result.Should().ContainEquivalentOf(expectedElement);
            result.Should().Contain(x => x.Age == 30);
            result.Should().NotBeNull();
            result.Count.Should().Be(3);
        }

        [Theory]
        [InlineData(" Name ,Age , Title\r\nEmployee1,27, Manager\r\nEmployee2,30, SeniorManager\r\nEmployee3,35, SeniorManager2")]
        public void Parse_WithTrailingAndLeadingSpacesInHeaderPropertyNames_ReturnsListOfEmployees(string input)
        {
            var expectedElement = new Employee() { Name = "Employee1", Age = 27, Title = " Manager" };
            var result = StringParser<Employee>.Parse(input);

            result.Should().BeOfType<List<Employee>>();
            result.Should().ContainEquivalentOf(expectedElement);
            result.Should().Contain(x => x.Age == 30);
            result.Should().NotBeNull();
            result.Count.Should().Be(3);
        }

        [Theory]
        [InlineData("Name,Address,TotalAccountBalance,Age\r\nEleanor Rigby,101 Penny Lane Liverpool UK,1250500.75,72\r\nMarcus Thorne,452 Maple Drive Evanston IL,84200.00,38\r\nSora Tanaka,address,12450.50,26\r\nDr. Aris Thorne,789 Oak Terrace Portland OR,450000.10,54")]
        public void Parse_WithPersonObjectStringExcludingOneProperty_ReturnsListOfPersons(string input)
        {
            var expectedElement = new Person() { Name = "Eleanor Rigby", Age = 72, TotalAccountBalance = 1250500.75M, Address = "101 Penny Lane Liverpool UK", IsMarried = false };
            var result = StringParser<Person>.Parse(input);

            result.Should().BeOfType<List<Person>>();
            result.Should().ContainEquivalentOf(expectedElement);
            result.Should().AllSatisfy(x => x.IsMarried = false);
            result.Should().NotBeNull();
            result.Count.Should().Be(4);
        }


        [Theory]
        [InlineData("Name,Title\r\nEmployee1,27, Manager\r\nEmployee2,30, SeniorManager\r\nEmployee3,35, SeniorManager2")]
        public void Parse_WithIncompleteHeaderBlueprint_ThrowsFormatException(string input)
        {
            var act = () => StringParser<Employee>.Parse(input);

            act.Should().Throw<FormatException>();
        }

        [Theory]
        [InlineData("Employee1,27, Manager\r\nEmployee2,30, SeniorManager\r\nEmployee3,35, SeniorManager2")]
        public void Parse_WithNoHeaderStringLine_ThrowsFormatException(string input)
        {
            var act = () => StringParser<Employee>.Parse(input);

            act.Should().Throw<FormatException>();
        }

        [Theory]
        [InlineData("Name,Age,Title\r\nEmployee1,twenty, Manager\r\nEmployee2,30, SeniorManager\r\nEmployee3,35, SeniorManager2")]
        public void Parse_WithIncompatibleDataTypesAndValues_ThrowsFormatException(string input)
        {
            var act = () => StringParser<Employee>.Parse(input);

            act.Should().Throw<FormatException>();
        }

        [Theory]
        [InlineData("Name,Age,FirstTime,Title\r\nnull,21,false,Manager\r\nEmployee2,30,true,SeniorManager\r\nEmployee3,35,true,SeniorManager2")]
        [InlineData("Name,Age,FirstTime,Title\r\nEmployee1,21,null,Manager\r\nEmployee2,30,true,SeniorManager\r\nEmployee3,35,true,SeniorManager2")]
        public void Parse_WithNullValuesForNonNullableProperties_ThrowsFormatException(string input)
        {
            var act = () => StringParser<Employee>.Parse(input);

            act.Should().Throw<FormatException>();
        }


        [Theory]
        [InlineData("Name,Age,Title\r\nEmployee1,null, Manager\r\nEmployee2,30, SeniorManager\r\nEmployee3,35, SeniorManager2")]
        public void Parse_WithNullValueForNullableValueTypeProperty_ReturnsListOfEmployees(string input)
        {
            var result = StringParser<Employee>.Parse(input);

            result.Should().Contain(x => x.Age == null);
        }

        [Theory]
        [InlineData("Name,Age,Title\r\nEmployee1,21, Manager\r\nEmployee2, 30 , SeniorManager\r\nEmployee3,35, SeniorManager2")]
        public void Parse_WithLeadingAndTrailingSpaceForValueTypeProperty_ReturnsListOfEmployees(string input)
        {
            var result = StringParser<Employee>.Parse(input);

            result.Should().Contain(x => x.Age == 30);
        }

        [Theory]
        [InlineData("Name,Address,TotalAccountBalance,Age\r\nEleanor Rigby,101 Penny Lane Liverpool UK,1250500.75,72\r\nMarcus Thorne,452 Maple Drive Evanston IL,84200.00,38\r\nSora Tanaka, null ,12450.50,26\r\nDr. Aris Thorne,789 Oak Terrace Portland OR,450000.10,54")]
        public void Parse_WithNullValueForNullableReferenceTypeProperty_ReturnsListOfPersons(string input)
        {
            var result = StringParser<Person>.Parse(input);

            result.Should().Contain(x => x.Address == null);
        }


        [Theory]
        [InlineData("")]
        [InlineData(" \r\n")]
        public void Parse_WithWhiteSpaceAndEmptyInputs_ThrowsArgumentException(string input)
        {
            var act = () => StringParser<Employee>.Parse(input);

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        public void Parse_WithNullInput_ThrowsArgumentNullException(string input)
        {
            var act = () => StringParser<Employee>.Parse(input);

            act.Should().Throw<ArgumentNullException>();
        }

    }
}