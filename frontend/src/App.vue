<template>
  <div id="app">
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary">
      <div class="container">
        <router-link class="navbar-brand" to="/">MQTT-Proxy</router-link>
        <button
          class="navbar-toggler"
          type="button"
          data-toggle="collapse"
          data-target="#navbarSupportedContent"
          aria-controls="navbarSupportedContent"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarSupportedContent">
          <ul class="navbar-nav mr-auto">
            <router-link to="/" exact tag="li" class="nav-item">
              <a class="nav-link">Home</a>
            </router-link>
            <router-link to="/interceptor" tag="li" class="nav-item">
              <a class="nav-link">Interceptor</a>
            </router-link>
            <router-link to="/clients" tag="li" class="nav-item">
              <a class="nav-link">Clients</a>
            </router-link>
          </ul>
        </div>
      </div>
    </nav>

    <router-view></router-view>
  </div>
</template>


<script>
export default {
  name: "app",
  components: {},
  data() {
    return {};
  },
  methods: {},
  created: function() {}
};
</script>