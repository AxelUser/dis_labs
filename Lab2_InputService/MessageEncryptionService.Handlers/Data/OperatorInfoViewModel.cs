using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Data
{
    public class OperatorInfoViewModel
    {
        public int Code { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public int Capacity { get; set; }
        public string OperatorName { get; set; }
        public string RegionName { get; set; }
    }
}
