﻿using System;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FbCalendar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class testing : Page
    {
        public testing()
        {
            this.InitializeComponent();
        }

        public string[] Name = { "Keval", "Tejas", "Ravi" };

        ObservableCollection<string> myCollection = new ObservableCollection<string>();
        

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //myListView.ItemsSource = myCollection;
            foreach (var item in Name)
            {
                myCollection.Add(item);
            }

            myListView.ItemsSource = MainPage.oc_statuses;
        }

        private void selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var a = gv.SelectedIndex;
            var b = gv.SelectedItem;
            var c = gv.SelectedValue;
        }

        private void stopProgressingRing(object sender, TappedRoutedEventArgs e)
        {
            prgrsRing.IsActive = false;
        }
    }
}
