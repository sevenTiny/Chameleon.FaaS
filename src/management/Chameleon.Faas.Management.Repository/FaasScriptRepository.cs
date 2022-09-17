using Chameleon.Faas.Management.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chameleon.Faas.Management.Repository
{
    public interface IFaasScriptRepository : IRepositoryBase<FaasScript>
    {

    }

    public class FaasScriptRepository : RepositoryBase<FaasScript>, IFaasScriptRepository
    {
        public FaasScriptRepository(FaasDbContext faasDbContext) : base(faasDbContext)
        {

        }
    }
}
