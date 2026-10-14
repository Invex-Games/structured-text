using Invex.StructuredText.Samples;

// Runs every sample, or only those whose name is given on the command line:
//   dotnet run --project samples/Invex.StructuredText.Samples -- conditions ci-workflow
var selected = args.Length > 0
    ? AllSamples.Items.Where(x => args.Contains(x.Name, StringComparer.OrdinalIgnoreCase))
    : AllSamples.Items;

foreach (var sample in selected)
{
    Console.WriteLine($"## {sample.Title}");
    Console.WriteLine();
    Console.WriteLine(sample.Summary);
    Console.WriteLine();
    Console.WriteLine(sample.Run());
    Console.WriteLine();
}
