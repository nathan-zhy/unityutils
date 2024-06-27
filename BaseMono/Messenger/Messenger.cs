using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityUtil
{
    public class CallBackException : Exception
    {
        public CallBackException(string msg)
            : base(msg)
        {
        }
    }

    public class BroadcastException : Exception
    {
        public BroadcastException(string msg)
            : base(msg)
        {
        }
    }

    public class ListenerException : Exception
    {
        public ListenerException(string msg)
            : base(msg)
        {
        }
    }

    public static class Messenger{
        static MessageTable messages = new MessageTable();
        static MessageTable permanentMessages = new MessageTable();
        static public void addListener(string eventStr, Delegate handler, bool isPermanent = false)
        {
            if (isPermanent)
            {
                permanentMessages.onListenerAdding(eventStr, handler);
            }
            else
            {
                messages.onListenerAdding(eventStr, handler);
            }
        }

        static public int removeListener(string eventStr, Delegate handler, bool isPermanent = false)
        {
            if (isPermanent)
            {
                return permanentMessages.onListenerRemoving(eventStr, handler);
            }
            return messages.onListenerRemoving(eventStr, handler);
        }
        static public int cleanListener(string eventStr, bool isPermanent = false)
        {
            if (isPermanent)
            {
                return permanentMessages.onListenerClean(eventStr);
            }
            return messages.onListenerClean(eventStr);
        }

        static public R callFunc<R>(string eventType, params object[] obj)
        {
            R res1 = permanentMessages.onCallFunc<R>(eventType,obj);
            R res2 = messages.onCallFunc<R>(eventType, obj);
            if (!res1.Equals(default(R)) && !res2.Equals(default(R)))
            {
                throw new CallBackException(string.Format("func not right{-1}."));
            }
            return res1.Equals(default(R))?res2:res1;
        }

        static public int broadcast(string eventType, params object[] obj)
        {
            int count = permanentMessages.onBroadcast(eventType, obj);
            count+=messages.onBroadcast(eventType, obj);
            return count;
        }
    }
}