using DCToken.Models;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DCToken.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        string title2 = string.Empty;
        public string Title2
        {
            get { return title2; }
            set { base.SetProperty(ref title2, value); }
        }
        string title3 = string.Empty;
        public string Title3
        {
            get { return title3; }
            set { base.SetProperty(ref title3, value); }
        }
        public AboutViewModel()
        {

            Title = LanLib.LanKey["AboutView_Title"];
            Title2 = LanLib.LanKey["AboutView_Title2"];
            Title3 = LanLib.LanKey["AboutView_Title3"];

            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://github.com/dcopensoft/DCToken"));

            SetENLanCommand = new Command(async () => {
                LanLib.SetLan();
                Title = LanLib.LanKey["AboutView_Title"];
                Title2 = LanLib.LanKey["AboutView_Title2"];
                Title3 = LanLib.LanKey["AboutView_Title3"];
            }); ;

            SetCNLanCommand = new Command(async () => {
                LanLib.SetLan("cn");
                Title = LanLib.LanKey["AboutView_Title"];
                Title2 = LanLib.LanKey["AboutView_Title2"];
                Title3 = LanLib.LanKey["AboutView_Title3"];
            }); ;


        }

        public ICommand OpenWebCommand { get; }
        public ICommand SetENLanCommand { get; }

        public ICommand SetCNLanCommand { get; }
    }
}