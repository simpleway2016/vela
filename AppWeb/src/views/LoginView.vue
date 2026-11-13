<script setup lang="ts">
import { GlobalInfo } from '@/GlobalInfo';
import Loading from '@/components/Loading.vue';
import { ref, reactive, onMounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import QRCode from 'qrcode';

const router = useRouter();
const data = reactive({ Name: "", Password: "", GoogleCode: "" });
const isBusy = ref(false);
const rememberMe = ref(false);
const requiresGoogleCode = ref(false);
const bindingSecret = ref("");
const bindingTitle = ref("");
const bindingQrCode = ref("");
watch([bindingSecret, bindingTitle], async ([secret, title], _, onCleanup) => {
    let cancelled = false;
    onCleanup(() => { cancelled = true; });
    bindingQrCode.value = "";
    if (!secret) return;
    // Generate locally so the binding secret never goes to a QR image service.
    const label = `${encodeURIComponent(title)}:${encodeURIComponent(data.Name)}`;
    const parameters = new URLSearchParams({
        secret, issuer: title, algorithm: "SHA1", digits: "6", period: "30"
    });
    try {
        const image = await QRCode.toDataURL(`otpauth://totp/${label}?${parameters}`, {
            width: 240, margin: 4, errorCorrectionLevel: "M"
        });
        if (!cancelled) bindingQrCode.value = image;
    } catch {
        // The manual setup key remains available if image generation fails.
    }
});
const resetVerification = () => {
    requiresGoogleCode.value = false;
    bindingSecret.value = "";
    bindingQrCode.value = "";
    data.GoogleCode = "";
};
watch(() => [data.Name, data.Password], resetVerification);

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

const fillValues = async () => {
    if (localStorage.getItem("UserName")) {
        data.Name = <any>localStorage.getItem("UserName");

        try {
            data.Password = await GlobalInfo.postForm("/User/Decrypt", { content: localStorage.getItem("Pwd") });
        } catch (error) {

        }

        rememberMe.value = true;
    }
}

const okClick = async () => {
    if (isBusy.value)
        return;

    isBusy.value = true;
    try {
        var token = await GlobalInfo.postJson("/User/Login", data);
        if (token.startsWith("{")) {
            const challenge = JSON.parse(token);
            if (challenge.RequiresGoogleCode) {
                requiresGoogleCode.value = true;
                bindingSecret.value = challenge.Secret || "";
                bindingTitle.value = challenge.Title || "Vela Web";
                data.GoogleCode = "";
                return;
            }
        }
        GlobalInfo.UserInfo.Token = token;

        var userinfo = await GlobalInfo.get("/User/GetUserInfo", null);
        //console.warn("用户信息",userinfo);
        userinfo = JSON.parse(userinfo);
        GlobalInfo.UserInfo.Name = userinfo.Name;
        GlobalInfo.UserInfo.Role = userinfo.Role;
        GlobalInfo.UserInfo.Version = userinfo.Password;

        localStorage.setItem("Token", token);
        if (rememberMe.value) {
            try {
                var pwd = await GlobalInfo.postForm("/User/Encrypt", { content: data.Password });
                localStorage.setItem("UserName", data.Name);
                localStorage.setItem("Pwd", pwd);
            } catch (error) {

            }
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

</script>

<template>
    <div class="login-form">
        <form @submit.prevent="okClick">
            <div class="top">
                <img src="/img/kode-icon.png" alt="icon" class="icon">
                <h4>Rapid Deployment Platform</h4>
            </div>
            <div class="form-area">
                <div class="group">
                    <input type="text" class="form-control" placeholder="Username" v-model="data.Name">
                    <i class="fa fa-user"></i>
                </div>
                <div class="group">
                    <input type="password" class="form-control" placeholder="Password"
                        v-model="data.Password">
                    <i class="fa fa-key"></i>
                </div>
                <div v-if="requiresGoogleCode" class="google-verification">
                    <div v-if="bindingSecret" class="binding-help">
                        <p>请在 Google Authenticator 中选择“扫描二维码”，扫码添加账号：</p>
                        <img v-if="bindingQrCode" :src="bindingQrCode" class="binding-qr"
                            alt="Google Authenticator 绑定二维码" width="240" height="240">
                        <p>也可选择“输入设置密钥”，手动添加以下账号：</p>
                        <p>账号：{{ bindingTitle }}:{{ data.Name }}<br>密钥：<code>{{ bindingSecret }}</code><br>类型：基于时间</p>
                        <p>输入应用生成的 6 位验证码，验证成功后完成绑定并登录。绑定密钥 10 分钟内有效。</p>
                    </div>
                    <div class="group">
                        <input type="text" class="form-control" placeholder="请输入谷歌验证码"
                            v-model="data.GoogleCode" inputmode="numeric" pattern="[0-9]{6}"
                            maxlength="6" autocomplete="one-time-code" required aria-label="谷歌验证码">
                        <i class="fa fa-shield"></i>
                    </div>
                    <button v-if="bindingSecret" type="button" class="btn btn-link" :disabled="isBusy"
                        @click="resetVerification">重新获取绑定密钥</button>
                </div>
                <div class="checkbox checkbox-primary">
                    <input id="chkrememberMe" type="checkbox" v-model="rememberMe">
                    <label for="chkrememberMe"> Remember Me</label>
                </div>
                <button v-if="!isBusy" type="submit" class="btn btn-default btn-block">{{ requiresGoogleCode ? (bindingSecret ? "绑定并登录" : "验证并登录") : "LOGIN" }}</button>
                <Loading v-else class="loadingV2" />
            </div>
        </form>
        <!-- <div class="footer-links row">
            <div class="col-xs-6"><a href="#"><i class="fa fa-external-link"></i> Register Now</a></div>
            <div class="col-xs-6 text-right"><a href="#"><i class="fa fa-lock"></i> Forgot password</a></div>
        </div> -->
    </div>
</template>

<style scoped>
.login-form { width: min(360px, calc(100% - 32px)); }
@media (max-height: 800px) {
    .login-form { padding-top: 24px; }
}
.binding-help { margin-bottom: 16px; line-height: 1.6; }
.binding-qr { display: block; max-width: 100%; height: auto; margin: 12px auto; }
.binding-help code { overflow-wrap: anywhere; user-select: all; }
</style>