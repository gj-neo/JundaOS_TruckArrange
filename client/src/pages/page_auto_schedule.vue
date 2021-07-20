<template>
<div>
  <!--标题栏-->
    <el-row >
      <el-col :span="24">
        <div class="grid-content">
         <el-divider content-position="left">自动排班
         </el-divider>
        </div>
      </el-col>
    </el-row>
    <!--时间、班次选择栏-->
    <el-row>
     <el-col :span="24">
        <el-steps :active="stepActive" finish-status="success">
          <el-step title="1.选择班次"></el-step>
          <el-step title="2.选择时间"></el-step>
          <el-step title="3.选择工地和消纳场"></el-step>
          <el-step title="4.选择通行证"></el-step>
          <el-step title="5.点击派车"></el-step>
      </el-steps>       
     </el-col>
    </el-row>
    <el-row >
      <el-col :span="24">
         <el-col :span="4">
           <el-radio-group v-model="schedule_type" @change="ScheduleTypeChange" size="small">
           <el-radio  label="1" border size="medium">白班</el-radio>
          <el-radio  label="2" border size="medium">夜班</el-radio>
           </el-radio-group>
         </el-col>
         <el-col :span="4"   >
         <el-tooltip effect="dark" v-if="stepActive>0" content="选择排班时间" placement="top">
            <el-date-picker
            v-model="selected_time"
            type="datetime"
            placeholder="选择日期时间"    
            value-format="yyyy-MM-dd HH:mm:ss"        
            :picker-options="pickerOptions"
            @change="SelectedTimeChange">
          </el-date-picker>
         </el-tooltip>
        </el-col>
          <el-col  :span="12" >
       <el-button  @click="StartNewSendBtn">新的排班</el-button>
       <el-tooltip effect="dark" content="仅支持查看所选时间当天的排班记录" placement="top">
       <el-button @click="ReloadPageClick">排班记录</el-button>        
       </el-tooltip>
       <el-button @click="CheckNearlyScheduleTimeClick">查看最新排班时间</el-button>
        <el-button @click="CheckToBeCompensatedTruckClick">查看待补偿车辆列表</el-button>
     </el-col >
      </el-col>
    </el-row>
    <!--工地消纳场选择栏-->
    <el-row >
      <el-col :span="24" v-if="stepActive>1">
        <el-col :span="4" >
        <el-tooltip effect="dark" content="选择工地" placement="top">
          <el-select v-model="selectedFromSiteId" @change="SelectFromSiteChangeEvent" filterable  placeholder="请选择工地">
          <el-option
            v-for="item in from_site_info_list"
            :key="item.site_id"
            :label="item.site_name"
            :value="item.site_id">
            
          </el-option>
        </el-select>
        </el-tooltip>
        </el-col>
        <el-col :span="4" >
        <el-tooltip effect="dark" content="选择消纳场" placement="top">
            <el-select v-model="selectedToSiteId"  @change="SelectToSiteChangeEvent" filterable   placeholder="请选择消纳场">
          <el-option
            v-for="item in to_site_info_list"
            :key="item.site_id"
            :label="item.site_name"
            :value="item.site_id">
         </el-option>
        </el-select>
        </el-tooltip>
        </el-col>
        <el-col :span="4" >
        <el-tooltip  effect="dark" content="通行证有效时间会自行判断" placement="top">
            <el-select v-model="selectedPermitId"   filterable   placeholder="选择是否需要通行证">
          <el-option
            v-for="item in permit_info_list"
            :key="item.permit_id"
            :label="item.permit_name"
            :value="item.permit_id">
         </el-option>
        </el-select>
        </el-tooltip>
        </el-col>
        <el-col :span="6" >
        <el-tooltip effect="dark" content="派车数量" placement="top">
         <el-input-number v-model="schedule_truck_count"  :min="1" :max="100" label="派车数量"></el-input-number>
        </el-tooltip>
        <el-tooltip effect="dark" content="添加任务" placement="top">
        <el-button type="primary" icon="el-icon-plus" @click="add_new_task"></el-button>
         </el-tooltip>
        </el-col>
      <el-col :span="4"  v-if="stepActive>2" >
        <el-button @click="SendCarBtnClick">派车</el-button>        
        <el-button @click="ExportExcel" style="float:right">导出结果</el-button>
      </el-col>
      </el-col>
    </el-row>
    <!--任务栏-->
    <el-row v-if="stepActive>0">
      <el-col :span="24">
        <el-table id="schedule_table"
    stripe
    :data="schedule_task_table_data"
    style="width: 100%">
    <el-table-column
      type="index"
      :index="schedule_task_table_index_method"
      width="50">
    </el-table-column>    
    <el-table-column
      prop="schedule_type"
      label="班次"
      width="50">
    </el-table-column>
    <el-table-column      
      prop="send_time"
      label="时间"
      width="150">
    </el-table-column>
    <el-table-column
      prop="from_site_name"
      label="工地"
      width="180">
    </el-table-column>
    <el-table-column
      prop="to_site_name"
      label="消纳场">
    </el-table-column>
     <el-table-column
      prop="need_permit_name"
      label="所需通行证(时间）">
    </el-table-column>    
     <el-table-column
      prop="need_truck_count"
      label="需求数量" 
      width="80">
    </el-table-column>
     <el-table-column
      prop="actual_send_count"
      label="派车数量" 
      width="80">
    </el-table-column>
     <el-table-column
      prop="calc_truck_result"
      label="派车结果"
      width="500">
    </el-table-column>
    <el-table-column
      prop="check_result"
      label="备注"
      width="100">
    </el-table-column>
     <el-table-column
      prop="operate"
      label="操作">
      <template slot-scope="scope">
        <el-button
          size="mini"
          type="danger"
          @click="rebackClick(scope.$index,scope.row)">撤回</el-button>
      </template>
    </el-table-column>
    
  </el-table>
      </el-col>
    </el-row>
    <!--结果栏-->
    <el-row>
      <el-col :span="24">
      </el-col>
    </el-row>
    <el-drawer  size="100%"
    :visible.sync="drawer"
    direction="ttb">    
 <record_table_drawer :record_data="drawer_table_data"> 
 </record_table_drawer>
    </el-drawer>
