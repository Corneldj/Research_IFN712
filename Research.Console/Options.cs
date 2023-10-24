using CommandLine;

namespace Research.Cmd
{
    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('g', "generate", Required = false, HelpText = "Generates a file called data.csv, specify amount with `-g 1000` to randomly generate a thousand lines")]
        public int? Amount { get; set; }

        [Option('p', "path", Required = true, HelpText = "Specifies the path folder")]
        public string Path { get; set; } = null!;
    }
}
