using CheckmateDX;
using Classes;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.ViewInfo;
using ProjectCommon;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoSync
{
    public partial class AutoSync : Form
    {
        public ObservableCollection<SyncStatus> SyncStatuses = new ObservableCollection<SyncStatus>();
        public ObservableCollection<SyncProject> SyncProjects = new ObservableCollection<SyncProject>();
        public List<SyncStatus> SyncQueue = new List<SyncStatus>();
        public AutoSync()
        {
            InitializeComponent();
            dateEditTime.DateTime = DateTime.Now;
            syncStatusBindingSource.DataSource = SyncStatuses;
            syncProjectBindingSource.DataSource = SyncProjects;

            if (!Common.SetConnStrFromXML())
                MessageBox.Show("Connection string not set, please start checkmate and set connection string");
            else
            {
                // To cater for instances where database is fresh
                using (AdapterPROJECT daPROJECT = new AdapterPROJECT())
                {
                    dsPROJECT.PROJECTDataTable dtPROJECT = daPROJECT.Get(); //Try to get projects from local datatable
                    if (dtPROJECT != null)
                    {
                        foreach (dsPROJECT.PROJECTRow drPROJECT in dtPROJECT.Rows)
                        {
                            SyncProject newProject = new SyncProject();
                            newProject.Guid = drPROJECT.GUID;
                            newProject.Number = drPROJECT.NUMBER;
                            newProject.IsSync = false;
                            SyncProjects.Add(newProject);
                        }
                    }
                }

                gridControl2.RefreshDataSource();
                timer1.Start();
            }
        }

        bool isBusy;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((bool)toggleSwitch1.EditValue == false || isBusy)
                return;

            IEnumerable<SyncProject> syncProjects = SyncProjects.Where(x => x.IsSync);
            foreach (SyncProject project in syncProjects)
            {
                //already completed, skip
                if (SyncStatuses.Any(x => x.StartTime != null && ((DateTime)x.StartTime).Date == DateTime.Now.Date && x.ProjectGuid == project.Guid))
                    continue;

                //already added to queue, skip
                if (SyncQueue.Any(x => x.StartTime != null && ((DateTime)x.StartTime).Date == DateTime.Now.Date && x.ProjectGuid == project.Guid))
                    continue;

                //run twice in a day with 1 hour delay
                if (DateTime.Now.TimeOfDay.Hours == dateEditTime.DateTime.TimeOfDay.Hours && DateTime.Now.TimeOfDay.Minutes == dateEditTime.DateTime.TimeOfDay.Minutes)
                {
                    SyncStatus currentSyncStatus = new SyncStatus();
                    currentSyncStatus.ProjectGuid = project.Guid;
                    currentSyncStatus.Project = project.Number;

                    SyncQueue.Add(currentSyncStatus);
                }
            }

            foreach(SyncStatus sync in SyncQueue)
            {
                if (isBusy)
                    break;

                //already processed for the day, skip
                if (SyncStatuses.Any(x => x.DailyRunCount == 2 && x.StartTime != null && ((DateTime)x.StartTime).Date == DateTime.Now.Date && x.ProjectGuid == sync.ProjectGuid))
                    continue;

                int dailyRunCount = 0;
                SyncStatus firstRunStatus = SyncStatuses.FirstOrDefault(x => x.DailyRunCount == 1 && x.StartTime != null && ((DateTime)x.StartTime).Date == DateTime.Now.Date && x.ProjectGuid == sync.ProjectGuid);
                if (firstRunStatus != null)
                {
                    if (firstRunStatus.EndTime == null)
                        continue;
                    else if (DateTime.Now.TimeOfDay.Hours < ((DateTime)firstRunStatus.EndTime).TimeOfDay.Hours + 1)
                        continue;

                    dailyRunCount = 2;
                }
                else
                    dailyRunCount = 1;

                isBusy = true;
                frmSync_Status syncStatus = new frmSync_Status(true);
                sync.frmSyncStatus = syncStatus;
                sync.StartTime = DateTime.Now;

                //need to create a copy because queue will be run twice
                SyncStatus status = new SyncStatus();
                status.Project = sync.Project;
                status.ProjectGuid = sync.ProjectGuid;
                status.StartTime = DateTime.Now;
                status.frmSyncStatus = sync.frmSyncStatus;
                status.DailyRunCount = dailyRunCount;

                SyncStatuses.Add(status);
                gridControl1.RefreshDataSource();
                syncStatus.SyncCompleted += syncCompleted;
                syncStatus.Show();
                syncStatus.StartSyncing(sync.ProjectGuid);
            }
        }

        //populate end time and set isBusy false so that next project can start
        void syncCompleted(object sender, SyncProcessedEventArg e)
        {
            int dailyRunCount = 0;
            SyncStatus findSyncQueue = SyncQueue.FirstOrDefault(x => x.ProjectGuid == e.SyncProjectGuid && x.StartTime != null && ((DateTime)x.StartTime).Date == DateTime.Now.Date);
            if (findSyncQueue != null)
            {
                findSyncQueue.DailyRunCount += 1;
                dailyRunCount = findSyncQueue.DailyRunCount;
                if (findSyncQueue.DailyRunCount == 2)
                    SyncQueue.Remove(findSyncQueue);
            }

            SyncStatus findSyncStatus = SyncStatuses.FirstOrDefault(x => x.ProjectGuid == e.SyncProjectGuid && x.StartTime != null && ((DateTime)x.StartTime).Date == DateTime.Now.Date && x.DailyRunCount == dailyRunCount);
            if(findSyncStatus != null)
            {
                isBusy = false;
                findSyncStatus.frmSyncStatus.Close();
                findSyncStatus.EndTime = DateTime.Now;
                gridControl1.RefreshDataSource();
            }
        }

        private void toggleSwitch1_Toggled(object sender, EventArgs e)
        {
            //clear statuses so sync can be restarted
            if((bool)toggleSwitch1.EditValue == true)
            {
                SyncQueue.Clear();
                SyncStatuses.Clear();
                gridControl1.RefreshDataSource();
            }
        }
    }
}
