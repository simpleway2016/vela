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
            result.Append("H4sIAAAAAAAAA+1dW3PTSBb+Kyk/h2A7tompyQPEsONaCKlk2N2qNQ+KrRgNtuSRZDIMm6qwXBIgicMlLIFQFdgAGSBOoGaHkMvkz7gt52n+wrYusaW2fFXbbsV6gejWlr7++pzuc77uvun6gRpP0ILr9D9vqn8OU0naddo19lOCEWlXr2uUm1SvhkU6qf6l3cLE4OW/");
            result.Append("UYk0PPBM9RbPizdSdOmKa4inKZFWyj4TFRmOdenujXKsSLOi7vabEfU9Iq7T8E8mBv/39vf3RlxRKiU/DY8jrsPb69L+F+nFncPX36SX2YgLXpdLVC6O8NyPdFRUToYokRqnBDocgpd8bniGucBFr8ED9xQ8GOIS6SQrROTP0n7ME1RuK/0am04k9MXLd8HjsHAmLXJh");
            result.Append("NsrTSfgF8JLIp2m5TIodho/AExNUQpDPxMZ/gJAoD48zcYZVX007pxV/jk0nQ/QEw+rOJWg2Ll4tHcfoCSqdEBWkSmcVtJTvU3EKCyN/VQ619+H4GM0rn+ZWroYYIcozSYalRI7XveVQghIE7Su1opVT5+HfutOw8kpIeaoipf5hilXxR4tgaW+rw+o6xUevUnyjYEVc");
            result.Append("frfyTINwHb1SCS8Pdry8CI/zC48Pl9cgj/PPZnK7vxt5fO7naCIdo88zsH12Akav348HRy92HM3tAcjO5TeXwOJC4WCx8HoOwgoe7ed214ywjqbZoWSlNmwPQPuxA+pDAAW3VvIbb6riOCZyKbsD6cMC5BW5lNA/hjh2gokrzkQ9o5ncywLNhxWcp/SuL6a5JoMbPQUL");
            result.Append("k13umAh9puKkajheb2sdbxkv1qCd2q9gsM5S0WvplGyvmve9ftv6Xl+bfW+gKlJyLXTK/3rc9TpgA2btcMCnUAes0BjyORnzg+k9I58vhvzauxPch6kBIX7fO4BAmDt4BTaeF75ugj/uIB6Cuk6PUPIHkewiagCI39cGTTkI1t6DrYwRQLkRX9C+qWkImzN7ERce+Frl");
            result.Append("YW/KxV5mmZ+U9zoqLywMJdKCSPN0TP8riqeRy1Me1RvHKyW4i/XUfLklg4G3XD0P0JLb19Xor9LVuEqx8Ya6GpcSsWK4IYKO2ofpycoXWXpS13WQPz+q/HoMORujE7RYdrYai7S2U2elmBOofdXhI7E6SmbOb4yiQDM3NwOyL9SRmpm3GEmPJxjhaqccRv2dlraMdruX");
            result.Append("137Cee1BeH14d17az2rjoQj7o8Cx+aUtafcO2H8D9hCPrtZRx0I62EiOp1fUvSQPEE5yNFAZZ0SwsgVeTRvp/BdGvMwnyO7f12DyKdz9ez8anJSxm72Xf7oJFuel91tIzISn2OhVu0fKB7CDiAaePIOFmQ/gwXqPdzB//yH8w7TzcJGL1Y+jSYgI20AJElQtqH4Mg9gx");
            result.Append("9CMY5p9DCr47XJkuvLsl7T0rbD4yghgWhmk6djbNJOqP6prG2bCB6G4URA/2OJs/gKCoImcWEVeQs3tI3IMn6Na9rv0U4a4dDYGC/Sfg/rz04Pf89C3Yf3UPqrmfHmhxlfRPbnsDTfxQYrr+7mtrzWzjFgJ7RNSPRkSjqTSYX5WerksLM0j3P5UehTSyYCBiXHpcy6+0");
            result.Append("IppXCzzs0VA/Gg0F9+6CjecV8LtIJzn+hr0hxBcR7U77OkCifTWvjuPUn23Y0GLpz/YeZZXVqo3BGkjRvMgoZLxZ9ppyjfJyvoNKGHriU1MkdIw76vawdIxrVsfRx5pWhDIwgFXRDVYqSKKVKjWBABqdjzMidLj52a9gcb48xgORckIUSM8lgAaC5TjP5j1p9VYZfiOT");
            result.Append("HRkT4oIOT2Sie3ssHjfhxgCN9qo+UE1p5O9Pg5X1I43CN5OIB/kSj/aMahohuCO9DGDD/BgZimrTKEgwFGhqI7+xBjb+k8/+T3r6GWTfwO6D9HEzt38AuxJGxl6aZGl+jOav07ydPaEHT7aoi11hNb0yCQw3WmOtEkt9uXRFBbI9+OvQ1xp9ba+Bc4x5qTHgGRIeM4YT");
            result.Append("LisMoLE7taMMFhdgX7lw8FJaf6iZaH2xUVoQwlYsd4tF1O1japfabcJFhQFUZBBnxMLBCiRzbvcJ2HlSFlEapZNcA0kwk2A0ZvMr01VuI43GpR1mW2Q24UrCQJncYGUdPPmQ2/4gvcyC/aXc9sP8/fdyqKOirGaIi9lgGlEtE+53iG6N6ETqamrlfY8n/SOuvpMNJyCx");
            result.Append("NIBey/lghbbdkYP02FIqYZ/UJDbn0BapRPHFTXPzcq/OqJIgP7+Jzzm3RRxRVwV0hV0iXBxxqkwflMkUDpCJGyFaiNqa9M4ULosrrxCe1R9As/ptiOnKkx/HOxUl81pWdhd1l3+nx/tUMPpCZ2URZkLoC4mc0HcmroDRzbQnMkfdXArEaSItbiLHiPaEJ64H0GweyDwC");
            result.Append("27dzOztGEh+B1DR/O5sJsU7gbu+1HKMMdtdPZXMS2CYEJzyBPYDqPXPbD3J7q2BxQZ2FAjKbud23Kl+RQDDFjqbZMxOwCi6nEhxVvxVv7SRtT6O8dYy4VY4Tmc6uFdhtnvlh4TzDCyJRrG/YWuNhfW+NIKN2q2mI0cSATJV/mumjnq4JTnqJTKg33LbUJgX/lT59ym1P");
            result.Append("5z+9Vlsb+Q2rQ+6kVsOqp5V0TwjfS2Q2Xtce0DUZNBfz9Yu0m1EnXKtjA7Q9yFNrUjQpbaFxJ+NE9S0Sm8icuY7YZasHq8ReWY9x0Ws0D4e45j0o6PnDbEi5x7bkdpbOsEhuwhOvwbI1Y9UpqCv38y9/yz9DMrDyKprfU8LlFATHxhbbEQZaXJaa8DxsENXRqIZaenFH");
            result.Append("+rgJMv/NP18FW3cQcYFyywjHixeplJ3n2DjstspuItOtOnYbU6haJSJEhnUhUpBq1jJPnZfMePFNWu9SNhOZRW1y/mS3r5qBby78MSI4kelVnbk2ygRQc31MmNl+M+3M+GhpNRwjA0F4ejpYthzR47nc9m5+ZUeVxIFFeDgts3n/kbS7gkaZvucEcZgWJzn+mm0H5E6U");
            result.Append("ySLFicxO6yiOyvul9YdgJ6N22VQjbaQ1LDTOU0nCTHITfrH9zG5s0VVFsrX4rsczCC+ipsX+S7C2Zy88mxkLItPtOmOBrsxgv000Gk6oO0uUWyV1tew4hl2Z0RxgIbsFnRb4+lb2ZHN3weLHCjs0a4wv7T/WzB7NQeMIBx1DVlwhjYA9moN64rd+j+bgQFWktNpoICiK");
            result.Append("DbG69fgGxFq1oJwesmBVyGTmkt8PqwEaPvkrxj1xdWREDaf1zWvVKuugQW6LqqNkVit1NUp3tG4XW3z13sb6aYswofn6Kbpet9soUUDtU1g4zyViViU2+Cy62TClhnUiT0FmR0b72qJKwG1x2sxzm5O81+JuKGozb1oxbMtmQbScAdLeXKyjbkp2uLwIZpERlboj2QVI");
            result.Append("ItHWOh1nHUaLxK6mbLA+5PehykiwcA9kvqhjfOn97uHyWiH7Gsw+LpdIhhTkx2hRZNh4k4N+2DCq63wIHvT73Hrqt3rQD5EqW1dbU4T8Cj4vgcX5P/fmwLt/9/zCpIx1de7n+i2ICYgtNyFGHFseCoBAls0XPHglLS2DmVWw9iuil6RukDGLu3HYWhUMaKPpa63mRe1i");
            result.Append("yZbM3K0j1yt6dmMgHzVgmo1mkrQFNy4L0cWjIloRjfNV5xJ5YtsmYkXmntxaDOpiyK+hij+2dUGrxQ72PVqrKbHUAB0yYSGTtZL1xq2DNG2tLsS8r1vGVvPbcJK2jYhWSZ6r39jwoKMGTm36LqJnF3vdXlT6XZzSU1R8V57VowbOOjSvB99UCGcnNIscJ3qiMeR42YJy");
            result.Append("s/ekz7uIUhl+bJzjb9g59OWIXSwSmehJxZDIaBihMPNBXv5hdvVweS23PS89Xa+gllHd3zAH4SulUTqf7GhYz9XvrLdljeJ+oqcYQ4qbb4qWf/YN7GWMnB5NsyoFm6dyHcGyiOvkSenlb/kFbTkvVWEdibCalHrQ0wsPTp7M7b69NBQG2W9geV1btiLCwjPFCaODnu++");
            result.Append("K92rdqCKN6qH8i3+nn/16B/rVV6wBUsNkRdhsVlLIjL/15yivauXYnHmdpmQm8jZzTo3gU5NLK1EsbAJ3twGmeeHM5lC9g9pP2s2cj3HXu/UchQeL6ZhK5atdioY8OMQIMCHNBZFQa+lPXWUaEeXrMjnJ3vmecBTNrF0DbaE/fzSFrQ5f+7NyXAVXq9LaztwTGamtNF4");
            result.Append("wKVZK0qb1mV6PX5PjWGYE2moTnF4qL27QvLS7x9RXRauyBUHj8duwPdN9oVZMeBTfslwt8b/8vvHRJ5h4+UPlBpB/c8YPqriq12RP0u+OkaL2oO0z++eoKLUCW8g1n/C5+0PnqCibs+JWDAQDA643ZTPBx/8P+ZDctCbtQAA");
            return result.ToString();
        }
    }
}

/*<design>
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
U3Y+Frqx9H/9DHnUtjcAAA==
<design>*/

