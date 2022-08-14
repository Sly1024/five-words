public class FiveWords {
    public static void Main() {
        var bitmasks = new List<int>();
        var alternatives = new List<List<string>>();

        var inputwords = File.ReadAllText("words.txt").Split("\n");

        // filter out words with repeating letters and combine "alternatives" (anagrams)
        foreach (var word in inputwords) {
            var mask = GetBitMask(word);
            if (Get1BitCount(mask) != 5) continue; // number of distinct letters is not 5
            var wordIdx = bitmasks.IndexOf(mask);
            if (wordIdx >= 0) {     // an anagram already appeared
                alternatives[wordIdx].Add(word);
                continue;
            }
            alternatives.Add(new () { word });
            bitmasks.Add(mask);
        }

        Console.WriteLine($"Unique words: {bitmasks.Count}");

        // for each word, get a list of "matching" words that have no common letters
        // this only stores the index of the words
        var pairs = GetWordPairs(bitmasks);

        // this will store the index of the 5 words (or alternatives)
        var solution = new int[5];
        
        // first loop goes through the index of each word
        for (int word1 = 0; word1 < bitmasks.Count-4; word1++) {
            solution[0] = word1;
            var mask1 = bitmasks[word1];
            // second loop only goes through the "paired" words that have no common letter with word1
            foreach (var word2 in pairs[word1]) {
                // no need to check if there's a common letter with word1, we use the pairs list
                solution[1] = word2;
                // mask2 contains 1s for letters in both word1 and word2
                var mask2 = bitmasks[word2] | mask1;
                // looking at the pairs list of word2 (no common letter)
                foreach (var word3 in pairs[word2]) {
                    var mask3 = bitmasks[word3];
                    // we know that word2 and word3 has no common letters,
                    // but we need to check because word1 and word3 might have
                    if ((mask3 & mask2) != 0) continue;
                    // combine the masks, so mask3 now contains 1s for letters in word1, word2 and word3
                    mask3 |= mask2;
                    solution[2] = word3;
                    // word4 logic: same as word3
                    foreach (var word4 in pairs[word3]) {
                        var mask4 = bitmasks[word4];
                        if ((mask4 & mask3) != 0) continue;
                        mask4 |= mask3;
                        solution[3] = word4;
                        foreach (var word5 in pairs[word4]) {
                            var mask5 = bitmasks[word5];
                            if ((mask5 & mask4) != 0) continue;
                            solution[4] = word5;
                            // found five words with no common letters, print it with all alternative words
                            ListSolutions(solution, alternatives);
                        }
                    }
                }
            }
        }

        Console.WriteLine("Done.");
    }

    private static void ListSolutions(int[] solution, List<List<string>> alternatives) {
        var words = solution.Select(idx => alternatives[idx]).ToArray();

        void CartesianProduct(string aggregate = "", int wordIdx = 0) {
            if (wordIdx == words.Length) {
                Console.Write(aggregate + ",");
                return;
            }
            foreach (var word in words[wordIdx]) {
                CartesianProduct(aggregate + " " + word, wordIdx + 1);
            }
        }

        CartesianProduct();
        Console.WriteLine();
    }

    // Returns the number of 1 bits in a bitmask
    private static int Get1BitCount(int bitmask) {
        var count = 0;
        while (bitmask > 0) {
            if ((bitmask & 1) == 1) count++;
            bitmask >>= 1;
        }
        return count;
    }

    // returns a bitmask containing 1s where the corresponding letter in the alphabet appears in the word
    // last bit = 'a', second to last = 'b', etc.
    // 'abc' = '00111', 'bce' = '10110'
    private static int GetBitMask(string word) {
        return word.Aggregate(0, (mask, c) => mask | (1 << (c-'a')));
    }

    private static int[][] GetWordPairs(IList<int> bitmasks) {
        var pairs = new int[bitmasks.Count][];
        for (int i = 0; i < bitmasks.Count; i++) {
            var mask = bitmasks[i];
            var list = new List<int>();
            // notice: j starts from (i+1) so we don't count the same pair twice ( [i,j] and [j,i] )
            // without this optimization, the runtime goes up to 16 mins, instead of 22 seconds
            // and it obviously finds all permutations of the possible words
            for (int j = i + 1; j < bitmasks.Count; j++) {
                // if the binary AND of the two bitmasks results in zero => no common bits/letters
                if ((mask & bitmasks[j]) == 0) {
                    list.Add(j);    // add the index of the word to the list
                }
            }
            pairs[i] = list.ToArray();  // assign a list of word indices for word 'i'
        }
        return pairs;
    }
}