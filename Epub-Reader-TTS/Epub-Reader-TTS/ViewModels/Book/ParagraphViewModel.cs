using System;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// View model to store the paragraph details
    /// </summary>
    public class ParagraphViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Action to be called when the reading of this paragraph is finnished
        /// </summary>
        public Action<int> OnFinnished;

        /// <summary>
        /// If this paragpraph is active and the application is currently reading it 
        /// </summary>
        public bool Active { get; set; }

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

        #endregion
        
        #region Private Methods

        /// <summary>
        /// To fire the action stored to be fired when this paragraph is finnished
        /// </summary>
        private void Finnished()
        {
            if (OnFinnished != null)
                OnFinnished(this.Index);
        }

        #endregion

    }
}
