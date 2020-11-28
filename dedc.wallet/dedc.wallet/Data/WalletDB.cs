using DCToken.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCToken.Services
{
    public class WalletDB
    {
        readonly SQLiteAsyncConnection _database;
         
        public WalletDB(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<WalletInfo>().Wait();
            
        }

        public Task<List<WalletInfo>> GetWalletListAsync()
        {
            return _database.Table<WalletInfo>().OrderByDescending(o => o.ID).ToListAsync();
        }
     
        public Task<List<WalletCoin>> GetWalletCionListAsync(int id)
        {
            return _database.Table<WalletCoin>()
                            .Where(i => i.WalletId == id)
                            .ToListAsync();
        }
        public Task<WalletInfo> GetWalletAsync(int id)
        {
            return _database.Table<WalletInfo>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }
        public Task<WalletInfo> GetWalletAsyncByAddress(string address)
        {
            return _database.Table<WalletInfo>()
                            .Where(i => i.Address == address)
                            .FirstOrDefaultAsync();
        }
        public Task<WalletCoin> GetWalletCoinAsync(int id)
        {
            return _database.Table<WalletCoin>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }
        public Task<int> SaveWalletAsync(WalletInfo Wallet)
        {
            if (Wallet.ID != 0)
            {
                return _database.UpdateAsync(Wallet);
            }
            else
            {
                return _database.InsertAsync(Wallet);
            }
        }
        public Task<int> SaveWalletCoinAsync(WalletCoin Coin)
        {
            if (Coin.ID != 0)
            {
                return _database.UpdateAsync(Coin);
            }
            else
            {
                return _database.InsertAsync(Coin);
            }
        }
        public Task<int> DeleteWalletAsync(WalletInfo Wallet)
        {
            return _database.DeleteAsync(Wallet);
        }

        public Task<int> DeleteWalletCoinAsync(WalletCoin Coin)
        {
            return _database.DeleteAsync(Coin);
        }
    }
}

