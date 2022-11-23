namespace UniGame.Context.Runtime
{
    using System;
    using Core.Runtime;

    [Serializable]
    public class AssetReferenceContextValueWriter : AssetReferenceApiT<IValueWriter<IContext>>
    {
        public AssetReferenceContextValueWriter(string guid) : base(guid)
        {
        }
    }
}