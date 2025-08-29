using Legal.Service.Infrastructure.Interface;

namespace Legal.Service.Infrastructure.Model;

public abstract class BaseModel : IBaseModel
{
    public string Id { get; set; }

    public DateTimeOffset CreateTime { get; set; }

    public DateTimeOffset? LastModifiedTime { get; set; }

    public bool IsDeleted { get; set; } = false;

    protected BaseModel()
    {
        Id = Ulid.NewUlid().ToString();
        CreateTime = DateTimeOffset.UtcNow;
    }
}