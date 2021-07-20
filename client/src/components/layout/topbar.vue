<template>
    <div > 
        <el-row>
            <el-col :span="1">
                <div class="grid-content bg-purple-dark">
                    <img src="../../assets/logo.png" height="60" width="60">
                     <!-- <span>该版本仅供测试</span> -->
                </div>
            </el-col>
        </el-row>
    </div>
</template>
<script>
  export default{
      name:"topbar",
      data(){
          return {
              msg:""
          }
      }
  }
</script>
<style scoped>
 
</style>