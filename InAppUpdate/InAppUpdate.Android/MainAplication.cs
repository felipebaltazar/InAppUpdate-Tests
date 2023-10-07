using Android.App;
using Android.Runtime;
using System;
using System.Diagnostics;
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
        private const string MICROFRONTEND_DIR = "microfrontend";

        public MainApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Debug.WriteLine("[InAppUpdate] Assembly not found + " + args.Name);
            var assemblyToFind = new AssemblyName(args.Name);
            var moduleName = $"{assemblyToFind.Name}.dll";
            var downloadedAssembly = Path.Combine(Path.GetTempPath(), moduleName);
            var modulesDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), MICROFRONTEND_DIR);

            if (!Directory.Exists(modulesDir))
            {
                Debug.WriteLine("[InAppUpdate] Creating " + modulesDir);
                Directory.CreateDirectory(modulesDir);
            }

            var moduleFilePath = Path.Combine(modulesDir, moduleName);

            try
            {
                if (!File.Exists(downloadedAssembly) && !File.Exists(moduleFilePath))
                {
                    Debug.WriteLine("[InAppUpdate] " + downloadedAssembly + " does not exists");
                    Debug.WriteLine("[InAppUpdate] " + moduleFilePath + " does not exists");

                    return TryUnpackEmbeededAssemmbly(moduleName, moduleFilePath);
                }
                else if (File.Exists(downloadedAssembly))
                {
                    Debug.WriteLine("[InAppUpdate] moving " + downloadedAssembly);

                    File.Copy(downloadedAssembly, moduleFilePath, true);
                    File.Delete(downloadedAssembly);

                    Debug.WriteLine("[InAppUpdate] loading " + moduleFilePath);
                    return Assembly.LoadFrom(moduleFilePath);
                }
                else
                {

                    Debug.WriteLine("[InAppUpdate] No condition");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[InAppUpdate] Error");
                Debug.WriteLine("[InAppUpdate]" + ex.Message);
            }

            return TryUnpackEmbeededAssemmbly(moduleName, moduleFilePath);
        }

        private Assembly TryUnpackEmbeededAssemmbly(string moduleName, string moduleFilePath)
        {
            try
            {
                UnpackEmbeddedReference(moduleName, moduleFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[InAppUpdate]" + ex.Message);
            }

            if (!File.Exists(moduleFilePath))
                return null;

            return Assembly.LoadFrom(moduleFilePath);
        }

        public void UnpackEmbeddedReference(string moduleName, string localDllFile, bool deleteIfExists = false)
        {
            using (var embedded = Assets.Open(moduleName))
            {
                if (embedded is null)
                    return;

                if (deleteIfExists && File.Exists(localDllFile))
                    File.Delete(localDllFile);

                using (var fileStream = File.Create(localDllFile))
                {
                    embedded.CopyTo(fileStream);
                }
            }
        }
    }
}