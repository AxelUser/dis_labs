using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Model
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public virtual ICollection<PlannedTask> PlannedTasks { get; set; }
    }
}
