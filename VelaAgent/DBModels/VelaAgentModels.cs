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
    public class Project :Way.EntityDB.DataItem
    {
        System.Nullable<Int64> _id;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisallowNull]
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
        string? _Name;
        [MaxLength(50)]
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
        string? _ExcludeFiles;
        /// <summary>
        /// 排除的文件
        /// json数组形式
        /// </summary>
        [MaxLength(255)]
        [Display(Name = "排除的文件 json数组形式")]
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
        string? _PublishPath;
        /// <summary>指定部署路径</summary>
        [MaxLength(100)]
        [Display(Name = "指定部署路径")]
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
        string? _GitUrl;
        /// <summary>git地址</summary>
        [MaxLength(255)]
        [Display(Name = "git地址")]
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
        string? _BranchName;
        /// <summary>git分支名称</summary>
        [MaxLength(50)]
        [Display(Name = "git分支名称")]
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
        string? _BuildCmd;
        /// <summary>编译命令</summary>
        [MaxLength(255)]
        [Display(Name = "编译命令")]
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
        System.Nullable<double> _CpuRate;
        /// <summary>cpu占用率</summary>
        [Display(Name = "cpu占用率")]
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
        string? _GitUserName;
        /// <summary>git用户名</summary>
        [MaxLength(255)]
        [Display(Name = "git用户名")]
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
        string? _Guid;
        [MaxLength(50)]
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
        System.Nullable<Int64> _ProcessId;
        /// <summary>运行后的进程id</summary>
        [Display(Name = "运行后的进程id")]
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
        string _GitRemote="origin";
        /// <summary>git远程仓库</summary>
        [MaxLength(50)]
        [DisallowNull]
        [Display(Name = "git远程仓库")]
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
        string? _Desc;
        /// <summary>描述</summary>
        [MaxLength(255)]
        [Display(Name = "描述")]
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
        System.Nullable<Int64> _UserId;
        /// <summary>发布人</summary>
        [Display(Name = "发布人")]
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
        Boolean _FileHasUpdated=false;
        /// <summary>文件有更新</summary>
        [DisallowNull]
        [Display(Name = "文件有更新")]
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
        string? _DockerPortMap;
        /// <summary>docker的端口映射</summary>
        [MaxLength(50)]
        [Display(Name = "docker的端口映射")]
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
        string? _DockerContainerId;
        [MaxLength(255)]
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
        string? _BuildPath;
        [MaxLength(255)]
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
        Boolean _IsHostNetwork=false;
        /// <summary>和主机使用同一个网络</summary>
        [DisallowNull]
        [Display(Name = "和主机使用同一个网络")]
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
        string? _ProgramPath;
        /// <summary>程序所在目录</summary>
        [MaxLength(255)]
        [Display(Name = "程序所在目录")]
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
        Boolean _IsNeedBuild=true;
        /// <summary>是否需要编译</summary>
        [DisallowNull]
        [Display(Name = "是否需要编译")]
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
        string? _MemoryLimit;
        /// <summary>docker内存限制</summary>
        [MaxLength(50)]
        [Display(Name = "docker内存限制")]
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
        string? _DockerFolderMap;
        /// <summary>docker的文件夹映射</summary>
        [MaxLength(255)]
        [Display(Name = "docker的文件夹映射")]
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
        string? _Category;
        /// <summary>分类</summary>
        [MaxLength(50)]
        [Display(Name = "分类")]
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
        Boolean _DeleteNoUseFiles=true;
        /// <summary>自动删除不用的文件</summary>
        [DisallowNull]
        [Display(Name = "自动删除不用的文件")]
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
        Project_RunTypeEnum _RunType=(Project_RunTypeEnum)(1);
        /// <summary>运行方式</summary>
        [DisallowNull]
        [Display(Name = "运行方式")]
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
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param name="exp">指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
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
    public class BackupFile :Way.EntityDB.DataItem
    {
        System.Nullable<Int64> _id;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisallowNull]
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
        /// <param name="exp">指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<BackupFile, bool>> exp)
        {
            base.SetValue<BackupFile>(exp);
        }
    }
    /// <summary>记录工程包含的文件</summary>
    [TableConfig]
    [Table("projectfiles")]
    [Way.EntityDB.DataItemJsonConverter]
    public class ProjectFiles :Way.EntityDB.DataItem
    {
        System.Nullable<Int64> _id;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisallowNull]
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
        /// <param name="exp">指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
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
            result.Append("H4sIAAAAAAAACu1dW3PTSBb+Kyk/Q5Ac24mpyQPEMONaCKlk2N2q8TwolhI0yJJHksmwLFVhB0iA3Lgul1AV2AAZhlygZiHEyeTPuC3naf/CdqsVX2TZiSNZbtl6SayW1JJOf33O6e+c7r4W+J4ZFTglcPKHa/jnIJPiAicDIz8LvMoFjgWGpQl8Nq5yKfzLuIRn4em/");
            result.Append("MkIGHtDXjxXL1atprnQmMCBzjMrpdZ9KqrwkBsquTUqiyolq2eXXEvg9EoGT8CfPwv/Bnp5jiUCSSaO74XEisPfrirbzSXt+c+/VV+3FWiIAz6Ma9ZNDsvQTl1T1whijMqOMwsVj8FSIgiX8OSl5GR5Q1+HBgCRkUqICD3/Yfxgd1S8rPU3MCEJ59egqeBxXTmVUKS4m");
            result.Append("ZS4FvwCeUuUMh+pkxEF4CywYYwQFlbCj30OR6DeP8uM8uhiWGmVG9WfETCrGjfFiWZnAiePqpdIxy40xGUHVJVUq1aWlfx+WU1wZ+ot+aLyPJLOcrH8a+q64EuOVpMyneJFRJbnsLQcERlGMrzSq1ovOwt9lxbDxSpKi60oK/7CUVfGhRWEZb1smqyuMnLzEyI0KKxEI");
            result.Append("U/o9DYpr/5VK8qIdl1fQhOP83IO9Z8sQx/knU7ns50ocn/klKWRY7iwP+2crxBgMh52RY9BxOVrrA7A2k19/DBbmCrsLhVczUKzg/k4uu1wp1uGMOJBiPS3QHscFGjIJFNxYzK++rivHEVVKe12QIUcE+SOqJfb3AUkc48d1Y4JLDJV7UeHkuC7n6+WmjzVMU7kZDVGw");
            result.Append("MmRyR1RoM3UjdYDhDTbX8FbhYhnqqZ0aCus0k7ycSSN9dXTbG/as7Q25bHsjdSWFWqFV9pemDmuAQ24b4F6zAdZhDPGcYsNgcrsSz+djYePdCfZhQm7b3j6TCHO7L8Hq08KXdfDnTZOFYK5wQwz6IJJNRMhtWxu1xCBYfgc25isFiDrxOeObjizCo6m9RMAZ8TXLwkJ5");
            result.Append("xpWLIv+z/l779cWVASGjqJzMseVP0S0Nqk+/tVw5oorN7XT0eksKw9l6y3Fgrtk9V6OnjqtxiRHHG3I1LghskW6oGrUPchO1T4rcRJnrgD4/qT+dNZWynMCpVaX1UGT0nUM2ijWA3GuOEInNUVJzYcqs5mamwNpzPFKzshZDmVGBVy61ymAc3mlxZbTbubgOE45r2sw9");
            result.Append("3JrVdtaM8VBC/EmRxPzjDS17E+y8Btsmi47bqGWUjmMgd8Yr6lyQRwgHuZmoHOdVsLgBXk5WwvlbXr0oC2T79wcguddp/z7cYyG76dv5R+tgYVZ7t2HiTGRGTF7yOlPe57gQzcQT3V+Yeg/urnQF+/N37sEfls7DeYk9vBwtKCLHBkoQoLiiw8sw6rgMw2YP7CmE4Nu9");
            result.Append("xcnC2xva9pPC+v1KIcaVQY5jT2d44fCsriXP5pgQMTwbCdk4zrOFK3m2RABLzooR1yXndUqcdoZ061zT3ku4aTdToGDnIbgzq939nJ+8Af1Xqh/HfrqgxtXDP7nNVXPgh1EzCiFqtnEN4TgjGjYzosl0BswuaY9WtLkpk/ufzgxDGNlQEKwELR3XNDbvIOE5zoaGzWwo");
            result.Append("uH0L0sk15HeeS0nyVW+L0DlGtDP1ax+J+tW6OdrJn6Va4s8e248q46ZlYQukOVnldTBeq3pN1KIyincwQoUnfv06CY5xS80e5Upz7H+sZUPoAwPYFJ2gpaIkaqlSF4hQ1SQFNLj56S+QpKjmeKCkfIrC5LlEaAueZ/22tnSjSn5DE6yX2R1nmInO9VhoinBlYGZ7sQ3E");
            result.Append("IY38nUmwuLKfo/DVgvEgP8XDnVFNIwD3Uy8jriSGeExR0IQrCnNoI7+6DFb/nV/7r/boI1h7Dd0H7ff13M4udCUqFcWFCZGTRzj5Cid72RLSzkSLOtgUBglHeKhuVu23mZoZyN7Arw9fe/D1fA6cr8xLncGZqHWbKWjC0wojZu4OB6nAwpz2/GZh94W2cs9Q0eXVJjlF");
            result.Append("ibPkJlG7h9QOdTsITyqMRKq5pMLuIgRzLvsQbD2sYpSGYSCsgSCYBRntsC+C4Ir6SKO8tI9sm8gmPJMwUpVusLgCHr7Pbb7XXqyBnce5zXv5O+/QLNOaaTUDMAjmfY4J2S1fhdsAeq8X477tCf9EoPtEwwFIRzqA/Xiw7nl0RgyS9mSqhHdCk44ZB1dSJYovbhmbR15d");
            result.Append("ZZYE+fFN54wzRUwDdIReIjw5orcqP2h+vrBrmrgR45Skp0HvT+GyufIK4VH9PnNU34UAHZr8ONoqlixoO7O7mHf5N260GwujO3YaJWEKSndMlZTuU+O6MDqYSgvS7RMC8btIk7tIG8Ge8MB1nzk1A8zfB5u/5ra2KvX8vpA8Ggmxr+M7PBISbKMIdsdPZfMD2BYAJzyA");
            result.Append("3WeeCZTbvJvbXoIBbDwLBcyv57JvcFTbRAQz4nBGPDUGdczFtCAxpEzSphulgn0lbleJh71I7B4d+XHlLC8rKlGop1qD+oNIRuNSS4rRQoGghVgPM4eO7hhyMhhpi76FuxT8q334kNuczH94hXsb+R2LJrNjHaaXdA6FHyQyGl/WH8xrMhgm5ssnLTuPJ1zjZS7M/QGt");
            result.Append("ap7mvGtkfFbfJrD7CAd21BrYiyuslLzMybnNVWsPClr+uBjTr/EsuP2lM2yCm/DAa7RqzVg8BXXxTv7FH/knpggsWkXzO0a5mIYmzcMa208MtLksNeFx2Kg5jwYrau35TRiKBfP/yT9dAhumtZCxnh6SZPU8k/byhDMf3XbRTROO7mDdCZMYyHDQozIQavYiT61PmQk6");
            result.Append("N2m9M6NPPcE2mj/Z6atmODcXvo0A3kO4uu6pq67bBJnuq2l/xkdTm6GNFATh4elo1Y5/D2Zym9n84hbOGgUL8HASzV/aua9lF80s03eSog5y6oQkX/bsgNxnmWxCPEw4xM3p/drKPbA1j102PC2vEtaw0nGZSXnfLrqP7MYWXdVXH19420X3w5PttwSrO3vhecweRghX");
            result.Append("FhHPb6LRcEDdX6LcLqh7m7srszkGWFjbgEYLfHmDLNnMLbDwe40dmg3El/YfO8oezdFez+7RHHV3j+ZoX11JGa3RACnqmMQOnY8fdWNBuXKRReuKDCGXfD/sAKEFSdwTtwyMzm9ei5ushV6GK1kdJbVay9UoXdG8XWyda3cX2ydKdvsUTS9FUXX1E0yolATYz0nR6FbD");
            result.Append("lAO0E3kZZF5ENLyDaETXTyh2CeceB7nd3VBwNz9yxrAnuwXR6QwQ9tbJOnhTsr1nC2D6s9WOZOcgiFRP5+n46zDaBHawqUN+fWheEZaZuw3mP+ExvvYuu/dsubD2Ckw/qE6RjOmKZYRTVV4cP+KgH3aMoFcH/foHujboh5Kq2iTByAj5DXx8DBZm/7c9A97+q+sfPE7s");
            result.Append("K4rwzC8qQeu5HiTHplMBUJBV8wV3X2qPn4GpJbD8mylfkrmqEBEWaFxszSIDXLTpzc15wS4W0mTWZt10vqZlj9RVYIaO5tHhkc04SkRX96toBhsXqo8l8lLJj8AVWUek7HFQ52NhQ6rOc1vnjFZsoVMdIrcD+mByBEz2YFqu3FoI0+bmhVj7ulVotb7MSdC6KNE6wXP8");
            result.Append("jQ0POg6Qk0vfRfTs4iClzzi3nNJTzPiuPasHE2ctmtfj3FQIfyc0mxjvIxzjVQvKTd/WPmYrET0AP3Yc8l9epr78dE+bQI4SDmQzjVCYeo+Wf5hegqRZbnMWJjXXyJbB5m9QgrauFEbxYD5Xj79ooj2IhynCIW69KVr+yVewPV+J6eGMiCHYVLIsEThxQnvxR37OWM4L");
            result.Append("Z1jD/F4jlbofkoUJ8cSJXPbNhYE4WPsKnq0Yy1YkRFhSnDDaT3/zTela7EAVL8SH6JJw1z+7ym+D79OUnuRPZ7bbk9pp9ehOXoqlE+d2wUMDCAEE79Lz90GOAmuoNeDxyFX4vqnuuKhGQvqTKq42kF99/Ygqw6F29Q0l+B/+noqPqvlqP6LPQmfhMN+4kQuFqTEmyRwP");
            result.Append("Rtie4yGYTHGcSVL0cTYagSmtFMWEQvDG/wMjmxeUO64AAA==");
            return result.ToString();
        }
    }
}

/*<design>
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
<design>*/

