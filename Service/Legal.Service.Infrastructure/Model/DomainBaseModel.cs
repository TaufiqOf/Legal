using System.ComponentModel.DataAnnotations.Schema;

namespace Legal.Service.Infrastructure.Model;

public abstract class DomainBaseModel : BaseModel
{
    public string? CreatedBy { get; set; } = default!;

    [ForeignKey("CreatedBy")]
    public User? CreatedByUser { get; set; }

    public string? LastModifiedBy { get; set; } = default!;

    [ForeignKey("LastModifiedBy")]
    public User? LastModifiedByUser { get; set; }
}