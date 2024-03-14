import { createRouter, createWebHashHistory } from 'vue-router'
import AgentListView from "../views/AgentListView.vue"
import ServiceListView from '@/views/ServiceListView.vue'
import OpenSourcesView from '@/views/OpenSourcesView.vue'
import { GlobalInfo } from '@/GlobalInfo';

var timer = 0;

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path:"/",
      name:"home",
      component:AgentListView //import('@/views/TestFile.vue')
    },
    {
      path:"/agentList",
      name:"agentList",
      component:AgentListView
    },
    {
      path:"/serviceList/:search?",
      name:"serviceList",
      component:ServiceListView
    },
    {
      path:"/userManagement",
      name:"userManagement",
      component:() => import('@/views/UserManagement.vue')
    },
    {
      path:"/changePassword",
      name:"changePassword",
      component:() => import('@/views/ChangePassword.vue')
    },
    {
      path:"/projectUserList/:guid",
      name:"projectUserList",
      component:() => import('@/views/ProjectUserList.vue')
    },
    {
      path:"/agentUserList/:id",
      name:"agentUserList",
      component:() => import('@/views/AgentUserList.vue')
    },
    {
      path:"/logList",
      name:"logList",
      component:() => import('@/views/LogList.vue')
    },
    {
      path:"/restoreProject/:guid/:name",
      name:"restoreProject",
      component:() => import('@/views/RestoreProject.vue')
    }
    ,
    {
      path:"/fileDeleteSetting",
      name:"fileDeleteSetting",
      component:() => import('@/views/FileDeleteSettingView.vue')
    },
    {
      path:"/alarmSetting/:guid",
      name:"alarmSetting",
      component:() => import('@/views/AlarmSettingView.vue')
    },
    {
      path:"/codeMission/:search?",
      name:"codeMission",
      component:() => import('@/views/CodeMissionList.vue')
    },
    {
      path:"/openSources",
      name:"openSources",
      component:OpenSourcesView
    }
  ]
});

router.beforeEach((to, from, next) => {
  GlobalInfo.routeInfo.currentPath = <any>to.name;
  timer = window.setTimeout(()=>{
    GlobalInfo.routeInfo.isBusy = true;
  } , 200);
  next()
});

router.afterEach(()=>{
  window.clearTimeout(timer);
  timer = 0;
  GlobalInfo.routeInfo.isBusy = false;
});

export default router
