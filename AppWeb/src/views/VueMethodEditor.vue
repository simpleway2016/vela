<script setup lang="ts">

import { GlobalInfo } from "@/GlobalInfo";
import * as monaco from 'monaco-editor'
import Loading from "@/components/Loading.vue";
import { ref, onMounted } from "vue"

const props = defineProps(["modelValue"]);
const emit = defineEmits(['update:modelValue']);

const containerEle = ref(<HTMLElement><any>null);

const isBusy = ref(false);

var editor: any;

onMounted(() => {
    initEditor();

});

const initEditor = async () => {

    var code;
    try {
        code = await GlobalInfo.get("/CodeBuilder/GetVueMethod", null);

    } catch (e) {
        GlobalInfo.showError(e);
        return;
    }

    editor = monaco.editor.create(containerEle.value, {
        value: code,
        lineNumbers: 'off', // 控制行号的出现on | off
        theme: 'vs',//vs-dark
        language: 'csharp'
    });

    //内容改变事件
    editor.onDidChangeModelContent((e: any) => {
        // console.log(e);
        // console.log(editor.getValue());
    });


    //添加按键监听
    editor.addCommand(monaco.KeyMod.Shift | monaco.KeyMod.Alt | monaco.KeyCode.KeyF, function () {

        // const formatAction = editor.getAction('editor.action.formatDocument');

        // formatAction.run();
        // console.log('格式化完毕2');
        // 调用 formatDocument() 函数对 HTML 进行格式化

    });
}

const close = () => {
    editor?.dispose();
    emit("update:modelValue", "");
}

const save = async () => {
    isBusy.value = true;
    try {
        var data = new FormData();
        data.append("code", editor.getValue());
        await GlobalInfo.postFormData("/CodeBuilder/SaveVueMethod", data);
        close();
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}


</script>

<template>
    <div class="pageContent">
        <Loading v-if="isBusy" class="loadingV3" />

        <!-- Start Page Header -->
        <div class="page-header">
            <h1 class="title">Vue方法体</h1>
            <ol class="breadcrumb horow">
                自定义执行脚本时，Vue的内部方法
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
                    <div ref="containerEle" style="flex: 1;">

                    </div>
                </div>
            </div>
        </div>

    </div>
</template>

<style lang="scss" scoped>
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
    display: inline;
    position: relative;
    z-index: 2;

    span {
        cursor: pointer;
    }
}
</style>