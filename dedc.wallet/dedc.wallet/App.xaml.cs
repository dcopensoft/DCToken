using DCToken.Models;
using DCToken.Services;
using DCToken.Views;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DCToken
{
    public partial class App : Application
    {

        static WalletDB wdb = null;
        public static string ethapiurl = "";
        public static string serverurl = "";
        public App()
        {

            InitializeComponent();

            MainPage = new AppShell();


        }
        public static WalletDB Wdb
        {
            get
            {
                if (wdb == null)
                {
                    wdb = new WalletDB(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "wallet.db3"));
                }
                return wdb;
            }
        }
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
