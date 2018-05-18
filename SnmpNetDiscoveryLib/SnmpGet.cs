using System;
using System.Management.Automation; //Add this PowerShell directive
using System.Collections.Generic;
using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;


namespace SnmpNetDiscoveryLib
{
    [Cmdlet(VerbsCommon.Get, "SnmpNetworks")]
    public class SnmpGet : SharpSnmp
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Ip { get; set; }

        protected override void ProcessRecord()
        {
            Dictionary<string, string> results = new Dictionary<string, string>();

            try
            {
                var request = new GetBulkRequestMessage(0, VersionCode.V2, _community, 0, 5, _vList);
                var iPEndPoint = new IPEndPoint(IPAddress.Parse(Ip), 161);
                var reply = request.GetResponse(_timeOut, iPEndPoint);
                var variables = reply.Pdu().Variables;

                foreach (Variable variable in variables)
                {
                    var key = GetOidDescription(variable.Id.ToString());
                    var data = ConvertVariableData(variable.Data);
                    if (data.Length > 0 && !results.ContainsKey(key))
                    {
                        results.Add(key, data);
                    }
                }

                WriteObject(results);
            }
            catch (Exception e)
            {
                WriteWarning($"Could not complete the request: {e.Message}");
            }
        }
    }
}