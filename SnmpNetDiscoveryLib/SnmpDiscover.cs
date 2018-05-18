using System;
using System.Management.Automation; //Add this PowerShell directive
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;


namespace SnmpNetDiscoveryLib
{
    [Cmdlet(VerbsCommon.Get, "SnmpDiscovery")]
    public class SnmpDiscover : SharpSnmp
    {
        protected override void BeginProcessing()
        {
            WriteVerbose("Starting discovery process");
        }

        protected override void ProcessRecord()
        {
            var ips = DiscoverAsync().Result;
            WriteObject(ips);
        }

        protected override void EndProcessing()
        {
            WriteVerbose("Discovery process complete");
        }

        private async Task<HashSet<string>> DiscoverAsync()
        {
            var addresses = new HashSet<string>();
            var endPoint = new IPEndPoint(IPAddress.Broadcast, 161);

            try
            {
                var discoverer = new Discoverer();
                discoverer.AgentFound += (sender, e) => { addresses.Add(e.Agent.Address.ToString()); };

                await discoverer.DiscoverAsync(VersionCode.V1, endPoint, _community, _timeOut);
                await discoverer.DiscoverAsync(VersionCode.V2, endPoint, _community, _timeOut);
                await discoverer.DiscoverAsync(VersionCode.V3, endPoint, null, _timeOut);
            }
            catch (Exception e)
            {
                WriteWarning($"Could not complete the request: {e.Message}");
            }

            return addresses;
        }
    }
}