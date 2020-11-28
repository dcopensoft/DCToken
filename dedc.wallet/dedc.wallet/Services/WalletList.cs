using DCToken.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCToken.Services
{
    public class Wallets
    {
        public static List<WalletList> WalletData = new List<WalletList>();

    }

    public class WalletList
    {
       public WalletInfo WI { get; set; }
        public List<WalletCoin> WCList { get; set; }

    }
}
