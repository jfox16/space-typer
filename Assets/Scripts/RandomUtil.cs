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

    // Returns an array of random non-repeating integers. max is exclusive. amount is how many ints will be returned.
    // Uses Fisher-Yates shuffle to get a random int in O(n) time.
    public static int[] RandomInts(int min, int max, int amount)
    {
        // fill up pool with ints [min, min+1, min+2, ..., max-1]
        int[] pool = new int[max - min];
        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = min + i;
        }

        int[] random = new int[amount];
        for (int i = 0; i < amount; i++)
        {
            int swapIndex = Random.Range(0, pool.Length-i);
            // Debug.Log("swap " + swapIndex);
            random[i] = pool[swapIndex];
            pool[swapIndex] = pool[pool.Length-1-i];
            // pool[pool.Length-1-i] = -1;
            // Debug.Log(string.Join(", ", pool));
            // Debug.Log(string.Join(", ", random));
        }

        return random;
    }
}
