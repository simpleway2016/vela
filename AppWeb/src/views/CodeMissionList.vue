<script setup lang="ts">
import "../vue"
import "../initMonaco"
import { GlobalInfo } from "@/GlobalInfo";
import Loading from "@/components/Loading.vue";
import Selector from "@/components/Selector.vue";
import JmsUploader from "jms-uploader"
import CodeTransformEditor from "./CodeTransformEditor.vue";
import VueMethodEditor from "./VueMethodEditor.vue";
import esprime from "esprima";
import { ref, shallowRef, onMounted, onUnmounted } from "vue"
import { useToast } from "vue-toastification";
import { onBeforeRouteLeave, onBeforeRouteUpdate, useRoute } from "vue-router";
const isBusy = ref(false);
const ProjectListProperties = GlobalInfo.ProjectListProperties;
const editingModel = ref(<any>null);
const languages = ref(<any[]>[]);
const datas = ref(<any[]>[]);

const route = useRoute();

const historyPaths = ref([{ id: null, Name: "根目录" }]);
var parentId = <any>null;
const copyId = ref(0);

const toast = useToast();
const userInfo = GlobalInfo.UserInfo;

const currentItemId = ref(0);
const currentLanguage = ref("");
const childView = shallowRef(<any>null);

var unmounted = false;

if (route.params.search != undefined) {
    ProjectListProperties.codeMissionSearchKey = <string>route.params.search;
}

onMounted(() => {
    init();
});

onUnmounted(() => {
    unmounted = true;
});

const init = async () => {
    if (GlobalInfo.UserInfo.Token) {
        refreshDatas();
    }
    else {
        window.setTimeout(() => init(), 100);
    }

}

onBeforeRouteUpdate((to, from) => {
    console.log(to);
    if (to.name != "codeMission")
        return;

    ProjectListProperties.codeMissionSearchKey = <string>to.params.search;
    console.log("Code路由变更，search:", ProjectListProperties.codeMissionSearchKey);
    refreshDatas();
});

onBeforeRouteLeave((to, from, next) => {
    next();
});


