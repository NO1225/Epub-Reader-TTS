namespace Epub_Reader_TTS
{
    public class Program
    {
        [System.STAThreadAttribute()]
        public static void Main()
        {
            using (new Speaker.App())
            {
                App app = new App();
                app.InitializeComponent();
                app.Run();
            }

        }
    }
}
