using System;
using System.Collections.Generic;
using System.Text;

namespace Epub_Reader_TTS.Core
{
    public class BaseDataModel
    {
        public Guid Id { get; set; }

        public bool IsDisabled { get; set; }

        public DateTime DisabledOn { get; set; }
    }
}
