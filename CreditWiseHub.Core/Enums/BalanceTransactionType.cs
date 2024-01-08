using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Enums
{
    /// <summary>
    /// Enum representing transactions on the account balance.
    /// </summary>
    public enum BalanceTransactionType
    {
        /// <summary>
        /// Represents a transaction that increases the account balance.
        /// </summary>
        Increase,

        /// <summary>
        /// Represents a transaction that decreases the account balance.
        /// </summary>
        Decrease
    }

}
