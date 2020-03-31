using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// View model to store the paragraph details
    /// </summary>
    public class ParagraphViewModel : BaseViewModel
    {
        #region private fields

        private bool active;
        private int wordIndex;
        private int wordLength;

        #endregion

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
                active = value;
                if (!value)
                {
                    WordIndex = 0;
                    WordLength = 0;
                    SetWordIndexAndLength(0, 0);
                }
            }
        }

        /// <summary>
        /// The index of this paragraph
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The currently spoken word
        /// </summary>
        public int WordIndex
        {
            set => wordIndex = value;
        }

        /// <summary>
        /// The length of currently spoken word
        /// </summary>
        public int WordLength
        {
            set => wordLength = value;
        }

        /// <summary>
        /// The text of this paragraph
        /// </summary>
        public string ParagraphText { get; set; }

        /// <summary>
        /// List of parts on this paragraphs
        /// </summary>
        public ObservableCollection<ParagraphTextViewModel> Paragraphs { get; set; }

        #endregion
               
        #region Private Methods

        /// <summary>
        /// To fire the action stored to be fired when this paragraph is finnished
        /// </summary>
        private void Finnished()
        {
            if (OnFinnished != null)
                OnFinnished();
        }
        
        /// <summary>
        /// Split the paragraph according to the inputs
        /// </summary>
        /// <param name="paragraphText">the text to be splitted</param>
        /// <param name="currentAllowedHeight">the allowed height</param>
        /// <param name="fullAllowedHeight">the allwoed height for the next splice</param>
        /// <param name="allowedWidth">the allowed width</param>
        /// <param name="fontSize">the font size</param>
        private void Split(string paragraphText, double currentAllowedHeight, double fullAllowedHeight, double allowedWidth, int fontSize)
        {
            var paragrpahHeight = paragraphText.GetParagraphHeight(allowedWidth, fontSize);
            if (paragrpahHeight < currentAllowedHeight)
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

            var pattern = @"[ ]";
            Regex regex = new Regex(pattern);

            var matches = regex.Matches(paragraphText);

            int startingIndex = (int)(matches.Count * ratio);

            startingIndex = startingIndex >= matches.Count ? matches.Count - 1 : startingIndex;

            if (matches.Count == 77)
            {
                Debug.WriteLine("");
            }
            while (true)
            {
                string firstHalf, secondHalf;

                if((increasing||decreasing)&&startingIndex == matches.Count)
                {
                    firstHalf = paragraphText.Substring(0, matches[startingIndex-1].Index + 1);
                    secondHalf = paragraphText.Substring(matches[startingIndex-1].Index + 1, paragraphText.Length - (matches[startingIndex-1].Index + 1));

                    Paragraphs.Add(new ParagraphTextViewModel()
                    {
                        ParagraphText = firstHalf
                    });

                    Split(secondHalf, fullAllowedHeight, fullAllowedHeight, allowedWidth, fontSize);
                    break;
                }
                firstHalf = paragraphText.Substring(0, matches[startingIndex].Index);
                if (firstHalf.GetParagraphHeight(allowedWidth, fontSize) > currentAllowedHeight)
                {
                    decreasing = true;
                    startingIndex--;

                    if (increasing)
                    {
                        firstHalf = paragraphText.Substring(0, matches[startingIndex].Index + 1);
                        secondHalf = paragraphText.Substring(matches[startingIndex].Index + 1, paragraphText.Length - (matches[startingIndex].Index + 1));

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
                        firstHalf = paragraphText.Substring(0, matches[startingIndex].Index + 1);
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

        #region Public Methods

        /// <summary>
        /// Measure the height of this paragraph before splitting
        /// </summary>
        /// <param name="allowedWidth">the allowed width</param>
        /// <param name="fontSize">the font size</param>
        /// <returns></returns>
        public double GetParagraphHeight(double allowedWidth, double fontSize)
        {
            return ParagraphText.GetParagraphHeight(allowedWidth, fontSize);
        }

        /// <summary>
        /// Start the recursive splitting
        /// </summary>
        /// <param name="paragraphText">the text to be splitted</param>
        /// <param name="currentAllowedHeight">the allowed height</param>
        /// <param name="fullAllowedHeight">the allwoed height for the next splice</param>
        /// <param name="allowedWidth">the allowed width</param>
        /// <param name="fontSize">the font size</param>
        internal void StartSpliting(string paragraphText, double currentAllowedHeight, double fullAllowedHeight, double allowedWidth, int fontSize)
        {
            Paragraphs = new ObservableCollection<ParagraphTextViewModel>();
            Split(paragraphText, currentAllowedHeight, fullAllowedHeight, allowedWidth, fontSize);
        }

        /// <summary>
        /// Set the current highlighted word in the child parts
        /// </summary>
        /// <param name="characterPosition">the character position of the whole paragraph</param>
        /// <param name="characterCount">the word length</param>
        internal void SetWordIndexAndLength(int characterPosition, int characterCount)
        {
            if (Paragraphs == null)
            {
                return;
            }
            if (!Active)
            {
                foreach (var par in Paragraphs)
                {
                    par.Active = false;
                }
                return;
            }
            var sumOfLengths = 0;
            var nextSumOfLengths = 0;

            for (int i = 0; i < Paragraphs.Count; i++)
            {
                ParagraphTextViewModel splice = (ParagraphTextViewModel)Paragraphs[i];

                nextSumOfLengths += splice.ParagraphText.Length;

                if (characterPosition >= sumOfLengths && characterPosition < nextSumOfLengths - 1)
                {
                    splice.Active = true;
                    splice.WordLength = characterCount;
                    splice.WordIndex = characterPosition - sumOfLengths;
                }
                else
                {
                    splice.Active = false;
                }

                sumOfLengths = nextSumOfLengths;
            }
        } 
        #endregion
    }
}
