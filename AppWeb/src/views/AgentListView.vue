<script setup lang="ts">

import { GlobalInfo } from "@/GlobalInfo";
import Loading from "@/components/Loading.vue";
import JmsUploader from "jms-uploader"
import AgentUserList from "./AgentUserList.vue";
import { ref, shallowRef, onMounted } from "vue"
import { useToast } from "vue-toastification";
import { onBeforeRouteLeave } from "vue-router";
const isBusy = ref(false);
const editingModel = ref(<any>null);
const datas = ref(<any[]>[]);
const upgradeVersion = ref("");
const toast = useToast();
const userInfo = GlobalInfo.UserInfo;

const currentAgentId = ref(0);
const childView = shallowRef(<any>null);

onMounted(() => {
    init();
});

const init = async () => {
    if (GlobalInfo.UserInfo.Token) {
        refreshDatas();
    }
    else {
        window.setTimeout(() => init(), 100);
    }

}

onBeforeRouteLeave((to, from, next) => {
    if (isUploading.value || datas.value.some(x => x.upgradingInfo)) {
        toast.error("正在传输，请稍后再试！")
        next(false);
    }
    else {
        next();
    }
});

/**比较两个版本的大小，如果第一个版本大，返回true */
const compareVersion = (v1: any, v2: any) => {
    if (!v1) {
        return false;
    }
    v1 = v1.split(".");
    v2 = v2.split(".");
    const len = Math.max(v1.length, v2.length);

    while (v1.length < len) {
        v1.push("0");
    }
    while (v2.length < len) {
        v2.push("0");
    }
    for (let i = 0; i < len; i++) {
        const num1 = parseInt(v1[i]);
        const num2 = parseInt(v2[i]);

        if (num1 > num2) {
            return true;
        }
        else if (num1 < num2) {
            return false;
        }
    }
    return false;
}

