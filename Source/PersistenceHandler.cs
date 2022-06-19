using System;
using System.Collections.Generic;
using System.Linq;

namespace computing_project
{
    public class PersistenceHandler
    {

        // Stores the blacklist, once added they will not be created when level is loaded.
        private List<string> _doors;
        private List<string> _keys;

        public PersistenceHandler()
        {
            _doors = new List<string>();
            _keys = new List<string>();
        }

        // The blacklist functions add a key or door to their respective blacklist.
        public void BlacklistDoor(string name)
        {
            _doors.Add(name);
        }

        public void BlacklistKey(string name)
        {
            _keys.Add(name);
        }

        // The check functions will check if the name of a paricular key or door is on the blacklist, if it is then it returns true.
        public bool CheckKeyBlacklist(string name)
        {
            for (int i = 0; i < _keys.Count(); i++)
            {
                if (_keys.ElementAt(i) == name)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckDoorBlacklist(string name)
        {
            for (int i = 0; i < _doors.Count(); i++)
            {
                if (_doors.ElementAt(i) == name)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
