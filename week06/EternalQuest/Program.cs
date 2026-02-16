using System;

class Program
{
    // Creativity: Added a simple leveling system shown with the player's score.
    // Level is computed as (score / 1000) + 1 and is displayed by GoalManager.
    static void Main(string[] args)
    {
        var manager = new GoalManager();
        manager.Start();
    }
}