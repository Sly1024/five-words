const fs = require('fs');

const inputwords = fs.readFileSync('words.txt', 'utf-8').split('\n');

const alternatives = [];
const bitmasks = [];

// filter out words with repeating letters and combine "alternatives" (anagrams)
for (const word of inputwords) {
    const mask = GetBitMask(word);
    if (Get1BitCount(mask) != 5) continue; // number of distinct letters is not 5
    var wordIdx = bitmasks.indexOf(mask);
    if (wordIdx >= 0) {     // an anagram already appeared
        alternatives[wordIdx].push(word);
        continue;
    }
    alternatives.push([word]);
    bitmasks.push(mask);
}

console.log(`Unique words: ${bitmasks.length}`);

// for each word, get a list of "matching" words that have no common letters
// this only stores the index of the words
const pairs = GetWordPairs(bitmasks);

TestWords(0, bitmasks.length-4);

console.log("Done.");

function TestWords(startIdx, endIdx) {
    // this will store the index of the 5 words (or alternatives)
    const solution = new Array(5);
    // first loop goes through the index of each word
    for (let word1 = startIdx; word1 < endIdx; word1++) {
        solution[0] = word1;
        const mask1 = bitmasks[word1];
        // second loop only goes through the "paired" words that have no common letter with word1
        for (const word2 of pairs[word1]) {
            // no need to check if there's a common letter with word1, we use the pairs list
            solution[1] = word2;
            // mask2 contains 1s for letters in both word1 and word2
            const mask2 = bitmasks[word2] | mask1;
            // looking at the pairs list of word2 (no common letter)
            for (const word3 of pairs[word2]) {
                let mask3 = bitmasks[word3];
                // we know that word2 and word3 has no common letters,
                // but we need to check because word1 and word3 might have
                if ((mask3 & mask2) != 0) continue;
                // combine the masks, so mask3 now contains 1s for letters in word1, word2 and word3
                mask3 |= mask2;
                solution[2] = word3;
                // word4 logic: same as word3
                for (const word4 of pairs[word3]) {
                    let mask4 = bitmasks[word4];
                    if ((mask4 & mask3) != 0) continue;
                    mask4 |= mask3;
                    solution[3] = word4;
                    for (const word5 of pairs[word4]) {
                        const mask5 = bitmasks[word5];
                        if ((mask5 & mask4) != 0) continue;
                        solution[4] = word5;
                        // found five words with no common letters, print it with all alternative words
                        ListSolutions(solution);
                    }
                }
            }
        }
    }
}

function ListSolutions(solution) {
    const words = solution.map(idx => alternatives[idx]);

    function CartesianProduct(aggregate = "", wordIdx = 0) {
        if (wordIdx == words.length) {
            console.log(aggregate + ",");
            return;
        }
        for (const word of words[wordIdx]) {
            CartesianProduct(aggregate + " " + word, wordIdx + 1);
        }
    }

    CartesianProduct();
    console.log();
}

// Returns the number of 1 bits in a bitmask
function Get1BitCount(bitmask) {
    let count = 0;
    while (bitmask > 0) {
        if ((bitmask & 1) == 1) count++;
        bitmask >>= 1;
    }
    return count;
}

// returns a bitmask containing 1s where the corresponding letter in the alphabet appears in the word
// last bit = 'a', second to last = 'b', etc.
// 'abc' = '00111', 'bce' = '10110'
function GetBitMask(word) {
    return word.split('').reduce((mask, c) => mask | (1 << (c.charCodeAt(0) - 97)), 0);
}

function GetWordPairs(bitmasks) {
    const pairs = new Array(bitmasks.length);
    for (let i = 0; i < bitmasks.length; i++) {
        const mask = bitmasks[i];
        const list = [];
        // notice: j starts from (i+1) so we don't count the same pair twice ( [i,j] and [j,i] )
        // without this optimization, the runtime goes up 
        // and it obviously finds all permutations of the possible words
        for (let j = i + 1; j < bitmasks.length; j++) {
            // if the binary AND of the two bitmasks results in zero => no common bits/letters
            if ((mask & bitmasks[j]) == 0) {
                list.push(j);    // add the index of the word to the list
            }
        }
        pairs[i] = list;  // assign a list of word indices for word 'i'
    }
    return pairs;
}