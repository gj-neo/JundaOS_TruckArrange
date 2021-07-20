<template>
    <div id="homeDiv">
      <el-container>
        <el-header >
          <topbar></topbar>
        </el-header>
      <el-container >
        <el-aside style="width:150px" >
          <navbar ></navbar>
        </el-aside>
        <el-main>
          <keep-alive>
              <router-view v-if="this.$route.meta.keepAlive"></router-view>
            </keep-alive>
            <keep-alive>
              <router-view v-if="!this.$route.meta.keepAlive"></router-view>
            </keep-alive>
        </el-main>
      </el-container>
      
    </el-container>
    </div>
</template>
<script>
import navbar from '../components/layout/navbar'
import topbar from '../components/layout/topbar'
import bottombar from '../components/layout/bottombar'
import page_auto_schedule from '../pages/page_auto_schedule'
export default {
    name: 'home',
    components:{
      'navbar': navbar,
      'topbar': topbar,
      'bottombar': bottombar,
      'auto_schedule': page_auto_schedule
    }
}
</script>
<style >
.el-aside {
  height: 1020px;
  width:150px;
}
.el-header {
    position: relative;
    height: 60px;
    background-color:cornflowerblue;
    color: #333;
    text-align: center;
    line-height: 60px;
  }
  .el-footer{
     background-color: #B3C0D1;
     color: #333;
    text-align: center;
    line-height: 20px;
    position: absolute;
    margin-bottom: 10px;
  }
  .el-main{
    width:1366px;
    height: 100%;
    overflow-y:scroll;
  }
</style>