using Legal.Service.Infrastructure.Model;

namespace Legal.Application.Admin.Models;
public class Contract : DomainBaseModel
{
    public string Author { get; set; } = default!;
    
    public string Name { get; set; } = default!;
    
    public string Description { get; set; } = default!;

    public DateTime Created { get; set; }
    
    public DateTime? Updated { get; set; }
    
}
