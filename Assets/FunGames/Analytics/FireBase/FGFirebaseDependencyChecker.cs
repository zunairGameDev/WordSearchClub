using System;
using Firebase;
using Firebase.Extensions;
using FunGames.Tools.Utils;
using UnityEngine;

namespace FunGames.Analytics.FirebaseA
{
    public class FGFirebaseDependencyChecker : Singleton<FGFirebaseDependencyChecker>
    {
        [HideInInspector] public DependencyStatus Status = DependencyStatus.UnavailableOther;
        [HideInInspector] public bool StatusChecked => _statusChecked;
        private bool _statusChecked = false;

        private Action<DependencyStatus> _onDependencyResolved;

        private bool _checkAlreadyRequested = false;

        public event Action<DependencyStatus> OnDependencyResolved
        {
            add => _onDependencyResolved += value;
            remove => _onDependencyResolved -= value;
        }

        public void StartChecking()
        {
            if (_checkAlreadyRequested) return;

            _checkAlreadyRequested = true;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    Debug.Log("Firebase dependencies resolved properly !");
                }
                else
                {
                    Debug.LogWarning($"Could not resolve all Firebase dependencies: {task.Result}");
                }

                _statusChecked = true;
                Status = task.Result;
                _onDependencyResolved?.Invoke(task.Result);
            });
        }
    }
}