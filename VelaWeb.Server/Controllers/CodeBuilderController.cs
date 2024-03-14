using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Vela.CodeParser;
using VelaWeb.Server.DBModels;
using VelaWeb.Server.Dtos;
using Way.EntityDB;
using Way.Lib;

namespace VelaWeb.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class CodeBuilderController : AuthController
    {
        private readonly SysDBContext _db;
        private readonly CodeParserFactory _codeParserFactory;

        public CodeBuilderController(SysDBContext db, CodeParserFactory codeParserFactory)
        {
            _db = db;
            _codeParserFactory = codeParserFactory;
        }

        [HttpGet]
        public string[] GetSupportLanguages()
        {
            return _codeParserFactory.GetAllLanguages();
        }

        [HttpGet]
        public Task<CodeMissionDto[]> GetItems(long? parentId)
        {
            return (from m in _db.CodeMission
                    where m.ParentId == parentId && m.UserId == this.UserId
                    orderby m.Type, m.Name
                    select new CodeMissionDto
                    {
                        id = m.id,
                        Name = m.Name,
                        Language = m.Language,
                        Type = (CodeMissionDto_TypeEnum)m.Type
                    }).ToArrayAsync();
        }

        [HttpGet]
        public async Task<CodeMissionDto[]> SearchItems(long? parentId, string keyWord)
        {
            var query = _db.CodeMission;
            if (parentId != null)
            {
                var str = $"/{parentId}/";
                query = query.Where(m => m.Path.Contains(str));
            }
            var items = await (from m in query
                               where m.Name.Contains(keyWord) && m.UserId == this.UserId
                               orderby m.Type, m.Name
                               select new CodeMissionDto
                               {
                                   id = m.id,
                                   Name = m.Name,
                                   Path = m.Path,
                                   ParentId = m.ParentId,
                                   Language = m.Language,
                                   UserId = m.UserId,
                                   Type = (CodeMissionDto_TypeEnum)m.Type
                               }).ToArrayAsync();

            foreach (var item in items)
            {
                if (item.Path == null)
                {
                    item.FullName = item.Name;
                    continue;
                }

                var ids = item.Path.Split('/').Where(m => m.Length > 0).Select(m => long.Parse(m)).ToArray();
                var names = (from m in ids
                             select _db.CodeMission.Where(x => x.id == m).Select(x => x.Name).FirstOrDefault()).ToArray();
                item.FullName = string.Join("/", names) + "/" + item.Name;
            }
            return items;
        }

        async ValueTask<string?> getParentPath(long? parentId)
        {
            if (parentId == null)
                return "/";
            else
            {
                return await (from m in _db.CodeMission
                              where m.id == parentId
                              select m.Path + m.id + "/").FirstOrDefaultAsync();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<long> AddMission([FromBody] CodeMissionRequest model)
        {
            var codeContent = model.CodeContent;
            model.CodeContent = null;
            var data = model.ToJsonString().FromJson<CodeMission>();
            if (!string.IsNullOrWhiteSpace(codeContent))
            {
                data.Code = Encoding.UTF8.GetBytes(codeContent);
            }
            data.UserId = this.UserId;

            if (await _db.CodeMission.AnyAsync(m => m.Name == data.Name && m.UserId == data.UserId && m.ParentId == data.ParentId))
                throw new ServiceException("名称重复");
            try
            {
                data.Path = await getParentPath(data.ParentId);
                await _db.InsertAsync(data);
            }
            catch (RepeatException)
            {

                throw new ServiceException("名称重复");
            }
            return data.id.Value;
        }

        [HttpPost]
        public async Task Modify([FromBody] CodeMissionRequest model)
        {
            var data = await _db.CodeMission.FirstOrDefaultAsync(m => m.id == model.id && m.UserId == this.UserId);
            var parentId = data.ParentId;
            if (!string.IsNullOrWhiteSpace(model.CodeContent))
            {
                data.Code = Encoding.UTF8.GetBytes(model.CodeContent);
            }
            else
            {
                data.Code = null;
            }
            data.Name = model.Name;
            data.UserId = this.UserId;
            data.ParentId = parentId;
            if (data.Path == null)
                data.Path = await getParentPath(parentId);

            if (await _db.CodeMission.AnyAsync(m => m.Name == data.Name && m.UserId == data.UserId && m.ParentId == data.ParentId && m.id != data.id))
                throw new ServiceException("名称重复");

            try
            {
                await _db.UpdateAsync(data);
            }
            catch (RepeatException)
            {
                throw new ServiceException($"“{data.Name}”已存在");
            }
        }

        [HttpGet]
        public async Task Delete(long id)
        {
            await _db.DeleteAsync<CodeMission>(m => m.id == id && m.UserId == this.UserId);
            var items = await _db.CodeMission.Where(m => m.ParentId == id).Select(m => m.id.Value).ToArrayAsync();
            foreach (var idvalue in items)
            {
                await Delete(idvalue);
            }
        }

        /// <summary>
        /// 复制代码任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<CodeMissionDto> Clone(long id, long? parentId)
        {


            var data = await (from m in _db.CodeMission
                              where m.id == id && m.UserId == this.UserId
                              select m).FirstOrDefaultAsync();

            if (data.Type == CodeMission_TypeEnum.Folder && parentId != null)
            {
                //判断目标是否属于这个文件夹内
                var checkingParentId = parentId;
                while (checkingParentId != null)
                {
                    if (id == checkingParentId)
                        throw new ServiceException("目标文件夹和复制文件夹同源");

                    checkingParentId = await (from m in _db.CodeMission
                                              where m.id == checkingParentId
                                              select m.ParentId).FirstOrDefaultAsync();
                }
            }

            var codeMission = new CodeMission();
            data.CopyValueTo(codeMission, true, false);
            codeMission.ParentId = parentId;
            codeMission.Path = await getParentPath(parentId);

            var originalName = codeMission.Name = $"{codeMission.Name} 副本";
            if (codeMission.ParentId != data.ParentId)
                codeMission.Name = data.Name;

            var index = 0;

            while (true)
            {
                try
                {
                    if (await _db.CodeMission.AnyAsync(m => m.Name == codeMission.Name && m.UserId == codeMission.UserId && m.ParentId == codeMission.ParentId))
                        throw new RepeatException(new Exception());

                    await _db.InsertAsync(codeMission);

                    if (data.Type == CodeMission_TypeEnum.Folder)
                    {
                        var childrenIds = await (from m in _db.CodeMission
                                                 where m.ParentId == data.id
                                                 select m.id.Value).ToArrayAsync();
                        foreach (var cid in childrenIds)
                        {
                            await Clone(cid, codeMission.id);
                        }
                    }

                    return new CodeMissionDto
                    {
                        id = codeMission.id,
                        Name = codeMission.Name,
                        Language = codeMission.Language,
                        Type = (CodeMissionDto_TypeEnum)codeMission.Type
                    };
                }
                catch (RepeatException)
                {
                    index++;
                    codeMission.Name = $"{originalName} （{index}）";
                }
            }
        }

        /// <summary>
        /// 更改上级
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task ChangeParent(long id, long? parentId)
        {
            var data = await (from m in _db.CodeMission
                              where m.id == id && m.UserId == this.UserId
                              select m).FirstOrDefaultAsync();
            data.ParentId = parentId;
            data.Path = await getParentPath(parentId);
            await _db.UpdateAsync(data);
        }

        /// <summary>
        /// 获取代码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> GetCode(long id)
        {
            var bytes = await (from m in _db.CodeMission
                               where m.id == id && m.UserId == this.UserId
                               select m.Code).FirstOrDefaultAsync();
            if (bytes == null)
                return "";
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// 获取转换脚本
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> GetScript(long id)
        {
            var bytes = await (from m in _db.CodeMission
                               where m.id == id && m.UserId == this.UserId
                               select m.Script).FirstOrDefaultAsync();
            if (bytes == null)
                return "";
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// 获取转换脚本
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SaveScript([FromForm] long id, [FromForm] string script)
        {
            var data = await _db.CodeMission.FirstOrDefaultAsync(m => m.id == id && m.UserId == this.UserId);
            if (!string.IsNullOrWhiteSpace(script))
            {
                data.Script = Encoding.UTF8.GetBytes(script);
            }
            else
            {
                data.Script = null;
            }

            await _db.UpdateAsync(data);
        }

        /// <summary>
        /// 解析代码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> ParserCode(long id)
        {
            var codeModel = await (from m in _db.CodeMission
                                   where m.id == id && m.UserId == this.UserId
                                   select m).FirstOrDefaultAsync();
            if (codeModel == null)
                return new BaseCodeNodeInfo().ToJsonString(true);

            var code = Encoding.UTF8.GetString(codeModel.Code);

            var codeParser = _codeParserFactory.CreateCodeParser(codeModel.Language);
            var ret = codeParser.Parser(code);
            if (ret.Items.Count() == 1)
                return ret.Items.First().ToJsonString(true);
            else
                return ret.ToJsonString(true);
        }

        [HttpPost]
        public async Task<string> ParserCodeV2([FromForm] string code, [FromForm] long id)
        {
            var codeModel = await (from m in _db.CodeMission
                                   where m.id == id && m.UserId == this.UserId
                                   select m).FirstOrDefaultAsync();
            if (codeModel == null)
                return new BaseCodeNodeInfo().ToJsonString(true);

            if (string.IsNullOrWhiteSpace(code))
                return new BaseCodeNodeInfo().ToJsonString(true);


            var codeParser = _codeParserFactory.CreateCodeParser(codeModel.Language);
            var ret = codeParser.Parser(code);
            if (ret.Items == null)
                return ret.ToString();
            else if (ret.Items.Count() == 1)
                return ret.Items.First().ToJsonString(true);
            else
                return ret.ToJsonString(true);
        }

        [HttpGet]
        public async Task<string?> GetVueMethod()
        {
            var code = await (from m in _db.VueMethod
                              where m.UserId == this.UserId
                              select m.Code).FirstOrDefaultAsync();
            if (code == null)
            {
                code = @"{
    findNode : function(parentNode,nodeType,name){
        if(!parentNode) return null;
        if(parentNode.NodeType == nodeType && (!name || parentNode.Name == name))
            return parentNode;

        if(!parentNode.Items) return null;

        var classObj = parentNode.Items.find(m=>m.NodeType == nodeType && (!name || m.Name == name));
        if(classObj)
            return classObj;
        for(var i = 0 ; i < parentNode.Items.length ; i ++){
            classObj = this.findNode( parentNode.Items[i] , nodeType , name );
            if(classObj)
                return classObj;
        }
        return null;
    }
}";


            }
            return code;
        }

        [HttpPost]
        public async Task SaveVueMethod([FromForm] string code)
        {

            var data = await _db.VueMethod.FirstOrDefaultAsync(m => m.UserId == this.UserId);
            if (data == null)
            {
                data = new VueMethod
                {
                    UserId = this.UserId,
                    Code = code
                };
                await _db.InsertAsync(data);
            }
            else
            {
                data.Code = code;
                await _db.UpdateAsync(data);
            }

        }
    }
}
