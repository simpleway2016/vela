<script setup lang="ts">

import { GlobalInfo } from "@/GlobalInfo";
import Loading from "@/components/Loading.vue";
import Selector from "@/components/Selector.vue";
import { ref, onMounted } from "vue"
const isBusy = ref(false);
const editingModel = ref(<any>null);
const datas = ref(<any[]>[]);
const roles = ref([
    {
        name:"管理员",
        value:1 | 1<<20,
    },{
        name:"普通用户",
        value:1,
    }
]);
onMounted(() => {
   init();
});

const init = async ()=>{
    if(GlobalInfo.UserInfo.Token)
    {
        refreshDatas();
    }
    else{
        window.setTimeout(()=>init() , 100);
    }
   
}

const refreshDatas = async () => {
    if (isBusy.value)
        return;

    isBusy.value = true;
    try {
        var ret = await GlobalInfo.get("/User/GetSystemUsers", null);
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
        Name: "",
        Password: "123",
        Role: 1
    };
}

const editClick = (item: any) => {
    editingModel.value = JSON.parse(JSON.stringify(item));
}

const deleteClick = async (item: any) => {
    if (window.confirm(`确定删除${item.Name}吗？`)) {
        isBusy.value = true;
        try {
            await GlobalInfo.get("/User/DeleteUser?id=" + item.id, null);
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
        //role如果是字符串，服务器会转换失败
        editingModel.value.Role = parseInt(editingModel.value.Role);
        if (editingModel.value.id) {            
            await GlobalInfo.postJson("/User/ModifyUser", editingModel.value);
            var index = datas.value.findIndex(x => x.id == editingModel.value.id);
            datas.value.splice(index, 1, editingModel.value);
        }
        else {
            var id = await GlobalInfo.postJson("/User/AddUser", editingModel.value);
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
            <h1 class="title">系统用户</h1>
            <ol class="breadcrumb">
                <li class="active">给同事分配登录用户、角色</li>
            </ol>

            <!-- Start Page Header Right Div -->
            <div class="right" v-if="!editingModel">
                <div class="btn-group" role="group" aria-label="...">
                    <button class="btn btn-light" @click="addClick">添加用户</button>
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
                                    <th>用户名</th>
                                    <th>角色</th>
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
                                    <td>{{ item.Name }}</td>
                                    <td v-if="item.Role==roles[0].value">管理员</td>
                                    <td v-else>普通用户</td>
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

                            <div class="form-group">
                                <label for="input002" class="col-sm-2 control-label form-label">用户名</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" v-model="editingModel.Name">
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="input002" class="col-sm-2 control-label form-label">密码</label>
                                <div class="col-sm-10">
                                    <input type="password" class="form-control" v-model="editingModel.Password">
                                    <span v-if="editingModel.id" id="helpBlock" class="help-block">留空表示不修改</span>
                                    <span v-else id="helpBlock" class="help-block">密码默认123</span>
                                </div>
                            </div>


                            <div class="form-group">
                                <label class="col-sm-2 control-label form-label">角色</label>
                                <div class="col-sm-10">
                                    <Selector v-model="editingModel.Role" text-member="name" value-member="value" :datas="roles" />
                                </div>
                            </div>
                            <div style="text-align: right;">
                                <button type="button" class="btn btn-default" @click="okClick">保&nbsp;&nbsp;&nbsp;&nbsp;存</button>
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
