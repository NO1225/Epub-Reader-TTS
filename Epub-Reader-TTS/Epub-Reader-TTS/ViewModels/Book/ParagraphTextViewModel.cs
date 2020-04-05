using System;
using System.Windows.Input;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// View model to store the paragraph details
    /// </summary>
    public class ParagraphTextViewModel : BaseViewModel
    {
        #region Private Fields

        private bool active;

        #endregion

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

        #region Public COmmands

        /// <summary>
        /// Command to be called when we want to start reading from this paragraph
        /// </summary>
        public ICommand StartFromHereCommand { get; set; }

        #endregion

        #region Helping Methods

        /// <summary>
        /// Get the height of the text in this paragraph text
        /// </summary>
        /// <param name="allowedWidth">the allowed width</param>
        /// <param name="fontSize">the font size</param>
        /// <returns></returns>
        public double GetParagraphHeight(double allowedWidth, double fontSize)
        {
            return ParagraphText.GetParagraphHeight(allowedWidth, fontSize);
        }

        #endregion
    }
}
