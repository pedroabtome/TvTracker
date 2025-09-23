namespace TvTracker.Tests;

public static class Json
{
    public static async Task<T> ReadAs<T>(this HttpResponseMessage res)
    {
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadFromJsonAsync<T>();
        return body!;
    }
}
