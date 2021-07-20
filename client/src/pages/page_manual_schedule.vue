<template>
<div>
  <!--标题栏-->
    <el-row >
      <el-col :span="24">
        <div class="grid-content">
         <el-divider content-position="left">手动排班
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
          <el-step title="3.选择目的地"></el-step>
          <el-step title="4.查询可用车辆"></el-step>
          <el-step title="5.确定"></el-step>
      </el-steps>       
     </el-col>
    </el-row>
    <el-row >
      <el-col :span="24">
         <el-col :span="8">
           <el-radio-group v-model="schedule_type" @change="ScheduleTypeChange" size="small">
           <el-radio  label="1" border size="medium">白班</el-radio>
          <el-radio  label="2" border size="medium">夜班</el-radio>
           </el-radio-group>
         </el-col>
         <el-col :span="6"   >
         <el-tooltip effect="dark" v-if="stepActive>0" content="选择排班时间" placement="top">
            <el-date-picker
            v-model="selected_time"
            type="datetime"
            placeholder="选择日期时间"
           value-format="yyyy-MM-dd HH:mm:ss" 
            @change="SelectedTimeChange">
          </el-date-picker>
         </el-tooltip>
        </el-col>
          <el-col  :span="4" :offset="4">
       <el-tooltip effect="dark" content="仅支持查看所选是时间" placement="top">
       </el-tooltip>
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
            <el-select v-model="selectedPermitId"  @change="CheckPermitEnable" filterable   placeholder="选择是否需要通行证">
          <el-option
            v-for="item in permit_info_list"
            :key="item.permit_id"
            :label="item.permit_name"
            :value="item.permit_id">
         </el-option>
        </el-select>
        </el-tooltip>
        </el-col>
      <el-col :span="4"  v-if="stepActive>2" >
        <el-button @click="QeuryLeftTruckClick" >查询可用车辆</el-button>
        <el-button @click="SendCarBtnClick" style="float:right">确定派车</el-button>                
      </el-col>
      </el-col>
    </el-row>
    <!--任务栏-->
    <el-row v-if="stepActive>0">
      <el-col :span="24">
        
      </el-col>
    </el-row>
    <!--结果栏-->
    <el-row>
       <div><span class="tag-group__title">选用车辆</span></div>
      <div>
         <el-col :span="12" >
        <el-tag
          :key="truckTag"
          v-for="truckTag in selectedTrucks"
          closable
          @close="handleClose(truckTag)">
           {{truckTag}}
        </el-tag>
      </el-col>
      </div>
     <div>
        <el-col :span="24">
        <div><span class="tag-group__title">剩余可用车辆</span></div>
      <div class="card-box">
        <el-card  class="active-card"
        v-for="item in leftInvalidTrucks"
        :key="item.truck_id"           
        ><div >
          <el-button class="card-button" v-text="item.truck_id" @click="activeTruckClick(item.truck_id)"></el-button>      
          </div>    
    </el-card>
  </div>
      </el-col>
     </div>
    </el-row>    
</div>
</template>
<script>

//import schedule_task_table from '../components/schedule_task_table'
import axios from 'axios'
import Common from '../components/utlis/common'
import record_table_drawer from '../components/schedule_record_table_drawer'
import {Loading} from 'element-ui'
//获取通行证、工地信息

