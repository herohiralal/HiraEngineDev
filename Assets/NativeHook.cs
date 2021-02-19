using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine
{
    using Internal;

    [HiraManager]
    public class NativeHook : MonoBehaviour
    {
        private static NativeUnityHook _nativeHook = default;
        private static bool _initialized = false;
        public const string HIRA_ENGINE_NATIVE_DLL_NAME = "HiraEngine-Native";

        [RuntimeInitializeOnLoadMethod]
        private static void OnInitialize()
        {
            if (_initialized)
            {
                _initialized = false;
                Application.quitting -= OnQuit;

                if (_nativeHook.IsValid) _nativeHook.Destroy();
            }

            _initialized = true;
            Application.quitting += OnQuit;

            InitDebugLogToUnity(LogToUnity);
            _nativeHook = NativeUnityHook.Create();
        }

        private void Update()
        {
            if (_nativeHook.IsValid) _nativeHook.Update(Time.deltaTime);
        }

        private static void OnQuit()
        {
            _initialized = false;
            Application.quitting -= OnQuit;
            
            if (_nativeHook.IsValid) _nativeHook.Destroy();
        }

        [DllImport(HIRA_ENGINE_NATIVE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern void InitDebugLogToUnity(Action<string> logger);

        private static void LogToUnity(string message) => Debug.Log(message);
    }

    namespace Internal
    {
        internal struct NativeUnityHook
        {
            [DllImport(NativeHook.HIRA_ENGINE_NATIVE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr CreateUnityHook();

            [DllImport(NativeHook.HIRA_ENGINE_NATIVE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void DestroyUnityHook(IntPtr target);

            [DllImport(NativeHook.HIRA_ENGINE_NATIVE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
            private static extern void UnityHookUpdate(IntPtr target, float deltaTime);

            public bool IsValid { get; private set; }
            private IntPtr _target;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static NativeUnityHook Create() =>
                new NativeUnityHook {_target = CreateUnityHook(), IsValid = true};

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Destroy()
            {
                DestroyUnityHook(_target);
                _target = default;
                IsValid = false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Update(float deltaTime) => UnityHookUpdate(_target, deltaTime);
        }
    }
}