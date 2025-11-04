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

        foreach (int i in Querier.Lookup.Terms(input))
        {
            Console.Write(i);
        }
    }
}