
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

  const easyWords = words.filter(word => word.length <= 6);
  const mediumWords = words.filter(word => word.length >= 6 && word.length <= 10);
  const hardWords = words.filter(word => word.length >= 10);

  console.log(easyWords.length + ' easy words');
  console.log(mediumWords.length + ' medium words');
  console.log(hardWords.length + ' hard words');

  fs.writeFile('easy-words.json', JSON.stringify(easyWords), () => console.log('easy-words.json finished!'));
  fs.writeFile('medium-words.json', JSON.stringify(mediumWords), () => console.log('medium-words.json finished!'));
  fs.writeFile('hard-words.json', JSON.stringify(hardWords), () => console.log('hard-words.json finished!'));
});