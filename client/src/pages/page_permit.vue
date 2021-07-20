<template>
<div>
  <!--标题栏-->
    <el-row>
      <el-col :span="24">
        <div class="grid-content">
         <el-divider content-position="left">通行证管理</el-divider>
        </div>
      </el-col>
    </el-row>
    <el-row>
      <el-col :span="24">
        <permit_display 
        v-bind:permit_info_list=permit_info_list
        v-bind:truck_info_list=all_truck_info></permit_display>
      </el-col>
    </el-row>
    <!--结果栏-->
    <el-row>
      <el-col :span="24">
      </el-col>
    </el-row>
</div>
</template>
<script>
import axios from 'axios'
import Common from '../components/utlis/common'
import permit_display from '../components/permit_display'
export default {
    name: 'permit',
    data(){
      return {
        //班次
        schedule_type:0,
        selected_time:"",
        schedule_truck_count:1,
        all_truck_info:[],
        activeTruckInfo:[],
        stopTruckInfo:[],
        permit_info_list:[],
      }
    },
    components:{
      
      'permit_display': permit_display,
    },
    created(){
      //获取车辆
      axios
      .get(Common.GetApiUrl('api/permit/query')+Common.Create_CommonParam_HttpGet("all",null))
      .then((response)=>{
        if(response.data.code!=1000){
          this.$message.error(response.data.message);
          return;
        }
        
        console.log(response.data.data);
        this.permit_info_list=response.data.data;
      });
      //获取车辆
      axios
      .get(Common.GetApiUrl('api/truck/query')+Common.Create_CommonParam_HttpGet("all",null))
      .then((response)=>{
        if(response.data.code!=1000){
          this.$message.error(response.data.message);
          return;
        }
        
        console.log(response.data.data);
        this.all_truck_info=response.data.data;
      })
    },
    methods:{
      fun(){

      }
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