namespace UniModules.UniContextData.Runtime.Interfaces
{
    using UniGame.Core.Runtime.Interfaces;

    public interface IContextDataSource
    {
        void Register(IContext context);
    }
}