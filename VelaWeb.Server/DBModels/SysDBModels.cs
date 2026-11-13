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
    public partial class UserInfo :Way.EntityDB.DataItem
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
        string _Name;
        [MaxLength(50)]
        [DisallowNull]
        [DesignColumn(TypeName = "varchar")]
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
        [DesignColumn(TypeName = "varchar")]
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
        [DesignColumn(TypeName = "int")]
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
        [DesignColumn(TypeName = "int")]
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
        string? _GoogleCode;
        [MaxLength(255)]
        [DesignColumn(TypeName = "varchar")]
        [Column("googlecode")]
        public virtual string? GoogleCode
        {
            get
            {
                return _GoogleCode;
            }
            set
            {
                if ((_GoogleCode != value))
                {
                    SendPropertyChanging("GoogleCode",_GoogleCode,value);
                    _GoogleCode = value;
                    SendPropertyChanged("GoogleCode");
                }
            }
        }
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param>指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
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
    public partial class Agent :Way.EntityDB.DataItem
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
        string _Address;
        [MaxLength(50)]
        [DisallowNull]
        [DesignColumn(TypeName = "varchar")]
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
        [DesignColumn(TypeName = "int")]
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
        /// <summary>把字段的更新，设置为一个指定的表达式值</summary>
        /// <param>指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<Agent, bool>> exp)
        {
            base.SetValue<Agent>(exp);
        }
    }
    [TableConfig]
    [Table("logs")]
    [Way.EntityDB.DataItemJsonConverter]
    public partial class Logs :Way.EntityDB.DataItem
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
        Int64 _UserId;
        [DisallowNull]
        [DesignColumn(TypeName = "bigint")]
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
        [DesignColumn(TypeName = "varchar")]
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
        [DesignColumn(TypeName = "varchar")]
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
        [DesignColumn(TypeName = "datetime")]
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
        /// <param>指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<Logs, bool>> exp)
        {
            base.SetValue<Logs>(exp);
        }
    }
    /// <summary>用户对程序的权限</summary>
    [TableConfig]
    [Table("userprojectpower")]
    [Way.EntityDB.DataItemJsonConverter]
    public partial class UserProjectPower :Way.EntityDB.DataItem
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
        string _ProjectGuid;
        [MaxLength(50)]
        [DisallowNull]
        [DesignColumn(TypeName = "varchar")]
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
        [DesignColumn(TypeName = "bigint")]
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
        /// <param>指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<UserProjectPower, bool>> exp)
        {
            base.SetValue<UserProjectPower>(exp);
        }
    }
    /// <summary>文件定期删除设置</summary>
    [TableConfig]
    [Table("filedeletesetting")]
    [Way.EntityDB.DataItemJsonConverter]
    public partial class FileDeleteSetting :Way.EntityDB.DataItem
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
        string _Ext;
        /// <summary>文件扩展名，如 .zip</summary>
        [MaxLength(50)]
        [DisallowNull]
        [Display(Name = "文件扩展名，如 .zip")]
        [DesignColumn(TypeName = "varchar")]
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
        [DesignColumn(TypeName = "int")]
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
        /// <param>指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<FileDeleteSetting, bool>> exp)
        {
            base.SetValue<FileDeleteSetting>(exp);
        }
    }
    /// <summary>程序警报线设置</summary>
    [TableConfig]
    [Table("projectalarmsetting")]
    [Way.EntityDB.DataItemJsonConverter]
    public partial class ProjectAlarmSetting :Way.EntityDB.DataItem
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
        string _ProjectGuid;
        [MaxLength(50)]
        [DisallowNull]
        [DesignColumn(TypeName = "varchar")]
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
        [DesignColumn(TypeName = "double")]
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
        [DesignColumn(TypeName = "double")]
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
        [DesignColumn(TypeName = "varchar")]
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
        [DesignColumn(TypeName = "bit")]
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
        [DesignColumn(TypeName = "datetime")]
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
        [DesignColumn(TypeName = "bit")]
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
        /// <param>指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<ProjectAlarmSetting, bool>> exp)
        {
            base.SetValue<ProjectAlarmSetting>(exp);
        }
    }
    /// <summary>用户访问agent的权限</summary>
    [TableConfig]
    [Table("useragentpower")]
    [Way.EntityDB.DataItemJsonConverter]
    public partial class UserAgentPower :Way.EntityDB.DataItem
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
        Int64 _AgentId;
        [DisallowNull]
        [DesignColumn(TypeName = "bigint")]
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
        [DesignColumn(TypeName = "bigint")]
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
        /// <param>指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
         public virtual void SetValue(System.Linq.Expressions.Expression<Func<UserAgentPower, bool>> exp)
        {
            base.SetValue<UserAgentPower>(exp);
        }
    }
    /// <summary>代码转换任务</summary>
    [TableConfig]
    [Table("codemission")]
    [Way.EntityDB.DataItemJsonConverter]
    public partial class CodeMission :Way.EntityDB.DataItem
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
        string _Name;
        [MaxLength(50)]
        [DisallowNull]
        [DesignColumn(TypeName = "varchar")]
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
        [DesignColumn(TypeName = "int")]
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
        [DesignColumn(TypeName = "image")]
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
        [DesignColumn(TypeName = "image")]
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
        [DesignColumn(TypeName = "bigint")]
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
        [DesignColumn(TypeName = "varchar")]
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
        [DesignColumn(TypeName = "bigint")]
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
        [DesignColumn(TypeName = "varchar")]
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
        /// <param>指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
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
    public partial class VueMethod :Way.EntityDB.DataItem
    {
        Int64 _UserId;
        [Key]
        [DisallowNull]
        [DesignColumn(TypeName = "bigint")]
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
        [DesignColumn(TypeName = "text")]
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
        /// <param>指定的更新表达式，如 m=&gt;m.age == m.age + 1 &amp;&amp; name == name + "aa"，相当于sql语句的 age=age+1,name=name + 'aa'</param>
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
            result.Append("H4sIAAAAAAAACu1dbXPURBz/KkxmfAd6ebgkZfBFbcG5EZCh6Djj8SK9bI9oLql5sCB2BhUoFDv4hFitI1WU4kARR6G2ln6Zy93xyq/gZpO7S3LX3FMeNi2vetlsNru//e/v/7D/TS8SZ4RpGejE4XcvOj9PChVAHCamPpAlAxAHidPqnHO3YICK88utIonw9tuCbMIL");
            result.Append("cv5gq9y4MAvad4gJDQgGQG2PlwxJVQhP3ZKqGEAxPNUvFp1+FInD8Kckwr8UnT9YJErCrP00vFZMWYYFdhPwqki8pQOtoMyoRQKWTgqGMC3ooDAJ7zEkLJGOq6X34UVuHl5MqLJZUXR4+W6zeXKM5UPbt2vB64I+bhpqQSlpoAL7DG8ZmgnsNgXlJHwEFswIsm6XiNNn");
            result.Append("IAjo4WmpLNmVYalb5jZ/VDErk2BGUjxlMlDKxrn2tQhmBFM2EDbtUoQPGp+DTEE/9Qa6dPujaiLQ0NBy6O6kpJc0qSIpgqFqnl5OyIKuu6N0m0ZFx+BvTzGcrjZSY6FIOT+6YtV6aQsst7cerD4UtNI5QRsUrCKRz6FnBoSr2aU2XmTUeHG5ULxOwafn4OuzjBkVOWak");
            result.Append("D7Mi0bj3VeP6n6i3LeROqzZFDI1aH0sSvkzVKoJ84NUDsENFZVyEo4MXbunHB8gjRyh7ev2wRgMqHQmoZ+1WJt+ZUJUZqYw4zylxmQHxJsJ83svJosugfn4/i1TBlAG5HFFpD4VAxasQ2ICI1H9Yr91ZqK0sWYur1vKaX1bGy0gwhlQOHJVZ5cAmqxw4OhSpcVHUgK73");
            result.Append("vWq74BU/2bFJKwgmXEGomjEKYENIV98UxiahF1KkMDqEws4JSnkgCntTFlvmdafRehLMhdxVwJyHmezxl9D7xe7GbIQmWipLMAkbDRa/JpTeN2edmRRPaeos0AwJ2GBe9EJgz51m07QgN/tsM+n8fJxmHo64UzjiHrmpOByHRmgrFgnSeVvCxmKvmQl0s8v0OKqtvzm0");
            result.Append("SUwEMjB8JJYi2TPZJHuciYfK5zPEPK2OdxFah0RxFNt8rGLbdpw6ZLZ9yy+wHtEM+mi1mzcbO3/4uXkS6KU0gh/9yyablKPeucyxkzY2XqeeC+Wy46o92mGdeC6zTjyXsBMfHgtvitfw+mQ4uPr2SrmkHfixIM19vVTdXvHT3JtQ1QhODZw9AC7hSC8fbr1MAkOQZLzV");
            result.Append("A5eEevBCRoZCdkYazbuHagIYzTZSWJ5MTEEjCGBBf0uRPkD9arZX0CdkUzeABkTvW5ACsdtDj7Y5z242OC3Dt+pMVbDNxHQ5F68u54MB+m/WateeWo/+qa/dsDZv1r+/XPvxs+fLX/h50h4wNMzfAyXjlDoHtKH1PZ/PrL7nk9X3PBvuQTqz8bq5K2SY6C4+Yb3Pc9m2");
            result.Append("k/h0ovcjUaZPGKNl4904PjE+5mP15FvOU4cj37rTT+CpX480M+p2tDY9Zn1qcjOGawSIyuX8SrhIWNeu1h9v+VX+BBxnWdUuYJ0CwyZltWYgCETmYrUckYHn86e/XahuPbHWv6+t/GRdu/N8+W5j/Vl9e90vRsckGUwimKaAYUhKeUjTEUptZkNFaICJmY4QKb7rVNWu");
            result.Append("37ce37K+WPrv38+t3z498PJH0qx/so6eHympIfb17gcydmMSIhkMIlV3fqzfWrYW7lh37wci5sKFkVJoYrQpe8GW+YwQMt40Z4YKOs3IV248/K22+Gt9c6cb77km8bgsaJVRmY/MZZb5qGSZjyT3hNPshy0BniODEl6aNRtPrjR2FurLz6B1WP3nBuQ8V2/QOfijsbpW");
            result.Append("v7vp1KFzLwVsx1lzBLNRVE20AGMiQyrRYDrElg5a21evWA+/64TXj+EJUMHc+u6FJB05kkzQrrl+r7H6ufXldnXrbkACK5jnVvQCj8m8TqZidYB3068d7vBuFQfJ50lvEafLhTQmCT5eTLrWaq1XHHOBSDpLK8Ej8+F7NVBsT5sjbZxPSxG5PF1yM3uJdn5fhYaYjEpg");
            result.Append("MFWturFY3bhUe7Dq+F6120+e3/4L5vW6DtnC79bTx9XtHbipaZuoO8vVjd+th7dr639bK2u1W3/Ultatza+tezfqW9eqG5v1+5t+uj4u6E7nBtqu78LaMe/W9xJudl8Jdx7PFGGPGIdHLo9qmqpNqKbrhqcdPyoSjv0/QCIwE7mlH57sVtCdmAkWqmdgtPaX6ok3N5Wh");
            result.Append("u+azNNZ3nt9eF+ydsJCMFrRTNko+CxTUscyG5uhkQ3PomE3IIVR7LnBOzPDjlUBMjiIzncjSC6/sbzpw2TSqqfDEsoJ+VHFYNpuOHbevtCsfq3ZlO/yvrV/qdz5pbD+oLf1c3dqyFlcDYVdVBCckXW9m1A+hUsk8k1WViuBKTqWS+fAAjfMD3/0DP1zxa1QyH858ziBj");
            result.Append("Po98TJXh+Jrnkd21Ai8p9OjgdNgLxKi3s8g815UTOnlgJCgr0HaPyzLpBVnU+1ZkPpiP4xBo4zJMnXrgB24KvnPWGOUDSmkil/1Nq3izNoPqscOWC1YYaJMqUn2QRtpDIgohii9dYGcHUjmc5dYjoR35dBuL9c17rk3XdmkEbbBoQCcPxuvc9pLUfRXio8hMk2bk9gxm");
            result.Append("SplOnTXRu7CkTSp7kjvcUR9XJvs8P9OyFNpEnNo5GorOhmpjc0Ej/9HDxtql4AazUjabXICHGQbfNgWbc04CDEArOOwx7xkZZzIi4x1ngG78Wlu53rh0JWjB4b470Uu6cYgi7xnpzscbpA7PrnjbBCeAcU4Vhw5Is5n90DCCJsGANBv+oWHsSSHZbxRBvMK3OkaNohrg");
            result.Append("fGpYZX57l2Jj1cl+WurQyP7bg3m0PfgqrVWIE2n1cmfdql1zze1ezHcFb9CA4QvizAwZxJvrsZcCBZ6ZSM/i5DPiTwU/xNF4+sh6dtn9IGtReaUZHn8lGB+3GTX5nX0yRzHR+Ff83vOvsBD8DO5f4jddo318yFmcqYkAncP8OATFhW9TH5OFcrz/QSq2zH4cYqLJCRqZ");
            result.Append("pU/z97DAX5zReXFGJ54N/LRWJ4W3GmCC33LwW7ivq2pZBiNuu3exc933BJIyg7mXu3zFwa3YVghkVELkthwUIrsYF3UAL90XIZFrmz1NwbND6zbK8HrqAjSTKi8XFINl0It8tV1p7Kw/ZWjw8ELnA22R7P8Z35h27dpZe1j2XXhwwn2Qm+bzDDk9dmiapUqHGL7EH+Kp");
            result.Append("MfIQAzia4TiR5nMzxPz/mPoZ2qB3AAA=");
            return result.ToString();
        }
    }
}

/*<design>
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
<design>*/

