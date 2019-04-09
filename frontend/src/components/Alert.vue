<template>
    <b-alert
      :show="dismissCountDown"
      dismissible
      v-bind:variant="this.variant"
      @dismissed="dismissCountDown=0"
      @dismiss-count-down="countDownChanged"
    >
      <p>{{this.msg}}</p>
      <b-progress
        v-bind:variant="this.variant"
        :max="duration"
        :value="dismissCountDown"
        height="4px"
      ></b-progress>
    </b-alert>
</template>

<script>
export default {
    name: "Alert",
    props: ["duration"],
    data: function(){
        return {
            dismissCountDown: 0,
            msg: "",
            variant: "info"
        }
    },
    methods:{
        show: function(msg){
            this.dismissCountDown = this.duration;
            if(msg)
                this.msg = msg;
        },
        showError: function(msg){
            this.variant = "danger";
            this.show(msg);
        },
        showSuccess: function(msg){
            this.variant = "success";
            this.show(msg);
        },
        showInfo: function(msg){
            this.variant = "info";
            this.show(msg);
        },
        countDownChanged(dismissCountDown) {
            this.dismissCountDown = dismissCountDown
        },
    }
}
</script>

<style>
.alert{
    position: fixed;
    margin: 0 30%;
    bottom: 0px;
    width: 40%;
}
</style>
