<script setup lang="ts">

import { GlobalInfo } from "@/GlobalInfo";
import Loading from "@/components/Loading.vue";
import { ref, onMounted, watch } from "vue"
const props = defineProps(["modelValue", "project", "projects"]);
const emit = defineEmits(['update:modelValue']);

const selectedProject = ref(<any>null);
const allProjects = ref(<any[]>[]);
const isBusy = ref(false);
const configFiles = ref(<string[]>[]);
const selectedFile = ref("");
const editingContent = ref("");
const editingContent2 = ref("");
onMounted(async () => {
    if (props.projects) {
        //把同一个服务器的提到前面
        var arr = props.projects.filter((m: any) => m.OwnerServer.id == props.project.OwnerServer.id);
        allProjects.value.push(...arr);

        arr = props.projects.filter((m: any) => m.OwnerServer.id != props.project.OwnerServer.id && m.OwnerServer.Category == props.project.OwnerServer.Category);
        allProjects.value.push(...arr);

        arr = props.projects.filter((m: any) => m.OwnerServer.id != props.project.OwnerServer.id && m.OwnerServer.Category != props.project.OwnerServer.Category);
        allProjects.value.push(...arr);
    }

    if (props.project.ConfigFiles) {
        var arr: any = props.project.ConfigFiles.split(',');
        if (arr?.length) {
            arr = arr.map((m:any) => m.trim());
            if (arr.length > 0) {
                selectedFile.value = arr[0];
            }
            configFiles.value.splice(0, 0, ...arr);
        }
    }
    else {
        try {
            var ret = await GlobalInfo.get("/AgentService/GetTextFiles", { guid: props.modelValue });
            ret = JSON.parse(ret);
            if (ret.length > 0) {
                selectedFile.value = ret[0];
            }
            configFiles.value.splice(0, 0, ...ret);
        } catch (error) {
            GlobalInfo.showError(error);
        }
    }
});

const view = async (guid: any) => {
    if (isBusy.value)
        return;
    isBusy.value = true;
    try {
        editingContent2.value = await GlobalInfo.get("/AgentService/GetConfigContent", { guid: guid, filename: selectedFile.value });
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

watch(selectedFile, async (newValue, oldValue) => {
    console.log("selectedFile改变", newValue);
    if (isBusy.value)
        return;

    isBusy.value = true;
    try {
        editingContent.value = await GlobalInfo.get("/AgentService/GetConfigContent", { guid: props.modelValue, filename: selectedFile.value });
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
});

const save = async () => {
    if (isBusy.value)
        return;

    isBusy.value = true;
    try {
        await GlobalInfo.post("/AgentService/SaveConfigContent", { guid: props.modelValue, filename: selectedFile.value, content: editingContent.value });
        close();
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const close = () => {
    emit("update:modelValue", "");
}
</script>

<template>
    <div class="pageContent">
        <Loading v-if="isBusy" class="loadingV3" />
        <!-- Start Page Header -->
        <div class="page-header">
            <h1 class="title">编辑配置文件</h1>
            <ol class="breadcrumb horow">

                <div class="dropdown link" style="display: block;min-width: 100px;margin-right: 20px;">
                    <a data-toggle="dropdown" class="dropdown-toggle hdbutton" style="cursor: pointer;">{{ selectedFile }}
                        <span class="caret"></span></a>
                    <ul class="dropdown-menu dropdown-menu-list">
                        <li v-for="file in configFiles">
                            <a @click="selectedFile = file"><i class="fa falist fa-paper-plane-o"></i>{{ file }}</a>
                        </li>
                    </ul>
                </div>

                <div class="dropdown link" style="display: block;min-width: 100px;">
                    <a data-toggle="dropdown" class="dropdown-toggle hdbutton" style="cursor: pointer;">
                        <template v-if="selectedProject">
                            {{ selectedProject.OwnerServer.Category?selectedProject.OwnerServer.Category+'-&gt;':'' }}{{ selectedProject.Name }}
                        </template>
                        <template v-else>
                            选择其他程序的配置文件作为参考
                        </template>
                        <span class="caret"></span></a>
                    <ul class="dropdown-menu dropdown-menu-list">
                        <li v-for="project in allProjects">
                            <a @click="selectedProject=project;view(project.Guid);"><i class="fa falist fa-paper-plane-o"></i>{{ project.OwnerServer.Category?project.OwnerServer.Category+'-&gt;':'' }}{{ project.Name }}</a>
                        </li>
                    </ul>
                </div>
            </ol>

            <!-- Start Page Header Right Div -->
            <div class="right">
                <div class="btn-group" role="group" aria-label="...">
                    <button class="btn btn-light" @click="save">保存</button>
                    <a @click="close" class="btn btn-light"><i class="fa fa-close"></i></a>
                </div>
            </div>
            <!-- End Page Header Right Div -->

        </div>
        <!-- End Page Header -->


        <div class="row">
            <div class="col-md-12" style="height: 100%;">
                <div class="panel panel-default editor-panel" style="height: 100%;">
                    <div>
                        <textarea v-model="editingContent"></textarea>
                    </div>
                    <div v-if="editingContent2">
                        <textarea v-model="editingContent2"></textarea>
                    </div>
                </div>
            </div>
        </div>

    </div>
</template>

<style scoped>
.pageContent {
    position: fixed !important;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    z-index: 10;
    display: flex;
    flex-direction: column;
    background-color: #f5f5f5;
}

.row {
    flex: 1;
}

.editor-panel {
    display: flex;
    flex-direction: row;
}

.editor-panel div {
    flex: 1;
    height: 100%;
    overflow: hidden;
    padding-right: 10px;
}

.editor-panel textarea {
    border: 0;
    width: 100%;
    height: 100%;
    font-size: 13px;
    resize: none;
}

.editor-panel div:nth-child(2) {
    border-left: 1px solid #eee;
    padding-left: 10px;
}

.horow {
    display: flex;
    flex-direction: row;
}
</style>