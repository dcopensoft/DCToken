using DCToken.Models;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace DCToken.ViewModels
{
    public class WalletViewModel : BaseViewModel
    {
        public WalletViewModel()
        {
            Title = LanLib.LanKey["WalletView_Title"];
        }
    }
}