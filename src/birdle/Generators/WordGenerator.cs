using System;

namespace birdle.Generators;

public class WordGenerator : IGenerator
{
    private Random _random;
    
    public readonly string[] Words;

    public WordGenerator(string[] words)
    {
        Words = words;

        _random = new Random();
    }
    
    public string Generate()
    {
        return Words[_random.Next(Words.Length)];
    }

    public bool CheckIfValid(string response)
    {
        return Array.BinarySearch(Words, response.ToLower()) >= 0;
    }
}