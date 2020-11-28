using DCToken.Models;
using DCToken.ViewModels;
using DCToken.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DCToken
{
    public partial class AppShell : Xamarin.Forms.Shell 
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ethplorer), typeof(ethplorer));
            Routing.RegisterRoute(nameof(WalletPage), typeof(WalletPage));
            LanLib.SetLan();

          
        }

    }
}
