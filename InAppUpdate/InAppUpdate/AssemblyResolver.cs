using System;
using System.IO;
using System.Reflection;

namespace InAppUpdate
{
    public class AssemblyResolver
    {
        private static readonly string _basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Modules");
        private static readonly string _tempPath = Path.Combine(Path.GetTempPath(), "Modules");

        public static void Init()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var embeddedAssembly = new AssemblyName(args.Name);
            var moduleName = $"{embeddedAssembly.Name}.dll";
            var dllLocalPath = ResolveAssembly(moduleName);

            if (string.IsNullOrWhiteSpace(dllLocalPath))
                return null;

            if (!File.Exists(dllLocalPath))
                return null;

            try
            {
                return Assembly.LoadFrom(dllLocalPath);

            }
            catch (BadImageFormatException e)
            {
                Console.WriteLine("Unable to load {0}.", dllLocalPath);
                Console.WriteLine(e.Message.Substring(0,
                                  e.Message.IndexOf(".") + 1));

                //Caso corrompa a dll no download, pegamos uma versao estavel embarcada
                try
                {
                    var localDllFile = Path.Combine(_basePath, moduleName);
                    UnpackEmbeddedReference(moduleName, dllLocalPath, true);
                    return Assembly.LoadFrom(dllLocalPath);
                }
                catch (BadImageFormatException ex)
                {
                    Console.WriteLine("Unable to load {0}.", dllLocalPath);
                    Console.WriteLine(ex.Message.Substring(0,
                                      ex.Message.IndexOf(".") + 1));
                }
            }

            return null;
        }

        private static string ResolveAssembly(string moduleName)
        {
            // Se existir versão mais nova baixada, usa ela
            var tempLocalDll = Path.Combine(_tempPath, moduleName);
            var localDllFile = Path.Combine(_basePath, moduleName);
            if (File.Exists(tempLocalDll))
                File.Copy(tempLocalDll, localDllFile, true);

            if (File.Exists(localDllFile))
                return localDllFile;

            // Caso contrário usa a versão embarcada
            UnpackEmbeddedReference(moduleName, localDllFile);

            return localDllFile;
        }

        public static void UnpackEmbeddedReference(string moduleName, string localDllFile, bool deleteIfExists = false)
        {
            var aplication = Assembly.GetExecutingAssembly().GetName().Name;
            var resourceName = $"{aplication.Replace(" ", "_")}.Modules.{moduleName}";
            using (var embedded = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
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
