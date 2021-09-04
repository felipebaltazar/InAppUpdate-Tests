using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace InAppUpdate
{
    public class AssemblyResolver
    {
        public static void Resolve()
        {
            var tempFilePath = Path.Combine(Path.GetTempPath(), "AppModule.dll");
            using (var client = new WebClient())
            {
                //Download da dll no github
                client.DownloadFile("https://github.com/felipebaltazar/felipebaltazar/blob/output/AppModule.dll?raw=true", tempFilePath);
            }

            // Verifica se baixou corretamente
            if (!File.Exists(tempFilePath))
                return;

            // Busca o assembly carregado atualmente
            var assembly = AppDomain.CurrentDomain
                                    .GetAssemblies()
                                    .FirstOrDefault(a => a.GetName().Name == "AppModule");

            // Utiliza a localização do assembly carregado atualmente
            var filePath = assembly.Location;

            // Busca a versão das dlls (nova e atual)
            var newAssembly = FileVersionInfo.GetVersionInfo(tempFilePath)?.FileVersion ?? "1.0.0.0";
            var currentAssembly = FileVersionInfo.GetVersionInfo(filePath)?.FileVersion ?? "1.0.0.0";

            // Compara se a versão é a mesma
            if (!(new Version(currentAssembly) < new Version(newAssembly)))
                return;

            // Sobreescreve uma dll antiga
            File.Copy(tempFilePath, filePath, true);
        }
    }
}
