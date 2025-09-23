using FluentAssertions;
using TvTracker.Application.Shows;

namespace TvTracker.Tests.Unit;

public class ValidationTests
{
    [Fact]
    public void ListShowsQuery_Defaults_AreValid()
    {
        var v = new ListShowsQueryValidator();
        var q = new ListShowsQuery();
        var result = v.Validate(q);
        result.IsValid.Should().BeTrue();
    }
}
