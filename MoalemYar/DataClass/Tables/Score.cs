using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoalemYar.DataClass.Tables
{
    [Table("Scores")]
    public class Score
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long StudentId { get; set; }

        [Required]
        public string Book { get; set; }

        [Required]
        public string Date { get; set; }

        [Required]
        public string Scores { get; set; }

        [Required]
        public string Desc { get; set; }
    }
}