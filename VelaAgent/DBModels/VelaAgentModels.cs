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
        [MaxLength(512)]
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
        string? _DockerEnvMap;
        /// <summary>docker的环境变量设置</summary>
        [MaxLength(512)]
        [Display(Name = "docker的环境变量设置")]
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
            result.Append("H4sIAAAAAAAACu1dW1PbSBb+K5SfEyIJ2+DU8JDgZMa1CaFgMrtV43kQliCayJJHksNks6kikwskAUyumwupIlmSMJlgSGo2ITEMf8Ztmaf9C9tSC19k2WAkyy1bL2C1pJZ0+utzTn/ndPeVwPf0OM/KgeM/XkE/h+kkGzgeGPuF5xQ2cCQwKk6hszGFTaJfxiUcA0//");
            result.Append("QPNpeEBePVIqVy6n2PKZwJDE0gqr130ioXCiEKi4NiEKCisoFZdfiaP3iAeOw58cA/9TfX1H4oEEndLuhsfxwO71VXX7o/rsxu7LL+rzbDwAz2s16idHJPFnNqHohVFaocdpmY1F4akgAUu4M2LiIjwgrsKDIZFPJwUZHv649zAyol9WfpqQ5vnK6rWr4HFMPpFWxJiQ");
            result.Append("kNgk/AJ4SpHSrFYnLQzDW2DBBM3LWgkz/j0UiX7zODfJaRfDUqPMqP6UkE5G2QlOqCjjWWFSuVA+ZtgJOs0ruqTKpbq09O9DcorJI3/TD433ESWGlfRP074rJkc5OSFxSU6gFVGqeMshnpZl4yuNqvWi0/B3RTFsvLKkyIaSQj8sZVV6aElYxttWyOoSLSUu0FKzwooH");
            result.Append("QoR+T5Pi2nulsrxIx+VFmXBcWLi/+3QF4rjweCaf+1SN41O/Jvg0w57mYP9shxipUMgZOVKOy9FaH4DsXGH9EVhcKO4sFl/OQbGCe9v53Eq1WEfTwlCS8bRA+xwXaNAkUHBtqbD2qqEcxxQx5XVBBh0R5E9aLdF/DInCBDepGxNUYqjc8zIrxXQ5X600fYxhmirNaJCA");
            result.Append("lWkmd0yBNlM3UvsYXqq1hrcGFytQT23XUVgn6cTFdErTV4e3vSHP2t6gy7Y33FBSWiu0y/6SxEENcNBtA9xvNsA6jCGek0wITG9V4/lsNGS8O8Y+TNBt2ztgEmF+5wVYe1L8vA7+umGyEPQldoTWPghnExF029ZGLDEIVt6CjUy1ALVOfMb4pkOL8HBqLx5wRnytsrBQ");
            result.Append("njH5vMD9or/XXn0xeYhPyworsUzlU3RLo9Wn31qpHLWKze10+HrLCsPZeitxYK7ZPVejr4GrcYEWJptyNc7xTIluqBm1D7NT9U8K7FSF66B9fkJ/OmMqZVieVWpKG6HI6DsHbBRrALnXHEEcm6Os5kKEWc3NzYDsMzRSs7IWI+lxnpMvtMtgHNxpcWW02724DmGOa9LM");
            result.Append("PdycV7ezxngoLvwsi0Lh0YaauwG2X4Etk0VHbdQ2SscxkDvjFXUvyMOYg9xMVE5yCljaAC+mq+H8Laecl3i8/ft9kNzvtH8f6rOQ3eytwsN1sDivvt0wcSYSLSQueJ0pH3BciGbiiRwszrwDd1Z7qMHC7bvwh6XzcFZkDi5HC4rIsYESBCiq6OAyjDguw5DZA3sCIfhm");
            result.Append("d2m6+OaauvW4uH6vWogxeZhlmZNpjj84q2vJszkmRATPZkI2jvNsoWqeLR5AkrNixHXJeZ0SJ50h3brXtPdjbtrNFCjYfgBuz6t3PhWmr0H/lRhEsZ8eqHH18E9+c80c+KGVtIyJmm1eQzjOiIbMjGgilQbzy+rDVXVhxuT+p9KjEEY2FAQjQkvHtozN2094jrOhITMb");
            result.Append("Cm7dhHRyHfmdZZOidNnbInSOEe1O/TqAo361bo5O8meJtvizR/aiyqhpGdgCKVZSOB2MV2peU2tRSYt30HyVJ371Kg6OcVvNHuFKc+x9rGVD6AMD2BTdoKUiOGqpchcIE7UkBTS4hdnPkKSo5XigpHyKwuS5hEkLnmf9lrp8rUZ+I1OMl9kdZ5iJ7vVYSAJzZWBme5EN");
            result.Append("RCGNwu1psLS6l6PwxYLxwD/Fw51RTTMA91Mvw64khnhMUZCYKwpzaKOwtgLW/l3I/ld9+AFkX0H3Qf1jPb+9A12JakVxbkpgpTFWusRKXraEpDPRoi42hRTmCA82zKr9Nl03A9kb+PXhaw++ns+B85V5uTM4E7XuMAWNeVph2MzdoSAVWFxQn90o7jxXV+8aKrqy2gQr");
            result.Append("yzEG3yRq95DapW4H5kmF4XAtl1TcWYJgzucegK8PahilURgIayIIZkFGO+yLaHDV+kizvLSPbJvIxjyTMFyTbrC0Ch68y2++U59nwfaj/Obdwu232izTumk1QzAI5n2OSbNbvgq3AfR+L8Z9OxP+8UDvsaYDkI50APvxYN3z6I4YJOnJVAnvhCYdMw6upEqUXtwyNq95");
            result.Append("ddVZEvjHN50zzgQ2DdAVegnz5Ij+mvygTKa4Y5q4EWXlhKdB70/hsrnyCuZR/QFzVN+FAJ02+XG8XSwZZTuzu5R3+Xd2vBcJozd6UkvC5OXeqCLKvScmdWF0MZVGkZ0TAvG7SIu7SAfBHvPA9YA5NQNk7oHN6/mvX6v1/J6QPBoJsa/juzwSQnVQBLvrp7L5AWwLgGMe");
            result.Append("wB4wzwTKb97Jby3DADaahQIy6/ncaxTVNhHBtDCaFk5MQB1zPsWLNC6TtMlmqWBfidtV4iEvEruHR35MPs1JsoIV6on2oH4/ktG41JJitFAg2kKsB5lDR3YNOUmFO6JvoS4F/6rv3+c3pwvvX6Lehn/HIvHsWAfpJd1D4VNYRuMr+oN5TQbDxHz+qOYyaMI1WubC3B+0");
            result.Append("Vc1TrHeNjM/q2wT2AObAjlgDe2mVERMXWSm/uWbtQUHLHxOi+jWeBbe/dIZNcGMeeI3UrBmLpqAu3S48/7Pw2BSB1VbR/I6Wz6egSfOwxvYTA20uS415HDZizqNBilp9dgOGYkHmP4Uny2DDtBYy0tMjoqScpVNennDmo9suuknM0U01nDCJgAwHPQoNoWYv8tT+lBnK");
            result.Append("uUnr3Rl96qM6aP5kt6+a4dxc+A4CeB/m6rqvobruEGS6r6b9GR8tbYYOUhCYh6cjNTv+3Z/Lb+YKS19R1ihYhIfT2vyl7XtqbsnMMn0nysowq0yJ0kXPDsh9lskmxEOYQ9yc3q+u3gVfM8hlQ9PyqmENK52U6KT37aL7yG5u0VV99fHFNz3kIDzZeUuwurMXnsfsYRhz");
            result.Append("ZRH2/CYaTQfU/SXK7YK6v7W7MptjgMXsBjRa4PNrzZLN3QSLf9TZodlAfHn/scPs0Rzp9+wezRF392iODDSUlNEaTZCijknswPn4ETcWlKsUWaShyDTk4u+H7SM0Csc9cSvA6PzmtajJ2uhluJLVUVar9VyN8hWt28XWuXZ3sX0ieLdPyfQSBNFQP8GESpGH/RwXjW41");
            result.Append("TNlHO+GXQeZFRMM7sEZ044Ril3DucZDb3Q0FdfNDZwx7sltgnc4AYW+drIM2Jdt9ughmP1ntSHYGgkjxdJ6Ovw6jTWBTLR3y60PzqrDMwi2Q+YjG+Orb3O7TlWL2JZi9X5siGdUVyxirKJwwechBP+wYlFcH/foHujboh5Kq2STByAj5HXx4BBbn/7c1B9781vNPDiX2");
            result.Append("lUR46lcFo/Vc95Njy6kAKMia+YI7L9RHT8HMMlj53ZQvSV+WsQgLNC+2VpEBLtr01ua8IBdL02TWZt10vq5lDzdUYIaO5rTDQ5txLRFd2auiFWxcsDGW8EslPwRXZB2RssdBnY2GDKk6z22dMVqxjU51EN8O6IPJETDZg2mlcmsjTFubF2Lt69ag1foyJ0HrokQbBM/R");
            result.Append("NzY96NhHTi59F9aziylCn3FuOaWnlPFdf1YPIs7aNK/HuakQ/k5oNjE+gDnGaxaUm72lfshVI3oIfuwk5L+8TH356Z42gRzBHMhmGqE4805b/mF2GZJm+c15mNRcJ1sGmb9hEdq6chjFg/lcff6iifYgHiIwh7j1pmiFx1/AVqYa06NpAUGwpWRZPHDsmPr8z8KCsZwX");
            result.Append("yrCG+b1GKvUgJAvjwrFj+dzrc0MxkP0Cnq4ay1bEBVhSmjA6SH7zTfla5ECVLkSH2iWhnn/1VN4G36clPcmfzmy3J3XS6tHdvBSLP7fLAtwU5maiv+5KFAvr4NV1kHmyO5MpZv9St7NWI9dTwqV2LUdBUg4NWyMtpMg7gSBwTtKOZBTY21NHZzs6ZEU+eGi8e0CzreXn");
            result.Append("7ykhLaqvNTU8HrsM3zfZGxOUcFB/UtXVhmaqvX5MkSDPV3tDWT0d/J6qj6r7aj9pn6WdhRyjcSMbDBETdII+SoWZvqNBmMl1lE4Q5FEmEob59ARBB4Pwxv8D/oPnxLiyAAA=");
            return result.ToString();
        }
    }
}

/*<design>
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
<design>*/

