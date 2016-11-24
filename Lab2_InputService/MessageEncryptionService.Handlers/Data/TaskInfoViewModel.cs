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
        public string ExecutorLogin { get; set; }
        public string ExecutorFullName { get; set; }
        public string Tag { get; set; }
        public int EstimatedDifficulty { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime EstimatedCompletionDate { get; set; }
    }
}
