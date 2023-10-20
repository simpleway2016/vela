<script setup lang="ts">

import { GlobalInfo } from "@/GlobalInfo";
import Loading from "@/components/Loading.vue";
import Selector from "@/components/Selector.vue";
import { ref, onMounted } from "vue"
const isBusy = ref(false);
const datas = ref(<any[]>[]);
const currentPageIndex = ref(0);
const totalPageCount = ref(0);
const searchKey = ref("");
const allPagerButtons = ref([{
    text: "",
    value: 0,
    enable: false,
    isNumber: false
}]);
onMounted(() => {
    init();
});

const init = async () => {
    if (GlobalInfo.UserInfo.Token) {
        refreshDatas(0);
    }
    else {
        window.setTimeout(() => init(), 100);
    }

}

const refreshDatas = async (pageIndex: number) => {
    if (isBusy.value)
        return;

    isBusy.value = true;
    currentPageIndex.value = pageIndex;
    try {
        var ret = await GlobalInfo.get("/Log/GetLogs", { pageIndex: pageIndex, pageSize: 20,searchKey:searchKey.value });
        //console.log(ret);
        ret = JSON.parse(ret);
        datas.value.splice(0, datas.value.length, ...ret.Datas);

        var totalCount = parseInt(<any>(ret.Total / 20));
        if (ret.Total % 20 > 0) {
            totalCount++;
        }

        totalPageCount.value = totalCount;
        //计算当前显示哪10个页码
        var pageNumber = pageIndex + 1;
        var startNumber = pageNumber - pageNumber % 10 + 1;
        var endNumber = startNumber + 10 - 1;
        var pageButtons = <any[]>[];
        pageButtons.push({
            text: "首页",
            value: 0,
            enable: pageIndex > 0
        });
        pageButtons.push({
            text: "上一页",
            value: pageIndex - 1,
            enable: pageIndex > 0
        });

        if (startNumber >= 11) {
            pageButtons.push({
                text: "...",
                value: startNumber - 1,
                enable: true
            });
        }

        for (var i = startNumber; i <= endNumber; i++) {
            if (i <= totalCount) {
                pageButtons.push({
                    text: i,
                    isNumber: true,
                    value: i - 1,
                    enable: true
                });
            }
        }

        if (endNumber < totalCount) {
            pageButtons.push({
                text: "...",
                value: endNumber,
                enable: true
            });
        }

        pageButtons.push({
            text: "下一页",
            value: pageIndex + 1,
            enable: pageIndex < totalCount - 1
        });

        pageButtons.push({
            text: "尾页",
            value: totalCount - 1,
            enable: pageIndex < totalCount - 1
        });
        allPagerButtons.value.splice(0, allPagerButtons.value.length, ...pageButtons);

    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}
const searchKeyUp = (e:KeyboardEvent)=>{
    if(e.key == "Enter"){
        refreshDatas(0);
    }
}

const showTime = (time:any)=>{
    if(time.endsWith("Z") == false)
        time += "Z";
    return (<any>new Date(time)).Format("yyyy-MM-dd HH:mm:ss");
}

</script>

<template>
    <div class="pageContent">
        <Loading v-if="isBusy" class="loadingV3" />
        <!-- Start Page Header -->
        <div class="page-header">
            <h1 class="title">操作日志</h1>
            <ol class="breadcrumb">
                <li class="active">系统记录各用户的操作日志</li>
            </ol>

            <!-- Start Page Header Right Div -->
            <div class="right">
                <div class="btn-group" role="group" aria-label="...">
                    <a @click="refreshDatas(0)" class="btn btn-light"><i class="fa fa-refresh"></i></a>
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
                    <div class="panel-body dataTables_wrapper">
                        <div id="example0_filter" class="dataTables_filter">
                            <label>搜索:<input @keyup="searchKeyUp" v-model="searchKey" type="search" class="" placeholder="" aria-controls="example0"></label>
                        </div>

                        <table id="example0" class="table display">
                            <thead>
                                <tr>
                                    <th>操作</th>
                                    <th>操作人</th>
                                    <th>详情</th>
                                    <th>时间</th>
                                </tr>
                            </thead>

                            <tbody>
                                <tr v-for="item in datas">
                                    <td class="ktd">{{ item.Operation }}</td>
                                    <td>{{ item.UserName }}</td>
                                    <td>{{ item.Detail }}</td>
                                    <td class="ktd">{{showTime(item.Time) }}</td>
                                </tr>

                            </tbody>
                        </table>

                        <div class="dataTables_paginate paging_simple_numbers">
                            <a @click="refreshDatas(btnItem.value)" v-show="btnItem.enable" v-for="btnItem in allPagerButtons"
                                class="paginate_button" :class="{ current: btnItem.value == currentPageIndex }"
                                aria-controls="example0" tabindex="0">{{ btnItem.text }}</a>
                        </div>

                    </div>

                </div>
            </div>
            <!-- End Panel -->
        </div>
        <!-- End Row -->

    </div>
</template>

<style scoped>
.ktd {
    white-space: nowrap;
}
</style>
