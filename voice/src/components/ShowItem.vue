<template>
    <div>
        <aplayer 
            :music="{
                title: item.title,
                artist: item.keywords.slice(5).join(','),
                src: item.encloseUrl,
                pic: item.image
            }"/>
        <h5>{{item.title}}</h5>
        <p class="small">{{item.description}}</p>
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
