using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlazorEFIdentity.Models;

namespace BlazorEFIdentity.Data
{
    public class BankServices
    {
        private readonly ApplicationDbContext _context;

        public BankServices(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hämta alla bankkonton
        public async Task<List<BankAccount>> GetAccountsAsync()
        {
            return await _context.BankAccount.Include(a => a.Transaction).ToListAsync();
        }

        // Hämta ett specifikt konto via konto nmr
        public async Task<BankAccount?> GetAccountByIdAsync(string accountNumber)
        {
            return await _context.BankAccount.Include(a => a.Transaction)
                                              .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        // Skapa ett nytt konto och koppla det till användaren
        public async Task<bool> CreateAccountForUserAsync(BankAccount account, string userId)
        {
            bool accountExists = await _context.BankAccount
            .AnyAsync(a => a.AccountNumber == account.AccountNumber);

            if (accountExists)
            {
                return false;
            }
            account.UserId = userId;
            _context.BankAccount.Add(account);
            return await _context.SaveChangesAsync() > 0;
        }


        // Skapa en transaktion och uppdatera saldo
        public async Task<bool> CreateTransactionAsync(Transaction transaction)
        {
            var account = await _context.BankAccount.FirstOrDefaultAsync(a => a.Id == transaction.FromAccountId);
            if (account == null) return false;

            if (account.Balance < transaction.OutgoingBalance)
                return false; // Förhindrar negativa saldon

            account.Balance -= (int)transaction.OutgoingBalance;
            _context.Transaction.Add(transaction);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}


