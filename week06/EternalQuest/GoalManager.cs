using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

public class GoalManager
{
    private List<Goal> _goals;
    private int _score;

    public GoalManager()
    {
        _goals = new List<Goal>();
        _score = 0;
    }

    // Entry point for the manager - interactive menu loop
    public void Start()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine();
            DisplayPlayerInfo();
            Console.WriteLine("Menu:\n 1. Create New Goal\n 2. List Goals\n 3. Record Event\n 4. Save Goals\n 5. Load Goals\n 6. Quit");
            Console.Write("Select an option: ");
            var input = Console.ReadLine();
            Console.WriteLine();
            switch (input)
            {
                case "1":
                    CreateGoal();
                    break;
                case "2":
                    ListGoalDetails();
                    break;
                case "3":
                    RecordEvent();
                    break;
                case "4":
                    Console.Write("Enter filename to save to (default: goals.txt): ");
                    var saveName = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(saveName)) saveName = "goals.txt";
                    SaveGoals(saveName);
                    break;
                case "5":
                    Console.Write("Enter filename to load from (default: goals.txt): ");
                    var loadName = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(loadName)) loadName = "goals.txt";
                    LoadGoals(loadName);
                    break;
                case "6":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid selection.");
                    break;
            }
        }
    }

    public void AddGoal(Goal g)
    {
        _goals.Add(g);
    }

    public IReadOnlyList<Goal> GetGoals() => _goals.AsReadOnly();

    public void AddScore(int points)
    {
        _score += points;
    }

    public int Score => _score;

    public void DisplayPlayerInfo()
    {
        int level = (_score / 1000) + 1;
        Console.WriteLine($"Score: {_score}  |  Level: {level}");
    }

    public void ListGoalDetails()
    {
        if (_goals.Count == 0)
        {
            Console.WriteLine("No goals available.");
            return;
        }

        Console.WriteLine("Goals:");
        for (int i = 0; i < _goals.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_goals[i].GetDetailsString()}");
        }
    }

    public void CreateGoal()
    {
        Console.WriteLine("Select goal type:\n 1. Simple Goal\n 2. Eternal Goal\n 3. Checklist Goal");
        Console.Write("Type: ");
        var type = Console.ReadLine();
        Console.Write("Name: ");
        var name = Console.ReadLine() ?? "";
        Console.Write("Description: ");
        var desc = Console.ReadLine() ?? "";
        Console.Write("Points: ");
        if (!int.TryParse(Console.ReadLine(), out int pts)) pts = 0;

        switch (type)
        {
            case "1":
                var sg = new SimpleGoal(name, desc, pts);
                AddGoal(sg);
                Console.WriteLine("Simple goal created.");
                break;
            case "2":
                var eg = new EternalGoal(name, desc, pts);
                AddGoal(eg);
                Console.WriteLine("Eternal goal created.");
                break;
            case "3":
                Console.Write("Target count: ");
                if (!int.TryParse(Console.ReadLine(), out int target)) target = 1;
                Console.Write("Bonus points: ");
                if (!int.TryParse(Console.ReadLine(), out int bonus)) bonus = 0;
                var cg = new ChecklistGoal(name, desc, pts, target, bonus);
                AddGoal(cg);
                Console.WriteLine("Checklist goal created.");
                break;
            default:
                Console.WriteLine("Unknown type. Aborting creation.");
                break;
        }
    }

    public void RecordEvent()
    {
        if (_goals.Count == 0)
        {
            Console.WriteLine("No goals to record.");
            return;
        }

        Console.WriteLine("Which goal did you accomplish?");
        for (int i = 0; i < _goals.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_goals[i].GetName()}");
        }
        Console.Write("Choose number: ");
        if (!int.TryParse(Console.ReadLine(), out int choice))
        {
            Console.WriteLine("Invalid choice.");
            return;
        }
        if (choice < 1 || choice > _goals.Count)
        {
            Console.WriteLine("Choice out of range.");
            return;
        }

        var goal = _goals[choice - 1];
        int gained = goal.RecordEvent();
        AddScore(gained);
        Console.WriteLine($"You gained {gained} points.");
    }

    public void SaveGoals(string filename)
    {
        try
        {
            using (var w = new System.IO.StreamWriter(filename))
            {
                // Write score on first line
                w.WriteLine(_score);
                // Write each goal as a JSON object per-line for robust parsing
                foreach (var g in _goals)
                {
                    JsonObject obj = new JsonObject();
                    switch (g)
                    {
                        case SimpleGoal s:
                            obj["Type"] = "SimpleGoal";
                            obj["Name"] = s.GetName();
                            obj["Description"] = s.GetDetailsString();
                            obj["Raw"] = s.GetStringRepresentation();
                            break;
                        case EternalGoal e:
                            obj["Type"] = "EternalGoal";
                            obj["Name"] = e.GetName();
                            obj["Description"] = e.GetDetailsString();
                            obj["Raw"] = e.GetStringRepresentation();
                            break;
                        case ChecklistGoal c:
                            obj["Type"] = "ChecklistGoal";
                            obj["Name"] = c.GetName();
                            obj["Description"] = c.GetDetailsString();
                            obj["Raw"] = c.GetStringRepresentation();
                            break;
                        default:
                            obj["Type"] = "Unknown";
                            obj["Raw"] = g.GetStringRepresentation();
                            break;
                    }
                    w.WriteLine(obj.ToJsonString());
                }
            }
            Console.WriteLine($"Saved to {filename}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save: {ex.Message}");
        }
    }

    public void LoadGoals(string filename)
    {
        try
        {
            if (!System.IO.File.Exists(filename))
            {
                Console.WriteLine("File does not exist.");
                return;
            }
            var lines = System.IO.File.ReadAllLines(filename);
            if (lines.Length == 0) return;
            _goals.Clear();
            if (!int.TryParse(lines[0], out _score)) _score = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    var node = JsonNode.Parse(lines[i]) as JsonObject;
                    if (node == null) continue;
                    var type = node["Type"]?.GetValue<string>();
                    var raw = node["Raw"]?.GetValue<string>();
                    if (type == "SimpleGoal" && raw != null)
                    {
                        // Raw format: SimpleGoal|name|description|points|isComplete
                        var parts = raw.Split('|');
                        if (parts.Length >= 5)
                        {
                            var name = parts[1];
                            var desc = parts[2];
                            int pts = int.TryParse(parts[3], out int pval) ? pval : 0;
                            bool complete = bool.TryParse(parts[4], out bool bval) ? bval : false;
                            var sg = new SimpleGoal(name, desc, pts);
                            if (complete) sg.RecordEvent();
                            AddGoal(sg);
                        }
                    }
                    else if (type == "EternalGoal" && raw != null)
                    {
                        var parts = raw.Split('|');
                        if (parts.Length >= 4)
                        {
                            var name = parts[1];
                            var desc = parts[2];
                            int pts = int.TryParse(parts[3], out int pval) ? pval : 0;
                            var eg = new EternalGoal(name, desc, pts);
                            AddGoal(eg);
                        }
                    }
                    else if (type == "ChecklistGoal" && raw != null)
                    {
                        var parts = raw.Split('|');
                        if (parts.Length >= 7)
                        {
                            var name = parts[1];
                            var desc = parts[2];
                            int pts = int.TryParse(parts[3], out int p1) ? p1 : 0;
                            int amount = int.TryParse(parts[4], out int p2) ? p2 : 0;
                            int target = int.TryParse(parts[5], out int p3) ? p3 : 1;
                            int bonus = int.TryParse(parts[6], out int p4) ? p4 : 0;
                            var cg = new ChecklistGoal(name, desc, pts, target, bonus);
                            for (int k = 0; k < amount; k++) cg.RecordEvent();
                            AddGoal(cg);
                        }
                    }
                }
                catch { continue; }
            }
            Console.WriteLine($"Loaded from {filename}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load: {ex.Message}");
        }
    }
}
