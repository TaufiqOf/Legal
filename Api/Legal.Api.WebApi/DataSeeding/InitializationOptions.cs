namespace Legal.Api.WebApi.DataSeeding;

public sealed class InitializationOptions
{
    public string FilePath { get; set; } = null!;

    public bool DoUpdate { get; set; } = false;
}