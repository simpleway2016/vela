<script setup lang="ts">

import { GlobalInfo } from "@/GlobalInfo";
import Loading from "@/components/Loading.vue";
import Selector from "@/components/Selector.vue";
import { ref, onMounted } from "vue"

import { useToast } from "vue-toastification";
const isBusy = ref(false);

const datas = ref(<any[]>[]);
const tbody = ref(<HTMLElement><any>null);
const toast = useToast();

const props = defineProps(["modelValue","project"]);
const emit = defineEmits(["update:modelValue"]);

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
        var ret = await GlobalInfo.get("/AgentService/GetProjectBackups", { guid: props.modelValue });
        console.log(ret);
        ret = JSON.parse(ret);
        ret.forEach((item: any) => {
            item.Checked = false;
            item.Status = "";
        });
        datas.value.splice(0, datas.value.length, ...ret);
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

const okClick = async () => {
    var chks = tbody.value.querySelectorAll("INPUT");
    for (var i = 0; i < chks.length; i++) {
        var chk = (<any>chks[i]);
        if (chk.checked) {
            var index = parseInt(chk.getAttribute("dataIndex"));
            var data = datas.value[index];

            if (window.confirm(`确定恢复吗？`)) {
                data.Status = "正在还原...";
                isBusy.value = true;
                try {
                    await GlobalInfo.get("/AgentService/Restore", { guid: props.modelValue, backupFileName: data.FileName });
                    toast("成功还原");
                    cancelClick();
                } catch (error) {
                    GlobalInfo.showError(error);
                }
                finally {
                    data.Status = "";
                    isBusy.value = false;
                }
            }
        }
    }
}
</script>

<template>
    <div class="pageContent">
        <Loading v-if="isBusy" class="loadingV3" />
        <!-- Start Page Header -->
        <div class="page-header">
            <h1 class="title">{{ project.Name }} - 备份列表</h1>
            <ol class="breadcrumb">
                <li class="active">查看所有此程序的自动备份</li>
            </ol>

            <!-- Start Page Header Right Div -->
            <div class="right">
                <div class="btn-group" role="group" aria-label="...">
                    <button @click="okClick" class="btn btn-light">立刻恢复</button>
                    <a @click="cancelClick" class="btn btn-light"><i class="fa fa-close"></i></a>
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
                                    <th>部署时间</th>
                                </tr>
                            </thead>

                            <tbody ref="tbody">
                                <tr v-for="item, index in datas">
                                    <td>
                                        <div class="radio radio-info radio-inline">
                                            <input type="radio" :id="`inlineRadio${index}`" :dataIndex="index" :value="true"
                                                name="radioChecked">
                                            <label :for="`inlineRadio${index}`"> 选中此备份 </label>
                                        </div>
                                    </td>
                                    <td>{{ (<any>new Date(item.CreateTime)).Format("yyyy-MM-dd HH:mm:ss") }}{{ " " + item.Status }}</td>
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
