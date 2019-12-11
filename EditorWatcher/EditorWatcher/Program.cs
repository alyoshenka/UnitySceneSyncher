#define UNITY_EDITOR

using System;

namespace EditorWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("boop");
#if UNITY_ENGINE_AVAILABLE
                       Console.WriteLine(EditorWindow.focusedWindow);
#endif
            Console.ReadLine();
        }
    }
}
