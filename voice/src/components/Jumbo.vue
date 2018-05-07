<template>
  <b-jumbotron bg-variant="info" text-variant="white">
        <template slot="header">
          СЛОВО / [slovo]
        </template>
        <template slot="lead">
          This is a platform for Open Source Podcasts, which can be used for creating RSS audio feeds out of anything.
        </template>
        <hr class="my-4">
        <p>
          In order to use the service, there should be an adapter reading for parsing the website needed. There are a couple of them ready and you can check those at the links below. If your website is not there, you can use a simple DSL to create your own parser and submit it as a pull request. Or drop a note.
        </p>
        <b-btn variant="warning" href="https://github.com/nakedslavin/osp">List of adapters</b-btn>
        <b-btn variant="success" v-b-modal.modalNote>Drop a note</b-btn>
        <b-modal id="modalNote" ref="modalNote" centered title="Send me a note" header-text-variant="white" header-bg-variant="info" footer-bg-variant="info" body-bg-variant="info" ok-variant="success" cancel-variant="warning" @ok="handleOk">
            <form @submit.stop.prevent="handleSubmit">
                <b-form-textarea class="my-4"
                        v-model="note"
                        placeholder="Enter something"
                        :rows="3"
                        :max-rows="6">
                </b-form-textarea>
            </form>
        </b-modal>
    </b-jumbotron>
</template>
<script>
export default {
    data() {
        return {
            note: null
        }
    },
    methods: {
        handleOk (evt) {
            evt.preventDefault()
            this.handleSubmit()
        },
        handleSubmit (evt) {
            if (this.note != null) {
                this.$http.post(this.$apiHost + 'home/send', this.note)
                    .then(
                        res => {this.$refs.modalNote.hide()}, 
                        res => {this.$refs.modalNote.hide()}
                    );
            }
            
        }
    }
}
</script>

