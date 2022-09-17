using System;

namespace Chameleon.Faas.Management.Entities
{
    public abstract class EntityBase
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDeleted { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public int CreateBy { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public int ModifyBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }
    }
}
