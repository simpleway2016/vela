//此代码由工具自动生成，请不要随意修改
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VelaAgent.DBModels.Dtos
{
    /// <summary>部署的项目</summary>
    public class Project
    {
        System.Nullable<Int64> _id;
        public virtual System.Nullable<Int64> id
        {
            get
            {
                return _id;
            }
            set
            {
                if ((_id != value))
                {
                    _id = value;
                }
            }
        }
        string? _Name;
        public virtual string? Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if ((_Name != value))
                {
                    _Name = value;
                }
            }
        }
        string? _ExcludeFiles;
        /// <summary>
        /// 排除的文件
        /// json数组形式
        /// </summary>
        public virtual string? ExcludeFiles
        {
            get
            {
                return _ExcludeFiles;
            }
            set
            {
                if ((_ExcludeFiles != value))
                {
                    _ExcludeFiles = value;
                }
            }
        }
        string? _RunCmd;
        /// <summary>部署完毕后运行的命令</summary>
        public virtual string? RunCmd
        {
            get
            {
                return _RunCmd;
            }
            set
            {
                if ((_RunCmd != value))
                {
                    _RunCmd = value;
                }
            }
        }
        string? _PublishPath;
        /// <summary>指定部署路径</summary>
        public virtual string? PublishPath
        {
            get
            {
                return _PublishPath;
            }
            set
            {
                if ((_PublishPath != value))
                {
                    _PublishPath = value;
                }
            }
        }
        string? _ConfigFiles;
        /// <summary>
        /// 配置文件
        /// json数组形式
        /// </summary>
        public virtual string? ConfigFiles
        {
            get
            {
                return _ConfigFiles;
            }
            set
            {
                if ((_ConfigFiles != value))
                {
                    _ConfigFiles = value;
                }
            }
        }
        string? _GitUrl;
        /// <summary>git地址</summary>
        public virtual string? GitUrl
        {
            get
            {
                return _GitUrl;
            }
            set
            {
                if ((_GitUrl != value))
                {
                    _GitUrl = value;
                }
            }
        }
        string? _BranchName;
        /// <summary>git分支名称</summary>
        public virtual string? BranchName
        {
            get
            {
                return _BranchName;
            }
            set
            {
                if ((_BranchName != value))
                {
                    _BranchName = value;
                }
            }
        }
        Int32 _PublishMode;
        /// <summary>1=自动 0=手动</summary>
        public virtual Int32 PublishMode
        {
            get
            {
                return _PublishMode;
            }
            set
            {
                if ((_PublishMode != value))
                {
                    _PublishMode = value;
                }
            }
        }
        string? _BuildCmd;
        /// <summary>编译命令</summary>
        public virtual string? BuildCmd
        {
            get
            {
                return _BuildCmd;
            }
            set
            {
                if ((_BuildCmd != value))
                {
                    _BuildCmd = value;
                }
            }
        }
        System.Nullable<double> _CpuRate;
        /// <summary>cpu占用率</summary>
        public virtual System.Nullable<double> CpuRate
        {
            get
            {
                return _CpuRate;
            }
            set
            {
                if ((_CpuRate != value))
                {
                    _CpuRate = value;
                }
            }
        }
        System.Nullable<double> _MemoryRate;
        /// <summary>内存占用率</summary>
        public virtual System.Nullable<double> MemoryRate
        {
            get
            {
                return _MemoryRate;
            }
            set
            {
                if ((_MemoryRate != value))
                {
                    _MemoryRate = value;
                }
            }
        }
        string? _GitUserName;
        /// <summary>git用户名</summary>
        public virtual string? GitUserName
        {
            get
            {
                return _GitUserName;
            }
            set
            {
                if ((_GitUserName != value))
                {
                    _GitUserName = value;
                }
            }
        }
        string? _GitPwd;
        /// <summary>git密码</summary>
        public virtual string? GitPwd
        {
            get
            {
                return _GitPwd;
            }
            set
            {
                if ((_GitPwd != value))
                {
                    _GitPwd = value;
                }
            }
        }
        string? _Guid;
        public virtual string? Guid
        {
            get
            {
                return _Guid;
            }
            set
            {
                if ((_Guid != value))
                {
                    _Guid = value;
                }
            }
        }
        System.Nullable<Int64> _ProcessId;
        /// <summary>运行后的进程id</summary>
        public virtual System.Nullable<Int64> ProcessId
        {
            get
            {
                return _ProcessId;
            }
            set
            {
                if ((_ProcessId != value))
                {
                    _ProcessId = value;
                }
            }
        }
        string _GitRemote;
        /// <summary>git远程仓库</summary>
        public virtual string GitRemote
        {
            get
            {
                return _GitRemote;
            }
            set
            {
                if ((_GitRemote != value))
                {
                    _GitRemote = value;
                }
            }
        }
        string? _Desc;
        /// <summary>描述</summary>
        public virtual string? Desc
        {
            get
            {
                return _Desc;
            }
            set
            {
                if ((_Desc != value))
                {
                    _Desc = value;
                }
            }
        }
        System.Nullable<Int64> _UserId;
        /// <summary>发布人</summary>
        public virtual System.Nullable<Int64> UserId
        {
            get
            {
                return _UserId;
            }
            set
            {
                if ((_UserId != value))
                {
                    _UserId = value;
                }
            }
        }
        Boolean _IsFirstUpload;
        /// <summary>是否是第一次上传</summary>
        public virtual Boolean IsFirstUpload
        {
            get
            {
                return _IsFirstUpload;
            }
            set
            {
                if ((_IsFirstUpload != value))
                {
                    _IsFirstUpload = value;
                }
            }
        }
        Boolean _IsStopped;
        /// <summary>是否已经手动停止</summary>
        public virtual Boolean IsStopped
        {
            get
            {
                return _IsStopped;
            }
            set
            {
                if ((_IsStopped != value))
                {
                    _IsStopped = value;
                }
            }
        }
        Boolean _FileHasUpdated;
        /// <summary>文件有更新</summary>
        public virtual Boolean FileHasUpdated
        {
            get
            {
                return _FileHasUpdated;
            }
            set
            {
                if ((_FileHasUpdated != value))
                {
                    _FileHasUpdated = value;
                }
            }
        }
        string? _DockerPortMap;
        /// <summary>docker的端口映射</summary>
        public virtual string? DockerPortMap
        {
            get
            {
                return _DockerPortMap;
            }
            set
            {
                if ((_DockerPortMap != value))
                {
                    _DockerPortMap = value;
                }
            }
        }
        string? _DockerContainerId;
        public virtual string? DockerContainerId
        {
            get
            {
                return _DockerContainerId;
            }
            set
            {
                if ((_DockerContainerId != value))
                {
                    _DockerContainerId = value;
                }
            }
        }
        string? _BuildPath;
        public virtual string? BuildPath
        {
            get
            {
                return _BuildPath;
            }
            set
            {
                if ((_BuildPath != value))
                {
                    _BuildPath = value;
                }
            }
        }
        Boolean _IsHostNetwork;
        /// <summary>和主机使用同一个网络</summary>
        public virtual Boolean IsHostNetwork
        {
            get
            {
                return _IsHostNetwork;
            }
            set
            {
                if ((_IsHostNetwork != value))
                {
                    _IsHostNetwork = value;
                }
            }
        }
        string? _ProgramPath;
        /// <summary>程序所在目录</summary>
        public virtual string? ProgramPath
        {
            get
            {
                return _ProgramPath;
            }
            set
            {
                if ((_ProgramPath != value))
                {
                    _ProgramPath = value;
                }
            }
        }
        Boolean _IsNeedBuild;
        /// <summary>是否需要编译</summary>
        public virtual Boolean IsNeedBuild
        {
            get
            {
                return _IsNeedBuild;
            }
            set
            {
                if ((_IsNeedBuild != value))
                {
                    _IsNeedBuild = value;
                }
            }
        }
        string? _MemoryLimit;
        /// <summary>docker内存限制</summary>
        public virtual string? MemoryLimit
        {
            get
            {
                return _MemoryLimit;
            }
            set
            {
                if ((_MemoryLimit != value))
                {
                    _MemoryLimit = value;
                }
            }
        }
        string? _DockerFolderMap;
        /// <summary>docker的文件夹映射</summary>
        public virtual string? DockerFolderMap
        {
            get
            {
                return _DockerFolderMap;
            }
            set
            {
                if ((_DockerFolderMap != value))
                {
                    _DockerFolderMap = value;
                }
            }
        }
        string? _Category;
        /// <summary>分类</summary>
        public virtual string? Category
        {
            get
            {
                return _Category;
            }
            set
            {
                if ((_Category != value))
                {
                    _Category = value;
                }
            }
        }
        Boolean _DeleteNoUseFiles;
        /// <summary>自动删除不用的文件</summary>
        public virtual Boolean DeleteNoUseFiles
        {
            get
            {
                return _DeleteNoUseFiles;
            }
            set
            {
                if ((_DeleteNoUseFiles != value))
                {
                    _DeleteNoUseFiles = value;
                }
            }
        }
        Project_RunTypeEnum _RunType;
        /// <summary>运行方式</summary>
        public virtual Project_RunTypeEnum RunType
        {
            get
            {
                return _RunType;
            }
            set
            {
                if ((_RunType != value))
                {
                    _RunType = value;
                }
            }
        }
        string? _DockerEnvMap;
        /// <summary>docker的环境变量设置</summary>
        public virtual string? DockerEnvMap
        {
            get
            {
                return _DockerEnvMap;
            }
            set
            {
                if ((_DockerEnvMap != value))
                {
                    _DockerEnvMap = value;
                }
            }
        }
    }
    public enum Project_RunTypeEnum:int
    {
        /// <summary>直接运行程序</summary>
        Program=1,
        /// <summary>以OCI容器运行</summary>
        OCIContainer=1<<1,
        /// <summary>以Docker运行</summary>
        Docker=1<<5 | OCIContainer,
    }
    /// <summary>备份的文件</summary>
    public class BackupFile
    {
        System.Nullable<Int64> _id;
        public virtual System.Nullable<Int64> id
        {
            get
            {
                return _id;
            }
            set
            {
                if ((_id != value))
                {
                    _id = value;
                }
            }
        }
        string? _FileName;
        public virtual string? FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                if ((_FileName != value))
                {
                    _FileName = value;
                }
            }
        }
        string? _MD5Value;
        /// <summary>文件的md5值</summary>
        public virtual string? MD5Value
        {
            get
            {
                return _MD5Value;
            }
            set
            {
                if ((_MD5Value != value))
                {
                    _MD5Value = value;
                }
            }
        }
        string? _SavePath;
        /// <summary>保存路径</summary>
        public virtual string? SavePath
        {
            get
            {
                return _SavePath;
            }
            set
            {
                if ((_SavePath != value))
                {
                    _SavePath = value;
                }
            }
        }
        System.Nullable<Int64> _FileLength;
        /// <summary>文件大小</summary>
        public virtual System.Nullable<Int64> FileLength
        {
            get
            {
                return _FileLength;
            }
            set
            {
                if ((_FileLength != value))
                {
                    _FileLength = value;
                }
            }
        }
        System.Nullable<DateTime> _CreateTime;
        public virtual System.Nullable<DateTime> CreateTime
        {
            get
            {
                return _CreateTime;
            }
            set
            {
                if ((_CreateTime != value))
                {
                    _CreateTime = value;
                }
            }
        }
    }
    /// <summary>记录工程包含的文件</summary>
    public class ProjectFiles
    {
        System.Nullable<Int64> _id;
        public virtual System.Nullable<Int64> id
        {
            get
            {
                return _id;
            }
            set
            {
                if ((_id != value))
                {
                    _id = value;
                }
            }
        }
        Int64 _ProjectId;
        public virtual Int64 ProjectId
        {
            get
            {
                return _ProjectId;
            }
            set
            {
                if ((_ProjectId != value))
                {
                    _ProjectId = value;
                }
            }
        }
        string? _FilePath;
        public virtual string? FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                if ((_FilePath != value))
                {
                    _FilePath = value;
                }
            }
        }
        Boolean _IsFolder;
        public virtual Boolean IsFolder
        {
            get
            {
                return _IsFolder;
            }
            set
            {
                if ((_IsFolder != value))
                {
                    _IsFolder = value;
                }
            }
        }
    }
}
