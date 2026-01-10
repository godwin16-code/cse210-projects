using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello World! This is the Exercise3 Project.");

        string keepPlaying = "yes";
        while (keepPlaying == "yes")
        {
            Random randomGenerator = new Random();
            int magicNumber = randomGenerator.Next(1, 100);
            int guessCount = 0;
            int guess = -1;

            while (guess != magicNumber)
            {
                Console.Write("What is your guess? ");
                string number = Console.ReadLine();
                guess = int.Parse(number);

                if (guess < magicNumber)
                {
                    Console.WriteLine("Higher");
                    guessCount += 1;
                }
                else if (guess> magicNumber)
                {
                    Console.WriteLine("Lower");
                    guessCount += 1;
                }
                else 
                {
                    Console.WriteLine("You guessed it");
                }
            }
            Console.WriteLine($"It took you {guessCount} guesses");
            Console.Write("Do you want to play again? ");
            keepPlaying = Console.ReadLine();
        }
        Console.WriteLine("Thank you for playing! Goodbye");
    } 
}