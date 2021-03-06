using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomUtil
{
    // Returns a HashSet containing unique integers, with size equal to amount and values up to max.
    // Gets slower the closer amount is to max.
    public static HashSet<int> UniqueIntegerHashSet(int max, int amount)
    {
        HashSet<int> uniqueInts = new HashSet<int>();

        if (amount > max)
        {
            return null;
        }

        while (uniqueInts.Count < amount)
        {
            int n = (int) Random.Range(0, max);
            uniqueInts.Add(n);
        }

        return uniqueInts;
    }
}
