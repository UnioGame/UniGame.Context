using UnityEngine;

namespace UniModules.UniGame.SerializableContext.Examples.TypeContexts
{
    using Runtime;
    using TMPro;
    using global::UniGame.Context.Runtime;
    using UniRx;

    public class DemoContextValueView : MonoBehaviour
    {
        public IntContextValue intContextValue;

        public TextMeshProUGUI intText;
        
        private void Start()
        {
            intContextValue.
                Subscribe(x => intText.text = x.ToString()).
                AddTo(this);
        }
    }
}
