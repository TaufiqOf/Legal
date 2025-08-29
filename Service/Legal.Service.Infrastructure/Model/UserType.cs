namespace Legal.Service.Infrastructure.Model;

public class UserType
{
    public string Name { get; set; }

    public string NormalizedName => Name.ToUpper();
}