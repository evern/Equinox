using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;
using ProjectCommon;

namespace CheckmateDX
{
    public class SignatureUser
    {
        public Bitmap Signature { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public DateTime SignDate { get; set; }
        public string AdditionalInfo { get; set; }
        public ITR_Status SignStatus { get; set; }
        public CVC_Status SignCertificateStatus { get; set; }
    }

    public static class SignatureUserHelper
    {
        public static List<SignatureUser> GetSignatureUser(dsITR_STATUS.ITR_STATUSDataTable ITR_Statuses, ITR_Status latest_Status)
        {
            List<SignatureUser> signature_users = new List<SignatureUser>();
            using (AdapterUSER_MAIN _daUser = new AdapterUSER_MAIN())
            {
                for (int i = ITR_Statuses.Rows.Count - 1; i >= 0; i--)
                {
                    dsITR_STATUS.ITR_STATUSRow drITRStatus = ITR_Statuses[i];
                    if (drITRStatus.STATUS_NUMBER <= (int)latest_Status)
                    {
                        SignatureUser signature_user = new SignatureUser();
                        dsUSER_MAIN.USER_MAINRow drUser = _daUser.GetIncludeDeletedBy(drITRStatus.CREATEDBY);
                        if (drUser != null)
                        {
                            signature_user.Name = drUser.LASTNAME + ", " + drUser.FIRSTNAME;
                            signature_user.Company = drUser.COMPANY;
                            signature_user.AdditionalInfo = drUser.IsINFONull() ? string.Empty : drUser.INFO;

                            if (!drUser.IsSIGNATURENull())
                            {
                                signature_user.Signature = new Bitmap(Common.ConvertByteArrayToImage(drUser.SIGNATURE));
                                signature_user.Signature = Common.ResizeBitmap(signature_user.Signature, 300, 150);
                            }
                        }
                        else
                        {
                            signature_user.Name = "SUPERADMIN";
                            signature_user.Company = "SUPERADMIN";
                        }

                        signature_user.SignStatus = (ITR_Status)drITRStatus.STATUS_NUMBER;
                        signature_user.SignDate = drITRStatus.CREATED;
                        signature_users.Add(signature_user);
                    }
                }
            }

            return signature_users;
        }

        public static List<SignatureUser> GetCertificateSignatureUser(dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable certificateStatuses, CVC_Status latest_Status)
        {
            List<SignatureUser> signature_users = new List<SignatureUser>();
            using (AdapterUSER_MAIN _daUser = new AdapterUSER_MAIN())
            {
                for (int i = certificateStatuses.Rows.Count - 1; i >= 0; i--)
                {
                    dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow drCERTIFICATE_STATUS = certificateStatuses[i];
                    if (drCERTIFICATE_STATUS.STATUS_NUMBER <= (int)latest_Status)
                    {
                        SignatureUser signature_user = new SignatureUser();
                        dsUSER_MAIN.USER_MAINRow drUser = _daUser.GetIncludeDeletedBy(drCERTIFICATE_STATUS.CREATEDBY);
                        if (drUser != null)
                        {
                            signature_user.Name = drUser.LASTNAME + ", " + drUser.FIRSTNAME;
                            signature_user.Company = drUser.COMPANY;
                            signature_user.AdditionalInfo = drUser.IsINFONull() ? string.Empty : drUser.INFO;

                            if (!drUser.IsSIGNATURENull())
                            {
                                signature_user.Signature = new Bitmap(Common.ConvertByteArrayToImage(drUser.SIGNATURE));
                                signature_user.Signature = Common.ResizeBitmap(signature_user.Signature, 300, 150);
                            }
                        }
                        else
                        {
                            signature_user.Name = "SUPERADMIN";
                            signature_user.Company = "SUPERADMIN";
                        }

                        signature_user.SignCertificateStatus = (CVC_Status)drCERTIFICATE_STATUS.STATUS_NUMBER;
                        signature_user.SignDate = drCERTIFICATE_STATUS.CREATED;
                        signature_users.Add(signature_user);
                    }
                }
            }

            return signature_users;
        }

        public static SignatureUser GetSignatureUser(Guid? userGuid = null)
        {
            if(userGuid == null)
                userGuid = System_Environment.GetUser().GUID;

            SignatureUser signature_user = new SignatureUser();
            using (AdapterUSER_MAIN _daUser = new AdapterUSER_MAIN())
            {
                dsUSER_MAIN.USER_MAINRow drUser = _daUser.GetIncludeDeletedBy((Guid)userGuid);
                if (drUser != null)
                {
                    signature_user.Name = drUser.LASTNAME + ", " + drUser.FIRSTNAME;
                    signature_user.Company = drUser.COMPANY;
                    signature_user.AdditionalInfo = drUser.IsINFONull() ? string.Empty : drUser.INFO;

                    if (!drUser.IsSIGNATURENull())
                    {
                        signature_user.Signature = new Bitmap(Common.ConvertByteArrayToImage(drUser.SIGNATURE));
                        signature_user.Signature = Common.ResizeBitmap(signature_user.Signature, 300, 150);
                    }
                }
                else
                {
                    signature_user.Name = "SUPERADMIN";
                    signature_user.Company = "SUPERADMIN";
                }
            }

            return signature_user;
        }
    }
}
