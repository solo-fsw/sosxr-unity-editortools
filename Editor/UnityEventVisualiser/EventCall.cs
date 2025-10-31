using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;


namespace EventVisualizer.Base
{
    [Serializable]
    public class EventCall
    {
        public NodeData nodeSender;
        public NodeData nodeReceiver;
        public readonly Object sender;
        public readonly Object receiver;
        public readonly string eventShortName;
        public readonly string eventFullName;
        public readonly string method;
        public readonly Color color;
        public readonly UnityEventBase unityEvent;

        public Action OnTriggered;

        private static Regex parenteshesPattern = new(@"\(([^\(]*)\)$");


        public EventCall(Object sender, Object receiver, string eventShortName, string eventFullName, string methodName, UnityEventBase unityEvent)
        {
            this.sender = (Component) sender ? ((Component) sender).gameObject : sender;
            this.receiver = (Component) receiver ? ((Component) receiver).gameObject : receiver;
            this.eventShortName = eventShortName;
            this.eventFullName = eventFullName;
            method = methodName;
            color = EdgeGUI.ColorForIndex(this.eventShortName);
            this.unityEvent = unityEvent;

            UpdateReceiverComponentName(receiver);
            AttachTrigger(unityEvent);
        }


        public string ReceiverComponentName { get; private set; }
        public string ReceiverComponentNameSimple { get; private set; }
        public double lastTimeExecuted { get; private set; }
        public int timesExecuted { get; private set; }

        public string MethodFullPath => ReceiverComponentName + "." + method;


        private void AttachTrigger(UnityEventBase unityEvent)
        {
            if (unityEvent == null)
            {
                return;
            }

            var eventRegisterMethod = unityEvent.GetType().GetMethod("AddListener");

            if (eventRegisterMethod != null)
            {
                var eventType = eventRegisterMethod.GetParameters()[0].ParameterType;
                var eventParameters = eventType.GetMethod("Invoke").GetParameters();

                if (eventParameters.Length == 0)
                {
                    var methodInfo = GetType()
                        .GetMethod("TriggerZeroArgs", BindingFlags.Public | BindingFlags.Instance);

                    var actionT = typeof(UnityAction);
                    var triggerAction = Delegate.CreateDelegate(actionT, this, methodInfo);

                    eventRegisterMethod.Invoke(unityEvent, new object[]
                    {
                        triggerAction
                    });
                }

                else if (eventParameters.Length == 1)
                {
                    var t0 = eventParameters[0].ParameterType;

                    var methodInfo = GetType()
                                     .GetMethod("TriggerOneArg", BindingFlags.Public | BindingFlags.Instance)
                                     .MakeGenericMethod(t0);

                    var actionT = typeof(UnityAction<>).MakeGenericType(t0);
                    var triggerAction = Delegate.CreateDelegate(actionT, this, methodInfo);

                    eventRegisterMethod.Invoke(unityEvent, new object[]
                    {
                        triggerAction
                    });
                }
                else if (eventParameters.Length == 2)
                {
                    var t0 = eventParameters[0].ParameterType;
                    var t1 = eventParameters[1].ParameterType;

                    var methodInfo = GetType()
                                     .GetMethod("TriggerTwoArgs", BindingFlags.Public | BindingFlags.Instance)
                                     .MakeGenericMethod(t0, t1);

                    var actionT = typeof(UnityAction<,>).MakeGenericType(t0, t1);
                    var triggerAction = Delegate.CreateDelegate(actionT, this, methodInfo);

                    eventRegisterMethod.Invoke(unityEvent, new object[]
                    {
                        triggerAction
                    });
                }
                else if (eventParameters.Length == 3)
                {
                    var t0 = eventParameters[0].ParameterType;
                    var t1 = eventParameters[1].ParameterType;
                    var t2 = eventParameters[2].ParameterType;

                    var methodInfo = GetType()
                                     .GetMethod("TriggerThreeArgs", BindingFlags.Public | BindingFlags.Instance)
                                     .MakeGenericMethod(t0, t1, t2);

                    var actionT = typeof(UnityAction<,,>).MakeGenericType(t0, t1, t2);
                    var triggerAction = Delegate.CreateDelegate(actionT, this, methodInfo);

                    eventRegisterMethod.Invoke(unityEvent, new object[]
                    {
                        triggerAction
                    });
                }
                else if (eventParameters.Length == 2)
                {
                    var t0 = eventParameters[0].ParameterType;
                    var t1 = eventParameters[1].ParameterType;
                    var t2 = eventParameters[2].ParameterType;
                    var t3 = eventParameters[3].ParameterType;

                    var methodInfo = GetType()
                                     .GetMethod("TriggerFourArgs", BindingFlags.Public | BindingFlags.Instance)
                                     .MakeGenericMethod(t0, t1, t2, t3);

                    var actionT = typeof(UnityAction<,,,>).MakeGenericType(t0, t1, t2, t3);
                    var triggerAction = Delegate.CreateDelegate(actionT, this, methodInfo);

                    eventRegisterMethod.Invoke(unityEvent, new object[]
                    {
                        triggerAction
                    });
                }
            }
        }


        private void UpdateReceiverComponentName(Object component)
        {
            if (receiver != null)
            {
                var matches = parenteshesPattern.Matches(component.ToString());

                if (matches != null && matches.Count == 1)
                {
                    ReceiverComponentName = matches[0].Value;
                    ReceiverComponentName = ReceiverComponentName.Substring(1, ReceiverComponentName.Length - 2);
                    var lastDot = ReceiverComponentName.LastIndexOf('.') + 1;
                    ReceiverComponentNameSimple = ReceiverComponentName.Substring(lastDot, ReceiverComponentName.Length - lastDot);
                }
            }
        }


        public override bool Equals(object obj)
        {
            var ec = (EventCall) obj;

            return null != ec && ec.unityEvent == unityEvent && receiver == ec.receiver && method == ec.method;
        }


        public override int GetHashCode()
        {
            return unityEvent == null ? 0 : unityEvent.GetHashCode() ^ (receiver == null ? 0 : receiver.GetHashCode() ^ method.GetHashCode());
        }


        #region generic callers

        public void TriggerZeroArgs()
        {
            OnExecuted();

            OnTriggered?.Invoke();
        }


        public void TriggerOneArg<T0>(T0 arg0)
        {
            OnExecuted();

            OnTriggered?.Invoke();
        }


        public void TriggerTwoArgs<T0, T1>(T0 arg0, T1 arg1)
        {
            OnExecuted();

            OnTriggered?.Invoke();
        }


        public void TriggerThreeArgs<T0, T1, T2>(T0 arg, T1 arg1, T2 arg2)
        {
            OnExecuted();

            OnTriggered?.Invoke();
        }


        public void TriggerFourArgs<T0, T1, T2, T3>(T0 arg, T1 arg1, T2 arg2, T3 arg3)
        {
            OnExecuted();

            OnTriggered?.Invoke();
        }


        private void OnExecuted()
        {
            timesExecuted++;
            lastTimeExecuted = EditorApplication.timeSinceStartup;
        }

        #endregion
    }
}