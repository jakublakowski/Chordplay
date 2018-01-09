using ChordPlay.Helpers;
using ChordPlay.Models;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ChordPlay
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage instance;
        RecordHelper rh = new RecordHelper();
        Statistics statistics = new Statistics();
        List<string> chords = new List<string>() { "C", "A", "G", "D" };
        public ObservableCollection<Record> RecordingsList = new ObservableCollection<Record>();
        public ObservableCollection<Place> EventPlaces = new ObservableCollection<Place>();
        public ObservableCollection<Event> EventsList = new ObservableCollection<Event>();

        public MainPage()
        {
            this.InitializeComponent();

            instance = this;

            ChordsComboBox.ItemsSource = chords;
            ChordsComboBox.SelectedIndex = 0;
            PickChordComboBox.ItemsSource = chords;
            PickChordComboBox.SelectedIndex = 0;
            PlayRecordingButton.IsEnabled = false;
            SaveRecordingButton.IsEnabled = false;
            StopRecordingButton.IsEnabled = false;

            Init();
        }

        private void Init()
        {
            var task = Task.Run(async () => { await PopulateRecordings(); });
            task.Wait();

            task = Task.Run(async () => { await PopulatePlaces(); });
            task.Wait();

            CreateFoldersStructure();
            if (RecordingsList.Count > 0)
                GuessingHelper.PickNewRecord();
            statistics = StatisticsHelper.LoadStatistics();
            StatisticsListView.ItemsSource = statistics.StatisticsObservableList;
        }
        private async Task PopulateRecordings()
        {
            var recordings = await rh.GetRecordsPaths();
            foreach (var item in recordings)
            {
                RecordingsList.Add(new Record() { FilePath = item, Note = Path.GetDirectoryName(item) });
            }
        }

        private async Task PopulatePlaces()
        {
            var events = await EventHelper.GetPlaces();
            foreach (var item in events)
            {
                EventPlaces.Add(item);
            }
        }

        private async void CreateFoldersStructure()
        {
            if (ApplicationData.Current.LocalFolder.TryGetItemAsync("Recordings") == null)
                await ApplicationData.Current.LocalFolder.CreateFolderAsync("Recordings");
            StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Recordings");
            foreach (var chord in chords)
            {
                if (await ApplicationData.Current.LocalFolder.TryGetItemAsync($"Recordings\\{chord}") == null)
                    await folder.CreateFolderAsync(chord);
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            rootPivot.SelectedIndex = 0;
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            rootPivot.SelectedIndex = 1;
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            rootPivot.SelectedIndex = 2;
        }

        private void AppBarButton_Click_3(object sender, RoutedEventArgs e)
        {
            rootPivot.SelectedIndex = 3;
        }

        private void AppBarButton_Click_4(object sender, RoutedEventArgs e)
        {
            rootPivot.SelectedIndex = 4;
        }

        private void Record_Click(object sender, RoutedEventArgs e)
        {
            BlockUIForRecording();
            rh.Record();
        }

        private void StopRecording_Click(object sender, RoutedEventArgs e)
        {
            if (rh.Recording) ToastHelper.Notify("ChordPlay", "Finished recording");

            UnblockUIAfterRecording();
            rh.Stop();
        }


        private async void PlayRecording_Click(object sender, RoutedEventArgs e)
        {
            await rh.Play(Dispatcher);
        }

        private void SaveRecording_Click(object sender, RoutedEventArgs e)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string path = $"{(string)ChordsComboBox.SelectedValue}\\{timestamp}.m4a";
            rh.Save(path);
            RecordingsList.Add(new Record() { Note = (string)ChordsComboBox.SelectedValue, FilePath = path });
        }

        private void BlockUIForRecording()
        {
            rootPivot.IsLocked = true;
            CommandBar.IsEnabled = false;
            ChordsComboBox.IsEnabled = false;
            RecordButton.IsEnabled = false;
            PlayRecordingButton.IsEnabled = false;
            SaveRecordingButton.IsEnabled = false;
            StopRecordingButton.IsEnabled = true;
        }

        private void UnblockUIAfterRecording()
        {
            rootPivot.IsLocked = false;
            CommandBar.IsEnabled = true;
            ChordsComboBox.IsEnabled = true;
            RecordButton.IsEnabled = true;
            PlayRecordingButton.IsEnabled = true;
            SaveRecordingButton.IsEnabled = true;
        }

        private void TestRecording_Click(object sender, RoutedEventArgs e)
        {
            rh.PlayRecord((sender as Button).Tag.ToString());
        }

        private async void RemoveRecording_Click(object sender, RoutedEventArgs e)
        {
            var result = await DialogHelper.DisplayConfirmCancelDialog("Are you sure?", "Are you sure that you want to permanently delete the selected element?");
            if (result == ContentDialogResult.Primary)
            {
                string path = (sender as Button).Tag.ToString();
                rh.RemoveRecord(path);
                RecordingsList.Remove(RecordingsList.Where(x => x.FilePath == path).First());
            }
        }

        private void replay_Click(object sender, RoutedEventArgs e)
        {
            if (GuessingHelper.GetRecord() == null) return;
            rh.PlayRecord(GuessingHelper.GetRecord().FilePath);
            resultTextBlock.Text = "";
        }

        private void submit_Click(object sender, RoutedEventArgs e)
        {
            if (GuessingHelper.GetRecord() == null) return;
            rh.StopPlaying();
            if (GuessingHelper.GetRecord().Note == (string)PickChordComboBox.SelectedValue)
            {
                GuessingHelper.PickNewRecord();
                resultTextBlock.Text = "Good";
                statistics.AddGoodTryObservable(GuessingHelper.GetRecord().Note);
                StatisticsHelper.SaveStatistics(statistics);
            }
            else
            {
                resultTextBlock.Text = "Wrong";
                statistics.AddWrongTryObservable(GuessingHelper.GetRecord().Note);
                StatisticsHelper.SaveStatistics(statistics);
            }

            ToastHelper.Tile();
        }

        private void PickPlaceComboBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = EventPlaces.Where(x => x.name.ToLower().Contains(sender.Text.ToLower()));
            }
        }

        private async void PickPlaceComboBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            Place selectedPlace = (args.SelectedItem as Place);
            selectedEvent.Text = selectedPlace.address.ToString();
            EventsList.Clear();
            var events = await EventHelper.GetEvents(selectedPlace.id);
            foreach (var eventVar in events)
            {
                EventsList.Add(eventVar);
            }
        }

        private void ShowEventDetails_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button);
            int id = int.Parse(btn.Tag.ToString());
            Event eventVar = EventsList.Where(x => x.id == id).First();

            DialogHelper.DisplayInformationDialog("Description", eventVar.descShort);
        }

        private void ResetStatistics_Click(object sender, RoutedEventArgs e)
        {
            statistics.StatisticsObservableList.Clear();
            StatisticsHelper.SaveStatistics(statistics);
        }

        private void rootPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecordingsList.Count > 0)
                GuessingHelper.PickNewRecord();
        }
    }
}
