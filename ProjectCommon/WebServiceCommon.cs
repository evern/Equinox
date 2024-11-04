using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;

namespace ProjectCommon
{
    public static class WebServiceCommon
    {

        /// <summary>
        /// Generate list of all ITRs to be checked for syncing
        /// </summary>
        public static List<WebServer_ITRSummary> GenerateSyncITR(Guid projectGuid, string discipline)
        {
            AdapterITR_MAIN daITR = new AdapterITR_MAIN();

            try
            {
                List<WebServer_ITRSummary> SyncITRSummary = daITR.GenerateTagStatusSummary(projectGuid, discipline);
                
                return SyncITRSummary;
            }
            finally
            {
                daITR.Dispose();
            }
        }
    }
}
