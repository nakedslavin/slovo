<template>
  <b-card class="col-md-7 col-xs-12 mt-2">
    <Jumbo v-show="jsCode == null" />
    <b-card-body v-show="jsCode != null">
      <b-input-group prepend="	&#x1f517;">
        <b-form-input v-model="podcastLink"></b-form-input>
        <b-input-group-append>
          <b-btn-group>
            <b-btn variant="info" @click="previewRSS"><strong>RSS</strong></b-btn>
            <b-btn variant="warning" @click="jsCode = null"><strong>Close</strong></b-btn>
          </b-btn-group>
        </b-input-group-append>
      </b-input-group>
      <hr/>
      <Printer :jscode="jsCode"  />
    </b-card-body>
  </b-card>
</template>

<script>
import Printer from '@/components/Printer.vue'
import Jumbo from '@/components/Jumbo.vue'
export default {
  data() {
    return {
      jsCode: null
    }
  },
  components:{ Printer, Jumbo },
  computed: {
    podcastLink: {
      get() {
        if (this.code && this.code.name) {
          return this.$apiHost + 'l/' + this.code.name
          //return location.protocol+'//'+location.host+'/link/'+ this.code.name;
        }
        return null;
      },
      set(value) {}
    }
  },
  watch: {
    code : function() {
      this.jsCode = this.code
    }
  },
  props: ["code"],
  methods: {
    previewRSS() {
      this.$http.get(this.podcastLink).then(
          res => {
            this.jsCode = res.body
          });
    
    }
  }
}
</script>