<template>
<div>
  <!--标题栏-->
  <el-row>
    <el-col :span="24">
        <div class="grid-content">
         <el-divider content-position="left">统计-单个车辆出勤查询</el-divider>
        </div>
      </el-col>
       <el-col :span="24">
        <el-steps :active="4" finish-status="success">
          <el-step title="1.选择时间段"></el-step>
          <el-step title="2.选择车辆"></el-step>
          <el-step title="3.点击立即查询"></el-step>
      </el-steps>       
     </el-col>
  </el-row>
  <el-row>
      <el-col :span="10">
        <el-tooltip  effect="dark" content="选择要查询的开始日期" placement="top">
         <el-date-picker
          v-model="selectedStartTime"
          align="right"
          type="datetime"
          placeholder="开始日期"
          value-format="yyyy-MM-dd HH:mm:ss"     
          :picker-options="pickerOptions">
        </el-date-picker>        
       </el-tooltip>    
       <el-tooltip  effect="dark" content="选择要查询的截止日期" placement="top">
         <el-date-picker
          v-model="selectedEndTime"
          align="right"
          type="datetime"
          placeholder="截止日期"
          value-format="yyyy-MM-dd HH:mm:ss" 
          :picker-options="pickerOptions">
        </el-date-picker>        
       </el-tooltip>    
      </el-col>
      <el-col :span="8" >
      <el-tooltip effect="dark" content="只允许输入一个车辆编号" placement="top">
          <el-input v-model="inputTruckNum"    placeholder="输入车辆编号">          
          
        </el-input>
        </el-tooltip>
      </el-col>
      <el-col :span="4">
        <el-button @click="QueryBtnClick">立即查询</el-button>
        <!-- <el-button @click="ExportResultExcelClick">导出</el-button> -->
      </el-col>
    </el-row>
    <el-row>
      <el-table
      sortable
    :data="attend_table_data"    
    stripe
    style="width: 100%">    
    <el-table-column
       type="index"
      :index="site_table_index_method"
      label="序号">
    </el-table-column>
    <el-table-column
      prop="send_time"
      format="yyyy-MM-dd HH:mm:ss"
      sortable
      label="出勤时间"
      width="250">
    </el-table-column>
     <el-table-column
      prop="day_night_display"
      label="班次"
      sortable
      width="230">
    </el-table-column>
    <el-table-column
      prop="task_type_display"
      label="任务类型"
      sortable
      width="230">
    </el-table-column>
    <el-table-column
      prop="attend_type"
      label="出勤类型"
      sortable
      width="230">
    </el-table-column>
     <el-table-column
      prop="permit_name"
      label="通行证"
      sortable
      width="230">
    </el-table-column>
    <el-table-column
      prop="from_site"
      label="工地"
      sortable
      width="160">
    </el-table-column>
    <el-table-column
      prop="to_site"
      label="消纳场"
      sortable
      width="150">
    </el-table-column>
  </el-table>
    </el-row>
</div>
</template>
<script>
import axios from 'axios'
import Common from '../components/utlis/common'
import {Loading} from 'element-ui'
export default {
    name: 'home',
    data(){
      return {
        attend_table_data:[{
          
        }],
        truck_info_list:[],
        tableDataHeader:[{
          "col":"车辆编号",
          "prop":"truck_id",
        }],
        from_site_info_list:[],
        to_site_info_list:[],
        permit_info_list:[],
        from_site_info_list:[],
        inputTruckNum:'',
        selectedStartTime:"",
        selectedEndTime:"",
        is_need_permit:0,//选择是否包含通行证，1=不包含，2=包含
        permit_invalid_time:"选中后显示通行时间",//通行证允许时间
        pickerOptions: {
          disabledDate(time) {
            //return time.getTime() > Date.now();
          },
          shortcuts: [{
            text: '今天',
            onClick(picker) {
              picker.$emit('pick', new Date());
            }
          }, {
            text: '昨天',
            onClick(picker) {
              const date = new Date();
              date.setTime(date.getTime() - 3600 * 1000 * 24);
              picker.$emit('pick', date);
            }
          }, {
            text: '一周前',
            onClick(picker) {
              const date = new Date();
              date.setTime(date.getTime() - 3600 * 1000 * 24 * 7);
              picker.$emit('pick', date);
            }
          }]
        },
      }
    },
    components:{
     
    },
    created(){
        
    },
    methods:{
      IsNeedPermitChange(){

      },
       QueryBtnClick(){
         //
        
         //
         if(this.selectedStartTime>this.selectedEndTime){
           this.$message.warning("截止时间不能早于开始时间！");
           return;
         }
         if(this.inputTruckNum==null||this.inputTruckNum==0){
           this.$message.warning("请输入正确的车辆编号！");
           return;
         }
        this.attend_table_data=[];
        let loadingInstance=Loading.service("test");
       
        console.log(Common.GetApiUrl('api/statistics/get_single_truck_schedule_record')+"?startTime="+this.selectedStartTime+"&endTime="+this.selectedEndTime+"&truckNum="+this.inputTruckNum)

          axios.get(Common.GetApiUrl('api/statistics/get_single_truck_schedule_record')+"?startTime="+this.selectedStartTime+"&endTime="+this.selectedEndTime+"&truckNum="+this.inputTruckNum)
         .then(res=>{
           if(res.data.code!=1000){
             loadingInstance.close();
            this.$message({ message:res.data.message,type:'warning' })
            return;
           }
           console.log("SinglelResultClick_queryClick"+res.data.data);
           if(res.data.data==null){
             this.$message.warning("未查询到任何出勤信息");
             return;
           }
           //遍历条数，每条 {truck_id:1,68:1,69:1,70:1}
           res.data.data.forEach((item)=>{
            
             this.attend_table_data.push(
               {
                  'send_time':item.send_time.replace("T"," "),
                  'from_site':item.from_site_name,
                  'to_site':item.to_site_name,
                  'day_night_display':item.day_night==1?"白班":"夜班",
                  'task_type_display':item.task_type==1?"自动":(item.task_type==2?"手动":"一键连续排班"),
                  'permit_name':item.permit_name,
                  "task_id":item.task_id,
                  "attend_type":item.is_compensate==1?"补偿出勤":"顺序出勤",
               }
             );
              loadingInstance.close();
           });
         })
         loadingInstance.close();
         console.log("this.attend_table_data"+this.attend_table_data);
       },
       DisbaleBtnClick(){
         this.$message.error("功能未开放，敬请期待");
       }
       ,
       site_table_index_method(index){
        return index+1;
      }
    },
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