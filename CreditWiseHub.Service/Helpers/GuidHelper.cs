using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Service.Helpers
{
    public class GuidHelper
    {
        public virtual Guid NewGuid() => Guid.NewGuid();
        public virtual string GetAccountNumberBaseGuid() => Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
    }
}
