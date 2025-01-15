using System;
using UnityEngine;


namespace SOSXR.EditorTools
{
    public class GlobalExceptionHandler : MonoBehaviour
    {
        private void Awake()
        {
            // Register the global exception handler
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            Application.logMessageReceived += HandleUnityLog;
        }


        private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is NullReferenceException nullRefEx)
            {
                LogNullReferenceException(nullRefEx);
            }
        }


        private void HandleUnityLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception && logString.Contains("NullReferenceException"))
            {
                Debug.LogWarning("SOSXR: A NullReferenceException occurred in Unity's main loop!");
                // The idea is to get some additional info here
            }
        }


        private void LogNullReferenceException(NullReferenceException ex)
        {
            Debug.LogError($"SOSXR: Global NullReferenceException caught: {ex.Message} - from: {ex.Source}\n{ex.StackTrace}");
            // Add custom handling logic here (e.g., telemetry, in-game feedback, etc.)
        }


        private void OnDestroy()
        {
            // Clean up when the object is destroyed
            AppDomain.CurrentDomain.UnhandledException -= HandleUnhandledException;
            Application.logMessageReceived -= HandleUnityLog;
        }
    }
}