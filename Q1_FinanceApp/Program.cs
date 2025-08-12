// Q1_FinanceApp/Program.cs
using System;
using System.Collections.Generic;

record Transaction(int Id, DateTime Date, decimal Amount, string Category);

interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[BankTransfer] Processing {transaction.Category}: {transaction.Amount:C} on {transaction.Date:d}");
    }
}

class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[MobileMoney] Processing {transaction.Category}: {transaction.Amount:C} on {transaction.Date:d}");
    }
}

class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[CryptoWallet] Processing {transaction.Category}: {transaction.Amount:C} on {transaction.Date:d}");
    }
}

class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        // deduct amount
        Balance -= transaction.Amount;
        Console.WriteLine($"Applied {transaction.Amount:C}. New balance: {Balance:C}");
    }
}

sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
            return;
        }
        Balance -= transaction.Amount;
        Console.WriteLine($"Transaction {transaction.Category} of {transaction.Amount:C} applied. Updated balance: {Balance:C}");
    }
}

class FinanceApp
{
    private readonly List<Transaction> _transactions = new();

    public void Run()
    {
        var account = new SavingsAccount("SA-11133878", 1000m);

        var t1 = new Transaction(1, DateTime.Now, 120.50m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 250.00m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 600.00m, "Entertainment");

        ITransactionProcessor p1 = new MobileMoneyProcessor();
        ITransactionProcessor p2 = new BankTransferProcessor();
        ITransactionProcessor p3 = new CryptoWalletProcessor();

        p1.Process(t1);
        account.ApplyTransaction(t1);
        _transactions.Add(t1);

        p2.Process(t2);
        account.ApplyTransaction(t2);
        _transactions.Add(t2);

        p3.Process(t3);
        account.ApplyTransaction(t3);
        _transactions.Add(t3);

        Console.WriteLine("\nAll transactions:");
        foreach (var t in _transactions)
            Console.WriteLine(t);
    }
}

class Program
{
    static void Main()
    {
        new FinanceApp().Run();
    }
}

