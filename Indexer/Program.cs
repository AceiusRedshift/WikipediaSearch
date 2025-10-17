
using Indexer;

var tokens = Parser.Parse().Select(Tokenizer.Tokenize);
