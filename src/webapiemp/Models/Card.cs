using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapiemp.Models
{
    //[Table("Card", Schema = "dbo")]
    public class Card
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Card ID")]
        public int Id { get; set; }

        [Required]
        [Column(TypeName ="nvarchar(150)")]
        [Display(Name = "Card Name")]
        public string title { get; set; }

        
        [Column(TypeName ="nvarchar(255)")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName ="varchar(15)")]
        [Display(Name ="Priority")]
        public string Priority { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name ="Date")]
        public DateTime Date { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string? State { get; set; }



        public int GroupId { get; set; }
        public virtual Group? Group { get; set; }
        
        public int? ResponderId { get; set; }
        public virtual User? Responder { get; set; }

        
        public int AuthorId { get; set; }
        public virtual User? Author { get; set; }





        //[Display(Name ="Employee Name")]
        //[NotMapped]
        //public string EmployeeName { get; set; }

        
        
        

    }
}