</div>
</template>
<script>

//import schedule_task_table from '../components/schedule_task_table'
import axios from 'axios'
import Common from '../components/utlis/common'
import record_table_drawer from '../components/schedule_record_table_drawer'
import {Loading} from 'element-ui'
//导出excel使用
import FileSaver from 'file-saver'
import XLSX from 'xlsx'
//获取通行证、工地信息

export default {
    name: 'home',
    data(){
      return {
        //班次
        schedule_type:0,
        selected_time:"",
        schedule_truck_count:1,
        selectedFromSiteId:"",
        selectedToSiteId:"",//消纳场可能为多选
        selectedPermitId:"",//选择的通行证ID
        permit_invalid_time:"选中后显示通行时间",//通行证允许时间
         from_site_info_list:[],
        to_site_info_list:[],
        schedule_task_table_data:[],
        permit_info_list:[],
        stepActive:5,
        drawer:false,
        drawer_table_data:[],//抽屉表格 当日排班记录的表格数据
        //isNeedPermit:false,//是否需要通行证
        isCurrentTaskNew:true,//当前任务是否为新开始的
        isCurrentTaskComplete:false,//当前任务是否已经计算结束
        pickerOptions: {
          disabledDate(time) {
            //return time.getTime() > Date.now();
          },
          shortcuts: [{
            text: '今天8点',
            onClick(picker) {
              const start = new Date(new Date(new Date().toLocaleDateString()).getTime()+8*60*60*1000);
              picker.$emit('pick', start);
            }
          }, {
            text: '今天19点',
            onClick(picker) {
             const start = new Date(new Date(new Date().toLocaleDateString()).getTime()+19*60*60*1000);
               picker.$emit('pick', start);
            }
          }, {
            text: '明天8点',
            onClick(picker) {
              const start = new Date(new Date(new Date().toLocaleDateString()).getTime()+(24+8)*60*60*1000);
               picker.$emit('pick', start);
            }
          }]
        },
       
      }
    },
    
    created(){
      this.isCurrentTaskComplete=false;
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
});
    },
    methods:{
      //导出excel
      ExportExcel(){
        var xlsxParam={raw:true}
        var wb=XLSX.utils.table_to_book(document.querySelector('#schedule_table'),xlsxParam);
        var wbout=XLSX.write(wb,{bookType:'xlsx',bookSST:true,type:'array'});
        try
        {
          var fileName=this.selected_time+"-"+(this.schedule_type==1?"白":"晚")+"-排班.xlsx";
          FileSaver.saveAs(new Blob([wbout],{type:'application/octet-stream'}),fileName);
        }
        catch(e)
        {
             console.log(e,wbout);
        }
        return wbout;
      },
      //查看当前最新排班时间，便于调试或排错
      CheckNearlyScheduleTimeClick(){          
        axios
        .get(Common.GetApiUrl('api/schedule/check_near_schedule_time'))
        .then((response)=>{
          if(response.data.code!=1000){
            this.$message.error(response.data.message);
            return;
          }
          console.log("CheckNearlyScheduleTimeClick"+response.data.data);
          this.$message.success("最新排班时间为 "+response.data.data.replace("T"," "));
          });
      },
      //查看待补偿车辆
      CheckToBeCompensatedTruckClick(){
          axios
        .get(Common.GetApiUrl('api/schedule/get_current_compensation'))
        .then((response)=>{
          if(response.data.code!=1000){
            this.$message.error(response.data.message);
            return;
          }
          console.log("CheckToBeCompensatedTruckClick"+response.data.data);
          this.$message.success("待补偿车辆（按从小到大的顺序）: "+response.data.data);
          });
      },
      //选择工地触发
      SelectFromSiteChangeEvent(){
          
      },
      //选择消纳场触发
      SelectToSiteChangeEvent(){

      },
      //新的排班
      StartNewSendBtn(){    
        this.$router.go(0);    
        this.isCurrentTaskNew=true; 
      },
      ScheduleTypeChange:function(event){        
        if(this.stepActive==0){
          this.stepActive+=1;
        }
      },
      SelectedTimeChange:function(event){
         if(this.stepActive==1){
           this.stepActive+=1;
         }
      },
      //派车
      SendCarBtnClick(){
        
         if(!this.isCurrentTaskNew){
            this.$message({ message:"当前任务已完成排班，若要继续排班，请选择'新的排班'",type:'warning' })
           return;
         };
         if(this.schedule_task_table_data.length==0){
           this.$message({ message:"未选择排班任务",type:'warning' })
           return;
         }
         let loadingInstance=Loading.service("test");
         var taskModeList=[];
         this.schedule_task_table_data.forEach((item)=>{
           
           taskModeList.push({             
             "fromSiteId":item.from_site_id,
             "toSiteId":item.to_site_id,
             "needPermitId":item.permit_id,             
             "needTruckCount":item.need_truck_count,             
           });
         })
         var data={
           "sendTime":this.selected_time,
           "day_night":this.schedule_type,
           "task_type":1,
           "taskLst":taskModeList,
         }
         
         axios.post(Common.GetApiUrl('api/schedule/autoSend'),data)
         .then(res=>{
           if(res.data.code!=1000){
             loadingInstance.close();
            this.$message({ message:res.data.message,type:'warning' })
            return;
           }
           console.log("calc reuslt",res.data);
           res.data.data.forEach((item,index)=>{
             console.log("calc_result",item,index);
             this.schedule_task_table_data[index].task_id=item.taskId;
             this.schedule_task_table_data[index].calc_truck_result=item.resultTrucksNum;
             this.schedule_task_table_data[index].actual_send_count=item.resultTruckIdLst.length;
             this.schedule_task_table_data[index].check_result=item.resultTruckIdLst.length==this.schedule_task_table_data[index].need_truck_count?"":"需求数量与实际数量不匹配";
             loadingInstance.close();
           });
         })
         this.isCurrentTaskNew=false;
         this.isCurrentTaskComplete=true;
      },
      //重载页面 排班记录
      ReloadPageClick(){
        //先获取当天数据
        console.log(Common.GetApiUrl('api/schedule/load_all_schedule_by_date')+"?current_time="+this.selected_time)
        //判断条件
        if(this.selected_time==""){
          this.$message({ message:"请选择时间",type:'warning' })
           return;
        }
        //获取通行证列表
        axios
        .get(Common.GetApiUrl('api/schedule/load_all_schedule_by_date')+"?current_time="+this.selected_time)
        .then((response)=>{
          if(response.data.code!=1000){
            this.$message.error(response.data.message);
            return;
          }
          console.log(response.data.data);
          this.drawer_table_data=[];
          response.data.data.forEach((element)=>{
            this.drawer_table_data.push({
              "task_id":element.task_id,
              "day_night_display":element.day_night==1?"白班":"夜班",
              "day_night":element.day_night,
              "send_time":element.send_time.replace("T"," "),
              "task_type_display":element.task_type==1?"自动":(element.task_type==2?"手动":"一键连续排班"),
              "task_type":element.task_type,
              "from_site":this.from_site_info_list.find((n)=>n.site_id==element.from_site_id).site_name,
              "from_site_id":element.from_site_id,
              "to_site":this.to_site_info_list.find((n)=>n.site_id==element.to_site_id).site_name,
              "to_site_id":element.to_site_id,
              "permit":this.permit_info_list.find((n)=>n.permit_id==element.permit_id).permit_name,
              "permit_id":element.permit_id,
              "truck_count":element.truck_count,
              "result":element.calc_result,
              "check_result":element.calc_result.split('、').length-1==element.truck_count?"":"需求数量与结果数量不匹配",
            });
          });
          //再展示下拉抽屉窗体
          this.drawer=true;
        });
      },
      //导出结果
      ExportResultClick(){
          this.$message.error("功能未开放，敬请期待"); 
          return;
          if(this.schedule_task_table_data.length==0){
           this.$message({ message:"没有可导出的结果",type:'warning' })
           return;
         }
      },
      //撤回  根据是否已排出结果，来判断是单纯撤回任务 ，还是连带排班结果一起撤回
      rebackClick(index,row){
          
        let loadingInstance=Loading.service("test");   
        if(!this.isCurrentTaskComplete){
            this.schedule_task_table_data.splice(index,1);
            this.$message({ message:"撤回任务成功",type:'success' })
        }
        else
        {           
          //获取该条结果
          let data=this.schedule_task_table_data[index];
          axios
        .get(Common.GetApiUrl('api/schedule/reback_task')+"?task_id="+data.task_id)
        .then((response)=>{
          if(response.data.code!=1000){
            this.$message.error(response.data.message);
            return;
          }
          this.$message.success("撤回任务和派车结果成功！");
          this.schedule_task_table_data.splice(index,1);
           
        });       
        }
         loadingInstance.close();
      },
      
      //添加新任务到table中
      add_new_task(){  
        
         if(this.schedule_type==0){
            this.$message({ message:"请选择班次！",type:'warning' })
           return;
         }
         console.log("this.selected_time"+this.selected_time)
          if(this.selected_time==""){
            this.$message({ message:"请选择时间！",type:'warning' })
           return;
         }
         if(!this.isCurrentTaskNew){
            this.$message({ message:"当前任务已完成排班，若要继续排班，请选择'新的排班'",type:'warning' })
           return;
         };
         if(this.schedule_task_table_data.find((n)=>n.from_site_id==this.selectedFromSiteId)){
           this.$message({ message:"请勿重复选择工地！",type:'warning' })
           return;
         }
         if(this.stepActive==2){
           this.stepActive+1;
         }         
         console.log("this.selectedPermitId"+this.selectedPermitId)
         if(this.selectedPermitId==""&&this.selectedPermitId!="0"){
           this.$message({ message:"请选择是否需要通行证",type:'warning' })
           return;
         }
         //debugger;
         let permit=this.selectedPermitId=='0'?null:this.permit_info_list.find((n)=>n.permit_id==this.selectedPermitId);
         console.log(permit)
        // if(permit!=null){
        //       let selectedHour=this.selected_time.split(" ")[1].split(":")[0];  
        //     let startHour=permit.invalid_start_time.split(":")[0];
        //     let endHour=permit.invalid_end_time.split(":")[0];
        //     if(startHour>endHour)
        //     {
        //       //例如20：00-07:00
        //       if(selectedHour<startHour&&selectedHour>endHour){
        //       this.$message({ message:"注意：当前选择的排班时间不在有效时刻内，所选择的通行证有效通行时刻为"+permit.invalid_start_time+"-"+permit.invalid_end_time,type:'warning' })
        //       return false;
        //       }
        //     }
        //     else{
        //       //例如7:00-20：00
        //       if(selectedHour<startHour&&selectedHour>endHour){           
        //         this.$message({ message:"注意：当前选择的排班时间不在有效时刻内，所选择的通行证有效通行时刻为"+permit.invalid_start_time+"-"+permit.invalid_end_time,type:'warning' })
        //       return false;          
        //       }
        //     }
        // }
         
         var from_site_info=this.from_site_info_list.find((n)=>n.site_id==this.selectedFromSiteId);
         var to_site_info=this.to_site_info_list.find((n)=>n.site_id==this.selectedToSiteId);
        
         this.schedule_task_table_data.push({
            "send_time":this.selected_time,
             "schedule_type":this.schedule_type==1?"白班":"晚班",
            "orderId":1,
            "from_site_id":from_site_info.site_id,
            "from_site_name":from_site_info.site_name,
            "permit_id":this.selectedPermitId,            
            "to_site_id":to_site_info.site_id,
            "to_site_name":to_site_info.site_name,
            "need_permit_name":permit==null?"":permit.permit_name,
            "need_truck_count":this.schedule_truck_count,
            "calc_truck_result":"",
            
         });
      },
      schedule_task_table_index_method(index){
        return index+1;
      }
    },
    components:{
      // 'schedule_task_table': schedule_task_table,
      'record_table_drawer':record_table_drawer,
    }
}
</script>
<style scoped >
.el-row {
    
    margin-bottom: 10px;
    text-align: left;
  }
  .el-col{
    margin-top:10px;
  }
  .el-select{
    height: 100%;
  }
  .el-drawer{
    overflow:auto;
   
  }
  .el-drawer_body{
    overflow:auto;
  }
</style>