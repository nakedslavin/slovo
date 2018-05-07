<template>
  <div class="home">
    <b-card-group>
      <CreateFeed @feedUpdated="feedUpdated" />
      <Panel :code="jsonData"  />
    </b-card-group>
    <hr/>
    <b-row>
      <b-col xs="6">
        <ShowLine items="6" title="Last Added"  :auth="auth" :authenticated="authenticated" />
      </b-col>
      <b-col xs="6">
        <ShowLine items="6" title="Most Popular" :auth="auth" :authenticated="authenticated" />
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
    }
  }
}
</script>