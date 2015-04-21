using Exortech.NetReflector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ccnet.tfsrevisionlabeller.plugin
{
    [ReflectorType("tfsRevisionLabeller")]
    public class TfsRevisionLabeller : ITask, ILabeller
    {
        public TfsRevisionLabeller()
        {
        }

        #region ITask Members

        public void Run(IIntegrationResult result)
        {
            result.Label = this.Generate(result);
        }

        #endregion ITask Members

        #region ILabeller Members

        public string Generate(IIntegrationResult integrationResult)
        {
            string version = "0.0.0.1";
            _build = GetChangeSetId(Server, ProjectPath, Domain, Username, Password);
            var _rebuild = GetRebuildNumber(_build, integrationResult.LastSuccessfulIntegrationLabel);
            version = string.Format("{0}{1}.{2}.{3}.{4}", Prefix, Major, Minor, Build, _rebuild);
            return version;
        }

        private int GetRebuildNumber(int currentBuild, string lastLabel)
        {
            var p = @"([\d]+)\.([\d]+)\.([\d]+)\.([\d+])";
            var v = Regex.Match(lastLabel, p).Groups[3].Value;
            var r = Regex.Match(lastLabel, p).Groups[4].Value;

            int number = 0;
            int rebuild = 0;
            Int32.TryParse(v, out number);
            Int32.TryParse(r, out rebuild);

            if (number == currentBuild)
            {
                rebuild++;
            }
            else
            {
                rebuild = 1;
            }
            return rebuild;
        }

        /// <summary>
        /// tf history /collection:"http://tfs:8080/tfs/Collection" "$/Project" /recursive /stopafter:1 /noprompt /login:domain\user,password
        /// </summary>
        private int GetChangeSetId(string collectionUrl, string projectPath, string domain, string user, string password)
        {
            var arguments = string.Format("history /collection:\"{0}\" \"{1}\" /recursive /noprompt /stopafter:1 /login:{2}\\{3},{4}",
                collectionUrl.Trim(), projectPath.Trim(), domain.Trim(), user.Trim(), password.Trim());

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Executable,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            string line = string.Empty;
            while (!proc.StandardOutput.EndOfStream)
            {
                line = proc.StandardOutput.ReadLine();
            }

            line = Regex.Match(line, @"^[\d][\d]*").Value;
            int number = 0;

            Int32.TryParse(line, out number);
            return number;
        }

        #endregion ILabeller Members

        /// <summary>
        /// The name or URL of the team foundation server.  For example http://vstsb2:8080 or vstsb2 if it has already
        /// been registered on the machine.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("server")]
        public string Server { get; set; }

        private string executable;

        /// <summary>
        /// The path to the executable
        /// </summary>
        /// <default>From registry</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable
        {
            get { return executable ?? (executable = ReadTfFromRegistry()); }
            set { executable = value; }
        }

        /// <summary>
        /// The path to the project in source control, for example $\VSTSPlugins
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("project")]
        public string ProjectPath { get; set; }

        /// <summary>
        /// Username that should be used.  Domain cannot be placed here, rather in domain property.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("username", Required = false)]
        public string Username { get; set; }

        /// <summary>
        /// The password in clear text of the domain user to be used.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("password", Required = false)]
        public string Password { get; set; }

        /// <summary>
        ///  The domain of the user to be used.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("domain", Required = false)]
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the major version.
        /// </summary>
        /// <value>The major version number.</value>
        private int _major = 1;

        [ReflectorProperty("major", Required = false)]
        public int Major
        {
            get { return _major; }
            set { _major = value; }
        }

        /// <summary>
        /// Gets or sets the minor version.
        /// </summary>
        /// <value>The minor version number.</value>
        private int _minor = 0;

        [ReflectorProperty("minor", Required = false)]
        public int Minor
        {
            get { return _minor; }
            set { _minor = value; }
        }

        /// <summary>
        /// Gets or sets the build number.
        /// </summary>
        /// <value>The build number.</value>
        private int _build = -1;

        [ReflectorProperty("build", Required = false)]
        public int Build
        {
            get { return _build; }
            set { _build = value; }
        }

        /// <summary>
        /// Prefix for the build label.
        /// </summary>
        [ReflectorProperty("prefix", Required = false)]
        public string Prefix { get; set; }

        #region Memebers

        private const string VS2013_32_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\12.0";
        private const string VS2012_32_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\11.0";
        private const string VS2010_32_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\10.0";
        private const string VS2008_32_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\9.0";
        private const string VS2005_32_REGISTRY_PATH = @"Software\Microsoft\VisualStudio\8.0";
        private const string VS2013_64_REGISTRY_PATH = @"Software\Wow6432Node\Microsoft\VisualStudio\12.0";
        private const string VS2012_64_REGISTRY_PATH = @"Software\Wow6432Node\Microsoft\VisualStudio\11.0";
        private const string VS2010_64_REGISTRY_PATH = @"Software\Wow6432Node\Microsoft\VisualStudio\10.0";
        private const string VS2008_64_REGISTRY_PATH = @"Software\Wow6432Node\Microsoft\VisualStudio\9.0";
        private const string VS2005_64_REGISTRY_PATH = @"Software\Wow6432Node\Microsoft\VisualStudio\8.0";
        private const string VS_REGISTRY_KEY = @"InstallDir";
        private const string TF_EXE = "TF.exe";
        private readonly IRegistry registry = new Registry();

        #endregion Memebers

        private string ReadTfFromRegistry()
        {
            string registryValue = registry.GetLocalMachineSubKeyValue(VS2013_64_REGISTRY_PATH, VS_REGISTRY_KEY);

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2012_64_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2010_64_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2008_64_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2005_64_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2013_32_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2012_32_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2010_32_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2008_32_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                registryValue = registry.GetLocalMachineSubKeyValue(VS2005_32_REGISTRY_PATH, VS_REGISTRY_KEY);
            }

            if (registryValue == null)
            {
                Log.Debug("[TFS] Unable to find TF.exe and it was not defined in Executable Parameter");
                throw new CruiseControlException("Unable to find TF.exe and it was not defined in Executable Parameter");
            }

            return Path.Combine(registryValue, TF_EXE);
        }
    }
}