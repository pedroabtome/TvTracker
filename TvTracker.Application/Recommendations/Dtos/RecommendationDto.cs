namespace TvTracker.Application.Recommendations.Dtos;

public class RecommendationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public double Score { get; set; }
    public List<string> Genres { get; set; } = new();
}