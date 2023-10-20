<script setup lang="ts">

import { GlobalInfo } from "@/GlobalInfo";
import Loading from "@/components/Loading.vue";
import { ref, onMounted } from "vue"
const isBusy = ref(false);
const editingModel = ref(<any>null);
const datas = ref(<any[]>[]);

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

const refreshDatas = async () => {
    if (isBusy.value)
        return;

    isBusy.value = true;
    try {
        var ret = await GlobalInfo.get("/AgentService/GetFileDeleteSettings", null);
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
        Ext: "",
        Days: 5
    };
}

const editClick = (item: any) => {
    editingModel.value = JSON.parse(JSON.stringify(item));
}

const deleteClick = async (item: any) => {
    if (window.confirm(`确定删除${item.Ext}吗？`)) {
        isBusy.value = true;
        try {
            await GlobalInfo.get("/AgentService/DeleteFileDeleteSetting?id=" + item.id, null);
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
            await GlobalInfo.postJson("/AgentService/ModifyFileDeleteSetting", editingModel.value);
            var index = datas.value.findIndex(x => x.id == editingModel.value.id);
            datas.value.splice(index, 1, editingModel.value);
        }
        else {
            var id = await GlobalInfo.postJson("/AgentService/AddFileDeleteSetting", editingModel.value);
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
</script>

<template>
    <div class="pageContent">
        <Loading v-if="isBusy" class="loadingV3" />
        <!-- Start Page Header -->
        <div class="page-header">
            <h1 class="title">文件缓存清理设置</h1>
            <ol class="breadcrumb">
                <li class="active">让VelaAgent定期清理指定的文件缓存</li>
            </ol>

            <!-- Start Page Header Right Div -->
            <div class="right" v-if="!editingModel">
                <div class="btn-group" role="group" aria-label="...">
                    <button class="btn btn-light" @click="addClick">添加扩展名</button>
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
                                    <th>扩展名</th>
                                    <th>保留期限</th>
                                </tr>
                            </thead>

                            <tbody>
                                <tr v-for="item in datas">
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
                                    <td>{{ item.Ext }}</td>
                                    <td>{{ item.Days }}天</td>
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
                        {{ editingModel.id ? "编辑扩展名" : "新增扩展名" }}
                        <ul class="panel-tools">
                            <li><a @click="cancelClick" class="icon closed-tool"><i class="fa fa-times"></i></a></li>
                        </ul>
                    </div>

                    <div class="panel-body">
                        <form class="form-horizontal">

                            <div class="form-group">
                                <label for="input002" class="col-sm-2 control-label form-label">扩展名</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" v-model="editingModel.Ext">
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="input002" class="col-sm-2 control-label form-label">保留期限</label>

                                <div class="col-sm-10">
                                    <label class="sr-only" for="exampleInputAmount">Amount (in dollars)</label>
                                    <div class="input-group">
                                        <div class="input-group-addon"><i class="fa fa-clock-o"></i></div>
                                        <input type="number" class="form-control" v-model="editingModel.Days">
                                        <div class="input-group-addon">天</div>
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
