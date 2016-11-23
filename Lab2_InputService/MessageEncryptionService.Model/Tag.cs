using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Model
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<PlannedTask> PlannedTasks { get; set; }
    }
}
