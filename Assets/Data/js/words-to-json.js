
const fs = require('fs');

const relativeFilePath = process.argv[2];

fs.readFile(relativeFilePath, 'utf8', (err, data) => {
  if (err) throw err;

  // split by whitespaces or commas
  let words = data.split(/[\s,]+/);

  // remove empty strings
  words = words.filter(Boolean);

  // remove Capitalized words
  words = words.filter(word => word[0] === word[0].toLowerCase());

  // remove duplicates
  const wordSet = new Set(words);
  words = Array.from(wordSet);

  // sort alphabetically, then by length.
  words = words.sort();
  words = words.sort((a, b) => a.length - b.length);

  console.log("Parsed " + words.length + " words");

  wordsByLevel = [];

  wordsByLevel[0] = words.filter(word => word.length <= 2);
  wordsByLevel[1] = words.filter(word => word.length >= 3 && word.length <= 4);
  wordsByLevel[2] = words.filter(word => word.length >= 5 && word.length <= 7);
  wordsByLevel[3] = words.filter(word => word.length >= 8 && word.length <= 10);
  wordsByLevel[4] = words.filter(word => word.length >= 11);

  wordsByLevel.forEach((words, i) => {
    console.log (`Level ${i}: ${words.length} words`);
  });

  const csArrayStrings = wordsByLevel.map(words => `new string[] ${wordsToCsArrayString(words)}`);

  const fileData = `
class Data
{
    public static string[][] wordsByLevel = {

        ${csArrayStrings.join(',\n\n        ')}

    };
}
  `;

  fs.writeFile('Data.cs', fileData, () => console.log('File is done!'));
});

function wordsToCsArrayString(words) {
  const quotedWords = words.map(word => `"${word}"`);
return (`{ ${quotedWords.join(', ')} }`);
}