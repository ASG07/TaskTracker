using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace webapiemp.Models
{
    //[Table("Employee", Schema ="dbo")]
    public class User : IdentityUser<int>
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Display(Name ="Employee Id")]
        //public override int Id { get; set; }

        //[Required]
        //[Column(TypeName ="nvarchar(150)")]
        //public string Name { get; set; }

        //[Required]
        //public string Email { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //public string password { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string? ArabicName { get; set; }
        public ICollection<GroupMembership>? GroupMemberships { get; set;}
        public ICollection<Group>? GroupsAuthored { get; set; }
        public ICollection<Card>? CardsAuthored { get; set; }
        public ICollection<Card>? CardsRespondedTo { get; set; }


    }
}
