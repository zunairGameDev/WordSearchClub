using System;
using FunGames.Core.Modules;

namespace FunGames.MMP
{
    public class FGMMPCallbacks : FGModuleCallbacks
    {
        internal Action<string> _onDeferredDeepLink;
        internal Action<FGAttributionInfo> _onAttributionChanged;
        
        public event Action<string> OnDeferredDeepLink
        {
            add => _onDeferredDeepLink += value;
            remove => _onDeferredDeepLink -= value;
        }
        
        public event Action<FGAttributionInfo> OnAttributionChanged
        {
            add => _onAttributionChanged += value;
            remove => _onAttributionChanged -= value;
        }
    }
}