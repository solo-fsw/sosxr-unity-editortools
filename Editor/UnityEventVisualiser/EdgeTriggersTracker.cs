using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;

namespace EventVisualizer.Base
{
    public static class EdgeTriggersTracker
    {
        public static readonly float TimeToLive = 1f;
        private static readonly List<EdgeTrigger> triggers = new();


        public static void RegisterTrigger(Edge edge, EventCall eventCall)
        {
            triggers.Add(new EdgeTrigger { edge = edge, eventCall = eventCall, triggeredTime = Time.unscaledTime });
        }


        public static List<float> GetTimings(EventCall eventCall)
        {
            var now = Time.unscaledTime;
            var acceptedTriggers = triggers.FindAll(t => t.eventCall == eventCall);

            return GetTimings(acceptedTriggers);
        }


        public static List<float> GetTimings(Edge edge)
        {
            var acceptedTriggers = triggers.FindAll(t => t.edge == edge);

            return GetTimings(acceptedTriggers);
        }


        private static List<float> GetTimings(List<EdgeTrigger> acceptedTriggers)
        {
            var now = Time.unscaledTime;
            var timings = new List<float>(); //TODO cache

            foreach (var t in acceptedTriggers)
            {
                var time = Mathf.Abs(t.triggeredTime - now) / TimeToLive;

                if (time <= 1f)
                {
                    timings.Add(time);
                }
                else
                {
                    triggers.Remove(t);
                }
            }

            return timings;
        }


        public static void CleanObsolete()
        {
            var now = Time.unscaledTime;
            triggers.RemoveAll(trigger => Mathf.Abs(now - trigger.triggeredTime) > TimeToLive);
        }


        public static bool HasData()
        {
            return triggers.Count > 0;
        }


        public class EdgeTrigger
        {
            public EventCall eventCall;
            public Edge edge;
            public float triggeredTime;
        }
    }
}