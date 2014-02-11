using FbCalendar.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace FbCalendar
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ExploreStatuses : FbCalendar.Common.LayoutAwarePage
    {
        public ExploreStatuses()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        List<string> list_years = new List<string>();
        List<string> list_month = new List<string>()
            {
                "January", "February", "March", "April", "May", "June", "July",
                "August", "September", "October", "November", "December"
            };
        // string date;
        Status objStatus = new Status();
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //base.OnNavigatedTo(e);

            myListView.ItemsSource = MainPage.oc_statuses;


            foreach(var oneStatus in MainPage.oc_statuses)
            {
                objStatus.date = oneStatus.str_updatedTime.Substring(0, 10);

                char[] separator = { '-' };
                string[] separatDate = objStatus.date.Split(separator);

                string year = separatDate[0];
                string month = separatDate[1];

                if (list_years.Count == 0)
                {
                    list_years.Add(year);
                }
                else
                {
                    if (!(year == list_years.Last()))
                    {
                        list_years.Add(year);
                    }
                }
            }

            // AppBar_Top.IsOpen = true;
            gv_appBar_years.ItemsSource = list_years;
            gv_appBar_months.ItemsSource = list_month;
        }

        public static int numOfYears;
        public static int numOfMonth;
        public static string selectedYear;
        public static string selectedMonth;
        public static bool isYearSelected;
        public static bool isMonthSelected;
        private void yearChanged(object sender, SelectionChangedEventArgs e)
        {
            oc_sortedStatuses = null;
            oc_sortedStatuses = new ObservableCollection<Status>();

            gv_appBar_months.Visibility = Visibility.Visible;
            grid_noUpdates.Visibility = Visibility.Collapsed;

            numOfYears = list_years.Count();
            var index_selectedYear = gv_appBar_years.SelectedIndex;
            
            if(index_selectedYear == -1)
            {
                gv_appBar_months.Visibility = Visibility.Collapsed;

                myListView.Visibility = Visibility.Collapsed;
                tBlock_instruction.Visibility = Visibility.Visible;

                isYearSelected = false;
            }
            else
            {
                myListView.Visibility = Visibility.Visible;
                tBlock_instruction.Visibility = Visibility.Collapsed;

                isYearSelected = true;
                
                for (int i = 0; i <= numOfYears; i++)
                {
                    if(index_selectedYear == i)
                    {
                        selectedYear = list_years[i];
                    }
                }

                
                sortStatuses();

                if(oc_sortedStatuses.Count() == 0)
                {
                    myListView.Visibility = Visibility.Collapsed;
                    grid_noUpdates.Visibility = Visibility.Visible;
                }
                else
                {
                    myListView.ItemsSource = oc_sortedStatuses;
                }
            }

            if (!isYearSelected || !isMonthSelected)
            {
                myListView.Visibility = Visibility.Collapsed;
                tBlock_instruction.Visibility = Visibility.Visible;
                grid_noUpdates.Visibility = Visibility.Collapsed;
            }
            
        }

        private void monthChanged(object sender, SelectionChangedEventArgs e)
        {
            oc_sortedStatuses = null;
            oc_sortedStatuses = new ObservableCollection<Status>();

            numOfMonth = list_month.Count();
            var index_selectedMonth = gv_appBar_months.SelectedIndex;

            grid_noUpdates.Visibility = Visibility.Collapsed;

            if(index_selectedMonth == -1)
            {
                myListView.Visibility = Visibility.Collapsed;
                tBlock_instruction.Visibility = Visibility.Visible;

                isMonthSelected = false;
            }
            else
            {
                isMonthSelected = true;

                myListView.Visibility = Visibility.Visible;
                tBlock_instruction.Visibility = Visibility.Collapsed;

                for (int i = 0; i < 12; i++)
                {
                    if(index_selectedMonth == i)
                    {
                        selectedMonth = list_month[i];
                    }
                }

                AppBar_Top.IsOpen = false;
                sortStatuses();

                if (oc_sortedStatuses.Count() == 0)
                {
                    myListView.Visibility = Visibility.Collapsed;
                    grid_noUpdates.Visibility = Visibility.Visible;
                }
                else
                {
                    myListView.ItemsSource = oc_sortedStatuses;
                }
            }

            if (!isYearSelected || !isMonthSelected)
            {
                myListView.Visibility = Visibility.Collapsed;
                tBlock_instruction.Visibility = Visibility.Visible;
            }
        }

        ObservableCollection<Status> oc_sortedStatuses = new ObservableCollection<Status>();
        public void sortStatuses()
        {
            foreach(var checkStatus in MainPage.oc_statuses)
            {
                string date = checkStatus.str_updatedTime.Substring(0, 10);

                char[] separator = { '-' };
                string[] separatDate = date.Split(separator);

                string year = separatDate[0];
                string month = separatDate[1];

                #region month conversion
                switch(month)
                {
                    case "01":
                        month = "January";
                        break;

                    case "02":
                        month = "February";
                        break;

                    case "03":
                        month = "March";
                        break;

                    case "04":
                        month = "April";
                        break;

                    case "05":
                        month = "May";
                        break;

                    case "06":
                        month = "June";
                        break;

                    case "07":
                        month = "July";
                        break;

                    case "08":
                        month = "Augast";
                        break;

                    case "09":
                        month = "September";
                        break;

                    case "10":
                        month = "October";
                        break;

                    case "11":
                        month = "November";
                        break;

                    case "12":
                        month = "December";
                        break;
                }
#endregion

                if(year == selectedYear && month == selectedMonth)
                {
                    oc_sortedStatuses.Add(checkStatus);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //base.OnNavigatedFrom(e);

            isMonthSelected = false;
            isYearSelected = false;
        }

    }
}
