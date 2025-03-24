using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Way.EntityDB.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace VelaAgent.DBModels
{
    /// <summary>部署的项目</summary>
    [TableConfig]
    [Table("project")]
    [Way.EntityDB.DataItemJsonConverter]
    public partial class Project :Way.EntityDB.DataItem
    {
        System.Nullable<Int64> _id;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisallowNull]
        [DesignColumn(TypeName = "bigint")]
        [Column("id")]
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
                    SendPropertyChanging("id",_id,value);
                    _id = value;
                    SendPropertyChanged("id");
                }
            }
        }
        string? _Guid;
        [MaxLength(50)]
        [DesignColumn(TypeName = "varchar")]
        [Column("guid")]
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
                    SendPropertyChanging("Guid",_Guid,value);
                    _Guid = value;
                    SendPropertyChanged("Guid");
                }
            }
        }
        string? _Name;
        [MaxLength(50)]
        [DesignColumn(TypeName = "varchar")]
        [Column("name")]
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
                    SendPropertyChanging("Name",_Name,value);
                    _Name = value;
                    SendPropertyChanged("Name");
                }
            }
        }
        string? _Desc;
        /// <summary>描述</summary>
        [MaxLength(255)]
        [Display(Name = "描述")]
        [DesignColumn(TypeName = "varchar")]
        [Column("desc")]
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
                    SendPropertyChanging("Desc",_Desc,value);
                    _Desc = value;
                    SendPropertyChanged("Desc");
                }
            }
        }
        string? _Category;
        /// <summary>分类</summary>
        [MaxLength(50)]
        [Display(Name = "分类")]
        [DesignColumn(TypeName = "varchar")]
        [Column("category")]
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
                    SendPropertyChanging("Category",_Category,value);
                    _Category = value;
                    SendPropertyChanged("Category");
                }
            }
        }
        string? _LogPath;
        /// <summary>
        /// 自定义控制台日志输出路径
        /// 非docker模式专用
        /// </summary>
        [MaxLength(255)]
        [Display(Name = "自定义控制台日志输出路径 非docker模式专用")]
        [DesignColumn(TypeName = "varchar")]
        [Column("logpath")]
        public virtual string? LogPath
        {
            get
            {
                return _LogPath;
            }
            set
            {
                if ((_LogPath != value))
                {
                    SendPropertyChanging("LogPath",_LogPath,value);
                    _LogPath = value;
                    SendPropertyChanged("LogPath");
                }
            }
        }
        System.Nullable<double> _LogMaxSize=500;
        /// <summary>log文件最大限制,单位M</summary>
        [Display(Name = "log文件最大限制,单位M")]
        [DesignColumn(TypeName = "double")]
        [Column("logmaxsize")]
        public virtual System.Nullable<double> LogMaxSize
        {
            get
            {
                return _LogMaxSize;
            }
            set
            {
                if ((_LogMaxSize != value))
                {
                    SendPropertyChanging("LogMaxSize",_LogMaxSize,value);
                    _LogMaxSize = value;
                    SendPropertyChanged("LogMaxSize");
                }
            }
        }
        string? _PublishPath;
        /// <summary>指定部署路径</summary>
        [MaxLength(100)]
        [Display(Name = "指定部署路径")]
        [DesignColumn(TypeName = "varchar")]
        [Column("publishpath")]
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
                    SendPropertyChanging("PublishPath",_PublishPath,value);
                    _PublishPath = value;
                    SendPropertyChanged("PublishPath");
                }
            }
        }
        string? _ConfigFiles;
        /// <summary>
        /// 配置文件
        /// json数组形式
        /// </summary>
        [MaxLength(100)]
        [Display(Name = "配置文件 json数组形式")]
        [DesignColumn(TypeName = "varchar")]
        [Column("configfiles")]
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
                    SendPropertyChanging("ConfigFiles",_ConfigFiles,value);
                    _ConfigFiles = value;
                    SendPropertyChanged("ConfigFiles");
                }
            }
        }
        string? _ExcludeFiles;
        /// <summary>
        /// 排除的文件
        /// json数组形式
        /// </summary>
        [MaxLength(255)]
        [Display(Name = "排除的文件 json数组形式")]
        [DesignColumn(TypeName = "varchar")]
        [Column("excludefiles")]
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
                    SendPropertyChanging("ExcludeFiles",_ExcludeFiles,value);
                    _ExcludeFiles = value;
                    SendPropertyChanged("ExcludeFiles");
                }
            }
        }
        string? _RunCmd;
        /// <summary>部署完毕后运行的命令</summary>
        [MaxLength(255)]
        [Display(Name = "部署完毕后运行的命令")]
        [DesignColumn(TypeName = "varchar")]
        [Column("runcmd")]
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
                    SendPropertyChanging("RunCmd",_RunCmd,value);
                    _RunCmd = value;
                    SendPropertyChanged("RunCmd");
                }
            }
        }
        string? _GitUrl;
        /// <summary>git地址</summary>
        [MaxLength(255)]
        [Display(Name = "git地址")]
        [DesignColumn(TypeName = "varchar")]
        [Column("giturl")]
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
                    SendPropertyChanging("GitUrl",_GitUrl,value);
                    _GitUrl = value;
                    SendPropertyChanged("GitUrl");
                }
            }
        }
        string _GitRemote="origin";
        /// <summary>git远程仓库</summary>
        [MaxLength(50)]
        [DisallowNull]
        [Display(Name = "git远程仓库")]
        [DesignColumn(TypeName = "varchar")]
        [Column("gitremote")]
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
                    SendPropertyChanging("GitRemote",_GitRemote,value);
                    _GitRemote = value;
                    SendPropertyChanged("GitRemote");
                }
            }
        }
        string? _GitUserName;
        /// <summary>git用户名</summary>
        [MaxLength(255)]
        [Display(Name = "git用户名")]
        [DesignColumn(TypeName = "varchar")]
        [Column("gitusername")]
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
                    SendPropertyChanging("GitUserName",_GitUserName,value);
                    _GitUserName = value;
                    SendPropertyChanged("GitUserName");
                }
            }
        }
        string? _GitPwd;
        /// <summary>git密码</summary>
        [MaxLength(255)]
        [Display(Name = "git密码")]
        [DesignColumn(TypeName = "varchar")]
        [Column("gitpwd")]
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
                    SendPropertyChanging("GitPwd",_GitPwd,value);
                    _GitPwd = value;
                    SendPropertyChanged("GitPwd");
                }
            }
        }
        string? _BranchName;
        /// <summary>git分支名称</summary>
        [MaxLength(50)]
        [Display(Name = "git分支名称")]
        [DesignColumn(TypeName = "varchar")]
        [Column("branchname")]
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
                    SendPropertyChanging("BranchName",_BranchName,value);
                    _BranchName = value;
                    SendPropertyChanged("BranchName");
                }
            }
        }
        Int32 _PublishMode=0;
        /// <summary>1=自动 0=手动</summary>
        [DisallowNull]
        [Display(Name = "1=自动 0=手动")]
        [DesignColumn(TypeName = "int")]
        [Column("publishmode")]
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
                    SendPropertyChanging("PublishMode",_PublishMode,value);
                    _PublishMode = value;
                    SendPropertyChanged("PublishMode");
                }
            }
        }
        Boolean _IsNeedBuild=true;
        /// <summary>是否需要编译</summary>
        [DisallowNull]
        [Display(Name = "是否需要编译")]
        [DesignColumn(TypeName = "bit")]
        [Column("isneedbuild")]
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
                    SendPropertyChanging("IsNeedBuild",_IsNeedBuild,value);
                    _IsNeedBuild = value;
                    SendPropertyChanged("IsNeedBuild");
                }
            }
        }
        string? _ProgramPath;
        /// <summary>程序所在目录</summary>
        [MaxLength(255)]
        [Display(Name = "程序所在目录")]
        [DesignColumn(TypeName = "varchar")]
        [Column("programpath")]
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
                    SendPropertyChanging("ProgramPath",_ProgramPath,value);
                    _ProgramPath = value;
                    SendPropertyChanged("ProgramPath");
                }
            }
        }
        string? _BuildCmd;
        /// <summary>编译命令</summary>
        [MaxLength(255)]
        [Display(Name = "编译命令")]
        [DesignColumn(TypeName = "varchar")]
        [Column("buildcmd")]
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
                    SendPropertyChanging("BuildCmd",_BuildCmd,value);
                    _BuildCmd = value;
                    SendPropertyChanged("BuildCmd");
                }
            }
        }
        string? _BuildPath;
        [MaxLength(255)]
        [DesignColumn(TypeName = "varchar")]
        [Column("buildpath")]
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
                    SendPropertyChanging("BuildPath",_BuildPath,value);
                    _BuildPath = value;
                    SendPropertyChanged("BuildPath");
                }
            }
        }
        System.Nullable<double> _CpuRate;
        /// <summary>cpu占用率</summary>
        [Display(Name = "cpu占用率")]
        [DesignColumn(TypeName = "double")]
        [Column("cpurate")]
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
                    SendPropertyChanging("CpuRate",_CpuRate,value);
                    _CpuRate = value;
                    SendPropertyChanged("CpuRate");
                }
            }
        }
        System.Nullable<double> _MemoryRate;
        /// <summary>内存占用率</summary>
        [Display(Name = "内存占用率")]
        [DesignColumn(TypeName = "double")]
        [Column("memoryrate")]
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
                    SendPropertyChanging("MemoryRate",_MemoryRate,value);
                    _MemoryRate = value;
                    SendPropertyChanged("MemoryRate");
                }
            }
        }
        System.Nullable<Int64> _ProcessId;
        /// <summary>运行后的进程id</summary>
        [Display(Name = "运行后的进程id")]
        [DesignColumn(TypeName = "bigint")]
        [Column("processid")]
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
                    SendPropertyChanging("ProcessId",_ProcessId,value);
                    _ProcessId = value;
                    SendPropertyChanged("ProcessId");
                }
            }
        }
        System.Nullable<Int64> _UserId;
        /// <summary>发布人</summary>
        [Display(Name = "发布人")]
        [DesignColumn(TypeName = "bigint")]
        [Column("userid")]
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
                    SendPropertyChanging("UserId",_UserId,value);
                    _UserId = value;
                    SendPropertyChanged("UserId");
                }
            }
        }
        Boolean _IsFirstUpload=true;
        /// <summary>是否是第一次上传</summary>
        [DisallowNull]
        [Display(Name = "是否是第一次上传")]
        [DesignColumn(TypeName = "bit")]
        [Column("isfirstupload")]
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
                    SendPropertyChanging("IsFirstUpload",_IsFirstUpload,value);
                    _IsFirstUpload = value;
                    SendPropertyChanged("IsFirstUpload");
                }
            }
        }
        Boolean _IsStopped=false;
        /// <summary>是否已经手动停止</summary>
        [DisallowNull]
        [Display(Name = "是否已经手动停止")]
        [DesignColumn(TypeName = "bit")]
        [Column("isstopped")]
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
                    SendPropertyChanging("IsStopped",_IsStopped,value);
                    _IsStopped = value;
                    SendPropertyChanged("IsStopped");
                }
            }
        }
        Project_RunTypeEnum _RunType=(Project_RunTypeEnum)(1);
        /// <summary>运行方式</summary>
        [DisallowNull]
        [Display(Name = "运行方式")]
        [DesignColumn(TypeName = "int")]
        [Column("runtype")]
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
                    SendPropertyChanging("RunType",_RunType,value);
                    _RunType = value;
                    SendPropertyChanged("RunType");
                }
            }
        }
        Boolean _IsHostNetwork=false;
        /// <summary>和主机使用同一个网络</summary>
        [DisallowNull]
        [Display(Name = "和主机使用同一个网络")]
        [DesignColumn(TypeName = "bit")]
        [Column("ishostnetwork")]
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
                    SendPropertyChanging("IsHostNetwork",_IsHostNetwork,value);
                    _IsHostNetwork = value;
                    SendPropertyChanged("IsHostNetwork");
                }
            }
        }
        string? _DockerPortMap;
        /// <summary>docker的端口映射</summary>
        [MaxLength(50)]
        [Display(Name = "docker的端口映射")]
        [DesignColumn(TypeName = "varchar")]
        [Column("dockerportmap")]
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
                    SendPropertyChanging("DockerPortMap",_DockerPortMap,value);
                    _DockerPortMap = value;
                    SendPropertyChanged("DockerPortMap");
                }
            }
        }
        string? _DockerFolderMap;
        /// <summary>docker的文件夹映射</summary>
        [MaxLength(512)]
        [Display(Name = "docker的文件夹映射")]
        [DesignColumn(TypeName = "varchar")]
        [Column("dockerfoldermap")]
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
                    SendPropertyChanging("DockerFolderMap",_DockerFolderMap,value);
                    _DockerFolderMap = value;
                    SendPropertyChanged("DockerFolderMap");
                }
            }
        }
        string? _DockerEnvMap;
        /// <summary>docker的环境变量设置</summary>
        [MaxLength(512)]
        [Display(Name = "docker的环境变量设置")]
        [DesignColumn(TypeName = "varchar")]
        [Column("dockerenvmap")]
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
                    SendPropertyChanging("DockerEnvMap",_DockerEnvMap,value);
                    _DockerEnvMap = value;
                    SendPropertyChanged("DockerEnvMap");
                }
            }
        }
        string? _DockerContainerId;
        [MaxLength(255)]
        [DesignColumn(TypeName = "varchar")]
        [Column("dockercontainerid")]
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
                    SendPropertyChanging("DockerContainerId",_DockerContainerId,value);
                    _DockerContainerId = value;
                    SendPropertyChanged("DockerContainerId");
                }
            }
        }
        string? _MemoryLimit;
        /// <summary>docker内存限制</summary>
        [MaxLength(50)]
        [Display(Name = "docker内存限制")]
        [DesignColumn(TypeName = "varchar")]
        [Column("memorylimit")]
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
                    SendPropertyChanging("MemoryLimit",_MemoryLimit,value);
                    _MemoryLimit = value;
                    SendPropertyChanged("MemoryLimit");
                }
            }
        }
        Boolean _FileHasUpdated=false;
        /// <summary>文件有更新</summary>
        [DisallowNull]
        [Display(Name = "文件有更新")]
        [DesignColumn(TypeName = "bit")]
        [Column("filehasupdated")]
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
                    SendPropertyChanging("FileHasUpdated",_FileHasUpdated,value);
                    _FileHasUpdated = value;
                    SendPropertyChanged("FileHasUpdated");
                }
            }
        }
        Boolean _DeleteNoUseFiles=true;
        /// <summary>自动删除不用的文件</summary>
        [DisallowNull]
        [Display(Name = "自动删除不用的文件")]
        [DesignColumn(TypeName = "bit")]
        [Column("deletenousefiles")]
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
                    SendPropertyChanging("DeleteNoUseFiles",_DeleteNoUseFiles,value);
                    _DeleteNoUseFiles = value;
                    SendPropertyChanged("DeleteNoUseFiles");
                }
            }
        }
        System.Nullable<Int32> _BackupCount;
        /// <summary>备份数量，null表示不限制</summary>
        [Display(Name = "备份数量，null表示不限制")]
        [DesignColumn(TypeName = "int")]
        [Column("backupcount")]
        public virtual System.Nullable<Int32> BackupCount
        {
            get
            {
                return _BackupCount;
            }
            set
            {
                if ((_BackupCount != value))
                {
                    SendPropertyChanging("BackupCount",_BackupCount,value);
                    _BackupCount = value;
                    SendPropertyChanged("BackupCount");
                }
            }
        }
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param>指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<Project, bool>> exp)
        {
            base.SetValue<Project>(exp);
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
    [TableConfig]
    [Table("backupfile")]
    [Way.EntityDB.DataItemJsonConverter]
    public partial class BackupFile :Way.EntityDB.DataItem
    {
        System.Nullable<Int64> _id;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisallowNull]
        [DesignColumn(TypeName = "bigint")]
        [Column("id")]
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
                    SendPropertyChanging("id",_id,value);
                    _id = value;
                    SendPropertyChanged("id");
                }
            }
        }
        string? _FileName;
        [MaxLength(100)]
        [DesignColumn(TypeName = "varchar")]
        [Column("filename")]
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
                    SendPropertyChanging("FileName",_FileName,value);
                    _FileName = value;
                    SendPropertyChanged("FileName");
                }
            }
        }
        string? _MD5Value;
        /// <summary>文件的md5值</summary>
        [MaxLength(50)]
        [Display(Name = "文件的md5值")]
        [DesignColumn(TypeName = "varchar")]
        [Column("md5value")]
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
                    SendPropertyChanging("MD5Value",_MD5Value,value);
                    _MD5Value = value;
                    SendPropertyChanged("MD5Value");
                }
            }
        }
        string? _SavePath;
        /// <summary>保存路径</summary>
        [MaxLength(255)]
        [Display(Name = "保存路径")]
        [DesignColumn(TypeName = "varchar")]
        [Column("savepath")]
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
                    SendPropertyChanging("SavePath",_SavePath,value);
                    _SavePath = value;
                    SendPropertyChanged("SavePath");
                }
            }
        }
        System.Nullable<Int64> _FileLength;
        /// <summary>文件大小</summary>
        [Display(Name = "文件大小")]
        [DesignColumn(TypeName = "bigint")]
        [Column("filelength")]
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
                    SendPropertyChanging("FileLength",_FileLength,value);
                    _FileLength = value;
                    SendPropertyChanged("FileLength");
                }
            }
        }
        System.Nullable<DateTime> _CreateTime;
        [DesignColumn(TypeName = "datetime")]
        [Column("createtime")]
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
                    SendPropertyChanging("CreateTime",_CreateTime,value);
                    _CreateTime = value;
                    SendPropertyChanged("CreateTime");
                }
            }
        }
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param>指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<BackupFile, bool>> exp)
        {
            base.SetValue<BackupFile>(exp);
        }
    }
    /// <summary>记录工程包含的文件</summary>
    [TableConfig]
    [Table("projectfiles")]
    [Way.EntityDB.DataItemJsonConverter]
    public partial class ProjectFiles :Way.EntityDB.DataItem
    {
        System.Nullable<Int64> _id;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisallowNull]
        [DesignColumn(TypeName = "bigint")]
        [Column("id")]
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
                    SendPropertyChanging("id",_id,value);
                    _id = value;
                    SendPropertyChanged("id");
                }
            }
        }
        Int64 _ProjectId;
        [DisallowNull]
        [DesignColumn(TypeName = "bigint")]
        [Column("projectid")]
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
                    SendPropertyChanging("ProjectId",_ProjectId,value);
                    _ProjectId = value;
                    SendPropertyChanged("ProjectId");
                }
            }
        }
        string? _FilePath;
        [MaxLength(255)]
        [DesignColumn(TypeName = "varchar")]
        [Column("filepath")]
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
                    SendPropertyChanging("FilePath",_FilePath,value);
                    _FilePath = value;
                    SendPropertyChanged("FilePath");
                }
            }
        }
        Boolean _IsFolder=false;
        [DisallowNull]
        [DesignColumn(TypeName = "bit")]
        [Column("isfolder")]
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
                    SendPropertyChanging("IsFolder",_IsFolder,value);
                    _IsFolder = value;
                    SendPropertyChanged("IsFolder");
                }
            }
        }
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param>指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<ProjectFiles, bool>> exp)
        {
            base.SetValue<ProjectFiles>(exp);
        }
    }
}

