using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Model
{
    public class PlannedTask
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int EstimatedDifficulty { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime EstimatedCompletionDate { get; set; }
        public virtual User Executor { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
