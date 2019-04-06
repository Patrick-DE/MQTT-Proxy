<template>
    <div id="app">
        <nav class="navbar navbar-expand-lg navbar-dark bg-primary">
            <div class="container">
                <router-link class="navbar-brand" to="/">MQTT-Proxy</router-link>
                <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
              <span class="navbar-toggler-icon"></span>
            </button>
                <div class="collapse navbar-collapse" id="navbarSupportedContent">
                    <ul class="navbar-nav mr-auto">
                        <router-link to="/" exact tag="li" class="nav-item">
                            <a class="nav-link">Home</a>
                        </router-link>
                        <router-link to="/iterceptor" tag="li" class="nav-item">
                            <a class="nav-link">Interceptor</a>
                        </router-link>
                        <!--<router-link to="/about" tag="li" class="nav-item">
                            <a class="nav-link">Über</a>
                        </router-link>
                        <div class="nav-item">
                            <a class="nav-link" v-on:click="handleLoginLogout">
                                {{user ? `Logout (${this.user.name})` : `Login`}}
                            </a>
                            <LoginModal ref="loginModal" @loggedIn="loggedIn"></LoginModal>
                        </div>-->
                    </ul>
                </div>
            </div>
        </nav>
    
        <router-view></router-view>
    </div>
</template>


<script>
import LoginModal from './components/LoginModal';

export default {
    name: 'app',
    components: { LoginModal },
    data() {
        return {
            user: null
        };
    },
    methods: {
        handleLoginLogout: function() {
            if (!this.user) {
                this.$refs.loginModal.showModal();
            } else {
                this.logout();
            }
        },
        loggedIn: function(user) {
            this.user = user;
        },
        logout: function() {
            this.axios
                .post("/auth/logout")
                .then(res => {
                    this.user = null;
                }).catch((error) => {
                    console.log(error.response);
                });
        }
    },
    created: function() {
        this.axios
            .get("/auth/status")
            .then(res => {
                this.user = res.data.user;
            }).catch(err => { this.user = { name: "dummy", role: "admin"}});
    }
}
</script>