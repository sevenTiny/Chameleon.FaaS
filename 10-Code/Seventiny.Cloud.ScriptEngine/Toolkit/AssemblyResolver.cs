using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace Seventiny.Cloud.ScriptEngine.Toolkit
{
    internal class AssemblyResolver
    {
        private bool _hasInit;
        private readonly List<string> _searchPaths = new List<string>();
        private static readonly AssemblyResolver instance = new AssemblyResolver();

        public static AssemblyResolver Instance
        {
            get
            {
                return instance;
            }
        }

        private AssemblyResolver()
        {
            var baseLibPath = AppDomain.CurrentDomain.BaseDirectory;
            //var baseLibPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.SetupInformation.PrivateBinPath ?? "");
            //string appName = ConfigurationManager.AppSettings[DynamicScriptConst.AppName];
            //string cloudAppName = ConfigurationManager.AppSettings[DynamicScriptConst.CloudAppName];
            //var appLibPath = Path.Combine(DynamicScriptEngineSettings.Instance.AppLibPath,
            //    string.IsNullOrEmpty(cloudAppName) ? DynamicScriptConst.Common : cloudAppName,
            //    string.IsNullOrEmpty(appName) ? "" : appName);

            _searchPaths.Add(baseLibPath);
            //_searchPaths.Add(appLibPath);

            //if (!Directory.Exists(appLibPath))
            //    Directory.CreateDirectory(appLibPath);

            //DirectoryInfo[] subdirs = new DirectoryInfo(appLibPath).GetDirectories("*", SearchOption.AllDirectories);
            //foreach (DirectoryInfo dir in subdirs)
            //{
            //    if (!_searchPaths.Contains(dir.FullName))
            //        _searchPaths.Add(dir.FullName);
            //}
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            _hasInit = true;
        }

        public ReadOnlyCollection<string> SearchPaths
        {
            get { return _searchPaths.AsReadOnly(); }
        }

        public void AddPath(string path)
        {
            if (!_searchPaths.Contains(path))
            {
                _searchPaths.Add(path);
            }
        }

        public bool Init()
        {
            return _hasInit;
        }

        //public void AddAppPath(string applicationName)
        //{
        //    string path = Path.Combine(DynamicScriptEngineSettings.Instance.AppLibPath, applicationName);
        //    if (!_searchPaths.Contains(path))
        //    {
        //        _searchPaths.Add(path);
        //    }
        //}

        //public void AddTanantPath(int tenantId)
        //{
        //    string path = Path.Combine(DynamicScriptEngineSettings.Instance.TenantLibPath, tenantId.ToString());
        //    if (!_searchPaths.Contains(path))
        //    {
        //        _searchPaths.Add(path);
        //    }
        //}

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);
            foreach (string searchPath in SearchPaths)
            {
                string filePath = Path.Combine(searchPath, assemblyName.Name + ".dll");
                if (File.Exists(filePath))
                    return Assembly.LoadFrom(filePath);
            }
            return null;
        }

        ~AssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }

    }
}
