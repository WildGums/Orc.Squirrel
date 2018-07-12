namespace Orc.Squirrel
{
    using Catel;
    using System;

    public class SquirrelEventArgs : EventArgs
    {
        public SquirrelEventArgs(SquirrelResult result)
        {
            Argument.IsNotNull(() => result);

            SquirrelResult = result;
        }

        public SquirrelResult SquirrelResult { get; private set; }
    }
}
