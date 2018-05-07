<template>
    <b-navbar toggleable="md" type="dark" variant="info">

        <b-navbar-toggle target="nav_collapse"></b-navbar-toggle>

        <b-navbar-brand href="#"><span class="mx-3"><strong>SLOVO</strong></span></b-navbar-brand>
        
        <b-collapse is-nav id="nav_collapse">

            <b-navbar-nav>
                <router-link class="nav-link" to="/">Home</router-link>
                <router-link class="nav-link" to="/discover">Discover</router-link>
                <router-link class="nav-link" to="/about">About</router-link>
            </b-navbar-nav>
            <!-- Right aligned nav items -->
            <b-navbar-nav class="ml-auto">
                <b-btn-group size="sm" v-if="authenticated">
                    <img class="rounded" width="30" height="30" :src="auth.userProfile.picture"/>
                    <b-btn variant="success">
                        {{auth.userProfile.name}} a.k.a. {{auth.userProfile.nickname}}
                    </b-btn>
                    <b-btn variant="light" @click="auth.logout()">Logout</b-btn>
                </b-btn-group>
                <b-btn-group size="sm" v-if="!authenticated">
                    <b-btn variant="light" @click="auth.login()">Login</b-btn>
                </b-btn-group>
            </b-navbar-nav>
        </b-collapse>
    </b-navbar>
</template>
<script>
export default {
  name: 'NavBar',
  props: ['auth', 'authenticated'],
  created() {
  }
}
</script>