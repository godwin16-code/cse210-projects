using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        List<int> numbers = new List<int>();

        Console.WriteLine("Enter a list of numbers, type 0 when finished.");

        int number;
        while (true)
        {
            Console.Write("Enter number: ");
            if (!int.TryParse(Console.ReadLine(), out number))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            if (number == 0)
                break;

            numbers.Add(number);
        }

        if (numbers.Count == 0)
        {
            Console.WriteLine("No numbers were entered.");
            return;
        }

        // Sum
        int sum = 0;
        foreach (int n in numbers)
        {
            sum += n;
        }

        // Average
        double average = (double)sum / numbers.Count;

        // Largest number
        int largest = numbers[0];
        foreach (int n in numbers)
        {
            if (n > largest)
                largest = n;
        }

        // Smallest positive number
        int smallestPositive = int.MaxValue;
        foreach (int n in numbers)
        {
            if (n > 0 && n < smallestPositive)
                smallestPositive = n;
        }

        // Sort list (C# built-in method)
        numbers.Sort();

        // Output
        Console.WriteLine($"The sum is: {sum}");
        Console.WriteLine($"The average is: {average}");
        Console.WriteLine($"The largest number is: {largest}");

        if (smallestPositive != int.MaxValue)
            Console.WriteLine($"The smallest positive number is: {smallestPositive}");
        else
            Console.WriteLine("There is no positive number.");

        Console.WriteLine("The sorted list is:");
        foreach (int n in numbers)
        {
            Console.WriteLine(n);
        }
    }
}
