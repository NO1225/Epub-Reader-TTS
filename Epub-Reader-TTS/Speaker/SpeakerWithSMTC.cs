using Epub_Reader_TTS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Speaker
{

    public sealed class SpeakerWithSMTC : UserControl
    {
        public double Rate
        {
            get
            {
                return synthesizer.Options.SpeakingRate;
            }
            set
            {
                synthesizer.Options.SpeakingRate = value;
            }
        }

        public double Pitch
        {
            get
            {
                return synthesizer.Options.AudioPitch;
            }
            set
            {
                synthesizer.Options.AudioPitch = value;
            }
        }

        public Action<int, int> SpeakProgress { get; set; }

        public Action<CompletionReason> SpeakComplete { get; set; }

        public Action PlayPressed { get; set; }
        public Action PausePressed { get; set; }
        public Action NextPressed { get; set; }
        public Action PreviousPressed { get; set; }


        private MediaPlayerElement mediaPlayerElement;

        private SystemMediaTransportControls systemMediaTransportControls;

        private SpeechSynthesizer synthesizer;

        public SpeakerWithSMTC()
        {
            //InitializeComponent();
            synthesizer = new SpeechSynthesizer();
            mediaPlayerElement = new MediaPlayerElement();
            systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();

            var mediaPlayer = new MediaPlayer();
            mediaPlayer.AutoPlay = false;
            mediaPlayer.CommandManager.IsEnabled = false;
            mediaPlayer.MediaEnded += media_MediaEnded;

            mediaPlayerElement.SetMediaPlayer(mediaPlayer);

            synthesizer.Options.AppendedSilence = SpeechAppendedSilence.Min;

            systemMediaTransportControls.IsPlayEnabled = true;
            systemMediaTransportControls.IsNextEnabled = true;
            systemMediaTransportControls.IsPauseEnabled = true;
            systemMediaTransportControls.IsPreviousEnabled = true;

            systemMediaTransportControls.ButtonPressed += SystemMediaTransportControls_ButtonPressed;
        }

        private void SystemMediaTransportControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    PlayPressed?.Invoke();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    PausePressed?.Invoke();
                    break;
                case SystemMediaTransportControlsButton.Next:
                    NextPressed?.Invoke();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    PreviousPressed?.Invoke();
                    break;
                default:
                    break;
            }
        }

        public void UpdateSystemMediaTrasportControls(
            string title,
            string chapter,
            string coverPath = "",
            Epub_Reader_TTS.Core.MediaPlaybackStatus mediaPlaybackStatus = Epub_Reader_TTS.Core.MediaPlaybackStatus.Playing,
            bool isPLayEnabled = true,
            bool isNextEnabled = true,
            bool isPreviousEnabled = true,
            bool isPauseEnabled = true
            )
        {
            systemMediaTransportControls.IsPlayEnabled = isPLayEnabled;
            systemMediaTransportControls.IsNextEnabled = isNextEnabled;
            systemMediaTransportControls.IsPauseEnabled = isPauseEnabled;
            systemMediaTransportControls.IsPreviousEnabled = isPreviousEnabled;

            systemMediaTransportControls.PlaybackStatus = (Windows.Media.MediaPlaybackStatus)((int)mediaPlaybackStatus);

            var updater = systemMediaTransportControls.DisplayUpdater;

            updater.Type = MediaPlaybackType.Music;

            //updater.MusicProperties.Artist = "artist";
            updater.MusicProperties.AlbumArtist = chapter;
            updater.MusicProperties.Title = title;

            if(!string.IsNullOrEmpty(coverPath))
                updater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(coverPath));
            updater.Update();
        }

        /// <summary>
        /// This is invoked when the user clicks on the speak/stop button.
        /// </summary>
        /// <param name="sender">Button that triggered this event</param>
        /// <param name="e">State information about the routed event</param>
        public async Task SpeakAsync(string text)
        {
            // If the media is playing, the user has pressed the button to stop the playback.
            if (false && mediaPlayerElement.MediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                mediaPlayerElement.MediaPlayer.Pause();
                //btnSpeak.Content = "Speak";
            }
            else
            {
                if (!String.IsNullOrEmpty(text))
                {
                    try
                    {
                        // Enable word marker generation (false by default). 
                        synthesizer.Options.IncludeWordBoundaryMetadata = true;
                        //synthesizer.Options.IncludeSentenceBoundaryMetadata = true;

                        SpeechSynthesisStream synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(text);


                        // Create a media source from the stream: 
                        var mediaSource = MediaSource.CreateFromStream(synthesisStream, synthesisStream.ContentType);

                        //Create a Media Playback Item   
                        var mediaPlaybackItem = new MediaPlaybackItem(mediaSource);

                        // Ensure that the app is notified for cues.  
                        RegisterForWordBoundaryEvents(mediaPlaybackItem);

                        // Set the source of the MediaElement or MediaPlayerElement to the MediaPlaybackItem 
                        // and start playing the synthesized audio stream.

                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            mediaPlayerElement.Source = mediaPlaybackItem;
                            mediaPlayerElement.MediaPlayer.Play();
                        });


                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        // If media player components are unavailable, (eg, using a N SKU of windows), we won't
                        // be able to start media playback. Handle this gracefully

                        var messageDialog = new Windows.UI.Popups.MessageDialog("Media player components unavailable");
                        await messageDialog.ShowAsync();
                    }
                    catch (Exception)
                    {
                        // If the text is unable to be synthesized, throw an error message to the user.

                        var messageDialog = new Windows.UI.Popups.MessageDialog("Unable to synthesize text");
                        await messageDialog.ShowAsync();
                    }
                }
            }
        }

        /// <summary>
        /// This is invoked when the stream is finished playing.
        /// </summary>
        /// <param name="sender">unused object parameter</param>
        /// <param name="e">unused event parameter</param>
        async void media_MediaEnded(MediaPlayer sender, object e)
        {
            //await SpeakAsync("Done");
            SpeakComplete?.Invoke(CompletionReason.Complete);
        }

        /// <summary>
        /// This function executes when a SpeechCue is hit and calls the functions to update the UI
        /// </summary>
        /// <param name="timedMetadataTrack">The timedMetadataTrack associated with the event.</param>
        /// <param name="args">the arguments associated with the event.</param>
        private async void metadata_SpeechCueEntered(TimedMetadataTrack timedMetadataTrack, MediaCueEventArgs args)
        {
            // Check in case there are different tracks and the handler was used for more tracks 
            if (timedMetadataTrack.TimedMetadataKind == TimedMetadataKind.Speech)
            {
                var cue = args.Cue as SpeechCue;
                if (cue != null)
                {
                    SpeakProgress?.Invoke((int)cue.StartPositionInInput, (int)cue.EndPositionInInput - (int)cue.StartPositionInInput + 1);
                }
            }
        }

        /// <summary>
        /// Register for all boundary events and register a function to add any new events if they arise.
        /// </summary>
        /// <param name="mediaPlaybackItem">The Media PLayback Item add handlers to.</param>
        private void RegisterForWordBoundaryEvents(MediaPlaybackItem mediaPlaybackItem)
        {
            // If tracks were available at source resolution time, itterate through and register: 
            for (int index = 0; index < mediaPlaybackItem.TimedMetadataTracks.Count; index++)
            {
                RegisterMetadataHandlerForWords(mediaPlaybackItem, index);
            }

            // Since the tracks are added later we will  
            // monitor the tracks being added and subscribe to the ones of interest 
            mediaPlaybackItem.TimedMetadataTracksChanged += (MediaPlaybackItem sender, IVectorChangedEventArgs args) =>
            {
                if (args.CollectionChange == CollectionChange.ItemInserted)
                {
                    RegisterMetadataHandlerForWords(sender, (int)args.Index);
                }
                else if (args.CollectionChange == CollectionChange.Reset)
                {
                    for (int index = 0; index < sender.TimedMetadataTracks.Count; index++)
                    {
                        RegisterMetadataHandlerForWords(sender, index);
                    }
                }
            };
        }

        /// <summary>
        /// Register for just word boundary events.
        /// </summary>
        /// <param name="mediaPlaybackItem">The Media Playback Item to register handlers for.</param>
        /// <param name="index">Index of the timedMetadataTrack within the mediaPlaybackItem.</param>
        private void RegisterMetadataHandlerForWords(MediaPlaybackItem mediaPlaybackItem, int index)
        {
            var timedTrack = mediaPlaybackItem.TimedMetadataTracks[index];
            //register for only word cues
            if (timedTrack.Id == "SpeechWord")
            {
                timedTrack.CueEntered += metadata_SpeechCueEntered;
                mediaPlaybackItem.TimedMetadataTracks.SetPresentationMode((uint)index, TimedMetadataTrackPresentationMode.ApplicationPresented);
            }
        }

        public IEnumerable<InstalledVoice> GetInstalledVoices()
        {
            return SpeechSynthesizer.AllVoices.Select(v =>
                new InstalledVoice()
                {
                    Description = v.Description,
                    DisplayName = v.DisplayName,
                    Gender = (Epub_Reader_TTS.Core.VoiceGender)((int)v.Gender),
                    Language = v.Language,
                    Id = v.Id
                }
            );
        }


        public bool SelectVoice(InstalledVoice voice)
        {
            var voiceInformation = SpeechSynthesizer.AllVoices.FirstOrDefault(v => v.Id == voice.Id);

            if (voiceInformation != null)
            {
                synthesizer.Voice = voiceInformation;
                return true;
            }

            return false;
        }


        public InstalledVoice GetSelectedVoice()
        {
            return new InstalledVoice()
            {
                Description = synthesizer.Voice.Description,
                DisplayName = synthesizer.Voice.DisplayName,
                Gender = (Epub_Reader_TTS.Core.VoiceGender)((int)synthesizer.Voice.Gender),
                Id = synthesizer.Voice.Id,
                Language = synthesizer.Voice.Language,
            };
        }


        public async Task SpeakAsyncCancelAll()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    mediaPlayerElement.MediaPlayer.Pause();

                    mediaPlayerElement.Source = null;
                });


            SpeakComplete?.Invoke(CompletionReason.Cancel);
        }

    }

}
