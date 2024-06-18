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
        System.Nullable<Int32> _Flag=0;
        [Column("flag")]
        public virtual System.Nullable<Int32> Flag
        {
            get
            {
                return _Flag;
            }
            set
            {
                if ((_Flag != value))
                {
                    SendPropertyChanging("Flag",_Flag,value);
                    _Flag = value;
                    SendPropertyChanged("Flag");
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
            result.Append("H4sIAAAAAAAACu1d23LTRhh+FUYzveNgHSwpTHuRJtDxFChDaKczmAvF2hgVWQo6NKVpZmgLTQhkoKWUhoYpaTkEBgJ0WghJQ17Gss1VX6GrlWxLsiOfdEy4ine1Wu1+++/3H/aXMo2d5MZFoGIHT01bP49xJYAdxMbOiYIGsL3YCXnKuprTQMn6ZTcReHj5M07UYQGf");
            result.Append("2duo185PguYVbEQBnAZQ38MFTZAlzNG2IEsakDRH8+m8NY48dhD+FHj4lyCze/NYgZs074ZlSRdFWGF2AUt57FMVKDlpQs5jsHaU07hxTgW5UXiNwmGNcEQunIWFzAwsjMiiXpJUWDxV7x4folnf/s1WsJxTh3VNzkkFBZTgmOElTdGB2ScnHYO3wIoJTlTNGn78JAQB");
            result.Append("3TwuFAWzMay16+zuD0l6aRRMCJKjTgRSUTvTLPNggtNFDWHTrEX4oPlZyOTU4x+joj0eWeGBgqaWQVdHBbWgCCVB4jRZcYxyRORU1Z6l3TWqOgx/O6rhcjWRGvJFyvrRFqvGQxtg2aN1YPUlpxTOcEqvYOWxbAbd0yNc9SE18cKDxovJ+OJ1HN49BR+fZsyIwDHDXZjl");
            result.Append("sdrDn2qX/0KjbSB3QjYpom/UutiS8GGyUuLEPR/sgQPKS8M8nB0s2LXf7MHff58wl9cNazCgkoGAetrsZfTzEVmaEIqI86wamxkQbyLMZ5yczNsM6uR3CoedmbpgTINkjri0g0YgwtUItEdGqr+tVu7OVpYWjPllY3HFLSzDRSQZfWoHhkitdqCj1Q4M6YvUMM8rQFW7");
            result.Append("3rZt8Aqf7eioNQTlryFkRRsEsD6kq2sOo6NQDHFyGOnDYWc4qdgTh30i8g0Du9VsPQamfK5KYMpBTSYABfR8vr05G6CRFssejMJKg9UfcoWz+qS1kvxxRZ4EiiYAE8xpJwTm2ikmT3Nifcwmlc7MhGnoJRF3Iom4B24s9keiAVqLeQy3nhaxudhpZTzDbLM8lm7rbg1N");
            result.Append("EuOBCDQXicXJ9lQ62T7JzENksyminsbA20itxaKJlNtsqHLb9J1ahLZ5yS2xDtn0ummVa9dqW8/d7DwK1EIcAZDuhZOOyllv3efJEzc6XMee8WWzI7I53X4deSa1jjwTsSPvHxCvy1f/GqU/uLr2TJmonfghL8/dWChvLrl57hOobDirRZKdACbicC/rb7+MAo0TxGTr");
            result.Append("ByYK/eCEDPeF7KQwmIMP9QTQ6n3EsD2pkAJHEMCc+qkknEPjqveXU0dEXdWAAnjnU5ACMftDtzY5z+zWuyz992otlbfP6JQ5E64yZ71R+p9XKnOvjGevqytXjPVr1dsXK3e+f7t43U2U5oyhbf4FKGjH5Smg9K3w2WxqFT4brcJnaX8n0lqNj/RtIUuI8mIjVvwsk25D");
            result.Append("iY0nhD8QZ7qEMVg63o7koyNkNlRnvuE+tfjyjSvdBJ+6dUpTo3AH69Nh2McnOENJjQIRmYxbDecxY+6H6osNt9IfgfMsysr5RKfC0FEZrmkIBOGZUI1HZOO5fOpfZssbL43V25Wl3425u28X79VW31Q3V91ydFgQwSjCaQxomiAV+7QeodimNlyEJhiZ9QiRYtsuVeXy");
            result.Append("I+PFTeP6wn//XjUefLdn/9fCpHuxDn01UHJD6BveDWTo9iRE0htIKm/dqd5cNGbvGvceecLm3PmBUmlCNCs7wZb+zBA83IRnivA6zshfrj19UJm/X13fakd8tlk8LHJKaVDqwzOppT4iWurD8R3hOLthi4DocK+EFyb12stLta3Z6uIbaB+WX1+BpGcrDjIDf9SWV6r3");
            result.Append("1q02ZOY9j/U4qQ9gOPKyjjZgSGxIRBpRh9iSXnv7h0vG019b4XVjeBSUEm5/d0KSDBxJymvYXH5YW75q/LhZ3rjnkcBSwlMsOoFHpV8pE6H6wNsp2BaPeLuGveT1xLeL4yVDMiGJPk5M2rZqbNhE5gThZJq2gkPo/U9soNye0Ac6Px8XAvJ62mRpdpLt7O4KD1EpFUFv");
            result.Append("zlp5bb68dqHyZNlyvyq3Xr699TdM8bV9stnHxqsX5c0teLZpWqlbi+W1x8bTW5XVf4yllcrN55WFVWP9hvHwSnVjrry2Xn207ibsI5xqDa6nY/s2vB3yqX0n6aZ3l3Rnk5kt7JBj//DlIUWRlRFZt13xuINIeczyAXrICaYCt/b9s95yqhU3SYTy6RmtXaZ8ws1Spci2");
            result.Append("iS211a23t1Y580DMJ7UFHZgNktgCJXUotfE5Mtr4HHrnxueVVHMtkpyh4cYrgsAcgac6o6UTXjvg6IFJp11N+KeY5dRDkkWz6XTumN2lX9lQ9Svd4oNt/Fm9+21t80ll4Y/yxoYxv+yJvso8OCqoaj27vg+limeptCpVBFd0ShXP+kdprB/JPUZwwxW+TsWz/tRnTTLk");
            result.Append("15MPyyKcX/31ZHuvwCKBbu2dDzuBGPSpFp5l2nJCKw8MBGUJWu9h2SadIAv6+ArPevNyLAKtXYQpVE/cwI3BZ05qg3xRKU7kdsDZVbj5m1792GLNeRv0dFYVqEKII/0hEo0QxJcvkmcJEpkkC65DRFsy69bmq+sPbauu6dVwSm8RgVYmDNfB7SSquyvOR+Cpps3ATZqE");
            result.Append("6WUydt5Ez0omcRLpE93+XvyxhbLLt2kaxkKTiuN7q4Yg06Hd6IzX0n/2tLZywXvSLBX1OhskwxSDTxuD3VmvBfRALEk4bN45Qk6lRMhb3gi6cr+ydLl24ZLXikv6KUUn8U5CMHnniHc23Fi1f57FZzo4CrQzMt93XJpO7feHETQRxqVp/+8PJ54Vov1sEcTL/8Rj0GCq");
            result.Append("Br6KDav0n/MSdKha2c1LLTrZfbk3t7YDYcW1DZPEWp18Wrtp28xzcxQzbcHrNW74jjnTwwbhZn3spGiBYyliNDrZlPhU3m9z1F49M95ctL/TmpcO1MPkB7xxcpNToz/jxzMEFYyPxe48HysZkp/Ck8zkrddgXySydmd8MkBmEv52BMH4n1gfFrliuP9eKrRE/ySERiOU");
            result.Append("NDxNX+3vYIa/e2fn3Ts74Zzlh7U9YdEWZLRBm4qovk3NgKfJgrA8dh4qrtL+nKTRFHqSq7W9d1vbj2kKTC1vvaG5gbu/xzWpbYd22pyWeRWmtds3MuNslsLHh/aN00RhH8UW2H0sMYTvowBDUgzDk2xmApv5H+ZuHbZOdQAA");
            return result.ToString();
        }
    }
}

/*<design>
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
<design>*/

