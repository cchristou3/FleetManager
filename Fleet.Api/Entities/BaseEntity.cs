using System.ComponentModel.DataAnnotations.Schema;

namespace Fleet.Api.Entities;

public class BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
}