<template>
<b-container fluid>
  <NavBar :auth="auth" :authenticated="authenticated" />
  <b-container id="app">
     <router-view 
        :auth="auth" 
        :authenticated="authenticated" />
  </b-container>
</b-container>
</template>
<script>
import NavBar from '@/components/NavBar.vue'
import AuthService from './auth/AuthService'

const auth = new AuthService()
const { login, logout, authenticated, authNotifier } = auth

export default {
  name: 'app',
  data () {
    authNotifier.on('authChange', authState => {
      this.authenticated = authState.authenticated
    })
    return {
      auth,
      authenticated
    }
  },
  components: { NavBar },
  created() {
    document.title = this.$route.meta.title || this.$appName
  },
  methods: {
    login,
    logout
  }
}
</script>

<style>
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
}
#nav {
  padding: 30px;
}
#nav a {
  font-weight: bold;
  color: #2c3e50;
}
#nav a.router-link-exact-active {
  color: #42b983;
}
</style>
