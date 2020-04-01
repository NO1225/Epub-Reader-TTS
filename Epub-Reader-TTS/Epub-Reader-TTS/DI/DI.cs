using Dna;
using Epub_Reader_TTS.Core;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// A shorthand access class to get DI services with nice clean short code
    /// </summary>
    public static class DI
    {

        /// <summary>
        /// A shortcut to access the <see cref="ISettingsManager"/>
        /// </summary>
        public static ISettingsManager SettingsManager => Framework.Service<ISettingsManager>();

        /// <summary>
        /// A shortcut to access the <see cref="ISettingsManager"/>
        /// </summary>
        public static ISpeechSynthesizer SpeechSynthesizer;//=> Framework.Service<ISpeechSynthesizer>();

        /// <summary>
        /// A shortcut to access the <see cref="ApplicationViewModel"/>
        /// </summary>
        public static ApplicationViewModel ViewModelApplication => Framework.Service<ApplicationViewModel>();

        /// <summary>
        /// A shortcut to access the <see cref="DashboardViewModel"/>
        /// </summary>
        public static DashboardViewModel ViewModelDashboard => Framework.Service<DashboardViewModel>();

        /// <summary>
        /// A shortcut to access toe <see cref="IClientDataStore"/> service
        /// </summary>
        public static IClientDataStore ClientDataStore => Framework.Service<IClientDataStore>();

        /// <summary>
        /// A shortcut to access toe <see cref="IFileManager"/> service
        /// </summary>
        public static IFileManager FileManager => Framework.Service<IFileManager>();

        /// <summary>
        /// A shortcut to access toe <see cref="ITaskManager"/> service
        /// </summary>
        public static ITaskManager TaskManager => Framework.Service<ITaskManager>();
    }
}
