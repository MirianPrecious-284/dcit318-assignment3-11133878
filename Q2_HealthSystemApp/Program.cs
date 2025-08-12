// Q2_HealthSystemApp/Program.cs
using System;
using System.Collections.Generic;
using System.Linq;

public class Repository<T>
{
    private readonly List<T> items = new();
    public void Add(T item) => items.Add(item);
    public List<T> GetAll() => new List<T>(items);
    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item == null) return false;
        items.Remove(item);
        return true;
    }
}

public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id; Name = name; Age = age; Gender = gender;
    }

    public override string ToString() => $"{Name} (ID: {Id}, Age: {Age}, Gender: {Gender})";
}

public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id; PatientId = patientId; MedicationName = medicationName; DateIssued = dateIssued;
    }

    public override string ToString() => $"{MedicationName} (Issued: {DateIssued:d})";
}

public class HealthSystemApp
{
    Repository<Patient> _patientRepo = new();
    Repository<Prescription> _prescriptionRepo = new();
    Dictionary<int, List<Prescription>> _prescriptionMap = new();

    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Alice Smith", 30, "F"));
        _patientRepo.Add(new Patient(2, "John Doe", 45, "M"));
        _patientRepo.Add(new Patient(3, "Mary Johnson", 52, "F"));

        _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Paracetamol", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Atorvastatin", DateTime.Now.AddDays(-15)));
        _prescriptionRepo.Add(new Prescription(4, 3, "Lisinopril", DateTime.Now.AddDays(-2)));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();
        foreach (var p in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(p.PatientId))
                _prescriptionMap[p.PatientId] = new List<Prescription>();
            _prescriptionMap[p.PatientId].Add(p);
        }
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("Patients:");
        foreach (var pt in _patientRepo.GetAll())
            Console.WriteLine(pt);
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        return _prescriptionMap.TryGetValue(patientId, out var list) ? list : new List<Prescription>();
    }
}

class Program
{
    static void Main()
    {
        var app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();

        Console.WriteLine("\nPrescriptions for patient ID 1:");
        foreach (var pres in app.GetPrescriptionsByPatientId(1))
            Console.WriteLine(pres);
    }
}

