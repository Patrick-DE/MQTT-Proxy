import Vue from 'vue'
import BootstrapVue from 'bootstrap-vue'
import Router from 'vue-router'
import Home from '@/components/Home'
import Interceptor from '@/components/Interceptor'
import Clients from '@/components/Clients'

Vue.use(Router)
Vue.use(BootstrapVue)

export default new Router({
  mode: 'history',
  linkActiveClass: 'active',
  routes: [
    {
      path: '/',
      name: 'Home',
      component: Home
    },
    {
      path: '/interceptor',
      name: 'Interceptor',
      component: Interceptor,
    },
    {
      path: '/clients',
      name: 'Clients',
      component: Clients,
    }
  ]
})
