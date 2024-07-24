using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TrainTry.Models
{
    public class News
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }
        public DateTime dateBegin { get; set; }
        public DateTime dateEnd { get; set; }
        public string? topic { get; set; }
        public string? article { get; set; }
        public int importance { get; set; }
        public DateTime datePublish { get; set; }
        public string? author { get; set; }
    }
    
}
