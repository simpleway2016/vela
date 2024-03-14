namespace VelaWeb.Server.Dtos
{
    /// <summary>代码转换任务</summary>
    public class CodeMissionDto
    {
        public virtual System.Nullable<Int64> id
        {
            get;
            set;
        }
        public virtual string Name
        {
            get;
            set;
        }
        public virtual string FullName
        {
            get;
            set;
        }
        public virtual string Path
        {
            get;
            set;
        }
        public virtual CodeMissionDto_TypeEnum Type
        {
            get;
            set;
        }
        /// <summary>代码</summary>
        public virtual Byte[]? Code
        {
            get;
            set;
        }
        /// <summary>转换脚本</summary>
        public virtual Byte[]? Script
        {
            get;
            set;
        }
        /// <summary>上级id</summary>
        public virtual System.Nullable<Int64> ParentId
        {
            get;
            set;
        }
        /// <summary>语言</summary>
        public virtual string Language
        {
            get;
            set;
        }
        /// <summary>拥有者id</summary>
        public virtual Int64 UserId
        {
            get;
            set;
        }
    }
    public enum CodeMissionDto_TypeEnum : int
    {
        Folder = 1,
        Mission = 2
    }

}
