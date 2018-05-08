<template>
  <div class="home">
    <b-card-group class="mt-3">
      <CreateFeed @feedUpdated="feedUpdated" />
      <Panel :code="jsonData"  />
    </b-card-group>
    <hr/>
    <b-row>
      <b-col xs="6">
        <ShowLine itemsCount="6" title="Last Added" filter="{}" sort="{CreationTimestamp: -1}"  :auth="auth" :authenticated="authenticated" />
      </b-col>
      <b-col xs="6">
        <ShowLine v-if="authenticated" itemsCount="6" title="My Radios and Podcasts"  :filter="getQuery()" sort="{BuildTimestamp: -1}" :auth="auth" :authenticated="authenticated" />
        <ShowLine v-if="!authenticated" itemsCount="6" title="Last Updated"  filter="{}" sort="{BuildTimestamp: -1}" :auth="auth" :authenticated="authenticated" />
      </b-col>
    </b-row>
  </div>
</template>

<script>
// @ is an alias to /src
import CreateFeed from '@/components/CreateFeed.vue'
import Panel from '@/components/Panel.vue'
import ShowLine from '@/components/ShowLine.vue'

export default {
  name: 'home',
  data () {
    return {
      jsonData: null
    }
  },
  props: ['auth', 'authenticated'],
  components: {
    CreateFeed,
    Panel,
    ShowLine
  },
  methods: {
    feedUpdated(value) {
      this.jsonData = value;
    },
    getQuery() {
      return { HumanId: this.auth.userProfile.email }
    }
  }
}
</script>
<style scoped>

</style>