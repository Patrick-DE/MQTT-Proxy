<template>
  <div>
    <div class="container">
      <h1>Interceptor</h1>
      <!--Filter-->
      <b-row>
        <b-col cols="3">
          <b-form-input v-model="ftopic" placeholder="Topic"></b-form-input>
        </b-col>
        <b-col cols="3">
          <b-form-input :type="'number'" v-model="fmsgId" placeholder="MsgId"></b-form-input>
        </b-col>
        <b-col cols="3">
          <b-form-input v-model="fclientId" placeholder="ClientId"></b-form-input>
        </b-col>
        <b-col cols="3">
          <b-form-input v-model="fpayloadString" placeholder="PayloadString"></b-form-input>
        </b-col>
      </b-row>
      <b-row>
        <b-col cols="12">
          <b-form-input v-model="fpayload" placeholder="Payload"></b-form-input>
        </b-col>
      </b-row>
      <b-row>
        <b-col cols="12">
          <b-button size="sm" variant="success">New Message</b-button>
        </b-col>
      </b-row>

      <!--Table for messages-->
      <b-table striped hover :items="this.msg" :fields="this.fields" v-bind:filter-function="this.filterData" v-bind:filter="'dummy'">
        
        <!--Button for editing area-->
        <template slot="show_details" slot-scope="row">
          <b-button size="sm" @click="row.toggleDetails" class="mr-2">
            {{ row.detailsShowing ? 'Hide' : 'Show'}} Details
          </b-button>
        </template>
        
        <!--Textarea in editing area-->
        <template slot="row-details" slot-scope="row">
          <b-card>
            <b-form-textarea
              id="textarea"
              v-model="row.item.PayloadString"
              @keyup="onKeyUp(row.item)"
              placeholder="Payload"
              rows="3"
              max-rows="6"
            ></b-form-textarea>
            <b-button size="sm" @click="sendMessage(row.item, 'clientOut')" variant="primary">Send request</b-button>
            <b-button size="sm" @click="sendMessage(row.item, 'clientIn')" variant="primary">Send response</b-button>
            <b-button size="sm" @click="updateMessage(row.item)" variant="info">Save</b-button>
            <b-button size="sm" @click="copyMessage(row.item)" variant="warning">Copy</b-button>
            <!--<b-button size="sm" @click="row.toggleDetails">Hide Details</b-button>-->
          </b-card>
        </template>

      </b-table>
    </div>
  <Alert ref="yourMomGayAlert" v-bind:duration=3></Alert>
  </div>
</template>

<script>
import Alert from "@/components/Alert"

const STATES = {"New":"New", "Sent":"Sent", "Intercepted":"Intercepted", "Modified":"Modified", "Dropped":"Dropped"};

export default {
  name: "Interceptor",
  components: {Alert},
  //props:['user'], No data transfert to this component
  data() {
    return {
      fields: [
        { key: "Timestamp", sortable: true,
          formatter: value => {
            return new Date(value).toLocaleString();
          }
        },
        { key: "MsgId", sortable: true },
        { key: "QoS", label: "QoS", sortable: false },
        { key: "Topic", sortable: true},
        { key: "State", sortable: true, formatter: this.stateToString },
        { key: "ClientId", sortable: true },
        { key: "PayloadString", sortable: true },
        { key: "Payload", sortable: true, formatter: this.base64toHEX},
        { key: 'show_details' }
      ],
      msg: null,

      //formatter vars
      ftopic: "",
      fmsgId: -1,
      fclientId: "",
      fpayloadString: "",
      fpayload: "",
    };
  },
  created: function() {
    this.getAllMessages();
	  this.$options.sockets.onmessage = this.processData;
  },
  methods: {
	processData: function(event) {
      Console.log("WS: Incomming Message");
      Console.log(event.data);
      var data = JSON.parse(event.data);
      var tmp = this.msg.filter(m => m.MsgId == data.MsgId);
      if (tmp){
        this.msg[this.msg.indexOf(tmp)] = res;
        Console.log("WS: Updated Message");
      }else{
        Console.log("WS: Added Message");
        this.msg.push(res);
      }
    },
    getAllMessages: function(event) {
      this.axios
        .get("http://127.0.0.1/api/message/all")
        .then(res => {
          this.msg = res.data;
        })
        .catch(res => {
          console.error(res);
        });
    },
    stateToString: function(state) {
      if (state < 0 || state >= STATES.length) return "INVALID";
      else return STATES[state];
    },
    base64toHEX: function(base64) {
      var raw = atob(base64);
      var HEX = "[";
      for (var i = 0; i < raw.length; i++) {
        HEX += "0x";
        var _hex = raw.charCodeAt(i).toString(16);
        HEX += (_hex.length == 2 ? _hex : "0" + _hex).toUpperCase() + ", ";
      }
      return (HEX = HEX.substring(0, HEX.length - 2) + "]");
    },
    filterData: function(item, filterProp){
      if(this.ftopic != "" && item.Topic.indexOf(this.ftopic) === -1)
        return false;
      if(this.fmsgId > -1 && item.MsgId != this.fmsgId)
        return false;
      if(this.fclientId != "" && item.ClientId.indexOf(this.fclientId) === -1)
        return false;
      if(this.fpayloadString != "" && item.PayloadString.indexOf(this.fpayloadString) === -1)
        return false;
      var tmp = this.base64toHEX(item.Payload);
      if(this.fpayload != "" && tmp.indexOf(this.fpayload) === -1)
        return false;
      return true;
    },
    /**
     * Modify payload depending on PayloadString changes
     * */
    onKeyUp: function(item) {
      item.Payload = btoa(item.PayloadString);
    },
    sendMessage: function(item, direction){
      //first save message
      this.updateMessage(item, function(err, data){
        if(err) return console.error(err);
        if(direction != "clientIn" && direction != "clientOut") return console.error("Please enter a valid direction");

        //res.State = STATES.New;
        this.axios
          .post(`http://127.0.0.1/api/manager/${data.ClientId}/${direction}/${data.MsgId}/send`)
          .then(response => {
            this.$refs.yourMomGayAlert.showSuccess("Message " + response.data.MsgId + " successfully sent");
          })
          .catch(err => {
            console.error(err);
          });
      }.bind(this));
    },
    craftMessage: function(item, next){
      this.axios
          .post(`http://127.0.0.1/api/message/new`, JSON.stringify(data))
          .then(response => {
            this.msg.push(response.data);
            this.$refs.yourMomGayAlert.showSuccess("Message " + response.data.MsgId + " successfully created");
            next(null, res.data);
          })
          .catch(err => {
            console.error(err);
          });
    },
    updateMessage: function(item, next){
      this.axios
        .post(`http://127.0.0.1/api/message/${item.MsgId}`, JSON.stringify(item))
        .then(res => {
          var tmp = this.msg.filter(m => m.MsgId == res.MsgId)
          if (tmp){
            this.msg[this.msg.indexOf(tmp)] = res;
          }else{
            this.msg.push(res);
          }
          if(next)
            next(null,res.data);
          this.$refs.yourMomGayAlert.showSuccess("Successfully saved");
        })
        .catch(res => {
          console.error(res);
          next(res);
        });
    },
    copyMessage: function(item){
      this.axios
        .post(`http://127.0.0.1/api/message/${item.MsgId}/copy`)
        .then(res => {
          this.msg.push(res.data);
          this.$refs.yourMomGayAlert.showSuccess("Successfully saved");
        })
        .catch(res => {
          console.error(res);
        });
    }   
  }
};
</script>

<style>
.form-control{
  margin-bottom: 10px;
}
</style>