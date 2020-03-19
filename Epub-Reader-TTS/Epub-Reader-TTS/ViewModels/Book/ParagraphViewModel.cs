using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// View model to store the paragraph details
    /// </summary>
    public class ParagraphViewModel : BaseViewModel
    {
        private bool active;

        #region Public Properties

        /// <summary>
        /// Action to be called when the reading of this paragraph is finnished
        /// </summary>
        public Action OnFinnished;

        /// <summary>
        /// If this paragpraph is active and the application is currently reading it 
        /// </summary>
        public bool Active
        {
            get => active; set
            {
                if(!value)
                {
                    WordIndex = 0;
                    WordLength = 0;
                }

                active = value;
            }
        }

        /// <summary>
        /// The index of this paragraph
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The currently spoken word
        /// </summary>
        public int WordIndex { get; set; }

        /// <summary>
        /// The length of currently spoken word
        /// </summary>
        public int WordLength { get; set; }

        /// <summary>
        /// The text of this paragraph
        /// </summary>
        public string ParagraphText { get; set; }

        //public ParagraphTextViewModel CurrentParagraphText { get; set; }

        public ObservableCollection<ParagraphTextViewModel> Paragraphs { get; set; }

        #endregion

        public double GetParagraphHeight(double allowedWidth, double fontSize)
        {
            return ParagraphText.GetParagraphHeight(allowedWidth, fontSize);
        }


        #region Private Methods

        /// <summary>
        /// To fire the action stored to be fired when this paragraph is finnished
        /// </summary>
        private void Finnished()
        {
            if (OnFinnished != null)
                OnFinnished();
        }

        internal void Split(string paragraphText, double currentAllowedHeight, double fullAllowedHeight, double allowedWidth, int fontSize)
        {
            if (Paragraphs == null)
                Paragraphs = new ObservableCollection<ParagraphTextViewModel>();

            var paragrpahHeight = paragraphText.GetParagraphHeight(allowedWidth, fontSize);
            if(paragrpahHeight<currentAllowedHeight)
            {
                Paragraphs.Add(new ParagraphTextViewModel()
                {
                    ParagraphText = paragraphText
                });
                return;
            }
            var ratio = currentAllowedHeight / paragrpahHeight;


            var decreasing = false;
            var increasing = false;

            var pattern = @"[^a-zA-Z0-9]";
            Regex regex = new Regex(pattern);

            var matches = regex.Matches(paragraphText);

            int startingIndex = (int)(matches.Count * ratio);


            while (true)
            {
                var firstHalf = paragraphText.Substring(0, matches[startingIndex].Index);
                string secondHalf;
                if (firstHalf.GetParagraphHeight(allowedWidth,fontSize)>currentAllowedHeight)
                {
                    decreasing = true;
                    startingIndex--;

                    if (increasing)
                    {
                        firstHalf = paragraphText.Substring(0, matches[startingIndex].Index);
                        secondHalf = paragraphText.Substring(matches[startingIndex].Index + 1 ,paragraphText.Length-(matches[startingIndex].Index + 1));

                        Paragraphs.Add(new ParagraphTextViewModel()
                        {
                            ParagraphText = firstHalf
                        });

                        Split(secondHalf, fullAllowedHeight, fullAllowedHeight, allowedWidth, fontSize);
                        break;
                    }
                }
                else
                {
                    increasing = true;
                    if (decreasing)
                    {
                        firstHalf = paragraphText.Substring(0, matches[startingIndex].Index);
                        secondHalf = paragraphText.Substring(matches[startingIndex].Index + 1, paragraphText.Length - (matches[startingIndex].Index + 1));

                        Paragraphs.Add(new ParagraphTextViewModel()
                        {
                            ParagraphText = firstHalf
                        });

                        Split(secondHalf, fullAllowedHeight, fullAllowedHeight, allowedWidth, fontSize);
                        break;
                    }
                    startingIndex++;
                }
            }
        }

        #endregion

    }
}
