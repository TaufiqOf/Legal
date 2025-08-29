using System.ComponentModel.DataAnnotations;

namespace Legal.Shared.SharedModel.ParameterModel;

public record IdParameterModel : BaseParameterModel
{
    [MinLength(1)]
    public required string Id { get; set; } = null!;
}