<script setup lang="ts">

import { GlobalInfo } from "@/GlobalInfo";
import Loading from "@/components/Loading.vue";
import Selector from "@/components/Selector.vue";
import Checkbox from "@/components/Checkbox.vue";
import { ref, onMounted } from "vue"
import { useToast } from "vue-toastification";

const props = defineProps(["modelValue"]);
const emit = defineEmits(["update:modelValue"]);

const toast = useToast();
const isBusy = ref(false);
const editingModel = ref(<any>null);
const datas = ref(<any[]>[]);
const roles = ref([
    {
        name: "管理员",
        value: 1 | 1 << 20,
    }, {
        name: "普通用户",
        value: 1,
    }
]);
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
        var ret = await GlobalInfo.get("/User/GetAgentOwnerUsers", { agentId: props.modelValue });
        console.log(ret);
        ret = JSON.parse(ret);
        ret.forEach((user: any) => {
            if (!user.Password) {
                user.Checked = false;
            }
            else {
                user.Checked = true;
            }
        });
        datas.value.splice(0, datas.value.length, ...ret);
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const saveClick = async () => {
    var userIds = datas.value.filter(m => m.Checked).map(m => m.id);

    if (isBusy.value)
        return;

    isBusy.value = true;
    try {
        await GlobalInfo.post("/User/SetAgentPowers", { agentId: props.modelValue, userids: userIds });
        toast("设置成功");
        cancelClick();
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const cancelClick = () => {
    emit("update:modelValue", "");
}
</script>

<template>
    <div class="pageContent">
        <Loading v-if="isBusy" class="loadingV3" />
        <!-- Start Page Header -->
        <div class="page-header">
            <h1 class="title">服务器授权部署的用户</h1>
            <ol class="breadcrumb">
                <li class="active">设置可以在此服务器部署的用户</li>
            </ol>

            <!-- Start Page Header Right Div -->
            <div class="right" v-if="!editingModel">
                <div class="btn-group" role="group" aria-label="...">
                    <button class="btn btn-light" @click="saveClick">保存</button>
                    <a @click="cancelClick" class="btn btn-light"><i class="fa fa-close"></i></a>
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
                                    <td class="text-center">
                                        <Checkbox class="checkbox-primary margin-t-0" v-model="item.Checked"/>
                                    </td>
                                    <td>{{ item.Name }}</td>
                                    <td v-if="item.Role == roles[0].value">管理员</td>
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


    </div>
</template>

<style scoped>
input[type='checkbox'] {
    width: 15px;
    height: 15px;
    border-radius: 2px;
    border: 1px solid #666;
    outline: none;
}

.pageContent{
    background-color: #f5f5f5 !important;
    position: absolute !important;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    z-index: 10;
}
</style>