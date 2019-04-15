<template>
    <div>
        <div class="container">
            <h1>Create custom message</h1>
            <b-form @submit="onSubmit" @reset="onReset" v-if="show">
            <b-form-group id="input-group-1" label="Your Topic:" label-for="input-1">
                <b-form-input
                id="input-1"
                v-model="form.topic"
                required
                placeholder="Enter topic"
                ></b-form-input>
            </b-form-group>

            <b-form-group id="input-group-2" label="Your PayloadString:" label-for="input-2">
                <b-form-input
                id="input-2"
                v-model="form.payloadString"
                required
                placeholder="Enter Payload (only string supported)"
                ></b-form-input>
            </b-form-group>


            <b-form-group id="input-group-3" label="QoS:" label-for="input-3">
                <b-form-select
                id="input-3"
                v-model="form.QoS"
                :options="qos"
                required
                ></b-form-select>
            </b-form-group>

            <b-form-group id="input-group-4" label="Your clientId:" label-for="input-4">
                <b-form-select
                id="input-4"
                v-model="form.ClientId"
                :options="clientId"
                required
                ></b-form-select>
            </b-form-group>

            <b-form-group id="input-group-5" label="Has RetainMsg:" label-for="input-5">
                <b-form-select
                id="input-5"
                v-model="form.RetainMsg"
                :options="retainMsg"
                required
                ></b-form-select>
            </b-form-group>

            <b-button type="submit" variant="primary">Submit</b-button>
            <b-button type="reset" variant="danger">Reset</b-button>
            </b-form>
            <b-card class="mt-3" header="Form Data Result">
            <pre class="m-0">{{ form }}</pre>
            </b-card>
        </div>
        <Alert ref="yourMomGayAlert" v-bind:duration=3></Alert>
    </div>
</template>

<script>
import Alert from "@/components/Alert"

export default {
    name: "NewMessage",
    components: {Alert},
    data() {
      return {
        form: {
          Topic: '',
          PayloadString: '',
          ClientId: null,
          QoS: 0,
          RetainMsg: false,
        },
        qos: [{ text: 'At most once', value: 0 }, { text: 'At least once', value: 1 }, { text: 'Exactly once', value: 2 }],
        clientId: [{ text: 'Select One', value: null }],
        retainMsg: [{ text: 'No', value: false }, { text: 'Yes', value: true }],
        show: true,
        ip: process.env.IP
      }
    },
    created: function() {
        this.getClients();
        console.log(process.env.ENV_VARIABLE)
    },
    methods: {
        getClients: function(){
            this.axios
                .get(`http://${this.ip}/api/manager/all`)
                .then(response => {
                    for(var elem in response.data){
                        this.clientId.push({text: elem.toUpperCase(), value: elem});
                    }
                })
                .catch(response => {
                    this.$refs.yourMomGayAlert.showError("Error: while fetching all clients!");
                    console.error(response);
                });
        },
        onSubmit(evt) {
            evt.preventDefault()
            this.axios
                .post(`http://${this.ip}/api/message/new`, JSON.stringify(this.form))
                .then(response => {
                    this.$refs.yourMomGayAlert.showSuccess("Message " + response.data.MsgId + " successfully created");
                })
                .catch(err => {
                    console.error(err);
                });
        },
        onReset(evt) {
            evt.preventDefault()
            // Reset our form values
            this.form.Topic = ''
            this.form.PayloadString= ''
            this.form.ClientId = null
            this.form.QoS = 0
            this.form.RetainMsg = false
            // Trick to reset/clear native browser form validation state
            this.show = false
            this.$nextTick(() => {
            this.show = true
            })
        }
    }
}
</script>

<style>

</style>
