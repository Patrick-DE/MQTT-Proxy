<template>
  <div class="container">
    <h1>YourMomGay</h1>
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
    <b-table striped hover :items="this.msg" :fields="this.fields" v-bind:filter-function="this.filterData" v-bind:filter="'yourmomgay'">
    </b-table>
  </div>
</template>

<script>
const STATES = ["Sent", "Intercepted", "Modified", "Dropped"];

export default {
  name: "Interceptor",
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
        { key: "Payload", sortable: true, formatter: this.base64toHEX}
      ],
      msg: null,

      ftopic: "",
      fmsgId: -1,
      fclientId: "",
      fpayloadString: "",
      fpayload: "",
    };
  },
  created: function() {
    this.getAllMessages();
  },
  methods: {
    getAllMessages: function(event) {
      this.axios
        .get("http://141.19.142.229/api/message/all")
        .then(res => {
          this.msg = res.data;
        })
        .catch(res => {
          console.err(res);
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
    }
  }
};
</script>