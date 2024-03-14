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

namespace VelaWeb.Server.DBModels
{
    [TableConfig]
    [Table("userinfo")]
    [Way.EntityDB.DataItemJsonConverter]
    public class UserInfo :Way.EntityDB.DataItem
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
        string _Name;
        [MaxLength(50)]
        [DisallowNull]
        [Column("name")]
        public virtual string Name
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
        string _Password;
        [MaxLength(255)]
        [DisallowNull]
        [Column("password")]
        public virtual string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                if ((_Password != value))
                {
                    SendPropertyChanging("Password",_Password,value);
                    _Password = value;
                    SendPropertyChanged("Password");
                }
            }
        }
        UserInfo_RoleEnum _Role=(UserInfo_RoleEnum)(1);
        /// <summary>角色</summary>
        [DisallowNull]
        [Display(Name = "角色")]
        [Column("role")]
        public virtual UserInfo_RoleEnum Role
        {
            get
            {
                return _Role;
            }
            set
            {
                if ((_Role != value))
                {
                    SendPropertyChanging("Role",_Role,value);
                    _Role = value;
                    SendPropertyChanged("Role");
                }
            }
        }
        Int32 _ErrorCount=0;
        [DisallowNull]
        [Column("errorcount")]
        public virtual Int32 ErrorCount
        {
            get
            {
                return _ErrorCount;
            }
            set
            {
                if ((_ErrorCount != value))
                {
                    SendPropertyChanging("ErrorCount",_ErrorCount,value);
                    _ErrorCount = value;
                    SendPropertyChanged("ErrorCount");
                }
            }
        }
        Boolean _IsLock=false;
        [DisallowNull]
        [Column("islock")]
        public virtual Boolean IsLock
        {
            get
            {
                return _IsLock;
            }
            set
            {
                if ((_IsLock != value))
                {
                    SendPropertyChanging("IsLock",_IsLock,value);
                    _IsLock = value;
                    SendPropertyChanged("IsLock");
                }
            }
        }
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param name="exp">指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<UserInfo, bool>> exp)
        {
            base.SetValue<UserInfo>(exp);
        }
    }
    public enum UserInfo_RoleEnum:int
    {
        Normal = 1,
        Admin = Normal | 1<<20,
    }
    /// <summary>目标服务器</summary>
    [TableConfig]
    [Table("agent")]
    [Way.EntityDB.DataItemJsonConverter]
    public class Agent :Way.EntityDB.DataItem
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
        string _Address;
        [MaxLength(50)]
        [DisallowNull]
        [Column("address")]
        public virtual string Address
        {
            get
            {
                return _Address;
            }
            set
            {
                if ((_Address != value))
                {
                    SendPropertyChanging("Address",_Address,value);
                    _Address = value;
                    SendPropertyChanged("Address");
                }
            }
        }
        Int32 _Port;
        [DisallowNull]
        [Column("port")]
        public virtual Int32 Port
        {
            get
            {
                return _Port;
            }
            set
            {
                if ((_Port != value))
                {
                    SendPropertyChanging("Port",_Port,value);
                    _Port = value;
                    SendPropertyChanged("Port");
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
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param name="exp">指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<Agent, bool>> exp)
        {
            base.SetValue<Agent>(exp);
        }
    }
    [TableConfig]
    [Table("logs")]
    [Way.EntityDB.DataItemJsonConverter]
    public class Logs :Way.EntityDB.DataItem
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
        Int64 _UserId;
        [DisallowNull]
        [Column("userid")]
        public virtual Int64 UserId
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
        string _Operation;
        /// <summary>操作</summary>
        [MaxLength(50)]
        [DisallowNull]
        [Display(Name = "操作")]
        [Column("operation")]
        public virtual string Operation
        {
            get
            {
                return _Operation;
            }
            set
            {
                if ((_Operation != value))
                {
                    SendPropertyChanging("Operation",_Operation,value);
                    _Operation = value;
                    SendPropertyChanged("Operation");
                }
            }
        }
        string? _Detail;
        [MaxLength(255)]
        [Column("detail")]
        public virtual string? Detail
        {
            get
            {
                return _Detail;
            }
            set
            {
                if ((_Detail != value))
                {
                    SendPropertyChanging("Detail",_Detail,value);
                    _Detail = value;
                    SendPropertyChanged("Detail");
                }
            }
        }
        DateTime _Time;
        [DisallowNull]
        [Column("time")]
        public virtual DateTime Time
        {
            get
            {
                return _Time;
            }
            set
            {
                if ((_Time != value))
                {
                    SendPropertyChanging("Time",_Time,value);
                    _Time = value;
                    SendPropertyChanged("Time");
                }
            }
        }
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param name="exp">指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<Logs, bool>> exp)
        {
            base.SetValue<Logs>(exp);
        }
    }
    /// <summary>用户对程序的权限</summary>
    [TableConfig]
    [Table("userprojectpower")]
    [Way.EntityDB.DataItemJsonConverter]
    public class UserProjectPower :Way.EntityDB.DataItem
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
        string _ProjectGuid;
        [MaxLength(50)]
        [DisallowNull]
        [Column("projectguid")]
        public virtual string ProjectGuid
        {
            get
            {
                return _ProjectGuid;
            }
            set
            {
                if ((_ProjectGuid != value))
                {
                    SendPropertyChanging("ProjectGuid",_ProjectGuid,value);
                    _ProjectGuid = value;
                    SendPropertyChanged("ProjectGuid");
                }
            }
        }
        Int64 _UserId;
        [DisallowNull]
        [Column("userid")]
        public virtual Int64 UserId
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
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param name="exp">指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<UserProjectPower, bool>> exp)
        {
            base.SetValue<UserProjectPower>(exp);
        }
    }
    /// <summary>文件定期删除设置</summary>
    [TableConfig]
    [Table("filedeletesetting")]
    [Way.EntityDB.DataItemJsonConverter]
    public class FileDeleteSetting :Way.EntityDB.DataItem
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
        string _Ext;
        /// <summary>文件扩展名，如 .zip</summary>
        [MaxLength(50)]
        [DisallowNull]
        [Display(Name = "文件扩展名，如 .zip")]
        [Column("ext")]
        public virtual string Ext
        {
            get
            {
                return _Ext;
            }
            set
            {
                if ((_Ext != value))
                {
                    SendPropertyChanging("Ext",_Ext,value);
                    _Ext = value;
                    SendPropertyChanged("Ext");
                }
            }
        }
        Int32 _Days;
        /// <summary>保留几天</summary>
        [DisallowNull]
        [Display(Name = "保留几天")]
        [Column("days")]
        public virtual Int32 Days
        {
            get
            {
                return _Days;
            }
            set
            {
                if ((_Days != value))
                {
                    SendPropertyChanging("Days",_Days,value);
                    _Days = value;
                    SendPropertyChanged("Days");
                }
            }
        }
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param name="exp">指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<FileDeleteSetting, bool>> exp)
        {
            base.SetValue<FileDeleteSetting>(exp);
        }
    }
    /// <summary>程序警报线设置</summary>
    [TableConfig]
    [Table("projectalarmsetting")]
    [Way.EntityDB.DataItemJsonConverter]
    public class ProjectAlarmSetting :Way.EntityDB.DataItem
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
        string _ProjectGuid;
        [MaxLength(50)]
        [DisallowNull]
        [Column("projectguid")]
        public virtual string ProjectGuid
        {
            get
            {
                return _ProjectGuid;
            }
            set
            {
                if ((_ProjectGuid != value))
                {
                    SendPropertyChanging("ProjectGuid",_ProjectGuid,value);
                    _ProjectGuid = value;
                    SendPropertyChanged("ProjectGuid");
                }
            }
        }
        System.Nullable<double> _Cpu;
        /// <summary>cpu超过百分之几，如 30，表示超过30%</summary>
        [Display(Name = "cpu超过百分之几，如 30，表示超过30%")]
        [Column("cpu")]
        public virtual System.Nullable<double> Cpu
        {
            get
            {
                return _Cpu;
            }
            set
            {
                if ((_Cpu != value))
                {
                    SendPropertyChanging("Cpu",_Cpu,value);
                    _Cpu = value;
                    SendPropertyChanged("Cpu");
                }
            }
        }
        System.Nullable<double> _Memory;
        /// <summary>内存超过百分之几</summary>
        [Display(Name = "内存超过百分之几")]
        [Column("memory")]
        public virtual System.Nullable<double> Memory
        {
            get
            {
                return _Memory;
            }
            set
            {
                if ((_Memory != value))
                {
                    SendPropertyChanging("Memory",_Memory,value);
                    _Memory = value;
                    SendPropertyChanged("Memory");
                }
            }
        }
        string _Cmd;
        /// <summary>执行命令</summary>
        [MaxLength(255)]
        [DisallowNull]
        [Display(Name = "执行命令")]
        [Column("cmd")]
        public virtual string Cmd
        {
            get
            {
                return _Cmd;
            }
            set
            {
                if ((_Cmd != value))
                {
                    SendPropertyChanging("Cmd",_Cmd,value);
                    _Cmd = value;
                    SendPropertyChanged("Cmd");
                }
            }
        }
        Boolean _CanRun=true;
        [DisallowNull]
        [Column("canrun")]
        public virtual Boolean CanRun
        {
            get
            {
                return _CanRun;
            }
            set
            {
                if ((_CanRun != value))
                {
                    SendPropertyChanging("CanRun",_CanRun,value);
                    _CanRun = value;
                    SendPropertyChanged("CanRun");
                }
            }
        }
        System.Nullable<DateTime> _LastAlarmTime;
        /// <summary>
        /// 上一次警报时间
        /// 程序自己使用，这个字段在数据库始终为空
        /// </summary>
        [Display(Name = "上一次警报时间 程序自己使用，这个字段在数据库始终为空")]
        [Column("lastalarmtime")]
        public virtual System.Nullable<DateTime> LastAlarmTime
        {
            get
            {
                return _LastAlarmTime;
            }
            set
            {
                if ((_LastAlarmTime != value))
                {
                    SendPropertyChanging("LastAlarmTime",_LastAlarmTime,value);
                    _LastAlarmTime = value;
                    SendPropertyChanged("LastAlarmTime");
                }
            }
        }
        Boolean _IsEnable=true;
        [DisallowNull]
        [Column("isenable")]
        public virtual Boolean IsEnable
        {
            get
            {
                return _IsEnable;
            }
            set
            {
                if ((_IsEnable != value))
                {
                    SendPropertyChanging("IsEnable",_IsEnable,value);
                    _IsEnable = value;
                    SendPropertyChanged("IsEnable");
                }
            }
        }
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param name="exp">指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<ProjectAlarmSetting, bool>> exp)
        {
            base.SetValue<ProjectAlarmSetting>(exp);
        }
    }
    /// <summary>用户访问agent的权限</summary>
    [TableConfig]
    [Table("useragentpower")]
    [Way.EntityDB.DataItemJsonConverter]
    public class UserAgentPower :Way.EntityDB.DataItem
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
        Int64 _AgentId;
        [DisallowNull]
        [Column("agentid")]
        public virtual Int64 AgentId
        {
            get
            {
                return _AgentId;
            }
            set
            {
                if ((_AgentId != value))
                {
                    SendPropertyChanging("AgentId",_AgentId,value);
                    _AgentId = value;
                    SendPropertyChanged("AgentId");
                }
            }
        }
        Int64 _UserId;
        [DisallowNull]
        [Column("userid")]
        public virtual Int64 UserId
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
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param name="exp">指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<UserAgentPower, bool>> exp)
        {
            base.SetValue<UserAgentPower>(exp);
        }
    }
    /// <summary>代码转换任务</summary>
    [TableConfig]
    [Table("codemission")]
    [Way.EntityDB.DataItemJsonConverter]
    public class CodeMission :Way.EntityDB.DataItem
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
        string _Name;
        [MaxLength(50)]
        [DisallowNull]
        [Column("name")]
        public virtual string Name
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
        CodeMission_TypeEnum _Type=(CodeMission_TypeEnum)(1);
        [DisallowNull]
        [Column("type")]
        public virtual CodeMission_TypeEnum Type
        {
            get
            {
                return _Type;
            }
            set
            {
                if ((_Type != value))
                {
                    SendPropertyChanging("Type",_Type,value);
                    _Type = value;
                    SendPropertyChanged("Type");
                }
            }
        }
        Byte[]? _Code;
        /// <summary>代码</summary>
        [Display(Name = "代码")]
        [Column("code")]
        public virtual Byte[]? Code
        {
            get
            {
                return _Code;
            }
            set
            {
                if ((_Code != value))
                {
                    SendPropertyChanging("Code",_Code,value);
                    _Code = value;
                    SendPropertyChanged("Code");
                }
            }
        }
        Byte[]? _Script;
        /// <summary>转换脚本</summary>
        [Display(Name = "转换脚本")]
        [Column("script")]
        public virtual Byte[]? Script
        {
            get
            {
                return _Script;
            }
            set
            {
                if ((_Script != value))
                {
                    SendPropertyChanging("Script",_Script,value);
                    _Script = value;
                    SendPropertyChanged("Script");
                }
            }
        }
        System.Nullable<Int64> _ParentId;
        /// <summary>上级id</summary>
        [Display(Name = "上级id")]
        [Column("parentid")]
        public virtual System.Nullable<Int64> ParentId
        {
            get
            {
                return _ParentId;
            }
            set
            {
                if ((_ParentId != value))
                {
                    SendPropertyChanging("ParentId",_ParentId,value);
                    _ParentId = value;
                    SendPropertyChanged("ParentId");
                }
            }
        }
        string _Language="CSharp";
        /// <summary>语言</summary>
        [MaxLength(50)]
        [DisallowNull]
        [Display(Name = "语言")]
        [Column("language")]
        public virtual string Language
        {
            get
            {
                return _Language;
            }
            set
            {
                if ((_Language != value))
                {
                    SendPropertyChanging("Language",_Language,value);
                    _Language = value;
                    SendPropertyChanged("Language");
                }
            }
        }
        Int64 _UserId;
        /// <summary>拥有者id</summary>
        [DisallowNull]
        [Display(Name = "拥有者id")]
        [Column("userid")]
        public virtual Int64 UserId
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
        string? _Path;
        /// <summary>
        /// 路径描述
        /// /上级id/
        /// </summary>
        [MaxLength(1024)]
        [Display(Name = "路径描述 /上级id/")]
        [Column("path")]
        public virtual string? Path
        {
            get
            {
                return _Path;
            }
            set
            {
                if ((_Path != value))
                {
                    SendPropertyChanging("Path",_Path,value);
                    _Path = value;
                    SendPropertyChanged("Path");
                }
            }
        }
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param name="exp">指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<CodeMission, bool>> exp)
        {
            base.SetValue<CodeMission>(exp);
        }
    }
    public enum CodeMission_TypeEnum:int
    {
        Folder = 1,
        Mission = 2
    }
    [TableConfig]
    [Table("vuemethod")]
    [Way.EntityDB.DataItemJsonConverter]
    public class VueMethod :Way.EntityDB.DataItem
    {
        Int64 _UserId;
        [Key]
        [DisallowNull]
        [Column("userid")]
        public virtual Int64 UserId
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
        string _Code;
        [DisallowNull]
        [Column("code")]
        public virtual string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                if ((_Code != value))
                {
                    SendPropertyChanging("Code",_Code,value);
                    _Code = value;
                    SendPropertyChanged("Code");
                }
            }
        }
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param name="exp">指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<VueMethod, bool>> exp)
        {
            base.SetValue<VueMethod>(exp);
        }
    }
}

