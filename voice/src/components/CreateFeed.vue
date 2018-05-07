<template>
  <b-card class="col-md-5 col-xs-12 mt-2">
    <b-card-header class="p-0 py-2">Create a podcast</b-card-header>
    <div>
      <b-alert variant="danger" :show="errors.length != 0">
        <ul class="mb-0"><li v-for="err in errors" :key="err">{{err}}</li></ul>
      </b-alert>
      <b-form @submit="onSubmit" @reset="onReset" v-if="show" class="mt-3">
        <b-form-group label="Address Url:"
                      description="
                      Examples:<br/>
                      https://echo.msk.ru/programs/code<br/>
                      https://echo.msk.ru/programs/razvorot-morning/<br/>
                      https://www.youtube.com/user/teamcoco<br/>
                      https://www.youtube.com/channel/UCsAw3WynQJMm7tMy093y37A<br/><br/>
                      List of adapters is on github. But you try even without having one.
                      ">
          <b-form-input type="text"
                        v-model="form.address"
                        required
                        placeholder="Enter URL">
          </b-form-input>
        </b-form-group>
        <b-form-group label="Friendly Name:"
                      description="Enter a short, unique name of the show. Avoid using non URL symbols">
          <b-form-input type="text"
                        v-model="form.name"
                        required
                        placeholder="Enter Name">
          </b-form-input>
        </b-form-group>
        <b-form-group label="Category:">
          <b-form-select :options="categories"
                        required
                        v-model="form.category">
          </b-form-select>
        </b-form-group>
        <b-form-group>
          <b-form-checkbox-group v-model="form.checked">
            <!-- <b-form-checkbox value="echo">Echo Msk</b-form-checkbox> -->
          </b-form-checkbox-group>
        </b-form-group>
        <b-alert :show="in_process">Processing your request</b-alert>
        <b-alert variant="danger" dismissible :show="is_error">There was an error while processing your request</b-alert>
        <b-button :disabled="in_process" type="submit" variant="info">Create</b-button>
      </b-form>
    </div>
  </b-card>
</template>

<script>
export default {
  data () {
    return {
      errors: [],
      form: {
        address: '',
        name: null,
        category: null,
        checked: []
      },
      categories: [
        { text: 'Select One', value: null },
        'Arts', 'Business', 'Comedy', 'Education', 'Games & Hobbies','Government & Organizations','Health','Music','News & Politics','Science & Medicine','Society & Culture','Sports & Recreation','Technology'
      ],
      show: true,
      in_process: false,
      is_error: false
    }
  },
  methods: {
    onSubmit (evt) {
      this.errors = [];
      evt.preventDefault();
      if (!this.form.address || !this.validLink(this.form.address))
        this.errors.push("Please provide a link");
      if (!this.form.category)
        this.errors.push("Pick a category");
      
      if (!this.form.name || !this.validName(this.form.name))
        this.errors.push("Name should only contain alphanumeric chars");
      
      var self = this;
      this.validUniqueName(this.form.name, function() {
          if (self.errors.length == 0) {
            self.in_process = true;
            self.$http.post(self.$apiHost, JSON.stringify(self.form)).then(
                res => {
                  self.in_process = false;
                  self.is_error = res.body === null;
                  if (!self.is_error) {
                    self.$emit('feedUpdated', res.body);
                  }
                },
                res => { self.in_process = false; self.is_error = true; });
          }
      });
    },
    onReset (evt) {
      evt.preventDefault();
      /* Reset our form values */
      this.form.address = '';
      this.form.name = null;
      this.form.category = null;
      this.form.checked = false;
      this.errors = [];
      /* Trick to reset/clear native browser form validation state */
      this.show = false;
      this.$nextTick(() => { this.show = true });
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
          if (res.indexOf(name) != -1)
            this.errors.push("Name exists. Pick another one")
          fn()
        }
      });
    },
    validLink: function(link) {
      let re = /^(http|https)/;
      return re.test(link);
    }
  }
}
</script>
