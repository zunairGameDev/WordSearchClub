using System;
using FunGames.Core.Modules;

namespace FunGames.UserConsent.ATT
{
    public class FGATTCallbacks : FGModuleCallbacks
    {
        internal Action _show;

        public event Action Show
        {
            add => _show += value;
            remove => _show -= value;
        }
    }
}