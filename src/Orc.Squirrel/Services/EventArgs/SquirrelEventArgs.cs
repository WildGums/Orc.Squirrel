namespace Orc.Squirrel
{
    using Catel;
    using System;

    public class SquirrelEventArgs : EventArgs
    {
        public SquirrelEventArgs(SquirrelResult result)
        {
            ArgumentNullException.ThrowIfNull(result);

            SquirrelResult = result;
        }

        public SquirrelResult SquirrelResult { get; private set; }
    }
}
