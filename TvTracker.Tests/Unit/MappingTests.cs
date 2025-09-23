using AutoMapper;
using TvTracker.Application.Shows.Mapping;

namespace TvTracker.Tests.Unit;

public class MappingTests
{
    [Fact]
    public void AutoMapper_Config_IsValid()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<ShowMappingProfile>());
        cfg.AssertConfigurationIsValid();
    }
}