const refreshDatas = async () => {
    if (isBusy.value)
        return;

    datas.value.splice(0, datas.value.length);
    isBusy.value = true;
    try {
        if(languages.value.length == 0){
            var ret = JSON.parse( await GlobalInfo.get("/CodeBuilder/GetSupportLanguages",null)).map((m:any)=>{
                return {name:m};
            });
            console.log(ret);
            languages.value.push(...ret);
        }

        if (ProjectListProperties.codeMissionSearchKey) {
            var ret = await GlobalInfo.get("/CodeBuilder/SearchItems", { parentId, keyWord: ProjectListProperties.codeMissionSearchKey });
            ret = JSON.parse(ret);
            datas.value.splice(0, datas.value.length, ...ret);

        }
        else {
            var ret = parentId ? await GlobalInfo.get("/CodeBuilder/GetItems?parentId=" + parentId, null) : await GlobalInfo.get("/CodeBuilder/GetItems", null);
            ret = JSON.parse(ret);

            datas.value.splice(0, datas.value.length, ...ret);

        }
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

const addClick = () => {
    editingModel.value = {
        Name: "",
        Type: 2,
        Language: languages.value[0].name,
        CodeContent: "",
        ParentId: parentId
    };
}

const editClick = async (item: any) => {
    var obj = JSON.parse(JSON.stringify(item));
    isBusy.value = true;
    try {
        obj.CodeContent = await GlobalInfo.get("/CodeBuilder/GetCode?id=" + item.id, null);
        editingModel.value = obj;
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const deleteClick = async (item: any) => {
    if (window.confirm(`确定删除${item.Name}吗？`)) {
        isBusy.value = true;
        try {
            await GlobalInfo.get("/CodeBuilder/Delete?id=" + item.id, null);
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

    if (!editingModel.Language && editingModel.Type == 2) {
        GlobalInfo.showError("请选择语言");
        return;
    }

    isBusy.value = true;
    try {
        if (editingModel.value.id) {
            await GlobalInfo.postJson("/CodeBuilder/Modify", editingModel.value);
            var index = datas.value.findIndex(x => x.id == editingModel.value.id);
            datas.value.splice(index, 1, editingModel.value);
        }
        else {
            var id = await GlobalInfo.postJson("/CodeBuilder/AddMission", editingModel.value);
            var data = editingModel.value;
            data.id = id;

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

const itemClick = (item: any) => {
    var index = historyPaths.value.indexOf(item);
    if (index >= 0) {
        if (isBusy.value)
            return;

        ProjectListProperties.codeMissionSearchKey = "";
        parentId = item.id;
        refreshDatas();
        historyPaths.value.splice(index + 1);
        return;
    }

    if (item.Type == 1) {
        //folder

        if (isBusy.value)
            return;

        parentId = item.id;
        historyPaths.value.push(item);
        refreshDatas();
    }
    else if (item.Type == 2) {
        childView.value = CodeTransformEditor;
        currentLanguage.value = item.Language;
        currentItemId.value = item.id;
    }
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

const vueMethodClick = async () => {
    childView.value = VueMethodEditor;
    currentItemId.value = 1;

}

const parseClick = async () => {
    isBusy.value = true;
    try {
        var data = await GlobalInfo.get("/CodeBuilder/Clone", { id: copyId.value, parentId: parentId });
        data = JSON.parse(data);
        var index = datas.value.findIndex(m => m.id == copyId.value);
        if (index >= 0)
            datas.value.splice(index + 1, 0, data);
        else {
            datas.value.push(data);
        }
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const exportAllClick = async ()=>{
    isBusy.value = true;
    try {
        var content = await GlobalInfo.get("/CodeBuilder/GetAllItems", null);
        
        const blob = new Blob([content], { type: 'text/plain' });

        // 创建一个链接元素用于下载
        const a = document.createElement('a');
        a.style.display = 'none';
        a.href = URL.createObjectURL(blob);
        a.download = "代码转换备份.json";

        // 将链接元素添加到文档中，并触发点击事件
        document.body.appendChild(a);
        a.click();

        // 移除链接元素，释放URL对象
        document.body.removeChild(a);
        URL.revokeObjectURL(a.href);
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const fileEle = ref(<HTMLInputElement><any>null);
const onSelectedFile = async () => {
    if (fileEle.value.files?.length) {
        const reader = new FileReader();

        reader.onload = async (e: any) => {
            fileEle.value.value = "";
            try {
                const obj = JSON.parse(e.target.result);

                if (!obj.length) {
                    GlobalInfo.showError("文件里没有任何脚本");
                    return;
                }

                 isBusy.value = true;
                 await GlobalInfo.postJson("/CodeBuilder/ImportItems?parentId=" + (parentId?parentId:""), obj);
                 GlobalInfo.toast("成功导入");
                 isBusy.value = false;
                 
                 refreshDatas();
            } catch (error) {
                isBusy.value = false;
                GlobalInfo.showError(error);
            }
        };

        reader.onerror = () => {
            fileEle.value.value = "";
            GlobalInfo.showError("读取文件错误");
        };

        reader.readAsText(fileEle.value.files[0]);
    }
}

</script>

<template>
    <div ref="pageContentEle" class="pageContent">

        <template v-if="currentItemId">
            <component :is="childView" v-model="currentItemId" :language="currentLanguage"/>
        </template>

        <Loading v-if="isBusy" class="loadingV3" />
        <!-- Start Page Header -->
        <div class="page-header">
            <h1 class="title">我的代码转换模型</h1>
            <ol class="breadcrumb">
                <li class="active">
                    <template v-for="item, index in historyPaths">
                        <a @click="itemClick(item)">{{ item.Name }}</a> /
                    </template>
                </li>
            </ol>

            <!-- Start Page Header Right Div -->
            <div class="right" v-if="!editingModel">
                <div class="btn-group" role="group" aria-label="...">
                    <button class="btn btn-light" @click="addClick">新增项</button>
                    <button class="btn btn-light" @click="vueMethodClick">Vue方法体</button>
                    <button class="btn btn-light" @click="parseClick" v-if="copyId">粘贴</button>
                    <button class="btn btn-light" @click="exportAllClick" v-if="parentId==null">全部导出</button>
                    <span class="btn btn-light" style="position: relative;"><i class="fa fa-share-square-o"></i>
                            导入
                            <input type="file" accept=".json" @change="onSelectedFile" ref="fileEle"
                                style="position: absolute;left:0;top:0;right:0;bottom: 0;opacity: 0;">
                        </span>
                    <a @click="refreshDatas" class="btn btn-light" title="刷新列表"><i class="fa fa-refresh"></i></a>
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

                        <table class="table display">
                            <tbody>
                                <tr v-for="item, index in datas">
                                    <td style="width: 50px;" :class="{ noborder: index === 0 }">
                                        <li class="dropdown" style="display: block;">
                                            <a @mousedown="menuClick" data-toggle="dropdown"
                                                style="color:#555;white-space: nowrap;cursor: pointer;">. . .</a>
                                            <ul class="dropdown-menu dropdown-menu-list">
                                                <li> <a @click="editClick(item)"><i class="fa falist fa-edit"></i>编辑</a>
                                                </li>
                                                <li> <a @click="copyId = item.id"><i class="fa falist fa-copy"></i>复制</a>
                                                </li>
                                                <li> <a @click="deleteClick(item)"><i class="fa falist fa-trash"></i>删除</a>
                                                </li>
                                            </ul>
                                        </li>
                                    </td>
                                    <td class="td" :class="{ noborder: index === 0 }" @click="itemClick(item)">
                                        <i class="fa fa-folder folder" v-if="item.Type == 1"></i>
                                        <i class="fa fa-file-code-o" v-if="item.Type == 2"></i> {{ item.FullName ?? item.Name
                                        }}
                                    </td>
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
                        {{ editingModel.id ? "编辑" : "新增" }}
                        <ul class="panel-tools">
                            <li><a @click="cancelClick" class="icon closed-tool"><i class="fa fa-times"></i></a></li>
                        </ul>
                    </div>

                    <div class="panel-body">
                        <form class="form-horizontal">

                            <div class="form-group">
                                <label class="col-sm-2 control-label form-label">名称</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" v-model="editingModel.Name">
                                </div>
                            </div>

                            <div class="form-group" v-if="!editingModel.id">
                                <label class="col-sm-2 control-label form-label">类型</label>
                                <div class="col-sm-10">
                                    <div class="radio radio-inline">
                                        <input type="radio" id="radTypeRadio1" v-model="editingModel.Type" :value="1"
                                            name="radType">
                                        <label for="radTypeRadio1"> 文件夹 </label>
                                    </div>
                                    <div class="radio radio-info radio-inline">
                                        <input type="radio" id="radTypeRadio2" v-model="editingModel.Type" :value="2"
                                            name="radType">
                                        <label for="radTypeRadio2"> 代码任务 </label>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group" v-if="editingModel.Type == 2">
                                <label class="col-sm-2 control-label form-label">语言</label>
                                <div class="col-sm-10">
                                    <Selector v-model="editingModel.Language" :datas="languages" text-member="name"
                                        value-member="name" />
                                </div>
                            </div>

                            <div class="form-group" v-if="editingModel.Type == 2">
                                <label class="col-sm-2 control-label form-label">代码样本</label>
                                <div class="col-sm-10">
                                    <textarea class="form-control" ref="txtCmd" style="height: 170px;overflow-y: auto;"
                                        v-model="editingModel.CodeContent"></textarea>
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

<style scoped>
a {
    cursor: pointer;
}

.noborder {
    border-top: 0;
}

.td {
    cursor: pointer;
}

.folder {
    color: #f5aa14;
}
</style>
