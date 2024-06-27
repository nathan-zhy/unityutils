using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityUtil;

namespace TestBaseMono
{
    internal class TestMessager
    {
        public bool addFuncfailed()
        {
            try
            {
                Messenger.addListener("funcA", add);
                Messenger.addListener("funcA", add);
            } catch(ListenerException e)
            {
                Console.Write(e.Message);
                return true;
            }
            return false;
        }
        public bool addActionSuc()
        {
            try
            {
                Messenger.addListener("funcB", add1);
                Messenger.addListener("funcB", add1);
            }
            catch (ListenerException e)
            {
                Console.Write(e.Message);
                return false;
            }
            return true;
        }
        public bool cathchBroadcast() {
            try
            {
                Messenger.callFunc<int>("funcB", 1, 2);
            }
            catch(CallBackException e)
            {
                Console.Write(e.Message);
                return true;
            }
            return false;
        }


        int add(int a, int b) {
            return a + b;
        }
        void add1(int a, int b)
        {
            Console.WriteLine("a:{0},b:{1},count:{2}", a, b, a + b + 1);
        }
    }
}
