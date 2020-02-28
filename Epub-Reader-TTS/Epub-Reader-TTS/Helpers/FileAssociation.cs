using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Epub_Reader_TTS
{
    public class FileAssociation
    {
        public string Extension { get; set; }
        public string ProgId { get; set; }
        public string FileTypeDescription { get; set; }
        public string ExecutableFilePath { get; set; }
        public string FileType { get; internal set; }
    }

    public class FileAssociations
    {
        // needed so that Explorer windows get refreshed after the registry is updated
        [System.Runtime.InteropServices.DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const int SHCNF_FLUSH = 0x1000;

        public static void EnsureAssociationsSet()
        {

            var filePath = Process.GetCurrentProcess().MainModule;

            var epubSampleName = "epub-sample.epub";

            var epubSample = filePath.FileName.Substring(0, filePath.FileName.Length - filePath.FileName.Split("\\").Last().Length) + epubSampleName;

            var a = GetExecFileAssociatedToExtension(".epub");

            if (GetExecFileAssociatedToExtension(".epub") != filePath.FileName)
            {
                var result = MessageBox.Show("Do you want to open epub files by default with this program?", "Default Program", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if(result == MessageBoxResult.Yes)
                {

                    EnsureAssociationsSet(
                                new FileAssociation
                                {
                                    Extension = ".epub",
                                    ProgId = filePath.ModuleName,
                                    FileTypeDescription = "EPUB File",
                                    ExecutableFilePath = filePath.FileName,
                                    FileType = "E-Book"
                                });

                    ShowFileProperties(epubSample);

                    // TODO: Show a windows that explain what the user has to do
                    MessageBox.Show("Our hero, this is your last mession before being able to make this the default program... \nYou are required to press Change from the shown window then select this program, \nPress Ok, then press Ok and you are done, Hero...", "A mession to save the world",
                        MessageBoxButton.OK,MessageBoxImage.Information);

                }
            }


        }

        private static void EnsureAssociationsSet(params FileAssociation[] associations)
        {
            bool madeChanges = false;
            foreach (var association in associations)
            {
                madeChanges |= SetAssociation(
                    association.Extension,
                    association.ProgId,
                    association.FileTypeDescription,
                    association.FileType,
                    association.ExecutableFilePath);
            }

            if (madeChanges)
            {
                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private static bool SetAssociation(string extension, string progId, string fileTypeDescription, string fileType, string applicationFilePath)
        {
            bool madeChanges = false;
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + extension, "");
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + extension + @"\PerceivedType\", fileType);

            madeChanges |= SetKeyDefaultValue(@"Software\Classes\Applications\" + progId, "");
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\Applications\" + progId + @"\shell\open\command", $"\"{applicationFilePath}\" \"%1\"");

            madeChanges |= SetKeyDefaultValue($@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\{extension}", "");
            madeChanges |= SetKeyDefaultValue($@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\{extension}\OpenWithList", $@"{progId}", "b");
            madeChanges |= SetKeyDefaultValue($@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\{extension}\OpenWithList", $@"b", "MRUList");
            return madeChanges;
        }

        private static bool SetKeyDefaultValue(string keyPath, string value, string name = null)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(keyPath, true))
            {
                var a = key.GetValue(name);
                if (key.GetValue(name) as string != value)
                {
                    key.SetValue(name, value);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="verb"></param>
        /// <returns>Return null if not found</returns>
        private static string GetExecFileAssociatedToExtension(string ext, string verb = null)
        {
            if (ext[0] != '.')
            {
                ext = "." + ext;
            }

            string executablePath = FileExtentionInfo(AssocStr.Executable, ext, verb); // Will only work for 'open' verb
            if (string.IsNullOrEmpty(executablePath))
            {
                executablePath = FileExtentionInfo(AssocStr.Command, ext, verb); // required to find command of any other verb than 'open'

                // Extract only the path
                if (!string.IsNullOrEmpty(executablePath) && executablePath.Length > 1)
                {
                    if (executablePath[0] == '"')
                    {
                        executablePath = executablePath.Split('\"')[1];
                    }
                    else if (executablePath[0] == '\'')
                    {
                        executablePath = executablePath.Split('\'')[1];
                    }
                }
            }

            // Ensure to not return the default OpenWith.exe associated executable in Windows 8 or higher
            if (!string.IsNullOrEmpty(executablePath) && File.Exists(executablePath) &&
                !executablePath.ToLower().EndsWith(".dll"))
            {
                if (executablePath.ToLower().EndsWith("openwith.exe"))
                {
                    return null; // 'OpenWith.exe' is th windows 8 or higher default for unknown extensions. I don't want to have it as associted file
                }
                return executablePath;
            }
            return executablePath;
        }

        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint AssocQueryString(AssocF flags, AssocStr str, string pszAssoc, string pszExtra, [Out] StringBuilder pszOut, [In][Out] ref uint pcchOut);

        private static string FileExtentionInfo(AssocStr assocStr, string doctype, string verb)
        {
            uint pcchOut = 0;
            AssocQueryString(AssocF.Verify, assocStr, doctype, verb, null, ref pcchOut);

            Debug.Assert(pcchOut != 0);
            if (pcchOut == 0)
            {
                return "";
            }

            StringBuilder pszOut = new StringBuilder((int)pcchOut);
            AssocQueryString(AssocF.Verify, assocStr, doctype, verb, pszOut, ref pcchOut);
            return pszOut.ToString();
        }

        [Flags]
        private enum AssocF
        {
            Init_NoRemapCLSID = 0x1,
            Init_ByExeName = 0x2,
            Open_ByExeName = 0x2,
            Init_DefaultToStar = 0x4,
            Init_DefaultToFolder = 0x8,
            NoUserSettings = 0x10,
            NoTruncate = 0x20,
            Verify = 0x40,
            RemapRunDll = 0x80,
            NoFixUps = 0x100,
            IgnoreBaseClass = 0x200
        }

        private enum AssocStr
        {
            Command = 1,
            Executable,
            FriendlyDocName,
            FriendlyAppName,
            NoOpen,
            ShellNewValue,
            DDECommand,
            DDEIfExec,
            DDEApplication,
            DDETopic
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        private const int SW_SHOW = 5;
        private const uint SEE_MASK_INVOKEIDLIST = 12;
        private static bool ShowFileProperties(string Filename)
        {
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = Filename;
            info.nShow = SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            return ShellExecuteEx(ref info);
        }


    }
}