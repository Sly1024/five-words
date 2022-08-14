# Five five-letter words with no common letters
This is a solution to a problem I found interesting:

https://www.youtube.com/watch?v=_-AfhLQfb6w

Find five five-letter (English) words that have no common letters. This means using 25 letters from the 26 letter English alphabet.

For English words I combined these two sources:  
[wordle-answers-alphabetical.txt](https://gist.github.com/cfreshman/a03ef2cba789d8cf00c08f767e0fad7b)  
[wordle-allowed-guesses.txt](https://gist.github.com/cfreshman/cdcdf777450c5b5301e439061d29694c)

## The trick
My idea is that we can use a 26 bit bitmask for each word - indicating which letters are present - to quickly test whether two words
have common letters. Then we do a brute-force search, and when we already selected (let's say) three words, we combine their bitmasks
(binary OR), so we can test the bitmask of the fourth word against that combined mask with one binary AND operation.

Another idea is that for each word we can pre-calculate the list of words that have no common letters with it,
so the inner loops of the brute-force algorithm don't have to iterate over the entire set, only the ones that we
know work.

Also, we don't care about permutations of the 5 word solutions, so for each word (index 'i') in the list, we only store
the matching words (index 'j') that come after it (i < j), not the ones before it.


## Compiling
You need [.NET SDK 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) installed, the `-c Release` is to optimize the code for speed
```
dotnet build -c Release
```

## Running
Run the release build and measure the time
```
time ./bin/Release/net6.0/five-words 
```

## Results
This was run on a Ryzen 7 CPU, but it still runs on a signle thread.  
The line with two solutions is when an anagram (cylix <-> xylic) was found.
```
Unique words: 5182
 bemix clunk grypt vozhd waqfs,
 bling treck vozhd waqfs jumpy,
 blunk cimex grypt vozhd waqfs,
 brung cylix kempt vozhd waqfs, brung xylic kempt vozhd waqfs,
 clipt jumby kreng vozhd waqfs,
 glent jumby vozhd waqfs prick,
 glent vozhd waqfs brick jumpy,
 gucks vibex fjord nymph waltz,
 gymps vibex chunk fjord waltz,
 jumby pling treck vozhd waqfs,
Done.

real    0m21,857s
user    0m21,837s
sys     0m0,024s
```

### Running in parallel
This is relateively easy to run in parallel, we just split the top-level loop range into chunks and let each
thread process a chunk. 
We could make the number of chunnks equal to the number of CPUs on the machine, but that would not be splitting
the work equally, because ranges in the beginning need more processing - remember we test words only that come
*after* the current word.

The dotnet ThreadPool is efficient in scheduling tasks to threads, so we can just split the entire range into 
small enough chunks and leave the scheduling to the runtime. I chose a chunk size of 50.

This also means we will get results in an indeterminate order, depending on which thread finishes first.

```
Unique words: 5182
Chunks: 104
 brung cylix kempt vozhd waqfs, brung xylic kempt vozhd waqfs,
 bling treck vozhd waqfs jumpy,
 bemix clunk grypt vozhd waqfs,
 clipt jumby kreng vozhd waqfs,
 blunk cimex grypt vozhd waqfs,
 glent jumby vozhd waqfs prick,
 glent vozhd waqfs brick jumpy,
 gymps vibex chunk fjord waltz,
 gucks vibex fjord nymph waltz,
 jumby pling treck vozhd waqfs,
Done.

real    0m5,000s
user    1m14,193s
sys     0m0,045s
``` 
