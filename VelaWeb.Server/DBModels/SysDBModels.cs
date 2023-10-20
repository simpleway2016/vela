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
        protected override string GetDesignString()
        {
            var result = new StringBuilder();
            result.Append("\r\n");
            result.Append("H4sIAAAAAAAACuVca2/TVhj+K5WlfaMoviR2EPvQJWWKxqCiME0ifHDi0+Dh2MGXdV1XadoGLZcKNsa6sqJRqYOCRgbTBqVd1z8TJ+mn/YWdYzuJ7aTO1Y4Nn+pz9TnPec/zXvymi9h5NicABTtxcdF8PMMWAXYCm70q8CrAjmHnpHmzNaOCovlkdeE52PwJK2iwgC8d");
            result.Append("a9arCyXQasFSMmBVYMw9lVd5ScRsffOSqAJRtXVfzJrryGIn4CPPwb8EGT+WxfJsCY2GZVETBFiBpoClLHZBAXJGnJOyGKxNsyqbYxWQScM2Coc1/GkpfwUWYkuwkJIErSgqsHixMT2eTDCe86NesJxRpjRVyoh5GRThmmGTKmsAzcmKZ+AQWDHHCgqq4XLnIQjG4Bxf");
            result.Append("4FFnWGvVWdNPi1oxDeZ40VYnALGgXm6VOTDHaoJqYNOqNfAx9mcik1FmPjKK1nokmQOysbWY0ZrmlbzMF3mRVSXZtsqUwCqKtUtraqPqFHy2VcPjaiGV9ETKfOiIVfOlTbCs1dqw+pyV85dZuV+wslg8ZozpE67Gklp44aPGi4554jUDR8/D10cZM2LkmOEOzLJY/ckP");
            result.Append("9Rt/GqttIndOQhQxMGo9XEn4MkkussLE+xNwQVlxioO7gwWr9qsJ/ORJAh2vE9bRgEqOBNRLaJb0pylJnOMLBueZNRYzGLxpYL5k52TOYlA7v1M4nAzpglkVkrnBpV00AuGvRki4ZKT2S7n6aLm6sarf3NTXt53CMlUwJGNA7UATkdUOiWC1A016IjXFcTJQlJ6vbQe8");
            result.Append("/Ge7RNAagvLWEJKsDgPYANLVM4clglAM4+Qw0oPDLrNioS8OOytwTQO73Ww9A+Y9WkUwb6MmBEDeeD/X2ZwdoZE2ljsYhJUGqz9g81e0knmS3IwslYCs8gCBuWiHAJ2djHiaFRprRlS6tOSnoRdG3Ikw4j5yY3EwEh2htZjFcPNtAZuL3U7GtcwOx2Pqtt7OEJEYBwSg");
            result.Append("OkhsnGxPRZPtw8w8RDweIeppLryD1JosGkq5jfsqty3fqU1oW01OibXJpttNq965Uz944WTnNFDy4wiA9C6ciaCc9fZ7Hj5xS/jr2NOebHZaQtsd1JGnI+vI0wE78t4B8YZ8Da5RBoOrZ8+UDtqJT7p57t5qZX/DyXNnobJhzR5hdgLogMO9jLf9kgYqywvh1g90EPrB");
            result.Append("DhnuCdl5fjgHH+oJoDbmGMP1pHwKHEEAM8oFkb9qrKsxX0ZJCZqiAhlw9rcYCgTNZwxtcR6a1n0sg89qHpV7zuCUOe2vMmfcUfoft6srr/U/3tS2b+m7d2oPvqs+/PZw/a6TKNGOoW3+GcirM9I8kAdW+Ew8sgqfCVbhMwlvJ9I8jQ+1IyELifJiAlb8DB1tQ4kZTwh/");
            result.Append("KM50CONo6fgokg+OkBlfnfmm+9Tmyzdbegk+9eqURkbhDjenzbAfn+AkwxoFImIxpxrOYvrK9drLPafST8F9FiR5IdSpMImgDNcoBILwmK/Go2HjOXzqn5Yre6/08oPqxq/6yqPD9a16+d/aftkpR6d4AaQNnGaBqvJiYUDrEYptZMNFxgYDsx4hUkzHo6reeKq/vK/f");
            result.Append("Xf3vn9v6428mjn/Jl5yHNf3FUMkNvl94J5C+25MQSXcgqXLwsHZ/XV9+pG89dYXN2YWhUml8NCu7wRb9zBDc34RninA7zoa/XH/+uHrzt9ruQSfis8ziKYGVi8NSHx6LLPURwVIfjr8VjrMTtgCIDndLeL6k1V9dqx8s19b/hfZh5c0tSHqW4iBj8KG+uV3b2jX7kLH3");
            result.Append("XNZjSRvCcOQkzbiAPrEhEWhEHWJLuu3t69f05z+3w+vE8GNQDLn93Q1JcuRIUm7D5saT+uZt/fv9yt6WSwKLIU+x6AYeFX2lTPjqAx+lYNs84qM69pPXM75bPF4yJEOS6GPHpGOv5oUNZU4QTkbpKtiE3vuLDZTbc9pQ389z/Ii8ng5Zmt1kO/5uhYeoiIqgO2etsnOz");
            result.Append("svN19fdN0/2qrr06XPsLpvhaPtnyM/31y8r+Afy2iazUg/XKzjP9+Vq1/Le+sV29/6K6WtZ37+lPbtX2Vio7u7Wnu07CPs0q5uL6+mzfgbd9/mrfTboT75Z0x8OZLWyTY+/w5bQsS3JK0ixXfNxBpCxm+gB95ARTI7f2vbPeMooZNwmF8ukbrXdM+fibpUqRHRNb6uWD");
            result.Append("w7Uyiz6IeaS2GB/MhklsgZKajGx8jgw2Pmf85sbjJ6noLMKcoeHEK4DAHIFHOqOlG15vwacHOpp2NeGdYpZRpkWTZqPp3NFvlX6FRetNGBLDVspLQxiR1kIwo38VtABTZIrHM6KaoIw3OXpbEtref1aVoci0D2iJae9jHJs6cmmX0LZQKxRXayCdY+IUnktO5hJEfpJi");
            result.Append("8swkQyTxSQrQJEXTHMnE5rCl/wHEf/EVJkkAAA==");
            return result.ToString();
        }
    }
}

/*<design>
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
<design>*/

