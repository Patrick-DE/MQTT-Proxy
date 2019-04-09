<template>
    <div>
        <div role="tablist">
            <b-card no-body class="mb-1" v-for="client in clients" v-bind:key="client.clientId">
            <b-card-header header-tag="header" class="p-1" role="tab">
                <b-button block href="#" v-b-toggle.accordion-1 variant="info">{{client.clientId}}</b-button>
            </b-card-header>
            <b-collapse id="accordion-1" visible accordion="my-accordion" role="tabpanel">
                <b-card-body>
                    <b-card-text>
                        <b-form-checkbox v-model="checked" name="check-button" switch>
                            Intercept:  <b>(Checked: {{ client.intercept }})</b>
                        </b-form-checkbox>
                    </b-card-text>
                    <b-row>
                        <b-col cols="6">
                            <h2>Client Out</h2>
                            <b-card-text>{{ text }}</b-card-text>
                        </b-col>
                        <b-col cols="6">
                            <h2>Client In</h2>
                            <b-card-text>{{ text }}</b-card-text>
                        </b-col>
                    </b-row>
                </b-card-body>
            </b-collapse>
            </b-card>
        </div>
        <Alert ref="yourMomGayAlert" v-bind:duration=3></Alert>
    </div>
</template>

<script>
import Alert from "@/components/Alert"

export default {
    name: "Clients",
    components: {Alert},
    data: function(){
        return {
            clients: [],
            text: `
                Anim pariatur cliche reprehenderit, enim eiusmod high life accusamus terry
                richardson ad squid. 3 wolf moon officia aute, non cupidatat skateboard dolor
                brunch. Food truck quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon
                tempor, sunt aliqua put a bird on it squid single-origin coffee nulla
                assumenda shoreditch et. Nihil anim keffiyeh helvetica, craft beer labore
                wes anderson cred nesciunt sapiente ea proident. Ad vegan excepteur butcher
                vice lomo. Leggings occaecat craft beer farm-to-table, raw denim aesthetic
                synth nesciunt you probably haven't heard of them accusamus labore VHS.
                `,
            checked: false,
        }
    },
    created: function(){
        this.getClients();
    },
    methods:{
        getClients: function(){
            this.axios
                .get('http://127.0.0.1/api/manager/all')
                .then(response => {
                    var keys = Object.keys(response.data)
                    for(var i=0; i<keys.length; i++){
                        this.clients[i] = response.data[keys[i]];
                    }
                    console.log(this.clients);
                })
                .catch(response => {
                    this.$refs.yourMomGayAlert.showError("Error: fetching all clients!");
                    console.error(response);
                });
        }
    }
}
</script>

<style>
</style>
