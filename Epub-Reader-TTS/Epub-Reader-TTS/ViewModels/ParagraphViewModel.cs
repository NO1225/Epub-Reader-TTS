using System;
using System.Collections.Generic;
using System.Text;

namespace Epub_Reader_TTS
{
    public class ParagraphViewModel : BaseViewModel
    {
        #region Public Properties

        public Action<int> OnFinnished;

        public bool Active { get; set; }

        public int Index { get; set; }

        public int WordIndex { get; set; }

        public int WordLength { get; set; }

        public string ParagraphText { get; set; }

        #endregion


        #region Private Methods

        private void Finnished()
        {
            if (OnFinnished != null)
                OnFinnished(this.Index);
        }

        #endregion
    }
}