export default {
    name: 'manual_schedule',
    data(){
      return {
        //班次
        schedule_type:0,
        selected_time:"",
        schedule_truck_count:1,
        selectedFromSiteId:"",
        selectedToSiteId:"",//消纳场可能为多选
        selectedPermitId:0,//选择的通行证ID
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
        selectedTrucks:[],
        leftInvalidTrucks:[],       
       
      }
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
        this.permit_info_list=response.data.data;
        this.permit_info_list.push({
          "permit_id":0,
          "permit_name":"无",
        })
});
    },
    methods:{
      //判断选择的时间是否在通行证允许范围内
      CheckPermitEnable(){
        let permitInfo=this.permit_info_list.find((n)=>n.permit_id==this.selectedPermitId);
        let startTime=permitInfo.startTime;
        let endTime=permitInfo.endTime;
        if(this.selected_time<startTime||this.selected_time>endTime){
          this.$message({ message:"注意：当前选择的排班时间不在有效时刻内，所选择的通行证有效通行时刻为"+startTime+"-"+endTime,type:'warning' })
           return;
        }
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
        
        //  if(!this.isCurrentTaskNew){
        //     this.$message({ message:"当前任务已完成排班，若要继续排班，请选择'新的排班'",type:'warning' })
        //    return;
        //  };
        //  if(this.schedule_task_table_data.length==0){
        //    this.$message({ message:"未选择排班任务",type:'warning' })
        //    return;
        //  }
         let loadingInstance=Loading.service("test");
         
         var data={
             "task_id":"",
             "from_site_id":this.selectedFromSiteId,
             "send_site_id":this.selectedToSiteId,
             "permit_id":this.selectedPermitId,
             "truckLst":this.selectedTrucks,
             "send_time":this.selected_time,     
             "schedule_type": this.schedule_type,       
           };
         axios.post(Common.GetApiUrl('api/schedule/manual'),data)
         .then(res=>{
           if(res.data.code!=1000){
            this.$message({ message:res.data.message,type:'warning' })
            return;
           }
           console.log("manual calc reuslt",res.data);
           this.$message({ message:"手动排班成功！",type:'success' })
           loadingInstance.close();
         })
         
         this.isCurrentTaskNew=false;
      },
      //重载页面 排班记录
      ReloadPageClick(){
        //先获取当天数据
        console.log(Common.GetApiUrl('api/schedule/load_all_schedule_by_date')+"?current_time="+this.selected_time)
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
              "schedule_type":element.schedule_type==1?"白班":"夜班",
              "schedule_time":element.schedule_time,
              "auto_manual":element.auto_manual==1?"自动":"手动",
              "from-site":this.from_site_info_list.find((n)=>n.site_id==element.from_site_id).site_name,
              "to-site":this.to_site_info_list.find((n)=>n.site_id==element.to_site_id).site_name,
              "permit":(element.permit_id==null||element.permit_id==0)?"无":this.permit_info_list.find((n)=>n.permit_id==element.permit_id).permit_name,
              "count":element.truck_count,
              "result":element.calc_result,
            });
          });
          console.log(this.drawer_table_data);
          //再展示下拉抽屉窗体
          this.drawer=true;
        });
      },
      //点击 运营中的车辆
      activeTruckClick(truck_id){
        console.log(truck_id);
        if(this.selectedTrucks.indexOf(truck_id)>=0){          
           this.$message({ message:"请勿重复选择",type:'warning' })
           return;
        }
        this.selectedTrucks.push(truck_id);
      },
      //查询可用车辆
      QeuryLeftTruckClick(){    
        
      
         this.leftInvalidTrucks=[];    
          if(this.selected_time=="")
          {
              this.$message({ message:"请选择时间",type:'warning' })
              return;
          }
          
          if(this.schedule_type==""||this.schedule_type==0)
          {
              this.$message({ message:"请选择班次",type:'warning' })
              return;
          }
          if(this.selectedFromSiteId==""||this.selectedFromSiteId==0)
          {
              this.$message({ message:"请选择工地",type:'warning' })
              return;
          }
          if(this.selectedToSiteId==""||this.selectedToSiteId==0)
          {
              this.$message({ message:"请选择消纳场",type:'warning' })
              return;
          }
        
          console.log("QeuryLeftTruckClick",this.selected_time);
          var data ={
            "current_time":this.selected_time,
            "schedule_type":this.schedule_type,
            "from_site_id":this.selectedFromSiteId,
            "to_site_id":this.selectedToSiteId,
            "permit_id":this.selectedPermitId,
          };
          console.log(data);
          //获取通行证列表
          axios
          .post(Common.GetApiUrl('api/schedule/query_left_invalid_trucks'),data)
          .then((response)=>{
            if(response.data.code!=1000){
              this.$message.error(response.data.message);
              return;
            }
            console.log(response.data.data);
            this.drawer_table_data=[];
            response.data.data.forEach((element)=>{
              this.leftInvalidTrucks.push({"truck_id":element});
            });            
          });
      },
      //撤回
      rebackClick(index,row){
        console.log(index);
        console.log(row);
        this.schedule_task_table_data.splice(index,1);
      },
      //添加新任务到table中
      add_new_task(){  
         if(this.schedule_type==0){
            this.$message({ message:"请选择班次！",type:'warning' })
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
         
         var from_permit_id=0;
         var to_permit_id=0;
         var from_site_info=this.from_site_info_list.find((n)=>n.site_id==this.selectedFromSiteId);
         var to_site_info=this.to_site_info_list.find((n)=>n.site_id==this.selectedToSiteId);
        
         this.schedule_task_table_data.push({
            "orderId":1,
            "from_site_id":from_site_info.site_id,
            "from_site_name":from_site_info.site_name,
            "from_permit_id":from_permit_id,
            "to_permit_id":to_permit_id,
            "to_site_id":to_site_info.site_id,
            "to_site_name":to_site_info.site_name,
            "need_permit_name":(this.selectedPermitId==null||this.selectedPermitId==0)?"":this.permit_info_list.find((n)=>n.permit_id==this.selectedPermitId).permit_name,
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
    overflow:scroll;
   
  }
  .el-tag{
   width: 60px;
   height: 30px;
   margin-right: 20px;
   margin-top:10px;
 }
 .card-box{
   display:flex;
   flex-direction:row;
   flex-wrap:wrap;
   justify-content:flex-start
 }
 .el-card{
   width:80px;
   height: 40px;
   margin-right: 20px;
   margin-top:10px;
   font-size: 120%;
   
   text-align: center;
   display:flex;
   justify-content:center;
   align-items:center;
 }
 .active-card{
   background-color: lightgreen;
 }
 .stop-card{
   background-color: salmon;
 }
 .el-card .el-button{
   background-color: transparent;
   border: none;
   font-size: 120%;
   text-align: center;
 }
 .el-input{
   width:250px;
 }
</style>