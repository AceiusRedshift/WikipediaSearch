using Common;

IndexFile index = IndexFile.Load();

while (true)
{
    Console.Write("> ");
    string input = Console.ReadLine() ?? string.Empty;

    if (!string.IsNullOrWhiteSpace(input))
    {
        if (input == ":quit")
        {
            return;
        }

        foreach (string result in index.Query(input.ToTerms()))
        {
            Console.WriteLine(result);
        }
    }
}