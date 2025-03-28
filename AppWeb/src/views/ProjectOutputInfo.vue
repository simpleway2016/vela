<script setup lang="ts">
import { GlobalInfo } from '@/GlobalInfo';
import Loading from '@/components/Loading.vue';
import { WebSocketConnection } from "@/WebSocketConnection";
import { ref, onMounted, onUnmounted, defineProps, onUpdated } from 'vue';
import { Terminal } from "xterm";
import { FitAddon } from "xterm-addon-fit";
import { WebglAddon } from "xterm-addon-webgl"
import "xterm/css/xterm.css";

const props = defineProps(["modelValue", "project"]);
const emit = defineEmits(['update:modelValue']);

const isViewingProgramOutput = ref(false);
var viewProgramSocket: WebSocketConnection = <any>undefined;

const lines = ref([10, 100, 200, 300, 500, 800, 1000, 2000, 5000]);
const setPreLineNum = ref(10);

const files = ref(<any[]>[]);
const containerEle = ref(<HTMLElement><any>null);
var _autoScroll = true;
const isBusy = ref(false);
const isBusyWritingBlob = ref(false);
var tempBuffer = <any[]><any>[];

const isPublishing = ref(false);
var showBuffer = <any[]><any>null;

const isStopScrolling = ref(false);
onMounted(() => {
    initTerm();
    init();

});

const fitAddon = new FitAddon();
const webglAddon = new WebglAddon();
var term: Terminal;
const initTerm = () => {
    term = new Terminal({
        lineHeight: 1.2,
        fontSize: 13,
        fontFamily: 'Consolas,Liberation Mono,Menlo,Courier,monospace',
        theme: {
            foreground: '#d2d2d2',
            background: '#2b2b2b',
            cursor: '#adadad',
            black: '#000000',
            red: '#d81e00',
            green: '#5ea702',
            yellow: '#cfae00',
            blue: '#427ab3',
            magenta: '#89658e',
            cyan: '#00a7aa',
            white: '#dbded8',
            brightBlack: '#686a66',
            brightRed: '#f54235',
            brightGreen: '#99e343',
            brightYellow: '#fdeb61',
            brightBlue: '#84b0d8',
            brightMagenta: '#bc94b7',
            brightCyan: '#37e6e8',
            brightWhite: '#f1f1f0',
        },
        // 光标闪烁
        disableStdin: true,
        cursorBlink: true,
        cursorStyle: 'block',
        scrollback: 5000,//允许回滚多少条内容，超出5000后，前面的内容被自动清除
        tabStopWidth: 4
    });
    term.open(containerEle.value);
    term.loadAddon(fitAddon);
    term.loadAddon(webglAddon);

    // 不能初始化的时候fit,需要等terminal准备就绪,可以设置延时操作
    setTimeout(() => {
        fitAddon.fit();
    }, 5);

    window.addEventListener("resize", onResize);
}

const onResize = () => {
    fitAddon.fit();
};

const init = async () => {
    isBusy.value = true;
    isBusyWritingBlob.value = true;
    try {


        var blob = await GlobalInfo.getForBlob("/AgentService/GetOutputInfo", { guid: props.modelValue });
        term.clear();
        writeBlob(blob);


        var arr = await GlobalInfo.get("/AgentService/GetLogFiles", { guid: props.modelValue });
        arr = JSON.parse(arr);
        files.value.splice(0, 0, ...arr)
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusyWritingBlob.value = false;
        isBusy.value = false;
    }

    GlobalInfo.WebSocketReceivers.push(receive);
}

const writeBlob = async (blob: Blob) => {
    var uint8Array = new Uint8Array(await blob.arrayBuffer());
    term.write(uint8Array);
}

const viewFile = async (file: any) => {
    if (isBusy.value)
        return;

    if (isViewingProgramOutput.value) {
        viewProgramOutput();
    }

    isBusyWritingBlob.value = true;
    isBusy.value = true;
    try {
        var blob = await GlobalInfo.postForBlob("/AgentService/GetLogFileContent", { guid: props.modelValue, filename: file.FileName });
        term.clear();
        await writeBlob(blob);
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusyWritingBlob.value = false;
        isBusy.value = false;
    }
}

