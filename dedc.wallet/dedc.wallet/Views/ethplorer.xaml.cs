 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCToken.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ethplorer : ContentPage
    {
        public ethplorer()
        {
            InitializeComponent();
    
        }

        void webviewNavigating(object sender, WebNavigatingEventArgs e)
        {
            labelLoading.IsEnabled = true;
            labelLoading.IsRefreshing = true;
        }

        void webviewNavigated(object sender, WebNavigatedEventArgs e)
        {
            labelLoading.IsRefreshing = false;
            labelLoading.IsEnabled = false;
        }

    
    }
}