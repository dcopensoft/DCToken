using DCToken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.KeyStore;
using Nethereum.KeyStore.Model;
using Nethereum.Signer;
using DCToken.Services;
using System.Net.Http;
using Newtonsoft.Json;
using Nethereum.Hex.HexTypes;
using System.IO;
using Xamarin.Essentials;
using ZXing.Net.Mobile.Forms;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Util;
using System.Numerics;

namespace DCToken.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WalletPage : ContentPage
    {
        static ScryptParams scryptParams = new ScryptParams() { Dklen = 32, N = 4096, R = 8, P = 6 };
        private List<dcinfo> dclist = new List<dcinfo>();
        public WalletPage()
        {
            App.ethapiurl = "";
            InitializeComponent();
            this.Appearing += WalletPage_Appearing;
            this.Disappearing += WalletPage_Disappearing;

            try
            {

                Uri uri = new Uri(App.serverurl + "/home/dclist");
                HttpClient client = new HttpClient();
                StringContent content = new StringContent("{}", Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(uri, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    dclist = JsonConvert.DeserializeObject<List<dcinfo>>(result);
                    if (dclist != null && dclist.Count > 0)
                    {
                        selectcion.ItemsSource = null;
                        foreach (var item in dclist)
                            selectcion.Items.Add(item.symbol.ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("", ex.Message, "ok");
            }
        }
        private string selcionflag = "eth";
        private string contractAddress = "";
        private short decimals = 18;
        async void WalletBind()
        {
            try
            {
                
                List< WalletInfo> list = await App.Wdb.GetWalletListAsync();
                if (list != null)
                {
                    HttpClient client = new HttpClient();
                    foreach (var item in list)
                    {
                        item.Balance = 0;
                        Uri uri = new Uri(App.ethapiurl);
                        if (selcionflag.ToLower() == "eth")
                        {
                            //Nethereum.Web3.Web3 web3 = new Nethereum.Web3.Web3(App.ethapiurl);

                            //var money_bigint =    web3.Eth.GetBalance.SendRequestAsync(item.Address).Result;
                            //item.Balance = Nethereum.Util.UnitConversion.Convert.FromWei(money_bigint.Value);

                            string json = "{\"jsonrpc\":\"2.0\",\"method\":\"eth_getBalance\",\"params\": [\"" + item.Address + "\", \"latest\"],\"id\":1}";
                            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                            HttpResponseMessage response = client.PostAsync(uri, content).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                string result = response.Content.ReadAsStringAsync().Result;
                                var resultJson = JsonConvert.DeserializeObject<dynamic>(result);
                                string money_hex = resultJson.result;
                                HexBigInteger money_bigint = new HexBigInteger(money_hex);
                                item.Balance = Nethereum.Util.UnitConversion.Convert.FromWei(money_bigint.Value);
                            }
                        }
                        else
                        {
                            string queryaddress = item.Address;
                            string data = EthHelper.CallContractFunData("balanceOf(address)", new List<object> { queryaddress });
                          
                            string json = "{\"jsonrpc\":\"2.0\",\"method\":\"eth_call\",\"params\": [{\"from\": \"" + queryaddress + "\",\"to\": \"" + contractAddress + "\",\"data\": \"" + data + "\"}, \"latest\"],\"id\":1}";
                            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                            HttpResponseMessage response = client.PostAsync(uri, content).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                string result = response.Content.ReadAsStringAsync().Result;
                                var resultJson = JsonConvert.DeserializeObject<dynamic>(result);
                                string money_hex = resultJson.result;
                                HexBigInteger money_bigint = new HexBigInteger(money_hex);

                                item.Balance = Nethereum.Util.UnitConversion.Convert.FromWei(money_bigint, int.Parse(decimals.ToString()));// Nethereum.Util.UnitConversion.Convert.FromWei(money_bigint.Value);
                            }

                        }
                    }
                }
                walletview.ItemsSource = list;
            }
            catch (Exception ex)
            {
                DisplayAlert("Exception!", "Try again later " + ex.Message, "OK");
            }
        }
        private void SelectNetCion()
        {
            var item = dclist.Find(f => f.symbol.ToLower() == selcionflag.ToLower());
            if (item != null)
            {
                if (selectnet.SelectedItem == null || selectnet.SelectedItem.ToString().ToLower() == "mainnet")
                    contractAddress = item.contractAddress;
                else
                    contractAddress = item.contractAddress1;

                decimals = item.decimals;
            }
            else
            {
                if (selcionflag.ToLower() == "dedc")
                {
                    contractAddress = "0x9983a1803b9D50B498473B3De387f2e2a15c772C";

                    decimals = 18;
                }
                else if (selcionflag.ToLower() == "usdt")
                {
                    contractAddress = "0xdac17f958d2ee523a2206206994597c13d831ec7";

                    decimals = 6;
                }
                else
                {
                    decimals = 18;
                }
            }
        }
 
        private async void SetPermission()
        {
            // Granted storage permission
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (storageStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                storageStatus = results[Permission.Storage];
            }

        }

        private void WalletPage_Disappearing(object sender, EventArgs e)
        {
            try
            {
                Uri uri = new Uri(App.serverurl + "/home/dclist");
                HttpClient client = new HttpClient();
                StringContent content = new StringContent("{}", Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(uri, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    dclist = JsonConvert.DeserializeObject<List<dcinfo>>(result);
                    if (dclist != null && dclist.Count > 0)
                    {
                        selectcion.ItemsSource = null;
                        if (selectcion.Items != null)
                            selectcion.Items.Clear();
                        foreach (var item in dclist)
                            selectcion.Items.Add(item.symbol.ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("", ex.Message, "ok");
            }
        }

        private void WalletPage_Appearing(object sender, EventArgs e)
        {

            Title = LanLib.LanKey["WalletView_Title"];
            WalletBind();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

             SetPermission();
        }

        async void OnSaveWalletClicked(object sender, EventArgs e)
        {
            txtpwd.Text = txtpwd.Text.Trim();
            if (txtpwd.Text.Trim() == "" || txtpwd.Text.Length < 8)
            {
                await DisplayAlert("Password Error!", "Password length cannot be less than 8", "OK");
                return;
            }
            (sender as Button).IsEnabled = false;
            try
            {

                bool answer = await DisplayAlert("Password Confirm!", "Password:" + txtpwd.Text, "Yes", "No");
                if (answer)
                {
                    try
                    {
                        WalletInfo wi = new WalletInfo();
                        var ecKey = EthECKey.GenerateKey();
                        wi.PublicKey = HexByteConvertorExtensions.ToHex(ecKey.GetPubKey());
                        wi.PrivateKey = ecKey.GetPrivateKey();

                        wi.Address = ecKey.GetPublicAddress().ToLower();
                        var scryptService = new KeyStoreScryptService();
                        wi.FJson = scryptService.EncryptAndGenerateKeyStoreAsJson(txtpwd.Text, ecKey.GetPrivateKeyAsBytes()
                            , wi.Address.Replace("0x", ""), scryptParams);
                        await App.Wdb.SaveWalletAsync(wi);
                       
                        txtpwd.Text = "";
                        WalletBind();
                        await DisplayAlert("Tips", "Success", "OK");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Exception!", "Try again later " + ex.Message, "OK");
                    }
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception!", "Try again later " + ex.Message, "OK");
            }
             (sender as Button).IsEnabled = true;
        }

        async void OnSelectChange(object sender, EventArgs e)
        {
            var picker = (sender as Picker);
            if (picker.SelectedItem != null)
            {
                if (selcionflag != picker.SelectedItem.ToString())
                {
                    stqrcodeview.IsVisible = false;
                    sendtran.IsVisible = false;
                    tranloading.IsVisible = sendtran.IsVisible;
                    selcionflag = picker.SelectedItem.ToString();
                    SelectNetCion();
                    WalletBind();
                }
            }
        }

        async void OnSelectNetChange(object sender, EventArgs e)
        {
            var picker = (sender as Picker);
            if (picker.SelectedItem != null)
            {
                stqrcodeview.IsVisible = false;
                sendtran.IsVisible = false;
                tranloading.IsVisible = sendtran.IsVisible;

                if (picker.SelectedItem.ToString().ToLower() == "mainnet")
                    App.ethapiurl = "https://mainnet.infura.io/v3/";
                else
                    App.ethapiurl = "https://ropsten.infura.io/v3/";

                SelectNetCion();

                WalletBind();

            }
        }

        async void OnDelWalletClicked(object sender, EventArgs e)
        {
            try
            {
                bool answer = await DisplayAlert("Question?", "Are you delete it?", "Yes", "No");
                if (answer)
                {
                    ImageButton but = (sender as ImageButton);
                    var walletid = but.CommandParameter;
                    var walletinfo = App.Wdb.GetWalletAsync(int.Parse(walletid.ToString())).Result;
                    if (walletinfo != null)
                    {
                        await App.Wdb.DeleteWalletAsync(walletinfo);
                        WalletBind();
                    }

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception!", "Try again later " + ex.Message, "OK");
            }
        }
        async void OnImportWalletClicked(object sender, EventArgs e)
        {
            try
            {

                var result = await Plugin.FilePicker.CrossFilePicker.Current.PickFile();
                if (result != null)
                {
                  
                    var stream = result.GetStream();
                    StreamReader reader = new StreamReader(stream);
                    string str = reader.ReadLine();
                    var resultJson = JsonConvert.DeserializeObject<dynamic>(str);
                    string address = "0x" + resultJson.address;
                    if (address.Length > 20)
                    {
                       var wallet= App.Wdb.GetWalletAsyncByAddress(address).Result;
                        if (wallet == null)
                        {
                            WalletInfo wi = new WalletInfo();

                            wi.PublicKey = "";
                            wi.PrivateKey = "";

                            wi.Address = address;
                            var scryptService = new KeyStoreScryptService();
                            wi.FJson = str;
                            await App.Wdb.SaveWalletAsync(wi);
                            await DisplayAlert("Tips", "Success", "OK");
                            WalletBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
                await DisplayAlert("Exception!", "Try again later " + ex.Message, "OK");
            }
        }

        async void OnExportWalletClicked(object sender, EventArgs e)
        {
            try
            {
                Button but = (sender as Button);
                var walletid = but.CommandParameter;
                if (walletid != null)
                {
                    var item=App.Wdb.GetWalletAsync(int.Parse(walletid.ToString())).Result;
                    if (item != null)
                    {
                        var backingDir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments));
                        var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), item.Address+".json");
                        bool an = await DisplayAlert("Tips", "Select folder to save?", "Yes", "No");
                        if (an)
                        {
                            var result = await Plugin.FilePicker.CrossFilePicker.Current.PickFile();
                            if (result != null)
                            {
                                backingDir = result.FilePath.Replace(result.FileName, "");
                                backingFile = Path.Combine(backingDir, item.Address);
                                using (var writer = File.CreateText(backingFile))
                                {
                                    await writer.WriteLineAsync(item.FJson);
                                }

                            }
                        }
                        else
                        {
                            using (var writer = File.CreateText(backingFile))
                            {
                                await writer.WriteLineAsync(item.FJson);
                            }
                        }

                        await DisplayAlert("Tips", "At:" + backingDir, "OK");
 
                    }
                }

            }
            catch(Exception ex)
            {
                await DisplayAlert("Exception!", "Try again later " + ex.Message, "OK");
            }
        }

        async void OnClipboardWalletClicked(object sender, EventArgs e)
        {
            try
            {
                ImageButton but = (sender as ImageButton);
                var address = but.CommandParameter;
                if (address != null)
                {
                    await Clipboard.SetTextAsync(address.ToString());
                    await DisplayAlert("Tips", "Copied to clipboard", "OK");
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception!", "Try again later " + ex.Message, "OK");
            }
        }

        async void OnQrcodeWalletClicked(object sender, EventArgs e)
        {
            try
            {
                sendtran.IsVisible = false;
                tranloading.IsVisible = sendtran.IsVisible;
                if (stqrcodeview.IsVisible)
                {
                    stqrcodeview.IsVisible = false;
                    return;
                }
                ImageButton but = (sender as ImageButton);
                var address = but.CommandParameter;
                if (address != null)
                {
                    var barcode = new ZXingBarcodeImageView
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand
                    };
                    barcode.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
                    barcode.BarcodeOptions.Width = 380;
                    barcode.BarcodeOptions.Height = 380;
                    barcode.BarcodeOptions.Margin = 5;
                    barcode.BarcodeValue = address.ToString();
                    QrCodeSite.Children.Add(barcode);

                    stqrcodeview.IsVisible = true;

                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception!", "Try again later " + ex.Message, "OK");
            }
        }

        async void OnHideQrcodeWalletClicked(object sender, EventArgs e)
        {
            try
            {
                stqrcodeview.IsVisible = false;

            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception!", "Try again later " + ex.Message, "OK");
            }
        }

        async void OnHideTranClicked(object sender, EventArgs e)
        {
            try
            {
                sendtran.IsVisible = false;
                tranloading.IsVisible = sendtran.IsVisible;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception!", "Try again later " + ex.Message, "OK");
            }
        }

        async void OnViewTranClicked(object sender, EventArgs e)
        {
            try
            {
                stqrcodeview.IsVisible = false;

                if (sendtran.IsVisible)
                {
                    sendtran.IsVisible = false;
                    tranloading.IsVisible = sendtran.IsVisible;
                    return;
                }
                Button but = (sender as Button);
                var address = but.CommandParameter;
                if (address != null)
                {
                    addfrom.Text = address.ToString();

                    sendtran.IsVisible = true;
                    tranloading.IsVisible = sendtran.IsVisible;
                    System.Numerics.BigInteger e_gas = 0;
                    Uri uri = new Uri(App.ethapiurl);
                    HttpClient client = new HttpClient();
                    string json = "";
                    StringContent content = new StringContent("");
                    HttpResponseMessage response = null;
                    addpwd.Text = "";
                    addto.Text = "";
                    tranamount.Text = "";
                    traegas.Text = "";
                    tranprice.Text = "";
                    tranprice.Text = Nethereum.Web3.Web3.Convert.ToWei(50, UnitConversion.EthUnit.Gwei).ToString();
                    if (selcionflag.ToLower() == "eth")
                    {
                        json = "{\"jsonrpc\":\"2.0\",\"id\":\"0\",\"method\":\"eth_estimateGas\",\"params\":[{\"from\":\"" + address.ToString() + "\",\"to\":\"" + address.ToString() + "\"}]}";
                        content = new StringContent(json, Encoding.UTF8, "application/json");
                        response = client.PostAsync(uri, content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string result = response.Content.ReadAsStringAsync().Result;
                            var resultJson = JsonConvert.DeserializeObject<dynamic>(result);
                            string money_hex = resultJson.result;
                            HexBigInteger money_bigint = new HexBigInteger(money_hex);
                            e_gas = money_bigint.Value+10000;
                        }
                    }
                    else
                    {
                        string queryaddress = address.ToString();
                        string data = EthHelper.CallContractFunData("balanceOf(address)", new List<object> { queryaddress });
                       
                        json = "{\"jsonrpc\":\"2.0\",\"id\":\"0\",\"method\":\"eth_estimateGas\",\"params\":[{\"from\":\"" + queryaddress + "\",\"to\":\"" + contractAddress + "\",\"data\":\"" + data + "\"}]}";
                        content = new StringContent(json, Encoding.UTF8, "application/json");
                        response = client.PostAsync(uri, content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string result = response.Content.ReadAsStringAsync().Result;
                            var resultJson = JsonConvert.DeserializeObject<dynamic>(result);
                            string money_hex = resultJson.result;
                            HexBigInteger money_bigint = new HexBigInteger(money_hex);
                            e_gas = money_bigint.Value+50000;
                        }
                    }

                    if (e_gas > 0)
                    {
                        traegas.Text = e_gas.ToString();
                        json = "{\"jsonrpc\":\"2.0\",\"id\":\"0\",\"method\":\"eth_gasPrice\",\"params\":[]}";
                        content = new StringContent(json, Encoding.UTF8, "application/json");
                        response = client.PostAsync(uri, content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string result = response.Content.ReadAsStringAsync().Result;
                            var resultJson = JsonConvert.DeserializeObject<dynamic>(result);
                            string money_hex = resultJson.result;
                            HexBigInteger money_bigint = new HexBigInteger(money_hex);
                           
                            decimal eprice = Math.Round(Nethereum.Util.UnitConversion.Convert.FromWei(money_bigint.Value * e_gas), 8);
                            egas.Text = "fee:" + eprice;// + (decimal.Parse((e_gas + 10000).ToString()) * eprice);
                            tranprice.Text = money_bigint.Value.ToString();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception!", "Try again later " + ex.Message, "OK");
            }
        }

        async void OnSendTranClicked(object sender, EventArgs e)
        {
            try
            {
                if (addfrom.Text == null || addfrom.Text.Trim() == "" || !addfrom.Text.StartsWith("0x"))
                {
                    await DisplayAlert("Error!", "Wallet address is error.", "OK");
                    return;
                }
                if (addto.Text == null || addto.Text.Trim() == "" || !addto.Text.StartsWith("0x"))
                {
                    await DisplayAlert("Error!", "Wallet address is error.", "OK");
                    return;
                }
                if (addpwd.Text == null || addpwd.Text.Trim() == "")
                {
                    await DisplayAlert("Error!", "Input password.", "OK");
                    return;
                }
                if (decimal.Parse(tranamount.Text.Trim()) <= 0)
                {
                    await DisplayAlert("Error!", "Wrong transaction amount.", "OK");
                    return;
                }
                sendbut.IsEnabled = false;
                tranloading.IsRefreshing = true;
                var winfo=App.Wdb.GetWalletAsyncByAddress(addfrom.Text.Trim()).Result;
                try
                {
                    Account account = Account.LoadFromKeyStore(winfo.FJson, addpwd.Text.Trim());
                    if (account != null)
                    {  
                        Web3 web3 = new Web3(account, App.ethapiurl);
                       
                        if (selcionflag.ToLower() == "eth")
                        {
                            try
                            {
                                var gasprice = decimal.Parse(Nethereum.Util.UnitConversion.Convert.FromWei(System.Numerics.BigInteger.Parse(tranprice.Text)
                                    , UnitConversion.EthUnit.Gwei).ToString());

                                var hash = await web3.Eth.GetEtherTransferService().TransferEtherAsync(addto.Text.Trim(),
                                  decimal.Parse(tranamount.Text.Trim())
                                  , gasprice
                                  , System.Numerics.BigInteger.Parse(traegas.Text));
                                if (hash != null && hash.Length > 20 && !hash.StartsWith("0x0000000000"))
                                {
                                    tranamount.Text = "";
                                    await DisplayAlert("Tips", "Transaction hash:" + hash + " , Please wait a few minutes.", "OK");
                                }
                                else
                                {
                                    await DisplayAlert("Tips", "Transaction error", "OK");
                                }
                            }
                            catch (Exception ex)
                            {
                                await DisplayAlert("Exception", ex.Message, "OK");
                            }
                        }
                        else
                        {
                            try
                            {
                                 
                                var toamount= Nethereum.Util.UnitConversion.Convert.ToWei(decimal.Parse(tranamount.Text) ,int.Parse(decimals.ToString()));

                                string data = EthHelper.CallContractFunData("transfer(address,uint256)", new List<object> {addto.Text.Trim(), toamount });
                                Nethereum.RPC.Eth.DTOs.TransactionInput traninput = new Nethereum.RPC.Eth.DTOs.TransactionInput(data,contractAddress,account.Address
                                    , new HexBigInteger(traegas.Text), new HexBigInteger("0"));
                               
                                 
                                var hash = await web3.TransactionManager.SendTransactionAsync(traninput);

                                if (hash != null && hash.Length > 20 && !hash.StartsWith("0x0000000000"))
                                {
                                    tranamount.Text = "";
                                    await DisplayAlert("Tips", "Transaction hash:" + hash + " , Please wait a few minutes.", "OK");
                                }
                                else
                                {
                                    await DisplayAlert("Tips", "Transaction error", "OK");
                                }
                            }
                            catch (Exception ex)
                            {
                                await DisplayAlert("Exception", ex.Message, "OK");
                            }
                        }
                    
                    }
                    else
                    {
                        await DisplayAlert("Error", "Password is error", "OK");
                    }
                }
                catch { await DisplayAlert("Error", "Password is error", "OK"); }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception!", "Try again later " + ex.Message, "OK");
            }
            tranloading.IsRefreshing = false;
            sendbut.IsEnabled = true;
        }



        private async void Button_SCans_Clicked(object sender, EventArgs e)
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var result = await scanner.Scan();
            if (result != null)
            {
                addto.Text = result.Text.Trim();
            }

        }
    }


    public class dcinfo
    {

        public short decimals { get; set; }

        public string contractAddress { get; set; }
        public string contractAddress1 { get; set; }
        public string symbol { get; set; }
    }
}