onUnmounted(() => {
    window.removeEventListener("resize", onResize);

    window.clearTimeout(sendNothingTimer);
    sendNothingTimer = 0;

    if (viewProgramSocket) {
        viewProgramSocket.stop();
        viewProgramSocket = <any>undefined;
    }

    var index = GlobalInfo.WebSocketReceivers.indexOf(receive);
    GlobalInfo.WebSocketReceivers.splice(index, 1);
});

const publish = async () => {
    if (isPublishing.value || isBusy.value)
        return;

    if (isViewingProgramOutput.value) {
        viewProgramOutput();
    }
    isBusy.value = true;
    isPublishing.value = true;
    try {
        await GlobalInfo.get("/AgentService/Run", { guid: props.modelValue, cols: term.cols, rows: term.rows });
       term.clear();
    } catch (error) {
        GlobalInfo.showError(error);
    }
    finally {
        isBusy.value = false;
        isPublishing.value = false;
    }

}

const stopPublish = async () => {
    try {
        await GlobalInfo.get("/AgentService/StopBuild", { guid: props.modelValue });
    } catch (error) {
        GlobalInfo.showError(error);
    }
}

const showLen = (length: number) => {
    var k = parseInt(<any>(length / 1024));
    if (k == 0)
        return length + "字节"

    return k + "K";
}
const close = () => {
    if (viewProgramSocket) {
        viewProgramSocket.stop();
        viewProgramSocket = <any>undefined;
    }
    
    emit("update:modelValue", "");
}

const textDecoder = new TextDecoder("utf-8");
const receive = (data: any, guid: any) => {
    if (isBusyWritingBlob.value || isPublishing.value) {
        tempBuffer.push(new Uint8Array(data));
        return;
    }
    else if (tempBuffer.length > 0) {
        tempBuffer.forEach(d => {
            term.write(d);
        });
        tempBuffer = [];
    }

    if (!guid && data.Guid)
        guid = data.Guid;

    // if (watchWebSocket && guid == props.modelValue && isViewingProgramOutput.value == false) {
    //     showText(data.Info);
    // }

    if (guid == props.modelValue && isViewingProgramOutput.value == false) {
        if (showBuffer) {
            showBuffer.push(new Uint8Array(data));
        }
        else {
            try {
                if (textDecoder.decode(data).indexOf("正在编译Docker映像") >= 0) {
                    term.clear();
                }
            } catch (error) {
                console.error(error);
                console.log(data);
            }
            term.write(new Uint8Array(data));
        }
    }
}

var sendNothingTimer = 0;
var lastDatas: any;
const sendNothing = () => {
    try {
        viewProgramSocket?.send("ok");
    } catch (error) {
        return;
    }
    sendNothingTimer = window.setTimeout(sendNothing, 3000);
}

const preLineNumClick = (value: any) => {
    setPreLineNum.value = value;
    
    viewProgramSocket.stop();
    term.clear();
    viewProgramSocket.Url = `/ViewProgramOutput?guid=${props.modelValue}&preline=${setPreLineNum.value}`;
    //datas.value.splice(0, datas.value.length);
    (<any>viewProgramSocket)._toResetUrl = true;
    viewProgramSocket.listen();
}

const viewProgramOutput = () => {

    isViewingProgramOutput.value = !isViewingProgramOutput.value;
    if (isViewingProgramOutput.value) {
        term.clear();

        viewProgramSocket = new WebSocketConnection(`/ViewProgramOutput?guid=${props.modelValue}&preline=${setPreLineNum.value}`);
        (<any>viewProgramSocket)._toResetUrl = true;
        viewProgramSocket.onMessage = (s, data) => {
            if ((<any>viewProgramSocket)._toResetUrl) {
                (<any>viewProgramSocket)._toResetUrl = false;
                viewProgramSocket.Url = `/ViewProgramOutput?guid=${props.modelValue}&preline=0`;
            }
            term.write(data + "\r\n");
        };
        viewProgramSocket.listen();
        sendNothingTimer = window.setTimeout(sendNothing, 3000);
    }
    else {
        if (viewProgramSocket) {
            window.clearTimeout(sendNothingTimer);
            sendNothingTimer = 0;
            viewProgramSocket.stop();
            viewProgramSocket = <any>undefined;

            term.clear();

        }
    }
}

