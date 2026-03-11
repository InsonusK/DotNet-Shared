using System.ComponentModel.DataAnnotations;
using Ardalis.Result;

namespace InsonusK.Shared.Model.Template;

public class BulkResponseDto<TSingleDto>
{
    [Required]
    public required IEnumerable<TSingleDto> Items { get; init; }

    public IEnumerable<ValidationError> ValidationMessages { get; init; } = [];
}

public class SingleResponseDto<TSingleDto>
{
    [Required]
    public required TSingleDto Item { get; init; }

    public IEnumerable<ValidationError> ValidationMessages { get; init; } = [];
}