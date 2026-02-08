using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

/*
 Exceeded Requirements:
 - Added a simple session log (`mindfulness_log.txt`) that appends activity name, duration, and timestamp.
 - Reflection and Listing prompts/questions are shuffled and will not repeat until all have been used in the session.
 - All code uses a base `Activity` class with derived classes for each activity type.
*/

abstract class Activity
{
    private string _name;
    private string _description;
    protected int _durationSeconds;
    protected static Random _rand = new Random();

    protected Activity(string name, string description)
    {
        _name = name;
        _description = description;
    }

    public void Run()
    {
        ShowStartingMessage();
        PromptDuration();
        Console.WriteLine("Prepare to begin...");
        ShowSpinner(3);
        DoActivity();
        ShowEndingMessage();
        LogActivity();
    }

    protected virtual void ShowStartingMessage()
    {
        Console.Clear();
        Console.WriteLine($"--- {_name} ---");
        Console.WriteLine(_description);
    }

    protected virtual void ShowEndingMessage()
    {
        Console.WriteLine();
        Console.WriteLine("Well done!");
        ShowSpinner(2);
        Console.WriteLine($"You have completed the {_name} for {_durationSeconds} seconds.");
        ShowSpinner(3);
    }

    protected void PromptDuration()
    {
        while (true)
        {
            Console.Write("Enter duration in seconds: ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out int seconds) && seconds > 0)
            {
                _durationSeconds = seconds;
                return;
            }
            Console.WriteLine("Please enter a positive integer for seconds.");
        }
    }

    protected void ShowSpinner(int seconds)
    {
        var sw = Stopwatch.StartNew();
        var spinner = new[] {"|", "/", "-", "\\"};
        int i = 0;
        while (sw.Elapsed.TotalSeconds < seconds)
        {
            Console.Write(spinner[i % spinner.Length]);
            Thread.Sleep(250);
            Console.Write('\b');
            i++;
        }
        Console.WriteLine();
    }

    protected void ShowCountdown(int seconds)
    {
        for (int i = seconds; i >= 1; i--)
        {
            Console.Write(i);
            Thread.Sleep(1000);
            Console.Write('\r');
            Console.Write(new string(' ', i.ToString().Length));
            Console.Write('\r');
        }
    }

    protected void LogActivity()
    {
        try
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mindfulness_log.txt");
            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\t{_name}\t{_durationSeconds}s";
            File.AppendAllLines(logPath, new[] { line });
        }
        catch
        {
            // logging is best-effort; ignore failures
        }
    }

    protected abstract void DoActivity();
}

class BreathingActivity : Activity
{
    public BreathingActivity() : base("Breathing Activity",
        "This activity will help you relax by walking you through breathing in and out slowly. Clear your mind and focus on your breathing.") { }

    protected override void DoActivity()
    {
        var sw = Stopwatch.StartNew();
        while (sw.Elapsed.TotalSeconds < _durationSeconds)
        {
            Console.WriteLine("\nBreathe in...");
            ShowCountdown(4);
            if (sw.Elapsed.TotalSeconds >= _durationSeconds) break;
            Console.WriteLine("Breathe out...");
            ShowCountdown(4);
        }
    }
}

class ReflectionActivity : Activity
{
    private List<string> _prompts = new List<string>
    {
        "Think of a time when you stood up for someone else.",
        "Think of a time when you did something really difficult.",
        "Think of a time when you helped someone in need.",
        "Think of a time when you did something truly selfless."
    };

    private List<string> _questions = new List<string>
    {
        "Why was this experience meaningful to you?",
        "Have you ever done anything like this before?",
        "How did you get started?",
        "How did you feel when it was complete?",
        "What made this time different than other times when you were not as successful?",
        "What is your favorite thing about this experience?",
        "What could you learn from this experience that applies to other situations?",
        "What did you learn about yourself through this experience?",
        "How can you keep this experience in mind in the future?"
    };

    private Queue<string> _promptQueue;
    private Queue<string> _questionQueue;

    public ReflectionActivity() : base("Reflection Activity",
        "This activity will help you reflect on times in your life when you have shown strength and resilience. This will help you recognize the power you have and how you can use it in other aspects of your life.")
    {
        _promptQueue = new Queue<string>(Shuffle(_prompts));
        _questionQueue = new Queue<string>(Shuffle(_questions));
    }

    protected override void DoActivity()
    {
        if (_promptQueue.Count == 0) _promptQueue = new Queue<string>(Shuffle(_prompts));
        var prompt = _promptQueue.Dequeue();
        Console.WriteLine();
        Console.WriteLine(prompt);
        Console.WriteLine("When you have something in mind, press Enter to continue.");
        Console.ReadLine();

        var sw = Stopwatch.StartNew();
        while (sw.Elapsed.TotalSeconds < _durationSeconds)
        {
            if (_questionQueue.Count == 0) _questionQueue = new Queue<string>(Shuffle(_questions));
            var question = _questionQueue.Dequeue();
            Console.WriteLine();
            Console.WriteLine(question);
            ShowSpinner(8);
        }
    }

    private IEnumerable<string> Shuffle(IEnumerable<string> items)
    {
        return items.OrderBy(x => _rand.Next()).ToArray();
    }
}

class ListingActivity : Activity
{
    private List<string> _prompts = new List<string>
    {
        "Who are people that you appreciate?",
        "What are personal strengths of yours?",
        "Who are people that you have helped this week?",
        "When have you felt the Holy Ghost this month?",
        "Who are some of your personal heroes?"
    };

    private Queue<string> _promptQueue;

    public ListingActivity() : base("Listing Activity",
        "This activity will help you reflect on the good things in your life by having you list as many things as you can in a certain area.")
    {
        _promptQueue = new Queue<string>(Shuffle(_prompts));
    }

    protected override void DoActivity()
    {
        if (_promptQueue.Count == 0) _promptQueue = new Queue<string>(Shuffle(_prompts));
        var prompt = _promptQueue.Dequeue();
        Console.WriteLine();
        Console.WriteLine(prompt);
        Console.WriteLine("You will have a few seconds to think, then begin listing. Press Enter after each item.");
        Console.WriteLine("Get ready...");
        ShowCountdown(5);

        var entries = new List<string>();
        var sw = Stopwatch.StartNew();
        while (sw.Elapsed.TotalSeconds < _durationSeconds)
        {
            Console.Write("> ");
            string? line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;
            entries.Add(line.Trim());
        }

        Console.WriteLine($"\nYou listed {entries.Count} items.");
    }

    private IEnumerable<string> Shuffle(IEnumerable<string> items)
    {
        return items.OrderBy(x => _rand.Next()).ToArray();
    }
}

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Mindfulness Program");
            Console.WriteLine("1. Breathing Activity");
            Console.WriteLine("2. Reflection Activity");
            Console.WriteLine("3. Listing Activity");
            Console.WriteLine("4. Quit");
            Console.Write("Select an option: ");
            var choice = Console.ReadLine();

            Activity? activity = choice switch
            {
                "1" => new BreathingActivity(),
                "2" => new ReflectionActivity(),
                "3" => new ListingActivity(),
                "4" => null,
                _ => null
            };

            if (choice == "4") break;
            if (activity == null)
            {
                Console.WriteLine("Invalid choice. Press Enter to continue.");
                Console.ReadLine();
                continue;
            }

            activity.Run();
            Console.WriteLine("Press Enter to return to the menu.");
            Console.ReadLine();
        }
    }
}