<template>
    <b-card>
        <b-card-header>{{this.items}} {{this.title}}</b-card-header>
        <b-list-group-item v-for="podcast in podcasts.slice(0,this.items)" :key="podcast.id">
            <b-img class="float-right mwp-100" thumbnail fluid :src="podcast.image" />
            <b-link :href="constructShowLink(podcast.name)"><h5>{{ podcast.title }}</h5></b-link>
            <p class="card-text">
                {{ podcast.description | trim }}
                <br />
                <small>Podcasts available: {{ podcast.items.length }}</small>                    
            </p>
            <b-button-group size="sm">
                <b-button :href="constructShowLink(podcast.name)" variant="info">Show</b-button>
                <b-button :href="podcast.link" variant="info">Origin</b-button>
                <b-button :href="constructLink(podcast.name)" variant="warning">Feed</b-button>
                <b-button variant="danger" @click="radioAdd(podcast)"><strong>+ Radio</strong></b-button>
            </b-button-group>
            <b-badge pill variant="secondary" class="float-right mt-2 clear">{{podcast.host}}</b-badge>

        </b-list-group-item>
        <b-modal ref="radio" centered title="Your Radio" ok-title="Create" ok-variant="info" @ok="radioAdded" cancel-variant="light" :ok-disabled="newRadio.podcasts.length == 0">
            <b-alert show variant="success" dismissible>
                <!-- <h4 class="alert-heading">Now this is just great!</h4> -->
                <p class="mb-0">
                    Hey {{auth.userProfile.name}}!<br/>
                    <small>It's now the right time to create your own radio channel.
                    It's a standard Podcasting RSS feed, which you can use anywhere you like.</small>
                </p>
            </b-alert>
            <b-card sub-title="Selected shows">
                <b-card-body>
                    <b-list-group>
                        <b-list-group-item v-for="pod in newRadio.podcasts" :key="pod.guid">
                            {{pod.title}}
                            <b-btn variant="sm" class="float-right" @click="removeFromRadio(pod)">Remove</b-btn>
                        </b-list-group-item>
                    </b-list-group>
                </b-card-body>
            </b-card>
            <hr/>
            <b-card sub-title="Radio information">
                <b-card-body>
                    <b-alert variant="danger" :show="errors.length > 0">
                        <ul class="mb-0"><li v-for="err in errors" :key="err">{{err}}</li></ul>
                    </b-alert>
                    <b-form>
                        <b-form-group description="Enter a title of your show">
                            <b-form-input v-model="newRadio.title" type="text" required placeholder="Enter title"></b-form-input>
                        </b-form-group>
                        <b-form-group description="Enter a short, unique name of the show. Avoid using non-URL symbols">
                            <b-input-group prepend="http://slovo.io/l/">
                                <b-form-input v-model="newRadio.name" type="text" required placeholder="Enter unique name"></b-form-input>
                            </b-input-group>
                        </b-form-group>
                    </b-form>
                </b-card-body>
            </b-card>
        </b-modal>
    </b-card>
</template>
<script>
import router from './../router'

export default {
    data() {
        return {
            errors: [],
            podcasts:[],
            newRadio: {
                title : null,
                name : null,
                podcasts: [],
                humanid : null,
                humanname: null,
                humanfullname: null,
                image: null
            }
        }
    },
    props: ['items','title', 'auth', 'authenticated'],
    created() {
        this.$http.get(this.$apiHost + 'home/list').then(
            res => {
                this.podcasts = res.body.sort(function(a,b){
                    return new Date(b.pubDate) - new Date(a.pubDate);
                });
            });
    },
    methods: {
        radioAdd(podcast) {
            if (!this.authenticated)
                this.auth.login()
            else {
                if (this.newRadio.podcasts.indexOf(podcast) == -1)
                    this.newRadio.podcasts.push(podcast)
                this.$refs.radio.show()
            }
        },
        radioAdded(evt) {
            evt.preventDefault()
            this.errors = []
            this.newRadio.name = this.newRadio.name == null ? null : this.newRadio.name.toLowerCase()

            this.newRadio.humanid = this.auth.userProfile.email
            this.newRadio.humanname = this.auth.userProfile.nickname
            this.newRadio.humanfullname = this.auth.userProfile.name
            this.newRadio.image = this.auth.userProfile.picture
            if (!this.newRadio.title) this.errors.push("Title is emply")
            if (!this.newRadio.name || !this.validName(this.newRadio.name)) this.errors.push("Name is invalid")

            if(this.errors.length == 0) {
                var self = this;
                this.validUniqueName(this.newRadio.name, function() {
                    if (self.errors.length == 0) {
                        var pay = self.newRadio;
                        pay.podcasts = self.newRadio.podcasts.map(p => p.id)
                        self.$http.post(self.$apiHost + 'home/radio/' + self.newRadio.name, JSON.stringify(pay)).then(res => {
                            self.errors = []
                            router.replace('/edit/'+res.body.name)
                        }, res => { self.errors.push("Information couldn't be proccessed"); pay.podcasts = []; });
                    }
                });
            }
        },
        removeFromRadio(podcast) {
            this.newRadio.podcasts.splice(this.newRadio.podcasts.indexOf(podcast),1)
        },
        constructLink(value) {
            return this.$apiHost + 'l/' + value 
        },
        constructShowLink(showName) {
            return '/show/' +showName;
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
                if (res.indexOf(this.newRadio.name) != -1)
                    this.errors.push("Name exists. Pick another one")
                fn()
                }
            });
        }
    },
    filters: {
        trim: function (value) {
            if (!value) return ''
            value = value.toString()
            return value.substr(0,100) + '..'
        }
    }
}
</script>
<style>
    body {
        overflow-x: hidden
    }
    .clear {
        clear: both
    }
    .mwp-100 {
        max-width: 100px !important;
    }
</style>
