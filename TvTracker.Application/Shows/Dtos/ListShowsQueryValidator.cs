using FluentValidation;

namespace TvTracker.Application.Shows;

public class ListShowsQueryValidator : AbstractValidator<ListShowsQuery>
{
    public ListShowsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Sort)
            .Must(s => s is null || new[] { "Name", "-Name", "Premiered", "-Premiered", "LastUpdated", "-LastUpdated" }.Contains(s))
            .WithMessage("Sort inválido. Ex.: Name, -Name, Premiered, -Premiered, LastUpdated, -LastUpdated");
    }
}
