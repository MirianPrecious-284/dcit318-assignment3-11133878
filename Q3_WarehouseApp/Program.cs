// Q3_WarehouseApp/Program.cs
using System;
using System.Collections.Generic;

interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
    }

    public override string ToString() => $"{Name} (ID:{Id}) Brand:{Brand} Qty:{Quantity} Warranty:{WarrantyMonths}m";
}

class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate;
    }

    public override string ToString() => $"{Name} (ID:{Id}) Qty:{Quantity} Expires:{ExpiryDate:d}";
}

// Custom exceptions
class DuplicateItemException : Exception { public DuplicateItemException(string msg) : base(msg) { } }
class ItemNotFoundException : Exception { public ItemNotFoundException(string msg) : base(msg) { } }
class InvalidQuantityException : Exception { public InvalidQuantityException(string msg) : base(msg) { } }

class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id)) throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out var item)) throw new ItemNotFoundException($"Item {id} not found.");
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id)) throw new ItemNotFoundException($"Item {id} not found.");
    }

    public List<T> GetAllItems() => new List<T>(_items.Values);

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0) throw new InvalidQuantityException("Quantity cannot be negative.");
        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

class WareHouseManager
{
    InventoryRepository<ElectronicItem> _electronics = new();
    InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        try {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 5, "BrandA", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 10, "BrandB", 12));

            _groceries.AddItem(new GroceryItem(101, "Rice", 50, DateTime.Now.AddMonths(12)));
            _groceries.AddItem(new GroceryItem(102, "Milk", 20, DateTime.Now.AddDays(30)));
        } catch (Exception ex) {
            Console.WriteLine("Seed error: " + ex.Message);
        }
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var it in repo.GetAllItems())
            Console.WriteLine(it);
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Increased item {id} by {quantity}. New qty: {repo.GetItemById(id).Quantity}");
        }
        catch (Exception ex) { Console.WriteLine($"Error increasing stock: {ex.Message}"); }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Removed item {id}");
        }
        catch (Exception ex) { Console.WriteLine($"Error removing item: {ex.Message}"); }
    }
}

class Program
{
    static void Main()
    {
        var manager = new WareHouseManager();
        manager.SeedData();

        Console.WriteLine("Grocery items:");
        manager.PrintAllItems(manager._groceries);

        Console.WriteLine("\nElectronic items:");
        manager.PrintAllItems(manager._electronics);

        // trigger exceptions
        try { manager._groceries.AddItem(new GroceryItem(101, "Duplicate Rice", 5, DateTime.Now.AddMonths(6))); }
        catch (Exception ex) { Console.WriteLine("Expected duplicate add error: " + ex.Message); }

        try { manager._electronics.RemoveItem(999); }
        catch (Exception ex) { Console.WriteLine("Expected remove error: " + ex.Message); }

        try { manager._groceries.UpdateQuantity(101, -5); }
        catch (Exception ex) { Console.WriteLine("Expected invalid quantity error: " + ex.Message); }
    }
}

