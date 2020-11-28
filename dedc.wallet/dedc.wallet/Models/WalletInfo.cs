using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace DCToken.Models
{
    public class WalletInfo
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Text { get; set; }

        public decimal Balance { get; set; }
        public string Address { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public string FJson { get; set; }
    }

    public class WalletCoin
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int WalletId { get; set; }
        /// <summary>
        /// 代币合约地址
        /// </summary>
        public string CAddress { get; set; }
        /// <summary>
        /// 小数位
        /// </summary>
        public short Decimals { get; set; }
        /// <summary>
        /// 代币名
        /// </summary>
        public string Symbol { get; set; }

        public string SymbolIconUrl { get; set; }
    }
}
