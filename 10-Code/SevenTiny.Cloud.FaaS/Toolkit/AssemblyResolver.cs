using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace SevenTiny.Cloud.FaaS.Toolkit
{
    internal class AssemblyResolver
    {
        private readonly bool _hasInit;
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
            var baseLibPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Const.DefaultOutPutDllPath);

            _searchPaths.Add(baseLibPath);

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

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);
            foreach (string searchPath in SearchPaths)
            {
                string filePath = Path.Combine(searchPath, assemblyName.Name + ".dll");
                if (File.Exists(filePath))
                    return Assembly.Load(filePath);
            }
            return null;
        }

        ~AssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }
    }
}