namespace VelaWeb.Server.DBModels.DB
{
    public class VelaServer : Way.EntityDB.DBContext
    {
         public VelaServer(string connection, Way.EntityDB.DatabaseType dbType , bool upgradeDatabase = true): base(connection, dbType , upgradeDatabase)
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
             var db =  sender as VelaServer;
            if (db == null) return;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfo>().HasKey(m => m.id);
            modelBuilder.Entity<Agent>().HasKey(m => m.id);
            modelBuilder.Entity<Logs>().HasKey(m => m.id);
            modelBuilder.Entity<UserProjectPower>().HasKey(m => m.id);
            modelBuilder.Entity<FileDeleteSetting>().HasKey(m => m.id);
            modelBuilder.Entity<ProjectAlarmSetting>().HasKey(m => m.id);
            modelBuilder.Entity<UserAgentPower>().HasKey(m => m.id);
            modelBuilder.Entity<CodeMission>().HasKey(m => m.id);
            modelBuilder.Entity<VueMethod>().HasKey(m => m.UserId);
        }
        System.Linq.IQueryable<UserInfo> _UserInfo;
        public virtual System.Linq.IQueryable<UserInfo> UserInfo
        {
            get
            {
                if (_UserInfo == null)
                {
                    _UserInfo = this.Set<UserInfo>();
                }
                return _UserInfo;
            }
        }
        System.Linq.IQueryable<Agent> _Agent;
        /// <summary>目标服务器</summary>
        public virtual System.Linq.IQueryable<Agent> Agent
        {
            get
            {
                if (_Agent == null)
                {
                    _Agent = this.Set<Agent>();
                }
                return _Agent;
            }
        }
        System.Linq.IQueryable<Logs> _Logs;
        public virtual System.Linq.IQueryable<Logs> Logs
        {
            get
            {
                if (_Logs == null)
                {
                    _Logs = this.Set<Logs>();
                }
                return _Logs;
            }
        }
        System.Linq.IQueryable<UserProjectPower> _UserProjectPower;
        /// <summary>用户对程序的权限</summary>
        public virtual System.Linq.IQueryable<UserProjectPower> UserProjectPower
        {
            get
            {
                if (_UserProjectPower == null)
                {
                    _UserProjectPower = this.Set<UserProjectPower>();
                }
                return _UserProjectPower;
            }
        }
        System.Linq.IQueryable<FileDeleteSetting> _FileDeleteSetting;
        /// <summary>文件定期删除设置</summary>
        public virtual System.Linq.IQueryable<FileDeleteSetting> FileDeleteSetting
        {
            get
            {
                if (_FileDeleteSetting == null)
                {
                    _FileDeleteSetting = this.Set<FileDeleteSetting>();
                }
                return _FileDeleteSetting;
            }
        }
        System.Linq.IQueryable<ProjectAlarmSetting> _ProjectAlarmSetting;
        /// <summary>程序警报线设置</summary>
        public virtual System.Linq.IQueryable<ProjectAlarmSetting> ProjectAlarmSetting
        {
            get
            {
                if (_ProjectAlarmSetting == null)
                {
                    _ProjectAlarmSetting = this.Set<ProjectAlarmSetting>();
                }
                return _ProjectAlarmSetting;
            }
        }
        System.Linq.IQueryable<UserAgentPower> _UserAgentPower;
        /// <summary>用户访问agent的权限</summary>
        public virtual System.Linq.IQueryable<UserAgentPower> UserAgentPower
        {
            get
            {
                if (_UserAgentPower == null)
                {
                    _UserAgentPower = this.Set<UserAgentPower>();
                }
                return _UserAgentPower;
            }
        }
        System.Linq.IQueryable<CodeMission> _CodeMission;
        /// <summary>代码转换任务</summary>
        public virtual System.Linq.IQueryable<CodeMission> CodeMission
        {
            get
            {
                if (_CodeMission == null)
                {
                    _CodeMission = this.Set<CodeMission>();
                }
                return _CodeMission;
            }
        }
        System.Linq.IQueryable<VueMethod> _VueMethod;
        public virtual System.Linq.IQueryable<VueMethod> VueMethod
        {
            get
            {
                if (_VueMethod == null)
                {
                    _VueMethod = this.Set<VueMethod>();
                }
                return _VueMethod;
            }
        }
        protected override string GetDesignString()
        {
            var result = new StringBuilder();
            result.Append("\r\n");
            result.Append("H4sIAAAAAAAACu1c23LTRhh+FUYzvSNgybKkMPQiTaDjKachwHQGc6FYm6AiS0GHBkozQ1sgJzLQUkpDw5S0gQQGAnRaSJOGvIxlm6u+Qlcr2ZZkRz7pmPTK3tVqtfvtv99/2F+6hp1hRwSgYIfOXzP/nmALADuEDV8WeBVg+7HT0oR5NauCgvnPasJz8PI5VtBgAZ/c");
            result.Append("X6tXr46D+hVsUAasClDfA3mVl0TM1jYviSoQVVvzazlzHDnsEPzLc/CXSGf257A8O27cDcuiJgiwwugClnLYWQXIWXFUymGwdohV2RFWAdkheI3EYQ1/TMpfgoXUJCwMSoJWEBVYPF/tHu+nGM/+jVawnFUGNFXKinkZFOCY4SVV1oDRJyuegLfAilFWUIwabuQMBAHd");
            result.Append("PMKP8UZjWGvVWd0fEbXCEBjlRVudAMQx9WK9zIFRVhNUhE29FuGD5mcik1VOfYaK1ngkmQMymloKXR3ilbzMF3iRVSXZNspBgVUUa5ZW16jqKPxvq4bLVUeq3xMp809TrGoPrYFljdaG1ZesnL/Iyp2ClcMyKXRPh3BVh1THC/cbLzrlidcpePcEfHySMSN8xwx3YJbD");
            result.Append("Kis/VGb+QKOtIXdaMiiia9Ta2JLwYZJcYIV9H++DA8qJAxycHSxYtV/vww8fJozldcLqD6hpX0C9YPQy9PmgJI7yY4jzzBqLGRBvIswn7ZzMWQxq53cSh50ZumBYhWSOuLSFRiCC1QiUS0bKv6yVHk+VFuf12SV9YdUpLANjSDK61A40kVjtQIWrHei0J1IDHCcDRWl7");
            result.Append("2zbBK3i2o8LWEKS3hpBktRfAupCutjmMCkMxRMlhaQ8Ou8iKYx1x2EmBqxnYjWbrCTDhcVUEEzZqMgDIo+dzzc1ZH420SPZgGFYarP6EzV/Sxs2V5E7J0jiQVR4YYF6zQ2CsnWzwNCtUx2xQ6eRkkIZeHHEn4oi778ZidyTqo7WYw3DzaSGbi61WxjXMJstj6rb21tAg");
            result.Append("MQ4IQHWQWJRsTyaT7ePMPEQmkyDqqQ28idSaLBpLuc0EKrd136lBaOuXnBJrk023m1a6c6ey/drJzkNAyUcRAGlfOKmwnPXGfR4/caOCdexpTzY7JhnT7daRpxPryNMhO/LeAfGqfHWvUbqDq23PlA7bie9389y9+eLWopPnTkJlw5ot4uwE0CGHexlv+2UIqCwvxFs/");
            result.Append("0GHoBztkuCdkZ/jeHHyoJ4Ba7SOC7UkGFDiCAGaVsyJ/GY2r2l9WGRQ0RQUy4OxPQQrE6A/dWuc8o1v3snTfq7lU7j7DU+Z0sMqccUfpf1wtTb/TX/1dXp3TN+6UH94oPfruw8JdJ1EaM4a2+Rcgr56SJoDctcJnMolV+Ey4Cp+hvJ1IczU+1XaELCbKiwlZ8TN0sg0l");
            result.Append("JpoQfk+c6RBGf+l4J5IPj5CZQJ35mvvU4MvXrrQTfGrXKU2Mwu2tT5thH53g9Mc1CkSkUk41nMP06VvlN5tOpT8I5zkmyVdjnQpDhWW4JiEQhKcCNR6RjefwqX+aKm6+1dcelhZ/1acff1hYrqy9L2+tOeXoKC+AIYTTMFBVXhzr0nqEYpvYcBGaYGjWI0SKabpUpZln");
            result.Append("+pv7+t35f/+5rT/9dt+Br/hx52IdudJTckPgG94JZOD2JETSHUgqbj8q31/Qpx7ry89cYXP2ak+pNAGala1gS35mCB5swjNJuB1n5C9XXj4tzT4pb2w3Iz7LLB4QWLnQK/XhqcRSHxEu9eH4rnCcnbCFQHS4W8Lz41rl7c3K9lR54T20D4t/z0HSsxRHOgX/VJZWy8sb");
            result.Append("Zpt06iOX9Tiu9WA4cpKGNmBAbEiEGlGH2Kbd9vatm/rLnxvhdWJ4HBRibn+3QjLtO5Kk27CZWaks3da/3ypuLrsksBDzFItW4JHJV8pEoD7wTgq2wSPeqWEneT3R7eJoyTAdk0QfOyZNW9U2bCxzgvB0kraCTei9T2yg3J7Wejo/H+F98nqaZGm2ku3M3goPkQkVQXfO");
            result.Append("WnF9trh+vfRiyXS/Sg/efnjwJ0zxtXyyqef6uzfFrW14tmlYqdsLxfXn+ssHpbW/9MXV0v3Xpfk1feOevjJX3pwurm+Un204CfsYq5iD6+jYvglvB3xq30q6qb0l3Zl4Zgvb5Ng7fHlEliV5UNIsVzzqIFIOM32ADnKCSd+tfe+st6xixk1ioXw6RmuPKZ9gs1TJdNPE");
            result.Append("lsra9ocHa6xxIOaR2oIOzHpJbIGS2p/Y+Fw63PgceufG45VUYy3inKHhxCuEwByBJzqjpRVeu+DogU6mXU14p5hllSOiSbPJdO7ovaVfmUD1K9Xgg23+Xn78TWXrRWn+t+Lmpj675Iq+Shw4zitKNbu+C6WKZ8ikKlUEV3hKFc94R2nMP/E9RnDCFbxOxTPe1GdOMuDX");
            result.Append("k49KApxf9fVka6/AIoFu7ZwPW4Ho96kWnqGbckIjD/QEZQFa70HZJq0g8/v4Cs+483JMAq3cgClUL5zADcNnjqu9fFEpSuR2wdlVsPmbbv3YYM25G3R0VuWrQogi/SEUjeDHly/iZwkSqTgLrk1EGzLr1mfLGyuWVVf3ali5s4hAIxMG6+C2EtW9Fecj8ETTpu8mTcz0");
            result.Append("cjpy3kTPiidxEskT3e5e/LGEss23aWrGQp2Ko3urhkgnQ7tRKbel/+plZfW6+6RZHNOqbBAPUww+bRh2Z74W0AGxxOGwefcIOZkQIW94I2juSWlxpnL9ptuKi/spRSvxjkMwefeIdybYWLV3nsU5DRwH6kWJ6zouTSX2+8MImhDj0pT394djzwrhfrYI4uV94tFrMFUF");
            result.Append("VyLDKvnnvAQVqFZ28lKDTnZe7sytbUFYUW3DOLFWK5/Wato089wYxWRT8DqNG/7PnMlhg2CzPnZTtMC2FBEanUxCfCr3tzkq717p729Y32nNiQerYfKD7ji5wanhn/HjKYL0x8didp+PFQ/JT+BJZvzWq7cvEpm7MzgZgEULJyQF9edXZcHwc419DcvDV+F4CweyokqR");
            result.Append("6EmO1paANLYfVmWYUdh4Q11K2r/HMakdh3bBmJZxFWYzWjfSI0yGxEf6+0YoIt9HMnmmjyH68T4S0GmSprk0kxrFJv8DyspSAUVvAAA=");
            return result.ToString();
        }
    }
}

