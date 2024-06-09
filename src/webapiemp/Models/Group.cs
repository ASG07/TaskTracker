using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapiemp.Models;

public class Group
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(150)")]
    public string Name { get; set; }

    [Required]
    [Column(TypeName = "varchar(150)")]
    public string InvitationCode { get; set; }

    public int AuthorId { get; set; }
    public virtual User? Author {get; set;}

    public ICollection<GroupMembership> GroupMemberships { get; set; }
    public ICollection<Card> Cards { get; set; }

}
