using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ProjectCommon;
using ProjectDatabase;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using ProjectLibrary;
using DevExpress.XtraEditors.Controls;
using System.Xml.Linq;
using System.IO;
using System.Threading;
using Amib.Threading;
using DevExpress.XtraRichEdit;

namespace CheckmateDX
{
    public partial class frmSync_Status : frmParent
    {
        List<Sync_Status> _Sync_Status = new List<Sync_Status>();

        string _HWID = Common.GetHWID();
        string _remoteConnStr = string.Empty;
        Guid _RemoteSYNCGUID = Guid.Empty;
        Guid _ProjectGUID = Guid.Empty;
        bool _ForceSyncAll = false;

        ////Local Adapters
        SQLBase daLOCAL = new SQLBase(Variables.ConnStr);

        ////Remote Adapters
        SQLBase daREMOTE;

        dsSYNC_TABLE.SYNC_TABLEDataTable _dtREMOTE_SYNC_TABLE;
        dsSYNC_CONFLICT.SYNC_CONFLICTDataTable _dtREMOTE_CONFLICT_TABLE;
        dsTOMBSTONE.TOMBSTONEDataTable _dtREMOTE_TOMBSTONE;
        private BackgroundWorker processWorker = new BackgroundWorker();
        private ProcessThreadManager processManager = new ProcessThreadManager();
        public EventHandler<SyncProcessedEventArg> SyncCompleted;

        public frmSync_Status(bool manualStart = false, bool forceSyncAll = false)
        {
            InitializeComponent();
            gridControlStatus.DataSource = _Sync_Status;
            Populate_Repository();
            Initialize_Sync_Status();

            //Gets the Remote Connection String and Sets the Form Name
            string s;
            _remoteConnStr = Get_Remote_ConnStr(out s);
            daREMOTE = new SQLBase(_remoteConnStr);
            this.Text = s;

            this.processWorker.WorkerSupportsCancellation = true;
            this.processWorker.WorkerReportsProgress = true;
            this.processWorker.DoWork += new DoWorkEventHandler(processWorker_DoWork);
            processManager.ProcessStatus += new ProcessThreadManager.ProcessStatusHandler(Update_Overall_Progress);

            if(!manualStart)
                tSync_Start.Enabled = true;

            colSyncStatus_CreatedBy.Visible = false;
            colSyncStatus_CreatedStr.Visible = false;
            _ForceSyncAll = forceSyncAll;
        }

        public void StartSyncing(Guid projectGuid)
        {
            System_Environment.SetUser(new User(Guid.Empty)
            {
                userFirstName = "SUPER",
                userLastName = "ADMIN",
                userDiscipline = Common.ConvertDBDisciplineForDisplay(string.Empty, Guid.Empty),
                userProject = new ValuePair(Common.ConvertProjectGuidToName(Guid.Empty, Guid.Empty), Guid.Empty),
                userRole = new ValuePair(Common.ConvertRoleGuidToName(Guid.Empty), Guid.Empty),
                userQANumber = "SUPERADMINUSER"
            });

            _ProjectGUID = projectGuid;
            tSync_Start.Enabled = true;
        }

        public frmSync_Status(string HWID, string MachineName)
        {
            InitializeComponent();
            Populate_Repository();
            this.Text = MachineName;
            using(AdapterSYNC_RECORD daLOCAL_SYNC_RECORD = new AdapterSYNC_RECORD())
            {
                dsSYNC_RECORD.SYNC_RECORDDataTable dtLOCAL_SYNC_RECORD = daLOCAL_SYNC_RECORD.Get_By(HWID);
                if(dtLOCAL_SYNC_RECORD != null)
                {
                    foreach (dsSYNC_RECORD.SYNC_RECORDRow drLOCAL_SYNC_RECORD in dtLOCAL_SYNC_RECORD.Rows)
                    {
                        _Sync_Status.Add(new Sync_Status()
                        {
                            SyncStatus_Type = Common.Replace_WithSpaces(drLOCAL_SYNC_RECORD.TABLE_NAME),
                            SyncStatus_Status = drLOCAL_SYNC_RECORD.STATUS,
                            SyncStatus_Download = drLOCAL_SYNC_RECORD.DOWNLOAD_COUNT,
                            SyncStatus_Delete = drLOCAL_SYNC_RECORD.DELETE_COUNT,
                            SyncStatus_Upload = drLOCAL_SYNC_RECORD.UPLOAD_COUNT,
                            SyncStatus_Same = drLOCAL_SYNC_RECORD.SAME_COUNT,
                            SyncStatus_Conflict = drLOCAL_SYNC_RECORD.CONFLICT_COUNT,
                            SyncStatus_Created = drLOCAL_SYNC_RECORD.SYNCED,
                            SyncStatus_CreatedBy = new ValuePair(Common.ConvertUserGuidToName(drLOCAL_SYNC_RECORD.SYNCED_BY), drLOCAL_SYNC_RECORD.GUID),
                            SyncStatus_OverallPercentage = 100
                        });

                    }
                }
            }

            colSyncStatus_CreatedBy.Visible = true;
            colSyncStatus_CreatedStr.Visible = true;
            gridControlStatus.DataSource = _Sync_Status;
            gridViewStatus.BestFitColumns();
        }

        #region Initialization
        /// <summary>
        /// Populate repository used within gridview
        /// </summary>
        private void Populate_Repository()
        {
            repositoryItemImageComboBox1.Items.Add(new ImageComboBoxItem(SyncStatus_Display.Pending.ToString(), 0));
            repositoryItemImageComboBox1.Items.Add(new ImageComboBoxItem(SyncStatus_Display.Comparing.ToString(), 1));
            repositoryItemImageComboBox1.Items.Add(new ImageComboBoxItem(SyncStatus_Display.Skipped.ToString(), 2));
            repositoryItemImageComboBox1.Items.Add(new ImageComboBoxItem(SyncStatus_Display.Ok.ToString(), 3));
            repositoryItemImageComboBox1.Items.Add(new ImageComboBoxItem(SyncStatus_Display.Downloading.ToString(), 4));
            repositoryItemImageComboBox1.Items.Add(new ImageComboBoxItem(Common.Replace_WithSpaces(SyncStatus_Display.Downloading_Template.ToString()), 4));
            repositoryItemImageComboBox1.Items.Add(new ImageComboBoxItem(SyncStatus_Display.Uploading.ToString(), 5));
            repositoryItemImageComboBox1.Items.Add(new ImageComboBoxItem(Common.Replace_WithSpaces(SyncStatus_Display.Uploading_Template.ToString()), 5));
            repositoryItemImageComboBox1.Items.Add(new ImageComboBoxItem(SyncStatus_Display.Downloading.ToString(), 4));
            repositoryItemImageComboBox1.Items.Add(new ImageComboBoxItem(Common.Replace_WithSpaces(SyncStatus_Display.Saving_Locally.ToString()), 6));
            repositoryItemImageComboBox1.Items.Add(new ImageComboBoxItem(Common.Replace_WithSpaces(SyncStatus_Display.Saving_Remotely.ToString()), 6));
        }

        /// <summary>
        /// Initializes the Sync Status
        /// </summary>
        private void Initialize_Sync_Status()
        {
            _Sync_Status.Clear();
            foreach(Sync_Type SyncType in Enum.GetValues(typeof(Sync_Type)))
            {
                _Sync_Status.Add(new Sync_Status() { SyncStatus_Type = Common.Replace_WithSpaces(SyncType.ToString()), SyncStatus_Status = SyncStatus_Display.Pending.ToString() });
            }

            gridViewStatus.RefreshData();
            gridViewStatus.BestFitColumns();
        }
        #endregion

        #region Views
        private void SplashScreen_Progress(object sender, SqlRowUpdatedEventArgs e)
        {
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
        }

        private void Update_Last_Sync(object sender, ProcessLastSyncDateProcessThreadEventArgs e)
        {
            Sync_Status Sync_Item_Status = _Sync_Status.FirstOrDefault(obj => obj.SyncStatus_Type == Common.Replace_WithSpaces(e.progress_type.ToString()));
            if (Sync_Item_Status != null)
            {
                Sync_Item_Status.SyncStatus_LastSynced = e.LastSyncDateTime;
            }
        }

        /// <summary>
        /// Update the Sync Item Progress
        /// </summary>
        private void Update_Overall_Progress(object sender, ProcessThreadEventArgs e)
        {
            Sync_Status Sync_Item_Status = _Sync_Status.FirstOrDefault(obj => obj.SyncStatus_Type == Common.Replace_WithSpaces(e.progress_type.ToString()));
            if (Sync_Item_Status != null)
            {
                //Status Update
                Sync_Item_Status.SyncStatus_Status = e.progress_status;

                //Count Update
                if (e.progress_countType == SyncStatus_Count.Upload)
                {
                    if (e.count_reset)
                    {
                        Sync_Item_Status.SyncStatus_Upload = 0;
                        e.count_reset = false;
                    }
                    else
                        Sync_Item_Status.SyncStatus_Upload += 1;
                }
                else if (e.progress_countType == SyncStatus_Count.Download)
                {
                    if (e.count_reset)
                    {
                        Sync_Item_Status.SyncStatus_Download = 0;
                        e.count_reset = false;
                    }
                    else
                        Sync_Item_Status.SyncStatus_Download += 1;
                }
                else if (e.progress_countType == SyncStatus_Count.Same)
                    Sync_Item_Status.SyncStatus_Same += 1;
                else if (e.progress_countType == SyncStatus_Count.Conflict)
                    Sync_Item_Status.SyncStatus_Conflict += 1;
                else if (e.progress_countType == SyncStatus_Count.Resolve)
                    Sync_Item_Status.SyncStatus_Resolve += 1;
                else if (e.progress_countType == SyncStatus_Count.Delete)
                    Sync_Item_Status.SyncStatus_Delete += 1;

                //Percentage Update
                if (e.progress_doublestep)
                    Sync_Item_Status.SyncStatus_OverallPercentage += (e.progress_step * 2);
                else
                    Sync_Item_Status.SyncStatus_OverallPercentage += e.progress_step;

                if (Sync_Item_Status.SyncStatus_OverallPercentage > 100)
                    Sync_Item_Status.SyncStatus_OverallPercentage = 100;

                //Refresh View
                int FindIndex = _Sync_Status.IndexOf(Sync_Item_Status);
                gridViewStatus.RefreshRow(FindIndex);

                if (Sync_Item_Status.SyncStatus_OverallPercentage >= 95)
                {
                    Sync_Item_Status.SyncStatus_Created = DateTime.Now;
                    Sync_Item_Status.SyncStatus_CreatedBy = new ValuePair(System_Environment.GetUser().userFirstName + " " + System_Environment.GetUser().userLastName, System_Environment.GetUser().GUID);
                }
            }
        }

        /// <summary>
        /// Update the Sync Item Progress
        /// </summary>
        private void Update_Current_Progress(object sender, ProcessThreadEventArgs e)
        {
            Sync_Status Sync_Item_Status = _Sync_Status.FirstOrDefault(obj => obj.SyncStatus_Type == Common.Replace_WithSpaces(e.progress_type.ToString()));
            if (Sync_Item_Status != null)
            {
                if(e.progress_status != string.Empty)
                    Sync_Item_Status.SyncStatus_Status = e.progress_status;

                if(e.progress_reset)
                {
                    Sync_Item_Status.SyncStatus_CurrentPercentage = 0.0;
                    int FindIndex1 = _Sync_Status.IndexOf(Sync_Item_Status);
                    gridViewStatus.RefreshRow(FindIndex1);
                    e.progress_reset = false;
                    return;
                }

                if (e.progress_countType == SyncStatus_Count.Upload)
                {
                    Sync_Item_Status.SyncStatus_Upload += 1;
                }
                else if (e.progress_countType == SyncStatus_Count.Download)
                {
                    Sync_Item_Status.SyncStatus_Download += 1;
                }

                //Percentage Update
                if (e.progress_doublestep)
                    Sync_Item_Status.SyncStatus_CurrentPercentage += (e.progress_step * 2);
                else
                    Sync_Item_Status.SyncStatus_CurrentPercentage += e.progress_step;

                if (Sync_Item_Status.SyncStatus_CurrentPercentage > 100)
                    Sync_Item_Status.SyncStatus_CurrentPercentage = 100;

                //Refresh View
                if (Sync_Item_Status.SyncStatus_CurrentPercentage >= 95)
                {
                    Sync_Item_Status.SyncStatus_Created = DateTime.Now;
                    Sync_Item_Status.SyncStatus_CreatedBy = new ValuePair(System_Environment.GetUser().userFirstName + " " + System_Environment.GetUser().userLastName, System_Environment.GetUser().GUID);
                }

                int FindIndex = _Sync_Status.IndexOf(Sync_Item_Status);
                gridViewStatus.RefreshRow(FindIndex);
            }
        }

        private void Update_Same(object sender, Sync_Type SyncType)
        {
            Sync_Status Sync_Item_Status = _Sync_Status.FirstOrDefault(obj => obj.SyncStatus_Type == Common.Replace_WithSpaces(SyncType.ToString()));
            if (Sync_Item_Status != null)
            {
                Sync_Item_Status.SyncStatus_Same += 1;
                int FindIndex = _Sync_Status.IndexOf(Sync_Item_Status);
                gridViewStatus.RefreshRow(FindIndex);
            }
        }
        #endregion

        #region Sync
        /// <summary>
        /// Fired when the processManager background worker monitor object is started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void processWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (processManager.Count > 0)
            {
                // start the thread pool
                this.processManager.Start();
                //process as long as necessary
                while (!worker.CancellationPending && this.processManager.Pool.ActiveThreads > 0)
                {
                    Thread.Sleep(100);
                }
            }
        }
        #endregion

        #region Sync Initialization
        int _count = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(_count == 7)
            {
                timer1.Enabled = false;
                return;
            }

