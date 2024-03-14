<script setup lang="ts">
import { GlobalInfo } from '@/GlobalInfo';
import Loading from '@/components/Loading.vue';
import { ref, reactive, onMounted } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const data = reactive({ Name: "", Password: "" });
const isBusy = ref(false);
const rememberMe = ref(false);

onMounted(async () => {
    var token = localStorage.getItem("Token");
    if (token) {
        try {
            var userinfo = await GlobalInfo.get("/User/GetUserInfo", null, token);
            //console.warn("用户信息",userinfo);
            userinfo = JSON.parse(userinfo);
            GlobalInfo.UserInfo.Name = userinfo.Name;
            GlobalInfo.UserInfo.Role = userinfo.Role;
            GlobalInfo.UserInfo.Version = userinfo.Password;
            GlobalInfo.UserInfo.Token = token;
        } catch (error) {
            GlobalInfo.UserInfo.Token = "";

            fillValues();
        }
    }
    else {
        fillValues();
    }
});

const fillValues = () => {
    if (localStorage.getItem("UserName")) {
        data.Name = <any>localStorage.getItem("UserName");
        data.Password = <any>localStorage.getItem("Pwd");
        rememberMe.value = true;
    }
}

const okClick = async () => {
    if (isBusy.value)
        return;

    isBusy.value = true;
    try {
        var token = await GlobalInfo.postJson("/User/Login", data);
        GlobalInfo.UserInfo.Token = token;

        var userinfo = await GlobalInfo.get("/User/GetUserInfo", null);
        //console.warn("用户信息",userinfo);
        userinfo = JSON.parse(userinfo);
        GlobalInfo.UserInfo.Name = userinfo.Name;
        GlobalInfo.UserInfo.Role = userinfo.Role;
        GlobalInfo.UserInfo.Version = userinfo.Password;

        localStorage.setItem("Token", token);
        if (rememberMe) {
            localStorage.setItem("UserName", data.Name);
            localStorage.setItem("Pwd", data.Password);
        }
        data.Password = "";

        router.push("/");
    } catch (e) {
        GlobalInfo.showError(e);
    }
    finally {
        isBusy.value = false;
    }
}

const keyup = (e: KeyboardEvent) => {
    if (e.key == "Enter") {
        okClick();
    }
}
</script>

<template>
    <div class="login-form">
        <form>
            <div class="top">
                <img src="/img/kode-icon.png" alt="icon" class="icon">
                <h4>Rapid Deployment Platform</h4>
            </div>
            <div class="form-area">
                <div class="group">
                    <input type="text" @keyup="keyup" class="form-control" placeholder="Username" v-model="data.Name">
                    <i class="fa fa-user"></i>
                </div>
                <div class="group">
                    <input type="password" @keyup="keyup" class="form-control" placeholder="Password"
                        v-model="data.Password">
                    <i class="fa fa-key"></i>
                </div>
                <div class="checkbox checkbox-primary">
                    <input id="chkrememberMe" type="checkbox" v-model="rememberMe">
                    <label for="chkrememberMe"> Remember Me</label>
                </div>
                <button v-if="!isBusy" type="button" @click="okClick" class="btn btn-default btn-block">LOGIN</button>
                <Loading v-else class="loadingV2" />
            </div>
        </form>
        <!-- <div class="footer-links row">
            <div class="col-xs-6"><a href="#"><i class="fa fa-external-link"></i> Register Now</a></div>
            <div class="col-xs-6 text-right"><a href="#"><i class="fa fa-lock"></i> Forgot password</a></div>
        </div> -->
    </div>
</template>

<style scoped></style>