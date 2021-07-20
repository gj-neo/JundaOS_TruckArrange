<template>
<div>
  <!--标题栏-->
    <el-row>
      <el-col :span="24">
        <div class="grid-content">
         <el-divider content-position="left">调试功能</el-divider>
        </div>
      </el-col>
    </el-row>
    <el-row>
     <!-- <el-button @click="FillTruck2Site_CountStatistic">truck_site_attend_statistic数据生成</el-button> -->
     <el-button @click="ClearStatisticCount">清空所有车辆的出勤及统计数据</el-button>
     
    </el-row>
     <div class="grid-content">
         <el-divider content-position="left">随机排班(可选择时间：月，选择前请先清空所有排班数据）</el-divider>
        </div>
    <!--结果栏-->
    <el-row>
      <span class="demonstration">选择排班持续时间(/月)（从当前时间算起)</span>
      <el-input-number v-model="randomScheduleTime" @change="handleChange" :min="1" :max="10" label="月"></el-input-number>
     <!-- <el-button @click="FillTruck2Site_CountStatistic">truck_site_attend_statistic数据生成</el-button> -->
     <el-button @click="RandomSchedule_WithoutPermit">无通行证排班</el-button>
      <el-button @click="RandomSchedule_OnlyPermit">只有通行证排班</el-button>
       <el-button @click="RandomSchedule_Mixed">混合排班</el-button>
    </el-row>
    <el-row>
      <el-col :span="24">
        <el-progress :text-inside="true" :stroke-width="24" :percentage="progress_value" status="success"></el-progress>
      </el-col>
    </el-row>
</div>
</template>
<script>
import axios from 'axios'
import Common from '../components/utlis/common'
import {Loading} from 'element-ui'
export default {
    name: 'debug',
    data(){
      return {
        //班次
        schedule_type:0,
        selected_time:"",
        schedule_truck_count:1,
        all_truck_info:[],
        activeTruckInfo:[],
        stopTruckInfo:[],
        //进度条的值
        progress_value:0,
        //随机排班的时间长度（月)
        randomScheduleTime:1,
         from_site_info_list:[],
        to_site_info_list:[],
        permit_info_list:[],
      }
    },
    components:{
      
    },
    created(){
      //获取工地列表
      axios
      .get(Common.GetApiUrl('api/site/query')+Common.Create_CommonParam_HttpGet("site_type",1))
      .then((response)=>{
        if(response.data.code!=1000){
          this.$message.error(response.data.message);
          return;
        }
        console.log(response.data.data);
        this.from_site_info_list=response.data.data;       
      })
      //获取消纳场列表
      axios
      .get(Common.GetApiUrl('api/site/query')+Common.Create_CommonParam_HttpGet("site_type",2))
      .then((response)=>{
        if(response.data.code!=1000){
          this.$message.error(response.data.message);
          return;
        }
        this.to_site_info_list=response.data.data;
        
      })
      //获取通行证列表
      axios
      .get(Common.GetApiUrl('api/permit/query')+Common.Create_CommonParam_HttpGet("all",null))
      .then( (response)=>{
        if(response.data.code!=1000){
          this.$message.error(response.data.message);
          return;
        }
        console.log("permit_info_list");
        console.log(response.data.data);
        this.permit_info_list=response.data.data;
        this.permit_info_list.push({
          "permit_id":0,
          "permit_name":"无",
        });
});
    },
    methods:{
      handleChange(){

      },
       FillTruck2Site_CountStatistic(){
          axios
          .get(Common.GetApiUrl('api/debug/truck_site_attend_statistic?debug_type=null'))
          .then((response)=>{
            if(response.data.code!=1000){
              this.$message.error(response.data.message);
              return;
            }
             this.$message.success("写入成功！");
              return;
          })
       },
       //清空所有车辆的统计数据
       ClearStatisticCount(){
          let loadingInstance=Loading.service("test");
          axios
          .get(Common.GetApiUrl('api/debug/clear_statistic_data?debug_type=null'))
          .then((response)=>{
            if(response.data.code!=1000){
               loadingInstance.close();
              this.$message.error(response.data.message);
              return;
            }
             this.$message.success("清除成功");
              loadingInstance.close();
              return;
          })
       },
       //随机排班(无通行证）)
       RandomSchedule_WithoutPermit(){
         let loading = this.$loading({
          lock: true,
          text: '正在自动排班中（无通行证排班），请耐心等待~',
          spinner: 'el-icon-loading',
          background: 'rgba(0, 0, 0, 0.7)'
        });
         axios.get(Common.GetApiUrl('api/debug/random_schedule')+"?time_lenth="+this.randomScheduleTime+"&type=1")
          .then((response)=>{
            if(response.data.code!=1000){
               loadingInstance.close();
              this.$message.error(response.data.message);
              return;
            }
             this.$message.success("排班成功");
              loading.close();
              return;
          })
       },
       //随机排班（只有通行证）
       RandomSchedule_OnlyPermit(){
         let loading = this.$loading({
          lock: true,
          text: '正在自动排班中（仅通行证排班），请耐心等待~',
          spinner: 'el-icon-loading',
          background: 'rgba(0, 0, 0, 0.7)'
        });
         axios.get(Common.GetApiUrl('api/debug/random_schedule')+"?time_lenth="+this.randomScheduleTime+"&type=2")
          .then((response)=>{
            if(response.data.code!=1000){
               loadingInstance.close();
              this.$message.error(response.data.message);
              return;
            }
             this.$message.success("排班成功");
              loading.close();
              return;
          })
       },
       //随机排班（混合穿插排班）
       RandomSchedule_Mixed(){
          let loading = this.$loading({
          lock: true,
          text: '正在自动排班中（非通行证排班和通行证排班混合），请耐心等待~',
          spinner: 'el-icon-loading',
          background: 'rgba(0, 0, 0, 0.7)'
        });
         axios.get(Common.GetApiUrl('api/debug/random_schedule')+"?time_lenth="+this.randomScheduleTime+"&type=3")
          .then((response)=>{
            if(response.data.code!=1000){
               loadingInstance.close();
              this.$message.error(response.data.message);
              return;
            }
             this.$message.success("排班成功");
              loading.close();
              return;
          })
       },
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