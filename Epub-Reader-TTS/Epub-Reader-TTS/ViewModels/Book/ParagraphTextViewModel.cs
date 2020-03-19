using System;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// View model to store the paragraph details
    /// </summary>
    public class ParagraphTextViewModel : BaseViewModel
    {
        private bool active;

        #region Public Properties

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

        #endregion

        public double GetParagraphHeight(double allowedWidth, double fontSize)
        {
            return ParagraphText.GetParagraphHeight(allowedWidth, fontSize);
        }

    }
}
