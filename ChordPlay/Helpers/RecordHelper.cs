using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace ChordPlay.Helpers
{
    public class RecordHelper
    {
        private const string audioFilename = "temp.m4a";
        private string filename;
        private MediaCapture capture;
        MediaElement player = new MediaElement();
        private InMemoryRandomAccessStream buffer;

        public bool Recording;

        private async Task<bool> init()
        {
            if (buffer != null)
            {
                buffer.Dispose();
            }
            buffer = new InMemoryRandomAccessStream();
            if (capture != null)
            {
                capture.Dispose();
            }
            try
            {
                MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Audio
                };
                capture = new MediaCapture();
                await capture.InitializeAsync(settings);
                capture.RecordLimitationExceeded += (MediaCapture sender) =>
                {
                    Stop();
                    throw new Exception();
                };
                capture.Failed += (MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs) =>
                {
                    Recording = false;
                    throw new Exception();
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(UnauthorizedAccessException))
                {
                    throw ex.InnerException;
                }
                throw;
            }
            return true;
        }

        public async void Record()
        {
            await init();
            await capture.StartRecordToStreamAsync(MediaEncodingProfile.CreateM4a(AudioEncodingQuality.Auto), buffer);
            if (Recording) throw new InvalidOperationException();
            Recording = true;
        }

        public async void Stop()
        {
            if(Recording)
            {
                await capture.StopRecordAsync();
                Recording = false;
            }
        }

        public async Task Play(CoreDispatcher dispatcher)
        {
            MediaElement playback = new MediaElement();
            IRandomAccessStream audio = buffer.CloneStream();
            if (audio == null) throw new ArgumentNullException("buffer");
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            if (!string.IsNullOrEmpty(filename))
            {
                StorageFile original = await storageFolder.GetFileAsync(filename);
                await original.DeleteAsync();
            }
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                StorageFile storageFile = await storageFolder.CreateFileAsync(audioFilename, CreationCollisionOption.ReplaceExisting);
                filename = storageFile.Name;
                using (IRandomAccessStream fileStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await RandomAccessStream.CopyAndCloseAsync(audio.GetInputStreamAt(0), fileStream.GetOutputStreamAt(0));
                    await audio.FlushAsync();
                    audio.Dispose();
                }
                IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read);
                playback.SetSource(stream, "");
                playback.Play();
            });
        }

        public async void Save(string path)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            var tempRecordingFile = await storageFolder.GetFileAsync("temp.m4a");

            StorageFolder destFolder = await storageFolder.GetFolderAsync($"Recordings\\{Path.GetDirectoryName(path)}");

            await tempRecordingFile.CopyAsync(destFolder,$"{Path.GetFileNameWithoutExtension(path)}.m4a");
            
        }

        public async void PlayRecord(string path)
        {
                StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Recordings");
                StorageFile file = await folder.GetFileAsync(path);
                var stream2 = await file.OpenAsync(FileAccessMode.Read);
                player.SetSource(stream2, file.ContentType);
                player.Play();
        }

        public void StopPlaying()
        {
            player.Stop();
        }

        public async void RemoveRecord(string path)
        {
            StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Recordings");
            StorageFile file = await folder.GetFileAsync(path);
            await file.DeleteAsync();
        }

        private void DeleteAllRecords()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            foreach (var directory in isoStore.GetDirectoryNames())
            {
                foreach (var file in isoStore.GetFileNames($"{directory}/*"))
                {
                    isoStore.DeleteFile($"{directory}\\{file}");
                }
            }
        }

        public async Task<string[]> GetRecordsPaths()
        {
            List<string> paths = new List<string>();

            StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Recordings");
            var directories = await storageFolder.GetFoldersAsync();
            foreach (var directory in directories)
            {
                var files = await directory.GetFilesAsync();
                foreach (var file in files)
                {
                    paths.Add($"{directory.DisplayName}\\{file.Name}");
                }
            }
            return paths.ToArray();
        }
    }
}
