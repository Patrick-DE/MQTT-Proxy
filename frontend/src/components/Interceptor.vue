<template>
  <div>
    <h1> YourMomGay</h1>
    <b-table striped hover :items="msg"></b-table>
  </div>
</template>

<script>
const MSG = [
  { isActive: true, age: 40, first_name: "Dickerson", last_name: "Macdonald" },
  { isActive: false, age: 21, first_name: "Larsen", last_name: "Shaw" },
  { isActive: false, age: 89, first_name: "Geneva", last_name: "Wilson" },
  { isActive: true, age: 38, first_name: "Jami", last_name: "Carney" }
];

export default {
  name: "Interceptor",
  data: function() {
    return {
      msg: MSG
    };
  },
  created: function() {
    this.getAllMessages();
  },
  methods: {
    getAllMessages: function(event) {
      this.axios.get("/api/message/all")
        .success(res => {
            this.msg = res.data.msg;
        })
        .fail(res =>{
            console.log(res);
        });
    }
  }
};
</script>