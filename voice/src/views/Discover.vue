<template>
  <div class="discover my-5">
    <b-row>
      <b-col xs="12"><b-card title="Discover shows" sub-title="You can subscribe to the show, listen to it, or syndicate them into a single radio station."></b-card></b-col>
    </b-row>
    <b-row class="w-100 m-2"></b-row>
    <b-row>
      <b-col xs="6">
        <ShowLine items="20" title="Last Added" :auth="auth" :authenticated="authenticated"/>
      </b-col>
      <b-col xs="6">
        <ShowLine items="20" title="Most Popular" :auth="auth" :authenticated="authenticated" />
      </b-col>
    </b-row>
  </div>
</template>

<script>
import ShowLine from '@/components/ShowLine.vue'
export default {
  name: 'discover',
  data () {
    return {
    }
  },
  props:['auth', 'authenticated'],
  components: {
    ShowLine
  },
  methods: {
  }
}
</script>