            if(_count == 0)
            {
                //Refresh the remote tombstone for any slave device syncing to it
                using (AdapterTOMBSTONE daLOCAL_TOMBSTONE = new AdapterTOMBSTONE())
                {
                    splashScreenManager1.ShowWaitForm();
                    splashScreenManager1.SetWaitFormCaption("Downloading Tombstone ... ");
                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, _dtREMOTE_TOMBSTONE.Rows.Count);
                    dsTOMBSTONE.TOMBSTONEDataTable dtLOCAL_TOMBSTONE = daLOCAL_TOMBSTONE.Get();

                    List<Guid> GuidList = dtLOCAL_TOMBSTONE.AsEnumerable().Select(obj => obj.Field<Guid>("GUID")).ToList();

                    foreach (dsTOMBSTONE.TOMBSTONERow drREMOTE_TOMBSTONE in _dtREMOTE_TOMBSTONE.Rows)
                    {
                        if (!GuidList.Any(x => x == drREMOTE_TOMBSTONE.Field<Guid>("GUID")))
                        {
                            dsTOMBSTONE.TOMBSTONERow drNEWLOCAL = dtLOCAL_TOMBSTONE.NewTOMBSTONERow();
                            drNEWLOCAL.ItemArray = drREMOTE_TOMBSTONE.ItemArray;
                            dtLOCAL_TOMBSTONE.Rows.Add(drNEWLOCAL);
                            daLOCAL_TOMBSTONE.Save(drNEWLOCAL);
                        }

                        splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                    }
                    //if (dtLOCAL_TOMBSTONE.Rows.Count > 0)
                    //    daLOCAL_TOMBSTONE.Save(dtLOCAL_TOMBSTONE);
                    splashScreenManager1.CloseWaitForm();
                }
            }

            if(!processWorker.IsBusy)
            {
                if (_count == 0)
                {
                    processManager.Reset_Step1(_ProjectGUID, daLOCAL, daREMOTE, _dtREMOTE_SYNC_TABLE, _dtREMOTE_CONFLICT_TABLE, _dtREMOTE_TOMBSTONE, _remoteConnStr, _ForceSyncAll);
                    processManager[0].ProcessProgress_Overall += new ProcessThread.ProcessProgressHandler(Update_Overall_Progress);
                    processManager[0].ProcessProgress_Current += new ProcessThread.ProcessProgressHandler(Update_Current_Progress);
                    processManager[0].ProcessLastSyncDateTime += new ProcessThread.ProcessLastSyncHandler(Update_Last_Sync);
                    this.processWorker.RunWorkerAsync();
                }
                else if (_count == 1)
                {
                    processManager.Reset_Step2(_ProjectGUID, daLOCAL, daREMOTE, _dtREMOTE_SYNC_TABLE, _dtREMOTE_CONFLICT_TABLE, _dtREMOTE_TOMBSTONE, _remoteConnStr, _ForceSyncAll);
                    processManager[0].ProcessProgress_Overall += new ProcessThread.ProcessProgressHandler(Update_Overall_Progress);
                    processManager[1].ProcessProgress_Overall += new ProcessThread.ProcessProgressHandler(Update_Overall_Progress);
                    processManager[2].ProcessProgress_Overall += new ProcessThread.ProcessProgressHandler(Update_Overall_Progress);
                    processManager[3].ProcessProgress_Overall += new ProcessThread.ProcessProgressHandler(Update_Overall_Progress);
                    processManager[0].ProcessProgress_Current += new ProcessThread.ProcessProgressHandler(Update_Current_Progress);
                    processManager[1].ProcessProgress_Current += new ProcessThread.ProcessProgressHandler(Update_Current_Progress);
                    processManager[2].ProcessProgress_Current += new ProcessThread.ProcessProgressHandler(Update_Current_Progress);
                    processManager[3].ProcessProgress_Current += new ProcessThread.ProcessProgressHandler(Update_Current_Progress);
                    processManager[0].ProcessLastSyncDateTime += new ProcessThread.ProcessLastSyncHandler(Update_Last_Sync);
                    processManager[1].ProcessLastSyncDateTime += new ProcessThread.ProcessLastSyncHandler(Update_Last_Sync);
                    processManager[2].ProcessLastSyncDateTime += new ProcessThread.ProcessLastSyncHandler(Update_Last_Sync);
                    processManager[3].ProcessLastSyncDateTime += new ProcessThread.ProcessLastSyncHandler(Update_Last_Sync);
                    this.processWorker.RunWorkerAsync();
                }
                else if (_count == 2)
                {
                    processManager.Reset_Step3(_ProjectGUID, daLOCAL, daREMOTE, _dtREMOTE_SYNC_TABLE, _dtREMOTE_CONFLICT_TABLE, _dtREMOTE_TOMBSTONE, _remoteConnStr, _ForceSyncAll);

                    processManager[0].ProcessProgress_Overall += new ProcessThread.ProcessProgressHandler(Update_Overall_Progress);
                    processManager[1].ProcessProgress_Overall += new ProcessThread.ProcessProgressHandler(Update_Overall_Progress);
                    processManager[0].ProcessProgress_Current += new ProcessThread.ProcessProgressHandler(Update_Current_Progress);
                    processManager[1].ProcessProgress_Current += new ProcessThread.ProcessProgressHandler(Update_Current_Progress);
                    processManager[0].ProcessLastSyncDateTime += new ProcessThread.ProcessLastSyncHandler(Update_Last_Sync);
                    processManager[1].ProcessLastSyncDateTime += new ProcessThread.ProcessLastSyncHandler(Update_Last_Sync);
                    this.processWorker.RunWorkerAsync();
                }
                else if (_count == 3)
                {
                    processManager.Reset_Step4(_ProjectGUID, daLOCAL, daREMOTE, _dtREMOTE_SYNC_TABLE, _dtREMOTE_CONFLICT_TABLE, _dtREMOTE_TOMBSTONE, _remoteConnStr, _ForceSyncAll);
                    processManager[0].ProcessProgress_Overall += new ProcessThread.ProcessProgressHandler(Update_Overall_Progress);
                    processManager[1].ProcessProgress_Overall += new ProcessThread.ProcessProgressHandler(Update_Overall_Progress);
                    processManager[0].ProcessProgress_Current += new ProcessThread.ProcessProgressHandler(Update_Current_Progress);
                    processManager[1].ProcessProgress_Current += new ProcessThread.ProcessProgressHandler(Update_Current_Progress);
                    processManager[0].ProcessLastSyncDateTime += new ProcessThread.ProcessLastSyncHandler(Update_Last_Sync);
                    processManager[1].ProcessLastSyncDateTime += new ProcessThread.ProcessLastSyncHandler(Update_Last_Sync);
                    this.processWorker.RunWorkerAsync();
                }
                else if (_count == 4)
                {
                    processManager.Reset_Step5(_ProjectGUID, daLOCAL, daREMOTE, _dtREMOTE_SYNC_TABLE, _dtREMOTE_CONFLICT_TABLE, _dtREMOTE_TOMBSTONE, _remoteConnStr, _ForceSyncAll);
                    processManager[0].ProcessProgress_Overall += new ProcessThread.ProcessProgressHandler(Update_Overall_Progress);
                    processManager[1].ProcessProgress_Overall += new ProcessThread.ProcessProgressHandler(Update_Overall_Progress);
                    processManager[0].ProcessProgress_Current += new ProcessThread.ProcessProgressHandler(Update_Current_Progress);
                    processManager[1].ProcessProgress_Current += new ProcessThread.ProcessProgressHandler(Update_Current_Progress);
                    processManager[0].ProcessLastSyncDateTime += new ProcessThread.ProcessLastSyncHandler(Update_Last_Sync);
                    processManager[1].ProcessLastSyncDateTime += new ProcessThread.ProcessLastSyncHandler(Update_Last_Sync);
                    this.processWorker.RunWorkerAsync();
                }
                else if (_count == 5)
                {
                    processManager.Reset_Step6(_ProjectGUID, daLOCAL, daREMOTE, _dtREMOTE_SYNC_TABLE, _dtREMOTE_CONFLICT_TABLE, _dtREMOTE_TOMBSTONE, _remoteConnStr, _ForceSyncAll);
                    processManager[0].ProcessProgress_Overall += new ProcessThread.ProcessProgressHandler(Update_Overall_Progress);
                    processManager[0].ProcessProgress_Current += new ProcessThread.ProcessProgressHandler(Update_Current_Progress);
                    processManager[0].ProcessLastSyncDateTime += new ProcessThread.ProcessLastSyncHandler(Update_Last_Sync);
                    this.processWorker.RunWorkerAsync();
                }
                else if (_count == 6)
                {
                    Save_Sync_Status();
                    SyncCompleted?.Invoke(this, new SyncProcessedEventArg() { SyncProjectGuid = _ProjectGUID });
                }

                _count += 1;
            }
        }

        /// <summary>
        /// Save the sync status locally and remotely
        /// </summary>
        private void Save_Sync_Status()
        {
            AdapterSYNC_RECORD daLOCAL_SYNC_RECORD = new AdapterSYNC_RECORD();
            AdapterSYNC_RECORD daREMOTE_SYNC_RECORD = new AdapterSYNC_RECORD(_remoteConnStr);
            daLOCAL_SYNC_RECORD.Set_Update_Event(SplashScreen_Progress);
            daREMOTE_SYNC_RECORD.Set_Update_Event(SplashScreen_Progress);

            dsSYNC_RECORD dsLOCAL_SYNC_RECORD = new dsSYNC_RECORD();
            dsSYNC_RECORD dsREMOTE_SYNC_RECORD = new dsSYNC_RECORD();
            dsSYNC_RECORD.SYNC_RECORDDataTable dtLOCAL_SYNC_RECORD = new dsSYNC_RECORD.SYNC_RECORDDataTable();
            dsSYNC_RECORD.SYNC_RECORDDataTable dtREMOTE_SYNC_RECORD = new dsSYNC_RECORD.SYNC_RECORDDataTable();

            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormCaption("Logging Record ... ");
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, _Sync_Status.Count * 2);

            dtLOCAL_SYNC_RECORD = daLOCAL_SYNC_RECORD.Get_By(_HWID);
            dtREMOTE_SYNC_RECORD = daREMOTE_SYNC_RECORD.Get_By(_HWID);
            if (dtLOCAL_SYNC_RECORD == null)
                dtLOCAL_SYNC_RECORD = new dsSYNC_RECORD.SYNC_RECORDDataTable();

            if (dtREMOTE_SYNC_RECORD == null)
                dtREMOTE_SYNC_RECORD = new dsSYNC_RECORD.SYNC_RECORDDataTable();

            foreach(Sync_Status SyncStatus in _Sync_Status)
            {
                dsSYNC_RECORD.SYNC_RECORDRow drLOCAL_SYNC_RECORD = dtLOCAL_SYNC_RECORD.FirstOrDefault(obj => obj.TABLE_NAME == SyncStatus.SyncStatus_Type);
                dsSYNC_RECORD.SYNC_RECORDRow drREMOTE_SYNC_RECORD = dtREMOTE_SYNC_RECORD.FirstOrDefault(obj => obj.TABLE_NAME == SyncStatus.SyncStatus_Type);

                dsSYNC_RECORD.SYNC_RECORDRow drLOCAL_NEW_SYNC_RECORD = dtLOCAL_SYNC_RECORD.NewSYNC_RECORDRow();

                if(drLOCAL_SYNC_RECORD != null)
                {
                    drLOCAL_SYNC_RECORD.STATUS = SyncStatus.SyncStatus_Status;
                    drLOCAL_SYNC_RECORD.DELETE_COUNT = SyncStatus.SyncStatus_Delete;
                    drLOCAL_SYNC_RECORD.UPLOAD_COUNT = SyncStatus.SyncStatus_Upload;
                    drLOCAL_SYNC_RECORD.DOWNLOAD_COUNT = SyncStatus.SyncStatus_Download;
                    drLOCAL_SYNC_RECORD.SAME_COUNT = SyncStatus.SyncStatus_Same;
                    drLOCAL_SYNC_RECORD.CONFLICT_COUNT = SyncStatus.SyncStatus_Conflict;
                    drLOCAL_SYNC_RECORD.SYNCED = SyncStatus.SyncStatus_Created;
                    drLOCAL_SYNC_RECORD.SYNCED_BY = (Guid)SyncStatus.SyncStatus_CreatedBy.Value;
                }
                else
                {
                    drLOCAL_NEW_SYNC_RECORD.GUID = Guid.NewGuid();
                    drLOCAL_NEW_SYNC_RECORD.HWID = _HWID;
                    drLOCAL_NEW_SYNC_RECORD.TABLE_NAME = SyncStatus.SyncStatus_Type;
                    drLOCAL_NEW_SYNC_RECORD.STATUS = SyncStatus.SyncStatus_Status;
                    drLOCAL_NEW_SYNC_RECORD.DELETE_COUNT = SyncStatus.SyncStatus_Delete;
                    drLOCAL_NEW_SYNC_RECORD.UPLOAD_COUNT = SyncStatus.SyncStatus_Upload;
                    drLOCAL_NEW_SYNC_RECORD.DOWNLOAD_COUNT = SyncStatus.SyncStatus_Download;
                    drLOCAL_NEW_SYNC_RECORD.SAME_COUNT = SyncStatus.SyncStatus_Same;
                    drLOCAL_NEW_SYNC_RECORD.CONFLICT_COUNT = SyncStatus.SyncStatus_Conflict;
                    drLOCAL_NEW_SYNC_RECORD.SYNCED = SyncStatus.SyncStatus_Created;
                    drLOCAL_NEW_SYNC_RECORD.SYNCED_BY = (Guid)SyncStatus.SyncStatus_CreatedBy.Value;
                    dtLOCAL_SYNC_RECORD.AddSYNC_RECORDRow(drLOCAL_NEW_SYNC_RECORD);
                }

                if(drREMOTE_SYNC_RECORD != null)
                {
                    if (drLOCAL_SYNC_RECORD != null)
                        drREMOTE_SYNC_RECORD.ItemArray = drLOCAL_SYNC_RECORD.ItemArray;
                    else
                        drREMOTE_SYNC_RECORD.ItemArray = drLOCAL_NEW_SYNC_RECORD.ItemArray;
                }
                else
                {
                    dsSYNC_RECORD.SYNC_RECORDRow drREMOTE_NEW_SYNC_RECORD = dtREMOTE_SYNC_RECORD.NewSYNC_RECORDRow();
                    if (drLOCAL_SYNC_RECORD != null)
                        drREMOTE_NEW_SYNC_RECORD.ItemArray = drLOCAL_SYNC_RECORD.ItemArray;
                    else
                        drREMOTE_NEW_SYNC_RECORD.ItemArray = drLOCAL_NEW_SYNC_RECORD.ItemArray;

                    dtREMOTE_SYNC_RECORD.AddSYNC_RECORDRow(drREMOTE_NEW_SYNC_RECORD);
                }
            }

            daLOCAL_SYNC_RECORD.Save(dtLOCAL_SYNC_RECORD);
            daREMOTE_SYNC_RECORD.Save(dtREMOTE_SYNC_RECORD);

            daLOCAL_SYNC_RECORD.Dispose();
            daREMOTE_SYNC_RECORD.Dispose();

            splashScreenManager1.CloseWaitForm();
        }

        private void tSync_Start_Tick(object sender, EventArgs e)
        {
            tSync_Start.Enabled = false;
            if(!isMachine_Paired())
            {
                Common.Warn("Sync Request isn't Approved Yet");
                return;
            }

            if(isEnvironment_Established())
            {
                using(AdapterSYNC_TABLE daREMOTE_SYNC_TABLE = new AdapterSYNC_TABLE(_remoteConnStr))
                {
                    _dtREMOTE_SYNC_TABLE = daREMOTE_SYNC_TABLE.GetBy(_RemoteSYNCGUID);
                    if(_dtREMOTE_SYNC_TABLE == null)
                    {
                        _dtREMOTE_SYNC_TABLE = new dsSYNC_TABLE.SYNC_TABLEDataTable();
                    }
                }

                //Cache the remote sync conflict table
                using(AdapterSYNC_CONFLICT daREMOTE_SYNC_CONFLICT = new AdapterSYNC_CONFLICT(_remoteConnStr))
                {
                    _dtREMOTE_CONFLICT_TABLE = daREMOTE_SYNC_CONFLICT.GetAll();
                    if(_dtREMOTE_CONFLICT_TABLE == null)
                    {
                        _dtREMOTE_CONFLICT_TABLE = new dsSYNC_CONFLICT.SYNC_CONFLICTDataTable();
                    }
                }

                using(AdapterTOMBSTONE daREMOTE_TOMBSTONE = new AdapterTOMBSTONE(_remoteConnStr))
                {
                    DateTime tableLastSyncDate = getLastSyncDate();
                    string tableLastSyncDateStr = tableLastSyncDate.ToString(Common.SQLDateFormatString());
                    _dtREMOTE_TOMBSTONE = daREMOTE_TOMBSTONE.GetUpdatedRecords(tableLastSyncDateStr);
                }

                timer1.Interval = 100;
                timer1.Enabled = true;
            }
        }

        private DateTime getLastSyncDate()
        {
            using (AdapterSYNC_RECORD daREMOTE_SYNC_RECORD = new AdapterSYNC_RECORD(_remoteConnStr))
            {
                dsSYNC_RECORD.SYNC_RECORDRow drREMOTE_SYNC_RECORD = daREMOTE_SYNC_RECORD.Get_Any_By(_HWID);
                if (drREMOTE_SYNC_RECORD != null)
                {
                    return drREMOTE_SYNC_RECORD.SYNCED;
                }
                else
                    return new DateTime(1900, 1, 1);
            }
        }

        /// <summary>
        /// Checks whether sync was approved for this machine
        /// </summary>
        /// <returns></returns>
        private bool isMachine_Paired()
        {
            using(AdapterSYNC_PAIR daSYNCPAIR_Remote = new AdapterSYNC_PAIR(_remoteConnStr))
            {
                dsSYNC_PAIR.SYNC_PAIRRow drSYNCPAIR = daSYNCPAIR_Remote.GetBy(_HWID);
                if (drSYNCPAIR != null && drSYNCPAIR.APPROVED)
                {
                    _RemoteSYNCGUID = drSYNCPAIR.GUID;
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Establish the sync environment, also allows superuser to select environment parameters
        /// </summary>
        private bool isEnvironment_Established()
        {
            //when project id is pre-established from AutoSync
            if (_ProjectGUID != Guid.Empty)
                return true;

            // To cater for instances where database is fresh
            using (AdapterPROJECT daPROJECT = new AdapterPROJECT())
            {
                dsPROJECT.PROJECTDataTable dtPROJECT = daPROJECT.Get(); //Try to get project from local datatable
                if (dtPROJECT == null)
                {
                    _ProjectGUID = Guid.Empty;
                    return true;
                }
            }

            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
            {
                frmTool_Project frmSelectProject = new frmTool_Project();
                frmSelectProject.ShowAsDialog();
                if (frmSelectProject.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _ProjectGUID = frmSelectProject.GetSelectedProject().GUID;
                }
                else
                {
                    this.Close();
                    return false;
                }

                return true;
            }
            else
                _ProjectGUID = (Guid)System_Environment.GetUser().userProject.Value;

            return true;
        }
        #endregion

        #region Auxiliary
        /// <summary>
        /// Read the remote connection string from XML
        /// </summary>
        private string Get_Remote_ConnStr(out string dbName)
        {
            string xmlFilePath = Common.DatabaseXMLFilePath(false);
            dbName = "N/A";

            if (File.Exists(xmlFilePath))
            {
                try
                {
                    XDocument doc = XDocument.Load(xmlFilePath);
                    XElement findDatabase = doc.Root.Descendants().FirstOrDefault(obj => obj.Name == "Remote");
                    if (findDatabase != null)
                    {
                        string server = findDatabase.Attribute("Url").Value;
                        string database = Common.Decrypt(findDatabase.Attribute("Database").Value, true);
                        string username = Common.Decrypt(findDatabase.Attribute("Username").Value, true);
                        string password = Common.Decrypt(findDatabase.Attribute("Password").Value, true);
                        string connStr = Common.ConstructConnString(server, database, username, password);

                        dbName = server;
                        return connStr;
                    }
                }
                catch
                {

                }
            }

            return string.Empty;
        }

        #endregion
    }

    public class ProcessThread
    {
        public delegate void ProcessProgressHandler(object sender, ProcessThreadEventArgs e);
        public delegate void ProcessLastSyncHandler(object sender, ProcessLastSyncDateProcessThreadEventArgs e);
        public event ProcessProgressHandler ProcessProgress_Overall = delegate { };
        public event ProcessProgressHandler ProcessProgress_Current = delegate { };
        public event ProcessLastSyncHandler ProcessLastSyncDateTime = delegate { };
        private ProcessThreadEventArgs _CurrentProgressEventArg;

        public IWorkItemResult workItemResult = null;
        public object threadPoolState = null;

        SQLBase daLOCAL, daREMOTE;
        DataTable dtLOCAL, dtREMOTE;
        DataTable dtLOCAL_ALL, dtREMOTE_ALL;
        Guid ProjectGUID = Guid.Empty;
        string remoteConnStr = string.Empty;
        string sql = string.Empty;
        List<string> Sync_Database = new List<string>();
        string HWID = Common.GetHWID();
        bool _forceSyncAll = false;

        //Local Adapters
        AdapterGENERAL_EQUIPMENT daLOCAL_EQUIPMENT;
        AdapterITR_MAIN daLOCAL_ITR_MAIN;
        AdapterITR_STATUS daLOCAL_ITR_STATUS;
        AdapterITR_STATUS_ISSUE daLOCAL_ITR_STATUS_ISSUE;
        AdapterPREFILL_MAIN daLOCAL_PREFILL_MAIN;
        AdapterPREFILL_REGISTER daLOCAL_PREFILL_REGISTER;
        AdapterPROJECT daLOCAL_PROJECT;
        AdapterPUNCHLIST_MAIN daLOCAL_PUNCHLIST_MAIN;
        AdapterPUNCHLIST_STATUS daLOCAL_PUNCHLIST_STATUS;
        AdapterPUNCHLIST_STATUS_ISSUE daLOCAL_PUNCHLIST_STATUS_ISSUE;
        AdapterROLE_MAIN daLOCAL_ROLE_MAIN;
        AdapterSCHEDULE daLOCAL_SCHEDULE;
        AdapterTAG daLOCAL_TAG;
        AdapterTEMPLATE_MAIN daLOCAL_TEMPLATE_MAIN;
        AdapterTEMPLATE_REGISTER daLOCAL_TEMPLATE_REGISTER;
        AdapterTEMPLATE_TOGGLE daLOCAL_TEMPLATE_TOGGLE;
        AdapterMATRIX_ASSIGNMENT daLOCAL_MATRIX_ASSIGNMENT;
        AdapterMATRIX_TYPE daLOCAL_MATRIX_TYPE;
        AdapterUSER_MAIN daLOCAL_USER_MAIN;
        AdapterWBS daLOCAL_WBS;
        AdapterWORKFLOW_MAIN daLOCAL_WORKFLOW;

        AdapterSYNC_CONFLICT daREMOTE_SYNC_CONFLICT;
        dsTOMBSTONE.TOMBSTONEDataTable _dtREMOTE_TOMBSTONE;
        dsSYNC_CONFLICT.SYNC_CONFLICTDataTable _dtREMOTE_SYNC_CONFLICT; //Used to store server sync conflict for sync resolve verification
        dsSYNC_CONFLICT dsSYNCCONFLICT = new dsSYNC_CONFLICT();

        //Remote Adapters
        AdapterGENERAL_EQUIPMENT daREMOTE_EQUIPMENT;
        AdapterITR_MAIN daREMOTE_ITR_MAIN;
        AdapterITR_STATUS daREMOTE_ITR_STATUS;
        AdapterITR_STATUS_ISSUE daREMOTE_ITR_STATUS_ISSUE;
        AdapterPREFILL_MAIN daREMOTE_PREFILL_MAIN;
        AdapterPREFILL_REGISTER daREMOTE_PREFILL_REGISTER;
        AdapterPROJECT daREMOTE_PROJECT;
        AdapterPUNCHLIST_MAIN daREMOTE_PUNCHLIST_MAIN;
        AdapterPUNCHLIST_STATUS daREMOTE_PUNCHLIST_STATUS;
        AdapterPUNCHLIST_STATUS_ISSUE daREMOTE_PUNCHLIST_STATUS_ISSUE;
        AdapterROLE_MAIN daREMOTE_ROLE_MAIN;
        AdapterSCHEDULE daREMOTE_SCHEDULE;
        AdapterTAG daREMOTE_TAG;
        AdapterTEMPLATE_MAIN daREMOTE_TEMPLATE_MAIN;
        AdapterTEMPLATE_REGISTER daREMOTE_TEMPLATE_REGISTER;
        AdapterTEMPLATE_TOGGLE daREMOTE_TEMPLATE_TOGGLE;
        AdapterMATRIX_ASSIGNMENT daREMOTE_MATRIX_ASSIGNMENT;
        AdapterMATRIX_TYPE daREMOTE_MATRIX_TYPE;
        AdapterUSER_MAIN daREMOTE_USER_MAIN;
        AdapterWBS daREMOTE_WBS;
        AdapterWORKFLOW_MAIN daREMOTE_WORKFLOW;

        AdapterCERTIFICATE_DATA daLOCAL_CERTIFICATE_DATA;
        AdapterCERTIFICATE_DATA daREMOTE_CERTIFICATE_DATA;
        AdapterCERTIFICATE_MAIN daLOCAL_CERTIFICATE_MAIN;
        AdapterCERTIFICATE_MAIN daREMOTE_CERTIFICATE_MAIN;
        AdapterCERTIFICATE_STATUS daLOCAL_CERTIFICATE_STATUS;
        AdapterCERTIFICATE_STATUS daREMOTE_CERTIFICATE_STATUS;
        AdapterCERTIFICATE_STATUS_ISSUE daLOCAL_CERTIFICATE_STATUS_ISSUE;
        AdapterCERTIFICATE_STATUS_ISSUE daREMOTE_CERTIFICATE_STATUS_ISSUE;
        public ProcessThread(Guid ProjectGuid, SQLBase SQLBaseLocal, SQLBase SQLBaseRemote, List<string> SyncDatabase, dsSYNC_CONFLICT.SYNC_CONFLICTDataTable dtREMOTE_SYNC_CONFLICT, dsTOMBSTONE.TOMBSTONEDataTable dtTOMBSTONE, string RemoteConnStr, bool forceSyncAll)
        {
            ProjectGUID = ProjectGuid;
            daLOCAL = SQLBaseLocal;
            daREMOTE = SQLBaseRemote;
            Sync_Database = SyncDatabase;
            remoteConnStr = RemoteConnStr;
            _dtREMOTE_SYNC_CONFLICT = dtREMOTE_SYNC_CONFLICT;
            _dtREMOTE_TOMBSTONE = dtTOMBSTONE;
            _forceSyncAll = forceSyncAll;
            //Set the global progressEventArg for adapter update event
            _CurrentProgressEventArg = new ProcessThreadEventArgs();
        }

        private void Update_Adapter_Progress(object sender, SqlRowUpdatedEventArgs e)
        {
            ProcessProgress_Current(this, _CurrentProgressEventArg);
        }

        private DateTime getTableLastSyncDate(string tableName)
        {
            if(_forceSyncAll)
                return new DateTime(1900, 1, 1);

            string tableNameUnderscore = Common.Replace_WithSpaces(tableName);
            using (AdapterSYNC_RECORD daREMOTE_SYNC_RECORD = new AdapterSYNC_RECORD(remoteConnStr))
            {
                dsSYNC_RECORD.SYNC_RECORDRow drREMOTE_SYNC_RECORD = daREMOTE_SYNC_RECORD.Get_By(HWID, tableNameUnderscore);
                if (drREMOTE_SYNC_RECORD != null)
                {
                    return drREMOTE_SYNC_RECORD.SYNCED;
                }
                else
                    return new DateTime(1900, 1, 1);
            }
        }

        /// <summary>s
        /// Main Process
        /// </summary>
        private void AdvanceStep(Sync_Type SyncType)
        {
            daLOCAL = new SQLBase(Variables.ConnStr); //have to reinitialize for multithread operation because relying on constructor object will sometimes fail
            daREMOTE = new SQLBase(remoteConnStr); //have to reinitialize for multithread operation because relying on constructor object will sometimes fail

            daREMOTE_SYNC_CONFLICT = new AdapterSYNC_CONFLICT(remoteConnStr);
            
            dtLOCAL = new DataTable();
            dtREMOTE = new DataTable();

            dtLOCAL_ALL = new DataTable();
            dtREMOTE_ALL = new DataTable();

            DataTable dtCONFLICT = new DataTable(); //used to reduce dtREMOTE_COMPARE for conflict identification
            DataTable dtREMOTE_COMPARE = new DataTable(); //used to compare any rows in remote that doesn't exists in local datatable

            string sqlIncremental = "";
            string sqlAll = "";
            DateTime tableLastSyncDate = getTableLastSyncDate(SyncType.ToString());
            string tableLastSyncDateStr = tableLastSyncDate.ToString(Common.SQLDateFormatString());

            ProcessLastSyncDateProcessThreadEventArgs ProcessLastSyncEventArg = new ProcessLastSyncDateProcessThreadEventArgs();
            ProcessLastSyncEventArg.progress_type = SyncType;
            ProcessLastSyncEventArg.LastSyncDateTime = tableLastSyncDate;
            ProcessLastSyncDateTime(this, ProcessLastSyncEventArg); //update last sync date

            //Retrieving Data
            //General Section
            if(SyncType == Sync_Type.PROJECT)
            {
                //always sync all projects even when it's incremental only so that upon first sync it doesn't miss anything
                sqlAll += "SELECT * FROM " + SyncType.ToString();
                sqlAll += " ORDER BY CREATED ASC";

                sqlIncremental = sqlAll;
            }
            else if (SyncType == Sync_Type.GENERAL_EQUIPMENT || SyncType == Sync_Type.WORKFLOW_MAIN
                || SyncType == Sync_Type.TEMPLATE_TOGGLE || SyncType == Sync_Type.PREFILL_MAIN
                || SyncType == Sync_Type.ROLE_MAIN || SyncType == Sync_Type.USER_MAIN
                || SyncType == Sync_Type.MATRIX_TYPE)
            {
                sqlIncremental += "SELECT * FROM " + SyncType.ToString();
                sqlIncremental += " WHERE (CREATED >= '" + tableLastSyncDateStr + "' OR UPDATED >= '" + tableLastSyncDateStr + "' OR DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " ORDER BY CREATED ASC";

                sqlAll += "SELECT * FROM " + SyncType.ToString();
                sqlAll += " ORDER BY CREATED ASC";
            }
            else if (SyncType == Sync_Type.CERTIFICATE_DATA)
            {
                sqlIncremental += "SELECT CERTIFICATE_DATA.* FROM CERTIFICATE_DATA ";
                sqlIncremental += " JOIN CERTIFICATE_MAIN ON CERTIFICATE_MAIN.GUID = CERTIFICATE_DATA.CERTIFICATEGUID ";
                sqlIncremental += " WHERE CERTIFICATE_MAIN.PROJECTGUID = '" + ProjectGUID + "'";
                sqlIncremental += " AND (CERTIFICATE_DATA.CREATED >= '" + tableLastSyncDateStr + "' OR CERTIFICATE_DATA.UPDATED >= '" + tableLastSyncDateStr + "' OR CERTIFICATE_DATA.DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " ORDER BY CERTIFICATE_DATA.CREATED ASC";

                sqlAll += "SELECT CERTIFICATE_DATA.* FROM CERTIFICATE_DATA ";
                sqlAll += " JOIN CERTIFICATE_MAIN ON CERTIFICATE_MAIN.GUID = CERTIFICATE_DATA.CERTIFICATEGUID ";
                sqlAll += " WHERE CERTIFICATE_MAIN.PROJECTGUID = '" + ProjectGUID + "'";
                sqlAll += " ORDER BY CERTIFICATE_DATA.CREATED ASC";
            }
            else if (SyncType == Sync_Type.CERTIFICATE_STATUS)
            {
                sqlIncremental += "SELECT CERTIFICATE_STATUS.* FROM CERTIFICATE_STATUS ";
                sqlIncremental += " JOIN CERTIFICATE_MAIN ON CERTIFICATE_MAIN.GUID = CERTIFICATE_STATUS.CERTIFICATE_MAIN_GUID ";
                sqlIncremental += " WHERE CERTIFICATE_MAIN.PROJECTGUID = '" + ProjectGUID + "'";
                sqlIncremental += " AND (CERTIFICATE_STATUS.CREATED >= '" + tableLastSyncDateStr + "' OR CERTIFICATE_STATUS.DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " ORDER BY CERTIFICATE_STATUS.CREATED ASC";

                sqlAll += "SELECT CERTIFICATE_STATUS.* FROM CERTIFICATE_STATUS ";
                sqlAll += " JOIN CERTIFICATE_MAIN ON CERTIFICATE_MAIN.GUID = CERTIFICATE_STATUS.CERTIFICATE_MAIN_GUID ";
                sqlAll += " WHERE CERTIFICATE_MAIN.PROJECTGUID = '" + ProjectGUID + "'";
                sqlAll += " ORDER BY CERTIFICATE_STATUS.CREATED ASC";
            }
            else if (SyncType == Sync_Type.CERTIFICATE_STATUS_ISSUE)
            {
                sqlIncremental += "SELECT CERTIFICATE_STATUS_ISSUE.* FROM CERTIFICATE_STATUS_ISSUE ";
                sqlIncremental += " JOIN CERTIFICATE_STATUS ON CERTIFICATE_STATUS.GUID = CERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_GUID ";
                sqlIncremental += " JOIN CERTIFICATE_MAIN ON CERTIFICATE_MAIN.GUID = CERTIFICATE_STATUS.CERTIFICATE_MAIN_GUID ";
                sqlIncremental += " WHERE CERTIFICATE_MAIN.PROJECTGUID = '" + ProjectGUID + "'";
                sqlIncremental += " AND (CERTIFICATE_STATUS_ISSUE.CREATED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " ORDER BY CERTIFICATE_STATUS_ISSUE.CREATED ASC";

                sqlAll += "SELECT CERTIFICATE_STATUS_ISSUE.* FROM CERTIFICATE_STATUS_ISSUE ";
                sqlAll += " JOIN CERTIFICATE_STATUS ON CERTIFICATE_STATUS.GUID = CERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_GUID ";
                sqlAll += " JOIN CERTIFICATE_MAIN ON CERTIFICATE_MAIN.GUID = CERTIFICATE_STATUS.CERTIFICATE_MAIN_GUID ";
                sqlAll += " WHERE CERTIFICATE_MAIN.PROJECTGUID = '" + ProjectGUID + "'";
                sqlAll += " ORDER BY CERTIFICATE_STATUS_ISSUE.CREATED ASC";
            }
            //table which doesn't have updated field
            else if (SyncType == Sync_Type.ROLE_PRIVILEGE || SyncType == Sync_Type.USER_DISC || SyncType == Sync_Type.USER_PROJECT)
            {
                sqlIncremental += "SELECT * FROM " + SyncType.ToString();
                sqlIncremental += " WHERE (CREATED >= '" + tableLastSyncDateStr + "' OR DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " ORDER BY CREATED ASC";

                sqlAll += "SELECT * FROM " + SyncType.ToString();
                sqlAll += " ORDER BY CREATED ASC";
            }
            else if (SyncType == Sync_Type.MATRIX_ASSIGNMENT)
            {
                sqlIncremental += "SELECT * FROM MATRIX_ASSIGNMENT WHERE GUID_PROJECT = '" + ProjectGUID + "'";
                sqlIncremental += " AND (CREATED >= '" + tableLastSyncDateStr + "' OR UPDATED >= '" + tableLastSyncDateStr + "' OR DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " ORDER BY CREATED ASC";

                sqlAll += "SELECT * FROM MATRIX_ASSIGNMENT WHERE GUID_PROJECT = '" + ProjectGUID + "'";
                sqlAll += " ORDER BY CREATED ASC";
            }
            //By Project Section
            else if (SyncType == Sync_Type.TEMPLATE_MAIN)
            {
                sqlIncremental += "SELECT [GUID],[WORKFLOWGUID],[NAME],[REVISION],[DISCIPLINE],[DESCRIPTION],[QRSUPPORT],[SKIPAPPROVED],[CREATED],[CREATEDBY],[UPDATED],[UPDATEDBY],[DELETED],[DELETEDBY] FROM TEMPLATE_MAIN ";
                sqlIncremental += " WHERE (CREATED >= '" + tableLastSyncDateStr + "' OR UPDATED >= '" + tableLastSyncDateStr + "' OR DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " ORDER BY CREATED ASC";

                sqlAll += "SELECT [GUID],[WORKFLOWGUID],[NAME],[REVISION],[DISCIPLINE],[DESCRIPTION],[QRSUPPORT],[SKIPAPPROVED],[CREATED],[CREATEDBY],[UPDATED],[UPDATEDBY],[DELETED],[DELETEDBY] FROM TEMPLATE_MAIN ";
                sqlAll += " ORDER BY CREATED ASC";
            }
            else if (SyncType == Sync_Type.CERTIFICATE_MAIN)
            {
                sqlIncremental += "SELECT [GUID],[PROJECTGUID],[TEMPLATE_NAME],[NUMBER],[DESCRIPTION],[CREATED],[CREATEDBY],[UPDATED],[UPDATEDBY],[DELETED],[DELETEDBY] FROM CERTIFICATE_MAIN";
                sqlIncremental += " WHERE (CREATED >= '" + tableLastSyncDateStr + "' OR UPDATED >= '" + tableLastSyncDateStr + "' OR DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " ORDER BY CREATED ASC";

                sqlAll += "SELECT [GUID],[PROJECTGUID],[TEMPLATE_NAME],[NUMBER],[DESCRIPTION],[CREATED],[CREATEDBY],[UPDATED],[UPDATEDBY],[DELETED],[DELETEDBY] FROM CERTIFICATE_MAIN ";
                sqlAll += " ORDER BY CREATED ASC";
            }
            else if (SyncType == Sync_Type.SCHEDULE)
            {
                sqlIncremental += "SELECT * FROM SCHEDULE WHERE PROJECTGUID = '" + ProjectGUID + "'";
                sqlIncremental += " AND (CREATED >= '" + tableLastSyncDateStr + "' OR UPDATED >= '" + tableLastSyncDateStr + "' OR DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " ORDER BY CREATED ASC";

                sqlAll += "SELECT * FROM SCHEDULE WHERE PROJECTGUID = '" + ProjectGUID + "'";
                sqlAll += " ORDER BY CREATED ASC";
            }
            else if (SyncType == Sync_Type.TAG || SyncType == Sync_Type.WBS)
            {
                sqlIncremental += "SELECT tagwbs.* FROM " + SyncType.ToString() + " tagwbs JOIN SCHEDULE sch ON (tagwbs.SCHEDULEGUID = sch.GUID)";
                sqlIncremental += " WHERE sch.PROJECTGUID = '" + ProjectGUID + "'";
                sqlIncremental += " AND (tagwbs.CREATED >= '" + tableLastSyncDateStr + "' OR tagwbs.UPDATED >= '" + tableLastSyncDateStr + "' OR tagwbs.DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " ORDER BY tagwbs.CREATED ASC";

                sqlAll += "SELECT tagwbs.* FROM " + SyncType.ToString() + " tagwbs JOIN SCHEDULE sch ON (tagwbs.SCHEDULEGUID = sch.GUID)";
                sqlAll += " WHERE sch.PROJECTGUID = '" + ProjectGUID + "'";
                sqlAll += " ORDER BY tagwbs.CREATED ASC";
            }
            else if (SyncType == Sync_Type.ITR_MAIN)
            {
                //Exclude the actual ITR to reduce transaction duration
                sqlIncremental += "SELECT MainTable.[GUID], MainTable.[TAG_GUID], MainTable.[WBS_GUID], MainTable.[TEMPLATE_GUID], MainTable.[SEQUENCE_NUMBER], MainTable.[NAME], MainTable.[DESCRIPTION], MainTable.[REVISION], MainTable.[DISCIPLINE], MainTable.[TYPE], MainTable.[CREATED], MainTable.[CREATEDBY], MainTable.[UPDATED], MainTable.[UPDATEDBY], MainTable.[DELETED], MainTable.[DELETEDBY] ";
                sqlIncremental += "FROM " + SyncType.ToString() + " MainTable JOIN TAG Tag ON (MainTable.TAG_GUID = Tag.GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID) ";
                sqlIncremental += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlIncremental += " AND (MainTable.CREATED >= '" + tableLastSyncDateStr + "' OR MainTable.UPDATED >= '" + tableLastSyncDateStr + "' OR MainTable.DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " UNION ";
                sqlIncremental += "SELECT MainTable.[GUID], MainTable.[TAG_GUID], MainTable.[WBS_GUID], MainTable.[TEMPLATE_GUID], MainTable.[SEQUENCE_NUMBER], MainTable.[NAME], MainTable.[DESCRIPTION], MainTable.[REVISION], MainTable.[DISCIPLINE], MainTable.[TYPE], MainTable.[CREATED], MainTable.[CREATEDBY], MainTable.[UPDATED], MainTable.[UPDATEDBY], MainTable.[DELETED], MainTable.[DELETEDBY] ";
                sqlIncremental += "FROM " + SyncType.ToString() + " MainTable JOIN WBS Wbs ON (MainTable.WBS_GUID = Wbs.GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Wbs.SCHEDULEGUID) ";
                sqlIncremental += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlIncremental += " AND (MainTable.CREATED >= '" + tableLastSyncDateStr + "' OR MainTable.UPDATED >= '" + tableLastSyncDateStr + "' OR MainTable.DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += "ORDER BY MainTable.CREATED ASC";

                sqlAll += "SELECT MainTable.[GUID], MainTable.[TAG_GUID], MainTable.[WBS_GUID], MainTable.[TEMPLATE_GUID], MainTable.[SEQUENCE_NUMBER], MainTable.[NAME], MainTable.[DESCRIPTION], MainTable.[REVISION], MainTable.[DISCIPLINE], MainTable.[TYPE], MainTable.[CREATED], MainTable.[CREATEDBY], MainTable.[UPDATED], MainTable.[UPDATEDBY], MainTable.[DELETED], MainTable.[DELETEDBY] ";
                sqlAll += "FROM " + SyncType.ToString() + " MainTable JOIN TAG Tag ON (MainTable.TAG_GUID = Tag.GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID) ";
                sqlAll += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlAll += " UNION ";
                sqlAll += "SELECT MainTable.[GUID], MainTable.[TAG_GUID], MainTable.[WBS_GUID], MainTable.[TEMPLATE_GUID], MainTable.[SEQUENCE_NUMBER], MainTable.[NAME], MainTable.[DESCRIPTION], MainTable.[REVISION], MainTable.[DISCIPLINE], MainTable.[TYPE], MainTable.[CREATED], MainTable.[CREATEDBY], MainTable.[UPDATED], MainTable.[UPDATEDBY], MainTable.[DELETED], MainTable.[DELETEDBY] ";
                sqlAll += "FROM " + SyncType.ToString() + " MainTable JOIN WBS Wbs ON (MainTable.WBS_GUID = Wbs.GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Wbs.SCHEDULEGUID) ";
                sqlAll += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlAll += "ORDER BY MainTable.CREATED ASC";
            }
            else if (SyncType == Sync_Type.PREFILL_REGISTER || SyncType == Sync_Type.PUNCHLIST_MAIN)
            {
                sqlIncremental += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN TAG Tag ON (MainTable.TAG_GUID = Tag.GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID) ";
                sqlIncremental += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlIncremental += " AND (MainTable.CREATED >= '" + tableLastSyncDateStr + "' OR MainTable.UPDATED >= '" + tableLastSyncDateStr + "' OR MainTable.DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " UNION ";
                sqlIncremental += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN WBS Wbs ON (MainTable.WBS_GUID = Wbs.GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Wbs.SCHEDULEGUID) ";
                sqlIncremental += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "' ";
                sqlIncremental += " AND (MainTable.CREATED >= '" + tableLastSyncDateStr + "' OR MainTable.UPDATED >= '" + tableLastSyncDateStr + "' OR MainTable.DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += "ORDER BY MainTable.CREATED ASC";

                sqlAll += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN TAG Tag ON (MainTable.TAG_GUID = Tag.GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID) ";
                sqlAll += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlAll += " UNION ";
                sqlAll += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN WBS Wbs ON (MainTable.WBS_GUID = Wbs.GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Wbs.SCHEDULEGUID) ";
                sqlAll += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "' ";
                sqlAll += "ORDER BY MainTable.CREATED ASC";
            }
            //table which doesn't have update field
            else if (SyncType == Sync_Type.TEMPLATE_REGISTER)
            {
                sqlIncremental += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN TAG Tag ON (MainTable.TAG_GUID = Tag.GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID) ";
                sqlIncremental += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlIncremental += " AND (MainTable.CREATED >= '" + tableLastSyncDateStr + "' OR MainTable.DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " UNION ";
                sqlIncremental += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN WBS Wbs ON (MainTable.WBS_GUID = Wbs.GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Wbs.SCHEDULEGUID) ";
                sqlIncremental += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "' ";
                sqlIncremental += " AND (MainTable.CREATED >= '" + tableLastSyncDateStr + "' OR MainTable.DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += "ORDER BY MainTable.CREATED ASC";

                sqlAll += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN TAG Tag ON (MainTable.TAG_GUID = Tag.GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID) ";
                sqlAll += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlAll += " UNION ";
                sqlAll += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN WBS Wbs ON (MainTable.WBS_GUID = Wbs.GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Wbs.SCHEDULEGUID) ";
                sqlAll += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "' ";
                sqlAll += "ORDER BY MainTable.CREATED ASC";
            }
            else if (SyncType == Sync_Type.ITR_STATUS || SyncType == Sync_Type.PUNCHLIST_STATUS)
            {
                string LinkedTableName = SyncType.ToString().Split('_').First();

                sqlIncremental += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN " + LinkedTableName + "_MAIN LinkedTable ON (LinkedTable.GUID = MainTable." + LinkedTableName + "_MAIN_GUID) ";
                sqlIncremental += "JOIN TAG Tag ON (Tag.GUID = LinkedTable.TAG_GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID) ";
                sqlIncremental += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlIncremental += " AND (MainTable.CREATED >= '" + tableLastSyncDateStr + "' OR MainTable.DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " UNION ";
                sqlIncremental += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN " + LinkedTableName + "_MAIN LinkedTable ON (LinkedTable.GUID = MainTable." + LinkedTableName + "_MAIN_GUID) ";
                sqlIncremental += "JOIN WBS Wbs ON (Wbs.GUID = LinkedTable.WBS_GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Wbs.SCHEDULEGUID) ";
                sqlIncremental += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlIncremental += " AND (MainTable.CREATED >= '" + tableLastSyncDateStr + "' OR MainTable.DELETED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += "ORDER BY MainTable.CREATED ASC";

                sqlAll += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN " + LinkedTableName + "_MAIN LinkedTable ON (LinkedTable.GUID = MainTable." + LinkedTableName + "_MAIN_GUID) ";
                sqlAll += "JOIN TAG Tag ON (Tag.GUID = LinkedTable.TAG_GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID) ";
                sqlAll += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlAll += " UNION ";
                sqlAll += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN " + LinkedTableName + "_MAIN LinkedTable ON (LinkedTable.GUID = MainTable." + LinkedTableName + "_MAIN_GUID) ";
                sqlAll += "JOIN WBS Wbs ON (Wbs.GUID = LinkedTable.WBS_GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Wbs.SCHEDULEGUID) ";
                sqlAll += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlAll += "ORDER BY MainTable.CREATED ASC";
            }
            else if (SyncType == Sync_Type.ITR_STATUS_ISSUE || SyncType == Sync_Type.PUNCHLIST_STATUS_ISSUE)
            {
                string LinkedTableName = SyncType.ToString().Split('_').First();

                sqlIncremental += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN " + LinkedTableName + "_STATUS LinkedStatusTable ON (LinkedStatusTable.GUID = MainTable." + LinkedTableName + "_STATUS_GUID) ";
                sqlIncremental += "JOIN " + LinkedTableName + "_MAIN LinkedStatusMain ON (LinkedStatusMain.GUID = LinkedStatusTable." + LinkedTableName + "_MAIN_GUID) ";
                sqlIncremental += "JOIN TAG Tag ON (Tag.GUID = LinkedStatusMain.TAG_GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID) ";
                sqlIncremental += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlIncremental += " AND (MainTable.CREATED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += " UNION ";
                sqlIncremental += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN " + LinkedTableName + "_STATUS LinkedStatusTable ON (LinkedStatusTable.GUID = MainTable." + LinkedTableName + "_STATUS_GUID) ";
                sqlIncremental += "JOIN " + LinkedTableName + "_MAIN LinkedStatusMain ON (LinkedStatusMain.GUID = LinkedStatusTable." + LinkedTableName + "_MAIN_GUID) ";
                sqlIncremental += "JOIN WBS Wbs ON (Wbs.GUID = LinkedStatusMain.WBS_GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Wbs.SCHEDULEGUID) ";
                sqlIncremental += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlIncremental += " AND (MainTable.CREATED >= '" + tableLastSyncDateStr + "')";
                sqlIncremental += "ORDER BY MainTable.CREATED ASC";

                sqlAll += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN " + LinkedTableName + "_STATUS LinkedStatusTable ON (LinkedStatusTable.GUID = MainTable." + LinkedTableName + "_STATUS_GUID) ";
                sqlAll += "JOIN " + LinkedTableName + "_MAIN LinkedStatusMain ON (LinkedStatusMain.GUID = LinkedStatusTable." + LinkedTableName + "_MAIN_GUID) ";
                sqlAll += "JOIN TAG Tag ON (Tag.GUID = LinkedStatusMain.TAG_GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID) ";
                sqlAll += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlAll += " UNION ";
                sqlAll += "SELECT MainTable.* FROM " + SyncType.ToString() + " MainTable JOIN " + LinkedTableName + "_STATUS LinkedStatusTable ON (LinkedStatusTable.GUID = MainTable." + LinkedTableName + "_STATUS_GUID) ";
                sqlAll += "JOIN " + LinkedTableName + "_MAIN LinkedStatusMain ON (LinkedStatusMain.GUID = LinkedStatusTable." + LinkedTableName + "_MAIN_GUID) ";
                sqlAll += "JOIN WBS Wbs ON (Wbs.GUID = LinkedStatusMain.WBS_GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Wbs.SCHEDULEGUID) ";
                sqlAll += "WHERE Schedule.PROJECTGUID = '" + ProjectGUID + "'";
                sqlAll += "ORDER BY MainTable.CREATED ASC";
            }

            ProcessThreadEventArgs ProgressEventArg_Overall = new ProcessThreadEventArgs();
            ProcessThreadEventArgs ProgressEventArg_Current = new ProcessThreadEventArgs();

            ProgressEventArg_Overall.progress_type = SyncType;
            ProgressEventArg_Overall.progress_countType = SyncStatus_Count.None;
            ProgressEventArg_Overall.progress_status = SyncStatus_Display.Downloading.ToString();
            ProgressEventArg_Overall.progress_step = 0;
            ProcessProgress_Overall(this, ProgressEventArg_Overall);

            daLOCAL.ExecuteQuery(sqlIncremental, dtLOCAL);
            daLOCAL.ExecuteQuery(sqlAll, dtLOCAL_ALL);

            ProgressEventArg_Current.progress_type = SyncType;
            ProgressEventArg_Current.progress_step = 100;
            ProcessProgress_Current(this, ProgressEventArg_Current); //Current Progress 100

            ProgressEventArg_Overall.progress_step = 5;
            ProcessProgress_Overall(this, ProgressEventArg_Overall); //Overall Progress 5

            ProgressEventArg_Current.progress_reset = true;
            ProcessProgress_Current(this, ProgressEventArg_Current); //Current Progress 0

            daREMOTE.ExecuteQuery(sqlIncremental, dtREMOTE);
            daREMOTE.ExecuteQuery(sqlAll, dtREMOTE_ALL);
            ProgressEventArg_Overall.progress_step = 5;
            ProcessProgress_Overall(this, ProgressEventArg_Overall); //Overall Progress 10

            ProgressEventArg_Current.progress_step = 100;
            ProcessProgress_Current(this, ProgressEventArg_Current); //Current Progress 100

            if(SyncType == Sync_Type.ITR_MAIN || SyncType == Sync_Type.TEMPLATE_MAIN || SyncType == Sync_Type.CERTIFICATE_MAIN)
            {
                Image emptyImage = new Bitmap(1024, 768);
                byte[] emptyByte = Common.ConvertImageToByteArray(emptyImage);

                string dcColName = string.Empty;
                int dcOrdinal = 1;
                //Retrieve the actual ITR for saving
                if (SyncType == Sync_Type.ITR_MAIN)
                {
                    dcColName = "ITR";
                    dcOrdinal = 10;
                }
                else if (SyncType == Sync_Type.TEMPLATE_MAIN)
                {
                    dcColName = "TEMPLATE";
                    dcOrdinal = 6;
                }
                else if(SyncType == Sync_Type.CERTIFICATE_MAIN)
                {
                    dcColName = "CERTIFICATE";
                    dcOrdinal = 5;
                }

                DataColumn dcLOCAL = new DataColumn(dcColName);
                dcLOCAL.DataType = typeof(byte[]);
                dcLOCAL.DefaultValue = emptyByte;

                DataColumn dcLOCAL_ALL = new DataColumn(dcColName);
                dcLOCAL_ALL.DataType = typeof(byte[]);
                dcLOCAL_ALL.DefaultValue = emptyByte;

                DataColumn dcREMOTE = new DataColumn(dcColName);
                dcREMOTE.DataType = typeof(byte[]);
                dcREMOTE.DefaultValue = emptyByte;

                DataColumn dcREMOTE_ALL = new DataColumn(dcColName);
                dcREMOTE_ALL.DataType = typeof(byte[]);
                dcREMOTE_ALL.DefaultValue = emptyByte;

                dtLOCAL.Columns.Add(dcLOCAL);
                dcLOCAL.SetOrdinal(dcOrdinal);
                dtREMOTE.Columns.Add(dcREMOTE);
                dcREMOTE.SetOrdinal(dcOrdinal);

                dtLOCAL_ALL.Columns.Add(dcLOCAL_ALL);
                dcLOCAL_ALL.SetOrdinal(dcOrdinal);
                dtREMOTE_ALL.Columns.Add(dcREMOTE_ALL);
                dcREMOTE_ALL.SetOrdinal(dcOrdinal);
            }

            //copy the schema from another table to avoid new row having now columns
            if (dtLOCAL.Rows.Count == 0)
                dtLOCAL = dtREMOTE.Clone();

            if (dtREMOTE.Rows.Count == 0)
                dtREMOTE = dtLOCAL.Clone();

            if (dtLOCAL_ALL.Rows.Count == 0)
                dtLOCAL_ALL = dtREMOTE_ALL.Clone();

            if (dtREMOTE_ALL.Rows.Count == 0)
                dtREMOTE_ALL = dtLOCAL_ALL.Clone();

            //clone schema
            dtCONFLICT = dtLOCAL_ALL.Clone();
            dtREMOTE_COMPARE = dtREMOTE.Clone();

            dtREMOTE_COMPARE.Merge(dtREMOTE);

            bool AnyLocalUpdates = false;
            bool AnyRemoteUpdates = false;

            bool ContainsDeleteField = dtLOCAL.Columns.Contains("DELETED");
            bool ContainsUpdateField = dtLOCAL.Columns.Contains("UPDATED"); //sql query is same for remote so there is no need to check for remote

            ProgressEventArg_Current.progress_reset = true;
            ProcessProgress_Current(this, ProgressEventArg_Current); //Current Progress 0

            double Record_Step_Current = 100.0 / ((double)dtLOCAL.Rows.Count + (double)dtREMOTE.Rows.Count);
            double Record_Step_Overall = 60.0 / ((double)dtLOCAL.Rows.Count + (double)dtREMOTE.Rows.Count);
            ProgressEventArg_Current.progress_step = Record_Step_Current;
            ProgressEventArg_Overall.progress_step = Record_Step_Overall;

            if (double.IsInfinity(Record_Step_Overall))
            {
                ProgressEventArg_Current.progress_step = 100.0;
                ProgressEventArg_Overall.progress_step = 60.0;
                ProcessProgress_Overall(this, ProgressEventArg_Overall); //Overall Progress 60
                ProcessProgress_Current(this, ProgressEventArg_Current); //Current Progress 100
            }

            ProgressEventArg_Overall.progress_status = SyncStatus_Display.Comparing.ToString();
            List<Guid> SafeDeletionList = new List<Guid>();

            //Traverse local and sync new and edited
            foreach (DataRow drLOCAL in dtLOCAL.Rows)
            {
                try
                {
                    //Try to get remote project
                    DataRow drREMOTE_ALL = dtREMOTE_ALL.AsEnumerable().FirstOrDefault(obj => obj.Field<Guid>("GUID") == drLOCAL.Field<Guid>("GUID"));
                    //If remote project doesn't exist
                    if (drREMOTE_ALL == null)
                    {
                        DataRow drTOMBSTONE = _dtREMOTE_TOMBSTONE.AsEnumerable().FirstOrDefault(obj => obj.TABLENAME == SyncType.ToString() && obj.TOMBSTONE_GUID == drLOCAL.Field<Guid>("GUID"));
                        if (drTOMBSTONE != null)
                        {
                            SafeDeletionList.Add(drLOCAL.Field<Guid>("GUID"));
                            ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Delete;
                        }
                        else
                        {
                            DataRow drNEWREMOTE = dtREMOTE_ALL.NewRow();
                            drNEWREMOTE.ItemArray = drLOCAL.ItemArray;

                            bool fullyIdentical = false;
                            ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Upload;
                            if (ContainsDeleteField && !drLOCAL.IsNull("DELETED")) //if it was created and deleted locally before sync happens, it basically resolved itself
                            {
                                //Conflict_Identification requires that any one time each system only have 1 working record, the rest should be deleted
                                DataRow drREMOTE_IDENTICAL = Conflict_Identification(drLOCAL, dtREMOTE_COMPARE, SyncType, out fullyIdentical);
                                if (drREMOTE_IDENTICAL != null)
                                {
                                    //When remote record with the appropriate key is found but not all fields are matching, it'll be returned as not deleted
                                    //Hence its necessary to delete the local copy
                                    if (fullyIdentical)
                                    {
                                        //Delete local record because it already exists remotely
                                        drLOCAL["DELETED"] = DateTime.Now;
                                        drLOCAL["DELETEDBY"] = Guid.Empty;
                                        //Add the deleted record remotely so both database have the same record
                                        drNEWREMOTE["DELETED"] = drLOCAL["DELETED"];
                                        drNEWREMOTE["DELETEDBY"] = drLOCAL["DELETEDBY"];
                                        //Identical record will be used as the working record, no changes made to identical found
                                        //Mark as resolved
                                        ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Resolve;
                                    }
                                    //If the remote identical is not used as a working record locally
                                    //Add the local record (New Remote) to remote server as a conflict for conflict resolve usage
                                    else
                                    {
                                        //Current working local is added remotely as a conflict record
                                        drNEWREMOTE["DELETED"] = Variables.Conflict_Date;
                                        drNEWREMOTE["DELETEDBY"] = Guid.Empty;
                                        //Current working remote is added locally as a conflict record
                                        drREMOTE_IDENTICAL["DELETED"] = Variables.Conflict_Date;
                                        drREMOTE_IDENTICAL["DELETEDBY"] = Guid.Empty;
                                        ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Conflict;
                                    }

                                    //Local conflict contains remote data that has been identified as the working copy
                                    //Add it into a separate datatable so that dtREMOTE_COMPARE can be reduced
                                    DataRow drCONFLICT = dtCONFLICT.NewRow();
                                    drCONFLICT.ItemArray = drREMOTE_IDENTICAL.ItemArray;
                                    dtCONFLICT.Rows.Add(drCONFLICT); //add the server data locally

                                    //Remove the compared record because REMOTE_COMPARE should only contain new row not added locally
                                    //if it is auto resolved - local data is deleted and uploaded as deleted to remote, and remote data have to be added as normal, hence cannot be removed from remote_compare
                                    DataRow drREMOTE_COMPARE = dtREMOTE_COMPARE.AsEnumerable().FirstOrDefault(obj => obj.Field<Guid>("GUID") == drREMOTE_IDENTICAL.Field<Guid>("GUID"));
                                    if (drREMOTE_COMPARE != null)
                                    {
                                        dtREMOTE_COMPARE.Rows.Remove(drREMOTE_COMPARE);
                                        //a double step is performed by reducing the record to be added
                                        ProgressEventArg_Overall.progress_doublestep = true;
                                        ProgressEventArg_Current.progress_doublestep = true;
                                    }
                                }
                            }

                            dtREMOTE_ALL.Rows.Add(drNEWREMOTE);
                            AnyRemoteUpdates = true;
                        }
                    }
                    //If its found, compare datetime to determine whether record should be pushed or pulled
                    else
                    {
                        bool Evaluated_As_Conflict = false; //Subsequent sync without resolving will not be evaluated
                        bool ProcessedByDeletedField = false;
                        bool Update_Status_Handled = false;
                        if (ContainsDeleteField)
                        {
                            if (!drREMOTE_ALL.IsNull("DELETED") && drREMOTE_ALL.Field<DateTime>("DELETED") == Variables.Conflict_Date)
                            {
                                ProgressEventArg_Overall.progress_countType = SyncStatus_Count.None; //only do count on local to avoid doubling up
                                Evaluated_As_Conflict = true;
                            }
                            else if (!drLOCAL.IsNull("DELETED") && drLOCAL.Field<DateTime>("DELETED") == Variables.Conflict_Date)
                            {
                                //Look for GUID in Sync_Conflict table. The GUID to look for can be client GUID or server GUID (as Conflict_Date)
                                dsSYNC_CONFLICT.SYNC_CONFLICTRow drSYNC_CONFLICT = _dtREMOTE_SYNC_CONFLICT.AsEnumerable().FirstOrDefault(obj => obj.CONFLICT_GUID == drLOCAL.Field<Guid>("GUID") || obj.CONFLICT_ON_GUID == drLOCAL.Field<Guid>("GUID"));
                                if (drSYNC_CONFLICT != null)
                                {
                                    //If the resolved is current local. Report as resolve
                                    if (!drSYNC_CONFLICT.IsRESOLVE_GUIDNull() && drSYNC_CONFLICT.RESOLVE_GUID == drLOCAL.Field<Guid>("GUID"))
                                    {
                                        drLOCAL.ItemArray = drREMOTE_ALL.ItemArray;
                                        AnyLocalUpdates = true;
                                        //Not skipping conflict evaluation because local data might be more updated than remote data
                                        //There's an issue where local is deleted but chosen as a resolve, that'll mean remote record will be deleted as well
                                        ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Resolve;
                                        Update_Status_Handled = true; //Not allowing resolve status to be overriden
                                    }
                                    //If the resolved is not current local. Adjust the Deleted date as actual resolved date
                                    else if (!drSYNC_CONFLICT.IsRESOLVE_GUIDNull())
                                    {
                                        drLOCAL.ItemArray = drREMOTE_ALL.ItemArray;
                                        AnyLocalUpdates = true;
                                        Evaluated_As_Conflict = true;
                                        ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Resolve;
                                    }
                                    //If the conflict hasn't been resolved yet. Remain status quo
                                    else
                                    {
                                        Evaluated_As_Conflict = true;
                                        ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Conflict;
                                    }
                                }
                                //Should not happen unless sync server has been changed between conflict and conflict resolved sync
                                else
                                {
                                    Evaluated_As_Conflict = true;
                                    ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Conflict;
                                }
                            }
                            //only perform deletion if updated is the same
                            if (!ContainsUpdateField || drLOCAL.IsNull("UPDATED") && drREMOTE_ALL.IsNull("UPDATED") || !drLOCAL.IsNull("UPDATED") && !drREMOTE_ALL.IsNull("UPDATED") && drLOCAL.Field<DateTime>("UPDATED") == drREMOTE_ALL.Field<DateTime>("UPDATED"))
                            {
                                //undelete mechanism, when created is more updated then deleted, restore both remote and local
                                if (ContainsUpdateField && !drREMOTE_ALL.IsNull("UPDATED") && !drLOCAL.IsNull("DELETED") && drREMOTE_ALL.Field<DateTime>("UPDATED") > drLOCAL.Field<DateTime>("DELETED"))
                                {
                                    ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Resolve;
                                    drREMOTE_ALL["DELETED"] = DBNull.Value;
                                    drREMOTE_ALL["DELETEDBY"] = DBNull.Value;
                                    drLOCAL.ItemArray = drREMOTE_ALL.ItemArray;
                                    AnyLocalUpdates = true;
                                    AnyRemoteUpdates = true;
                                }
                                else if (drLOCAL.Field<DateTime>("CREATED") == drREMOTE_ALL.Field<DateTime>("CREATED"))
                                {
                                    //Validation for deleted is either one is null or both is not null and one is more deleted than the other
                                    if (!drLOCAL.IsNull("DELETED") && (drREMOTE_ALL.IsNull("DELETED") || drLOCAL.Field<DateTime>("DELETED") > drREMOTE_ALL.Field<DateTime>("DELETED")))
                                    {
                                        if (drREMOTE_ALL.IsNull("DELETED"))
                                            ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Delete;
                                        else
                                            ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Upload;

                                        ProcessedByDeletedField = true;
                                        drREMOTE_ALL.ItemArray = drLOCAL.ItemArray;
                                        AnyRemoteUpdates = true;
                                    }
                                    else if (!drREMOTE_ALL.IsNull("DELETED") && (drLOCAL.IsNull("DELETED") || drREMOTE_ALL.Field<DateTime>("DELETED") > drLOCAL.Field<DateTime>("DELETED")))
                                    {
                                        if (drLOCAL.IsNull("DELETED"))
                                            ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Delete;
                                        else
                                            ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Download;

                                        ProcessedByDeletedField = true;
                                        drLOCAL.ItemArray = drREMOTE_ALL.ItemArray;
                                        AnyLocalUpdates = true;
                                    }
                                    else if (!drLOCAL.IsNull("DELETED") && !drREMOTE_ALL.IsNull("DELETED") && drREMOTE_ALL.Field<DateTime>("DELETED") == drLOCAL.Field<DateTime>("DELETED"))
                                    {
                                        if(drREMOTE_ALL.Field<DateTime>("DELETED") == Variables.Conflict_Date)
                                            ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Resolve;
                                        else
                                            ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Same;

                                        ProcessedByDeletedField = true;
                                    }
                                }
                            }
                        }

                        if (!Evaluated_As_Conflict && !ProcessedByDeletedField)
                        {
                            //bool isProcessedByDeleted = (!ContainsDeleteField || (ContainsDeleteField && drLOCAL.Field<DateTime>("DELETED") == drREMOTE.Field<DateTime>("DELETED")));
                            if (ContainsUpdateField)
                            {
                                //Validation for updated if either one is null or both is not null and one is more updated than the other
                                if (!drLOCAL.IsNull("UPDATED") && (drREMOTE_ALL.IsNull("UPDATED") || drLOCAL.Field<DateTime>("UPDATED") > drREMOTE_ALL.Field<DateTime>("UPDATED")))
                                {
                                    drREMOTE_ALL.ItemArray = drLOCAL.ItemArray;
                                    AnyRemoteUpdates = true;
                                    if (!Update_Status_Handled)
                                        ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Upload;
                                }
                                else if (!drREMOTE_ALL.IsNull("UPDATED") && (drLOCAL.IsNull("UPDATED") || drREMOTE_ALL.Field<DateTime>("UPDATED") > drLOCAL.Field<DateTime>("UPDATED")))
                                {
                                    drLOCAL.ItemArray = drREMOTE_ALL.ItemArray;
                                    AnyLocalUpdates = true;
                                    if (!Update_Status_Handled)
                                        ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Download;
                                }
                                else if (!drLOCAL.IsNull("UPDATED") && !drREMOTE_ALL.IsNull("UPDATED") && drREMOTE_ALL.Field<DateTime>("UPDATED") == drLOCAL.Field<DateTime>("UPDATED"))
                                {
                                    if (!Update_Status_Handled)
                                        ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Same;
                                }
                            }

                            if ((!ContainsUpdateField || (ContainsUpdateField && drLOCAL.IsNull("UPDATED") && drREMOTE_ALL.IsNull("UPDATED"))))
                            {
                                if (drLOCAL.Field<DateTime>("CREATED") > drREMOTE_ALL.Field<DateTime>("CREATED"))
                                {
                                    drREMOTE_ALL.ItemArray = drLOCAL.ItemArray;
                                    AnyRemoteUpdates = true;
                                    //No need to check for update status handled because both local and remote will have the same created date
                                    //If the created date is different GUID will be different as well
                                    ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Upload;
                                }
                                else if (drREMOTE_ALL.Field<DateTime>("CREATED") > drLOCAL.Field<DateTime>("CREATED"))
                                {
                                    drLOCAL.ItemArray = drREMOTE_ALL.ItemArray;
                                    AnyLocalUpdates = true;
                                    ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Download;
                                }
                                else if (drREMOTE_ALL.Field<DateTime>("CREATED") == drLOCAL.Field<DateTime>("CREATED"))
                                    ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Same;
                            }
                        }

                        //Remove the compared record because REMOTE_COMPARE should only contain new row not added locally
                        DataRow drREMOTE_COMPARE = dtREMOTE_COMPARE.AsEnumerable().FirstOrDefault(obj => obj.Field<Guid>("GUID") == drLOCAL.Field<Guid>("GUID"));
                        if (drREMOTE_COMPARE != null)
                        {
                            dtREMOTE_COMPARE.Rows.Remove(drREMOTE_COMPARE);
                            ProgressEventArg_Overall.progress_doublestep = true;
                            ProgressEventArg_Current.progress_doublestep = true;
                        }
                    }
                }
                catch(Exception e)
                {
                }

                ProcessProgress_Overall(this, ProgressEventArg_Overall);
                ProcessProgress_Current(this, ProgressEventArg_Current);
                ProgressEventArg_Overall.progress_doublestep = false;
                ProgressEventArg_Current.progress_doublestep = false;
            }

            //Traverse remote and sync new
            //Use remote compare datatable because we only want to search for new record in original remote database
            foreach (DataRow drREMOTE in dtREMOTE_COMPARE.Rows)
            {
                //Try to get local project
                DataRow drLOCAL_ALL = dtLOCAL_ALL.AsEnumerable().FirstOrDefault(obj => obj.Field<Guid>("GUID") == drREMOTE.Field<Guid>("GUID"));

                if (drLOCAL_ALL == null)
                {
                    DataRow drNEWLOCAL = dtLOCAL_ALL.NewRow();
                    drNEWLOCAL.ItemArray = drREMOTE.ItemArray;
                    AnyLocalUpdates = true;
                    ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Download;

                    dtLOCAL_ALL.Rows.Add(drNEWLOCAL);
                }
                else
                {
                    ProgressEventArg_Overall.progress_countType = SyncStatus_Count.None;
                }

                ProcessProgress_Overall(this, ProgressEventArg_Overall);
                ProcessProgress_Current(this, ProgressEventArg_Current);
            }

            //dtCONFLICT is used to reduce the dtREMOTE_COMPARE row count for conflict identification and should be treated the same as dtREMOTE_COMPARE
            foreach (DataRow drCONFLICT in dtCONFLICT.Rows)
            {
                //Try to get local project
                DataRow drLOCAL_ALL = dtLOCAL_ALL.AsEnumerable().FirstOrDefault(obj => obj.Field<Guid>("GUID") == drCONFLICT.Field<Guid>("GUID"));

                if (drLOCAL_ALL == null)
                {
                    DataRow drNEWLOCAL = dtLOCAL_ALL.NewRow();
                    drNEWLOCAL.ItemArray = drCONFLICT.ItemArray;
                    AnyLocalUpdates = true;
                    ProgressEventArg_Overall.progress_countType = SyncStatus_Count.Download;
                    dtLOCAL_ALL.Rows.Add(drNEWLOCAL);
                }
                else
                {
                    ProgressEventArg_Overall.progress_countType = SyncStatus_Count.None;
                }

                ProcessProgress_Overall(this, ProgressEventArg_Overall);
                ProcessProgress_Current(this, ProgressEventArg_Current);
            }

            ProgressEventArg_Overall.progress_step = 0;
            ProgressEventArg_Overall.progress_countType = SyncStatus_Count.None;
            ProgressEventArg_Overall.progress_status = SyncStatus_Display.Downloading.ToString();
            ProcessProgress_Overall(this, ProgressEventArg_Overall); //Overall Progress 70

            ProgressEventArg_Current.progress_reset = true;
            ProcessProgress_Current(this, ProgressEventArg_Current); //Current Progress 0


            //Tombstone deletion
            for(int i = 0; i < dtLOCAL_ALL.Rows.Count - 1; i++)
            {
                if(SafeDeletionList.Any(obj => obj == dtLOCAL_ALL.Rows[i].Field<Guid>("GUID")))
                {
                    if (dtLOCAL_ALL.Rows[i].RowState != DataRowState.Deleted)
                    {
                        dtLOCAL_ALL.Rows[i].Delete();
                        AnyLocalUpdates = true;
                    }
                }    
            }

            if (AnyLocalUpdates)
            {
                //addition and updates can happen on both tables so save twice, row state will determine whether to interact with DB
                Save_DataTable(Database_Type.LOCAL, dtLOCAL, SyncType);
                Save_DataTable(Database_Type.LOCAL, dtLOCAL_ALL, SyncType);
            }
            else
                ProgressEventArg_Current.progress_step = 100.0;

            ProgressEventArg_Overall.progress_status = SyncStatus_Display.Uploading.ToString();
            ProgressEventArg_Overall.progress_step = 15;
            ProcessProgress_Overall(this, ProgressEventArg_Overall); //Overall Progress 85

            ProcessProgress_Current(this, ProgressEventArg_Current); //Current Progress 100
            ProgressEventArg_Current.progress_reset = true;
            ProcessProgress_Current(this, ProgressEventArg_Current); //Current Progress 0

            if (AnyRemoteUpdates)
            {
                //addition and updates can happen on both tables so save twice, row state will determine whether to interact with DB
                Save_DataTable(Database_Type.REMOTE, dtREMOTE, SyncType);
                Save_DataTable(Database_Type.REMOTE, dtREMOTE_ALL, SyncType);
            }
            else
                ProgressEventArg_Current.progress_step = 100.0;

            ProgressEventArg_Overall.progress_status = SyncStatus_Display.Ok.ToString();
            ProgressEventArg_Overall.progress_step = 15;
            ProcessProgress_Overall(this, ProgressEventArg_Overall); //Overall Progress 100
            ProcessProgress_Current(this, ProgressEventArg_Current); //Current Progress 100
        }

        /// <summary>
        /// Compare 2 rows with certain exclusion
        /// </summary>
        private bool isRow_Identical(DataRow drFROM, DataRow drTO, bool ignoreUpdated = true)
        {
            List<string> Exclusions = new List<string>();
            //add common exclusions
            Exclusions.Add("GUID");
            Exclusions.Add("CREATED");
            Exclusions.Add("CREATEDBY");

            if(ignoreUpdated)
            {
                Exclusions.Add("UPDATED");
                Exclusions.Add("UPDATEDBY");
            }

            Exclusions.Add("DELETED");
            Exclusions.Add("DELETEDBY");

            int ItemCount = drFROM.ItemArray.Count();

            foreach(DataColumn dcFROM in drFROM.Table.Columns)
            {
                if(!Exclusions.Contains(dcFROM.ColumnName))
                {
                    if (!drFROM[dcFROM.ColumnName].Equals(drTO[dcFROM.ColumnName]))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Identify Unique Entries in the Database and Log Conflict
        /// </summary>
        private DataRow Conflict_Identification(DataRow DataRow_From, DataTable DataTable_To, Sync_Type SyncType, out bool fullyIdentical)
        {
            fullyIdentical = false;
            if(SyncType == Sync_Type.GENERAL_EQUIPMENT)
            {
                dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtEQUIPMENT_To = new dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable();
                dtEQUIPMENT_To.Merge(DataTable_To);
                //put datarow into schema for comparison
                dsGENERAL_EQUIPMENT dsGENERALEQUIPMENT = new dsGENERAL_EQUIPMENT();
                dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow drEQUIPMENT_From = dsGENERALEQUIPMENT.GENERAL_EQUIPMENT.NewGENERAL_EQUIPMENTRow();
                drEQUIPMENT_From.ItemArray = DataRow_From.ItemArray;

                dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow drEQUIPMENT_To = dtEQUIPMENT_To.AsEnumerable().FirstOrDefault(obj => obj.SERIAL == drEQUIPMENT_From.SERIAL && obj.IsDELETEDNull());
                if(drEQUIPMENT_To != null)
                {
                    //compare with certain exclusions to determine whether problem can be resolved automatically
                    if (!isRow_Identical(drEQUIPMENT_From, drEQUIPMENT_To))
                        Log_Conflict(Common.GetHWID(), SyncType, drEQUIPMENT_To.GUID, drEQUIPMENT_From.GUID);
                    else
                        fullyIdentical = true;

                    return drEQUIPMENT_To; //return this datarow to be added as deleted
                }
                //if its already deleted, skip the conflict logging and skip removing from dtREMOTE_COMPARE so that it'll resume into local addition later on
                //hereby skips conflict that's already been recorded as well
            }
            else if(SyncType == Sync_Type.ITR_MAIN)
            {
                dsITR_MAIN.ITR_MAINDataTable dtITR_To = new dsITR_MAIN.ITR_MAINDataTable();
                dtITR_To.Merge(DataTable_To);
                dsITR_MAIN dsITRMAIN = new dsITR_MAIN();
                dsITR_MAIN.ITR_MAINRow drITR_From = dsITRMAIN.ITR_MAIN.NewITR_MAINRow();
                drITR_From.ItemArray = DataRow_From.ItemArray;

                dsITR_MAIN.ITR_MAINRow drITR_To;
                if(drITR_From.IsWBS_GUIDNull())
                    drITR_To = dtITR_To.AsEnumerable().FirstOrDefault(obj => (!obj.IsTAG_GUIDNull()  && (obj.TAG_GUID == drITR_From.TAG_GUID)) 
                                                                                && obj.TEMPLATE_GUID == drITR_From.TEMPLATE_GUID
                                                                                && obj.IsDELETEDNull());
                else
                    drITR_To = dtITR_To.AsEnumerable().FirstOrDefault(obj => (!obj.IsWBS_GUIDNull() && (obj.WBS_GUID == drITR_From.WBS_GUID))
                                                                            && obj.TEMPLATE_GUID == drITR_From.TEMPLATE_GUID
                                                                            && obj.IsDELETEDNull());

                if(drITR_To != null)
                {
                    //Do not ignore updated because the ITR content couldn't be compared
                    if (!isRow_Identical(drITR_From, drITR_To, false))
                        Log_Conflict(Common.GetHWID(), SyncType, drITR_To.GUID, drITR_From.GUID);
                    else
                        fullyIdentical = true;

                    return drITR_To;
                }
            }
            else if (SyncType == Sync_Type.PREFILL_MAIN)
            {
                dsPREFILL_MAIN.PREFILL_MAINDataTable dtPREFILL_To = new dsPREFILL_MAIN.PREFILL_MAINDataTable();
                dtPREFILL_To.Merge(DataTable_To);
                //put datarow into schema for comparison
                dsPREFILL_MAIN dsPREFILLMAIN = new dsPREFILL_MAIN();
                dsPREFILL_MAIN.PREFILL_MAINRow drPREFILL_From = dsPREFILLMAIN.PREFILL_MAIN.NewPREFILL_MAINRow();
                drPREFILL_From.ItemArray = DataRow_From.ItemArray;

                dsPREFILL_MAIN.PREFILL_MAINRow drPREFILL_To = dtPREFILL_To.AsEnumerable().FirstOrDefault(obj => obj.NAME == drPREFILL_From.NAME && obj.IsDELETEDNull());
                if (drPREFILL_To != null)
                {
                    if (!isRow_Identical(drPREFILL_From, drPREFILL_To))
                        Log_Conflict(Common.GetHWID(), SyncType, drPREFILL_To.GUID, drPREFILL_From.GUID);
                    else
                        fullyIdentical = true;

                    return drPREFILL_To; //return this datarow to be added as deleted
                }
            }
            else if (SyncType == Sync_Type.PREFILL_REGISTER)
            {
                dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPREFILL_To = new dsPREFILL_REGISTER.PREFILL_REGISTERDataTable();
                dtPREFILL_To.Merge(DataTable_To);
                dsPREFILL_REGISTER dsPREFILLREGISTER = new dsPREFILL_REGISTER();
                dsPREFILL_REGISTER.PREFILL_REGISTERRow drPREFILL_From = dsPREFILLREGISTER.PREFILL_REGISTER.NewPREFILL_REGISTERRow();
                drPREFILL_From.ItemArray = DataRow_From.ItemArray;

                dsPREFILL_REGISTER.PREFILL_REGISTERRow drPREFILL_To;
                if(drPREFILL_From.IsWBS_GUIDNull())
                    drPREFILL_To = dtPREFILL_To.AsEnumerable().FirstOrDefault(obj => (!obj.IsTAG_GUIDNull() && (obj.TAG_GUID == drPREFILL_From.TAG_GUID))
                                                                            && obj.NAME == drPREFILL_From.NAME && obj.IsDELETEDNull());
                else
                    drPREFILL_To = dtPREFILL_To.AsEnumerable().FirstOrDefault(obj => (!obj.IsWBS_GUIDNull() && (obj.WBS_GUID == drPREFILL_From.WBS_GUID))
                                                                            && obj.NAME == drPREFILL_From.NAME && obj.IsDELETEDNull());


                if (drPREFILL_To != null)
                {
                    if (!isRow_Identical(drPREFILL_From, drPREFILL_To))
                        Log_Conflict(Common.GetHWID(), SyncType, drPREFILL_To.GUID, drPREFILL_From.GUID);
                    else
                        fullyIdentical = true;

                    return drPREFILL_To;
                }
            }
            else if (SyncType == Sync_Type.PROJECT)
            {
                dsPROJECT.PROJECTDataTable dtPROJECT_To = new dsPROJECT.PROJECTDataTable();
                dtPROJECT_To.Merge(DataTable_To);
                dsPROJECT dsPROJECTMAIN = new dsPROJECT();
                dsPROJECT.PROJECTRow drPROJECT_From = dsPROJECTMAIN.PROJECT.NewPROJECTRow();
                drPROJECT_From.ItemArray = DataRow_From.ItemArray;

                dsPROJECT.PROJECTRow drPROJECT_To = dtPROJECT_To.AsEnumerable().FirstOrDefault(obj => obj.NUMBER == drPROJECT_From.NUMBER && obj.IsDELETEDNull());
                if (drPROJECT_To != null)
                {
                    if (!isRow_Identical(drPROJECT_From, drPROJECT_To))
                        Log_Conflict(Common.GetHWID(), SyncType, drPROJECT_To.GUID, drPROJECT_From.GUID);
                    else
                        fullyIdentical = true;

                    return drPROJECT_To;
                }
            }
            else if (SyncType == Sync_Type.PUNCHLIST_MAIN)
            {
                dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_To = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
                dtPUNCHLIST_To.Merge(DataTable_To);
                dsPUNCHLIST_MAIN dsPUNCHLISTMAIN = new dsPUNCHLIST_MAIN();
                dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPUNCHLIST_From = dsPUNCHLISTMAIN.PUNCHLIST_MAIN.NewPUNCHLIST_MAINRow();
                drPUNCHLIST_From.ItemArray = DataRow_From.ItemArray;

                dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPUNCHLIST_To;
                if(drPUNCHLIST_From.IsWBS_GUIDNull())
                    drPUNCHLIST_To = dtPUNCHLIST_To.AsEnumerable().FirstOrDefault(obj => (!obj.IsTAG_GUIDNull() && (obj.TAG_GUID == drPUNCHLIST_From.TAG_GUID))                         
                                                                                        && obj.ITR_GUID == drPUNCHLIST_From.ITR_GUID && obj.ITR_PUNCHLIST_ITEM == drPUNCHLIST_From.ITR_PUNCHLIST_ITEM
                                                                                        && obj.IsDELETEDNull());
                else
                    drPUNCHLIST_To = dtPUNCHLIST_To.AsEnumerable().FirstOrDefault(obj => (!obj.IsWBS_GUIDNull() && (obj.WBS_GUID == drPUNCHLIST_From.WBS_GUID))
                                                                                        && obj.ITR_GUID == drPUNCHLIST_From.ITR_GUID && obj.ITR_PUNCHLIST_ITEM == drPUNCHLIST_From.ITR_PUNCHLIST_ITEM
                                                                                        && obj.IsDELETEDNull());

                if (drPUNCHLIST_To != null)
                {
                    if (!isRow_Identical(drPUNCHLIST_From, drPUNCHLIST_To))
                        Log_Conflict(Common.GetHWID(), SyncType, drPUNCHLIST_To.GUID, drPUNCHLIST_From.GUID);
                    else
                        fullyIdentical = true;

                    return drPUNCHLIST_To;
                }
            }
            else if (SyncType == Sync_Type.ROLE_MAIN)
            {
                dsROLE_MAIN.ROLE_MAINDataTable dtROLE_To = new dsROLE_MAIN.ROLE_MAINDataTable();
                dtROLE_To.Merge(DataTable_To);
                dsROLE_MAIN dsROLEMAIN = new dsROLE_MAIN();
                dsROLE_MAIN.ROLE_MAINRow drROLE_From = dsROLEMAIN.ROLE_MAIN.NewROLE_MAINRow();
                drROLE_From.ItemArray = DataRow_From.ItemArray;

                dsROLE_MAIN.ROLE_MAINRow drROLE_To = dtROLE_To.AsEnumerable().FirstOrDefault(obj => obj.NAME == drROLE_From.NAME && obj.IsDELETEDNull());
                if (drROLE_To != null)
                {
                    if (!isRow_Identical(drROLE_From, drROLE_To))
                        Log_Conflict(Common.GetHWID(), SyncType, drROLE_To.GUID, drROLE_From.GUID);
                    else
                        fullyIdentical = true;

                    return drROLE_To;
                }
            }
            else if (SyncType == Sync_Type.ROLE_PRIVILEGE)
            {
                dsROLE_MAIN.ROLE_PRIVILEGEDataTable dtROLE_PRIVILEGE_To = new dsROLE_MAIN.ROLE_PRIVILEGEDataTable();
                dtROLE_PRIVILEGE_To.Merge(DataTable_To);
                dsROLE_MAIN dsROLEMAIN = new dsROLE_MAIN();
                dsROLE_MAIN.ROLE_PRIVILEGERow drROLE_PRIVILEGE_From = dsROLEMAIN.ROLE_PRIVILEGE.NewROLE_PRIVILEGERow();
                drROLE_PRIVILEGE_From.ItemArray = DataRow_From.ItemArray;

                dsROLE_MAIN.ROLE_PRIVILEGERow drROLE_PRIVILEGE_To = dtROLE_PRIVILEGE_To.AsEnumerable().FirstOrDefault(obj => obj.ROLEGUID == drROLE_PRIVILEGE_From.ROLEGUID && obj.TYPEID == drROLE_PRIVILEGE_From.TYPEID
                                                                                                                        && obj.IsDELETEDNull());
                if (drROLE_PRIVILEGE_To != null)
                {
                    if (!isRow_Identical(drROLE_PRIVILEGE_From, drROLE_PRIVILEGE_To))
                        Log_Conflict(Common.GetHWID(), SyncType, drROLE_PRIVILEGE_To.GUID, drROLE_PRIVILEGE_From.GUID);
                    else
                        fullyIdentical = true;

                    return drROLE_PRIVILEGE_To;
                }
            }
            else if (SyncType == Sync_Type.SCHEDULE)
            {
                dsSCHEDULE.SCHEDULEDataTable dtSCHEDULE_To = new dsSCHEDULE.SCHEDULEDataTable();
                dtSCHEDULE_To.Merge(DataTable_To);
                dsSCHEDULE dsSCHEDULEMAIN = new dsSCHEDULE();
                dsSCHEDULE.SCHEDULERow drSCHEDULE_From = dsSCHEDULEMAIN.SCHEDULE.NewSCHEDULERow();
                drSCHEDULE_From.ItemArray = DataRow_From.ItemArray;

                dsSCHEDULE.SCHEDULERow drSCHEDULE_To = dtSCHEDULE_To.AsEnumerable().FirstOrDefault(obj => obj.NAME == drSCHEDULE_From.NAME && obj.IsDELETEDNull());
                if (drSCHEDULE_To != null)
                {
                    if (!isRow_Identical(drSCHEDULE_From, drSCHEDULE_To))
                        Log_Conflict(Common.GetHWID(), SyncType, drSCHEDULE_To.GUID, drSCHEDULE_From.GUID);
                    else
                        fullyIdentical = true;

                    return drSCHEDULE_To;
                }
            }
            else if (SyncType == Sync_Type.TAG)
            {
                dsTAG.TAGDataTable dtTAG_To = new dsTAG.TAGDataTable();
                dtTAG_To.Merge(DataTable_To);
                dsTAG dsTAGMAIN = new dsTAG();
                dsTAG.TAGRow drTAG_From = dsTAGMAIN.TAG.NewTAGRow();
                drTAG_From.ItemArray = DataRow_From.ItemArray;

                dsTAG.TAGRow drTAG_To = dtTAG_To.AsEnumerable().FirstOrDefault(obj => obj.SCHEDULEGUID == drTAG_From.SCHEDULEGUID && obj.NUMBER == drTAG_From.NUMBER
                                                                                && obj.IsDELETEDNull());
                if (drTAG_To != null)
                {
                    if (!isRow_Identical(drTAG_From, drTAG_To))
                        Log_Conflict(Common.GetHWID(), SyncType, drTAG_To.GUID, drTAG_From.GUID);
                    else
                        fullyIdentical = true;

                    return drTAG_To;
                }
            }
            else if (SyncType == Sync_Type.TEMPLATE_MAIN)
            {
                dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN_To = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
                dtTEMPLATE_MAIN_To.Merge(DataTable_To);
                dsTEMPLATE_MAIN dsTEMPLATEMAIN = new dsTEMPLATE_MAIN();
                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTEMPLATE_MAIN_From = dsTEMPLATEMAIN.TEMPLATE_MAIN.NewTEMPLATE_MAINRow();
                drTEMPLATE_MAIN_From.ItemArray = DataRow_From.ItemArray;

                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTEMPLATE_MAIN_To = dtTEMPLATE_MAIN_To.AsEnumerable().FirstOrDefault(obj => obj.NAME == drTEMPLATE_MAIN_From.NAME 
                                                                                                                        && obj.IsDELETEDNull());
                if (drTEMPLATE_MAIN_To != null)
                {
                    //Do not ignore updated because the Template content couldn't be compared
                    if (!isRow_Identical(drTEMPLATE_MAIN_From, drTEMPLATE_MAIN_To, false))
                        Log_Conflict(Common.GetHWID(), SyncType, drTEMPLATE_MAIN_To.GUID, drTEMPLATE_MAIN_From.GUID);
                    else
                        fullyIdentical = true;

                    return drTEMPLATE_MAIN_To;
                }
            }
            else if (SyncType == Sync_Type.TEMPLATE_REGISTER)
            {
                dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTEMPLATE_REGISTER_To = new dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable();
                dtTEMPLATE_REGISTER_To.Merge(DataTable_To);
                dsTEMPLATE_REGISTER dsTEMPLATEREGISTER = new dsTEMPLATE_REGISTER();
                dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTEMPLATE_REGISTER_From = dsTEMPLATEREGISTER.TEMPLATE_REGISTER.NewTEMPLATE_REGISTERRow();
                drTEMPLATE_REGISTER_From.ItemArray = DataRow_From.ItemArray;

                dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTEMPLATE_REGISTER_To;
                if(drTEMPLATE_REGISTER_From.IsWBS_GUIDNull())
                    drTEMPLATE_REGISTER_To = dtTEMPLATE_REGISTER_To.AsEnumerable().FirstOrDefault(obj => (!obj.IsTAG_GUIDNull() && (obj.TAG_GUID == drTEMPLATE_REGISTER_From.TAG_GUID))
                                                                                                            && obj.TEMPLATE_GUID == drTEMPLATE_REGISTER_From.TEMPLATE_GUID
                                                                                                            && obj.IsDELETEDNull());
                else
                    drTEMPLATE_REGISTER_To = dtTEMPLATE_REGISTER_To.AsEnumerable().FirstOrDefault(obj => (!obj.IsWBS_GUIDNull() && (obj.WBS_GUID == drTEMPLATE_REGISTER_From.WBS_GUID))
                                                                                                            && obj.TEMPLATE_GUID == drTEMPLATE_REGISTER_From.TEMPLATE_GUID
                                                                                                            && obj.IsDELETEDNull());

                if (drTEMPLATE_REGISTER_To != null)
                {
                    //Auto resolved, just have to return what was found in remote database
                    //Auto resolved because search criteria involves all relevant fields
                    fullyIdentical = true; //Use server copy
                    return drTEMPLATE_REGISTER_To;
                }
            }
            else if (SyncType == Sync_Type.TEMPLATE_TOGGLE)
            {
                dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATE_TOGGLE_To = new dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable();
                dtTEMPLATE_TOGGLE_To.Merge(DataTable_To);
                dsTEMPLATE_TOGGLE dsTEMPLATETOGGLE = new dsTEMPLATE_TOGGLE();
                dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLERow drTEMPLATE_TOGGLE_From = dsTEMPLATETOGGLE.TEMPLATE_TOGGLE.NewTEMPLATE_TOGGLERow();
                drTEMPLATE_TOGGLE_From.ItemArray = DataRow_From.ItemArray;

                dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLERow drTEMPLATE_TOGGLE_To = dtTEMPLATE_TOGGLE_To.AsEnumerable().FirstOrDefault(obj => obj.DISCIPLINE == drTEMPLATE_TOGGLE_From.DISCIPLINE && obj.NAME == drTEMPLATE_TOGGLE_From.NAME && obj.IsDELETEDNull());
                if (drTEMPLATE_TOGGLE_To != null)
                {
                    if (!isRow_Identical(drTEMPLATE_TOGGLE_From, drTEMPLATE_TOGGLE_To))
                        Log_Conflict(Common.GetHWID(), SyncType, drTEMPLATE_TOGGLE_To.GUID, drTEMPLATE_TOGGLE_From.GUID);
                    else
                        fullyIdentical = true;

                    return drTEMPLATE_TOGGLE_To;
                }
            }
            else if (SyncType == Sync_Type.MATRIX_TYPE)
            {
                dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMATRIX_TYPE_To = new dsMATRIX_TYPE.MATRIX_TYPEDataTable();
                dtMATRIX_TYPE_To.Merge(DataTable_To);
                dsMATRIX_TYPE dsTEMPLATETOGGLE = new dsMATRIX_TYPE();
                dsMATRIX_TYPE.MATRIX_TYPERow drMATRIX_TYPE_From = dsTEMPLATETOGGLE.MATRIX_TYPE.NewMATRIX_TYPERow();
                drMATRIX_TYPE_From.ItemArray = DataRow_From.ItemArray;

                dsMATRIX_TYPE.MATRIX_TYPERow drMATRIX_TYPE_To = dtMATRIX_TYPE_To.AsEnumerable().FirstOrDefault(obj => obj.NAME == drMATRIX_TYPE_From.NAME && obj.IsDELETEDNull());
                if (drMATRIX_TYPE_To != null)
                {
                    if (!isRow_Identical(drMATRIX_TYPE_From, drMATRIX_TYPE_To))
                        Log_Conflict(Common.GetHWID(), SyncType, drMATRIX_TYPE_To.GUID, drMATRIX_TYPE_From.GUID);
                    else
                        fullyIdentical = true;

                    return drMATRIX_TYPE_To;
                }
            }
            else if (SyncType == Sync_Type.MATRIX_ASSIGNMENT)
            {
                dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtMATRIX_ASSIGNMENT_To = new dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable();
                dtMATRIX_ASSIGNMENT_To.Merge(DataTable_To);
                dsMATRIX_ASSIGNMENT dsTEMPLATETOGGLE = new dsMATRIX_ASSIGNMENT();
                dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow drMATRIX_ASSIGNMENT_From = dsTEMPLATETOGGLE.MATRIX_ASSIGNMENT.NewMATRIX_ASSIGNMENTRow();
                drMATRIX_ASSIGNMENT_From.ItemArray = DataRow_From.ItemArray;

                dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow drMATRIX_ASSIGNMENT_To = dtMATRIX_ASSIGNMENT_To.AsEnumerable().FirstOrDefault(obj => obj.GUID_MATRIX_TYPE == drMATRIX_ASSIGNMENT_From.GUID_MATRIX_TYPE && obj.GUID_TEMPLATE == drMATRIX_ASSIGNMENT_From.GUID_TEMPLATE);
                if (drMATRIX_ASSIGNMENT_To != null)
                {
                    //Auto resolved, just have to return what was found in remote database
                    //Auto resolved because search criteria involves all relevant fields
                    fullyIdentical = true; //Use server copy
                    return drMATRIX_ASSIGNMENT_To;
                }
            }
            else if (SyncType == Sync_Type.USER_DISC)
            {
                dsUSER_MAIN.USER_DISCDataTable dtUSER_DISC_To = new dsUSER_MAIN.USER_DISCDataTable();
                dtUSER_DISC_To.Merge(DataTable_To);
                dsUSER_MAIN dsUSERMAIN = new dsUSER_MAIN();
                dsUSER_MAIN.USER_DISCRow drUSER_DISC_From = dsUSERMAIN.USER_DISC.NewUSER_DISCRow();
                drUSER_DISC_From.ItemArray = DataRow_From.ItemArray;

                dsUSER_MAIN.USER_DISCRow drUSER_DISC_To = dtUSER_DISC_To.AsEnumerable().FirstOrDefault(obj => obj.DISCIPLINE == drUSER_DISC_From.DISCIPLINE && obj.USERGUID == drUSER_DISC_From.USERGUID && obj.IsDELETEDNull());
                if (drUSER_DISC_To != null)
                {
                    //Auto resolved, just have to return what was found in remote database
                    //Auto resolved because search criteria involves all relevant fields
                    fullyIdentical = true; //Use server copy
                    return drUSER_DISC_To;
                }
            }
            else if (SyncType == Sync_Type.USER_PROJECT)
            {
                dsUSER_MAIN.USER_PROJECTDataTable dtUSER_PROJECT_To = new dsUSER_MAIN.USER_PROJECTDataTable();
                dtUSER_PROJECT_To.Merge(DataTable_To);
                dsUSER_MAIN dsUSERMAIN = new dsUSER_MAIN();
                dsUSER_MAIN.USER_PROJECTRow drUSER_PROJECT_From = dsUSERMAIN.USER_PROJECT.NewUSER_PROJECTRow();
                drUSER_PROJECT_From.ItemArray = DataRow_From.ItemArray;

                dsUSER_MAIN.USER_PROJECTRow drUSER_PROJECT_To = dtUSER_PROJECT_To.AsEnumerable().FirstOrDefault(obj => obj.USERGUID == drUSER_PROJECT_From.USERGUID 
                                                                                                                && obj.PROJECTGUID == drUSER_PROJECT_From.PROJECTGUID
                                                                                                                && obj.IsDELETEDNull());
                if (drUSER_PROJECT_To != null)
                {
                    //Auto resolved, just have to return what was found in remote database
                    //Auto resolved because search criteria involves all relevant fields
                    fullyIdentical = true; //Use server copy
                    return drUSER_PROJECT_To;
                }
            }
            else if (SyncType == Sync_Type.USER_MAIN)
            {
                dsUSER_MAIN.USER_MAINDataTable dtUSER_MAIN_To = new dsUSER_MAIN.USER_MAINDataTable();
                dtUSER_MAIN_To.Merge(DataTable_To);
                dsUSER_MAIN dsUSERMAIN = new dsUSER_MAIN();
                dsUSER_MAIN.USER_MAINRow drUSER_MAIN_From = dsUSERMAIN.USER_MAIN.NewUSER_MAINRow();
                drUSER_MAIN_From.ItemArray = DataRow_From.ItemArray;

                dsUSER_MAIN.USER_MAINRow drUSER_MAIN_To = dtUSER_MAIN_To.AsEnumerable().FirstOrDefault(obj => obj.QANUMBER == drUSER_MAIN_From.QANUMBER && obj.IsDELETEDNull());
                if (drUSER_MAIN_To != null)
                {
                    if (!isRow_Identical(drUSER_MAIN_From, drUSER_MAIN_To))
                        Log_Conflict(Common.GetHWID(), SyncType, drUSER_MAIN_To.GUID, drUSER_MAIN_From.GUID);
                    else
                        fullyIdentical = true;

                    return drUSER_MAIN_To;
                }
            }
            else if (SyncType == Sync_Type.WBS)
            {
                dsWBS.WBSDataTable dtWBS_To = new dsWBS.WBSDataTable();
                dtWBS_To.Merge(DataTable_To);
                dsWBS dsWBSMAIN = new dsWBS();
                dsWBS.WBSRow drWBS_From = dsWBSMAIN.WBS.NewWBSRow();
                drWBS_From.ItemArray = DataRow_From.ItemArray;

                dsWBS.WBSRow drWBS_To = dtWBS_To.AsEnumerable().FirstOrDefault(obj => obj.SCHEDULEGUID == drWBS_From.SCHEDULEGUID && obj.NAME == drWBS_From.NAME && obj.IsDELETEDNull());
                if (drWBS_To != null)
                {
                    if (!isRow_Identical(drWBS_From, drWBS_To))
                        Log_Conflict(Common.GetHWID(), SyncType, drWBS_To.GUID, drWBS_From.GUID);
                    else
                        fullyIdentical = true;

                    return drWBS_To;
                }
            }
            else if (SyncType == Sync_Type.WORKFLOW_MAIN)
            {
                dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWORKFLOW_MAIN_To = new dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable();
                dtWORKFLOW_MAIN_To.Merge(DataTable_To);
                dsWORKFLOW_MAIN dsWORKFLOWMAIN = new dsWORKFLOW_MAIN();
                dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWORKFLOW_MAIN_From = dsWORKFLOWMAIN.WORKFLOW_MAIN.NewWORKFLOW_MAINRow();
                drWORKFLOW_MAIN_From.ItemArray = DataRow_From.ItemArray;

                dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWORKFLOW_MAIN_To = dtWORKFLOW_MAIN_To.AsEnumerable().FirstOrDefault(obj => obj.NAME == drWORKFLOW_MAIN_From.NAME && obj.IsDELETEDNull());
                if (drWORKFLOW_MAIN_To != null)
                {
                    if (!isRow_Identical(drWORKFLOW_MAIN_From, drWORKFLOW_MAIN_To))
                        Log_Conflict(Common.GetHWID(), SyncType, drWORKFLOW_MAIN_To.GUID, drWORKFLOW_MAIN_From.GUID);
                    else
                        fullyIdentical = true;

                    return drWORKFLOW_MAIN_To;
                }
            }

            return null;
        }

        /// <summary>
        /// Log Conflict to Remote Server
        /// </summary>
        private void Log_Conflict(string HWID, Sync_Type SyncType, Guid ConflictOnGuid, Guid ConflictGuid)
        {
            dsSYNC_CONFLICT.SYNC_CONFLICTRow drNEW_SYNC_CONFLICT = dsSYNCCONFLICT.SYNC_CONFLICT.NewSYNC_CONFLICTRow();
            drNEW_SYNC_CONFLICT.GUID = Guid.NewGuid();
            drNEW_SYNC_CONFLICT.CONFLICT_HWID = HWID;
            drNEW_SYNC_CONFLICT.CONFLICT_TYPE = SyncType.ToString();
            drNEW_SYNC_CONFLICT.CONFLICT_GUID = ConflictGuid;
            drNEW_SYNC_CONFLICT.CONFLICT_ON_GUID = ConflictOnGuid;
            drNEW_SYNC_CONFLICT.CREATED = DateTime.Now;
            drNEW_SYNC_CONFLICT.CREATEDBY = System_Environment.GetUser().GUID;
            dsSYNCCONFLICT.SYNC_CONFLICT.AddSYNC_CONFLICTRow(drNEW_SYNC_CONFLICT);
            daREMOTE_SYNC_CONFLICT.Save(drNEW_SYNC_CONFLICT);
        }

        private void Save_DataTable(Database_Type DatabaseType, DataTable dtGENERIC, Sync_Type SyncType)
        {
            int TotalRecord = dtGENERIC.AsEnumerable().Count(obj => obj.RowState != DataRowState.Unchanged);
            _CurrentProgressEventArg.progress_type = SyncType;
            _CurrentProgressEventArg.progress_step = (100.0 / TotalRecord);

            ProcessThreadEventArgs Overall_ProgressEventArg = new ProcessThreadEventArgs();
            Overall_ProgressEventArg.progress_type = SyncType;
            Overall_ProgressEventArg.progress_step = (15.0 / TotalRecord);

            if(DatabaseType == Database_Type.LOCAL)
            {
                Overall_ProgressEventArg.progress_status = Common.Replace_WithSpaces(SyncStatus_Display.Downloading_Template.ToString());
                _CurrentProgressEventArg.progress_status = Common.Replace_WithSpaces(SyncStatus_Display.Saving_Locally.ToString());
                //These Lines Cause Double ups on counting during adapter update
                //Overall_ProgressEventArg.progress_countType = SyncStatus_Count.Download;
                //_CurrentProgressEventArg.progress_countType = SyncStatus_Count.Download;
            }
            else
            {
                Overall_ProgressEventArg.progress_status = Common.Replace_WithSpaces(SyncStatus_Display.Uploading_Template.ToString());
                _CurrentProgressEventArg.progress_status = Common.Replace_WithSpaces(SyncStatus_Display.Saving_Remotely.ToString());
                //Overall_ProgressEventArg.progress_countType = SyncStatus_Count.Upload;
                //_CurrentProgressEventArg.progress_countType = SyncStatus_Count.Upload;
            }

            //reset the count
            if(SyncType == Sync_Type.ITR_MAIN || SyncType == Sync_Type.TEMPLATE_MAIN)
            {
                Overall_ProgressEventArg.count_reset = true;
                ProcessProgress_Overall(this, Overall_ProgressEventArg);
                //Overall_ProgressEventArg.progress_countType = SyncStatus_Count.None; //Do not count from here because we only used this for resetting the count
            }

            switch (SyncType)
            {
                case Sync_Type.GENERAL_EQUIPMENT:
                    using (dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtEQUIPMENT = new dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable())
                    {
                        dtEQUIPMENT.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_EQUIPMENT.Save(dtEQUIPMENT);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_EQUIPMENT.Save(dtEQUIPMENT);
                    }

                    break;
                case Sync_Type.ITR_MAIN:
                    using (dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable())
                    {
                        dtITR_MAIN.Merge(dtGENERIC);
                        dsITR_MAIN.ITR_MAINDataTable dtCLONE_ITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
                        dtCLONE_ITR_MAIN.Merge(dtITR_MAIN);
                        AdapterITR_MAIN daRETRIEVE_ITR;

                        if (DatabaseType == Database_Type.LOCAL)
                            daRETRIEVE_ITR = daREMOTE_ITR_MAIN; //if sync context is local, it means that modified/added row should be updated by remote record
                        else
                            daRETRIEVE_ITR = daLOCAL_ITR_MAIN; //if sync context is remote, it means that modified/added row should be updated by local record

                        _CurrentProgressEventArg.progress_step = (100.0 / TotalRecord);
                        Overall_ProgressEventArg.progress_step = (10.0 / TotalRecord);

                        foreach (dsITR_MAIN.ITR_MAINRow drITR_MAIN in dtITR_MAIN.Rows)
                        {
                            if (drITR_MAIN.RowState == DataRowState.Modified || drITR_MAIN.RowState == DataRowState.Added)
                            {
                                dsITR_MAIN.ITR_MAINRow drCLONE_ITR_MAIN = dtCLONE_ITR_MAIN.NewITR_MAINRow();
                                //must get ITR with it's schema instead of generic because generic ITR will produce gibberrish
                                dsITR_MAIN.ITR_MAINRow drRETRIEVE_ITR_MAIN = daRETRIEVE_ITR.GetIncludeDeletedBy(drITR_MAIN.GUID);
                                if (drRETRIEVE_ITR_MAIN != null)
                                {
                                    dsITR_MAIN.ITR_MAINRow drFIND_ITR_MAIN = dtCLONE_ITR_MAIN.AsEnumerable().First(obj => obj.GUID == drITR_MAIN.GUID);
                                    int i = dtCLONE_ITR_MAIN.Rows.IndexOf(drFIND_ITR_MAIN);
                                    dtCLONE_ITR_MAIN.Rows[i].ItemArray = drRETRIEVE_ITR_MAIN.ItemArray;
                                    ProcessProgress_Current(this, _CurrentProgressEventArg);
                                    ProcessProgress_Overall(this, Overall_ProgressEventArg);
                                    if (dtCLONE_ITR_MAIN.Rows[i].RowState == DataRowState.Unchanged)
                                    {
                                        if (drITR_MAIN.RowState == DataRowState.Modified)
                                            //DataRowState.Added will get cloned over, so there is no need to handle it
                                            dtCLONE_ITR_MAIN.Rows[i].SetModified();
                                        else if (drITR_MAIN.RowState == DataRowState.Added)
                                            dtCLONE_ITR_MAIN.Rows[i].SetAdded();
                                    }
                                }
                                else
                                    Common.Warn("Error Retrieving ITR, Please Contact Administrator");
                            }
                        }

                        //the count type is used to determine reset type
                        if (DatabaseType == Database_Type.LOCAL)
                        {
                            //Overall_ProgressEventArg.progress_countType = SyncStatus_Count.Download;
                            Overall_ProgressEventArg.progress_status = Common.Replace_WithSpaces(SyncStatus_Display.Saving_Locally.ToString());
                        }
                        else
                        {
                            //Overall_ProgressEventArg.progress_countType = SyncStatus_Count.Upload;
                            Overall_ProgressEventArg.progress_status = Common.Replace_WithSpaces(SyncStatus_Display.Saving_Remotely.ToString());
                        }

                        Overall_ProgressEventArg.progress_step = 0.0;
                        Overall_ProgressEventArg.count_reset = true;
                        ProcessProgress_Overall(this, Overall_ProgressEventArg);

                        //Override the saving progress because it only accounts for 5% in this routine
                        _CurrentProgressEventArg.progress_reset = true;
                        ProcessProgress_Current(this, _CurrentProgressEventArg);
                        _CurrentProgressEventArg.progress_step = (100.0 / TotalRecord);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_ITR_MAIN.Save(dtCLONE_ITR_MAIN);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_ITR_MAIN.Save(dtCLONE_ITR_MAIN);

                        Overall_ProgressEventArg.progress_step = 5.0;
                        ProcessProgress_Overall(this, Overall_ProgressEventArg);
                    }
                    break;
                case Sync_Type.ITR_STATUS:
                    using (dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS = new dsITR_STATUS.ITR_STATUSDataTable())
                    {
                        dtITR_STATUS.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_ITR_STATUS.Save(dtITR_STATUS);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_ITR_STATUS.Save(dtITR_STATUS);
                    }
                    break;
                case Sync_Type.ITR_STATUS_ISSUE:
                    using (dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtITR_STATUS_ISSUE = new dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable())
                    {
                        dtITR_STATUS_ISSUE.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_ITR_STATUS_ISSUE.Save(dtITR_STATUS_ISSUE);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_ITR_STATUS_ISSUE.Save(dtITR_STATUS_ISSUE);
                    }
                    break;
                case Sync_Type.CERTIFICATE_STATUS:
                    using (dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable())
                    {
                        dtCERTIFICATE_STATUS.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_CERTIFICATE_STATUS.Save(dtCERTIFICATE_STATUS);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_CERTIFICATE_STATUS.Save(dtCERTIFICATE_STATUS);
                    }
                    break;
                case Sync_Type.CERTIFICATE_STATUS_ISSUE:
                    using (dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable dtCERTIFICATE_STATUS_ISSUE = new dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable())
                    {
                        dtCERTIFICATE_STATUS_ISSUE.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_CERTIFICATE_STATUS_ISSUE.Save(dtCERTIFICATE_STATUS_ISSUE);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_CERTIFICATE_STATUS_ISSUE.Save(dtCERTIFICATE_STATUS_ISSUE);
                    }
                    break;
                case Sync_Type.PREFILL_MAIN:
                    using (dsPREFILL_MAIN.PREFILL_MAINDataTable dtPREFILL_MAIN = new dsPREFILL_MAIN.PREFILL_MAINDataTable())
                    {
                        dtPREFILL_MAIN.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_PREFILL_MAIN.Save(dtPREFILL_MAIN);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_PREFILL_MAIN.Save(dtPREFILL_MAIN);
                    }
                    break;
                case Sync_Type.PREFILL_REGISTER:
                    using (dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPREFILL_REGISTER = new dsPREFILL_REGISTER.PREFILL_REGISTERDataTable())
                    {
                        dtPREFILL_REGISTER.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                        {
                            daLOCAL_PREFILL_REGISTER.Save(dtPREFILL_REGISTER);
                        } 
                        else if (DatabaseType == Database_Type.REMOTE)
                        {
                            daREMOTE_PREFILL_REGISTER.Save(dtPREFILL_REGISTER);
                        }
                    }
                    break;
                case Sync_Type.PROJECT:
                    using (dsPROJECT.PROJECTDataTable dtPROJECT = new dsPROJECT.PROJECTDataTable())
                    {
                        dtPROJECT.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_PROJECT.Save(dtPROJECT);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_PROJECT.Save(dtPROJECT);
                    }
                    break;
                case Sync_Type.PUNCHLIST_MAIN:
                    using (dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_MAIN = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable())
                    {
                        dtPUNCHLIST_MAIN.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_PUNCHLIST_MAIN.Save(dtPUNCHLIST_MAIN);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_PUNCHLIST_MAIN.Save(dtPUNCHLIST_MAIN);
                    }
                    break;
                case Sync_Type.PUNCHLIST_STATUS:
                    using (dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPUNCHLIST_STATUS = new dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable())
                    {
                        dtPUNCHLIST_STATUS.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_PUNCHLIST_STATUS.Save(dtPUNCHLIST_STATUS);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_PUNCHLIST_STATUS.Save(dtPUNCHLIST_STATUS);
                    }
                    break;
                case Sync_Type.PUNCHLIST_STATUS_ISSUE:
                    using (dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable dtPUNCHLIST_STATUS_ISSUE = new dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable())
                    {
                        dtPUNCHLIST_STATUS_ISSUE.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_PUNCHLIST_STATUS_ISSUE.Save(dtPUNCHLIST_STATUS_ISSUE);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_PUNCHLIST_STATUS_ISSUE.Save(dtPUNCHLIST_STATUS_ISSUE);
                    }
                    break;
                case Sync_Type.ROLE_MAIN:
                    using (dsROLE_MAIN.ROLE_MAINDataTable dtROLE_MAIN = new dsROLE_MAIN.ROLE_MAINDataTable())
                    {
                        dtROLE_MAIN.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_ROLE_MAIN.Save(dtROLE_MAIN);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_ROLE_MAIN.Save(dtROLE_MAIN);
                    }
                    break;
                case Sync_Type.ROLE_PRIVILEGE:
                    using (dsROLE_MAIN.ROLE_PRIVILEGEDataTable dtROLE_PRIVILEGE = new dsROLE_MAIN.ROLE_PRIVILEGEDataTable())
                    {
                        dtROLE_PRIVILEGE.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_ROLE_MAIN.Save(dtROLE_PRIVILEGE);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_ROLE_MAIN.Save(dtROLE_PRIVILEGE);
                    }
                    break;
                case Sync_Type.SCHEDULE:
                    using (dsSCHEDULE.SCHEDULEDataTable dtSCHEDULE = new dsSCHEDULE.SCHEDULEDataTable())
                    {
                        dtSCHEDULE.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_SCHEDULE.Save(dtSCHEDULE);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_SCHEDULE.Save(dtSCHEDULE);
                    }
                    break;
                case Sync_Type.TAG:
                    using (dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable())
                    {
                        dtTAG.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_TAG.Save(dtTAG);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_TAG.Save(dtTAG);
                    }
                    break;
                case Sync_Type.TEMPLATE_MAIN:
                    using (dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable())
                    {
                        dtTEMPLATE_MAIN.Merge(dtGENERIC);
                        dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtCLONE_TEMPLATE_MAIN = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
                        dtCLONE_TEMPLATE_MAIN.Merge(dtTEMPLATE_MAIN);
                        AdapterTEMPLATE_MAIN daRETRIEVE_ITR;

                        if (DatabaseType == Database_Type.LOCAL)
                            daRETRIEVE_ITR = daREMOTE_TEMPLATE_MAIN; //if sync context is local, it means that modified/added row should be updated by remote record
                        else
                            daRETRIEVE_ITR = daLOCAL_TEMPLATE_MAIN; //if sync context is remote, it means that modified/added row should be updated by local record

                        _CurrentProgressEventArg.progress_step = (100.0 / TotalRecord);
                        Overall_ProgressEventArg.progress_step = (10.0 / TotalRecord);

                        foreach (dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTEMPLATE_MAIN in dtTEMPLATE_MAIN.Rows)
                        {
                            if (drTEMPLATE_MAIN.RowState == DataRowState.Modified || drTEMPLATE_MAIN.RowState == DataRowState.Added)
                            {
                                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drCLONE_TEMPLATE_MAIN = dtCLONE_TEMPLATE_MAIN.NewTEMPLATE_MAINRow();
                                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drRETRIEVE_TEMPLATE_MAIN = daRETRIEVE_ITR.GetIncludeDeletedBy(drTEMPLATE_MAIN.GUID);
                                if (drRETRIEVE_TEMPLATE_MAIN != null)
                                {
                                    dsTEMPLATE_MAIN.TEMPLATE_MAINRow drFIND_TEMPLATE_MAIN = dtCLONE_TEMPLATE_MAIN.AsEnumerable().First(obj => obj.GUID == drTEMPLATE_MAIN.GUID);
                                    int i = dtCLONE_TEMPLATE_MAIN.Rows.IndexOf(drFIND_TEMPLATE_MAIN);
                                    dtCLONE_TEMPLATE_MAIN.Rows[i].ItemArray = drRETRIEVE_TEMPLATE_MAIN.ItemArray;
                                    ProcessProgress_Current(this, _CurrentProgressEventArg);
                                    ProcessProgress_Overall(this, Overall_ProgressEventArg);
                                    if (dtCLONE_TEMPLATE_MAIN.Rows[i].RowState == DataRowState.Unchanged)
                                    {
                                        if (drTEMPLATE_MAIN.RowState == DataRowState.Modified)
                                            //DataRowState.Added will get cloned over, so there is no need to handle it
                                            dtCLONE_TEMPLATE_MAIN.Rows[i].SetModified();
                                        else if (drTEMPLATE_MAIN.RowState == DataRowState.Added)
                                            dtCLONE_TEMPLATE_MAIN.Rows[i].SetAdded();
                                    }
                                }
                                else
                                    Common.Warn("Error Retrieving Template, Please Contact Administrator");
                            }
                        }

                        //the count type is used to determine reset type
                        if (DatabaseType == Database_Type.LOCAL)
                        {
                            //Overall_ProgressEventArg.progress_countType = SyncStatus_Count.Download;
                            Overall_ProgressEventArg.progress_status = Common.Replace_WithSpaces(SyncStatus_Display.Saving_Locally.ToString());
                        }
                        else
                        {
                            //Overall_ProgressEventArg.progress_countType = SyncStatus_Count.Upload;
                            Overall_ProgressEventArg.progress_status = Common.Replace_WithSpaces(SyncStatus_Display.Saving_Remotely.ToString());
                        }

                        Overall_ProgressEventArg.progress_step = 0.0;
                        Overall_ProgressEventArg.count_reset = true;
                        ProcessProgress_Overall(this, Overall_ProgressEventArg);

                        _CurrentProgressEventArg.progress_reset = true;
                        ProcessProgress_Current(this, _CurrentProgressEventArg);
                        //Override the saving progress because it only accounts for 5% in this routine
                        _CurrentProgressEventArg.progress_step = (100.0 / TotalRecord);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_TEMPLATE_MAIN.Save(dtCLONE_TEMPLATE_MAIN);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_TEMPLATE_MAIN.Save(dtCLONE_TEMPLATE_MAIN);

                        Overall_ProgressEventArg.progress_step = 5.0;
                        ProcessProgress_Overall(this, Overall_ProgressEventArg);
                    }
                    break;
                case Sync_Type.CERTIFICATE_MAIN:
                    using (dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable dtCERTIFICATE_MAIN = new dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable())
                    {
                        dtCERTIFICATE_MAIN.Merge(dtGENERIC);
                        dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable dtCLONE_CERTIFICATE_MAIN = new dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable();
                        dtCLONE_CERTIFICATE_MAIN.Merge(dtCERTIFICATE_MAIN);
                        AdapterCERTIFICATE_MAIN daRETRIEVE_ITR;

                        if (DatabaseType == Database_Type.LOCAL)
                            daRETRIEVE_ITR = daREMOTE_CERTIFICATE_MAIN; //if sync context is local, it means that modified/added row should be updated by remote record
                        else
                            daRETRIEVE_ITR = daLOCAL_CERTIFICATE_MAIN; //if sync context is remote, it means that modified/added row should be updated by local record

                        _CurrentProgressEventArg.progress_step = (100.0 / TotalRecord);
                        Overall_ProgressEventArg.progress_step = (10.0 / TotalRecord);

                        foreach (dsCERTIFICATE_MAIN.CERTIFICATE_MAINRow drCERTIFICATE_MAIN in dtCERTIFICATE_MAIN.Rows)
                        {
                            if (drCERTIFICATE_MAIN.RowState == DataRowState.Modified || drCERTIFICATE_MAIN.RowState == DataRowState.Added)
                            {
                                dsCERTIFICATE_MAIN.CERTIFICATE_MAINRow drCLONE_CERTIFICATE_MAIN = dtCLONE_CERTIFICATE_MAIN.NewCERTIFICATE_MAINRow();
                                dsCERTIFICATE_MAIN.CERTIFICATE_MAINRow drRETRIEVE_CERTIFICATE_MAIN = daRETRIEVE_ITR.GetIncludeDeletedBy(drCERTIFICATE_MAIN.GUID);
                                if (drRETRIEVE_CERTIFICATE_MAIN != null)
                                {
                                    dsCERTIFICATE_MAIN.CERTIFICATE_MAINRow drFIND_CERTIFICATE_MAIN = dtCLONE_CERTIFICATE_MAIN.AsEnumerable().First(obj => obj.GUID == drCERTIFICATE_MAIN.GUID);
                                    int i = dtCLONE_CERTIFICATE_MAIN.Rows.IndexOf(drFIND_CERTIFICATE_MAIN);
                                    dtCLONE_CERTIFICATE_MAIN.Rows[i].ItemArray = drRETRIEVE_CERTIFICATE_MAIN.ItemArray;
                                    ProcessProgress_Current(this, _CurrentProgressEventArg);
                                    ProcessProgress_Overall(this, Overall_ProgressEventArg);
                                    if (dtCLONE_CERTIFICATE_MAIN.Rows[i].RowState == DataRowState.Unchanged)
                                    {
                                        if (drCERTIFICATE_MAIN.RowState == DataRowState.Modified)
                                            //DataRowState.Added will get cloned over, so there is no need to handle it
                                            dtCLONE_CERTIFICATE_MAIN.Rows[i].SetModified();
                                        else if (drCERTIFICATE_MAIN.RowState == DataRowState.Added)
                                            dtCLONE_CERTIFICATE_MAIN.Rows[i].SetAdded();
                                    }
                                }
                                else
                                    Common.Warn("Error Retrieving Certificate, Please Contact Administrator");
                            }
                        }

                        //the count type is used to determine reset type
                        if (DatabaseType == Database_Type.LOCAL)
                        {
                            //Overall_ProgressEventArg.progress_countType = SyncStatus_Count.Download;
                            Overall_ProgressEventArg.progress_status = Common.Replace_WithSpaces(SyncStatus_Display.Saving_Locally.ToString());
                        }
                        else
                        {
                            //Overall_ProgressEventArg.progress_countType = SyncStatus_Count.Upload;
                            Overall_ProgressEventArg.progress_status = Common.Replace_WithSpaces(SyncStatus_Display.Saving_Remotely.ToString());
                        }

                        Overall_ProgressEventArg.progress_step = 0.0;
                        Overall_ProgressEventArg.count_reset = true;
                        ProcessProgress_Overall(this, Overall_ProgressEventArg);

                        _CurrentProgressEventArg.progress_reset = true;
                        ProcessProgress_Current(this, _CurrentProgressEventArg);
                        //Override the saving progress because it only accounts for 5% in this routine
                        _CurrentProgressEventArg.progress_step = (100.0 / TotalRecord);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_CERTIFICATE_MAIN.Save(dtCLONE_CERTIFICATE_MAIN);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_CERTIFICATE_MAIN.Save(dtCLONE_CERTIFICATE_MAIN);

                        Overall_ProgressEventArg.progress_step = 5.0;
                        ProcessProgress_Overall(this, Overall_ProgressEventArg);
                    }
                    break;
                case Sync_Type.CERTIFICATE_DATA:
                    using (dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable dtCERTIFICATE_DATA = new dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable())
                    {
                        dtCERTIFICATE_DATA.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_CERTIFICATE_DATA.Save(dtCERTIFICATE_DATA);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_CERTIFICATE_DATA.Save(dtCERTIFICATE_DATA);
                    }
                    break;
                case Sync_Type.TEMPLATE_REGISTER:
                    using (dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTEMPLATE_REGISTER = new dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable())
                    {
                        dtTEMPLATE_REGISTER.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_TEMPLATE_REGISTER.Save(dtTEMPLATE_REGISTER);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_TEMPLATE_REGISTER.Save(dtTEMPLATE_REGISTER);
                    }
                    break;
                case Sync_Type.TEMPLATE_TOGGLE:
                    using (dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATE_TOGGLE = new dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable())
                    {
                        dtTEMPLATE_TOGGLE.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_TEMPLATE_TOGGLE.Save(dtTEMPLATE_TOGGLE);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_TEMPLATE_TOGGLE.Save(dtTEMPLATE_TOGGLE);
                    }
                    break;
                case Sync_Type.MATRIX_TYPE:
                    using (dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMATRIX_TYPE = new dsMATRIX_TYPE.MATRIX_TYPEDataTable())
                    {
                        dtMATRIX_TYPE.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_MATRIX_TYPE.Save(dtMATRIX_TYPE);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_MATRIX_TYPE.Save(dtMATRIX_TYPE);
                    }
                    break;
                case Sync_Type.MATRIX_ASSIGNMENT:
                    using (dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtMATRIX_ASSIGNMENT = new dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable())
                    {
                        dtMATRIX_ASSIGNMENT.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_MATRIX_ASSIGNMENT.Save(dtMATRIX_ASSIGNMENT);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_MATRIX_ASSIGNMENT.Save(dtMATRIX_ASSIGNMENT);
                    }
                    break;
                case Sync_Type.USER_DISC:
                    using (dsUSER_MAIN.USER_DISCDataTable dtUSER_DISC = new dsUSER_MAIN.USER_DISCDataTable())
                    {
                        dtUSER_DISC.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_USER_MAIN.Save(dtUSER_DISC);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_USER_MAIN.Save(dtUSER_DISC);
                    }
                    break;
                case Sync_Type.USER_MAIN:
                    using (dsUSER_MAIN.USER_MAINDataTable dtUSER_MAIN = new dsUSER_MAIN.USER_MAINDataTable())
                    {
                        dtUSER_MAIN.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_USER_MAIN.Save(dtUSER_MAIN);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_USER_MAIN.Save(dtUSER_MAIN);
                    }
                    break;
                case Sync_Type.USER_PROJECT:
                    using (dsUSER_MAIN.USER_PROJECTDataTable dtUSER_PROJECT = new dsUSER_MAIN.USER_PROJECTDataTable())
                    {
                        dtUSER_PROJECT.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_USER_MAIN.Save(dtUSER_PROJECT);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_USER_MAIN.Save(dtUSER_PROJECT);
                    }
                    break;
                case Sync_Type.WBS:
                    using (dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable())
                    {
                        dtWBS.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_WBS.Save(dtWBS);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_WBS.Save(dtWBS);
                    }
                    break;
                case Sync_Type.WORKFLOW_MAIN:
                    using (dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWORKFLOW = new dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable())
                    {
                        dtWORKFLOW.Merge(dtGENERIC);
                        if (DatabaseType == Database_Type.LOCAL)
                            daLOCAL_WORKFLOW.Save(dtWORKFLOW);
                        else if (DatabaseType == Database_Type.REMOTE)
                            daREMOTE_WORKFLOW.Save(dtWORKFLOW);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Starts this process as a thread
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public object Start(object state)
        {
            //do if statement instead of foreach for Sync_Database to ensure that sync is performed in this sequence
            if (Sync_Database.Any(obj => obj == Sync_Type.PROJECT.ToString()))
            {
                daLOCAL_PROJECT = new AdapterPROJECT();
                daREMOTE_PROJECT = new AdapterPROJECT(remoteConnStr);
                daLOCAL_PROJECT.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_PROJECT.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.PROJECT);
                daLOCAL_PROJECT.Dispose();
                daREMOTE_PROJECT.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.GENERAL_EQUIPMENT.ToString()))
            {
                daLOCAL_EQUIPMENT = new AdapterGENERAL_EQUIPMENT();
                daREMOTE_EQUIPMENT = new AdapterGENERAL_EQUIPMENT(remoteConnStr);
                daLOCAL_EQUIPMENT.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_EQUIPMENT.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.GENERAL_EQUIPMENT);
                daLOCAL_EQUIPMENT.Dispose();
                daREMOTE_EQUIPMENT.Dispose();
            }
            if(Sync_Database.Any(obj => obj == Sync_Type.WORKFLOW_MAIN.ToString()))
            {
                daLOCAL_WORKFLOW = new AdapterWORKFLOW_MAIN();
                daREMOTE_WORKFLOW = new AdapterWORKFLOW_MAIN(remoteConnStr);
                daLOCAL_WORKFLOW.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_WORKFLOW.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.WORKFLOW_MAIN);
                daLOCAL_WORKFLOW.Dispose();
                daREMOTE_WORKFLOW.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.TEMPLATE_MAIN.ToString()))
            {
                daLOCAL_TEMPLATE_MAIN = new AdapterTEMPLATE_MAIN();
                daREMOTE_TEMPLATE_MAIN = new AdapterTEMPLATE_MAIN(remoteConnStr);
                daLOCAL_TEMPLATE_MAIN.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_TEMPLATE_MAIN.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.TEMPLATE_MAIN);
                daLOCAL_TEMPLATE_MAIN.Dispose();
                daREMOTE_TEMPLATE_MAIN.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.CERTIFICATE_MAIN.ToString()))
            {
                daLOCAL_CERTIFICATE_MAIN = new AdapterCERTIFICATE_MAIN();
                daREMOTE_CERTIFICATE_MAIN = new AdapterCERTIFICATE_MAIN(remoteConnStr);
                daLOCAL_CERTIFICATE_MAIN.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_CERTIFICATE_MAIN.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.CERTIFICATE_MAIN);
                daLOCAL_CERTIFICATE_MAIN.Dispose();
                daREMOTE_CERTIFICATE_MAIN.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.CERTIFICATE_DATA.ToString()))
            {
                daLOCAL_CERTIFICATE_DATA = new AdapterCERTIFICATE_DATA();
                daREMOTE_CERTIFICATE_DATA = new AdapterCERTIFICATE_DATA(remoteConnStr);
                daLOCAL_CERTIFICATE_DATA.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_CERTIFICATE_DATA.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.CERTIFICATE_DATA);
                daLOCAL_CERTIFICATE_DATA.Dispose();
                daREMOTE_CERTIFICATE_DATA.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.TEMPLATE_TOGGLE.ToString()))
            {
                daLOCAL_TEMPLATE_TOGGLE = new AdapterTEMPLATE_TOGGLE();
                daREMOTE_TEMPLATE_TOGGLE = new AdapterTEMPLATE_TOGGLE(remoteConnStr);
                daLOCAL_TEMPLATE_TOGGLE.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_TEMPLATE_TOGGLE.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.TEMPLATE_TOGGLE);
                daLOCAL_TEMPLATE_TOGGLE.Dispose();
                daREMOTE_TEMPLATE_TOGGLE.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.MATRIX_TYPE.ToString()))
            {
                daLOCAL_MATRIX_TYPE = new AdapterMATRIX_TYPE();
                daREMOTE_MATRIX_TYPE = new AdapterMATRIX_TYPE(remoteConnStr);
                daLOCAL_MATRIX_TYPE.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_MATRIX_TYPE.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.MATRIX_TYPE);
                daLOCAL_MATRIX_TYPE.Dispose();
                daREMOTE_MATRIX_TYPE.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.MATRIX_ASSIGNMENT.ToString()))
            {
                daLOCAL_MATRIX_ASSIGNMENT = new AdapterMATRIX_ASSIGNMENT();
                daREMOTE_MATRIX_ASSIGNMENT = new AdapterMATRIX_ASSIGNMENT(remoteConnStr);
                daLOCAL_MATRIX_ASSIGNMENT.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_MATRIX_ASSIGNMENT.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.MATRIX_ASSIGNMENT);
                daLOCAL_MATRIX_ASSIGNMENT.Dispose();
                daREMOTE_MATRIX_ASSIGNMENT.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.PREFILL_MAIN.ToString()))
            {
                daLOCAL_PREFILL_MAIN = new AdapterPREFILL_MAIN();
                daREMOTE_PREFILL_MAIN = new AdapterPREFILL_MAIN(remoteConnStr);
                daLOCAL_PREFILL_MAIN.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_PREFILL_MAIN.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.PREFILL_MAIN);
                daLOCAL_PREFILL_MAIN.Dispose();
                daREMOTE_PREFILL_MAIN.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.SCHEDULE.ToString()))
            {
                daLOCAL_SCHEDULE = new AdapterSCHEDULE();
                daREMOTE_SCHEDULE = new AdapterSCHEDULE(remoteConnStr);
                daLOCAL_SCHEDULE.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_SCHEDULE.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.SCHEDULE);
                daLOCAL_SCHEDULE.Dispose();
                daREMOTE_SCHEDULE.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.WBS.ToString()))
            {
                daLOCAL_WBS = new AdapterWBS();
                daREMOTE_WBS = new AdapterWBS(remoteConnStr);
                daLOCAL_WBS.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_WBS.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.WBS);
                daLOCAL_WBS.Dispose();
                daREMOTE_WBS.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.TAG.ToString()))
            {
                daLOCAL_TAG = new AdapterTAG();
                daREMOTE_TAG = new AdapterTAG(remoteConnStr);
                daLOCAL_TAG.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_TAG.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.TAG);
                daLOCAL_TAG.Dispose();
                daREMOTE_TAG.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.TEMPLATE_REGISTER.ToString()))
            {
                daLOCAL_TEMPLATE_REGISTER = new AdapterTEMPLATE_REGISTER();
                daREMOTE_TEMPLATE_REGISTER = new AdapterTEMPLATE_REGISTER(remoteConnStr);
                daLOCAL_TEMPLATE_REGISTER.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_TEMPLATE_REGISTER.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.TEMPLATE_REGISTER);
                daLOCAL_TEMPLATE_REGISTER.Dispose();
                daREMOTE_TEMPLATE_REGISTER.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.PREFILL_REGISTER.ToString()))
            {
                daLOCAL_PREFILL_REGISTER = new AdapterPREFILL_REGISTER();
                daREMOTE_PREFILL_REGISTER = new AdapterPREFILL_REGISTER(remoteConnStr);
                daLOCAL_PREFILL_REGISTER.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_PREFILL_REGISTER.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.PREFILL_REGISTER);
                daLOCAL_PREFILL_REGISTER.Dispose();
                daREMOTE_PREFILL_REGISTER.Dispose();
            }
            //ROLE_PRIVILEGE must always be synced in tandem (due to common adapter) with ROLE_MAIN so don't need to add ROLE_PRIVILEGE in Sync_Database
            if (Sync_Database.Any(obj => obj == Sync_Type.ROLE_MAIN.ToString()))
            {
                daLOCAL_ROLE_MAIN = new AdapterROLE_MAIN();
                daREMOTE_ROLE_MAIN = new AdapterROLE_MAIN(remoteConnStr);
                daLOCAL_ROLE_MAIN.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_ROLE_MAIN.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.ROLE_MAIN);
                AdvanceStep(Sync_Type.ROLE_PRIVILEGE);
                daLOCAL_ROLE_MAIN.Dispose();
                daREMOTE_ROLE_MAIN.Dispose();
            }
            //USER_PROJECT and USER_DISC must always be synced in tandem (due to common adapter) with USER_MAIN so there is no need to add USER_PROJECT and USER_DISC in Sync_Database
            if (Sync_Database.Any(obj => obj == Sync_Type.USER_MAIN.ToString()))
            {
                daLOCAL_USER_MAIN = new AdapterUSER_MAIN();
                daREMOTE_USER_MAIN = new AdapterUSER_MAIN(remoteConnStr);
                daLOCAL_USER_MAIN.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_USER_MAIN.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.USER_MAIN);
                AdvanceStep(Sync_Type.USER_PROJECT);
                AdvanceStep(Sync_Type.USER_DISC);
                daLOCAL_USER_MAIN.Dispose();
                daREMOTE_USER_MAIN.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.ITR_MAIN.ToString()))
            {
                daLOCAL_ITR_MAIN = new AdapterITR_MAIN();
                daREMOTE_ITR_MAIN = new AdapterITR_MAIN(remoteConnStr);
                daLOCAL_ITR_MAIN.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_ITR_MAIN.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.ITR_MAIN);
                daLOCAL_ITR_MAIN.Dispose();
                daREMOTE_ITR_MAIN.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.ITR_STATUS.ToString()))
            {
                daLOCAL_ITR_STATUS = new AdapterITR_STATUS();
                daREMOTE_ITR_STATUS = new AdapterITR_STATUS(remoteConnStr);
                daLOCAL_ITR_STATUS.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_ITR_STATUS.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.ITR_STATUS);
                daLOCAL_ITR_STATUS.Dispose();
                daREMOTE_ITR_STATUS.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.ITR_STATUS_ISSUE.ToString()))
            {
                daLOCAL_ITR_STATUS_ISSUE = new AdapterITR_STATUS_ISSUE();
                daREMOTE_ITR_STATUS_ISSUE = new AdapterITR_STATUS_ISSUE(remoteConnStr);
                daLOCAL_ITR_STATUS_ISSUE.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_ITR_STATUS_ISSUE.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.ITR_STATUS_ISSUE);
                daLOCAL_ITR_STATUS_ISSUE.Dispose();
                daREMOTE_ITR_STATUS_ISSUE.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.CERTIFICATE_STATUS.ToString()))
            {
                daLOCAL_CERTIFICATE_STATUS = new AdapterCERTIFICATE_STATUS();
                daREMOTE_CERTIFICATE_STATUS = new AdapterCERTIFICATE_STATUS(remoteConnStr);
                daLOCAL_CERTIFICATE_STATUS.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_CERTIFICATE_STATUS.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.CERTIFICATE_STATUS);
                daLOCAL_CERTIFICATE_STATUS.Dispose();
                daREMOTE_CERTIFICATE_STATUS.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.CERTIFICATE_STATUS_ISSUE.ToString()))
            {
                daLOCAL_CERTIFICATE_STATUS_ISSUE = new AdapterCERTIFICATE_STATUS_ISSUE();
                daREMOTE_CERTIFICATE_STATUS_ISSUE = new AdapterCERTIFICATE_STATUS_ISSUE(remoteConnStr);
                daLOCAL_CERTIFICATE_STATUS_ISSUE.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_CERTIFICATE_STATUS_ISSUE.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.CERTIFICATE_STATUS_ISSUE);
                daLOCAL_CERTIFICATE_STATUS_ISSUE.Dispose();
                daREMOTE_CERTIFICATE_STATUS_ISSUE.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.PUNCHLIST_MAIN.ToString()))
            {
                daLOCAL_PUNCHLIST_MAIN = new AdapterPUNCHLIST_MAIN();
                daREMOTE_PUNCHLIST_MAIN = new AdapterPUNCHLIST_MAIN(remoteConnStr);
                daLOCAL_PUNCHLIST_MAIN.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_PUNCHLIST_MAIN.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.PUNCHLIST_MAIN);
                daLOCAL_PUNCHLIST_MAIN.Dispose();
                daREMOTE_PUNCHLIST_MAIN.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.PUNCHLIST_STATUS.ToString()))
            {
                daLOCAL_PUNCHLIST_STATUS = new AdapterPUNCHLIST_STATUS();
                daREMOTE_PUNCHLIST_STATUS = new AdapterPUNCHLIST_STATUS(remoteConnStr);
                daLOCAL_PUNCHLIST_STATUS.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_PUNCHLIST_STATUS.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.PUNCHLIST_STATUS);
                daLOCAL_PUNCHLIST_STATUS.Dispose();
                daREMOTE_PUNCHLIST_STATUS.Dispose();
            }
            if (Sync_Database.Any(obj => obj == Sync_Type.PUNCHLIST_STATUS_ISSUE.ToString()))
            {
                daLOCAL_PUNCHLIST_STATUS_ISSUE = new AdapterPUNCHLIST_STATUS_ISSUE();
                daREMOTE_PUNCHLIST_STATUS_ISSUE = new AdapterPUNCHLIST_STATUS_ISSUE(remoteConnStr);
                daLOCAL_PUNCHLIST_STATUS_ISSUE.Set_Update_Event(Update_Adapter_Progress);
                daREMOTE_PUNCHLIST_STATUS_ISSUE.Set_Update_Event(Update_Adapter_Progress);
                AdvanceStep(Sync_Type.PUNCHLIST_STATUS_ISSUE);
                daLOCAL_PUNCHLIST_STATUS_ISSUE.Dispose();
                daREMOTE_PUNCHLIST_STATUS_ISSUE.Dispose();
            }

            return state;
        }

        /// <summary>
        /// Queues this object in the thread pool.
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        public IWorkItemResult QueueProcess(SmartThreadPool pool)
        {
            workItemResult = null;
            if (pool != null)
            {
                try
                {
                    if (threadPoolState == null)
                    {
                        threadPoolState = new object();
                    }

                    workItemResult = pool.QueueWorkItem(new WorkItemCallback(this.Start), WorkItemPriority.Normal);
                }
                catch (Exception ex)
                {
                    ex.Data.Add("ProcessID", 0);
                    throw ex;
                }
            }
            return workItemResult;
        }
    }

    public class ProcessLastSyncDateProcessThreadEventArgs : EventArgs
    {
        public Sync_Type progress_type { get; set; }
        public DateTime LastSyncDateTime { get; set; }
    }

    public class ProcessThreadEventArgs : EventArgs
    {
        public Sync_Type progress_type { get; set; }
        public double progress_step { get; set; }
        public bool progress_doublestep { get; set; }
        public bool count_reset { get; set; }
        public bool progress_reset { get; set; }
        public string progress_status { get; set; }
        public SyncStatus_Count progress_countType { get; set; }

        public ProcessThreadEventArgs()
        {
            progress_step = 0;
            progress_doublestep = false;
            progress_status = string.Empty;
            progress_countType = SyncStatus_Count.None;
            count_reset = false;
            progress_reset = false;
        }
    }

    public class ProcessThreadList : List<ProcessThread>
    {
    }

    public class ProcessThreadManager : List<ProcessThread>
    {
        #region Properties
        public delegate void ProcessStatusHandler(object sender, ProcessThreadEventArgs e);
        public event ProcessStatusHandler ProcessStatus = delegate { };

        /// <summary>
        /// Get/set the thread pool
        /// </summary>
        public SmartThreadPool Pool { get; set; }
        /// <summary>
        /// Get/set the number of concurrent threads that can be running at once
        /// </summary>
        public int ConcurrentThreads { get; set; }
        /// <summary>
        /// Get/set the maximum number pf threads we will be running
        /// </summary>
        public int MaxThreads { get; set; }
        /// <summary>
        /// Get/set the default thread pool startup criteria
        /// </summary>
        public STPStartInfo StpStartInfo { get; set; }
        /// <summary>
        /// Get/set how long the thread pool is idle before it times out
        /// </summary>
        public int PoolIdleTimeout { get; set; }
        /// <summary>
        /// Get/set the progress reporting frequency to assign to all threads.
        /// </summary>
        public int ProgressFrequency { get; set; }
        #endregion Properties

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        public ProcessThreadManager()
        {
            this.Pool = null;
            this.ConcurrentThreads = 1;
            this.MaxThreads = 100;
            this.PoolIdleTimeout = 1000;
            this.ProgressFrequency = 10;

            this.StpStartInfo = new STPStartInfo();
            this.StpStartInfo.IdleTimeout = this.PoolIdleTimeout;
            this.StpStartInfo.MaxWorkerThreads = this.ConcurrentThreads;
            this.StpStartInfo.StartSuspended = true;
            this.StpStartInfo.PerformanceCounterInstanceName = "SmartThreadPool";
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Resets the manager with new criteria
        /// </summary>
        /// <param name="maxThreads"></param>
        /// <param name="concurrentThreads"></param>
        public void Reset_Step1(Guid ProjectGUID, SQLBase daLOCAL, SQLBase daREMOTE, dsSYNC_TABLE.SYNC_TABLEDataTable dtSYNC_OPTIONS, dsSYNC_CONFLICT.SYNC_CONFLICTDataTable dtREMOTE_SYNC_CONFLICT, dsTOMBSTONE.TOMBSTONEDataTable dtTOMBSTONE, string RemoteConnStr, bool forceSyncAll)
        {
            Clear();
            this.MaxThreads = 1;
            this.ConcurrentThreads = 1;
            //this.ProgressFrequency = frequency;
            this.StpStartInfo.MaxWorkerThreads = this.ConcurrentThreads;

            List<string> Sync_Database = new List<string>();
            if(dtSYNC_OPTIONS.Any(obj => obj.TYPE == Sync_Item.Project.ToString()))
                Sync_Database.Add(Sync_Type.PROJECT.ToString());
            else
            {
                ProcessThreadEventArgs ProgressEventArg = new ProcessThreadEventArgs();
                ProgressEventArg.progress_type = Sync_Type.PROJECT;
                ProgressEventArg.progress_status = SyncStatus_Display.Skipped.ToString();
                ProcessStatus(this, ProgressEventArg);
            }

            ProcessThread process = new ProcessThread(ProjectGUID, daLOCAL, daREMOTE, Sync_Database, dtREMOTE_SYNC_CONFLICT, dtTOMBSTONE, RemoteConnStr, forceSyncAll);
            Add(process);

            Reset_ThreadPool();
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Resets the manager with new criteria
        /// </summary>
        /// <param name="maxThreads"></param>
        /// <param name="concurrentThreads"></param>
        public void Reset_Step2(Guid ProjectGUID, SQLBase daLOCAL, SQLBase daREMOTE, dsSYNC_TABLE.SYNC_TABLEDataTable dtSYNC_OPTIONS, dsSYNC_CONFLICT.SYNC_CONFLICTDataTable dtREMOTE_SYNC_CONFLICT, dsTOMBSTONE.TOMBSTONEDataTable dtTOMBSTONE, string RemoteConnStr, bool forceSyncAll)
        {
            Clear();
            this.MaxThreads = 4;
            this.ConcurrentThreads = 4;
            //this.ProgressFrequency = frequency;
            this.StpStartInfo.MaxWorkerThreads = this.ConcurrentThreads;
            ProcessThreadEventArgs ProgressEventArg = new ProcessThreadEventArgs();

            List<string> Sync_Database_Thread1 = new List<string>();
            List<string> Sync_Database_Thread2 = new List<string>();
            List<string> Sync_Database_Thread3 = new List<string>();
            List<string> Sync_Database_Thread4 = new List<string>();

            if (dtSYNC_OPTIONS.Any(obj => obj.TYPE == Sync_Item.Equipment.ToString()))
                Sync_Database_Thread1.Add(Sync_Type.GENERAL_EQUIPMENT.ToString());
            else
            {
                ProgressEventArg.progress_type = Sync_Type.GENERAL_EQUIPMENT;
                ProgressEventArg.progress_status = SyncStatus_Display.Skipped.ToString();
                ProcessStatus(this, ProgressEventArg);
            }

            if (dtSYNC_OPTIONS.Any(obj => obj.TYPE == Sync_Item.Template_Components.ToString()))
            {
                Sync_Database_Thread1.Add(Sync_Type.PREFILL_MAIN.ToString());
                Sync_Database_Thread1.Add(Sync_Type.TEMPLATE_TOGGLE.ToString());
                Sync_Database_Thread1.Add(Sync_Type.MATRIX_TYPE.ToString());
                Sync_Database_Thread1.Add(Sync_Type.MATRIX_ASSIGNMENT.ToString());
            }
            else
            {
                ProgressEventArg.progress_type = Sync_Type.PREFILL_MAIN;
                ProgressEventArg.progress_status = SyncStatus_Display.Skipped.ToString();
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.TEMPLATE_TOGGLE;
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.MATRIX_TYPE;
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.MATRIX_ASSIGNMENT;
                ProcessStatus(this, ProgressEventArg);
            }
                
            if (dtSYNC_OPTIONS.Any(obj => obj.TYPE == Sync_Item.Template.ToString()))
            {
                Sync_Database_Thread2.Add(Sync_Type.WORKFLOW_MAIN.ToString());
                Sync_Database_Thread2.Add(Sync_Type.TEMPLATE_MAIN.ToString());
            }
            else
            {
                ProgressEventArg.progress_type = Sync_Type.WORKFLOW_MAIN;
                ProgressEventArg.progress_status = SyncStatus_Display.Skipped.ToString();
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.TEMPLATE_MAIN;
                ProcessStatus(this, ProgressEventArg);
            }

            if (dtSYNC_OPTIONS.Any(obj => obj.TYPE == Sync_Item.Role.ToString()))
                Sync_Database_Thread3.Add(Sync_Type.ROLE_MAIN.ToString());
            else
            {
                ProgressEventArg.progress_type = Sync_Type.ROLE_MAIN;
                ProgressEventArg.progress_status = SyncStatus_Display.Skipped.ToString();
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.ROLE_PRIVILEGE;
                ProcessStatus(this, ProgressEventArg);
            }

            if (dtSYNC_OPTIONS.Any(obj => obj.TYPE == Sync_Item.User.ToString()))
                Sync_Database_Thread3.Add(Sync_Type.USER_MAIN.ToString());
            else
            {
                ProgressEventArg.progress_type = Sync_Type.USER_MAIN;
                ProgressEventArg.progress_status = SyncStatus_Display.Skipped.ToString();
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.USER_DISC;
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.USER_PROJECT;
                ProcessStatus(this, ProgressEventArg);
            }

            if (dtSYNC_OPTIONS.Any(obj => obj.TYPE == Sync_Item.Schedule.ToString()))
                Sync_Database_Thread4.Add(Sync_Type.SCHEDULE.ToString());
            else
            {
                ProgressEventArg.progress_type = Sync_Type.SCHEDULE;
                ProgressEventArg.progress_status = SyncStatus_Display.Skipped.ToString();
                ProcessStatus(this, ProgressEventArg);
            }

            ProcessThread process1 = new ProcessThread(ProjectGUID, daLOCAL, daREMOTE, Sync_Database_Thread1, dtREMOTE_SYNC_CONFLICT, dtTOMBSTONE, RemoteConnStr, forceSyncAll);
            Add(process1);
            ProcessThread process2 = new ProcessThread(ProjectGUID, daLOCAL, daREMOTE, Sync_Database_Thread2, dtREMOTE_SYNC_CONFLICT, dtTOMBSTONE, RemoteConnStr, forceSyncAll);
            Add(process2);
            ProcessThread process3 = new ProcessThread(ProjectGUID, daLOCAL, daREMOTE, Sync_Database_Thread3, dtREMOTE_SYNC_CONFLICT, dtTOMBSTONE, RemoteConnStr, forceSyncAll);
            Add(process3);
            ProcessThread process4 = new ProcessThread(ProjectGUID, daLOCAL, daREMOTE, Sync_Database_Thread4, dtREMOTE_SYNC_CONFLICT, dtTOMBSTONE, RemoteConnStr, forceSyncAll);
            Add(process4);

            Reset_ThreadPool();
        }

        ///--------------------------------------------------------------------------------
        /// <summary>
        /// Resets the manager with new criteria
        /// </summary>
        /// <param name="maxThreads"></param>
        /// <param name="concurrentThreads"></param>
        public void Reset_Step3(Guid ProjectGUID, SQLBase daLOCAL, SQLBase daREMOTE, dsSYNC_TABLE.SYNC_TABLEDataTable dtSYNC_OPTIONS, dsSYNC_CONFLICT.SYNC_CONFLICTDataTable dtREMOTE_SYNC_CONFLICT, dsTOMBSTONE.TOMBSTONEDataTable dtTOMBSTONE, string RemoteConnStr, bool forceSyncAll)
        {
            Clear();
            this.MaxThreads = 2;
            this.ConcurrentThreads = 2;
            //this.ProgressFrequency = frequency;
            this.StpStartInfo.MaxWorkerThreads = this.ConcurrentThreads;
            ProcessThreadEventArgs ProgressEventArg = new ProcessThreadEventArgs();

            List<string> Sync_Database_Thread1 = new List<string>();
            List<string> Sync_Database_Thread2 = new List<string>();

            if (dtSYNC_OPTIONS.Any(obj => obj.TYPE == Sync_Item.Schedule.ToString()))
            {
                Sync_Database_Thread1.Add(Sync_Type.TAG.ToString());
                Sync_Database_Thread2.Add(Sync_Type.WBS.ToString());
            }
            else
            {
                ProgressEventArg.progress_type = Sync_Type.TAG;
                ProgressEventArg.progress_status = SyncStatus_Display.Skipped.ToString();
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.WBS;
                ProcessStatus(this, ProgressEventArg);
            }

            ProcessThread process1 = new ProcessThread(ProjectGUID, daLOCAL, daREMOTE, Sync_Database_Thread1, dtREMOTE_SYNC_CONFLICT, dtTOMBSTONE, RemoteConnStr, forceSyncAll);
            Add(process1);
            ProcessThread process2 = new ProcessThread(ProjectGUID, daLOCAL, daREMOTE, Sync_Database_Thread2, dtREMOTE_SYNC_CONFLICT, dtTOMBSTONE, RemoteConnStr, forceSyncAll);
            Add(process2);

            Reset_ThreadPool();
        }

        ///--------------------------------------------------------------------------------
        /// <summary>
        /// Resets the manager with new criteria
        /// </summary>
        /// <param name="maxThreads"></param>
        /// <param name="concurrentThreads"></param>
        public void Reset_Step4(Guid ProjectGUID, SQLBase daLOCAL, SQLBase daREMOTE, dsSYNC_TABLE.SYNC_TABLEDataTable dtSYNC_OPTIONS, dsSYNC_CONFLICT.SYNC_CONFLICTDataTable dtREMOTE_SYNC_CONFLICT, dsTOMBSTONE.TOMBSTONEDataTable dtTOMBSTONE, string RemoteConnStr, bool forceSyncAll)
        {
            Clear();
            this.MaxThreads = 2;
            this.ConcurrentThreads = 2;
            //this.ProgressFrequency = frequency;
            this.StpStartInfo.MaxWorkerThreads = this.ConcurrentThreads;
            ProcessThreadEventArgs ProgressEventArg = new ProcessThreadEventArgs();

            List<string> Sync_Database_Thread1 = new List<string>();
            List<string> Sync_Database_Thread2 = new List<string>();

            if (dtSYNC_OPTIONS.Any(obj => obj.TYPE == Sync_Item.ITR.ToString()))
            {
                Sync_Database_Thread1.Add(Sync_Type.ITR_MAIN.ToString());
                Sync_Database_Thread1.Add(Sync_Type.ITR_STATUS.ToString());
                Sync_Database_Thread1.Add(Sync_Type.ITR_STATUS_ISSUE.ToString());
            }
            else
            {
                ProgressEventArg.progress_type = Sync_Type.ITR_MAIN;
                ProgressEventArg.progress_status = SyncStatus_Display.Skipped.ToString();
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.ITR_STATUS;
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.ITR_STATUS_ISSUE;
                ProcessStatus(this, ProgressEventArg);
            }

            if (dtSYNC_OPTIONS.Any(obj => obj.TYPE == Sync_Item.Punchlist.ToString()))
            {
                Sync_Database_Thread2.Add(Sync_Type.PUNCHLIST_MAIN.ToString());
                Sync_Database_Thread2.Add(Sync_Type.PUNCHLIST_STATUS.ToString());
                Sync_Database_Thread2.Add(Sync_Type.PUNCHLIST_STATUS_ISSUE.ToString());
            }
            else
            {
                ProgressEventArg.progress_type = Sync_Type.PUNCHLIST_MAIN;
                ProgressEventArg.progress_status = SyncStatus_Display.Skipped.ToString();
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.PUNCHLIST_STATUS;
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.PUNCHLIST_STATUS_ISSUE;
                ProcessStatus(this, ProgressEventArg);
            }

            ProcessThread process1 = new ProcessThread(ProjectGUID, daLOCAL, daREMOTE, Sync_Database_Thread1, dtREMOTE_SYNC_CONFLICT, dtTOMBSTONE, RemoteConnStr, forceSyncAll);
            Add(process1);
            ProcessThread process2 = new ProcessThread(ProjectGUID, daLOCAL, daREMOTE, Sync_Database_Thread2, dtREMOTE_SYNC_CONFLICT, dtTOMBSTONE, RemoteConnStr, forceSyncAll);
            Add(process2);

            Reset_ThreadPool();
        }

        ///--------------------------------------------------------------------------------
        /// <summary>
        /// Resets the manager with new criteria
        /// </summary>
        /// <param name="maxThreads"></param>
        /// <param name="concurrentThreads"></param>
        public void Reset_Step5(Guid ProjectGUID, SQLBase daLOCAL, SQLBase daREMOTE, dsSYNC_TABLE.SYNC_TABLEDataTable dtSYNC_OPTIONS, dsSYNC_CONFLICT.SYNC_CONFLICTDataTable dtREMOTE_SYNC_CONFLICT, dsTOMBSTONE.TOMBSTONEDataTable dtTOMBSTONE, string RemoteConnStr, bool forceSyncAll)
        {
            Clear();
            this.MaxThreads = 2;
            this.ConcurrentThreads = 2;
            //this.ProgressFrequency = frequency;
            this.StpStartInfo.MaxWorkerThreads = this.ConcurrentThreads;
            ProcessThreadEventArgs ProgressEventArg = new ProcessThreadEventArgs();

            List<string> Sync_Database_Thread1 = new List<string>();
            List<string> Sync_Database_Thread2 = new List<string>();

            if (dtSYNC_OPTIONS.Any(obj => obj.TYPE == Sync_Item.Schedule.ToString()))
                Sync_Database_Thread1.Add(Sync_Type.TEMPLATE_REGISTER.ToString());
            else
            {
                ProgressEventArg.progress_type = Sync_Type.TEMPLATE_REGISTER;
                ProgressEventArg.progress_status = SyncStatus_Display.Skipped.ToString();
                ProcessStatus(this, ProgressEventArg);
            }

            if (dtSYNC_OPTIONS.Any(obj => obj.TYPE == Sync_Item.Header_Data.ToString()))
                Sync_Database_Thread2.Add(Sync_Type.PREFILL_REGISTER.ToString());
            else
            {
                ProgressEventArg.progress_type = Sync_Type.PREFILL_REGISTER;
                ProgressEventArg.progress_status = SyncStatus_Display.Skipped.ToString();
                ProcessStatus(this, ProgressEventArg);
            }

            ProcessThread process1 = new ProcessThread(ProjectGUID, daLOCAL, daREMOTE, Sync_Database_Thread1, dtREMOTE_SYNC_CONFLICT, dtTOMBSTONE, RemoteConnStr, forceSyncAll);
            Add(process1);
            ProcessThread process2 = new ProcessThread(ProjectGUID, daLOCAL, daREMOTE, Sync_Database_Thread2, dtREMOTE_SYNC_CONFLICT, dtTOMBSTONE, RemoteConnStr, forceSyncAll);
            Add(process2);

            Reset_ThreadPool();
        }

        ///--------------------------------------------------------------------------------
        /// <summary>
        /// Resets the manager with new criteria
        /// </summary>
        /// <param name="maxThreads"></param>
        /// <param name="concurrentThreads"></param>
        public void Reset_Step6(Guid ProjectGUID, SQLBase daLOCAL, SQLBase daREMOTE, dsSYNC_TABLE.SYNC_TABLEDataTable dtSYNC_OPTIONS, dsSYNC_CONFLICT.SYNC_CONFLICTDataTable dtREMOTE_SYNC_CONFLICT, dsTOMBSTONE.TOMBSTONEDataTable dtTOMBSTONE, string RemoteConnStr, bool forceSyncAll)
        {
            Clear();
            this.MaxThreads = 2;
            this.ConcurrentThreads = 2;
            //this.ProgressFrequency = frequency;
            this.StpStartInfo.MaxWorkerThreads = this.ConcurrentThreads;
            ProcessThreadEventArgs ProgressEventArg = new ProcessThreadEventArgs();
            List<string> Sync_Database_Thread1 = new List<string>();

            if (dtSYNC_OPTIONS.Any(obj => obj.TYPE == Sync_Item.Certificate.ToString()))
            {
                Sync_Database_Thread1.Add(Sync_Type.CERTIFICATE_MAIN.ToString());
                Sync_Database_Thread1.Add(Sync_Type.CERTIFICATE_DATA.ToString());
                Sync_Database_Thread1.Add(Sync_Type.CERTIFICATE_STATUS.ToString());
                Sync_Database_Thread1.Add(Sync_Type.CERTIFICATE_STATUS_ISSUE.ToString());
            }
            else
            {
                ProgressEventArg.progress_type = Sync_Type.CERTIFICATE_MAIN;
                ProgressEventArg.progress_status = SyncStatus_Display.Skipped.ToString();
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.CERTIFICATE_DATA;
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.CERTIFICATE_STATUS;
                ProcessStatus(this, ProgressEventArg);
                ProgressEventArg.progress_type = Sync_Type.CERTIFICATE_STATUS_ISSUE;
                ProcessStatus(this, ProgressEventArg);
            }

            ProcessThread process1 = new ProcessThread(ProjectGUID, daLOCAL, daREMOTE, Sync_Database_Thread1, dtREMOTE_SYNC_CONFLICT, dtTOMBSTONE, RemoteConnStr, forceSyncAll);
            Add(process1);

            Reset_ThreadPool();
        }

        /// <summary>
        /// Reset the thread pool
        /// </summary>
        public void Reset_ThreadPool()
        {
            if (this.Pool != null)
            {
                this.Pool.Cancel();
                while (this.Pool.ActiveThreads != 0)
                {
                    Thread.Sleep(50);
                }
                this.Pool.Dispose();
                this.Pool = null;
            }
            this.Pool = new SmartThreadPool(this.StpStartInfo);
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Executes the processes in the thread pool 
        /// </summary>
        public void Start()
        {
            if (this.Pool != null)
            {
                QueueWorkItems();
                this.Pool.Start();
            }
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Places the threads into the thread pool queue
        /// </summary>
        private void QueueWorkItems()
        {
            if (this.Pool != null)
            {
                foreach (ProcessThread thread in this)
                {
                    IWorkItemResult workItem = thread.QueueProcess(this.Pool);
                }
            }
        }
    }
}