bool keepRunning = true;

while (keepRunning)
{
    Console.Write("> ");
    string input = Console.ReadLine() ?? string.Empty;

    if (!string.IsNullOrWhiteSpace(input))
    {
        if (input == "/quit")
        {
            keepRunning = false;
            return;
        }

        foreach (int i in Querier.Lookup.Terms(input))
        {
            Console.Write(i);
        }
    }
}