const refreshDatas = async () => {
    if (isBusy.value)
        return;

    isBusy.value = true;
    try {
        var ret = await GlobalInfo.get("/AgentService/GetAgents", null);
        console.log(ret);
        ret = JSON.parse(ret);
        ret.forEach((m: any) => {
            m.Version = "";
            m.upgradingInfo = "";
        });

        datas.value.splice(0, datas.value.length, ...ret);
        getVersion();

        upgradeVersion.value = await GlobalInfo.get("/Upgrade/GetUpgradeVersion", null);
        console.log("是否有更新：", upgradeVersion.value);
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const getVersion = async () => {
    for (var i = 0; i < datas.value.length; i++) {
        checkAgentVersion(datas.value[i], 10);
    }
}

const addClick = () => {
    editingModel.value = {
        Address: "",
        Port: "",
        Desc: "",
        Category: ""
    };
}

const editClick = (item: any) => {
    editingModel.value = JSON.parse(JSON.stringify(item));
}

const deleteClick = async (item: any) => {
    if (window.confirm(`确定删除${item.Address}:${item.Port}吗？`)) {
        isBusy.value = true;
        try {
            await GlobalInfo.get("/AgentService/DeleteAgent?id=" + item.id, null);
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

const okClick = async () => {
    if (isBusy.value)
        return;

    isBusy.value = true;
    try {
        if (editingModel.value.id) {
            await GlobalInfo.postJson("/AgentService/ModifyAgent", editingModel.value);
            var index = datas.value.findIndex(x => x.id == editingModel.value.id);
            datas.value.splice(index, 1, editingModel.value);
        }
        else {
            var id = await GlobalInfo.postJson("/AgentService/AddAgent", editingModel.value);
            var data = editingModel.value;
            data.id = id;

            checkAgentVersion(data, 1);
            datas.value.splice(0, 0, data);
        }
        editingModel.value = null;
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const cancelClick = () => {
    editingModel.value = null;
}

const fileEle = ref(<HTMLInputElement><any>null);
const isUploading = ref(false);
const uploadingPercent = ref(0);
const onSelectedFile = async () => {
    if (datas.value.some(x => x.upgradingInfo)) {
        toast.error("正在传输，请稍后再试！");
        return;
    }
    uploadingPercent.value = 0;
    isUploading.value = true;
    try {
        if (fileEle.value.files?.length) {
            var uploader = new JmsUploader(`${GlobalInfo.ServerUrl}/Upgrade/UploadAgent`, fileEle.value.files[0], () => {
                return {
                    'Authorization': GlobalInfo.UserInfo.Token
                }
            }, {
                Name: "jack"
            });
            uploader.onUploading = (percent: any) => {
                uploadingPercent.value = percent;
            }
            var ret = await uploader.upload();
            if (ret == "upgradeWebServer") {
                toast("VelaWeb正在更新重启...");
            }
            else {
                upgradeVersion.value = ret;
            }
        }
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isUploading.value = false;
    }
}

const checkAgentVersion = async (agent: any, done: number) => {
    try {
        if (done > 10)
            return;

        var currentVersion = await GlobalInfo.get("/AgentService/GetAgentVersion", { id: agent.id });
        console.log(agent.Desc + "版本:" + currentVersion);
        if (currentVersion == agent.Version) {
            throw "retry";
        }
        else {
            agent.Version = currentVersion;
        }
    } catch (error) {
        window.setTimeout(async () => {
            checkAgentVersion(agent, done + 1);
        }, 1000);
    }
}

const updateAllAgent = () => {
    if (!upgradeVersion.value) {
        GlobalInfo.showError("请先上传VelaAgent更新包");
        return;
    }
    var finded = false;
    datas.value.forEach(item => {
        if (userInfo.Role == 1048577 && item.Version && compareVersion(upgradeVersion.value, item.Version)) {
            finded = true;
            upgradeAgentClick(item);
        }
    });
    if (!finded) {
        GlobalInfo.showError("没有需要更新的服务器");
    }
}

const upgradeAgentClick = (agent: any) => {
    if (agent.upgradingInfo || isUploading.value) {
        toast.error("正在传输中，请稍后再试");
        return;
    }
    agent.Version = "";
    agent.upgradingInfo = "正在准备传输...";

    var websocket = new WebSocket(`${GlobalInfo.ServerUrl.replace("http", "ws")}/Upgrade/UpgradeAgent?agentId=${agent.id}`);
    websocket.onmessage = (e: MessageEvent) => {
        agent.upgradingInfo = e.data;
    }
    websocket.onclose = (e) => {
        agent.upgradingInfo = "";
        console.log("websocket关闭", e);

        if (e.reason == "ok") {
            toast("更新完毕");
            window.setTimeout(() => {
                checkAgentVersion(agent, 0);
            }, 2000);
        }
        else if (e.reason) {
            GlobalInfo.showError(e.reason);
        }
        else {
            GlobalInfo.showError("意外中断");
        }
    }
    websocket.onerror = (e) => {
        agent.upgradingInfo = "";
        GlobalInfo.showError("意外中断");
    }
}

const editAgentUser = (item: any) => {
    childView.value = AgentUserList;
    currentAgentId.value = item.id;
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

const restart = async () => {
    if (window.confirm("确定重启web服务器吗？")) {
        try {
            await GlobalInfo.get("/AgentService/Restart",null);
            
        } catch (error) {
           GlobalInfo.showError(error);
        }
    }
}
</script>

<template>
    <div ref="pageContentEle" class="pageContent">

        <template v-if="currentAgentId">
            <component :is="childView" v-model="currentAgentId" />
        </template>

        <Loading v-if="isBusy" class="loadingV3" />
        <!-- Start Page Header -->
        <div class="page-header">
            <h1 class="title">服务器列表</h1>
            <ol class="breadcrumb">
                <li class="active">查看所有安装了Vela Agent的服务器 <span style="color: cadetblue;" v-if="upgradeVersion">已上传更新包 {{
                    (<any>upgradeVersion).version() }}</span></li>
            </ol>

            <!-- Start Page Header Right Div -->
            <div class="right" v-if="!editingModel">
                <div class="btn-group" role="group" aria-label="...">
                    <button class="btn btn-light" @click="addClick">添加服务器</button>
                    <span v-if="!isUploading" title="上传VelaAgent升级包" class="btn btn-light" style="position: relative;"><i
                            class="fa fa-upload"></i>
                        <input type="file" accept=".zip" @change="onSelectedFile" ref="fileEle"
                            style="position: absolute;left:0;top:0;right:0;bottom: 0;opacity: 0;">
                    </span>
                    <span v-else title="上传VelaAgent或Vela升级包" class="btn btn-light" style="position: relative;">
                        {{ uploadingPercent }}%
                    </span>

                    <a @click="updateAllAgent" class="btn btn-light" title="更新所有服务器"><i
                            class="fa fa-arrow-circle-up"></i></a>
                    <a @click="restart" class="btn btn-light" title="重启Web服务器"><i class="fa fa-toggle-right"></i></a>

                    <a @click="refreshDatas" class="btn btn-light"><i class="fa fa-refresh"></i></a>
                </div>
            </div>
            <!-- End Page Header Right Div -->

        </div>
        <!-- End Page Header -->


        <!-- Start Row -->
        <div v-if="!editingModel" class="row">

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
                                    <th>IP地址</th>
                                    <th>端口</th>
                                    <th>描述</th>
                                    <th>分类</th>
                                    <th>版本</th>
                                </tr>
                            </thead>

                            <tbody>
                                <tr v-for="item in datas">
                                    <td>
                                        <li class="dropdown" style="display: block;">
                                            <a @mousedown="menuClick" data-toggle="dropdown"
                                                style="color:#555;white-space: nowrap;cursor: pointer;">. . .</a>
                                            <ul class="dropdown-menu dropdown-menu-list">
                                                <li> <a @click="editClick(item)"><i class="fa falist fa-edit"></i>编辑</a>
                                                </li>
                                                <li> <a @click="editAgentUser(item)"><i
                                                            class="fa falist fa-user"></i>权限用户</a>
                                                </li>
                                                <li> <a @click="deleteClick(item)"><i class="fa falist fa-trash"></i>删除</a>
                                                </li>
                                            </ul>
                                        </li>
                                    </td>
                                    <td>{{ item.Address }}</td>
                                    <td>{{ item.Port }}</td>
                                    <td>{{ item.Desc }}</td>
                                    <td>{{ item.Category }}</td>
                                    <td v-if="!item.upgradingInfo">{{ item.Version?.version() }}
                                        &nbsp; &nbsp;
                                        <a v-if="upgradeVersion && userInfo.Role == 1048577 && item.Version && compareVersion(upgradeVersion, item.Version)"
                                            style="cursor: pointer;" @click="upgradeAgentClick(item)">更新</a>
                                    </td>
                                    <td v-else>{{ item.upgradingInfo }}</td>
                                </tr>

                            </tbody>
                        </table>


                    </div>

                </div>
            </div>
            <!-- End Panel -->
        </div>
        <!-- End Row -->

        <!-- Start 表单  -->
        <div v-else class="row">

            <div class="col-md-12">
                <div class="panel panel-default">

                    <div class="panel-title">
                        {{ editingModel.id ? "编辑服务器信息" : "新增服务器" }}
                        <ul class="panel-tools">
                            <li><a @click="cancelClick" class="icon closed-tool"><i class="fa fa-times"></i></a></li>
                        </ul>
                    </div>

                    <div class="panel-body">
                        <form class="form-horizontal">

                            <div class="form-group" v-if="!editingModel.id">
                                <label for="input002" class="col-sm-2 control-label form-label">IP地址</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" v-model="editingModel.Address">
                                </div>
                            </div>

                            <div class="form-group" v-if="!editingModel.id">
                                <label for="input002" class="col-sm-2 control-label form-label">端口</label>
                                <div class="col-sm-10">
                                    <input type="number" class="form-control" v-model="editingModel.Port">
                                </div>
                            </div>


                            <div class="form-group">
                                <label class="col-sm-2 control-label form-label">描述</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" v-model="editingModel.Desc">
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="col-sm-2 control-label form-label">分类</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" v-model="editingModel.Category">
                                </div>
                            </div>

                            <div style="text-align: right;">
                                <button type="button" class="btn btn-default"
                                    @click="okClick">保&nbsp;&nbsp;&nbsp;&nbsp;存</button>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <button type="button" class="btn" @click="cancelClick">取&nbsp;&nbsp;&nbsp;&nbsp;消</button>
                            </div>

                        </form>

                    </div>

                </div>
            </div>

        </div>
        <!-- End 表单 -->
    </div>
</template>
