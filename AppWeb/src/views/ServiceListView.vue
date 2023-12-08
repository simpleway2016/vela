<script setup lang="ts">

import { GlobalInfo } from "@/GlobalInfo";
import Loading from "@/components/Loading.vue";
import ProjectOutputInfo from "@/views/ProjectOutputInfo.vue"
import ConfigFileEditor from "./ConfigFileEditor.vue";
import Percent from "@/components/Percent.vue";
import { ref, shallowRef, onMounted, onUpdated, onUnmounted } from "vue"
import Selector from "@/components/Selector.vue";
import AlarmSettingView from "./AlarmSettingView.vue";
import ProjectUserList from "./ProjectUserList.vue";
import RestoreProject from "./RestoreProject.vue";
import { WebSocketConnection } from "@/WebSocketConnection";
import { useRoute, onBeforeRouteUpdate, useRouter } from "vue-router";
import { POSITION, useToast } from "vue-toastification";

const defaultDockerfileContent = `FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /vela/app
ENTRYPOINT ["dotnet", "yourApplication.dll"]`;

const isBusy = ref(false);
const editingModel = ref(<any>null);
const agents = ref(<any[]><any>null);
const groupAgents = ref(<any[]><any>[]);
const allCategories = ref(<any[]><any>[]);
const editorCategories = ref(<any[]><any>[]);
const txtCmd = ref(<HTMLTextAreaElement><any>null);
const ProjectListProperties = GlobalInfo.ProjectListProperties;
const datas = ref(<any[]>[]);
const isLoadingBranches = ref(false);
const branches = ref(<any[]><any>null);
const toast = useToast();

let childView = shallowRef(<any>null);
const publishingInfo = ref("");
const currentGuid = ref("");
const currentProject = ref(<any>null);

const websocketDisconneted = ref(false);
const dockerfileContent = ref("");
const route = useRoute();
const router = useRouter();

if (route.params.search != undefined) {
    ProjectListProperties.searchKey = <string>route.params.search;
}

var webSocket: WebSocketConnection;
var unmounted = false;



onMounted(() => {
    init();
    refreshPublishing();
});

const refreshPublishing = async () => {
    try {
        var ret = await GlobalInfo.get("/AgentService/GetBuildingProjects", null);
        ret = JSON.parse(ret);
        var str = "";
        if (ret && ret.length) {
            for (var i = 0; i < ret.length; i++) {
                str += ret[i];
                if (i < ret.length - 1)
                    str += "; ";
            }
        }
        publishingInfo.value = str;
    } catch (error) {

    }
    finally {
        window.setTimeout(()=>{ refreshPublishing(); } , 2000);
    }
}

const init = async () => {
    if (GlobalInfo.UserInfo.Token) {

        refreshDatas();

        var ret = await GlobalInfo.get("/AgentService/GetAgents", null);
        agents.value = JSON.parse(ret);

        var arr = <any[]>[];
        agents.value.forEach(m => {
            m.Name = m.Address + ":" + m.Port + " " + m.Desc;

            if (arr.some(x => x.Category == m.Category) == false) {
                arr.push(m);
            }

        });

        arr = arr.sort(function (a, b) {
            return a.Category.localeCompare(b.Category);
        });
        groupAgents.value.push(...arr);
    }
    else {
        window.setTimeout(() => init(), 100);
    }

}

onUnmounted(() => {
    unmounted = true;
    if (webSocket) {
        webSocket.stop();
    }
});

onBeforeRouteUpdate((to, from) => {
    ProjectListProperties.searchKey = <string>to.params.search;
    console.log("路由变更，search:", ProjectListProperties.searchKey);
    refreshDatas();
});

const decoder = new TextDecoder("utf-8");
const onWebSocketMessage = async (data: any) => {
    console.log(data);
    if (typeof data == "object") {
        var blobData: Blob = data;
        var guidBytes = await blobData.slice(0, 32).arrayBuffer();
        var guid = decoder.decode(guidBytes);
        console.log("解码后guid", guid);

        var termData = await blobData.slice(34).arrayBuffer();
        GlobalInfo.WebSocketReceivers.forEach(m => {
            m(termData, guid);
        });
        return;
    }
    else if (data.startsWith("Modify:")) {
        data = data.substr(7);
        data = JSON.parse(data);
        var index = datas.value.findIndex(m => m.Guid == data.Guid);
        if (index >= 0) {
            datas.value.splice(index, 1, data);
        }
        return;
    }
    else if (data.startsWith("Delete:")) {
        data = data.substr(7);
        var index = datas.value.findIndex(m => m.Guid == data);
        if (index >= 0) {
            datas.value.splice(index, 1);
        }
        return;
    }
    data = JSON.parse(data);
    if (Array.isArray(data)) {
        data.forEach(item => {
            var project = datas.value.find(m => m.Guid == item.Guid);
            if (project) {
                if (!project.noSetWhenEmpty || new Date().getTime() - project.noSetWhenEmpty >= 5000) {
                    project.isLoadingInfo = false;
                    if (project.noSetWhenEmpty) {
                        project.noSetWhenEmpty = 0;
                        project.Status = "";
                    }
                    project.Offline = false;
                    project.CpuPercent = item.CpuPercent;
                    project.MemoryPercent = item.MemoryPercent;
                    project.ProcessId = item.ProcessId;
                }
            }
        });
    }
    else if (data.IsError) {
        datas.value.forEach(item => {
            if (item.OwnerServer.Address == data.Server.Address && item.OwnerServer.Port == data.Server.Port) {
                item.OwnerServer.Offline = true;
            }
        });
    }
    else {
        var project = datas.value.find(m => m.Guid == data.Guid);
        if (project) {
            if (data.Status == null)
                data.Status = "";
            if (data.Error == null)
                data.Error = "";

            project.Status = data.Status;
            project.Error = data.Error;
        }
    }
}

