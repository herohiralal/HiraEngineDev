using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace UnityEngine.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct TimerHandle
    {
        internal TimerHandle(ushort bufferIndex, IntPtr owner, ulong hash)
        {
            BufferIndex = bufferIndex;
            Owner = owner;
            Hash = hash;
        }

        public readonly ushort BufferIndex;
        public readonly IntPtr Owner;
        public readonly ulong Hash;
    }

    public struct NativeGameplayCommandBuffer
    {
        [SuppressUnmanagedCodeSecurity, DllImport(HiraNativeHook.HIRA_ENGINE_NATIVE_DLL_NAME, CallingConvention = HiraNativeHook.CALLING_CONVENTION)]
        private static extern IntPtr CreateGameplayCommandBuffer(ushort startingBufferSize);

        [SuppressUnmanagedCodeSecurity, DllImport(HiraNativeHook.HIRA_ENGINE_NATIVE_DLL_NAME, CallingConvention = HiraNativeHook.CALLING_CONVENTION)]
        private static extern void DestroyGameplayCommandBuffer(IntPtr target);

        [SuppressUnmanagedCodeSecurity, DllImport(HiraNativeHook.HIRA_ENGINE_NATIVE_DLL_NAME, CallingConvention = HiraNativeHook.CALLING_CONVENTION)]
        private static extern TimerHandle GameplayCommandBufferSetTimer(IntPtr target, float timer);

        [SuppressUnmanagedCodeSecurity, DllImport(HiraNativeHook.HIRA_ENGINE_NATIVE_DLL_NAME, CallingConvention = HiraNativeHook.CALLING_CONVENTION)]
        private static extern byte GameplayCommandBufferIsHandleValid(IntPtr target, [In] in TimerHandle handle);

        [SuppressUnmanagedCodeSecurity, DllImport(HiraNativeHook.HIRA_ENGINE_NATIVE_DLL_NAME, CallingConvention = HiraNativeHook.CALLING_CONVENTION)]
        private static extern float GameplayCommandBufferGetTimeRemaining(IntPtr target, [In] in TimerHandle handle);

        [SuppressUnmanagedCodeSecurity, DllImport(HiraNativeHook.HIRA_ENGINE_NATIVE_DLL_NAME, CallingConvention = HiraNativeHook.CALLING_CONVENTION)]
        private static extern void GameplayCommandBufferPauseTimer(IntPtr target, [In] in TimerHandle handle);

        [SuppressUnmanagedCodeSecurity, DllImport(HiraNativeHook.HIRA_ENGINE_NATIVE_DLL_NAME, CallingConvention = HiraNativeHook.CALLING_CONVENTION)]
        private static extern void GameplayCommandBufferResumeTimer(IntPtr target, [In] in TimerHandle handle);

        [SuppressUnmanagedCodeSecurity, DllImport(HiraNativeHook.HIRA_ENGINE_NATIVE_DLL_NAME, CallingConvention = HiraNativeHook.CALLING_CONVENTION)]
        private static extern void GameplayCommandBufferCancelTimer(IntPtr target, [In] in TimerHandle handle);

        [SuppressUnmanagedCodeSecurity, DllImport(HiraNativeHook.HIRA_ENGINE_NATIVE_DLL_NAME, CallingConvention = HiraNativeHook.CALLING_CONVENTION)]
        private static extern void InitGameplayCommandBufferExecuteBufferedCommands(Action<IntPtr, IntPtr> executor);

        [AOT.MonoPInvokeCallback(typeof(Action<IntPtr, IntPtr>))]
        private static unsafe void ExecuteBufferedCommands(IntPtr buffer, IntPtr values)
        {
            var value = (ushort*) values;
            var count = *value;
            value++;
            for (var i = 0; i < count; i++)
            {
                var bufferIndex = *(value+i);
                if (ACTIONS.ContainsKey(bufferIndex))
                {
                    ACTIONS[bufferIndex]?.Invoke();
                    ACTIONS.Remove(bufferIndex);
                }
            }
        }

        public static readonly Dictionary<ushort, Action> ACTIONS = new Dictionary<ushort, Action>();

        private NativeGameplayCommandBuffer(IntPtr target) => _target = target;

        private IntPtr _target;
        public bool IsValid => _target != IntPtr.Zero;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeGameplayCommandBuffer Create(ushort startingBufferSize) =>
            new NativeGameplayCommandBuffer(CreateGameplayCommandBuffer(startingBufferSize));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy()
        {
            DestroyGameplayCommandBuffer(_target);
            _target = IntPtr.Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TimerHandle SetTimer(Action action, float timer)
        {
            var handle = GameplayCommandBufferSetTimer(_target, timer);
            if (ACTIONS.ContainsKey(handle.BufferIndex)) ACTIONS[handle.BufferIndex] = action;
            else ACTIONS.Add(handle.BufferIndex, action);
            return handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsHandleValid(in TimerHandle handle) => GameplayCommandBufferIsHandleValid(_target, in handle) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetTimeRemaining(in TimerHandle handle) => GameplayCommandBufferGetTimeRemaining(_target, in handle);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PauseTimer(in TimerHandle handle) => GameplayCommandBufferPauseTimer(_target, in handle);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResumeTimer(in TimerHandle handle) => GameplayCommandBufferResumeTimer(_target, in handle);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CancelTimer(in TimerHandle handle)
        {
            if (ACTIONS.ContainsKey(handle.BufferIndex))
                ACTIONS.Remove(handle.BufferIndex);

            GameplayCommandBufferCancelTimer(_target, in handle);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void OnLoad()
        {
            ACTIONS.Clear();
            
            HiraNativeHook.OnNativeHookCreated -= OnNativeHookCreated;
            HiraNativeHook.OnNativeHookCreated += OnNativeHookCreated;
        }

        public static NativeGameplayCommandBuffer? Instance { get; private set; }

        private static void OnNativeHookCreated()
        {
            HiraNativeHook.OnNativeHookCreated -= OnNativeHookCreated;
            
            Instance = Create(20);
            
            InitGameplayCommandBufferExecuteBufferedCommands(ExecuteBufferedCommands);
            HiraNativeHook.OnNativeHookDestroyed -= OnNativeHookDestroyed;
            HiraNativeHook.OnNativeHookDestroyed += OnNativeHookDestroyed;
        }

        private static void OnNativeHookDestroyed()
        {
            HiraNativeHook.OnNativeHookDestroyed -= OnNativeHookDestroyed;
            
            Instance?.Destroy();
            Instance = null;
        }
    }
}