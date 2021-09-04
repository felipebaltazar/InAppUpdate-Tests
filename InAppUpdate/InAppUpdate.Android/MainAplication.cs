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
            AssemblyResolver.Resolve();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            using (var stream = GetAssemblyStream(args.Name))
            {
                if (stream is null)
                    return null;

                var assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);

                return Assembly.Load(assemblyData);
            }
        }

        private Stream GetAssemblyStream(string name)
        {
            var tmpdir = Path.GetTempPath();
            var localDllFile = Path.Combine(tmpdir, $"{ name}.dll");
            if (File.Exists(localDllFile))
                return new FileStream(localDllFile, FileMode.Open);

            var aplication = Assembly.GetExecutingAssembly().GetName().Name;
            var embeddedAssembly = new AssemblyName(name);
            var resourceName = $"{aplication.Replace(" ", "_")}.EmbeddedAssembly.{embeddedAssembly.Name}.dll";
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }
    }
}