using System;
using System.Collections.Generic;
using System.Text;

namespace Epub_Reader_TTS.Core
{
    public class Book :BaseDataModel
    {
        public string BookName { get; set; }

        public string BookFilePath { get; set; }

        public string BookCoverPath { get; set; }

        public int CurrentPageIndex { get; set; }

        public int CurrentParagraphIndex { get; set; }

        public bool IsFinnished { get; set; }

        public DateTime LastOpenDate { get; set; }
    }
}
