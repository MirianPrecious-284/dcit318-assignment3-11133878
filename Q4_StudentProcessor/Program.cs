// Q4_StudentProcessor/Program.cs
using System;
using System.Collections.Generic;
using System.IO;

class InvalidScoreFormatException : Exception { public InvalidScoreFormatException(string msg) : base(msg) {} }
class MissingFieldException : Exception { public MissingFieldException(string msg) : base(msg) {} }

class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id; FullName = fullName; Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }

    public override string ToString() => $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
}

class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();
        using var sr = new StreamReader(inputFilePath);
        string? line;
        int lineNum = 0;
        while ((line = sr.ReadLine()) != null)
        {
            lineNum++;
            var parts = line.Split(',');
            if (parts.Length < 3) throw new MissingFieldException($"Line {lineNum}: missing fields.");

            if (!int.TryParse(parts[0].Trim(), out var id))
                throw new InvalidScoreFormatException($"Line {lineNum}: invalid ID format.");

            var name = parts[1].Trim();

            if (!int.TryParse(parts[2].Trim(), out var score))
                throw new InvalidScoreFormatException($"Line {lineNum}: score is not an integer.");

            students.Add(new Student(id, name, score));
        }
        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using var sw = new StreamWriter(outputFilePath);
        foreach (var s in students)
            sw.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
    }
}

class Program
{
    static void Main()
    {
        var processor = new StudentResultProcessor();
        string input = "input.txt";
        string output = "report.txt";

        try
        {
            var students = processor.ReadStudentsFromFile(input);
            processor.WriteReportToFile(students, output);
            Console.WriteLine($"Report written to {output}");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Input file not found.");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine("Invalid score format: " + ex.Message);
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine("Missing field: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected error: " + ex.Message);
        }
    }
}
