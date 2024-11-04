using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsPUNCHLIST_MAIN_PICTURETableAdapters;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterPUNCHLIST_MAIN_PICTURE : SQLBase, IDisposable
    {
        private PUNCHLIST_MAIN_PICTURETableAdapter _adapter;

        public AdapterPUNCHLIST_MAIN_PICTURE()
            : base()
        {
            _adapter = new PUNCHLIST_MAIN_PICTURETableAdapter(Variables.ConnStr);
        }

        public AdapterPUNCHLIST_MAIN_PICTURE(string connStr)
            : base(connStr)
        {
            _adapter = new PUNCHLIST_MAIN_PICTURETableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable Get()
        {
            string sql = "SELECT * FROM PUNCHLIST_MAIN_PICTURE WHERE DELETED IS NULL";
            dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable dtPUNCHLIST_MAIN_PICTURE = new dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_MAIN_PICTURE);

            if (dtPUNCHLIST_MAIN_PICTURE.Rows.Count > 0)
                return dtPUNCHLIST_MAIN_PICTURE;
            else
                return null;
        }

        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable GetBy(Guid punchlistGuid, PunchlistImageType punchlistImageType)
        {
            int intPunchlistImageType = (int)punchlistImageType;
            string sql = "SELECT * FROM PUNCHLIST_MAIN_PICTURE WHERE PUNCHLIST_MAIN_GUID = '" + punchlistGuid + "' AND PICTURE_TYPE = '" + intPunchlistImageType.ToString() + "' AND DELETED IS NULL";
            dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable dtPUNCHLIST_MAIN_PICTURE = new dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_MAIN_PICTURE);

            if (dtPUNCHLIST_MAIN_PICTURE.Rows.Count > 0)
                return dtPUNCHLIST_MAIN_PICTURE;
            else
                return null;
        }

        public void SavePunchlistPictures(Guid punchlistGuid, List<Image> images, PunchlistImageType punchlistImageType, bool skipChecking)
        {
            //convert save images to byte array
            List<byte[]> saveByteArrays = new List<byte[]>();
            List<Tuple<Guid, byte[]>> existingByteArrays = new List<Tuple<Guid, byte[]>>();
            foreach (Image image in images)
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                saveByteArrays.Add(stream.ToArray());
            }

            if (!skipChecking)
            {
                dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable dtPUNCHLIST_MAIN_PICTURE = GetBy(punchlistGuid, punchlistImageType);
                if (dtPUNCHLIST_MAIN_PICTURE != null)
                {
                    foreach (dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURERow drPUNCHLIST_MAIN_PICTURE in dtPUNCHLIST_MAIN_PICTURE.Rows)
                    {
                        existingByteArrays.Add(new Tuple<Guid, byte[]>(drPUNCHLIST_MAIN_PICTURE.GUID, drPUNCHLIST_MAIN_PICTURE.PICTURE));
                    }
                }

                //remove images that doesn't exist in save collection
                foreach (Tuple<Guid, byte[]> dbImage in existingByteArrays)
                {
                    if (!saveByteArrays.Any(x => x.SequenceEqual(dbImage.Item2)))
                    {
                        RemoveBy(dbImage.Item1);
                    }
                }
            }

            //save new images that doesn't exists in database
            using (dsPUNCHLIST_MAIN_PICTURE dsPUNCHLIST_MAIN_PICTURE = new dsPUNCHLIST_MAIN_PICTURE())
            {
                foreach (byte[] saveByteArray in saveByteArrays)
                {
                    if (!existingByteArrays.Any(x => x.Item2.SequenceEqual(saveByteArray)))
                    {
                        dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURERow drPUNCHLIST_MAIN_PICTURE = dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURE.NewPUNCHLIST_MAIN_PICTURERow();
                        drPUNCHLIST_MAIN_PICTURE.GUID = Guid.NewGuid();
                        drPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_GUID = punchlistGuid;
                        drPUNCHLIST_MAIN_PICTURE.PICTURE = saveByteArray;
                        drPUNCHLIST_MAIN_PICTURE.PICTURE_TYPE = (int)punchlistImageType;
                        drPUNCHLIST_MAIN_PICTURE.CREATED = DateTime.Now;
                        drPUNCHLIST_MAIN_PICTURE.CREATEDBY = System_Environment.GetUser().GUID;
                        dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURE.AddPUNCHLIST_MAIN_PICTURERow(drPUNCHLIST_MAIN_PICTURE);
                        Save(drPUNCHLIST_MAIN_PICTURE);
                    }
                }
            }
        }


        public dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURERow SavePunchlistPicture(Guid punchlistGuid, Image image, PunchlistImageType punchlistImageType)
        {
            //convert save images to byte array
            byte[] saveByteArray;
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            saveByteArray = stream.ToArray();

            //save new images that doesn't exists in database
            using (dsPUNCHLIST_MAIN_PICTURE dsPUNCHLIST_MAIN_PICTURE = new dsPUNCHLIST_MAIN_PICTURE())
            {
                dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURERow drPUNCHLIST_MAIN_PICTURE = dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURE.NewPUNCHLIST_MAIN_PICTURERow();
                drPUNCHLIST_MAIN_PICTURE.GUID = Guid.NewGuid();
                drPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_GUID = punchlistGuid;
                drPUNCHLIST_MAIN_PICTURE.PICTURE = saveByteArray;
                drPUNCHLIST_MAIN_PICTURE.PICTURE_TYPE = (int)punchlistImageType;
                drPUNCHLIST_MAIN_PICTURE.CREATED = DateTime.Now;
                drPUNCHLIST_MAIN_PICTURE.CREATEDBY = System_Environment.GetUser().GUID;
                dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURE.AddPUNCHLIST_MAIN_PICTURERow(drPUNCHLIST_MAIN_PICTURE);
                Save(drPUNCHLIST_MAIN_PICTURE);
                return drPUNCHLIST_MAIN_PICTURE;
            }
        }

        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable GetIncludeDeleted()
        {
            string sql = "SELECT * FROM PUNCHLIST_MAIN_PICTURE";
            dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable dtPUNCHLIST_MAIN_PICTURE = new dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_MAIN_PICTURE);

            if (dtPUNCHLIST_MAIN_PICTURE.Rows.Count > 0)
                return dtPUNCHLIST_MAIN_PICTURE;
            else
                return null;
        }

        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURERow GetIncludeDeletedBy(Guid prefillGuid)
        {
            string sql = "SELECT * FROM PUNCHLIST_MAIN_PICTURE WHERE GUID = '" + prefillGuid + "'";
            dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable dtPUNCHLIST_MAIN_PICTURE = new dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_MAIN_PICTURE);

            if (dtPUNCHLIST_MAIN_PICTURE.Rows.Count > 0)
                return dtPUNCHLIST_MAIN_PICTURE[0];
            else
                return null;
        }

        /// <summary>
        /// Remove a particular header by GUID
        /// </summary>
        public bool RemoveBy(Guid guid)
        {
            string sql = "SELECT * FROM PUNCHLIST_MAIN_PICTURE WHERE GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable dtHeader = new dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable();
            ExecuteQuery(sql, dtHeader);

            if (dtHeader != null)
            {
                dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURERow drPUNCHLIST_MAIN_PICTURE = dtHeader[0];
                drPUNCHLIST_MAIN_PICTURE.DELETED = DateTime.Now;
                drPUNCHLIST_MAIN_PICTURE.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drPUNCHLIST_MAIN_PICTURE);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove a particular header by GUID
        /// </summary>
        public int RemoveWithExclusionBy(string prefillName, Guid excludedGuid)
        {
            string sql = "SELECT * FROM PUNCHLIST_MAIN_PICTURE WHERE NAME = '" + prefillName + "'";
            sql += " AND GUID IS NOT '" + excludedGuid + "'";
            sql += " AND DELETED IS NULL";

            dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable dtPunchlistPicture = new dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable();
            ExecuteQuery(sql, dtPunchlistPicture);

            int removeCount = 0;
            if (dtPunchlistPicture != null)
            {
                foreach (dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURERow drPunchlistPicture in dtPunchlistPicture.Rows)
                {
                    drPunchlistPicture.DELETED = DateTime.Now;
                    drPunchlistPicture.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drPunchlistPicture);
                    removeCount++;
                }
            }

            return removeCount;
        }

        /// <summary>
        /// Update multiple templates
        /// </summary>
        public void Save(dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable dtPUNCHLIST_MAIN_PICTURE)
        {
            _adapter.Update(dtPUNCHLIST_MAIN_PICTURE);
        }

        /// <summary>
        /// Update one template
        /// </summary>
        public void Save(dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURERow drPUNCHLIST_MAIN_PICTURE)
        {
            string s = string.Empty;
            try
            {
                int result = _adapter.Update(drPUNCHLIST_MAIN_PICTURE);
            }
            catch (Exception e)
            {
                s = e.ToString();
            }
        }
        #endregion
        public void Dispose()
        {
            if (_adapter != null)
            {
                _adapter.Dispose();
                _adapter = null;
            }
        }
    }
}