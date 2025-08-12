// Q5_InventoryApp/Program.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

interface IInventoryEntity { int Id { get; } }

public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath) { _filePath = filePath; }

    public void Add(T item) => _log.Add(item);
    public List<T> GetAll() => new List<T>(_log);

    public void SaveToFile()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_log, options);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex) { Console.WriteLine("Save error: " + ex.Message); }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath)) { _log.Clear(); return; }
            var json = File.ReadAllText(_filePath);
            var items = JsonSerializer.Deserialize<List<T>>(json);
            _log.Clear();
            if (items != null) _log.AddRange(items);
        }
        catch (Exception ex) { Console.WriteLine("Load error: " + ex.Message); }
    }
}

class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger = new("inventory.json");

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Mouse", 15, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Keyboard", 10, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Monitor", 5, DateTime.Now));
    }

    public void SaveData() => _logger.SaveToFile();

    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        foreach (var item in _logger.GetAll())
            Console.WriteLine(item);
    }
}

class Program
{
    static void Main()
    {
        var app = new InventoryApp();
        app.SeedSampleData();

        app.SaveData();
        Console.WriteLine("Data saved. Clearing in-memory list and reloading...");

        // simulate new session
        var app2 = new InventoryApp();
        app2.LoadData();
        app2.PrintAllItems();
    }
}
