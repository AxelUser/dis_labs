using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Model
{
    public class PlannedTask
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int EstimatedDifficulty { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime EstimatedCompletionDate { get; set; }
        public virtual User Executor { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
