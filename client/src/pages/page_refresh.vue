<template>
<div>
  
</div>
</template>
<script>
export default {
    name: 'refresh',
    data(){
      return {
        //班次
        schedule_type:0,
        selected_time:"",
        schedule_truck_count:1,
      }
    },
    components:{
      
    },
    beforeRouteEnter(to,from,next){
      console.log('test');
      next((vm)=>{
        vm.$router.replace(from.path);
      })
    }
}
</script>
<style >
.el-row {
    margin-bottom: 20px;
    text-align: left;
  }
  .el-col{
    margin-right: 20px;
  }
</style>