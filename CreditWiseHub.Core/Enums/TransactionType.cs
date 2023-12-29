using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditWiseHub.Core.Enums
{
    //Outgoing money types are odd, incoming money types are even
    public enum TransactionType
    {
        Deposit = 1,        // Para Yatırma
        Withdraw = 2,       // Para Çekme
        OutgoingTransfer = 4, // Gönderilen Transfer
        IncomingTransfer = 3 // Gelen Transfer
    }
}
