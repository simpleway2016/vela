<script setup lang="ts">
import { GlobalInfo } from "@/GlobalInfo";
import JmsUploader  from "jms-uploader";
import { onMounted } from "vue";
import { ref } from "vue";
const myfile = ref(<HTMLInputElement><any>null);
const submit = async () => {
    if (myfile.value.files?.length) {
        var uploader = new JmsUploader(`${GlobalInfo.ServerUrl}/User/Test`, myfile.value.files[0], {}, {
            Name: "jack"
        });
        var ret = await uploader.upload();
        console.log("成功上传", ret);
    }
}

// 让divFiles支持文件拖拽功能，拖拽文件到它上面，自动赋值给 myfile，代码采用vue3响应式语法
const divFiles = ref(<HTMLDivElement><any>null);
onMounted(() => {
    divFiles.value.ondragover = (e) => {
        e.preventDefault();
        e.stopPropagation();
        return false;
    }
    divFiles.value.ondrop = (e: any) => {
        e.preventDefault();
        e.stopPropagation();
        myfile.value.files = e.dataTransfer.files;
        if (myfile.value.files?.length) {
            for (var i = 0; i < myfile.value.files.length; i++) {
                divFiles.value.innerHTML += (myfile.value.files[i].name) + "<br>";
            }
        }
        return false;
    }
});
</script>
<template>
    <div>
        <div style="width: 300px;height: 300px;background-color: #ccc;" ref="divFiles"></div>
        <input ref="myfile" type="file">
        <button @click="submit">提交</button>
    </div>
</template>
<style scoped>
div {
    background-color: #fff;
}
</style>