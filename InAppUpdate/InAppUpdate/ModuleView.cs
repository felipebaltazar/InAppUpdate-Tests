
using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace InAppUpdate
{
    public class ModuleView : ContentView
    {
        private const string RENDERER_PROPERTY_NAME = "Renderer";

        public static readonly BindableProperty ModuleViewNameProperty = BindableProperty.Create(
            nameof(ModuleViewName),
            typeof(string),
            typeof(ModuleView),
            null);

        public string ModuleViewName
        {
            get => (string)GetValue(ModuleViewNameProperty);
            set => SetValue(ModuleViewNameProperty, value);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == RENDERER_PROPERTY_NAME || propertyName == ModuleViewNameProperty.PropertyName)
            {
                LoadView();
            }
        }

        protected virtual void LoadView()
        {
            if(string.IsNullOrWhiteSpace(ModuleViewName))
            {
                Content = null;
                return;
            }

            var type = Type.GetType(ModuleViewName, throwOnError: false);
            if (type is null)
            {
                Content = null;
                return;
            }

            if (Content?.GetType() == type)
                return;

            Content = Activator.CreateInstance(type, new object[] { }, null) as View;
        }

        protected override void OnBindingContextChanged()
        {
            if (Content != null && !(Content is ActivityIndicator))
                Content.BindingContext = BindingContext;
        }
    }
}