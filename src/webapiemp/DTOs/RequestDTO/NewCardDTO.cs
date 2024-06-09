using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapiemp.DTOs.RequestDTO;

public class NewCardDTO
{
    [Required]
    [Column(TypeName = "nvarchar(150)")]
    [Display(Name = "Card Name")]
    public string title { get; set; }


    [Column(TypeName = "nvarchar(255)")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "varchar(15)")]
    [Display(Name = "Priority")]
    public string Priority { get; set; }


    [Column(TypeName = "varchar(150)")]
    public string? State { get; set; }

    [Required]
    public int AssigneeId { get; set; }
}
