using Dna;
using System;
using System.Collections.Generic;
using System.Text;

namespace Epub_Reader_TTS.Core
{
    /// <summary>
    /// The IoC container for our application
    /// </summary>
    public static class CoreDI
    {
        /// <summary>
        /// A shortcut to access the <see cref="IFileManager"/>
        /// </summary>
        public static IFileManager FileManager => Framework.Service<IFileManager>();

        /// <summary>
        /// A shortcut to access the <see cref="ITaskManager"/>
        /// </summary>
        public static ITaskManager TaskManager => Framework.Service<ITaskManager>();
    }
}
