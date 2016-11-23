using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Data
{
    [Serializable]
    public class TaskInfoViewModel
    {
        public Guid TaskId { get; set; }
        public string ExecutorLogin { get; set; }
        public string ExecutorFullName { get; set; }
        public string StatusName { get; set; }
        public string StatusCaption { get; set; }
        public IEquatable<string> Tags { get; set; }
        public int EstimatedDifficulty { get; set; }
        public string Text { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime EstimatedCompletionDate { get; set; }
    }
}
