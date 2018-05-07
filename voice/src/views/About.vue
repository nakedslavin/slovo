<template>
  <div class="about">
    <b-card-group class="my-5">
      <b-card>
        <b-card-header>
          SLOVO.io - Open Source Podcasts
        </b-card-header>
        <b-card-body>
          <p class="card-text">
This is a podcasting platform which can be used for creating podcasting feeds out of an html.<br/>
In order to use the service, there should be an adapter reading for parsing the website needed.<br/><br/>There are a couple of them ready and you can check those at the links below. If your website is not there, you can use a simple DSL to create your own parser and submit it as a pull request. Or drop a note.
          </p>
        </b-card-body>
      </b-card>
    </b-card-group>
  </div>
</template>

<script>
export default {
  name: 'about'
}
</script>