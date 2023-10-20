<script setup lang="ts">

import { GlobalInfo } from "@/GlobalInfo";
import Loading from "@/components/Loading.vue";
import Selector from "@/components/Selector.vue";
import { ref, onMounted } from "vue"
import { useRoute,useRouter } from "vue-router";
import { useToast } from "vue-toastification";

const router = useRouter();
const isBusy = ref(false);
const toast = useToast();

const editingModel = ref({
    password: "",
    oldPwd: "",
    password2: ""
});



const okClick = async () => {
    if (isBusy.value)
        return;

    if (!editingModel.value.oldPwd) {
        GlobalInfo.showError("请输入旧密码");
        return;
    }
    if (!editingModel.value.password) {
        GlobalInfo.showError("请输入新密码");
        return;
    }
    if (editingModel.value.password != editingModel.value.password2) {
        GlobalInfo.showError("新密码和确认密码不一致");
        return;
    }
    isBusy.value = true;
    try {
        await GlobalInfo.post("/User/ChangePassword", editingModel.value);
       
        toast("修改成功");
        router.back();
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const cancelClick = () => {
    router.back();
}
</script>

<template>
    <div class="pageContent">
        <Loading v-if="isBusy" class="loadingV3" />

        <!-- Start 表单  -->
        <div class="row">

            <div class="col-md-12">
                <div class="panel panel-default">

                    <div class="panel-title">
                        修改密码
                        <ul class="panel-tools">
                            <li><a @click="cancelClick" class="icon closed-tool"><i class="fa fa-times"></i></a></li>
                        </ul>
                    </div>

                    <div class="panel-body">
                        <form class="form-horizontal">

                            <div class="form-group">
                                <label for="input002" class="col-sm-2 control-label form-label">旧密码</label>
                                <div class="col-sm-10">
                                    <input type="password" class="form-control" v-model="editingModel.oldPwd">
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="input002" class="col-sm-2 control-label form-label">新密码</label>
                                <div class="col-sm-10">
                                    <input type="password" class="form-control" v-model="editingModel.password2">
                                   
                                </div>
                            </div>


                            <div class="form-group">
                                <label for="input002" class="col-sm-2 control-label form-label">新密码</label>
                                <div class="col-sm-10">
                                    <input type="password" class="form-control" v-model="editingModel.password">
                                    <span id="helpBlock" class="help-block">再次输入新密码</span>
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