const refreshDatas = async () => {
    if (isBusy.value)
        return;

    isBusy.value = true;
    try {
        console.log("searchKey:", ProjectListProperties.searchKey);
        var ret = await GlobalInfo.get("/AgentService/GetProjects", { search: ProjectListProperties.searchKey });
        console.log(ret);
        ret = JSON.parse(ret);

        ret.forEach((m: any) => {
            if (!m.Status)
                m.Status = "";

            if (!m.Error)
                m.Error = "";

            m.MemoryPercent = 0;
            m.CpuPercent = 0;
            m.OwnerServer.Offline = false;
            m.isLoadingInfo = true;

        });
        datas.value.splice(0, datas.value.length, ...ret);

        refreshAllCategories();


        if (!webSocket && !unmounted) {
            webSocket = new WebSocketConnection("/WebSocket");
            webSocket.onMessage = (sender, data) => {
                onWebSocketMessage(data);
            };
            webSocket.onDisconnect = () => {
                websocketDisconneted.value = true;
            };
            webSocket.onConnect = () => {
                websocketDisconneted.value = false;
                webSocket.keepAlive();
            };
            webSocket.listen();
        }
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const refreshAllCategories = () => {
    var arr = <any[]>[];
    allCategories.value.splice(0, allCategories.value.length);

    datas.value.forEach((project: any) => {
        if (!GlobalInfo.ProjectListProperties.selectedAgentCategory) {
            if (project.Category && arr.some(x => x[1] == project.Category) == false) {
                arr.push([project.OwnerServer.Category, project.Category]);
            }

            if (project.Category && GlobalInfo.ProgramCategories.some(x => x[1] == project.Category) == false) {
                GlobalInfo.ProgramCategories.push([project.OwnerServer.Category, project.Category]);
            }
        }
        else {
            if (project.Category && project.OwnerServer.Category == GlobalInfo.ProjectListProperties.selectedAgentCategory && arr.some(x => x[1] == project.Category) == false) {
                arr.push([project.OwnerServer.Category, project.Category]);
            }

            if (project.Category && project.OwnerServer.Category == GlobalInfo.ProjectListProperties.selectedAgentCategory && GlobalInfo.ProgramCategories.some(x => x[1] == project.Category) == false) {
                GlobalInfo.ProgramCategories.push([project.OwnerServer.Category, project.Category]);
            }
        }
    });

    arr = arr.sort(function (a, b) {
        return a[1].localeCompare(b[1]);
    });

    GlobalInfo.ProgramCategories = GlobalInfo.ProgramCategories.sort(function (a, b) {
        return a[1].localeCompare(b[1]);
    });

    editorCategories.value = GlobalInfo.ProgramCategories;

    allCategories.value.push(...arr);
    if (allCategories.value.some(x => x[1] == ProjectListProperties.selectedProjectCategory) == false) {
        ProjectListProperties.selectedProjectCategory = "";
    }
}


const addClick = () => {
    editingModel.value = {
        "OwnerServer": { "id": 0, "Address": "", "Port": 0, Offline: false },
        PublishPathMode: 0,
        "User": null, "id": null, "Name": "", "ExcludeFiles": "", "RunCmd": null, "PublishPath": null,
        "ConfigFiles": "", "GitUrl": "", "BranchName": "", "PublishMode": 0, "IsNeedBuild": false, "ProgramPath": "", "BuildCmd": "",
        "Status": 0, "CpuRate": null, "MemoryRate": null, "GitUserName": "", "GitPwd": "", "BuildPath": "", "Guid": "", "ProcessId": null,
        "GitRemote": "origin", "IsHostNetwork": false, MemoryLimit: "", DeleteNoUseFiles: true,
        "CodePath": "", "Desc": null, "UserId": null, RunType:1,
    };

    dockerfileContent.value = defaultDockerfileContent;

    branches.value = <any>null;
}

const copyNew = async (item: any) => {
    var newItem = JSON.parse(JSON.stringify(item));
    newItem.OwnerServer = { "id": 0, "Address": "", "Port": 0, Offline: false };
    newItem.id = null;
    newItem.Guid = null;
    newItem.DockerContainerId = null;
    newItem.ProcessId = null;

    isBusy.value = true;
    try {
        var pwd = await GlobalInfo.get("/AgentService/GetGitPwd", { guid: item.Guid });
        if ((item.RunType & 2) == 2) {
            dockerfileContent.value = await GlobalInfo.get("/AgentService/GetDockerfile", { guid: item.Guid });
        }
        else {
            dockerfileContent.value = defaultDockerfileContent;
        }
        newItem.GitPwd = pwd;
    } catch (error) {
        dockerfileContent.value = "";
        GlobalInfo.showError(error);
        return;
    }
    finally {
        isBusy.value = false;
    }

    if (newItem.PublishPath) {
        newItem.PublishPathMode = 1;
    }
    else {
        newItem.PublishPathMode = 0;
    }

    editingModel.value = newItem;



    loadBranches();
}

const publish = async (item: any) => {
    if(isBusy.value){
        GlobalInfo.showError("正在加载数据，请稍后再试...");
        return;
    }

    isBusy.value = true;
    try {
        await GlobalInfo.get("/AgentService/Run", { guid: item.Guid });
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally{
        isBusy.value = false;
    }
}

const editProjectUser = (item: any) => {
    childView.value = ProjectUserList;
    currentGuid.value = item.Guid;
}

const viewOutputs = async (item: any) => {
    currentProject.value = item;
    childView.value = ProjectOutputInfo;
    currentGuid.value = item.Guid;
}

const editConfigFile = async (item: any) => {
    currentProject.value = item;
    childView.value = ConfigFileEditor;
    currentGuid.value = item.Guid;
}

const getRunTypeStr = (item:any)=>{
    if(item.RunType == (1<<5 | 1<<1)){
        return "Docker";
    }
    else{
        return "Prog";
    }
}

const editClick = async (item: any) => {
    if (isBusy.value)
        return;

    if (item.GitPwd == "") {
        isBusy.value = true;
        try {
            var pwd = await GlobalInfo.get("/AgentService/GetGitPwd", { guid: item.Guid });
            item.GitPwd = pwd;
        } catch (error) {
            GlobalInfo.showError(error);
            return;
        }
        finally {
            isBusy.value = false;
        }
    }

    editingModel.value = JSON.parse(JSON.stringify(item));
    loadBranches();

    if ((editingModel.value.RunType & 2) == 2) {
        isBusy.value = true;
        try {
            dockerfileContent.value = await GlobalInfo.get("/AgentService/GetDockerfile", { guid: editingModel.value.Guid });
        } catch (error) {
            GlobalInfo.showError(error);
        }
        finally {
            isBusy.value = false;
        }
    }
    else {
        dockerfileContent.value = defaultDockerfileContent;
    }

    if (editingModel.value.PublishPath) {
        editingModel.value.PublishPathMode = 1;
    }
    else {
        editingModel.value.PublishPathMode = 0;
    }

    window.setTimeout(()=>{
        cmdInput(<any>{ target : txtCmd.value });
    } , 1000);
}

const restore = async (item: any) => {
    childView.value = RestoreProject;
    currentProject.value = item;
    currentGuid.value = item.Guid;
}

const alarmSetting = async (item: any) => {
    childView.value = AlarmSettingView;
    currentGuid.value = item.Guid;
}

const globalAlarmSettingClick = () => {
    childView.value = AlarmSettingView;
    currentGuid.value = "global";
}

const deleteClick = async (item: any) => {
    if (window.confirm(`确定删除${item.Name}吗？`)) {
        isBusy.value = true;
        try {
            await GlobalInfo.postJson("/AgentService/DeleteProject", item);
            var index = datas.value.findIndex(x => x.id == item.id);
            datas.value.splice(index, 1);
        } catch (error) {
            GlobalInfo.showError(error);
        }
        finally {
            isBusy.value = false;
        }
    }
}
const restartClick = async (item: any) => {
    if (window.confirm(`确定重启${item.Name}吗？`)) {
        isBusy.value = true;
        try {
            await GlobalInfo.get("/AgentService/StopProject", { guid: item.Guid });
            //设置一个时间，websocket推过来的状态里如果没有这个工程的信息，在时间没有超过5秒之前，不能变更状态
            item.noSetWhenEmpty = new Date().getTime();
            item.ProcessId = 0;
            item.Status = "正在重启";
            await GlobalInfo.get("/AgentService/StartProject", { guid: item.Guid });
        } catch (error) {
            GlobalInfo.showError(error);
        }
        finally {
            isBusy.value = false;
        }
    }
}

const stopClick = async (item: any) => {
    if (window.confirm(`确定停止${item.Name}吗？`)) {
        isBusy.value = true;
        try {
            await GlobalInfo.get("/AgentService/StopProject", { guid: item.Guid });
            //设置一个时间，websocket推过来的状态里如果没有这个工程的信息，在时间没有超过5秒之前，不能变更状态
            item.noSetWhenEmpty = new Date().getTime();
            item.ProcessId = 0;
        } catch (error) {
            GlobalInfo.showError(error);
        }
        finally {
            isBusy.value = false;
        }
    }
}

const startClick = async (item: any) => {
    isBusy.value = true;
    try {
        await GlobalInfo.get("/AgentService/StartProject", { guid: item.Guid });
        //设置一个时间，websocket推过来的状态里如果没有这个工程的信息，在时间没有超过5秒之前，不能变更状态
        item.noSetWhenEmpty = new Date().getTime();
        if ((item.RunType & 2) == 2) {
            item.ProcessId = -2;
        }
        else {
            item.ProcessId = 1;
        }
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const loadBranches = async () => {
    if (isLoadingBranches.value || !editingModel.value.GitUrl)
        return;

    branches.value = <any>null;
    isLoadingBranches.value = true;
    try {
        //console.log("编辑", JSON.stringify(editingModel.value));
        var names: any = await GlobalInfo.postJson("/AgentService/GetBranchNames", editingModel.value);
        console.log(names);
        names = JSON.parse(names);
        if (names.length > 0)
            branches.value = names;
        else {
            branches.value = <any>null;
        }
    } catch (error) {
        branches.value = <any>null;
    }
    finally {
        isLoadingBranches.value = false;
    }
}

const viewUrl = () => {
    publishPath.value = `${GlobalInfo.ServerUrl}/agentservice/RunWithUser?guid=${editingModel.value.Guid}&username=用户名&pwd=密码`;
}

const publishPath = ref("");
const viewPublishPath = async () => {
    if (isBusy.value)
        return;

    isBusy.value = true;
    try {
        publishPath.value = await GlobalInfo.postJson("/AgentService/GetPublishPath", editingModel.value);

    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const okClick = async () => {
    if (isBusy.value)
        return;

    var server = agents.value.find(m => m.id == editingModel.value.OwnerServer.id);
    if (!server) {
        toast.error("请选择所属服务器", {
            position: POSITION.BOTTOM_CENTER,
        });
        return;
    }

    if (editingModel.value.GitUrl && editingModel.value.GitUrl.trim() && !editingModel.value.BranchName) {
        toast.error("请选择分支", {
            position: POSITION.BOTTOM_CENTER,
        });
        return;
    }

    editingModel.value.OwnerServer = server;
    if (editingModel.value.PublishPathMode == 0) {
        editingModel.value.PublishPath = "";
    }

    if (editingModel.value.Category) {
        editingModel.value.Category = editingModel.value.Category.trim();
    }

    isBusy.value = true;
    try {
        console.log("当前提交数据", JSON.stringify(editingModel.value));
        //console.log("当前提交数据", editingModel.value.PublishPathMode , editingModel.value.PublishPath);
        if (editingModel.value.Guid) {
            await GlobalInfo.postJson("/AgentService/ModifyProject", editingModel.value);
            var index = datas.value.findIndex(x => x.Guid == editingModel.value.Guid);
            datas.value.splice(index, 1, editingModel.value);
        }
        else {
            var guid = await GlobalInfo.postJson("/AgentService/AddProject", editingModel.value);
            var data = editingModel.value;
            data.Guid = guid;
            data.User = GlobalInfo.UserInfo.Name;
            datas.value.splice(0, 0, data);


        }
        //刷新程序分类列表
        refreshAllCategories();

        //保存dockerfile
        if ((editingModel.value.RunType & 2) == 2) {
            await GlobalInfo.post("/AgentService/SaveDockerfile", { guid: editingModel.value.Guid, content: dockerfileContent.value });
        }

        cancelClick();
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const cancelClick = () => {
    editingModel.value = null;
    branches.value = <any>null;
    publishPath.value = "";
}

const pageContentEle = ref(<HTMLElement><any>null);
/**自动修正菜单的y轴位置 */
const menuClick = (e: Event) => {
    var aEle = <HTMLElement>e.target;
    var menuContainerEle = <HTMLElement>aEle.nextSibling;
    (<any>window).$(".dropdown").on("shown.bs.dropdown", () => {
        (<any>window).$(".dropdown").off("shown.bs.dropdown");

        var rect = aEle.getBoundingClientRect();
        var rect2 = pageContentEle.value.getBoundingClientRect();
        //如果a标签的 y + a.height + menu.offsetHeight 比 pageContentEle.y + pageContentEle.height 还大，
        //则menu.style.top 应该是 -20px 这样的形式
        if (rect.y + rect.height + menuContainerEle.offsetHeight >= rect2.y + rect2.height) {
            menuContainerEle.style.top = (rect2.y + rect2.height - rect.y - rect.height - menuContainerEle.offsetHeight) + "px";
        }
        else {
            menuContainerEle.style.top = "";
        }
    });
}

const cmdInput = (e:Event)=>{
    var ele = <HTMLTextAreaElement>e.target;
    if(ele.scrollHeight > 80){
        ele.style.height = `${ele.scrollHeight}px`;
    }
}
</script>

<template>
    <div class="pageContent">
        <div v-if="websocketDisconneted" class="errtip">与服务器连接中断</div>
        <Loading v-if="isBusy" class="loadingV3" />
        <template v-if="currentGuid">
            <component :is="childView" v-model="currentGuid" :project="currentProject" :projects="datas" />
        </template>

        <div ref="pageContentEle" class="pageContent">
            <!-- Start Page Header -->
            <div class="page-header">
                <h1 class="title">程序部署列表</h1>
                <ol class="breadcrumb horow">
                    <div class="dropdown link" style="display: block;min-width: 100px;margin-right: 20px;">
                        <a data-toggle="dropdown" class="dropdown-toggle hdbutton" style="cursor: pointer;">{{
                            ProjectListProperties.selectedAgentCategory ? ProjectListProperties.selectedAgentCategory :
                            "所有服务器类别" }}
                            <span class="caret"></span></a>
                        <ul class="dropdown-menu dropdown-menu-list">
                            <li>
                                <a @click="ProjectListProperties.selectedAgentCategory = ''; refreshAllCategories();"><i
                                        class="fa falist fa-dot-circle-o"
                                        v-if="!ProjectListProperties.selectedAgentCategory"></i>所有服务器类别</a>
                            </li>
                            <li v-for="agent in groupAgents">
                                <a
                                    @click="ProjectListProperties.selectedAgentCategory = agent.Category; refreshAllCategories();"><i
                                        v-if="ProjectListProperties.selectedAgentCategory == agent.Category"
                                        class="fa falist fa-dot-circle-o"></i>{{
                                            agent.Category }}</a>
                            </li>
                        </ul>
                    </div>

                    <div class="dropdown link" style="display: block;min-width: 100px;margin-right: 20px;">
                        <a data-toggle="dropdown" class="dropdown-toggle hdbutton" style="cursor: pointer;">{{
                            ProjectListProperties.selectedProjectCategory ?
                            ProjectListProperties.selectedProjectCategory :
                            "所有程序类别" }}
                            <span class="caret"></span></a>
                        <ul class="dropdown-menu dropdown-menu-list">
                            <li>
                                <a @click="ProjectListProperties.selectedProjectCategory = ''"><i
                                        class="fa falist fa-dot-circle-o"
                                        v-if="!ProjectListProperties.selectedProjectCategory[0]"></i>所有程序类别</a>
                            </li>
                            <li v-for="arr in allCategories">
                                <a @click="ProjectListProperties.selectedProjectCategory = arr[1]">
                                    <i v-if="ProjectListProperties.selectedProjectCategory == arr[1]"
                                        class="fa falist fa-dot-circle-o"></i>{{
                                            arr[1] }}</a>
                            </li>
                        </ul>
                    </div>
                </ol>

                <!-- Start Page Header Right Div -->
                <div class="right" v-if="!editingModel">
                    <div class="btn-group" role="group" aria-label="...">
                        <button class="btn btn-light" @click="addClick">新部署程序</button>
                        <a title="全局警报线设置" @click="globalAlarmSettingClick" class="btn btn-light"><i
                                class="fa fa-shield"></i></a>
                        <a title="刷新" @click="refreshDatas" class="btn btn-light"><i class="fa fa-refresh"></i></a>
                    </div>

                    <div class="publishing" v-if="publishingInfo">
                        正在部署：{{publishingInfo}}
                    </div>
                </div>
                <!-- End Page Header Right Div -->

            </div>
            <!-- End Page Header -->


            <!-- Start Row -->
            <div class="row">

                <!-- Start Panel -->
                <div class="col-md-12">
                    <div class="panel panel-default">

                        <!-- <div class="panel-title">
                        DAtaTables
                    </div> -->
                        <div class="panel-body">

                            <table id="example0" class="table display">
                                <thead>
                                    <tr>
                                        <th></th>
                                        <th>名称</th>
                                        <th>描述</th>
                                        <th>所属服务器</th>
                                        <th>发布人</th>
                                        <th>方式</th>
                                        <th>Cpu</th>
                                        <th>内存</th>
                                        <th></th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <template v-for="item in datas">
                                        <tr :class="{ offline: item.OwnerServer.offline }"
                                            v-if="(!ProjectListProperties.selectedAgentCategory || ProjectListProperties.selectedAgentCategory == item.OwnerServer.Category) &&
                                                (!ProjectListProperties.selectedProjectCategory || ProjectListProperties.selectedProjectCategory == item.Category)"
                                            :title="`${item.OwnerServer.Category ? item.OwnerServer.Category : ''}-${item.Category ? item.Category : ''}`">
                                            <td>
                                                <li class="dropdown" style="display: block;">
                                                    <a @mousedown="menuClick" data-toggle="dropdown"
                                                        style="color:#555;white-space: nowrap;cursor: pointer;">. . .</a>
                                                    <ul class="dropdown-menu dropdown-menu-list">
                                                        <li> <a @click="editClick(item)"><i
                                                                    class="fa falist fa-edit"></i>编辑</a>
                                                        </li>
                                                        <li class="divider"></li>
                                                        <li> <a @click="viewOutputs(item)"><i
                                                                    class="fa falist fa-desktop"></i>查看输出日志</a>
                                                        </li>
                                                        <li> <a @click="publish(item)"><i
                                                                    class="fa falist fa-cloud-upload"></i>立刻发布</a>
                                                        </li>
                                                        <template v-if="item.RunCmd || item.RunType > 1">
                                                            <li v-if="!(item.ProcessId > 0 || item.ProcessId == -2)"> <a
                                                                    @click="startClick(item)"><i
                                                                        class="fa falist fa-play"></i>启动</a>
                                                            </li>
                                                            <li v-if="item.ProcessId > 0 || item.ProcessId == -2"> <a
                                                                    @click="stopClick(item)"><i
                                                                        class="fa falist fa-square"></i>停止运行</a>
                                                            </li>
                                                            <li v-if="item.ProcessId > 0 || item.ProcessId == -2"> <a
                                                                    @click="restartClick(item)"><i
                                                                        class="fa falist fa-toggle-right"></i>重新启动</a>
                                                            </li>
                                                        </template>
                                                        <li> <a @click="editConfigFile(item)"><i
                                                                    class="fa falist fa-file-code-o"></i>编辑配置文件</a>
                                                        </li>
                                                        <li class="divider"></li>
                                                        <li> <a @click="editProjectUser(item)"><i
                                                                    class="fa falist fa-user"></i>权限用户</a>
                                                        </li>
                                                        <li> <a @click="restore(item)"><i
                                                                    class="fa falist fa-database"></i>自动备份列表</a>
                                                        </li>
                                                        <li> <a @click="alarmSetting(item)"><i
                                                                    class="fa falist fa-shield"></i>报警线设置</a>
                                                        </li>
                                                        <li> <a @click="copyNew(item)"><i
                                                                    class="fa falist fa-copy"></i>拷贝此参数去新增窗口</a>
                                                        </li>
                                                        <li class="divider"></li>

                                                        <li> <a @click="deleteClick(item)"
                                                                style="color:rgb(222, 106, 106);"><i
                                                                    class="fa falist fa-trash-o"></i>删除</a>
                                                        </li>
                                                    </ul>
                                                </li>
                                            </td>
                                            <td>{{ item.Name }}</td>
                                            <td>{{ item.Desc }}</td>
                                            <td>{{ item.OwnerServer.Address }}:{{ item.OwnerServer.Port }}</td>
                                            <td>{{ item.User }}</td>
                                            <td>
                                                <template v-if="item.RunType > 1 || (item.RunCmd && item.RunCmd.trim())">
                                                    <span v-if="item.ProcessId > 0 || item.ProcessId == -2"
                                                        class="running">{{getRunTypeStr(item)}}</span>
                                                    <span v-else class="stopped">{{getRunTypeStr(item)}}</span>
                                                </template>
                                            </td>
                                            <td>
                                                <Percent class="percent" v-if="item.ProcessId > 0 || item.ProcessId == -2"
                                                    :value="item.CpuPercent" />
                                            </td>
                                            <td>
                                                <Percent class="percent" v-if="item.ProcessId > 0 || item.ProcessId == -2"
                                                    :value="item.MemoryPercent" :under-value="item.MemoryLimit" />
                                            </td>
                                            <td>
                                                <div v-html="item.Status ? item.Status.replace(/\n/g, '<br>') : ''"></div>
                                                <div style="color: red;"
                                                    v-html="item.Error ? item.Error.replace(/\n/g, '<br>') : ''">
                                                </div>
                                            </td>
                                        </tr>
                                    </template>


                                </tbody>
                            </table>


                        </div>

                    </div>
                </div>
                <!-- End Panel -->
            </div>
            <!-- End Row -->

        </div>

        <div v-if="editingModel" class="pageContent editor">

            <!-- Start 表单  -->
            <div class="row">

                <div class="col-md-12">
                    <div class="panel panel-default">

                        <div class="panel-title">
                            {{ editingModel.Guid ? "编辑部署信息" : "新增部署" }}
                            <ul class="panel-tools">
                                <li><a @click="cancelClick" class="icon closed-tool"><i class="fa fa-times"></i></a></li>
                            </ul>
                        </div>

                        <div class="panel-body">
                            <form class="form-horizontal">

                                <div class="form-group" v-if="!editingModel.Guid">
                                    <label for="input002" class="col-sm-3 control-label form-label">所属服务器 *</label>
                                    <div class="col-sm-9">
                                        <Selector v-model="editingModel.OwnerServer.id" :datas="agents" text-member="Name"
                                            value-member="id" />
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label for="input002" class="col-sm-3 control-label form-label">名称 *</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" v-model="editingModel.Name">
                                    </div>
                                </div>


                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">描述</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" v-model="editingModel.Desc">
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">程序类别</label>

                                    <div class="col-sm-9">
                                        <div class="input-group">
                                            <input type="text" class="form-control" v-model="editingModel.Category">
                                            <div data-toggle="dropdown" class="input-group-addon" style="cursor: pointer;">
                                                <i class="fa fa-sort-desc"></i>
                                            </div>
                                            <ul class="dropdown-menu dropdown-menu-list">
                                                <li v-for="arr in editorCategories">
                                                    <a @click="editingModel.Category = arr[1]">
                                                        {{ arr[1] }}</a>
                                                </li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label"></label>
                                    <div class="col-sm-9">
                                        <div class="radio radio-inline">
                                            <input type="radio" id="inlineRadio1" v-model="editingModel.PublishPathMode"
                                                value="0" name="radioInline">
                                            <label for="inlineRadio1"> 服务器默认部署路径 </label>
                                        </div>
                                        <div class="radio radio-info radio-inline">
                                            <input type="radio" id="inlineRadio2" v-model="editingModel.PublishPathMode"
                                                value="1" name="radioInline">
                                            <label for="inlineRadio2"> 指定服务器的部署路径 </label>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group" v-if="editingModel.PublishPathMode == 1">
                                    <label class="col-sm-3 control-label form-label">指定部署路径</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" v-model="editingModel.PublishPath">
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">配置文件</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" v-model="editingModel.ConfigFiles">
                                        <span id="helpBlock" class="help-block">多个文件请用逗号隔开，配置文件在更新时会被排除</span>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">更新时需要排除的文件</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" v-model="editingModel.ExcludeFiles">
                                        <span id="helpBlock" class="help-block">多个文件请用逗号隔开</span>

                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">Git存储库地址</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" @blur="loadBranches"
                                            v-model="editingModel.GitUrl">
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">远程仓库</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" v-model="editingModel.GitRemote">
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">Git用户名</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" @blur="loadBranches"
                                            v-model="editingModel.GitUserName">
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">Git密码</label>
                                    <div class="col-sm-9">
                                        <input type="password" class="form-control" @blur="loadBranches"
                                            v-model="editingModel.GitPwd">
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label for="input002" class="col-sm-3 control-label form-label">分支 *</label>
                                    <div class="col-sm-9">
                                        <div v-if="branches">
                                            <Selector v-model="editingModel.BranchName" :datas="branches" />
                                        </div>
                                        <span id="helpBlock" class="help-block" v-if="isLoadingBranches">正在查询分支...</span>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">部署方式 *</label>
                                    <div class="col-sm-9">
                                        <div class="radio radio-info radio-inline">
                                            <input type="radio" id="inlineRadio3" v-model="editingModel.PublishMode"
                                                value="0" name="radioInline2">
                                            <label for="inlineRadio3"> 手动 </label>
                                        </div>
                                        <div class="radio radio-info radio-inline">
                                            <input type="radio" id="inlineRadio4" v-model="editingModel.PublishMode"
                                                value="1" name="radioInline2">
                                            <label for="inlineRadio4"> “命令执行目录”有提交自动触发 </label>
                                        </div>
                                        <div class="radio radio-info radio-inline">
                                            <input type="radio" id="inlineRadio5" v-model="editingModel.PublishMode"
                                                value="2" name="radioInline2">
                                            <label for="inlineRadio5"> 任何提交自动触发 </label>
                                        </div>
                                    </div>
                                </div>
                                <!-- 
                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">是否需要编译 *</label>
                                    <div class="col-sm-9">
                                        <div class="radio radio-info radio-inline">
                                            <input type="radio" id="inlineRadio5" v-model="editingModel.IsNeedBuild"
                                                :value="false" name="IsNeedBuild">
                                            <label for="inlineRadio5"> 否 </label>
                                        </div>
                                        <div class="radio radio-info radio-inline">
                                            <input type="radio" id="inlineRadio6" v-model="editingModel.IsNeedBuild"
                                                :value="true" name="IsNeedBuild">
                                            <label for="inlineRadio6"> 是 </label>
                                        </div>
                                    </div>
                                </div> -->

                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">是否删除残留文件 *</label>
                                    <div class="col-sm-9">
                                        <div class="radio radio-info radio-inline">
                                            <input type="radio" id="inlineRadio_DeleteNoUseFiles1"
                                                v-model="editingModel.DeleteNoUseFiles" :value="false"
                                                name="DeleteNoUseFiles">
                                            <label for="inlineRadio_DeleteNoUseFiles1"> 否 </label>
                                        </div>
                                        <div class="radio radio-info radio-inline">
                                            <input type="radio" id="inlineRadio_DeleteNoUseFiles2"
                                                v-model="editingModel.DeleteNoUseFiles" :value="true"
                                                name="DeleteNoUseFiles">
                                            <label for="inlineRadio_DeleteNoUseFiles2"> 是 </label>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">
                                        <span>命令执行目录</span>
                                    </label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" v-model="editingModel.ProgramPath">
                                        <span class="help-block">Git根目录下的相对路径（文件夹如果不存在会自动创建）</span>

                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">命令脚本 *</label>
                                    <div class="col-sm-9">
                                        <textarea class="form-control" ref="txtCmd" @input="cmdInput" style="height: 80px;overflow-y: auto;"
                                            v-model="editingModel.BuildCmd"></textarea>
                                        <span class="help-block">例如：dotnet publish
                                            "ProgramFolder/Program.csproj"
                                            --force -c release -o "ProgramFolder/bin/publish" --self-contained true
                                            --runtime
                                            linux-x64</span>
                                        <span class="help-block">支持：? --after 两种标识符</span>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">发布文件所在目录</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" v-model="editingModel.BuildPath">
                                        <span id="helpBlock"
                                            class="help-block">这是<b>命令执行目录</b>的相对路径，此文件夹编译前会被清空</span>

                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-sm-3 control-label form-label">运行模式</label>
                                    <div class="col-sm-9">
                                        <div class="radio radio-info radio-inline">
                                            <input type="radio" id="inlineRadio_IsRunInDocker1"
                                                v-model="editingModel.RunType" :value="1"
                                                name="lineRadio_IsRunInDocker">
                                            <label for="inlineRadio_IsRunInDocker1"> 在服务器直接运行程序 </label>
                                        </div>
                                        <div class="radio radio-info radio-inline">
                                            <input type="radio" id="inlineRadio_IsRunInDocker2"
                                                v-model="editingModel.RunType" :value="34"
                                                name="lineRadio_IsRunInDocker">
                                            <label for="inlineRadio_IsRunInDocker2"> 使用Docker容器运行 </label>
                                        </div>


                                    </div>
                                </div>

                                <div class="form-group" v-if="editingModel.RunType == 1">
                                    <label class="col-sm-3 control-label form-label">运行命令</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" v-model="editingModel.RunCmd">
                                        <span id="helpBlock" class="help-block">留空表示无需运行</span>

                                    </div>
                                </div>

                                <div class="form-group" v-if="(editingModel.RunType&2)==2">
                                    <label class="col-sm-3 control-label form-label">网络配置</label>
                                    <div class="col-sm-9">
                                        <div class="radio radio-info radio-inline">
                                            <input type="radio" id="inlineRadio_IsHostNetwork1"
                                                v-model="editingModel.IsHostNetwork" :value="false"
                                                name="lineRadio_IsHostNetwork">
                                            <label for="inlineRadio_IsHostNetwork1"> 使用端口映射 </label>
                                        </div>
                                        <div class="radio radio-info radio-inline">
                                            <input type="radio" id="inlineRadio_IsHostNetwork2"
                                                v-model="editingModel.IsHostNetwork" :value="true"
                                                name="lineRadio_IsHostNetwork">
                                            <label for="inlineRadio_IsHostNetwork2"> 和主机使用同一个网络 </label>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group" v-if="(editingModel.RunType&2)==2 && !editingModel.IsHostNetwork">
                                    <label class="col-sm-3 control-label form-label">容器端口映射</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" v-model="editingModel.DockerPortMap">
                                        <span id="helpBlock" class="help-block">如 80:8900,81:9200 格式：宿主机端口:容器端口</span>
                                    </div>
                                </div>

                                <div class="form-group" v-if="(editingModel.RunType&2)==2">
                                    <label class="col-sm-3 control-label form-label">容器文件夹映射</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" v-model="editingModel.DockerFolderMap">
                                        <span id="helpBlock" class="help-block">如 /data:/app/data ， "/home/my data":/myhome
                                            格式：宿主机目录:容器目录</span>
                                    </div>
                                </div>

                                <div class="form-group" v-if="(editingModel.RunType&2)==2">
                                    <label class="col-sm-3 control-label form-label">环境变量设置</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" v-model="editingModel.DockerEnvMap">
                                        <span id="helpBlock" class="help-block">如 POSTGRES_PASSWORD=123456 ， POSTGRES_HOST_AUTH_METHOD=trust</span>
                                    </div>
                                </div>

                                <div class="form-group" v-if="(editingModel.RunType&2)==2">
                                    <label class="col-sm-3 control-label form-label">内存限制</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" v-model="editingModel.MemoryLimit">
                                        <span id="helpBlock" class="help-block">如： 500m</span>
                                    </div>
                                </div>



                                <div class="form-group" v-if="(editingModel.RunType&2)==2">
                                    <label class="col-sm-3 control-label form-label">Dockerfile内容</label>
                                    <div class="col-sm-9">
                                        <span
                                            class="help-block">程序所在文件夹会被自动映射到容器的/vela/app路径，所以Dockerfile内无需拷贝程序文件到映像里</span>
                                        <textarea class="form-control" v-model="dockerfileContent"
                                            style="height: 300px;"></textarea>
                                    </div>
                                </div>


                                <div style="text-align: right;">
                                    <button class="btn btn-default" type="button" @click="viewUrl">查看触发Url</button>
                                    &nbsp;&nbsp;&nbsp;&nbsp;
                                    <button class="btn btn-default" type="button" @click="viewPublishPath">查看待上传路径</button>
                                    &nbsp;&nbsp;&nbsp;&nbsp;
                                    <button class="btn btn-default" type="button"
                                        @click="okClick">保&nbsp;&nbsp;&nbsp;&nbsp;存</button>
                                    &nbsp;&nbsp;&nbsp;&nbsp;
                                    <button class="btn" type="button"
                                        @click="cancelClick">取&nbsp;&nbsp;&nbsp;&nbsp;消</button>
                                </div>
                                <div v-if="publishPath" style="width: 100%;word-break: break-word;padding-top: 10px;">{{
                                    publishPath }}</div>
                            </form>

                        </div>

                    </div>
                </div>

            </div>
            <!-- End 表单 -->

        </div>
    </div>
</template>
<style scoped>
.percent {
    margin-top: -13px;
    margin-bottom: -13px;
}

.editor {
    position: absolute;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    background-color: #f5f5f5;
}

.offline {
    opacity: 0.3;
}

.stopped {
    opacity: 0.3;
}

.running {
    color: green;
    border: 1px solid green;
    border-radius: 3px;
    font-size: 12px;
    padding: 2px;
}

.horow {
    display: flex;
    flex-direction: row;
}

.errtip {
    position: fixed;
    right: 10px;
    bottom: 10px;
    padding: 10px;
    background-color: rgba(255, 0, 0, 0.6);
    border-radius: 5px;
    color: #fff;
    font-size: 15px;
    font-weight: 600;
    z-index: 999;
    pointer-events: none;
}

.help-block b {
    color: #cf4602;
}

.publishing {
    position: absolute;
    right: 5px;
    top: -30px;
}
</style>