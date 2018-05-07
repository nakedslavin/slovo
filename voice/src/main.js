import Vue from 'vue'
import App from './App.vue'
import router from './router'
import BootstrapVue from 'bootstrap-vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import VueResource from 'vue-resource'

Vue.use(VueResource)
Vue.use(BootstrapVue)
// Vue.prototype.$apiHost = "http://localhost:5000/"
Vue.prototype.$apiHost = "http://slovo.io/"
Vue.prototype.$appName = "SLOVO"

Vue.config.productionTip = false

new Vue({
  router,
  render: h => h(App)
}).$mount('#app')
