namespace Orc.Squirrel
{
    public class SquirrelProgressEventArgs
    {
        public SquirrelProgressEventArgs(int percentage)
        {
            Percentage = percentage;
        }

        public int Percentage { get; }
    }
}
