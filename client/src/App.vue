<template>
  <div id="app">    
   <home></home>
  </div>
</template>

<script>

import home from './pages/page_home'

export default {
  name: 'App',
  components:{
    'home':home,
  }
}
</script>

<style>
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 0px;
}
</style>
