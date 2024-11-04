using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ProjectLibrary
{
    public static class System_Environment
    {
        static User _user;
        static List<Privilege> _privileges;

        static System_Environment()
        {
            _privileges = new List<Privilege>();
        }

        public static void SetUser(User LogonUser)
        {
            _user = LogonUser;
        }

        public static void ChangeDefaultDiscipline(string discipline)
        {
            _user.userDiscipline = discipline;
        }

        public static void ChangeDefaultProject(ValuePair project)
        {
            _user.userProject = project;
        }

        public static void AddPrivileges(Privilege newPrivilege)
        {
            _privileges.Add(newPrivilege);
        }

        public static User GetUser()
        {
            return _user;
        }

        public static bool HasPrivilege(PrivilegeTypeID typeID)
        {
            return _privileges.Any(obj => obj.privTypeID == typeID.ToString());
        }

        public static void Clear()
        {
            _privileges.Clear();
            _user = null;
        }
    }
}
