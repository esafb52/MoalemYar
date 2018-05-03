using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoalemYar.DataClass.Tables
{

    [Table("Questions")]
    public class Question
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long SchoolId { get; set; }

        [Required]
        public long StudentId { get; set; }

        [Required]
        public string Book { get; set; }

        [Required]
        public string Date { get; set; }
    }
}
