// CmisCmdlets - Cmdlets to use CMIS from Powershell and Pash
// Copyright (C) GRAU DATA 2013-2014
//
// Author(s): Stefan Burnicki <stefan.burnicki@graudata.com>
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL was
// not distributed with this file, You can obtain one at
//  http://mozilla.org/MPL/2.0/.
using System;
using System.Management.Automation;
using System.Collections;
using DotCMIS;
using System.Net;
using System.Net.Security;
using Cmis.Utility;

namespace CmisCmdlets
{
    [Cmdlet(VerbsCommunications.Connect, "Cmis", DefaultParameterSetName = "AtomPub")]
    public class ConnectCmisCommand : CmisCommandBase
    {        
        [Parameter(Mandatory=true,
                            ParameterSetName = "AtomPub",
                            ValueFromPipelineByPropertyName = true,
                            Position = 0,
                            HelpMessage = "Url to the AtomPub interface of the Cmis repository")]
        public string Url { get; set; }

        [Parameter(Mandatory=true,
                   ParameterSetName = "AtomPub",
                   ValueFromPipelineByPropertyName = true,
                   Position = 1,
                   HelpMessage = "Username to the AtomPub interface of the Cmis repository")]
        public string UserName { get; set; }

        [Parameter(Mandatory=true,
                   ParameterSetName = "AtomPub",
                   ValueFromPipelineByPropertyName = true,
                   Position = 2,
                   HelpMessage = "Password to the AtomPub interface of the Cmis repository")]
        public string Password { get; set; }

        [Parameter(Mandatory=true,
                   ParameterSetName = "CustomParameters",
                   ValueFromPipeline = true,
                   Position = 0,
                   HelpMessage = "OpenCMIS parameters for the Cmis repository")]
        public Hashtable Parameters { get; set; }

        [Parameter]
        public SwitchParameter Insecure { get; set; }

        [Parameter(Mandatory=false,
                   Position = 3,
                   HelpMessage = "Repository name to connect to")]
        public string Repository { get; set; }

        private static RemoteCertificateValidationCallback _acceptInsecureSSLCallback = 
            (sender, certificate, chain, sslPolicyErrors) => true;

        protected override void ProcessRecord()
        {
            // first make sure to reset existing data
            ConnectionParameters = null;
            SetCmisSession(null);

            // set the connection parameters
            if (Parameters == null)
            {
                ConnectionParameters = ConnectionFactory.CreateAtomPubParams(Url, UserName, Password);
            }
            else
            {
                ConnectionParameters = Utilities.HashtableToStringDict(Parameters);
            }

            // check if insecure SSLs should be used anyway
            if (Insecure)
            {
                // TODO: improve unsecure SLL handling
                ServicePointManager.ServerCertificateValidationCallback += 
                    _acceptInsecureSSLCallback;
            }
            else
            {
                ServicePointManager.ServerCertificateValidationCallback -= 
                    _acceptInsecureSSLCallback;
            }

            // check if we should directly connect to a specified repository
            if (!String.IsNullOrEmpty(Repository))
            {
                // either by optional name
                SetCmisSession(ConnectionFactory.Connect(ConnectionParameters, Repository));
            }
            else if (ConnectionParameters.ContainsKey(SessionParameter.RepositoryId))
            {
                // or by provided parameters
                SetCmisSession(ConnectionFactory.Connect(ConnectionParameters));
            }
        }
    }
}

