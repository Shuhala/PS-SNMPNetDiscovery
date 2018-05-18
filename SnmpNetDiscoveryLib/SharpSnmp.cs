using System.Management.Automation;
using System.Collections.Generic;
using Lextm.SharpSnmpLib;


namespace SnmpNetDiscoveryLib
{
    public abstract class SharpSnmp : Cmdlet
    {
        [Alias("TimeOut")] [Parameter] public int _timeOut;
        [Alias("Community")] [Parameter] public OctetString _community;
        [Alias("Oids")] [Parameter] public Dictionary<string, string> _oids;
        public List<Variable> _vList;

        protected SharpSnmp()
        {
            _timeOut = 10000;
            _community = new OctetString("public");
            _vList = new List<Variable>();
            _oids = new Dictionary<string, string>
            {
                {"sysDescr", "1.3.6.1.2.1.1.1"},
                {"ifPhysAddress", "1.3.6.1.2.1.2.2.1.6"},
                {"interface", "1.3.6.1.2.1.2"},
                {"system", "1.3.6.1.2.1.1"}
            };
            foreach (var oid in _oids.Values)
            {
                _vList.Add(new Variable(new ObjectIdentifier(oid)));
            }
        }

        protected string ConvertVariableData(ISnmpData data)
        {
            if (!(data is OctetString s))
            {
                return data.ToString();
            }

            var count = System.Text.Encoding.ASCII.GetByteCount(s.ToString());
            return count == 6 ? s.ToHexString() : data.ToString();
        }

        protected string GetOidDescription(string oid)
        {
            foreach (KeyValuePair<string, string> pair in _oids)
            {
                if (oid.Contains(pair.Value))
                {
                    return $"{pair.Key}/{oid}";
                }
            }

            return null;
        }
    }
}