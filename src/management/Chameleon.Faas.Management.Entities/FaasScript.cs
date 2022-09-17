using System;
using System.ComponentModel.DataAnnotations;

namespace Chameleon.Faas.Management.Entities
{
    /// <summary>
    /// Faas脚本
    /// </summary>
    public class FaasScript : EntityBase
    {
        [Key]
        public Guid Uid { get; set; }
        public int TenantId { get; set; }
        public int Language { get; set; }
        public string ClassName { get; set; }
        public string FunctionName { get; set; }
        public string Script { get; set; }
    }
}
