namespace UniModules.UniGame.Context.Runtime.Extension
{
    using Connections;
    using Core.Runtime.Interfaces;

    public static class ContextConnectionExtensions
    {
        public static IContextConnection ToConnector(this IContext context)
        {
            var connector = new ContextConnection();
            connector.Bind(context);
            return connector;
        }
        
    }
}
