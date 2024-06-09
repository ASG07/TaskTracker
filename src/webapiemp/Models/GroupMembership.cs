using System.ComponentModel.DataAnnotations.Schema;

namespace webapiemp.Models;

public class GroupMembership
{
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public int GroupId { get; set; }
    public virtual Group? Group { get; set; }

    [Column(TypeName = "varchar(150)")]
    public string Role { get; set; }

    public int AmountWorkingOn { get; set; } = 0;

    public int AmountFinished { get; set; } = 0;

    public int AmountBugsSubmitted { get; set; } = 0;

    public string Status { get; set; }

}
