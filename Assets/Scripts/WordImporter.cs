using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;

public class WordImporter
{
    public enum Difficulty { EASY, MEDIUM, HARD };

    public static string[] GetWords(Difficulty difficulty, int numWords)
    {
        string filePath;

        switch(difficulty)
        {
            case Difficulty.EASY:
                filePath = "Assets/Data/js/easy-words.json";
                break;

            case Difficulty.MEDIUM:
                filePath = "Assets/Data/js/medium-words.json";
                break;

            case Difficulty.HARD:
                filePath = "Assets/Data/js/hard-words.json";
                break;

            default:
                filePath = "Assets/Data/js/easy-words.json";
                break;
        }

        StreamReader reader = new StreamReader(filePath);
        string jsonString = reader.ReadToEnd();
        JSONArray jsonArray = JSON.Parse(jsonString).AsArray;

        string[] words = new string[jsonArray.Count];
        for (int i = 0; i < jsonArray.Count; i++)
        {
            words[i] = jsonArray[i].Value;
        }

        return words;
    }
}
