<script setup lang="ts">
import { ref, reactive } from 'vue';
import { GlobalInfo } from './GlobalInfo';
import LoginView from './views/LoginView.vue';
import { useRouter } from 'vue-router';
const userInfo = GlobalInfo.UserInfo;
const routeInfo = GlobalInfo.routeInfo;
const ProjectListProperties = GlobalInfo.ProjectListProperties;
const router = useRouter();

const leftMenuEle = ref(<HTMLElement><any>null);
const showHideLeftMenu = () => {
    (<any>window).$(leftMenuEle.value).toggle(100);
}

const searchKeyUp = (e: KeyboardEvent) => {
    if (e.key == "Enter") {
        router.push({ name: 'serviceList', params: { search: ProjectListProperties.searchKey } });
    }
}
const searchKeyUp_codeMission = (e: KeyboardEvent) => {
    if (e.key == "Enter") {
        router.push({ name: 'codeMission', params: { search: ProjectListProperties.codeMissionSearchKey } });
    }
}
const logout = () => {
    userInfo.Token = <any>null;
    localStorage.removeItem("Token");
}
</script>

<template>
    <div class="main">
        <!-- //////////////////////////////////////////////////////////////////////////// -->
        <!-- START TOP -->
        <div id="top">

            <!-- Start App Logo -->
            <div class="applogo">
                <a href="index.html" class="logo">Vela</a>
            </div>
            <!-- End App Logo -->

            <!-- Start Sidebar Show Hide Button -->
            <a class="sidebar-open-button" @click="showHideLeftMenu"><i class="fa fa-bars"></i></a>
            <a class="sidebar-open-button-mobile"><i class="fa fa-bars"></i></a>
            <!-- End Sidebar Show Hide Button -->

            <!-- Start Searchbox -->
            <div class="searchform">
                <input type="text" class="searchbox" @keyup="searchKeyUp" v-model="ProjectListProperties.searchKey"
                    placeholder="Search" v-if="routeInfo.currentPath!='codeMission'">
                    <input type="text" class="searchbox" @keyup="searchKeyUp_codeMission" v-model="ProjectListProperties.codeMissionSearchKey"
                    placeholder="搜索模型" v-if="routeInfo.currentPath=='codeMission'">
                <span class="searchbutton"><i class="fa fa-search"></i></span>
            </div>
            <!-- End Searchbox -->



            <!-- Start Top Right -->
            <ul class="top-right">

                <li class="dropdown link">
                    <div style="display: flex;flex-direction: row;align-items: center;cursor: pointer;"
                        data-toggle="dropdown" class="dropdown-toggle profilebox"><img src="/img/profileimg.png"
                            alt="img"><b>{{ userInfo.Name }}</b><span class="caret"></span></div>
                    <ul class="dropdown-menu dropdown-menu-list dropdown-menu-right">
                        <li role="presentation" class="dropdown-header">用户中心</li>
                        <li>
                            <RouterLink to="/changePassword"><i class="fa falist fa-wrench"></i>修改密码</RouterLink>
                        </li>
                        <li><a @click="logout"><i class="fa falist fa-power-off"></i> 退出登录</a></li>
                    </ul>
                </li>

            </ul>
            <!-- End Top Right -->

        </div>
        <!-- END TOP -->
        <!-- //////////////////////////////////////////////////////////////////////////// -->

        <!-- bottom -->
        <div id="bottom">
            <!-- //////////////////////////////////////////////////////////////////////////// -->
            <!-- START SIDEBAR -->
            <div ref="leftMenuEle" id="leftMenu" class="sidebar">

                <ul class="sidebar-panel nav">
                    <li class="sidetitle">{{ routeInfo.isBusy ? "正在加载..." : "MAIN" }}</li>

                    <template v-if="!routeInfo.isBusy">
                        <li v-if="userInfo.Role == 1048577">
                            <RouterLink to="/agentList">
                                <span class="icon color8"><i class="fa fa-th"></i></span>服务器列表
                            </RouterLink>
                        </li>
                        <li>
                            <RouterLink to="/serviceList">
                                <span class="icon color8"><i class="fa fa-diamond"></i></span>程序部署列表
                            </RouterLink>
                        </li>
                    </template>
                </ul>

                <ul class="sidebar-panel nav" v-if="!routeInfo.isBusy">
                    <li class="sidetitle">TOOLS</li>
                    <li>
                        <RouterLink to="/codeMission">
                            <span class="icon color8"><i class="fa fa-fire"></i></span>代码转换
                        </RouterLink>
                    </li>
                </ul>

                <ul class="sidebar-panel nav" v-if="!routeInfo.isBusy">
                    <li class="sidetitle">SYSTEM</li>
                    <li v-if="userInfo.Role == 1048577">
                        <RouterLink to="/userManagement">
                            <span class="icon color8"><i class="fa fa-cubes"></i></span>系统用户
                        </RouterLink>
                    </li>
                    <li>
                        <RouterLink to="/logList">
                            <span class="icon color8"><i class="fa fa-file-text-o"></i></span>操作日志
                        </RouterLink>
                    </li>
                </ul>

                <div class="version">{{ (<any>userInfo.Version).version() }}</div>
            </div>
            <!-- END SIDEBAR -->
            <!-- //////////////////////////////////////////////////////////////////////////// -->

            <!-- //////////////////////////////////////////////////////////////////////////// -->
            <!-- START CONTENT -->
            <div id="mainContent">
                <RouterView v-slot="{ Component, route }">
                    <component :is="Component" />
                </RouterView>
            </div>
            <!-- End Content -->
            <!-- //////////////////////////////////////////////////////////////////////////// -->
        </div>


    </div>
    <div class="loginarea" v-show="!userInfo.Token">
        <LoginView />
    </div>
</template>

<style scoped>
.main {
    width: 100%;
    height: 100%;
}

#top {
    height: 60px;
    background: #399bff;
    color: #fff;
    width: 100%;
}

#bottom {
    position: absolute;
    top: 60px;
    left: 0;
    right: 0;
    bottom: 0;
    display: flex;
    flex-direction: row;
}

#leftMenu {
    width: 250px;
    flex-shrink: 0;
}

#mainContent {
    background-color: #f5f5f5;
    flex: 1;
    flex-shrink: 0;
    width: 1px;
}

.loginarea {
    position: absolute;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    background-color: #F5F5F5;
}

.version {
    position: fixed;
    bottom: 0;
    left: 0;
    width: 250px;
    height: 30px;
    font-size: 12px;
    text-align: center;
    pointer-events: none;
    opacity: 0.8;
}

.sidebar-panel li a[aria-current='page'] {
  color: #f2f2f2 !important;
  background: rgba(0, 0, 0, 0.2) !important;
}
</style>