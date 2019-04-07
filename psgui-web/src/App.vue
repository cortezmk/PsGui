<template>
  <div id="app" v-bind:style="{ width: width + 'px', height: height + 'px' }" @resize="onResize">
  <TableView></TableView>
  </div>
</template>

<script>
import CommandResult from './assets/data.json'
import TableView from './components/TableView.vue'

export default {
  name: 'app',
  components: {
    TableView
  },
  data: function() { 
    return {
      message: CommandResult.ProjectedObjects[0].Type,
      width: 0,
      height: 0
    }
  },
  methods: {
    onResize: function() {
      this.width = window.innerWidth;
      this.height = window.innerHeight;
    }
  },
  created: function() {
    window.addEventListener("resize", this.onResize);
    this.width = window.innerWidth;
    this.height = window.innerHeight;
  }
}
</script>

<style>
#app {
  font-family: 'Consolas', 'Avenir', Helvetica, Arial, sans-serif;
  /* -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale; */
  color: #2c3e50;
  font-size: 12px;
  overflow: auto;
}
</style>
