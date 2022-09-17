using Chameleon.Faas.Management.Entities;
using Chameleon.Faas.Management.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chameleon.Faas.Management.Service
{
    public interface IFaasScriptService
    {
        Guid AddFaasScript(FaasScript faasScript);
        Guid UpdateFaasScript(FaasScript faasScript);
    }

    public class FaasScriptService : IFaasScriptService
    {
        private readonly IFaasScriptRepository _faasScriptRepository;

        public FaasScriptService(IFaasScriptRepository faasScriptRepository)
        {
            _faasScriptRepository = faasScriptRepository;
        }

        public Guid AddFaasScript(FaasScript faasScript)
        {
            throw new NotImplementedException();
        }

        public Guid UpdateFaasScript(FaasScript faasScript)
        {
            throw new NotImplementedException();
        }
    }
}
