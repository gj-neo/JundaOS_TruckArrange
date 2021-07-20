<template>
<div>
  <!--标题栏-->
    <el-row>
      <el-col :span="24">
        <div class="grid-content">
         <el-divider content-position="left">车辆管理</el-divider>
        </div>
      </el-col>
    </el-row>
    <el-row>
      <el-col :span="24">
        <truck_display 
        v-bind:activeTruckInfo=activeTruckInfo
        v-bind:stopTruckInfo=stopTruckInfo
        v-bind:stopedTruckCount=stopedTruckCount
        v-bind:alivedTruckCount=alivedTruckCount></truck_display>
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
import truck_display from '../components/truck_display'
import {Loading} from 'element-ui'
export default {
    name: 'home',
    data(){
      return {
        //班次
        schedule_type:0,
        selected_time:"",
        schedule_truck_count:1,
        all_truck_info:[],
        activeTruckInfo:[],
        stopTruckInfo:[],
        stopedTruckCount:0,
        alivedTruckCount:0,
      }
    },
    components:{
      
      'truck_display': truck_display,
    },
    
    created(){
      
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
        this.all_truck_info.forEach(element => {
           if(element['truck_status']==1){             
             this.activeTruckInfo.push({
                        "truck_id":element["truck_id"],
                        "truck_num":element["truck_num"],
                        "truck_license":element["truck_license"],
                      });
           }
           else{
             this.stopTruckInfo.push({
                        "truck_id":element["truck_id"],
                        "truck_num":element["truck_num"],
                        "truck_license":element["truck_license"],
                      });
           }
        });
        this.stopedTruckCount=this.stopTruckInfo.length;
        this.alivedTruckCount=this.activeTruckInfo.length;
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