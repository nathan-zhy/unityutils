using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnityUtil
{
    internal class MessageTable
    {
        Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
        public void cleanAll()
        {
            eventTable.Clear();
        }
        public string toString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\t\t\t=== MESSENGER PrintEventTable ===");
            foreach (KeyValuePair<string, Delegate> pair in eventTable)
            {
                sb.Append("\t\t\t" + pair.Key + "\t\t" + pair.Value.GetInvocationList().Length);
            }
            return sb.ToString();
        }

        public int onListenerAdding(string eventType, Delegate listener)
        {
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, listener);
                return 1;
            }

            Delegate d = eventTable[eventType];
            int objres = objsIsMatch(d.Method.GetParameters(), listener.Method.GetParameters());
            if (objres != -1)
            {
                throw new ListenerException(string.Format("param index:{0} not match.", objres));
            }
            if (d != null && (listener.Method.ReturnType != typeof(void) || d.Method.ReturnType != typeof(void)))
            {
                throw new ListenerException(string.Format("Func already have call."));
            }
            eventTable[eventType] = Delegate.Combine(eventTable[eventType], listener);
            return eventTable[eventType].GetInvocationList().Length;
        }

        public int onListenerRemoving(string eventType, Delegate listener)
        {

            if (!eventTable.ContainsKey(eventType)) return 0;

            Delegate d = eventTable[eventType];

            if (d == null)
            {
                throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
            }
            else if (d.GetType() != listener.GetType())
            {
                throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listener.GetType().Name));
            }
            d = Delegate.Remove(d, listener);
            return d.GetInvocationList().Length;
        }

        public int onListenerClean(string eventType)
        {
            if (!eventTable.ContainsKey(eventType)) return 0;
            int res = eventTable[eventType].GetInvocationList().Length;
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
            return res;
        }

        //using
        public R onCallFunc<R>(string eventType, params object[] obj)
        {
            Delegate d;
            if (!eventTable.TryGetValue(eventType, out d)) return default(R);

            if (d == null) return default(R);
            if (d.Method.ReturnType == typeof(void))
            {
                throw new CallBackException(string.Format("Using broadcast for this message."));
            }
            if (d.GetInvocationList().Length > 1)
            {
                throw new CallBackException(string.Format("Func have {i} call now.", d.GetInvocationList().Length));
            }

            int objres = objsIsMatch(d.Method.GetParameters(), obj);
            if (objres != -1)
            {
                throw new CallBackException(string.Format("Param index:{0} not match.", objres));
            }

            object res = d.DynamicInvoke(obj);
            if (res.GetType() != d.Method.ReturnType) {
                throw new CallBackException("Return type not match");
            }
            return (R)res;
        }

        public int onBroadcast(string eventType, params object[] obj)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d)) return 0;

            if (d == null) return 0;
            int objres = objsIsMatch(d.Method.GetParameters(), obj);
            if (objres != -1)
            {
                throw new BroadcastException(string.Format("Param index:{0} not match.", objres));
            }
            d.DynamicInvoke(obj);
            return d.GetInvocationList().Length;
        }

        int objsIsMatch(ParameterInfo[] obj1, object[] obj2)
        {
            if (obj1.Length != obj2.Length) return -2;
            for (int i = 0; i < obj1.Length; i++)
            {
                Type t = obj2[i].GetType().BaseType== typeof(ParameterInfo) ? 
                    ((ParameterInfo)obj2[i]).ParameterType: obj2[i].GetType();
                if (obj1[i].ParameterType != t)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
