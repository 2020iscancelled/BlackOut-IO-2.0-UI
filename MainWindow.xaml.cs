using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Utilities;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using System.Net.Http;
using System.Data;

namespace BlackOutIO
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string appVersion = "2.0.2";
        string userAvatar;
        List<string> msgBBox = new List<string>();
        HttpClient msgClient = new HttpClient();

        string twoCapApiKeyS = String.Empty;
        string turboThh = String.Empty;
        Thread statsUpdater;
        HttpClient taskClient = new HttpClient();
        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(2),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        private void loadTurbo()
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("config").ToString());
                using (StreamReader r = new StreamReader("turboSettings.json"))
                {
                    string json = r.ReadToEnd();
                    Dictionary<string, string> items = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                   this.turboThreads.Text = (string)(items["threads"]);
                    turboThh = (string)(items["threads"]);
                }

            }
            catch
            {
                
            }
        }
        private void loadSettings()
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("config").ToString());
                using (StreamReader r = new StreamReader("settings.json"))
                {
                    string json = r.ReadToEnd();
                    Dictionary<string, string> items = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    this.twocapApiKey.Text = (string)(items["2captcha"]);
                    twoCapApiKeyS = (string)(items["2captcha"]);
                    this.discordWebhook.Text = (string)(items["webhook"]);
                }

            }
            catch
            {
                
            }
        }
        private void loadQTSettings()
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("config").ToString());
                using (StreamReader r = new StreamReader("qtsettings.json"))
                {
                    string json = r.ReadToEnd();
                    Dictionary<string, string> items = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    this.qtTasks.Text = (string)(items["threads"]);

                    this.qtProxylist.Text = (string)(items["proxylist"]);
                    this.qtProfile.Text = (string)(items["profiles"]);
                    this.qtSizes.Text = (string)(items["sizes"]);
                }

            }
            catch
            {
                notifier.ShowWarning("No QT Settings found, save some now!");
            }
        }
        private void loadSizegroups()
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("config").ToString());
                string[] lines = File.ReadAllLines("sizegroups");
                foreach (string line in lines)
                {
                    sizeGroupname.Items.Add(line.Split(':')[0]);
                }
            }
            catch
            {

            }
        }

        private void loadProfilegroups()
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("config").ToString());
                string[] lines = File.ReadAllLines("profilegroups");
                foreach (string line in lines)
                {
                    profileGroupName.Items.Add(line.Split(':')[0]);
                }
            }
            catch
            {

            }
        }
        private void updateUserStats(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("stats").ToString());

                if (!File.Exists("stats.json"))
                {
                    try
                    {
                        using (StreamWriter streamWriter = new StreamWriter("stats.json"))
                        {
                            streamWriter.Write("{\r\n                            \"CARTED\":0,\r\n                            \"FAILED\":0,\r\n                            \"PAYPAL\":0,\r\n                            \"CARD\":0\r\n\r\n                        }");
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {

                    {
                        using (StreamReader streamReader = new StreamReader("stats.json"))
                        {
                            dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                            this.FailedCheckouts.Text = (string)(obj1["FAILED"]);
                            this.SuccessfulCheckouts.Text = (string)(obj1["CARD"]);
                            this.paypalCheckouts.Text = (string)(obj1["PAYPAL"]);
                            this.CartedProducts.Text = (string)(obj1["CARTED"]);
                        }
                    }
                }
            }
            catch
            {

            }

        }
        async private Task<bool> updateCheck()
        {
            try
            {
                var client = new HttpClient();

                var resp = await client.GetAsync("https://patio.pythonanywhere.com/api/check-update?v=" + appVersion);
                string response = await resp.Content.ReadAsStringAsync();
                if (!response.Contains("ok"))
                {
                    this.notifier.ShowInformation("A new version is available for download, please close the app and launch the updater!");
                }
                else
                {
                    this.notifier.ShowSuccess("You are on the latest version!");
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        
        async private void bgInit()
        {
            try
            {
                updateCheck();
                
            }
            catch
            {

            }
        }

        private void trySetImg(string uri)
        {
            try
            {
                ImageBrush myBrush = new ImageBrush();
                Image image = new Image();
                image.Source = new BitmapImage(
                new Uri(
                 userAvatar
                 ));
                myBrush.ImageSource = image.Source;
                userIMG.Fill = myBrush;

            }
            catch
            {

                ImageBrush myBrush = new ImageBrush();
                Image image = new Image();
                image.Source = new BitmapImage(
                new Uri(
                 uri
                 ));
                myBrush.ImageSource = image.Source;
                userIMG.Fill = myBrush;
            }
        }

        private void loadUserData()
        {
            //removed some sensitive stuff
            trySetImg("https://emojipedia-us.s3.dualstack.us-west-1.amazonaws.com/thumbs/120/apple/237/rolling-on-the-floor-laughing_1f923.png");


        }
        public MainWindow()
        {
            InitializeComponent();
            loadUserData();
            updateUserStats(null, null);
            bgInit();
            loadSettings();
            loadTurbo();
            TaskPanel.Visibility = Visibility.Visible;
            ProfileManager.Visibility = Visibility.Collapsed;
            accPanel.Visibility = Visibility.Collapsed;
            SettingsPage.Visibility = Visibility.Collapsed;
            taskManager.Visibility = Visibility.Collapsed;
            proxyPanel.Visibility = Visibility.Collapsed;
            cookieTab.Visibility = Visibility.Collapsed;
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
            Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            try
            {
                FileInfo[] files = dir.GetFiles("*_bstn.json");
                foreach (FileInfo file in files)
                {
                    bstnCombo.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }
            try
            {
                FileInfo[] files = dir.GetFiles("*_43ein.json");
                foreach (FileInfo file in files)
                {
                    einhCombo.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }

            catch { }
            try
            {
                FileInfo[] files = dir.GetFiles("*_segons.json");
                foreach (FileInfo file in files)
                {
                    segonsCombo.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }
            try
            {
                FileInfo[] files = dir.GetFiles("*_svd.json");
                foreach (FileInfo file in files)
                {
                    svdCombo.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }
            try
            {
                FileInfo[] files = dir.GetFiles("*_b4b.json");
                foreach (FileInfo file in files)
                {
                    b4bCombo.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }


            try
            {
                FileInfo[] files = dir.GetFiles("*_awlab.json");
                foreach (FileInfo file in files)
                {
                    awCombo.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }

            try
            {
                FileInfo[] files = dir.GetFiles("*_montrose.json");
                foreach (FileInfo file in files)
                {
                    montroseCombo.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }
        }


        private void MouseDownMove(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {

            }
        }

        private void min(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void closeApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void showHome(object sender, RoutedEventArgs e)
        {
            TaskPanel.Visibility = Visibility.Visible;
            ProfileManager.Visibility = Visibility.Collapsed;
            taskManager.Visibility = Visibility.Collapsed;
            accPanel.Visibility = Visibility.Collapsed;
            SettingsPage.Visibility = Visibility.Collapsed;
            proxyPanel.Visibility = Visibility.Collapsed;
            proxyPanel.Visibility = Visibility.Collapsed;
            bstnCombo.Items.Clear();
            segonsCombo.Items.Clear();
            svdCombo.Items.Clear();
            b4bCombo.Items.Clear();
            einhCombo.Items.Clear();
            awCombo.Items.Clear();
            montroseCombo.Items.Clear();
            cookieTab.Visibility = Visibility.Collapsed;
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
            Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            try
            {
                FileInfo[] files = dir.GetFiles("*_bstn.json");
                foreach (FileInfo file in files)
                {
                    bstnCombo.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }
            try
            {
                FileInfo[] files = dir.GetFiles("*_43ein.json");
                foreach (FileInfo file in files)
                {
                    einhCombo.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }

            catch { }
            try
            {
                FileInfo[] files = dir.GetFiles("*_segons.json");
                foreach (FileInfo file in files)
                {
                    segonsCombo.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }
            try
            {
                FileInfo[] files = dir.GetFiles("*_svd.json");
                foreach (FileInfo file in files)
                {
                    svdCombo.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }
            try
            {
                FileInfo[] files = dir.GetFiles("*_b4b.json");
                foreach (FileInfo file in files)
                {
                    b4bCombo.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }


            try
            {
                FileInfo[] files = dir.GetFiles("*_awlab.json");
                foreach (FileInfo file in files)
                {
                    awCombo.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }

            try
            {
                FileInfo[] files= dir.GetFiles("*_montrose.json");
                foreach (FileInfo file in files)
                {
                    montroseCombo.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }

        }

        private void loadProfiles(dynamic comboBoxName)
        {
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
            Directory.SetCurrentDirectory(Directory.CreateDirectory("profiles").ToString());
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            comboBoxName.Items.Clear();
            try
            {
                FileInfo[] files = dir.GetFiles("*.txt");
                foreach (FileInfo file in files)
                {
                    comboBoxName.Items.Add(file.ToString().Split('.')[0]);
                }
            }
            catch { }
        }
        private void profileshow(object sender, RoutedEventArgs e)
        {
            ProfileManager.Visibility = Visibility.Visible;
            TaskPanel.Visibility = Visibility.Collapsed;
            SettingsPage.Visibility = Visibility.Collapsed;
            taskManager.Visibility = Visibility.Collapsed;
            accPanel.Visibility = Visibility.Collapsed;
            proxyPanel.Visibility = Visibility.Collapsed;
            cookieTab.Visibility = Visibility.Collapsed;
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
            Directory.SetCurrentDirectory(Directory.CreateDirectory("profiles").ToString());
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            profileName.Items.Clear();
            try
            {
                FileInfo[] files = dir.GetFiles("*.txt");
                foreach (FileInfo file in files)
                {
                    profileName.Items.Add(file.ToString().Split('.')[0]);
                }
            }
            catch { }

        }

        private void loadAllProxyLists()
        {
            try
            {
                dynamic proxylists = new List<ComboBox>
                {
                    bstnProxy,
                    einhalbProxylist,
                    segonsProxylist,
                    montroseProxylist,
                    b4bProxyList,
                    svdProxylist,
                    awlProxylist
                };
                dynamic loadedLists = new List<dynamic>();
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("proxies").ToString());
                DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                try
                {
                    FileInfo[] files = dir.GetFiles("*.txt");
                    foreach (FileInfo file in files)
                    {
                        loadedLists.Add(file.ToString().Split('.')[0]);
                    }
                    foreach (ComboBox listItem in proxylists)
                    {
                        listItem.Items.Clear();
                    }
                    foreach (ComboBox listItem in proxylists)
                    {
                        foreach (string proxyList in loadedLists)
                        {
                            listItem.Items.Add(proxyList.ToString());
                        }
                    }
                }
                catch
                {

                }
               

            }
            catch
            {

            }
        }
        private void editTask(object sender, RoutedEventArgs e)
        {
            ProfileManager.Visibility = Visibility.Collapsed;
            cookieTab.Visibility = Visibility.Collapsed;
            TaskPanel.Visibility = Visibility.Collapsed;
            taskManager.Visibility = Visibility.Visible;
            SettingsPage.Visibility = Visibility.Collapsed;
            accPanel.Visibility = Visibility.Collapsed;
            montroseTab.Visibility = Visibility.Collapsed;
            proxyPanel.Visibility = Visibility.Collapsed;
            bstnTab.Visibility = Visibility.Visible;
            SVDPanel.Visibility = Visibility.Collapsed;
            awTab.Visibility = Visibility.Hidden;
            einhManager.Visibility = Visibility.Collapsed;

            segonsTab.Visibility = Visibility.Collapsed;
            b4bTab.Visibility = Visibility.Collapsed;
            showSvd.Background = Brushes.Transparent;
            showAWL.Background = Brushes.Transparent;
            showB4B.Background = Brushes.Transparent;
            showBSTN.Background = showSvd.BorderBrush;
            showEinh.Background = Brushes.Transparent;
            showmontrose.Background = Brushes.Transparent;
            shopSegonsSelection.Background = Brushes.Transparent;
            loadAllProxyLists();

        }

        private void showSettings(object sender, RoutedEventArgs e)
        {
            ProfileManager.Visibility = Visibility.Collapsed;
            TaskPanel.Visibility = Visibility.Collapsed;
            taskManager.Visibility = Visibility.Collapsed;
            SettingsPage.Visibility = Visibility.Visible;
            accPanel.Visibility = Visibility.Collapsed;
            cookieTab.Visibility = Visibility.Collapsed;
            proxyPanel.Visibility = Visibility.Collapsed;
            this.loadSettings();
            this.loadQTSettings();
            this.loadTurbo();
            this.loadSizegroups();
            this.loadProfilegroups();
        }

        private void showAccPanel(object sender, RoutedEventArgs e)
        {
            ProfileManager.Visibility = Visibility.Collapsed;
            TaskPanel.Visibility = Visibility.Collapsed;
            taskManager.Visibility = Visibility.Collapsed;
            SettingsPage.Visibility = Visibility.Collapsed;
            proxyPanel.Visibility = Visibility.Collapsed;
            cookieTab.Visibility = Visibility.Collapsed;
            accPanel.Visibility = Visibility.Visible;
        }

        private void showProxyPanel(object sender, RoutedEventArgs e)
        {
            ProfileManager.Visibility = Visibility.Collapsed;
            montroseTab.Visibility = Visibility.Collapsed;
            TaskPanel.Visibility = Visibility.Collapsed;
            taskManager.Visibility = Visibility.Collapsed;
            cookieTab.Visibility = Visibility.Collapsed;
            SettingsPage.Visibility = Visibility.Collapsed;
            accPanel.Visibility = Visibility.Collapsed;
            proxyPanel.Visibility = Visibility.Visible;
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
            Directory.SetCurrentDirectory(Directory.CreateDirectory("proxies").ToString());
            proxylistN.Items.Clear();
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            try
            {
                FileInfo[] files = dir.GetFiles("*.txt");
                foreach (FileInfo file in files)
                {
                    proxylistN.Items.Add(file.ToString().Split('.')[0]);
                }
            }
            catch { }
        }

        private void delStats(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("stats").ToString());
                File.Delete("stats.json");
                this.SuccessfulCheckouts.Text = "0";
                this.CartedProducts.Text = "0";
                this.FailedCheckouts.Text = "0";
            }
            catch
            {

            }
        }

        async private void engineCheck(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpClient engine = new HttpClient();
                await engine.GetAsync("http://127.0.0.1:8888/enginecheck");
                this.notifier.ShowSuccess("Engine is ready!");
            }
            catch
            {
                this.notifier.ShowError("Engine is not connected!");
            }
        }

        private void openDiscord(object sender, RoutedEventArgs e)
        {
           //removed
        }

        private void saveProfile(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("profiles").ToString());
                using (StreamWriter profileWriter = new StreamWriter(profileName.Text + ".txt"))
                {
                    profileWriter.WriteLine(firstNamee.Text);
                    profileWriter.WriteLine(LastName.Text);
                    profileWriter.WriteLine(emailAddress.Text);
                    profileWriter.WriteLine(phoneNumber.Text);
                    profileWriter.WriteLine(street.Text);
                    profileWriter.WriteLine(HouseNumber.Text);
                    profileWriter.WriteLine(Address2.Text);
                    profileWriter.WriteLine(postcode.Text);
                    profileWriter.WriteLine(city.Text);
                    profileWriter.WriteLine(province.Text);
                    if (PPCheckout.IsChecked == true)
                    {
                        profileWriter.WriteLine("PP");
                        profileWriter.WriteLine("");
                        profileWriter.WriteLine("");
                        profileWriter.WriteLine("");
                        profileWriter.WriteLine("");
                        profileWriter.WriteLine("");

                    }
                    else
                    {
                        profileWriter.WriteLine("CC");
                        profileWriter.WriteLine(CCNum.Text);
                        profileWriter.WriteLine(CCMonth.Text);
                        profileWriter.WriteLine(CCYear.Text);
                        profileWriter.WriteLine(CCCVV.Text);
                        profileWriter.WriteLine(CardType.Text);

                    }

                    if (testingProfile.IsChecked == true)
                    {
                        profileWriter.WriteLine("1");
                    }
                    else
                    {
                        profileWriter.WriteLine("0");
                    }
                    profileWriter.WriteLine(country.Text);
                }
                notifier.ShowSuccess("Profile Saved.");
                profileName.Items.Add(profileName.Text);

            }
            catch
            {

            }
        }

        private void editProfile(object sender, RoutedEventArgs e)
        {
            try
            {

                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("profiles").ToString());
                string[] lines = File.ReadAllLines($"{profileName.Text}.txt");
                firstNamee.Text = lines[0];
                LastName.Text = lines[1];
                emailAddress.Text = lines[2];
                phoneNumber.Text = lines[3];
                street.Text = lines[4];
                HouseNumber.Text = lines[5];
                Address2.Text = lines[6];
                postcode.Text = lines[7];
                city.Text = lines[8];
                province.Text = lines[9];
                _ = lines[10].Contains("PP") ? this.PPCheckout.IsChecked = true : this.PPCheckout.IsChecked = false;
                CCNum.Text = lines[11];
                CCMonth.Text = lines[12];
                CCYear.Text = lines[13];
                CCCVV.Text = lines[14];
                CardType.Text = lines[15];


                _ = lines[16].Contains("1") ? this.testingProfile.IsChecked = true : this.testingProfile.IsChecked = false;
                country.Text = lines[17];
            }
            catch
            {
                this.notifier.ShowError("Failed to load select profile!");
            }

        }

        private void delProfile(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("profiles").ToString());
                File.Delete(profileName.Text + ".txt");
                profileName.Items.Remove(profileName.Text);
                firstNamee.Text = "";
                LastName.Text = "";
                emailAddress.Text = "";
                phoneNumber.Text = "";
                Address2.Text = "";
                street.Text = "";
                HouseNumber.Text = "";
                postcode.Text = "";
                city.Text = "";
                province.Text = "";
                country.Text = "";
                CCCVV.Text = "";
                CCNum.Text = "";
                CCYear.Text = "";
                CCMonth.Text = "";
                CardType.Text = "";
                testingProfile.IsChecked = false;


                PPCheckout.IsChecked = false;
                this.profileName.Items.Remove(this.profileName.Text);
                profileName.Text = "";
                notifier.ShowError($"Deleted {profileName.Text}");
            }
            catch
            {

            }
        }

        private void triggerBstnSelection(object sender, RoutedEventArgs e)
        {
            bstnTab.Visibility = Visibility.Visible;
            einhManager.Visibility = Visibility.Collapsed;

            SVDPanel.Visibility = Visibility.Collapsed;
            segonsTab.Visibility = Visibility.Collapsed;
            showSvd.Background = Brushes.Transparent;
            showAWL.Background = Brushes.Transparent;
            showB4B.Background = Brushes.Transparent;
            showBSTN.Background = showSvd.BorderBrush;
            montroseTab.Visibility = Visibility.Collapsed;
            showEinh.Background = Brushes.Transparent;
            b4bTab.Visibility = Visibility.Collapsed;
            showmontrose.Background = Brushes.Transparent;
            shopSegonsSelection.Background = Brushes.Transparent;
            awTab.Visibility = Visibility.Hidden;
            loadProfiles(bstnProfile);
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
            Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            bstnTaskN.Items.Clear();
            try
            {
                FileInfo[] files = dir.GetFiles("*_bstn.json");
                foreach (FileInfo file in files)
                {
                    bstnTaskN.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }


        }

        private void showSegons(object sender, RoutedEventArgs e)
        {
            bstnTab.Visibility = Visibility.Collapsed;
            einhManager.Visibility = Visibility.Collapsed;
            segonsTab.Visibility = Visibility.Visible;
            SVDPanel.Visibility = Visibility.Collapsed;
            segonsTab.Visibility = Visibility.Visible;
            showSvd.Background = Brushes.Transparent;
            showAWL.Background = Brushes.Transparent;
            showB4B.Background = Brushes.Transparent;
            showBSTN.Background = Brushes.Transparent;
            b4bTab.Visibility = Visibility.Collapsed;
            showEinh.Background = Brushes.Transparent;
            showmontrose.Background = Brushes.Transparent;
            montroseTab.Visibility = Visibility.Collapsed;
            shopSegonsSelection.Background = showAWL.BorderBrush;
            awTab.Visibility = Visibility.Hidden;
            loadProfiles(segonsProfile);
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
            Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            segonsTaskName.Items.Clear();
            try
            {
                FileInfo[] files = dir.GetFiles("*_segons.json");
                foreach (FileInfo file in files)
                {
                    segonsTaskName.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }
        }

        private void trigger43einOptions(object sender, RoutedEventArgs e)
        {
            bstnTab.Visibility = Visibility.Collapsed;
            einhManager.Visibility = Visibility.Visible;
            segonsTab.Visibility = Visibility.Collapsed;
            SVDPanel.Visibility = Visibility.Collapsed;
            segonsTab.Visibility = Visibility.Collapsed;
            b4bTab.Visibility = Visibility.Collapsed;
            montroseTab.Visibility = Visibility.Collapsed;
            showSvd.Background = Brushes.Transparent;
            showAWL.Background = Brushes.Transparent;
            showB4B.Background = Brushes.Transparent;
            showBSTN.Background = Brushes.Transparent;
            showEinh.Background = showAWL.BorderBrush;
            showmontrose.Background = Brushes.Transparent;
            shopSegonsSelection.Background = Brushes.Transparent;
            awTab.Visibility = Visibility.Hidden;
            loadProfiles(einhalbProfile);
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
            Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            einhalbTaskn.Items.Clear();
            try
            {
                FileInfo[] files = dir.GetFiles("*_43ein.json");
                foreach (FileInfo file in files)
                {
                    einhalbTaskn.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }
        }

        private void triggerAwlOptions(object sender, RoutedEventArgs e)
        {
            bstnTab.Visibility = Visibility.Collapsed;
            einhManager.Visibility = Visibility.Collapsed;
            segonsTab.Visibility = Visibility.Collapsed;
            awTab.Visibility = Visibility.Visible;
            segonsTab.Visibility = Visibility.Collapsed;
            showSvd.Background = Brushes.Transparent;
            b4bTab.Visibility = Visibility.Collapsed;
            showAWL.Background = showB4B.BorderBrush;
            showB4B.Background = Brushes.Transparent;
            showBSTN.Background = Brushes.Transparent;
            showEinh.Background = Brushes.Transparent;
            montroseTab.Visibility = Visibility.Collapsed;
            showmontrose.Background = Brushes.Transparent;
            SVDPanel.Visibility = Visibility.Collapsed;
            shopSegonsSelection.Background = Brushes.Transparent;
            loadProfiles(awlProfile);
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
            Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            awlTaskName.Items.Clear();
            try
            {
                FileInfo[] files = dir.GetFiles("*_awlab.json");
                foreach (FileInfo file in files)
                {
                    awlTaskName.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }

        }

        private void triggerMontroseOptions(object sender, RoutedEventArgs e)
        {
            bstnTab.Visibility = Visibility.Collapsed;
            einhManager.Visibility = Visibility.Collapsed;
            segonsTab.Visibility = Visibility.Collapsed;
            SVDPanel.Visibility = Visibility.Collapsed;
            awTab.Visibility = Visibility.Collapsed;
            segonsTab.Visibility = Visibility.Collapsed;
            b4bTab.Visibility = Visibility.Collapsed;
            showSvd.Background = Brushes.Transparent;
            showAWL.Background = Brushes.Transparent;
            showB4B.Background = Brushes.Transparent;
            showBSTN.Background = Brushes.Transparent;
            montroseTab.Visibility = Visibility.Visible;
            showEinh.Background = Brushes.Transparent;
            showmontrose.Background = showB4B.BorderBrush;
            shopSegonsSelection.Background = Brushes.Transparent;
            loadProfiles(montroseProfile);
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
            Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            montroseTaskName.Items.Clear();
            try
            {
                FileInfo[] files = dir.GetFiles("*_montrose.json");
                foreach (FileInfo file in files)
                {
                    montroseTaskName.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }
        }


        private void triggerB4bOptions(object sender, RoutedEventArgs e)
        {
            bstnTab.Visibility = Visibility.Collapsed;
            einhManager.Visibility = Visibility.Collapsed;
            segonsTab.Visibility = Visibility.Collapsed;
            SVDPanel.Visibility = Visibility.Collapsed;
            awTab.Visibility = Visibility.Hidden;
            segonsTab.Visibility = Visibility.Collapsed;
            b4bTab.Visibility = Visibility.Visible;
            showSvd.Background = Brushes.Transparent;
            showAWL.Background = Brushes.Transparent;
            showB4B.Background = showB4B.BorderBrush;
            showBSTN.Background = Brushes.Transparent;
            montroseTab.Visibility = Visibility.Collapsed;
            showEinh.Background = Brushes.Transparent;
            showmontrose.Background = Brushes.Transparent;
            shopSegonsSelection.Background = Brushes.Transparent;
            loadProfiles(b4bProfile);
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
            Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            b4bTaskName.Items.Clear();
            try
            {
                FileInfo[] files = dir.GetFiles("*_b4b.json");
                foreach (FileInfo file in files)
                {
                    b4bTaskName.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }
        }

        private void triggerSvdOptions(object sender, RoutedEventArgs e)
        {
            bstnTab.Visibility = Visibility.Collapsed;
            einhManager.Visibility = Visibility.Collapsed;
            segonsTab.Visibility = Visibility.Collapsed;
            SVDPanel.Visibility = Visibility.Visible;
            awTab.Visibility = Visibility.Hidden;
            segonsTab.Visibility = Visibility.Collapsed;
            b4bTab.Visibility = Visibility.Collapsed;
            showSvd.Background = showSvd.BorderBrush;
            showAWL.Background = Brushes.Transparent;
            showB4B.Background = Brushes.Transparent;
            montroseTab.Visibility = Visibility.Collapsed;
            showBSTN.Background = Brushes.Transparent;
            showEinh.Background = Brushes.Transparent;
            showmontrose.Background = Brushes.Transparent;
            shopSegonsSelection.Background = Brushes.Transparent;
            loadProfiles(svdProfile);
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
            Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());



            svdTaskName.Items.Clear();
            try
            {
                FileInfo[] files = dir.GetFiles("*_svd.json");
                foreach (FileInfo file in files)
                {
                    svdTaskName.Items.Add(file.ToString().Split('.')[0].Split('_')[0]);
                }
            }
            catch { }

        }

        private void saveProxies(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("proxies").ToString());
                if (this.proxylistField.Text == "" || this.proxylistN.Text == "")
                {
                    notifier.ShowInformation("Proxylist Name or Content must not be empty!");
                }
                else
                {
                    string[] proxies = this.proxylistField.Text.Split(Environment.NewLine.ToCharArray());
                    string editedList = "";

                    foreach (string proxy in proxies)
                    {
                        if (proxy == "" || proxy == Environment.NewLine)
                        {
                            continue;
                        }
                        else if (proxy.Contains("@"))
                        {
                            editedList = $"{editedList}{proxy}{Environment.NewLine}";
                        }
                        else if (proxy.Count(f => f == ':') == 3)
                        {
                            string[] proxAr = proxy.Split(new char[] { ':' });
                            editedList = $"{editedList}{proxAr[2]}:{proxAr[3]}@{proxAr[0]}:{proxAr[1]}{Environment.NewLine}";

                        }
                        else
                        {
                            editedList = $"{editedList}{proxy}{Environment.NewLine}";
                        }
                    }
                    using (StreamWriter proxyListWriter = new StreamWriter(this.proxylistN.Text + ".txt"))
                    {
                        proxyListWriter.WriteLine(editedList);
                    }
                    proxylistField.Text = editedList;
                    notifier.ShowSuccess("Saved Proxylist successfully");
                    this.proxylistN.Items.Add(this.proxylistN.Text);


                };
            }
            catch
            {
                notifier.ShowError("Failed to save proxylist.");
            }
        }

        private void delProxylist(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("proxies").ToString());
                File.Delete(this.proxylistN.Text);
                notifier.ShowSuccess("Deleted Proxylist");
                this.proxylistN.Items.Remove(this.proxylistN.Text);
            }
            catch
            {
                notifier.ShowError("Failed to delete the selected List");
            }
        }

        private void loadList(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("proxies").ToString());
                this.proxylistField.Text = File.ReadAllText(this.proxylistN.Text + ".txt");
            }
            catch { }

        }

        private void loadAcc(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("accounts").ToString());
                if (this.selAccSite.Text.Contains("Aw"))
                {
                    this.accountField.Text = File.ReadAllText("awlabacc");
                }
                else if (this.selAccSite.Text.Contains("Basket"))
                {
                    this.accountField.Text = File.ReadAllText("b4bacc");
                }
                else if (this.selAccSite.Text.Contains("Sivas"))
                {
                    this.accountField.Text = File.ReadAllText("svdacc");
                }
                else if (this.selAccSite.Text.Contains("Shinzo"))
                {
                    this.accountField.Text = File.ReadAllText("shinzoacc");
                }
                else
                {
                    notifier.ShowWarning("Please select the site you want to load accounts for!");
                }
            }
            catch
            {
                notifier.ShowError("Failed to load any saved accounts for this site!");
            }
        }
        private void saveAccount(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("accounts").ToString());
                if (selAccSite.Text.Contains("Basket4Ballers"))
                {
                    using (StreamWriter writer = new StreamWriter("b4bacc"))
                    {
                        writer.Write(accountField.Text);

                    }
                    notifier.ShowSuccess("Accounts saved successfully!");
                }
                else if (selAccSite.Text.Contains("Shinzo"))
                {

                    using (StreamWriter writer = new StreamWriter("shinzoacc"))
                    {
                        writer.Write(accountField.Text);
                    }
                    notifier.ShowSuccess("Accounts saved successfully!");
                }
                else if (selAccSite.Text.Contains("Sivas"))
                {

                    using (StreamWriter writer = new StreamWriter("svdacc"))
                    {
                        writer.Write(accountField.Text);
                    }
                    notifier.ShowSuccess("Accounts saved successfully!");
                }
                else if (selAccSite.Text.Contains("Aw"))
                {

                    using (StreamWriter writer = new StreamWriter("awlabacc"))
                    {
                        writer.Write(accountField.Text);
                    }
                    notifier.ShowSuccess("Accounts saved successfully!");
                }
                else if (selAccSite.Text.Contains("24se"))
                {

                    using (StreamWriter writer = new StreamWriter("segacc"))
                    {
                        writer.Write(accountField.Text);
                    }
                    notifier.ShowSuccess("Accounts saved successfully!");
                }
                else
                {
                    notifier.ShowWarning("Please select a site!");
                }
            }
            catch
            {
                notifier.ShowError("Failed to save accounts.");
            }
        }

        private void delAcc(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.selAccSite.Text.Contains("Basket"))
                {
                    File.Delete("b4bacc");
                    notifier.ShowSuccess("Deleted Accounts successfully");
                }
                else if (this.selAccSite.Text.Contains("Aw"))
                {
                    File.Delete("awlabacc");
                    notifier.ShowSuccess("Deleted Accounts successfully");
                }
                else if (this.selAccSite.Text.Contains("Shinzo"))
                {
                    File.Delete("shinzoacc");
                    notifier.ShowSuccess("Deleted Accounts successfully");
                }
                else if (this.selAccSite.Text.Contains("Sivas"))
                {
                    File.Delete("svdacc");
                    notifier.ShowSuccess("Deleted Accounts successfully");
                }
                else
                {
                    notifier.ShowWarning("Please select a site!");
                }
            }
            catch
            {
                notifier.ShowError("Failed to delete accounts, are there even any saved?");
            }
        }

        private void saveStandardSettings(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("config").ToString());
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, string> settings = new Dictionary<string, string>();
                settings.Add("webhook", discordWebhook.Text);
                settings.Add("2captcha", twocapApiKey.Text);
                twoCapApiKeyS = twocapApiKey.Text;
                using (StreamWriter settingWriter = new StreamWriter("settings.json"))
                {
                    serializer.Serialize(settingWriter, settings);
                }
                notifier.ShowSuccess("Saved Settings successfully");
            }
            catch
            {
                notifier.ShowSuccess("Failed to save settings!");
            }
        }

        private void delStandardSettings(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("config").ToString());
                File.Delete("settings.json");
                twocapApiKey.Text = "";

                discordWebhook.Text = "";
                this.notifier.ShowSuccess("Deleted saved settings!");
            }
            catch
            {
                this.notifier.ShowError("Failed to save settings!");
            }
        }

        async private void testWebhook(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpClient hookClient = new HttpClient();
                dynamic jObject = new JObject();
                dynamic obj = new JObject();
                dynamic jObject1 = new JObject();
                dynamic obj1 = new JObject();
                dynamic jObject2 = new JObject();
                dynamic obj2 = new JObject();
                dynamic jObject3 = new JObject();
                obj2.name = "UNIX TS:";
                obj1.text = "BlackOut IO by BlackOut Labs";
                dynamic totalSeconds = obj2;
                DateTime utcNow = DateTime.UtcNow;
                TimeSpan timeSpan = utcNow.Subtract(new DateTime(1970, 1, 1));
                totalSeconds.value = (int)timeSpan.TotalSeconds;
                obj2.inline = false;
                jObject2.name = "Link:";
                jObject2.value = "https://twitter.com/BlackOutÍO";
                jObject2.inline = false;
                jObject1.title = "Testing Notification!";
                jObject1.color = 1127128;
                jObject1.description = "If you got this, your Webhook was correctly configured!";
                obj.name = "BlackOut 2.0";
                obj.icon_url = "https://pbs.twimg.com/profile_images/1222620177210187783/e65J4wz4_400x400.jpg";
                jObject1.author = obj;
                jObject1.footer = obj1;
                jObject3.name = "Chrome Extension Test (redirects to google if correctly set):";
                jObject3.value = "[CLICK ME](https://checkouts.blackoutio.com/?TESTINGCOOKIE&continuetoPPUrl=https://google.com)";
                dynamic obj3 = new JArray(jObject2, obj2, jObject3);
                jObject1.fields = obj3;
                jObject.embeds = new JArray(jObject1);
                string str = (string)JsonConvert.SerializeObject(jObject);
                StringContent stringContent = new StringContent(str.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponseMessage = await hookClient.PostAsync(this.discordWebhook.Text, stringContent);

            }
            catch
            {
                notifier.ShowError("Failed to test webhook. Check the URL & your internet connection!");
            }
        }

        private void qtSave(object sender, RoutedEventArgs e)
        {
            try
            {

                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("config").ToString());
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, string> settings = new Dictionary<string, string>();
                settings.Add("profiles", qtProfile.Text);
                settings.Add("threads", qtTasks.Text);
                settings.Add("proxylist", qtProxylist.Text);

                settings.Add("sizes", qtSizes.Text);
                using (StreamWriter settingWriter = new StreamWriter("qtsettings.json"))
                {
                    serializer.Serialize(settingWriter, settings);
                }
                this.notifier.ShowSuccess("Successfully saved QT settings!");
            }
            catch
            {
                this.notifier.ShowError("Failed to save QT settings. Please retry.");
            }
        }

        private void delQTSettings(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("config").ToString());
                File.Delete("qtsettings.json");
                this.qtSizes.Text = "";

                this.qtProxylist.Text = "";
                this.qtProfile.Text = "";
                this.qtTasks.Text = "";
                notifier.ShowSuccess("Deleted QT Settings");
            }
            catch
            {
                notifier.ShowError("Failed to delete QT Settings. Are there any saved?");
            }
        }

        private void saveGroup(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("settings").ToString());
                using (StreamWriter settingWriter = File.AppendText("sizegroups"))
                {
                    settingWriter.WriteLine($"{sizeGroupname.Text}:{sizegroupContent.Text}");
                }

                this.sizeGroupname.Items.Add(this.sizeGroupname.Text);
                notifier.ShowSuccess("Saved Sizegroup successfully!");

            }
            catch
            {
                notifier.ShowError("Failed to save sizegroup!");
            }
        }

        private void loadSGroup(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("settings").ToString());
                string[] lines = File.ReadAllLines("sizegroups");
                foreach (string line in lines)
                {
                    if (line.Split(':')[0] == sizeGroupname.Text)
                    {
                        sizegroupContent.Text = line.Split(':')[1];
                    }
                }
            }
            catch
            {
                notifier.ShowError("Failed to load sizegroup, are you sure it exists?");
            }
        }

        private void delSizegroup(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("settings").ToString());
                File.Delete("sizegroups");
                sizeGroupname.Items.Clear();
                notifier.ShowSuccess("Deleted all sizegroups!");
            }
            catch
            {
                notifier.ShowError("Failed to delete sizegroup, are you sure it exists?");
            }
        }

        private void delProfilegroup(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("settings").ToString());
                File.Delete("profilegroups");
                sizeGroupname.Items.Clear();
                notifier.ShowSuccess("Deleted all profilegroups!");
            }
            catch
            {
                notifier.ShowError("Failed to delete profilegroups, are you sure it exists?");
            }
        }

        private void saveProfileGroup(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("settings").ToString());
                using (StreamWriter settingWriter = File.AppendText("profilegroups"))
                {
                    settingWriter.WriteLine($"{profileGroupName.Text}:{profileGroupContent.Text}");
                }

                this.sizeGroupname.Items.Add(this.profileGroupName.Text);
                notifier.ShowSuccess("Saved Profilegroup successfully!");

            }
            catch
            {
                notifier.ShowError("Failed to save profilegroup!");
            }
        }

        private void editProfileGroup(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("settings").ToString());
                string[] lines = File.ReadAllLines("profilegroup");
                foreach (string line in lines)
                {
                    if (line.Split(':')[0] == profileGroupName.Text)
                    {
                        profileGroupContent.Text = line.Split(':')[1];
                    }
                }
            }
            catch
            {
                notifier.ShowError("Failed to load sizegroup, are you sure it exists?");
            }
        }

        private void b4bSaveTask(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, string> settings = new Dictionary<string, string>();

                settings.Add("link", b4bURL.Text);
                settings.Add("sizes", b4bSizes.Text);
                settings.Add("proxylist", b4bProxyList.Text);
                settings.Add("profiles", b4bProfile.Text);
                settings.Add("retrydelay", b4bDelay.Text);
                settings.Add("threads", b4bTaskAmount.Text);
                settings.Add("cookieATC", b4bCookieATC.IsChecked == true ? "1" : "0");
                settings.Add("forceCaptcha", b4bForceaptcha.IsChecked == true ? "1" : "0");
                settings.Add("useHarvester", b4bUseHarv.IsChecked == true ? "1" : "0");
                settings.Add("earlyLogin", b4bEarlyLogin.IsChecked == true ? "1" : "0");
                settings.Add("clearCart", b4bAccClear.IsChecked == true ? "1" : "0");
                using (StreamWriter taskWriter = new StreamWriter(b4bTaskName.Text + "_b4b.json"))
                {
                    serializer.Serialize(taskWriter, settings);
                }
                this.notifier.ShowSuccess("Successfully saved Taskgroup!");

            }
            catch
            {

            }
        }

        private void b4bLoadTask(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                using (StreamReader streamReader = new StreamReader(b4bTaskName.Text + "_b4b.json"))
                {
                    dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                    this.b4bURL.Text = (string)(obj1["link"]);
                    this.b4bSizes.Text = (string)(obj1["sizes"]);
                    this.b4bProfile.Text = (string)(obj1["profiles"]);
                    this.b4bProxyList.Text = (string)(obj1["proxylist"]);
                    this.b4bDelay.Text = (string)(obj1["retrydelay"]);
                    this.b4bTaskAmount.Text = (string)(obj1["threads"]);
                    _ = (string)(obj1["cookieATC"]) == "1" ? this.b4bCookieATC.IsChecked = true : this.b4bCookieATC.IsChecked = false;
                    _ = (string)(obj1["forceCaptcha"]) == "1" ? this.b4bForceaptcha.IsChecked = true : this.b4bForceaptcha.IsChecked = false;
                    _ = (string)(obj1["useHarvester"]) == "1" ? this.b4bUseHarv.IsChecked = true : this.b4bUseHarv.IsChecked = false;
                    _ = (string)(obj1["earlyLogin"]) == "1" ? this.b4bEarlyLogin.IsChecked = true : this.b4bEarlyLogin.IsChecked = false;
                    _ = (string)(obj1["cartClear"]) == "1" ? this.b4bAccClear.IsChecked = true : this.b4bAccClear.IsChecked = false;




                }
                this.b4bTaskName.Items.Add(b4bTaskName.Text);

            }
            catch
            {
                notifier.ShowError("Failed to Load selected Task");
            }
        }

        private void b4bDelTask(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                File.Delete(this.b4bTaskName.Text + "_b4b.json");
                this.b4bUseHarv.IsChecked = false;
                this.b4bTaskAmount.Text = "";
                this.b4bTaskName.Items.Remove(this.b4bTaskName.Text);
                this.b4bSizes.Clear();
                this.b4bProxyList.Text = "";
                this.b4bProfile.Text = "";
                this.b4bDelay.Clear();
                this.b4bCookieATC.IsChecked = false;
                this.b4bEarlyLogin.IsChecked = false;
                this.b4bForceaptcha.IsChecked = false;
                this.notifier.ShowSuccess("Taskgroup Deleted");

            }
            catch
            {

            }
        }







        private void awlabSave(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, string> settings = new Dictionary<string, string>();

                settings.Add("link", this.awlUrl.Text);
                settings.Add("sizes", this.awlSizes.Text);
                settings.Add("proxylist", this.awlProxylist.Text);
                settings.Add("cproxy", this.awlCproxy.Text);
                settings.Add("profiles", this.awlProfile.Text);
                settings.Add("retrydelay", this.awlDelay.Text);
                settings.Add("threads", this.awlTaskAmount.Text);
                settings.Add("discount", this.awlDiscount.Text);
                settings.Add("cod", this.awlCOD.IsChecked == true ? "1" : "0");
                settings.Add("threeDs", this.awl3ds.IsChecked == true ? "1" : "0");
                this.awlTaskName.Items.Add(awlTaskName.Text);
                using (StreamWriter taskWriter = new StreamWriter(awlTaskName.Text + "_awlab.json"))
                {
                    serializer.Serialize(taskWriter, settings);
                }
                this.notifier.ShowSuccess("Taskgroup saved successfully!");
            }
            catch
            {
                this.notifier.ShowError("Failed to Save Task!");
            }
        }

        private void loadAWL(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                using (StreamReader streamReader = new StreamReader(this.awlTaskName.Text + "_awlab.json"))
                {
                    dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                    this.awlUrl.Text = (string)(obj1["link"]);
                    this.awlSizes.Text = (string)(obj1["sizes"]);
                    this.awlProfile.Text = (string)(obj1["profiles"]);
                    this.awlProxylist.Text = (string)(obj1["proxylist"]);
                    this.awlCproxy.Text = (string)(obj1["cproxy"]);
                    this.awlDelay.Text = (string)(obj1["retrydelay"]);
                    this.awlTaskAmount.Text = (string)(obj1["threads"]);
                    this.awlDiscount.Text = (string)(obj1["discount"]);
                    _ = (string)(obj1["cod"]) == "1" ? this.awlCOD.IsChecked = true : this.awlCOD.IsChecked = false;
                    _ = (string)(obj1["threeds"]) == "1" ? this.awl3ds.IsChecked = true : this.awl3ds.IsChecked = false;

                }


            }
            catch
            {
                notifier.ShowError("Failed to Load selected Task");
            }
        }

        private void delAwl(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                File.Delete(this.awlTaskName.Text + "_awlab.json");
                this.awlUrl.Text = "";
                this.awlSizes.Clear();
                this.awlProfile.Text = "";
                this.awlProxylist.Text = "";
                this.awlDelay.Text = "";
                this.awlCproxy.Text = "";
                this.awlDiscount.Text = "";
                this.awlCOD.IsChecked = false;
                awl3ds.IsChecked = false;
                this.awlTaskAmount.Text = "";

            }
            catch
            {
                this.notifier.ShowError("Failed to save Taskgroup. Does it even exist?");
            }
        }

        private void saveSVD(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, string> settings = new Dictionary<string, string>();

                settings.Add("link", this.svdURL.Text);
                settings.Add("sizes", this.svdSizes.Text);
                settings.Add("proxylist", this.svdProxylist.Text);
                settings.Add("profiles", this.svdProfile.Text);
                settings.Add("retrydelay", this.svdDelay.Text);
                settings.Add("threads", this.svdTaskAmount.Text);

                this.svdTaskName.Items.Add(this.svdTaskName.Text);
                using (StreamWriter taskWriter = new StreamWriter(svdTaskName.Text + "_svd.json"))
                {
                    serializer.Serialize(taskWriter, settings);
                }
                this.notifier.ShowSuccess("Taskgroup saved successfully!");
            }
            catch
            {
                this.notifier.ShowError("Failed to Save Task!");
            }
        }

        private void loadSVDTask(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                using (StreamReader streamReader = new StreamReader(this.svdTaskName.Text + "_svd.json"))
                {
                    dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                    this.svdURL.Text = (string)(obj1["link"]);
                    this.svdSizes.Text = (string)(obj1["sizes"]);
                    this.svdProfile.Text = (string)(obj1["profiles"]);
                    this.svdProxylist.Text = (string)(obj1["proxylist"]);
                    this.svdDelay.Text = (string)(obj1["retrydelay"]);
                    this.svdTaskName.Text = (string)(obj1["threads"]);

                }



            }
            catch
            {
                notifier.ShowError("Failed to Load selected Task");
            }
        }

        private void delSivas(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                File.Delete(this.svdTaskName.Text + "_svd.json");
                this.svdURL.Text = "";
                this.svdSizes.Clear();
                this.svdProfile.Text = "";
                this.svdProxylist.Text = "";
                this.svdDelay.Text = "";
                this.svdTaskAmount.Text = "";

                this.svdTaskName.Items.Remove(this.svdTaskName.Text);

            }
            catch
            {
                this.notifier.ShowError("Failed to save Taskgroup. Does it even exist?");
            }
        }

        private void saveSegons(object sender, RoutedEventArgs e)
        {

            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, string> settings = new Dictionary<string, string>();

                settings.Add("link", this.segonsURL.Text);
                settings.Add("sizes", this.segonsSizes.Text);
                settings.Add("proxylist", this.segonsProxylist.Text);
                settings.Add("profiles", this.segonsProfile.Text);
                settings.Add("retrydelay", this.segonsDelay.Text);
                settings.Add("threads", this.segonsTaskAmount.Text);
                settings.Add("earlyLogin", this.segonsEarlyLogin.IsChecked == true ? "1" : "0");
                settings.Add("clearCart", this.segonsCartClear.IsChecked == true ? "1" : "0");
                this.segonsTaskName.Items.Add(this.segonsTaskName.Text);
                using (StreamWriter taskWriter = new StreamWriter(segonsTaskName.Text + "_segons.json"))
                {
                    serializer.Serialize(taskWriter, settings);
                }
                this.notifier.ShowSuccess("Taskgroup saved successfully!");
            }
            catch
            {
                this.notifier.ShowError("Failed to Save Task!");
            }
        }

        private void loadSegons(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                using (StreamReader streamReader = new StreamReader(this.segonsTaskName.Text + "_segons.json"))
                {
                    dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                    this.segonsURL.Text = (string)(obj1["link"]);
                    this.segonsSizes.Text = (string)(obj1["sizes"]);
                    this.segonsProfile.Text = (string)(obj1["profiles"]);
                    this.segonsProxylist.Text = (string)(obj1["proxylist"]);
                    this.segonsDelay.Text = (string)(obj1["retrydelay"]);
                    this.segonsTaskAmount.Text = (string)(obj1["threads"]);
                    if ((obj1["earlyLogin"]).ToString().Contains("1"))
                    {
                        this.segonsEarlyLogin.IsChecked = true;
                    }if ((obj1["clearCart"]).ToString().Contains("1"))
                    {
                        this.segonsCartClear.IsChecked = true;
                    }

                }

            }
            catch
            {
                notifier.ShowError("Failed to Load selected Task");
            }
        }

        private void delSegons(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                File.Delete(this.segonsTaskName.Text + "_segons.json");
                this.segonsURL.Text = "";
                this.segonsSizes.Clear();
                this.segonsProfile.Text = "";
                this.segonsProxylist.Text = "";
                this.segonsDelay.Text = "";
                this.segonsTaskName.Text = "";
                this.segonsEarlyLogin.IsChecked = false;

                this.segonsCartClear.IsChecked = false;
                this.segonsTaskName.Items.Remove(this.segonsTaskName.Text);

            }
            catch
            {
                this.notifier.ShowError("Failed to save Taskgroup. Does it even exist?");
            }
        }

        private void saveEinh(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, string> settings = new Dictionary<string, string>();

                settings.Add("link", this.einhalbURL.Text);
                settings.Add("sizes", this.einhalbSizes.Text);
                settings.Add("proxylist", this.einhalbProxylist.Text);
                settings.Add("profiles", this.einhalbProfile.Text);
                settings.Add("retrydelay", this.einDelay.Text);
                settings.Add("threads", this.einhTasks.Text);
                settings.Add("username", this.einhUsern.Text);
                settings.Add("password", this.einPassword.Text);
                settings.Add("cproxy", this.einhalbCproxy.Text);
                settings.Add("useharv", this.einhHarv.IsChecked == true ? "1" : "0");
                settings.Add("instore", this.einInstore.IsChecked == true ? "1" : "0");
                settings.Add("cad", this.einhCad.IsChecked == true ? "1" : "0");
                settings.Add("extension", this.einhChrome.IsChecked == true ? "1" : "0");
                this.einhalbTaskn.Items.Add(this.einhalbTaskn.Text);
                using (StreamWriter taskWriter = new StreamWriter(einhalbTaskn.Text + "_43ein.json"))
                {
                    serializer.Serialize(taskWriter, settings);
                }
                this.notifier.ShowSuccess("Taskgroup saved successfully!");
            }
            catch
            {
                this.notifier.ShowError("Failed to Save Task!");
            }
        }

        private void loadEinh(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                using (StreamReader streamReader = new StreamReader(this.einhalbTaskn.Text + "_43ein.json"))
                {
                    dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                    this.einhalbURL.Text = (string)(obj1["link"]);
                    this.einhalbSizes.Text = (string)(obj1["sizes"]);
                    this.einhalbProfile.Text = (string)(obj1["profiles"]);
                    this.einhalbProxylist.Text = (string)(obj1["proxylist"]);
                    this.einDelay.Text = (string)(obj1["retrydelay"]);
                    this.einhTasks.Text = (string)(obj1["threads"]);
                    this.einPassword.Text = (string)(obj1["password"]);
                    this.einhUsern.Text = (string)(obj1["username"]);
                    this.einhalbCproxy.Text = (string)(obj1["cproxy"]);
                    _ = (string)(obj1["useharv"]) == "1" ? this.einhHarv.IsChecked = true : this.einhHarv.IsChecked = false;
                    _ = (string)(obj1["cad"]) == "1" ? this.einhCad.IsChecked = true : this.einhCad.IsChecked = false;
                    _ = (string)(obj1["extension"]) == "1" ? this.einhChrome.IsChecked = true : this.einhChrome.IsChecked = false;
                    _ = (string)(obj1["instore"]) == "1" ? this.einInstore.IsChecked = true : this.einInstore.IsChecked = false;
                }

            }
            catch
            {
                notifier.ShowError("Failed to Load selected Task");
            }
        }

        private void delEinh(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                File.Delete(this.einhalbTaskn.Text + "_43ein.json");
                this.einhalbURL.Text = "";
                this.einhalbSizes.Clear();
                this.einhalbProfile.Text = "";
                this.einhalbProxylist.Text = "";
                this.einDelay.Text = "";
                this.einhTasks.Text = "";
                this.einhHarv.IsChecked = false;
                this.einInstore.IsChecked = false;
                this.einhCad.IsChecked = false;
                this.einhChrome.IsChecked = false;
                this.einhalbTaskn.Items.Remove(this.einhalbTaskn.Text);

            }
            catch
            {
                this.notifier.ShowError("Failed to save Taskgroup. Does it even exist?");
            }
        }

        private void bstnSave(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, string> settings = new Dictionary<string, string>();
                settings.Add("link", this.bstnURL.Text);
                settings.Add("sizes", this.bstnSizes.Text);
                settings.Add("proxylist", this.bstnProxy.Text);
                settings.Add("profiles", this.bstnProfile.Text);
                settings.Add("retrydelay", this.bstnDelay.Text);
                settings.Add("threads", this.bstnTaskAmount.Text);
                settings.Add("cproxy", this.bstnCproxy.Text);
                settings.Add("useharv", this.bstnUseHarv.IsChecked == true ? "1" : "0");
                settings.Add("threeDs", this.bstnBrowser3Ds.IsChecked == true ? "1" : "0");

                this.bstnTaskN.Items.Add(this.bstnTaskN.Text);
                using (StreamWriter taskWriter = new StreamWriter(bstnTaskN.Text + "_bstn.json"))
                {
                    serializer.Serialize(taskWriter, settings);
                }
                this.notifier.ShowSuccess("Taskgroup saved successfully!");
            }
            catch
            {
                this.notifier.ShowError("Failed to Save Task!");
            }
        }

        private void loadBSTN(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                using (StreamReader streamReader = new StreamReader(this.bstnTaskN.Text + "_bstn.json"))
                {
                    dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                    this.bstnURL.Text = (string)(obj1["link"]);
                    this.bstnSizes.Text = (string)(obj1["sizes"]);
                    this.bstnProfile.Text = (string)(obj1["profiles"]);
                    this.bstnProxy.Text = (string)(obj1["proxylist"]);
                    this.bstnDelay.Text = (string)(obj1["retrydelay"]);
                    this.bstnTaskAmount.Text = (string)(obj1["threads"]);
                    _ = (string)(obj1["threeDs"]) == "1" ? this.bstnBrowser3Ds.IsChecked = true : this.bstnBrowser3Ds.IsChecked = false;

                    this.bstnCproxy.Text = (string)(obj1["cproxy"]);
                    _ = (string)(obj1["useharv"]) == "1" ? this.bstnUseHarv.IsChecked = true : this.bstnUseHarv.IsChecked = false;


                }

            }
            catch
            {
                notifier.ShowError("Failed to Load selected Task");
            }
        }

        private void delBSTN(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                File.Delete(this.bstnTaskN.Text + "_bstn.json");
                this.bstnURL.Text = "";
                this.bstnSizes.Clear();
                this.bstnCproxy.Text = "";
                this.bstnProxy.Text = "";
                this.bstnProfile.Text = "";
                this.bstnTaskAmount.Text = "";
                this.bstnUseHarv.IsChecked = false;

                this.bstnTaskN.Items.Remove(this.bstnTaskN.Text);

            }
            catch
            {
                this.notifier.ShowError("Failed to save Taskgroup. Does it even exist?");
            }
        }

        private void bstnEditandLoad(object sender, RoutedEventArgs e)
        {
            try
            {
                this.TaskPanel.Visibility = Visibility.Collapsed;
                this.taskManager.Visibility = Visibility.Visible;
                triggerBstnSelection(null, null);
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                using (StreamReader streamReader = new StreamReader(this.bstnCombo.Text + "_bstn.json"))
                {
                    dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                    this.bstnURL.Text = (string)(obj1["link"]);
                    this.bstnSizes.Text = (string)(obj1["sizes"]);
                    this.bstnProfile.Text = (string)(obj1["profiles"]);
                    this.bstnProxy.Text = (string)(obj1["proxylist"]);
                    this.bstnDelay.Text = (string)(obj1["retrydelay"]);
                    this.bstnTaskAmount.Text = (string)(obj1["threads"]);
                    this.bstnTaskN.Text = this.bstnCombo.Text;
                    this.bstnCproxy.Text = (string)(obj1["cproxy"]);
                    _ = (string)(obj1["useharv"]) == "1" ? this.bstnUseHarv.IsChecked = true : this.bstnUseHarv.IsChecked = false;
                    _ = (string)(obj1["threeDs"]) == "1" ? this.bstnBrowser3Ds.IsChecked = true : this.bstnBrowser3Ds.IsChecked = false;


                }
            }
            catch
            {
                this.notifier.ShowError("Failed to load task!");
            }
        }

        private void trigger43einSel(object sender, RoutedEventArgs e)
        {
            try
            {
                this.TaskPanel.Visibility = Visibility.Collapsed;
                this.taskManager.Visibility = Visibility.Visible;
                trigger43einOptions(null, null);
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                using (StreamReader streamReader = new StreamReader(this.einhCombo.Text + "_43ein.json"))
                {
                    dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                    this.einhalbURL.Text = (string)(obj1["link"]);
                    this.einhalbSizes.Text = (string)(obj1["sizes"]);
                    this.einhalbProfile.Text = (string)(obj1["profiles"]);
                    this.einhalbProxylist.Text = (string)(obj1["proxylist"]);
                    this.einDelay.Text = (string)(obj1["retrydelay"]);
                    this.einhTasks.Text = (string)(obj1["threads"]);
                    this.einPassword.Text = (string)(obj1["password"]);
                    this.einhUsern.Text = (string)(obj1["username"]);
                    this.einhalbCproxy.Text = (string)(obj1["cproxy"]);
                    this.einhalbTaskn.Text = this.einhCombo.Text;
                    _ = (string)(obj1["useharv"]) == "1" ? this.einhHarv.IsChecked = true : this.einhHarv.IsChecked = false;
                    _ = (string)(obj1["cad"]) == "1" ? this.einhCad.IsChecked = true : this.einhCad.IsChecked = false;
                    _ = (string)(obj1["extension"]) == "1" ? this.einhChrome.IsChecked = true : this.einhChrome.IsChecked = false;
                    _ = (string)(obj1["instore"]) == "1" ? this.einInstore.IsChecked = true : this.einInstore.IsChecked = false;
                }

            }
            catch
            {
                notifier.ShowError("Failed to Load selected Task");
            }
        }



        private void editLoadSegons(object sender, RoutedEventArgs e)
        {
            try
            {
                this.TaskPanel.Visibility = Visibility.Collapsed;
                this.taskManager.Visibility = Visibility.Visible;
                showSegons(null, null);
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                using (StreamReader streamReader = new StreamReader(this.segonsCombo.Text + "_segons.json"))
                {
                    dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                    this.segonsURL.Text = (string)(obj1["link"]);
                    this.segonsSizes.Text = (string)(obj1["sizes"]);
                    this.segonsProfile.Text = (string)(obj1["profiles"]);
                    this.segonsProxylist.Text = (string)(obj1["proxylist"]);
                    this.segonsDelay.Text = (string)(obj1["retrydelay"]);
                    this.segonsTaskAmount.Text = (string)(obj1["threads"]);
                    this.segonsTaskName.Text = this.segonsCombo.Text;
                    if ((obj1["earlyLogin"]).ToString().Contains("1"))
                    {
                        this.segonsEarlyLogin.IsChecked = true;
                    }if ((obj1["clearCart"]).ToString().Contains("1"))
                    {
                        this.segonsCartClear.IsChecked = true;
                    }
                }

            }
            catch
            {
                notifier.ShowError("Failed to Load selected Task");
            }
        }

        private void loadEditB4b(object sender, RoutedEventArgs e)
        {
            try
            {
                this.TaskPanel.Visibility = Visibility.Collapsed;
                this.taskManager.Visibility = Visibility.Visible;
                triggerB4bOptions(null, null);
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                using (StreamReader streamReader = new StreamReader(b4bCombo.Text + "_b4b.json"))
                {
                    dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                    this.b4bURL.Text = (string)(obj1["link"]);
                    this.b4bSizes.Text = (string)(obj1["sizes"]);
                    this.b4bProfile.Text = (string)(obj1["profiles"]);
                    this.b4bProxyList.Text = (string)(obj1["proxylist"]);
                    this.b4bDelay.Text = (string)(obj1["retrydelay"]);
                    this.b4bTaskAmount.Text = (string)(obj1["threads"]);
                    b4bTaskName.Text = b4bCombo.Text;
                    _ = (string)(obj1["cookieATC"]) == "1" ? this.b4bCookieATC.IsChecked = true : this.b4bCookieATC.IsChecked = false;
                    _ = (string)(obj1["forceCaptcha"]) == "1" ? this.b4bForceaptcha.IsChecked = true : this.b4bForceaptcha.IsChecked = false;
                    _ = (string)(obj1["useHarvester"]) == "1" ? this.b4bUseHarv.IsChecked = true : this.b4bUseHarv.IsChecked = false;
                    _ = (string)(obj1["cartClear"]) == "1" ? this.b4bAccClear.IsChecked = true : this.b4bAccClear.IsChecked = false;
                    _ = (string)(obj1["earlyLogin"]) == "1" ? this.b4bEarlyLogin.IsChecked = true : this.b4bEarlyLogin.IsChecked = false;




                }
                this.b4bTaskName.Items.Add(b4bTaskName.Text);

            }
            catch
            {
                notifier.ShowError("Failed to Load selected Task");
            }
        }


        private void svdLoadEdit(object sender, RoutedEventArgs e)
        {
            try
            {
                this.TaskPanel.Visibility = Visibility.Collapsed;
                this.taskManager.Visibility = Visibility.Visible;
                triggerSvdOptions(null, null);
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                using (StreamReader streamReader = new StreamReader(this.svdCombo.Text + "_svd.json"))
                {
                    dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                    this.svdURL.Text = (string)(obj1["link"]);
                    this.svdSizes.Text = (string)(obj1["sizes"]);
                    this.svdProfile.Text = (string)(obj1["profiles"]);
                    this.svdProxylist.Text = (string)(obj1["proxylist"]);
                    this.svdDelay.Text = (string)(obj1["retrydelay"]);
                    this.svdTaskName.Text = (string)(obj1["threads"]);
                    svdTaskName.Text = svdCombo.Text;

                }



            }
            catch
            {
                notifier.ShowError("Failed to Load selected Task");
            }
        }

        private void awlEditLoad(object sender, RoutedEventArgs e)
        {
            try
            {
                this.TaskPanel.Visibility = Visibility.Collapsed;
                this.taskManager.Visibility = Visibility.Visible;
                triggerAwlOptions(null, null);
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                using (StreamReader streamReader = new StreamReader(this.awCombo.Text + "_awlab.json"))
                {
                    dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                    this.awlUrl.Text = (string)(obj1["link"]);
                    this.awlSizes.Text = (string)(obj1["sizes"]);
                    this.awlProfile.Text = (string)(obj1["profiles"]);
                    this.awlProxylist.Text = (string)(obj1["proxylist"]);
                    this.awlCproxy.Text = (string)(obj1["cproxy"]);
                    this.awlDelay.Text = (string)(obj1["retrydelay"]);
                    this.awlTaskAmount.Text = (string)(obj1["threads"]);
                    this.awlDiscount.Text = (string)(obj1["discount"]);
                    awlTaskName.Text = awCombo.Text;
                    _ = (string)(obj1["cod"]) == "1" ? this.awlCOD.IsChecked = true : this.awlCOD.IsChecked = false;
                    _ = (string)(obj1["threeDs"]) == "1" ? this.awl3ds.IsChecked = true : this.awl3ds.IsChecked = false;

                }


            }
            catch
            {
                notifier.ShowError("Failed to Load selected Task");
            }
        }

        private void showCookies(object sender, RoutedEventArgs e)
        {
            TaskPanel.Visibility = Visibility.Collapsed;
            ProfileManager.Visibility = Visibility.Collapsed;
            taskManager.Visibility = Visibility.Collapsed;
            accPanel.Visibility = Visibility.Collapsed;
            SettingsPage.Visibility = Visibility.Collapsed;
            proxyPanel.Visibility = Visibility.Collapsed;
            proxyPanel.Visibility = Visibility.Collapsed;

            cookieTab.Visibility = Visibility.Visible;
            Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
            Directory.SetCurrentDirectory(Directory.CreateDirectory("kekse").ToString());
        }

        async private void startBstn(object sender, RoutedEventArgs e)
        {
            try
            {
                if (bstnSchedule.IsChecked == true)
                {
                    var t = int.Parse(minsBstn.Text);
                    string linkConstruct = $"http://127.0.0.1:8888/scheduled?site=bstn&taskId={bstnCombo.Text}&seconds={t * 60}";
                    await taskClient.GetAsync(linkConstruct);
                }
                else
                {
                    await taskClient.GetAsync($"http://127.0.0.1:8888/startTask?site=bstn&taskId={bstnCombo.Text}");
                }
            }
            catch
            {

            }
        }

        async private void startEinh(object sender, RoutedEventArgs e)
        {
            try
            {
                if (einhSchedule.IsChecked == true)
                {
                    var t = int.Parse(minsEinh.Text);
                    string linkConstruct = $"http://127.0.0.1:8888/scheduled?site=43ein&taskId={einhCombo.Text}&seconds={t * 60}";
                    await taskClient.GetAsync(linkConstruct);
                }
                else
                {
                    await taskClient.GetAsync($"http://127.0.0.1:8888/startTask?site=43ein&taskId={einhCombo.Text}");
                }
            }
            catch
            {

            }
        }

        async private void segonsStart(object sender, RoutedEventArgs e)
        {
            try
            {
                if (segonsSchedule.IsChecked == true)
                {
                    var t = int.Parse(mins24segons.Text);
                    string linkConstruct = $"http://127.0.0.1:8888/scheduled?site=segons&taskId={segonsCombo.Text}&seconds={t * 60}";
                    await taskClient.GetAsync(linkConstruct);
                }
                else
                {
                    await taskClient.GetAsync($"http://127.0.0.1:8888/startTask?site=segons&taskId={segonsCombo.Text}");
                }
            }
            catch
            {

            }
        }
        async private void startB4B(object sender, RoutedEventArgs e)
        {
            try
            {
                if (b4bSchedule.IsChecked == true)
                {
                    var t = int.Parse(minsB4b.Text);
                    string linkConstruct = $"http://127.0.0.1:8888/scheduled?site=b4b&taskId={b4bCombo.Text}&seconds={t * 60}";
                    await taskClient.GetAsync(linkConstruct);
                }
                else
                {
                    await taskClient.GetAsync($"http://127.0.0.1:8888/startTask?site=b4b&taskId={b4bCombo.Text}");
                }
            }
            catch
            {

            }
        }


        async private void svdStart(object sender, RoutedEventArgs e)
        {
            try
            {
                if (svdSchedule.IsChecked == true)
                {
                    var t = int.Parse(minsSvd.Text);
                    string linkConstruct = $"http://127.0.0.1:8888/scheduled?site=svd&taskId={svdCombo.Text}&seconds={t * 60}";
                    await taskClient.GetAsync(linkConstruct);
                }
                else
                {
                    await taskClient.GetAsync($"http://127.0.0.1:8888/startTask?site=svd&taskId={svdCombo.Text}");
                }
            }
            catch
            {

            }
        }
        async private void startAwl(object sender, RoutedEventArgs e)
        {
            try
            {
                if (awlabSchedule.IsChecked == true)
                {
                    var t = int.Parse(minsAw.Text);
                    string linkConstruct = $"http://127.0.0.1:8888/scheduled?site=awlab&taskId={awCombo.Text}&seconds={t * 60}";
                    await taskClient.GetAsync(linkConstruct);
                }
                else
                {
                    await taskClient.GetAsync($"http://127.0.0.1:8888/startTask?site=awlab&taskId={awCombo.Text}");
                }
            }
            catch
            {

            }
        }

        async private void bstnStop(object sender, RoutedEventArgs e)
        {
            try
            {
                await taskClient.GetAsync($"http://127.0.0.1:8888/stopTask?id={bstnCombo.Text}");
            }
            catch
            {

            }
        }
        async private void einhStop(object sender, RoutedEventArgs e)
        {
            try
            {
                await taskClient.GetAsync($"http://127.0.0.1:8888/stopTask?id={einhCombo.Text}");
            }
            catch
            {

            }
        }
        async private void segonsStop(object sender, RoutedEventArgs e)
        {
            try
            {
                await taskClient.GetAsync($"http://127.0.0.1:8888/stopTask?id={segonsCombo.Text}");
            }
            catch
            {

            }
        }
        async private void b4bStop(object sender, RoutedEventArgs e)
        {
            try
            {
                await taskClient.GetAsync($"http://127.0.0.1:8888/stopTask?id={b4bCombo.Text}");
            }
            catch
            {

            }
        }


        async private void svdStop(object sender, RoutedEventArgs e)
        {
            try
            {
                await taskClient.GetAsync($"http://127.0.0.1:8888/stopTask?id={svdCombo.Text}");
            }
            catch
            {

            }
        }

        async private void awlStop(object sender, RoutedEventArgs e)
        {
            try
            {
                await taskClient.GetAsync($"http://127.0.0.1:8888/stopTask?id={awCombo.Text}");
            }
            catch
            {

            }
        }

        private void startFS(object sender, RoutedEventArgs e)
        {

        }

        private void loadEditFS(object sender, RoutedEventArgs e)
        {

        }

        private void FSStop(object sender, RoutedEventArgs e)
        {

        }

        private void montroseSave(object sender, RoutedEventArgs e)
        {

            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, string> settings = new Dictionary<string, string>();

                settings.Add("link", this.montroseUrl.Text);
                settings.Add("sizes", this.montroseSizes.Text);
                settings.Add("proxylist", this.montroseProxylist.Text);
               
                settings.Add("profiles", this.montroseProfile.Text);
                settings.Add("retrydelay", this.montroseDelay.Text);
                settings.Add("threads", this.montroseTaskAmount.Text);
               
                this.montroseTaskName.Items.Add(awlTaskName.Text);
                using (StreamWriter taskWriter = new StreamWriter(montroseTaskName.Text + "_montrose.json"))
                {
                    serializer.Serialize(taskWriter, settings);
                }
                this.notifier.ShowSuccess("Taskgroup saved successfully!");
            }
            catch
            {
                this.notifier.ShowError("Failed to Save Task!");
            }
        }

        private void montroseLoad(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                using (StreamReader streamReader = new StreamReader(this.montroseTaskName.Text + "_montrose.json"))
                {
                    dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                    this.montroseUrl.Text = (string)(obj1["link"]);
                    this.montroseSizes.Text = (string)(obj1["sizes"]);
                    this.montroseProfile.Text = (string)(obj1["profiles"]);
                    this.montroseProxylist.Text = (string)(obj1["proxylist"]);
                   
                    this.montroseDelay.Text = (string)(obj1["retrydelay"]);
                    this.montroseTaskAmount.Text = (string)(obj1["threads"]);
                   

                }


            }
            catch
            {
                notifier.ShowError("Failed to Load selected Task");
            }
        }

        private void delMontrose(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                File.Delete(this.awlTaskName.Text + "_segons.json");
                this.awlUrl.Text = "";
                this.awlSizes.Clear();
                this.awlProfile.Text = "";
                this.awlProxylist.Text = "";
                this.awlDelay.Text = "";
                this.awlCproxy.Text = "";
                this.awlDiscount.Text = "";
                this.awlCOD.IsChecked = false;
                awl3ds.IsChecked = false;
                this.awlTaskAmount.Text = "";

            }
            catch
            {
                this.notifier.ShowError("Failed to save Taskgroup. Does it even exist?");
            }
        }

        async private void startMontrose(object sender, RoutedEventArgs e)
        {
            try
            {
                if (scheduleMontrose.IsChecked == true)
                {
                    var t = int.Parse(minsmontrose.Text);
                    string linkConstruct = $"http://127.0.0.1:8888/scheduled?site=montrose&taskId={montroseCombo.Text}&seconds={t * 60}";
                    await taskClient.GetAsync(linkConstruct);
                }
                else
                {
                    await taskClient.GetAsync($"http://127.0.0.1:8888/startTask?site=montrose&taskId={montroseCombo.Text}");
                }
            }
            catch
            {

            }
        }

        private void loadEditMontrose(object sender, RoutedEventArgs e)
        {
           
                try
                {
                    this.TaskPanel.Visibility = Visibility.Collapsed;
                    this.taskManager.Visibility = Visibility.Visible;
                    triggerMontroseOptions(null, null);
                   
                 Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                    Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                    Directory.SetCurrentDirectory(Directory.CreateDirectory("tasks").ToString());
                    using (StreamReader streamReader = new StreamReader(this.montroseCombo.Text + "_montrose.json"))
                    {
                        dynamic obj1 = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                        this.montroseUrl.Text = (string)(obj1["link"]);
                        this.montroseSizes.Text = (string)(obj1["sizes"]);
                        this.montroseProfile.Text = (string)(obj1["profiles"]);
                        this.montroseProxylist.Text = (string)(obj1["proxylist"]);

                        this.montroseDelay.Text = (string)(obj1["retrydelay"]);
                        this.montroseTaskAmount.Text = (string)(obj1["threads"]);


                    }


                }
                catch
                {
                    notifier.ShowError("Failed to Load selected Task");
                }
            
        }

        async private void montroseStop(object sender, RoutedEventArgs e)
        {
            try
            {
                await taskClient.GetAsync($"http://127.0.0.1:8888/stopTask?id={montroseCombo.Text}");
            }
            catch
            {

            }
        }

        private void saveTurbo(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("config").ToString());
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, string> settings = new Dictionary<string, string>();
                settings.Add("threads", turboThreads.Text);
                settings.Add("domain", turboDomain.Text);
                turboThh = turboThreads.Text;
                using (StreamWriter settingWriter = new StreamWriter("turboSettings.json"))
                {
                    serializer.Serialize(settingWriter, settings);
                }
                notifier.ShowSuccess("Saved Settings successfully");
            }
            catch
            {

            }
        }

        private void delTurbo(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                Directory.SetCurrentDirectory(Directory.CreateDirectory("BlackOut 2.0").ToString());
                Directory.SetCurrentDirectory(Directory.CreateDirectory("config").ToString());
                File.Delete("turboSettings.json");
                this.notifier.ShowInformation("Deleted Turbo-2Cap Settings");
            }
            catch
            {

            }
        }

        async private void startTurbo(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpClient turboClient = new HttpClient();
                await turboClient.GetAsync($"http://127.0.0.1:8888/turbo/start?threads={turboThh}&apikey={twoCapApiKeyS}");

            }
            catch
            {
                this.notifier.ShowError("Failed to start Turbo 2Cap Harvester");
            }
        }

        async private void stopTurbo(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpClient turboClient = new HttpClient();
                await turboClient.GetAsync($"http://127.0.0.1:8888/turbo/stop");

            }
            catch
            {
                this.notifier.ShowError("Failed to start Turbo 2Cap Harvester");
            }
        }
    }

}
