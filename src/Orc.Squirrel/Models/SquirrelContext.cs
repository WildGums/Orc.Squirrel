namespace Orc.Squirrel
{
    public class SquirrelContext
    {
        public SquirrelContext()
        {
            ChannelName = string.Empty;
        }

        /// <summary>
        /// The channel name. If <c>null</c>, the channel name will be used from the configuration.
        /// </summary>
        public string ChannelName { get; set; }
    }
}
