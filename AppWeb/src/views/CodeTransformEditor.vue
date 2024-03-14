<script setup lang="ts">

import { GlobalInfo } from "@/GlobalInfo";
import * as monaco from 'monaco-editor'
import Loading from "@/components/Loading.vue";
import { ref, onMounted, watch, createApp, reactive, h } from "vue"

const props = defineProps(["modelValue","language"]);
const emit = defineEmits(['update:modelValue']);

const containerEle = ref(<HTMLElement><any>null);
const scriptContainerEle = ref(<HTMLElement><any>null);

const simpleChars = ref(['<', '>', '{', '}', ':', '@', 'v']);
const simpleCharsText = ref(["&lt;", "&gt;", '{', '}', ':', '@', 'v']);

const value = ref('')
const language = ref('html')
const hightChange = ref<any>(false)

/** 0=normal 1=已经解析代码 2=已经运行脚本 */
const status = ref(0);
const isBusy = ref(false);

var editor: any;
var editorScript: any;
onMounted(() => {
    initEditor();

});

const initEditor = async () => {   
    var code;
    try {
        code = await GlobalInfo.get("/CodeBuilder/GetCode?id=" + props.modelValue, null);
    } catch (e) {
        GlobalInfo.showError(e);
        return;
    }

    console.log("语言",props.language.toLowerCase());
    
    editor = monaco.editor.create(containerEle.value, {
        value: code,
        lineNumbers: 'off', // 控制行号的出现on | off
        theme: 'vs',//vs-dark
        language: props.language.toLowerCase()
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

const initScriptEditor = async () => {

    var code;
    try {
        code = await GlobalInfo.get("/CodeBuilder/GetScript?id=" + props.modelValue, null);
    } catch (e) {
        GlobalInfo.showError(e);
        return;
    }

   

    editorScript = monaco.editor.create(scriptContainerEle.value, {
        value: code,
        lineNumbers: 'off', // 控制行号的出现on | off
        theme: 'vs',//vs-dark
        language: 'html'
    });

    //内容改变事件
    editorScript.onDidChangeModelContent((e: any) => {
        // console.log(e);
        // console.log(editor.getValue());
    });


}

const startParser = async () => {
    if (isBusy.value)
        return;

    isBusy.value = true;
    try {
        try {
            var data = new FormData();
            data.append("code", editor.getValue());
            data.append("id", props.modelValue);
            var code = await GlobalInfo.postFormData("/CodeBuilder/ParserCodeV2", data);

            status.value = 1;
            editor.setValue("");
            const model = editor.getModel(); // 获取编辑器的文本模型
            if (model) {
                monaco.editor.setModelLanguage(model, 'json');
                console.log("更改语言完成");
            }
            editor.setValue(code);

            initScriptEditor();
        } catch (e) {
            GlobalInfo.showError(e);
            return;
        }
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const getCodeString = (char: any) => {
    return `_c${char.charCodeAt(0)}_`;
}

const formatString = (content: string) => {
    content = content.replace(/\<!----\>/g, "");
    const regex = /_c(\d+)\_/g;

    return content.replace(regex, (match, num) => {
        // 将匹配到的数字转换为整数
        const codePoint = parseInt(num, 10);
        // 将整数转换为对应的 UTF-8 字符
        return String.fromCharCode(codePoint);
    });
}

const saveAndTransform = async () => {
    isBusy.value = true;
    try {
        var scriptMethod = await GlobalInfo.get("/CodeBuilder/GetVueMethod", null);
        eval("scriptMethod=" + scriptMethod);

        var data = new FormData();
        data.append("id", props.modelValue);
        data.append("script", editorScript.getValue());
        await GlobalInfo.postFormData("/CodeBuilder/SaveScript", data);

        var root = document.createElement("DIV");
        var childDiv = document.createElement("DIV");
        childDiv.innerHTML = editorScript.getValue();
        root.appendChild(childDiv);

        var list: any;
        eval("list=" + editor.getValue());
        (<any>window).Vue.config.errorHandler = (err: any, vm: any, info: any) => {
            alert(err.stack);
            (<any>window).Vue.config.errorHandler = null;
        }
        var vue = new (<any>window).Vue({
            el: childDiv,
            data: list,
            methods: scriptMethod,
            mounted: function () {
                var ret = formatString(this.$el.innerHTML);

                var newWindow: any = window.open("", "_blank");
                var preElement = newWindow.document.createElement("pre");
                preElement.innerText = ret;

                // 将 <pre> 元素附加到新窗口的 <body> 中
                newWindow.document.body.appendChild(preElement);

                root.innerHTML = "";
                root = <any>null;
            }
        });

    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
    }
}

const close = () => {
    editor?.dispose();
    editorScript?.dispose();
    emit("update:modelValue", "");
}

const copy = (char: any) => {
    if (!editorScript) {
        navigator.clipboard.writeText(getCodeString(char)).then(() => {
            GlobalInfo.toast("已拷贝");
        }).catch(err => {
            GlobalInfo.showError(err);
        });
        return;
    }
    // 要插入的字符串
    var textToInsert = getCodeString(char);

    // 获取当前的光标位置
    var position = editorScript.getPosition();

    // 使用 executeEdits 方法插入文本
    editorScript.executeEdits("", [
        { range: new monaco.Range(position.lineNumber, position.column, position.lineNumber, position.column), text: textToInsert }
    ]);

    // 如果需要，调整光标位置
    editorScript.setPosition(new monaco.Position(position.lineNumber, position.column + textToInsert.length));

    editorScript.focus();
}
</script>

<template>
    <div class="pageContent">
        <Loading v-if="isBusy" class="loadingV3" />

        <!-- Start Page Header -->
        <div class="page-header">
            <h1 class="title">代码转换</h1>
            <ol class="breadcrumb horow">
                常用字符转义（_cUTF8编码值_）：
                <template v-for="citem, index in simpleChars">
                    <span style="color:green;" @click="copy(citem)" v-html="simpleCharsText[index]"></span>&nbsp;=&nbsp;
                    <span style="color:green;" @click="copy(citem)">{{ getCodeString(citem) }}
                    </span>&nbsp;&nbsp;&nbsp;
                </template>
            </ol>

            <!-- Start Page Header Right Div -->
            <div class="right">
                <div class="btn-group" role="group" aria-label="...">
                    <button class="btn btn-light" @click="startParser" v-if="status == 0">解析代码</button>
                    <button class="btn btn-light" @click="saveAndTransform" v-else-if="status == 1">开始转换</button>
                    <a @click="close" class="btn btn-light"><i class="fa fa-close"></i></a>
                </div>
            </div>
            <!-- End Page Header Right Div -->

        </div>
        <!-- End Page Header -->


        <div class="row">
            <div class="col-md-12" style="height: 100%;">
                <div class="panel panel-default editor-panel" style="height: 100%;">
                    <div ref="containerEle" :style="{ flex: status === 1 ? 0.5 : 1 }">

                    </div>
                    <div ref="scriptContainerEle" v-if="status === 1" style="flex: 1;">

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