using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;

public class WordImporter
{
    public enum Difficulty { Baby, Easy, Medium, Hard, Legendary };

    public static string[] GetWords(Difficulty difficulty)
    {
        switch(difficulty)
        {
            case Difficulty.Baby:
                return Data.wordsByLevel[0];

            case Difficulty.Easy:
                return Data.wordsByLevel[1];

            case Difficulty.Medium:
                return Data.wordsByLevel[2];

            case Difficulty.Hard:
                return Data.wordsByLevel[3];

            case Difficulty.Legendary:
                return Data.wordsByLevel[4];

            default:
                return Data.wordsByLevel[0];
        }
    }

    public static string GetWord(Difficulty difficulty)
    {
        string[] words = GetWords(difficulty);
        return words[Random.Range(0, words.Length)];
    }
}
