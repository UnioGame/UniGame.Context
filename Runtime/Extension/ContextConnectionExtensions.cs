namespace UniModules.UniGame.Context.Runtime.Extension
{
    using Connections;
    using global::UniGame.Core.Runtime;

    public static class ContextConnectionExtensions
    {
        public static IContextConnection ToConnector(this IContext context)
        {
            var connector = new ContextConnection();
            connector.Broadcast(context);
            return connector;
        }
        
    }
}
