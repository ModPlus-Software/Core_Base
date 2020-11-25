namespace Tests
{
    using System.Linq;
    using FluentAssertions;
    using mpBaseInt;
    using Xunit;

    public abstract class BaseDbSectionTests<T> 
        where T: IDbSection, new()
    {
        [Fact]
        public void Documents_GetCount_MustBeNotEmpty()
        {
            new T().Documents.Count().Should().BePositive();
        }

        [Fact]
        public void DocumentNames_GetCount_MustBeNotEmpty()
        {
            new T().GetDocumentNames().Count.Should().BePositive();
        }
    }
}