const termClick = () => {
    // if (!showBuffer) {
    //     showBuffer = [];
    //     isStopScrolling.value = true;
    // }
    // else {
    //     showBuffer.forEach(item => {
    //         term.write(item);
    //     });
    //     showBuffer = <any>null;
    //     isStopScrolling.value = false;
    // }
}


</script>

<template>
    <div class="main">
        <Loading v-if="isBusy" class="loadingV3" />
        <div v-if="isStopScrolling" class="tip">暂停滚动</div>
        <div class="viewheader">
            <div class="right hrow">
                <div class="btn-group" role="group" aria-label="...">
                    <button class="btn btn-light" @click="publish"><i class="fa fa-cloud-upload"></i>立刻发布</button>
                    <button class="btn btn-light" @click="stopPublish"><i class="fa fa-times-circle"></i>中断发布</button>
                    <button class="btn btn-light"
                        :class="{ actived: isViewingProgramOutput }" @click="viewProgramOutput"><i
                            class="fa fa-desktop"></i>查看程序控制台输出</button>
                    <a @click="close" class="btn btn-light"><i class="fa fa-close"></i>关闭</a>
                </div>
                <div v-if="!isViewingProgramOutput" class="dropdown link" style="display: block;margin-left: 20px;">
                    <a data-toggle="dropdown" class="dropdown-toggle hdbutton" style="cursor: pointer;">发布日志历史
                        <span class="caret"></span></a>
                    <ul class="dropdown-menu dropdown-menu-list dropdown-menu-right">
                        <li v-for="file in files">
                            <a @click="viewFile(file)"><i class="fa falist fa-paper-plane-o"></i>{{ (<any>new
                                Date(file.CreateTime)).Format("yyyy-MM-dd HH:mm:ss") }} {{ showLen(file.Length) }}</a>
                        </li>
                    </ul>
                </div>

                <div v-if="isViewingProgramOutput" class="dropdown link" style="display: block;margin-left: 20px;">
                    <a data-toggle="dropdown" class="dropdown-toggle hdbutton" style="cursor: pointer;">往前{{ setPreLineNum
                    }}行
                        <span class="caret"></span></a>
                    <ul class="dropdown-menu dropdown-menu-list dropdown-menu-right">
                        <li v-for="lineNum in lines">
                            <a @click="preLineNumClick(lineNum)"><i class="fa falist fa-paper-plane-o"></i>{{ lineNum
                            }}行</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="viewcontentContainer">
            <div ref="containerEle" class="viewcontent" @click="termClick">

            </div>
        </div>
    </div>
</template>

<style scoped>
.main {
    background-color: #f5f5f5;
    position: fixed;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    z-index: 10;
    display: flex;
    flex-direction: column;
}

.viewheader {
    height: 60px;
    background-color: #fff;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;

}

.viewheader .btn {
    font-size: 13px;
}

.viewcontentContainer {
    flex: 1;
    overflow: hidden;
    padding: 8px;
    background-color: #2b2b2b;
}

.viewcontent {
    width: 100%;
    height: 100%;
    overflow: hidden;
}

.item {
    padding: 0 15px;
    word-break: break-all;
}

.hrow {
    display: flex;
    flex-direction: row;
    align-items: center;
}

.actived {
    background-color: #399bff;
    color: #fff;
}

.tip {
    position: fixed;
    top: 80px;
    right: 30px;
    padding: 5px;
    background-color: rgba(10, 71, 238, 0.6);
    border-radius: 5px;
    color: #fff;
    font-size: 15px;
    z-index: 999;
    pointer-events: none;
}
</style>

<style>
.viewcontent b {
    color: #000;
    font-weight: 600;
}

.viewcontent .error {
    color: red;
}

.xterm-viewport::-webkit-scrollbar {
    background-color: #2b2b2b;
    width: 10px;
}

.xterm-viewport::-webkit-scrollbar-track {
    background-color: #2b2b2b;
}

.xterm-viewport::-webkit-scrollbar-thumb {
    background: #dbded8;
    border-radius: 100px;

}
</style>