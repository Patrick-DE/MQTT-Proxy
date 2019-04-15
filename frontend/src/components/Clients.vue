<template>
    <div class="container">
        <div role="tablist">
            <b-card no-body class="mb-1" v-for="value,key in clients" :key="value.clientId">
            <b-card-header header-tag="header" class="p-1" role="tab">
                <b-button block href="#" v-b-toggle="value.clientId" variant="info">{{value.clientId}}</b-button>
            </b-card-header>
            <b-collapse v-bind:id="value.clientId" accordion="my-accordion" role="tabpanel">
                <b-card-body>
                    <b-card-text class="check">
                        <b-form-checkbox v-model="value.intercept" name="check-button" @input="toggleIntercept(key)" switch>
                            <b>Intercept</b>  (Checked: {{ value.intercept }})
                        </b-form-checkbox>
                    </b-card-text>
                    <b-row>
                        <b-col cols="6">
                            <h2>Client Out</h2>
                            <b-card-text>
                                <p v-for="value,key in value.clientOut" v-if="notObject(value)">
                                    {{key}}: {{ (value != null) ? value: emptySymbol }}
                                </p>
                                <b>Options</b><br/>
                                <p v-for="value,key in value.clientOut.options" v-if="notObject(value)">
                                    {{key}}: {{ (value != null) ? value: emptySymbol }}
                                </p>
                                <b>Credentials</b><br/>
                                <p v-for="value,key in value.clientOut.options.Credentials" v-if="notObject(value)">
                                    {{key}}: {{ (value != null) ? value: emptySymbol }}
                                </p>
                                <b>ChannelOptions</b><br/>
                                <p v-for="value,key in value.clientOut.options.ChannelOptions" v-if="notObject(value)">
                                    {{key}}: {{ (value != null) ? value: emptySymbol }}
                                </p>
                                <b>TlsOptions</b><br/>
                                <p v-for="value,key in value.clientOut.options.ChannelOptions.TlsOptions" v-if="notObject(value)">
                                    {{key}}: {{ (value != null) ? value: emptySymbol }}
                                </p>
                            </b-card-text>
                        </b-col>
                        <b-col cols="6">
                            <h2>Client In</h2>
                            <b-card-text>
                                <p v-for="value,key in value.clientOut" v-if="notObject(value)">
                                    {{key}}: {{ (value != null) ? value: emptySymbol }}
                                </p>
                                <b>Options</b><br/>
                                <p v-for="value,key in value.clientOut.options" v-if="notObject(value)">
                                    {{key}}: {{ (value != null) ? value: emptySymbol }}
                                </p>
                                <b>Credentials</b><br/>
                                <p v-for="value,key in value.clientOut.options.Credentials" v-if="notObject(value)">
                                    {{key}}: {{ (value != null) ? value: emptySymbol }}
                                </p>
                                <b>ChannelOptions</b><br/>
                                <p v-for="value,key in value.clientOut.options.ChannelOptions" v-if="notObject(value)">
                                    {{key}}: {{ (value != null) ? value: emptySymbol }}
                                </p>
                                <b>TlsOptions</b><br/>
                                <p v-for="value,key in value.clientOut.options.ChannelOptions.TlsOptions" v-if="notObject(value)">
                                    {{key}}: {{ (value != null) ? value: emptySymbol }}
                                </p>
                            </b-card-text>
                        </b-col>
                    </b-row>
                    <b-button size="sm" @click="disconnect(key)" class="client_remove_button" variant="danger">Force disconnect</b-button>
                </b-card-body>
            </b-collapse>
            </b-card>
        </div>
        <Alert ref="yourMomGayAlert" v-bind:duration=3></Alert>
    </div>
</template>

<script>
import Alert from "@/components/Alert"
import { isObject } from 'util';

export default {
    name: "Clients",
    components: {Alert},
    data: function(){
        return {
            clients: [],
            emptySymbol: '-',
            ip: '192.168.1.21'
        }
    },
    created: function(){
        this.getClients();
    },
    methods:{
        getClients: function(){
            this.axios
                .get(`http://${this.ip}/api/manager/all`)
                .then(response => {
                     this.clients = response.data;
                })
                .catch(response => {
                    this.$refs.yourMomGayAlert.showError("Error: while fetching all clients!");
                    console.error(response);
                });
        },
        notObject: function(elem) {
            return  !isObject(elem);
        },
        disconnect: function(clientId){
            this.axios
                .delete(`http://${this.ip}/api/manager/${clientId}`)
                .then(response => {
                    this.$refs.yourMomGayAlert.showSuccess(response.data);
                    console.log(this.clients);
                    console.log(this.clients[clientId]);
                    delete this.clients[clientId];
                    console.log(this.clients[clientId]);
                    console.log(this.clients);
                })
                .catch(response => {
                    this.$refs.yourMomGayAlert.showError("Error: while disconnecting client!");
                    console.error(response);
                });
        },
        toggleIntercept: function(clientId){
            this.axios
                .put(`http://${this.ip}/api/manager/${clientId}/intercept/${this.clients[clientId].intercept}`)
                .then(response => {
                    this.$refs.yourMomGayAlert.showSuccess(response.data);
                })
                .catch(response => {
                    this.$refs.yourMomGayAlert.showError("Error: while toggle intercept!");
                    console.error(response);
                });
        }
    }
}
</script>

<style>
.client_remove_button{
    margin: 0 auto;
    display: block;
}
.check{
    text-align: center;
}
</style>
