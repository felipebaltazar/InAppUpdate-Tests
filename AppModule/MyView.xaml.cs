using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AppModule
{
    public partial class MyView : ContentView
    {
        public MyView()
        {
            InitializeComponent();
        }
    }
}