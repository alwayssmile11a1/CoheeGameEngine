using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cohee.Editor
{
    public static class EditorHelper
    {
        public static readonly string CoheeLauncherExecFile = "CoheeLauncher.exe";
        public static readonly string BackupDirectory = "Backup";
        public static readonly string SourceDirectory = "Source";

        public static readonly string SourceMediaDirectory = Path.Combine(SourceDirectory, "Media");
        public static readonly string SourceCodeDirectory = Path.Combine(SourceDirectory, "Code");
        

    }
}