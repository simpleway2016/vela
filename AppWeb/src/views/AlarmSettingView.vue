<script setup lang="ts">

import { GlobalInfo } from "@/GlobalInfo";
import Loading from "@/components/Loading.vue";
import { ref, onMounted } from "vue"
const isBusy = ref(false);
const editingModel = ref(<any>null);
const datas = ref(<any[]>[]);

const props = defineProps(["modelValue"]);
const emit = defineEmits(['update:modelValue']);


onMounted(() => {
    console.log("当前guid", props.modelValue);
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

const refreshDatas = async () => {
    if (isBusy.value)
        return;

    isBusy.value = true;
    try {
        var ret = await GlobalInfo.get("/Alarm/GetAlarmSettings", { guid: props.modelValue });
        console.log(ret);
        ret = JSON.parse(ret);
        datas.value.splice(0, datas.value.length, ...ret);
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const addClick = () => {
    editingModel.value = {
        id: 0,
        ProjectGuid: props.modelValue,
        Cpu: 50,
        Memory: 50,
        Cmd: "",
        IsEnable:true
    };
}

const editClick = (item: any) => {
    editingModel.value = JSON.parse(JSON.stringify(item));
}

const deleteClick = async (item: any) => {
    if (window.confirm(`确定删除吗？`)) {
        isBusy.value = true;
        try {
            await GlobalInfo.get("/Alarm/DeleteAlarmSetting?id=" + item.id, null);
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
        console.log(JSON.stringify(editingModel.value));
        if (editingModel.value.id) {
            await GlobalInfo.postJson("/Alarm/ModifyAlarmSetting", editingModel.value);
            var index = datas.value.findIndex(x => x.id == editingModel.value.id);
            datas.value.splice(index, 1, editingModel.value);
        }
        else {
            var id = await GlobalInfo.postJson("/Alarm/AddAlarmSetting", editingModel.value);
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

const cancelClick = () => {
    editingModel.value = null;
}
const closeClick = () => {
    emit("update:modelValue", "");
}
</script>

<template>
    <div class="pageContent">
        <Loading v-if="isBusy" class="loadingV3" />
        <!-- Start Page Header -->
        <div class="page-header">
            <h1 class="title">程序警报线设置</h1>
            <ol class="breadcrumb">
                <li class="active">设置程序超过警报线后的报警行为</li>
            </ol>

            <!-- Start Page Header Right Div -->
            <div class="right" v-if="!editingModel">
                <div class="btn-group" role="group" aria-label="...">
                    <button class="btn btn-light" @click="addClick">添加警报线</button>
                    <a @click="closeClick" class="btn btn-light"><i class="fa fa-close"></i></a>
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
                                    <th>Cpu报警线</th>
                                    <th>内存报警线</th>
                                </tr>
                            </thead>

                            <tbody>
                                <tr v-for="item in datas" :class="{disabled:!item.IsEnable}">
                                    <td>
                                        <li class="dropdown" style="display: block;">
                                            <a href="#" data-toggle="dropdown" style="color:#555;">. . .</a>
                                            <ul class="dropdown-menu dropdown-menu-list">
                                                <li> <a @click="editClick(item)"><i class="fa falist fa-edit"></i>编辑</a>
                                                </li>
                                                <li> <a @click="deleteClick(item)"><i class="fa falist fa-trash"></i>删除</a>
                                                </li>
                                            </ul>
                                        </li>
                                    </td>
                                    <td v-if="item.Cpu">超过{{ item.Cpu }}%</td>
                                    <td v-else></td>
                                    <td v-if="item.Memory">超过{{ item.Memory }}%</td>
                                    <td v-else></td>
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
                        {{ editingModel.id ? "编辑报警线" : "新增报警线" }}
                        <ul class="panel-tools">
                            <li><a @click="cancelClick" class="icon closed-tool"><i class="fa fa-times"></i></a></li>
                        </ul>
                    </div>

                    <div class="panel-body">
                        <form class="form-horizontal">


                            <div class="form-group">
                                <label for="input002" class="col-sm-2 control-label form-label">Cpu</label>

                                <div class="col-sm-10">
                                    <div class="input-group">
                                        <div class="input-group-addon"><i class="fa fa-line-chart"></i></div>
                                        <input type="number" class="form-control" v-model="editingModel.Cpu">
                                        <div class="input-group-addon">%</div>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="input002" class="col-sm-2 control-label form-label">内存</label>

                                <div class="col-sm-10">
                                    <div class="input-group">
                                        <div class="input-group-addon"><i class="fa fa-line-chart"></i></div>
                                        <input type="number" class="form-control" v-model="editingModel.Memory">
                                        <div class="input-group-addon">%</div>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="col-sm-2 control-label form-label">执行命令</label>
                                <div class="col-sm-10">
                                    <textarea class="form-control" style="height: 150px;"
                                        v-model="editingModel.Cmd"></textarea>
                                    <span id="helpBlock" class="help-block">支持变量：%NAME%、%SERVERNAME%、%CPU%、%MEMORY%</span>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="col-sm-3 control-label form-label">状态</label>
                                <div class="col-sm-9">
                                    <div class="radio radio-info radio-inline">
                                        <input type="radio" id="inlineRadio_IsEnable1"
                                            v-model="editingModel.IsEnable" :value="false" name="IsEnable">
                                        <label for="inlineRadio_IsEnable1"> 禁用 </label>
                                    </div>
                                    <div class="radio radio-info radio-inline">
                                        <input type="radio" id="inlineRadio_IsEnable2"
                                            v-model="editingModel.IsEnable" :value="true" name="IsEnable">
                                        <label for="inlineRadio_IsEnable2"> 可用 </label>
                                    </div>
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
.pageContent {
    background-color: #f5f5f5 !important;
    position: absolute !important;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    z-index: 10;
}

.disabled{
    color:#ddd;
}
</style>