namespace VelaAgent.DBModels.DB
{
    public class VelaAgent : Way.EntityDB.DBContext
    {
         public VelaAgent(string connection, Way.EntityDB.DatabaseType dbType , bool upgradeDatabase = true): base(connection, dbType , upgradeDatabase)
        {
            if (!setEvented)
            {
                lock (lockObj)
                {
                    if (!setEvented)
                    {
                        setEvented = true;
                        Way.EntityDB.DBContext.BeforeDelete += Database_BeforeDelete;
                    }
                }
            }
        }
        static object lockObj = new object();
        static bool setEvented = false;
        static void Database_BeforeDelete(object sender, Way.EntityDB.DatabaseModifyEventArg e)
        {
             var db =  sender as VelaAgent;
            if (db == null) return;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().HasKey(m => m.id);
            modelBuilder.Entity<BackupFile>().HasKey(m => m.id);
            modelBuilder.Entity<ProjectFiles>().HasKey(m => m.id);
        }
        System.Linq.IQueryable<Project> _Project;
        /// <summary>部署的项目</summary>
        public virtual System.Linq.IQueryable<Project> Project
        {
            get
            {
                if (_Project == null)
                {
                    _Project = this.Set<Project>();
                }
                return _Project;
            }
        }
        System.Linq.IQueryable<BackupFile> _BackupFile;
        /// <summary>备份的文件</summary>
        public virtual System.Linq.IQueryable<BackupFile> BackupFile
        {
            get
            {
                if (_BackupFile == null)
                {
                    _BackupFile = this.Set<BackupFile>();
                }
                return _BackupFile;
            }
        }
        System.Linq.IQueryable<ProjectFiles> _ProjectFiles;
        /// <summary>记录工程包含的文件</summary>
        public virtual System.Linq.IQueryable<ProjectFiles> ProjectFiles
        {
            get
            {
                if (_ProjectFiles == null)
                {
                    _ProjectFiles = this.Set<ProjectFiles>();
                }
                return _ProjectFiles;
            }
        }
        protected override string GetDesignString()
        {
            var result = new StringBuilder();
            result.Append("\r\n");
            result.Append("H4sIAAAAAAAAA+1dW3PTSBb+K5SfA9iObWJqeIAEdlxLQioZdrdqzYNiK0aDLWUkmcCwqQpLIFySOBACBMJWYANkuDiBmh1CLuTPuCXnaf7Cti6xpbZ8VdtuxXqB6NaWvv76nNPn0n3D8xM1kqQFz8l/3tD+HKBStOekZ/iXJCPSni7PEDeuXY2IdEr7S7+FicPLf6OS");
            result.Append("aXjgm+gqnBevj9HFK55enqZEWm37dExkONZjuDfGsSLNiobbb0S194h6TsI/mTj839/d3RX1xKgx5Wl4HPXs31qTd7/Iz6f2X32TX2SjHnhdaVG9OMhzP9MxUT3ZR4nUCCXQkT54KeCFZ5jzXOwKPPBOwINeLplOsUJU+Sz9x3xh9bbir7HpZNLYvHIXPI4Ip9MiF2Fj");
            result.Append("PJ2CXwAviXyaVtqk2AH4CDwxSiUF5Ux85CcIifrwCJNgWO3V9HN682fZdKqPHmVYw7kkzSbEy8XjOD1KpZOiilTxrIqW+n0aThFh8K/qof4+HB+nefXTvOrVPkaI8UyKYSmR4w1v2ZukBEH/Sr1p9dQ5+LfhNOy8IlK+ikhpf1hiVfjRAlj62xqwukrxscsUXy9YUU/Q");
            result.Append("qz5TJ1wHr1TEy4cdLz/CY2nu0f7SKuSx9GQ6t/2Hmcdnr8WS6Th9joHjsx0w+oNBPDj6seNoLQ9AdkZaXwTzc/m9+fyrGQgreLib2141wzqUZntT5cawMwDtxg5oAAEU3FyWPr2uiOOwyI05HcgAFiAvKa30/aOXY0eZhKpMtDO6yL0o0HxExXnCqPriumoyqdETsDFF");
            result.Append("5Q6LUGeqSqqK4vU3V/GW8GIVyqndMgLrDBW7kh5T5FXjujfoWN0baLHuDVVESumFdulfn7dWBWzCrBUK+ASqgFUaQz6n4kEwuWPmc39fUH93gm2YKhDi1709CIS5vZfg07P813XwfQrRENRVepBSPohkFVEFQPy6NmzJQbD6DmxkzAAqg/i8/k0NQ9iY2It68MDXLA17");
            result.Append("Q2n2Isv8or7XQXsRoTeZFkSap+PGX1E1jdKe+qhROF4qwl3op8bbLQoMvO0aeYC23DpTo7uCqXGZYhN1mRoXkvGCuyGKztoH6PHyF1l63GA6KJ8fU389jpyN00laLDlbiUX62KmxU6wJ1LruCJDYHUUxFzR7UaCYm5kG2efaTM1KWwymR5KMcLldCqN2o6Uls93O5XWQ");
            result.Append("cF77EF7v356Vd7P6fCjK/ixwrLS4IW9Pgd3XYAfR6Foftc2lg43keKyiziV5iHCSo47KBCOC5Q3wctJM578w4kU+SbZ9X4XJJ3Db90HUOalgd/eO9HgdzM/K7zYQnwlPsbHLTveU92AHEXU8+U7lp9+D+2tH/Kekew/gH5bGQz8Xrx1HCxcRtokSJKjWUO0YhrFjGEQw");
            result.Append("lJ5BCr7dX57Mv70p7zzJrz80gxgRBmg6fibNJGv36lr62bCB6K0XRB92P1swhKCoIWflEVeRc7pL3IfH6da5qv0E4aoddYGC3QVwb1a+/4c0eRPar95TWuznCJS4avgnt/kJDfxQYrp287W5YrZ+CYHdIxpEPaKxsTSYXZEfr8lz04j5P5YegjSyISDiXHpEj680w5tX");
            result.Append("DTzs3tAg6g0Fd26DT8/K4NdPpzj+urMhxOcR7Uz52kOifLXujsNkz9YtaLHYs10HUWWta+OwB8ZoXmRUMt4oeU2lR3kl3kElTZb4xAQJhnFb1R4Ww7hqdxx8rGVHqBMD2BWdIKXCJEqp4hAIod75BCNChSvd/QrmZ0t9PBAp10WBWC4h1BGs+HnW78grN0vwGxxvy5wQ");
            result.Append("F3R4PBOda7H4vIQLA9Tbq+lALaQh3ZsEy2sHOQrfLDwe5Kd4tGZWUw/B3dTLEDbMD5GgqFRGQYKgQEMb0qdV8OmplP2f/PgzyL6G5oP8YT23uwdNCTNjL4yzND9M81dp3sma0IcnWtTBqrBSvjIJDDdLY70Ti7ZcumwGsjP469LXHn0dnwPnCvPiYMAzJTxkDCc8rTCE");
            result.Append("+u40QxnMz0FbOb/3Ql57oItoY7MxWhAidiR3k5OoW8fUDpXbhCcVhtAkgwQj5veWIZlz2wtga6HEozREp7g6gmAWzmjM4lehqzJG6vVLu8y2yWzCMwlDJekGy2tg4X1u8738Igt2F3ObD6R77xRXR9m0ml4u7oAyomoiPOgS3R7RicyrqRb3PZz0j3qOHa87AIllAHTZ");
            result.Append("jgertO2MGKTPkakSzglNYlMOLUmVKLy4ZWxeserMWRLkxzfxKeeWJEfU1AEdIZcIT444UZIflMnk95DCjT5aiDma9G4Jl82VVwiP6vegUf0W+HSV4seRdnnJ/LYzuwt5l3+nR45pYBzrO6MkYSaFY30iJxw7nVDB6GTaExmjbiwE4g6RJg+RQ0R7wgPXPWg0D2Qegs1b");
            result.Append("ua0tM4kPQGqYv+2NhNgncKdbLYcogt3xpWxuANuC4IQHsHvQfM/c5v3czgqYn9OqUEBmPbf9RuMr4gim2KE0e3oUdsHFsSRH1S7Fm1uk7auXt64Qt8txIsPZ1Ry7jTM/IpxjeEEkivV1S2s8rO+q4mTUb7V0MVoIkInST7N81Ncxzkk/kQH1useWNqTgv/LHj7nNSenj");
            result.Append("K220kT+w2qROqg2sWkZJ57jw/URG4w3jAV2TQVcxX7/I2xmt4FqbG6DjQSmtGaNJGQv1KxnXq2+T2ETGzA3ELlk9WCP28lqci12heTjFtbagoOaPsH3qPY4lt7t0hk1yEx54DZesGauVoC7fk178Lj1BIrDKKpo/UsLFMQiOgyW2mxhoc1lqwuOwYTSPRhPU8vMp+cM6");
            result.Append("yPxXerYCNqaQ5AL1lkGOF/upMSfX2LjststuIsOtBnabQ6h6JyJEhn0hUpBq9iJP7U+Z8eMrWu9QNhMZRW2wfrLTV83AVwt/iAhOZHjVIK7NaQKouD4kzGy9mHYrPpraDYdIQBAeng6XLEf0aCa3uS0tb2kpcWAeHk4qbN59KG8vo16mHzlBHKDFcY6/4tgJuetlsklx");
            result.Append("IqPTBoqj6f3y2gOwldFMNk1Im2kNG03wVIowkdyAXmw9s+tbdFVN2Zp/e8R3Cl5ERYvzl2BtzV54DhMWRIbbDcICXZnBeZto1B1Qd5cot0vqStFxDLsyozHAfHYDKi3w9Y2iyWZug/kPZXZo1hlf3H+skT2aw+YZDjqHLLtCGgF7NIeNxG/+Hs3hnopI6b1Rh1MUG2I1");
            result.Append("5+ObEGvWgnJGyMIVIVOYS74dVgU0fOmvGPfENZARFZz2N6/VuqyNArklWR1FsVrO1Cje0bxdbPH1ewv7pyWJCY33T0H1er3mFAVUPkWEc1wybjfFBp9Et5qmVJFO5GWQOZHRgZZkJeCWOC3mucNJ3mVzNxRtmDecMezIYUF0OgOkvXWyjrYp2f7SPLiLzKi0HcnOQxKJ");
            result.Append("js7TcddhtEnsSpkN9qf8ATQzEszdAZkv2hxffre9v7Saz74Cdx+Vpkj2qcgP06LIsIkGJ/1wYFTO8yF40h/wGqnf7Ek/RKpkXW09I+Q38HkRzM/+uTMD3v77yK/MmLmvzl6rXYJYgNh0EWLGsemuAAhkSb3g3kt5cQlMr4DV35B8Seo6GVXc9cPWLGdAC0Vfc3NeNBNL");
            result.Append("kWTWah25Xlazmx35qADTZTSTom2ocSURXTxoohneuEBlLpGXbNuAr8hak9vzQfX3BXVU8fu2zuu92Ebbo7k5JbYGoEsmLGSy17JRuLWRps3NC7G2dUvYan0bTtK2ENEKwXPtG+uedFTBqUXfRXR1sd/rR1O/CyU9hYzv8lU9muOsTXU9+Eoh3J3QbHKc6EJjyPGSBeXu");
            result.Append("3pE/byOZyvBjExx/3cmuLzfZxSaRiS4qhkRG3Qj56ffK8g93V/aXVnObs/LjtTLZMpr6G+AgfMUwSvuDHXXnc3W7623Zo3iQ6BJjSHHrTdGkJ9/ATsbM6aE0q1GwcSrX4CyLeo4fl1/8Ls3py3lpGdbRKKunUp/ydcGD48dz228u9EZA9htYWtOXrYiy8EyhYPSU74cf");
            result.Append("ivdqBlThRu1QuSV45F9HjI91qS/YhKWGyPOwOGwkERn/ayyjvaOXYnFruyzITWR1s0FNoKWJxZUo5tbB61sg82x/OpPPfpd3s1Yz17Ps1XYtR+HzY5q2Ytlqp4wAPwwOAnxIY8ko6LK1p47q7eiQFfmCZFeeh3wlhaWrcCTsSosbUOb8uTOjwJV/tSavbsE5mVWmjc4D");
            result.Append("Ls3aybRpXqTXF/RVmYa5ngabFCe7drrHhy46qXgass9z3+5Jc+8gn0FmQ3r6Buw9zX9fANNb+a/r4PsUnMfsv/yPph+ktVdwwpbbXCjZe+Q8lyC97KEq/925k036k11X3eND50NJLnGwgN8kWH2nCfUuMLuY253tL+F3P3VtmPm1ds9EE2W74lxGp0RV6e2uDWWT3kRW");
            result.Append("Ale29AkgfZxLj+iZFw7mfTU7v3Iqvb08erKGCTzU310dKMXfPxguSvauAgY8Hr4O3zd1LMKKoYD6S6a79TFUev+wyDNsovSB4kCq/RnTR5V9tUvKZylXh2lRf5AOBL2jVIw66g/Fu48G/N3ho1TM6zsaD4fC4R6vlwoE4IP/B+FzemagvgAA");
            return result.ToString();
        }
    }
}

/*<design>
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
<design>*/

