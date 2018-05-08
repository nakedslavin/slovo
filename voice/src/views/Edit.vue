<template>
  <div class="edit my-5">
    <b-row>
      <b-col xs="12">
        <b-card>
          <h4 class="card-title">{{podcast.title}}
            <b-btn variant="light" :href="construnctShowLink(podcast.name)" class="float-right close">x</b-btn>
          </h4>
          <h6 class="card-subtitle">Metadata Live editor</h6>
          <b-badge pill variant="secondary" class="float-right mt-2">{{podcast.host}}</b-badge>
          <p class="card-text">{{podcast.description}}</p>
          <b-btn-group size="sm">
            <b-btn variant="white" :href="construnctShowLink(podcast.name)">Back</b-btn>
            <b-btn variant="success" @click="save">Save</b-btn>
            <b-btn variant="danger" @click="del" v-if="podcast.humanId == auth.userProfile.email && podcast.humanId != null">Delete</b-btn>
            <b-btn variant="warning" :href="constructLink(podcast.name)">Feed</b-btn>
          </b-btn-group>
          <hr />
          <b-card-body>
            <b-alert variant="info" show dismissible>Some edits might require moderation</b-alert>
            <b-alert variant="success" :show="show" @dismissed="show=false" dismissible>Changes were successfuly submited</b-alert>
            <b-alert variant="danger" :show="errors.length > 0">
              <ul class="mb-0"><li v-for="err in errors" :key="err">{{err}}</li></ul>
            </b-alert>
            <b-form>
              <b-form-group label="Show Title:">
                  <b-form-input type="text"
                              v-model="podcast.title"
                              required
                              placeholder="Enter title">
                  </b-form-input>
              </b-form-group>
              <b-form-group label="Show Description:">
                  <b-form-textarea v-model="podcast.description"
                                  placeholder="Enter show notes"
                                  :rows="3"
                                  :max-rows="6">
                  </b-form-textarea>
              </b-form-group>
              <b-form-group label="Show Link:">
                  <b-form-input type="text"
                              v-model="podcast.link"
                              required
                              placeholder="Enter link">
                  </b-form-input>
              </b-form-group>
              <b-form-group label="Unique short name:">
                  <b-form-input type="text"
                              v-model="podcast.name"
                              required
                              placeholder="Enter name">
                  </b-form-input>
              </b-form-group>
              <b-form-group label="Copyrights Holder:">
                  <b-form-input type="text"
                              v-model="podcast.copyright"
                              required
                              placeholder="Enter copyright holder">
                  </b-form-input>
              </b-form-group>
              <b-form-group label="Show Author:">
                  <b-form-input type="text"
                              v-model="podcast.author"
                              required
                              placeholder="Enter author">
                  </b-form-input>
              </b-form-group>
              <b-form-group label="Show image source:">
                  <b-form-input type="text"
                              v-model="podcast.image"
                              required
                              placeholder="Enter image link">
                  </b-form-input>
              </b-form-group>
              <b-form-group label="Select a category:">
                  <b-form-select :options="categories"
                                required
                                v-model="podcast.category">
                  </b-form-select>
              </b-form-group>
              <b-form-group label="Select a language:">
                  <b-form-select :options="languages"
                                required
                                v-model="podcast.language">
                  </b-form-select>
              </b-form-group>
            </b-form>
          </b-card-body>
        </b-card>
      </b-col>
    </b-row>
    <b-row class="w-100 m-2"></b-row>
    <b-row>
      <b-col xs="6">
        <ShowLine itemsCount="6" title="Last Added" :auth="auth" :authenticated="authenticated"/>
      </b-col>
      <b-col xs="6">
        <ShowLine itemsCount="6" title="Most Popular" :auth="auth" :authenticated="authenticated" />
      </b-col>
    </b-row>
  </div>
</template>

<script>
import ShowLine from '@/components/ShowLine.vue'
import ShowItem from '@/components/ShowItem.vue'
import router from './../router'

export default {
  name: 'edit',
  data () {
    return {
      podcast: {},
      originalName : null,
      categories: [
        { text: 'Select One', value: null },
        'Arts', 'Business', 'Comedy', 'Education', 'Games & Hobbies','Government & Organizations','Health','Music','News & Politics','Science & Medicine','Society & Culture','Sports & Recreation','Technology'
      ],
      languages: ['ru','en','es','fr','de','cz','pl','uk','tr','am','sk','it'],
      show: false,
      errors: []
    }
  },
  props:['auth', 'authenticated'],
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
        this.$http.get(this.$apiHost + 's/' + this.$route.params.id).then(res => {
           this.podcast = res.body
           this.originalName = this.podcast.name
          })
    },
    constructLink(value) {
        return this.$apiHost + 'l/' + value 
    },
    construnctShowLink(value) {
        return '/show/' + value
    },
    save() {
      this.errors = []
      this.podcast.name = this.podcast.name == null ? null : this.podcast.name.toLowerCase()
      
      if (!this.podcast.title) this.errors.push("Title is emply")
      if (!this.podcast.link || !this.validLink(this.podcast.link)) this.errors.push("Link is incorrect. Use the http://....")
      if (!this.podcast.name || !this.validName(this.podcast.name)) this.errors.push("Name is invalid")

      if(this.errors.length == 0) {
        var self = this;
        this.validUniqueName(this.podcast.name, function() {
            if (self.errors.length == 0) {
              self.$http.post(self.$apiHost + 'home/update/' + self.podcast.name, JSON.stringify(self.podcast)).then(res => {
                self.show = true
                self.errors = []
              }, res => { self.errors.push("Information couldn't be proccessed") });
            }
        });
      }
    },
    del() {

      this.$http.post(this.$apiHost + 'home/delete/' + this.podcast.name, JSON.stringify(this.podcast), {
         headers: {
           Authorization: 'Bearer ' + this.auth.accessToken
         }
      }).then(res => {
                this.show = true
                this.errors = []
                router.replace('/')
              }, res => { this.errors.push("Information couldn't be proccessed") });
    },
    validEmail:function(email) {
      var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
      return re.test(email);
    },
    validName:function(name) {
      var re = /^([a-z\-0-9]+)$/;
      return re.test(name);
    },
    validUniqueName: function(name, fn) {
      fetch(this.$apiHost + 'home/names').then(res => res.json()).then(res => {
        if (res.error) {
          this.errors.push(res.error);
          fn()
        }
        else {
          res = Array.from(res).filter(el => el !== this.originalName)
          if (res.indexOf(this.podcast.name) != -1)
            this.errors.push("Name exists. Pick another one")
          fn()
        }
      });
    },
    validLink: function(link) {
      let re = /^(http|https)/;
      return re.test(link);
    }
  },
  created() {
    this.fetchPodcast()
  }
}
</script>