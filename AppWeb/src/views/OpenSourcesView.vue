<script setup lang="ts">

import { GlobalInfo } from "@/GlobalInfo";
import Loading from "@/components/Loading.vue";
import Selector from "@/components/Selector.vue";
import { ref, onMounted } from "vue"
const isBusy = ref(false);

</script>

<template>
    <div class="pageContent">
        <Loading v-if="isBusy" class="loadingV3" />
       

        <!-- Start Row -->
        <div class="row">

            <!-- Start BlogPost -->
            <div class="col-md-12">
                <div class="panel panel-widget blog-post">
                    <div class="panel-body">
                        <p class="text">开源服务器端软件列表，采用Vela部署方式，从源码直接编译后部署...</p>
                        <a href="#">Read More</a>
                        <p class="author" v-for="item in 50">
                            <img src="/img/profileimg.png" alt="img">
                            <span>Jonathan Doe</span>
                            <div><a href="https://github.com/simpleway2016/JackRemoteLog.git" target="_blank">https://github.com/simpleway2016/JackRemoteLog.git</a></div>
                            Designer
                            <button v-if="item<3" class="btn btn-info">部署</button>
                            <div v-else class="nor">已部署</div>
                        </p>


                    </div>
                </div>
            </div>
            <!-- End BlogPost -->

        </div>
        <!-- End Row -->
    </div>
</template>
<style scoped>
button{
    position: absolute;
    right: 10px;
    bottom: 10px;
}
.nor{
    position: absolute;
    right: 10px;
    bottom: 10px;
    background-color: #eee;
    color: #666;
    padding: 6px 16px;
}
.panel-body {
    position: relative;
}
</style>