/*<design>
H4sIAAAAAAAACtVbW3PTRhT+K5md6ZsJlnyR7IGHYEPH5dIMAdoZzINsbYKKLLm6ACllhrZACCEDLaVpaJhCyyUwEKDTckka8mcs2Tz1L3RXK1uSHTmyIxuRF0lr6dvdb8/59uzuyTmQ5zSuxKkQZM8BgQfZJBUD44r8FSxrhTzIoqdDXAX9Co5BkZuAymmogBjgS0em
q6iUjoGyLE1oCnqBR0gjqqwrZbi7CPjsTr6k7hzXS6KgniQfjvKlIsBfi+I4p51E35wrKkVpZAS9jn7JomsuW0R/R1WoqPjmM658Cl8JLL5TYFW2fjqN2oOvuF1fwNIoqQKX5PcclHkoWm9NTKvNx9GyWgSxZoWqUKmKsNdK7e6MTUFJ27pyhzKrblz1edR94YBcPoW4
yxBuJ6pcuUmwgzXaBEIffKrjkQFMiU0lqVJmRylNl3ck2TK7g6Uz1I4kZBJJhuETbHwSnI8B9J0uQhVkj5MhzdgVIQirjwixOep4iPGIF9R9ssijoc1OcqIKY6DKKaiL+Of4+RiBoeLxFo5FQP9AjlWZCw+MjYW+gegWkAo1TZCm+m9TogXVePbQvPqgvrrRLxbl9C+H
xnCPLlif9Ih2IgaOcCX3QNKJFPI3rqoJsgSyki6KnoEtSJPyJpXY5tZqHp1Iu1BA/bcV8+6MuTRvXL1nLC4D7xgHwmP8WnVA3nRAOhFYT4t+XjavvDaev60vzxmr1+u3L5p3fni/eAN4emuL1Lh8ZlNqO+rAhU4d5i8ztbVXxsptc+l348rd94v3Gyvv6usrTh37BBHm
oQg1OEFMK1AltKcjVvtb5tReg92DMZFTKr3Ukegkq7Gy8X5hhcMj5kOXNZqByUp7TKS29mf97neN9afm/B+1tTVkKMBj4AcFVcVvBgH2tZVjOjwItZMy3w2m6RMFqV3k6BTd/C1vewp5BT9iBTxrW+o0dlB0nRS5qWYT8D1NHlotTSU8eGk3niWFCJBKpi1Ahg6Al/Tg
MV48ym5gkjSQTgQATHkA2U06nMrQNh4bAI9142Ha3Q2kSQMp0kCMvyVexoNHe/ESBC9OCKTwdSvAdNwDmNi0w2SELXK2wmM9I4JN3tVArOF4hPGw4REOQCDrGRBs6Z14yTRFCOzSPmTkhfyXBYmHZ10GnmQy7eO9H06rjo5YUQKeU45Kwtc6bM0pBTUn6qoGFcjbZc0W
J9m4D6Q1nfSMlm638G2hMT5oRwSkFz1isT5Yn1ehwlli1BNgiu4wHRsQi1lsnMzifGyTjmuKvgV0ygfaCpmDNRNZUH4PmbxysjQpoFk4e9wqzMmiXpEcq6IyadZPkW1zGtM1uSCVFVjB8YDdgRwnHULvthrRXBCAkjAlWGEDeSaAeyW9koeTgtQqEaE0hVcA5ImHk5wu
asc4EfeLlHm1vKCO78cPpHZZQUETbj6y34KaF9SyIlQEidNkJ5TKiZyqkp4QQKtgH7p1ClsRWyad8WPBumzCQ7MaPyJOc0r5JKcEZgKk4qAXJuz6WlRQYVHBxP2oGEdfnUEVDoUOJOHb4oMOjQ9P4Nh49FNj9i8n/jksi32ZR3cnAYdkpcKJI7tHqFhRGuNRD9C9Xfjt
CLVrF41GycVWB1WAAkGZSoTGFB15KUkPXkqYhB8LYzyvQFWNjJqkh6EmSV81kRUtfM9po2FbJIQnIZ7llHn9emPjhSMheaiWgxBBDHbAipoehk4wkdcJZgg64Rt4ucLGHn2jRyoCuAczDI1wB1/AvDlfW19y3MMbo0dBN5khSAZemm1uHXmocYIYGcFghiAYLOVHRmsx
2KNdoMMKqJFvh+EqydCYSEVeOtnBSyfeafAJKtr2QqIgF+wQJBTvlkR/MmEHLpx0PO72EGBcuVx/uebarkZ+PyUr08OQz22G4MnQGIl8uEXOwgapGYgFtvP8x5x9bLy8ZdyY/+/fa8bD70dGvxGqjq3sPdvX+mQQduImaEAaghjyhGG1jTv1W4vGzF3j/mPXWoVDe5Ef
fNHWjY7QhISKR95t6MG7DT63+GimWjchA3MTynPGW67qjVeXGhsz9cV3aLKpvZ1DLmPLSSKObhr3luv3V8k7ifgnrqmoqvcxC/EySkEJM2ztxll4vuQ5szYuXzKe/dpJm8PNQVjpb5IeKj2J0Ohxb5IBc/ZR494148f12tp9l7lUorPv3o2U0MIWynepgzp7WO9rO6Ak
bGcaattT78ZCKjQWvJkYb67W3lwwn94jSSXmwqv3C38XJTvRZOaJ8fplbX0D5YRg4dlYrL15YjxbMFf+MZaWzVsvzPkVY/Wm8Wiuvnal9ma1/njVMa8DnEoyUYKuqNsdL/wFdTd+06Hx6xsc71UUWcnJujVpDzngAfHApzfh+ZvvfmRBtfJvhu9vwVkIz98ykY/6EoOP
+vDpos8ZFs4ji8h+gpuJgYV7NPUR7Kx0YyK0II723XUrqHsl3JZIT8lMSDxYKWHRlgicNTRgiaBwhl30M2bcTAxKIqiUr2NYHQk/O4RkjdvZIXb2LXqie8kI6cZMWJJBpZjOPGJv7nAfoaZQQYnOIYpnNybCWuJROLvWlUVk5VI3LqL086cOHxOolqoWcUbCijcpnB/s
WdnUVx9ZAtrKNiNZlH3wEfr82o2QsEJPCuc3u0zk+bPG8gX3Ak2a0skwfxBlBbkJBIG3x4PREtYKjcIZ067NkbkH5tJs48Ilt6lEKBLrRkloEUjaN9FuaEwECkMGnkWDqPANxoLOLu1EaPBsqAYx+AwaxILn0LPx+rnx7iJJMytKO5vSutOtrXYu+YCPQNH/jSS35TLW
ifE2GEJJ72VcXFVklEakCdY/VJw4/z8NQqsp+DoAAA==
<design>*/

