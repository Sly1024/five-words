# Five five-letter words with no common letters
This is a solution to a problem I found interesting:

https://www.youtube.com/watch?v=_-AfhLQfb6w

Find five five-letter (English) words that have no common letters. This means using 25 letters from the 26 letter English alphabet.

For English words I combined these two sources:  
[wordle-answers-alphabetical.txt](https://gist.github.com/cfreshman/a03ef2cba789d8cf00c08f767e0fad7b)  
[wordle-allowed-guesses.txt](https://gist.github.com/cfreshman/cdcdf777450c5b5301e439061d29694c)

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