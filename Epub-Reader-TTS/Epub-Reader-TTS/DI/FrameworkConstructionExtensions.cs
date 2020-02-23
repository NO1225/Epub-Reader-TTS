using Dna;
using Epub_Reader_TTS.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// Extension methods for the <see cref="FrameworkConstruction"/>
    /// </summary>
    public static class FrameworkConstructionExtensions
    {
        /// <summary>
        /// Injects the view models needed for Fasetto Word application
        /// </summary>
        /// <param name="construction"></param>
        /// <returns></returns>
        public static FrameworkConstruction AddApplicationViewModels(this FrameworkConstruction construction)
        {
            // Bind to a single instance of Application view model
            construction.Services.AddSingleton<ApplicationViewModel>();

            // Bind to a single instance of Dashboard view model
            construction.Services.AddSingleton<DashboardViewModel>();

            // Return the construction for chaining
            return construction;
        }

        /// <summary>
        /// Injects the Fasetto Word client application services needed
        /// for the Fasetto Word application
        /// </summary>
        /// <param name="construction"></param>
        /// <returns></returns>
        public static FrameworkConstruction AddClientServices(this FrameworkConstruction construction)
        {

            // Add our task manager
            construction.Services.AddTransient<ITaskManager, BaseTaskManager>();

            // Bind a file manager
            construction.Services.AddTransient<IFileManager, BaseFileManager>();

            // Bind a Settings manager
            construction.Services.AddSingleton<ISettingsManager, SettingsManager>();


            construction.Services.AddSingleton<ISpeechSynthesizer, SpeechSynthesizer>();

            // Return the construction for chaining
            return construction;
        }

        /// <summary>
        /// Injects the Fasetto Word client application services needed
        /// for the Fasetto Word application
        /// </summary>
        /// <param name="construction"></param>
        /// <returns></returns>
        public static FrameworkConstruction EnsureFileAssosiation(this FrameworkConstruction construction)
        {
#if DEBUG

#else

            FileAssociations.EnsureAssociationsSet();

#endif
            FileAssociations.EnsureAssociationsSet();

            var a = FileAssociations.GetExecFileAssociatedToExtension("epub","open");

            // Return the construction for chaining
            return construction;
        }
    }
}
