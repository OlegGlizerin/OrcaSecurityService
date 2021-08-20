namespace OrcaSecurityService
{
    public class ConsoleArgs
    {
        private string _pathToInputFile { get; set; }

        public ConsoleArgs(string[] args)
        {
            _pathToInputFile = args[0];
        }
    }
}
