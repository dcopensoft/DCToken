using System;
using System.Collections.Generic;
using System.Text;

namespace DCToken.Models
{
    public class LanLib
    {
        private static string en = @"AboutView_Title`About
AboutView_Title2`DC Token 1.0
AboutView_Title3`DC Token is an open source, secure private digital wallet management application. It supports ETH /ERC20 Token/USDT/DEDC/DEDC sub coin.
WalletView_Title`My Wallet";
        private static string cn = @"AboutView_Title`About
AboutView_Title2`DC Token 1.0
AboutView_Title3`DC Token is an open source, secure private digital wallet management application. It supports ETH /ERC20 Token/USDT/DEDC/DEDC sub coin.
WalletView_Title`My Wallet";
        public static Dictionary<string, string> LanKey = new Dictionary<string, string>();

        public static void SetLan(string lan="en")
        {
            LanKey.Clear();
            if (lan == "en")
            {
                
                foreach (string item in en.Split(new string[1] {"\r\n" },StringSplitOptions.RemoveEmptyEntries))
                {
                    LanKey.Add(item.Split('`')[0], item.Split('`')[1]);
                }
            }
            else if (lan == "cn")
            {
                foreach (string item in cn.Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    LanKey.Add(item.Split('`')[0], item.Split('`')[1]);
                }
            }
        }
    }

  
}
