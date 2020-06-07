using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Linq;

namespace Extansion
{
    namespace IO
    {
        public static class IO_Ext
        {
            public static IEnumerable<FileInfo> GetFilesByPattern(this DirectoryInfo dir, params string[] pattern)
            {
                if (pattern == null)
                    throw new ArgumentNullException("pattern");
                IEnumerable<FileInfo> files = System.Linq.Enumerable.Empty<FileInfo>();
                foreach (string ext in pattern)
                {
                    files = files.Concat(dir.GetFiles(ext));
                }
                return files;
            }

            static void rec_Files(DirectoryInfo di, ref string list, bool last, List<char> pres, string trenn, char Right, char Cross, char Line)
            {
                string pre = "";

                pre = System.String.Join(trenn, pres) + trenn;
                list += pre + (last ? Right : Cross) + " " + di.Name + "\n";

                if (last)
                    pres.Add(' ');
                else
                    pres.Add(Line);

                IEnumerable<DirectoryInfo> dilist = di.EnumerateDirectories();
                for (int i = 0; i < dilist.Count(); i++)
                    rec_Files(dilist.ElementAt(i), ref list, ((i == dilist.Count() - 1) && !(di.GetFiles().Length != 0)), new List<char>(pres), trenn, Right, Cross, Line);
                
                pre = System.String.Join(trenn, pres) + trenn;

                IEnumerable<FileInfo> filist = di.EnumerateFiles();
                for (int i = 0; i < filist.Count(); i++)
                    if (i == filist.Count() - 1)
                        list += pre + Right + filist.ElementAt(i).Name + "\n";
                    else
                        list += pre + Cross + filist.ElementAt(i).Name + "\n";
            }

            public static string ListPaths(this DirectoryInfo info, int spacing)
            {
                string val = "";
                rec_Files(info, ref val, true, new List<char>(), new string(' ', spacing), '\\', '>', '|');
                return val;
            }

            public static string ListPaths(this DirectoryInfo info, int spacing, char line, char cross, char end)
            {
                string val = "";
                rec_Files(info, ref val, true, new List<char>(), new string(' ', spacing), end, cross, line);
                return val;
            }

            /// <summary>
            /// Recursive Funktion die für das Zählen der Datein und Verzeichnisse in Unterverzeichnissen benutzt wird
            /// </summary>
            /// <param name="di">Das Verzeichness dessen Datein und Verzeichnisse der Zählung hinzugefügt werden und dessen Unterverzeichnisse recursive durchsucht werden</param>
            /// <param name="files">Eine Referenz auf die Anzahl der gezählten Datein</param>
            /// <param name="files">Eine Referenz auf die Anzahl der gezählten Verzeichnisse</param>
            static void rec_count_file(DirectoryInfo di, ref int files, ref int dics)
            {
                foreach (DirectoryInfo di_sub in di.GetDirectories())
                    rec_count_file(di_sub, ref files, ref dics);
                files += di.GetFiles().Length;
                dics += di.GetDirectories().Length;
            }

            /// <summary>
            /// Zählt alle Datein die sich in dem Verzeichnis oder darunterliegenden Verzeichnissen befinden.
            /// </summary>
            /// <param name="info"></param>
            /// <returns>Die Anzahl aller Datein</returns>
            public static int CountFiles(this DirectoryInfo info)
            {
                int count = 0, Bluff = 0;
                rec_count_file(info, ref count, ref Bluff);
                return count;
            }
        }
    }
}
