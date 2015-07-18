using System.IO;
using System.Diagnostics;
using System;

namespace MyTvShowsOrganizer
{
    public static class CloseIE
    {
        
        public static void CloseIee()
        {
            
            string savedFile = Path.Combine(Parameters.myprogpath, "IeId.inf");
            if (File.Exists(savedFile))
            {
                 string IeId = File.ReadAllText(savedFile);

                try
                {
                    foreach (string id in IeId.Split(','))
                    {
                        int IeIdint = Convert.ToInt32(id);
                        Process.GetProcessById(IeIdint).Kill();
                    }

                }
                catch
                {
                }
            }
        }
    }
}
