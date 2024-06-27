// See https://aka.ms/new-console-template for more information


using System;
using System.Diagnostics;
using TestBaseMono;
using UnityUtil;


Console.WriteLine("Hello, test!");

#region
//testmessenger
TestMessager testMessager = new TestMessager();
Console.WriteLine(testMessager.addFuncfailed());
Console.WriteLine(testMessager.addActionSuc());
Console.WriteLine(Messenger.callFunc<int>("funcA", 1, 2) == 3);
Messenger.broadcast("funcB", 1, 2);
Console.WriteLine(Messenger.cleanListener("funcB") == 2);
Console.WriteLine(testMessager.cathchBroadcast());

#endregion

Console.WriteLine("Test finish!");