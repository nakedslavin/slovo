<template>
    <div>
        <aplayer 
            :music="{
                title: item.title,
                artist: item.guestName + ' ' + item.pubDate,
                src: item.encloseUrl,
                pic: item.image
            }"/>
        <h5 class="pt-3">{{item.title}}</h5>
        <p class="small">{{item.description}}</p>
        <b-badge v-for="tag in item.keywords" :key="tag" class="mr-1">{{tag}}</b-badge>
    </div>
</template>
<script>
import Aplayer from 'vue-aplayer'

export default {
    data() {
        return {

        }
    },
    props: ['item'],
    components : { Aplayer },
    created() {
    },
    methods: {
        constructLink(value) {
            return this.$apiHost + 'l/' + value 
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

</style>
