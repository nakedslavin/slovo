import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'
import About from './views/About.vue'
import Discover from './views/Discover.vue'
import Show from './views/Show.vue'
import Edit from './views/Edit.vue'
import Callback from '@/components/Callback'

Vue.use(Router)

export default new Router({
  mode: 'history',
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home,
      meta: {
        title: 'SLOVO'
      }
    },
    {
      path: '/callback',
      name: 'Callback',
      component: Callback
    },
    {
      path: '/discover',
      name: 'discover',
      component: Discover,
      meta: {
        title: 'SLOVO'
      }
    },
    {
      path: '/show/:id',
      name: 'show',
      component: Show,
      meta: {
        title: 'SLOVO'
      }
    },
    {
      path: '/edit/:id',
      name: 'edit',
      component: Edit,
      meta: {
        title: 'SLOVO'
      }
    },
    {
      path: '/about',
      name: 'about',
      component: About,
      meta: {
        title: 'SLOVO'
      }
    },
    {
      path: '*',
      redirect: '/',
      meta: {
        title: 'SLOVO'
      }
    }
  ]
})
