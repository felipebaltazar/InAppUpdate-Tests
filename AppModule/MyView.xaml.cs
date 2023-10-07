using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AppModule
{
    public partial class MyView : ContentView
    {
        private static readonly string _basePath = Path.Combine(Path.GetTempPath());

        public MyView()
        {
            InitializeComponent();
        }

        private void button_Clicked(object sender, System.EventArgs e)
        {
            button.IsEnabled = false;
            _ = Task.Run(()=> UpdateAsync());
        }

        private async Task UpdateAsync()
        {
            using (var client = new WebClient())
            {
                
                client.DownloadProgressChanged += Client_DownloadProgressChanged;

                if(!Directory.Exists(_basePath))
                    Directory.CreateDirectory(_basePath);

                var filePath = Path.Combine(_basePath, "AppModule.dll");
                await client.DownloadFileTaskAsync("https://github.com/felipebaltazar/felipebaltazar/blob/inapp-update-tests/AppModule.dll?raw=true", filePath);
                
                client.DownloadProgressChanged -= Client_DownloadProgressChanged;
                Device.BeginInvokeOnMainThread(() => button.IsEnabled = true);
                UpdateLabel("Feito! Agora você pode reabrir o app tela para executar a versão atualizada");
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            UpdateLabel($"Baixando dados, aguarde... {e.ProgressPercentage}%");
        }

        private void UpdateLabel(string newText)
        {
            Device.BeginInvokeOnMainThread(() => label.Text = newText);
        }
    }
}