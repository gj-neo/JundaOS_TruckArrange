<template>
  <el-table
    :data="record_data"
    stripe
    style="width: 100%"
    max-height="768">
    <el-table-column
      type="index"
      :index="schedule_task_table_index_method"
      label="序号"
      width="50">
    </el-table-column>
    <el-table-column
      prop="task_id"
      label="任务编号"
      sortable
      width="180">
    </el-table-column>
    <el-table-column
      prop="day_night_display"
      label="班次"
      sortable
      width="90">
    </el-table-column>
    <el-table-column
      prop="send_time"
      format="yyyy-MM-dd HH:mm:ss"
      sortable
       width="150"
      label="时间">
    </el-table-column>
     <el-table-column
      prop="task_type_display"
      label="排班类型"
      width="100"
      sortable>
    </el-table-column>
     <el-table-column
      prop="from_site"
      label="工地"
      sortable
      width="150">
    </el-table-column>
     <el-table-column
      prop="to_site"
      label="消纳场"
      width="150">
    </el-table-column>
     <el-table-column
      prop="permit"
      label="通行证"
      sortable
      width="150"
      >
    </el-table-column>
      <el-table-column
      prop="truck_count"
      label="数量"
      width="50">
      
    </el-table-column>
      <el-table-column
      prop="result"
      label="结果">
      
    </el-table-column>
    
         <el-table-column
      prop="operate"
      label="操作">
      <template slot-scope="scope">
        <el-button
          size="mini"
          type="danger"
          @click="rebackClick(scope.$index,scope.row)">撤回</el-button>
          <el-button
          size="mini"
          type="success"
          v-if="scope.row.day_night_display=='白班'"
          @click="oneKeyContinueClick(scope.$index,scope.row)">一键继续作业</el-button>
      </template>
    </el-table-column>
     <el-table-column
      prop="check_result"
      label="备注">
      
    </el-table-column>
  </el-table>
</template>
<script>
import axios from 'axios'
import Common from '../components/utlis/common'
import {Loading} from 'element-ui'
  export default {
    props:{
      "record_data":Array,
    },
    data() {
      return {
        
      }
    },
    methods:{
      schedule_task_table_index_method(index){
        return index+1;
      },
      //撤回  包括任务以及结果
      rebackClick(index,row){
         let loadingInstance=Loading.service("test");
        console.log(index);
        console.log(row); 
       axios
        .get(Common.GetApiUrl('api/schedule/reback_task')+"?task_id="+this.record_data[index].task_id)
        .then((response)=>{
          if(response.data.code!=1000){
            this.$message.error(response.data.message);
            return;
          }
          this.$message.success("撤回成功！");
          this.record_data.splice(index,1);
           loadingInstance.close();
          return;
        });
         //
      },
      oneKeyContinueClick(index,row){
         var taskRecordData=this.record_data[index];
        //  this.drawer_table_data.push({
        //       "task_id":element.task_id,
        //       "schedule_type":element.schedule_type==1?"白班":"夜班",
        //       "schedule_time":element.schedule_time.replace("T"," "),
        //       "auto_manual":element.auto_manual==1?"自动":"手动",
        //       "from_site":this.from_site_info_list.find((n)=>n.site_id==element.from_site_id).site_name,
        //       "to_site":this.to_site_info_list.find((n)=>n.site_id==element.to_site_id).site_name,
        //       "permit":(element.permit_id==null||element.permit_id==0)?"无":this.permit_info_list.find((n)=>n.permit_id==element.permit_id).permit_name,
        //       "count":element.truck_count,
        //       "result":element.calc_result,
        //     });
        if(taskRecordData.schedule_type=="夜班"){
          this.$message.warning("一键连续作业只允许针对白班任务操作！");
          return;
        }
         let loadingInstance=Loading.service("test");
        
         axios.get(Common.GetApiUrl('api/schedule/auto_continue_schedule')+"?taskId="+taskRecordData.task_id)
         .then(res=>{
           if(res.data.code!=1000){
             loadingInstance.close();
            this.$message({ message:res.data.message,type:'warning' })
            return;
           }
           else{
             loadingInstance.close();
             this.$message("一键连续作业操作成功,默认时间选择为晚上8点，请关闭该页面后重新打开");
             
             return;
           }
           console.log("auto continue task ",res.data);
         })
      },
    }
  }
</script>
<style scoped>
  
</style>