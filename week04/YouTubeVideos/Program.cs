using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        List<Video> videos = new List<Video>();

        Video video1 = new Video("Learn C# in 10 Minutes", "TechWithSam", 600);
        video1.AddComment(new Comment("Alice", "Very helpful!"));
        video1.AddComment(new Comment("Bob", "Nice explanation"));
        video1.AddComment(new Comment("Chris", "Loved it"));

        Video video2 = new Video("OOP Concepts Explained", "CodeAcademy", 850);
        video2.AddComment(new Comment("Diana", "Great examples"));
        video2.AddComment(new Comment("Evan", "Clear and simple"));
        video2.AddComment(new Comment("Faith", "Thanks a lot"));

        Video video3 = new Video("Abstraction in C#", "DevWorld", 720);
        video3.AddComment(new Comment("George", "This helped me pass my test"));
        video3.AddComment(new Comment("Helen", "Well structured"));
        video3.AddComment(new Comment("Ivan", "Awesome content"));

        videos.Add(video1);
        videos.Add(video2);
        videos.Add(video3);

        foreach (Video video in videos)
        {
            Console.WriteLine($"Title: {video.Title}");
            Console.WriteLine($"Author: {video.Author}");
            Console.WriteLine($"Length: {video.LengthInSeconds} seconds");
            Console.WriteLine($"Number of Comments: {video.GetCommentCount()}");

            foreach (Comment comment in video.GetComments())
            {
                Console.WriteLine($"  {comment.CommenterName}: {comment.Text}");
            }

            Console.WriteLine();
        }
    }
}