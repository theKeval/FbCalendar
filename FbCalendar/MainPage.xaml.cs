using Facebook;
using Facebook.Client;
using FbCalendar.Common;
using FbCalendar.Helper;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FbCalendar
{

    public class FBusers
    {
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }
    }



    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : LayoutAwarePage
    {

        private MobileServiceCollection<FBusers, FBusers> items;
        private IMobileServiceTable<FBusers> var_fbUser = App.MobileService.GetTable<FBusers>();

        public MainPage()
        {
           this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void FbLogin(object sender, TappedRoutedEventArgs e)
        {
            // customDialog_alreadySignedIn.IsOpen = true;

            if (!App.isAuthenticated)
            {
                App.isAuthenticated = true;
                
                rectDark.Visibility = Visibility.Visible;
                progressRing.IsActive = true;
                loading_stackPanel.Visibility = Visibility.Visible;

                await Authenticate();

                if (App.isAuthenticated)
                {
                    // rectDark.Visibility = Visibility.Visible;
                    // progressRing.IsActive = true;

                    await saveUserInfo();
                    await saveStatuses();
                    // sortStatuses();
                    //await saveFriendsInfo();

                    rectDark.Visibility = Visibility.Collapsed;
                    progressRing.IsActive = false;
                    loading_stackPanel.Visibility = Visibility.Collapsed;

                    MessageDialog msd = new MessageDialog("Login Successfull.");
                    await msd.ShowAsync();

                    this.Frame.Navigate(typeof(ExploreStatuses));
                }
                //Frame.Navigate(typeof(ShowSlambook));
            }
            else
            {
                // this.Frame.Navigate(typeof(ExploreStatuses));
                customDialog_alreadySignedIn.IsOpen = true;

                //MessageDialog msd = new MessageDialog("You already signed in to Facebook.");
                //await msd.ShowAsync();
            }
        }

        private FacebookSession session;
        private async Task Authenticate()
        {
            string message = String.Empty;
            try
            {
                session = await App.FacebookSessionClient.LoginAsync("user_about_me,read_stream, email, user_mobile_phone, user_address,user_status");
                App.AccessToken = session.AccessToken;
                App.FacebookId = session.FacebookId;

                //App.FBLoginRecord += 1;

            }
            catch (Exception e)
            {
                App.isAuthenticated = false;

                message = "Login failed!";  // Exception details: " + e.Message
                MessageDialog dialog = new MessageDialog(message);
                dialog.ShowAsync();

                progressRing.IsActive = false;
                rectDark.Visibility = Visibility.Collapsed;
                loading_stackPanel.Visibility = Visibility.Collapsed;
            }
        }

        public async Task saveUserInfo()
        {

            FacebookClient fbClient = new FacebookClient(App.AccessToken);

            //var me = await fbClient.GetTaskAsync("/me/statuses");
            var me = await fbClient.GetTaskAsync("me");
            var result = (IDictionary<string, object>)me;


            Constants.user_name = result["name"].ToString();
            Constants.user_eMail = result["email"].ToString();
            Constants.user_gender = result["gender"].ToString();

            //Constants.user_mobileNumber = result[""].ToString();
            //string lastName = result["last_name"].ToString();
            
            #region new code

            var userData = new FBusers { Name = Constants.user_name,
                                         Email = Constants.user_eMail,
                                         Gender = Constants.user_gender };
            InsertFBuserData(userData);

            #endregion
        }

        private async void InsertFBuserData(FBusers fbUserData)
        {
            // This code inserts a new TodoItem into the database. When the operation completes
            // and Mobile Services has assigned an Id, the item is added to the CollectionView
            await var_fbUser.InsertAsync(fbUserData);
            
            items = await var_fbUser.ToCollectionAsync();
            items.Add(fbUserData);
        }

        //List<Status> list_statuses = new List<Status>();
        public static ObservableCollection<Status> oc_statuses = new ObservableCollection<Status>();
        public async Task saveStatuses()
        {
            FacebookClient fbClient = new FacebookClient(App.AccessToken);
            
            int limit = 100;
            int offset = 0;
            string statusRequest = "/me/statuses?limit=100&offset="; //+ offset;

            var statusesTask = await fbClient.GetTaskAsync(statusRequest+offset);

            while (statusesTask != null)    // && ((Facebook.JsonArray)statusesTask).Count > 0
            {
                var result = (IDictionary<string, object>)statusesTask;
                var data = (IEnumerable<object>)result["data"];

                if (data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        try
                        {
                            var status = (IDictionary<string, object>)item;

                            oc_statuses.Add(new Status
                            {
                                message = status["message"].ToString(),
                                str_updatedTime = status["updated_time"].ToString(),
                                date = (status["updated_time"].ToString()).Substring(0,10)
                            }
                                );
                        }
                        catch (KeyNotFoundException exe)
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    break;
                }

                offset += limit;
                statusesTask = await fbClient.GetTaskAsync(statusRequest+offset);

            }

            #region Commented old code
            //var result = (IDictionary<string, object>)statusesTask;
            //var data = (IEnumerable<object>)result["data"];

            //List<Status> friends = new List<Status>();

            //foreach (var item in data)
            //{
            //    try
            //    {
            //        var status = (IDictionary<string, object>)item;

            //        friends.Add(new Status
            //        {
            //            message = status["message"].ToString(),
            //            str_updatedTime = status["updated_time"].ToString()
            //        }
            //            );
            //    }
            //    catch (KeyNotFoundException exe)
            //    {
            //        continue;
            //    }
            //}
            #endregion
        }

        string day, month, year;
        public void sortStatuses()
        {
            foreach(var oneStatus in oc_statuses)
            {
                string date = oneStatus.str_updatedTime.Substring(0, 10);

                char[] separator = {'-'};
                string[] separatDate = date.Split(separator);

                year = separatDate[0]; 
                month = separatDate[1];
                day = separatDate[2];
            }
        }

        private void customDialogBackButton(object sender, RoutedEventArgs e)
        {
            customDialog_alreadySignedIn.IsOpen = false;
        }

        private async void LoginAgainOnTapped(object sender, TappedRoutedEventArgs e)
        {
            customDialog_alreadySignedIn.IsOpen = false;

            

            App.isAuthenticated = true;

            rectDark.Visibility = Visibility.Visible;
            progressRing.IsActive = true;
            loading_stackPanel.Visibility = Visibility.Visible;

            await Authenticate();

            if (App.isAuthenticated)
            {
                oc_statuses = null;
                oc_statuses = new ObservableCollection<Status>();

                //rectDark.Visibility = Visibility.Visible;
                //progressRing.IsActive = true;

                await saveUserInfo();
                await saveStatuses();
                // sortStatuses();
                //await saveFriendsInfo();

                rectDark.Visibility = Visibility.Collapsed;
                progressRing.IsActive = false;
                loading_stackPanel.Visibility = Visibility.Collapsed;

                MessageDialog msd = new MessageDialog("Login Successfull.");
                await msd.ShowAsync();

                this.Frame.Navigate(typeof(ExploreStatuses));
            }
        }

        private void ContinueOnTapped(object sender, TappedRoutedEventArgs e)
        {
            customDialog_alreadySignedIn.IsOpen = false;
            this.Frame.Navigate(typeof(ExploreStatuses));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //base.OnNavigatedFrom(e);
        }

    }
}
