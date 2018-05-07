<template>
  <div class="show my-5">
    <b-row>
      <b-col xs="12">
        <b-card>
          <h4 class="card-title">{{podcast.title}}
            <b-btn variant="light" href="/" class="float-right close">x</b-btn>
          </h4>
          <h6 class="card-subtitle">{{podcast.author}}</h6>
          <b-badge pill variant="secondary" class="float-right mt-2">{{podcast.host}}</b-badge>
          <p class="card-text">{{podcast.description}}</p>
          <b-btn-group size="sm">
            <b-btn variant="white" href="/">Back</b-btn>
            <b-btn variant="info" :href="constructEditLink(podcast.name)">Edit</b-btn>
            <b-btn variant="warning" :href="constructLink(podcast.name)">Feed</b-btn>
          </b-btn-group>
          <hr />
          <b-card-body>
            <b-list-group>
              <b-list-group-item v-for="item in podcast.items" :key="item.guid">
                  <ShowItem :item="item" />
              </b-list-group-item>
            </b-list-group>
          </b-card-body>
        </b-card>
      </b-col>
    </b-row>
    <b-row class="w-100 m-2"></b-row>
    <b-row>
      <b-col xs="6">
        <ShowLine items="6" title="Last Added" :auth="auth" :authenticated="authenticated"/>
      </b-col>
      <b-col xs="6">
        <ShowLine items="6" title="Most Popular" :auth="auth" :authenticated="authenticated" />
      </b-col>
    </b-row>
  </div>
</template>

<script>
import ShowLine from '@/components/ShowLine.vue'
import ShowItem from '@/components/ShowItem.vue'
export default {
  name: 'show',
  data () {
    return {
      podcast: {}
    }
  },
  props: ['auth', 'authenticated'],
  components: {
    ShowLine, ShowItem
  },
  watch: {
    '$route': function() {
      this.fetchPodcast()
    }
  },
  methods: {
    fetchPodcast() {
        this.$http.get(this.$apiHost + 'show/' + this.$route.params.id).then(res => this.podcast = res.body)
    },
    constructLink(value) {
        return this.$apiHost + 'l/' + value 
    },
    constructEditLink(value) {
        return '/edit/' + value
    }
  },
  created() {
    this.fetchPodcast()
  }
}
</script>