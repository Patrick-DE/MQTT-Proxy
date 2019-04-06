<template>
    <div>
    
        <b-modal id="modal" ref="modal" title="[FS-I] Login" @ok="handleOk">
    
            <div class="d-block text-center">
    
                <i class="fas fa-angle-double-left"></i><i>pls no hakkerino</i><i class="fas fa-angle-double-right"></i>
    
            </div>
    
            <form @submit.stop.prevent="handleSubmit">
    
                <label for="username">Username</label>
    
                <b-form-input id="username" type="text" placeholder="Enter your name" v-model="username" />
    
                <label for="username">Passwort</label>
    
                <b-form-input id="password" type="password" placeholder="Enter your password" v-model="password" />
    
                <b-alert dismissible fade :show="(errormsg && errormsg.length > 0)" @dismissed="errormsg=null" ref="alert" variant="danger">{{errormsg}}</b-alert>
    
            </form>
    
        </b-modal>
    
    </div>
</template>

<script>
export default {
    name: "LoginModal",
    data: function() {
        return {
            "username": "",
            "password": "",
            "errormsg": null
        };
    },
    methods: {
        showModal() {
            this.$refs.modal.show()
        },
        hideModal() {
            this.$refs.modal.hide()
        },
        handleOk(evt) {
            // Prevent modal from closing
            evt.preventDefault()
            if (!this.username || !this.password) {
                this.errormsg = 'Fehlender Username/Passwort';
            } else {
                this.handleSubmit()
            }
        },
        handleSubmit() {
            this.$refs.modal.busy = true;
            this.errormsg = null;
            this.axios
                .post("/auth/login", { username: this.username, password: this.password })
                .then(res => {
                    this.$emit('loggedIn', res.data.user);
                    this.hideModal();
                    this.$refs.modal.busy = false;
                }).catch((error) => {
                    console.log(error.response);
                    if (error.response && error.response.data && error.response.data.err) {
                        this.errormsg = error.response.data.err;
                    } else {
                        this.errormsg = error.toString();
                    }
                    this.$refs.modal.busy = false;
                });
        }
    }
}
</script>