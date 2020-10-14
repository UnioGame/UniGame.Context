namespace UniModules.UniGame.Context.SerializableContext.Runtime.Abstract
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.Interfaces;
    using UniModules.UniGame.SerializableContext.Runtime.Abstract;


    public class AsyncValueGate<TValue> :
        IAsyncContextPrototype<TValue>
    {
        private IAsyncSourceValue<TValue> _valueSource = null;
        private bool isProtected = true;
        private bool isShared = true;
        private IAsyncSourceValue<TValue> _instance;
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public AsyncValueGate(
            IAsyncSourceValue<TValue> value,
            bool isProtected = true,
            bool isShared = true)
        {
            this._valueSource = value;
            this.isProtected = isProtected;
            this.isShared = isShared;
        }

        public async UniTask<TValue> Create(IContext context)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                _instance = _instance == null &&
                            isProtected
                    ? await _valueSource.Create(context)
                    : _valueSource;

                var value = isShared && _instance.HasValue ? _instance.Value : await _instance.CreateValue(context);

                return value;
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _semaphoreSlim.Release();
            }
        }


        public void Dispose()
        {
            _instance = null;
            _valueSource = null;
            _semaphoreSlim?.Dispose();
        }
    }
}