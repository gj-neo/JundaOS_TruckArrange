<template>
     <el-col :span="24">
    <el-menu 
      router
      default-active="1-1"
      class="el-menu-vertical-demo"
      @open="handleOpen"
      @close="handleClose">
     <el-menu-item index="1-1">
        <i class="el-icon-thumb"></i>
          <span>排班</span>
     </el-menu-item>
      <el-menu-item index="2">
        <i class="el-icon-truck"></i>
          <span>车辆管理</span>
      </el-menu-item>
       <el-menu-item index="3">
         <i class="el-icon-map-location"></i>
          <span>工地管理</span>
      </el-menu-item>
      <el-menu-item index="4">
        <i class="el-icon-files"></i>
          <span>通行证管理</span>
      </el-menu-item>
      <el-submenu index="5">
        <template slot="title">
          <i class="el-icon-s-marketing"></i>
          <span>统计查询</span>
        </template>
        <el-menu-item-group>
          <el-menu-item index="5-5">车辆总出勤概览</el-menu-item>
          <el-menu-item index="5-2">排班任务概览</el-menu-item>
          <el-menu-item index="5-3">车辆出勤查询</el-menu-item>
          <!-- <el-menu-item index="5-4">调试功能</el-menu-item> -->
        </el-menu-item-group>
      </el-submenu>
    </el-menu>
  </el-col>
  </template>
<script>
  export default {
    methods: {
      handleOpen(key, keyPath) {
        console.log(key, keyPath);
      },
      handleClose(key, keyPath) {
        console.log(key, keyPath);
      }
    }
  }
</script>
<style scoped >
 .el-menu-item-group{
     margin-left: 0;
 }
 .el-menu-item{
     min-width: 50px!important;
 }
 .el-menu{
     text-align: left;
 }
 .el-col{
     height: 100%;
 }
</style>