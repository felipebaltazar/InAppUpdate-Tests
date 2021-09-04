using Android.App;
using Android.Runtime;
using System;
using System.IO;
using System.Reflection;

namespace InAppUpdate.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
[Application(Debuggable = false)]
#endif
    public class MainApp : Application
    {
        public MainApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            AssemblyResolver.Init();
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }
    }
}