<template>
<div>
  <!--标题栏-->
  <el-row>
    <el-col :span="24">
        <div class="grid-content">
         <el-divider content-position="left">历史排班任务概览</el-divider>
        </div>
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
      <el-checkbox v-model="allSiteChecked" @change="checkboxChangeEvent">查询全部工地</el-checkbox>
       <el-tooltip effect="dark" content="选择工地" placement="top">
          <el-select v-model="selectedSiteId"  :disabled="isAllSiteChecked" @change="SelectFromSiteChangeEvent" filterable  placeholder="请选择工地">          
          <el-option
            v-for="item in from_site_info_list"
            :key="item.site_id"
            :label="item.site_name"
            :value="item.site_id">            
          </el-option>
        </el-select>
        </el-tooltip>
      </el-col>
      
      <el-col :span="4">
        <el-button @click="QueryBtnClick">查询</el-button>
        <el-button @click="ExportExcelClick">导出</el-button>
      </el-col>
    </el-row>
    <el-row>
     <el-table id="record_data_table"
    :data="record_data"
    stripe
    style="width: 100%">
    <el-table-column
      type="index"
      :index="schedule_task_table_index_method"
      label="序号"
      width="50">
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
      prop="calc_result"
      label="结果">
    </el-table-column>
      <el-table-column
      prop="is_deleted"
      label="备注">
    </el-table-column>
  </el-table>
    </el-row>
</div>
</template>
<script>
import axios from 'axios'
import Common from '../components/utlis/common'
//导出excel使用
import FileSaver from 'file-saver'
import XLSX from 'xlsx'
export default {
    name: 'statistic_site_attend',
    data(){
      return {
        record_data:[],
        tableDataHeader:[{
          "col":"车辆编号",
          "prop":"truck_id",
        }],
        allSiteChecked:true,
        isAllSiteChecked:true,
        from_site_info_list:[],
        to_site_info_list:[],
        selectedSiteId:"",
        selectedStartTime:"",
        selectedEndTime:"",
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
            
          }
          , {
            text: '一月前',
            onClick(picker) {
              const date = new Date();
              date.setTime(date.getTime() - 3600 * 1000 * 24 * 30);
              picker.$emit('pick', date);
            }}
          ]
        },
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
      //获取工地列表      
      axios
      .get(Common.GetApiUrl('api/site/query')+Common.Create_CommonParam_HttpGet("site_type",2))
      .then((response)=>{
        if(response.data.code!=1000){
          this.$message.error(response.data.message);
          return;
        }
        console.log("to_site_info_lst");
        console.log(response.data.data);
        this.to_site_info_list=response.data.data;
               
      })
    },
    methods:{
      //导出excel
      ExportExcelClick(){
        if(this.record_data.length==0){
          this.$message.warning("未查询出任何数据");
           return;
        }
        var xlsxParam={raw:true}
        var wb=XLSX.utils.table_to_book(document.querySelector('#record_data_table'),xlsxParam);
        var wbout=XLSX.write(wb,{bookType:'xlsx',bookSST:true,type:'array'});
        try
        {
          var fileName="排班任务及结果统计_"+this.selectedStartTime+"-"+this.selectedEndTime+".xlsx";
          FileSaver.saveAs(new Blob([wbout],{type:'application/octet-stream'}),fileName);
        }
        catch(e)
        {
             console.log(e,wbout);
        }
        return wbout;
      },
      //选择全部工地 checkbox  勾选 触发event
      checkboxChangeEvent(){
          if(this.allSiteChecked){
            this.isAllSiteChecked=true;
          }
          else{
            this.isAllSiteChecked=false;
          };
      },
      SelectFromSiteChangeEvent(){
        console.log(this.selectedSiteId)
      },
       schedule_task_table_index_method(index){
        return index+1;
      },
      IsNeedPermitChange(){

      },
       QueryBtnClick(){
         if(this.selectedStartTime>this.selectedEndTime){
           this.$message.warning("截止时间不能早于开始时间！");
           return;
         };   
         if(this.isAllSiteChecked){
           this.selectedSiteId=0;
         }     
         else if(!this.isAllSiteChecked&&(this.selectedSiteId==""||this.selectedSiteId==null)){
           this.$message.warning("请选择工地后再进行查询");
           return;
         };
         console.log(Common.GetApiUrl('api/statistics/get_task_record_by_site')+"?startTime="+this.selectedStartTime+"&endTime="+this.selectedEndTime+"&siteId="+this.selectedSiteId)
         var paramModel={
           "startTime":this.selectedStartTime,
           "endTime":this.selectedEndTime,
           "siteId":this.selectedSiteId,
         };
         
         axios.get(Common.GetApiUrl('api/statistics/get_task_record_by_site')+"?startTime="+this.selectedStartTime+"&endTime="+this.selectedEndTime+"&siteId="+this.selectedSiteId)
        .then((response)=>{
          if(response.data.code!=1000){
            this.$message.error(response.data.message);
            return;
          }
          console.log(response.data.data);
          this.record_data=[];
          response.data.data.forEach((element)=>{
            if(this.to_site_info_list.find((n)=>n.site_id==element.to_site_id)==undefined){
              console.log("to_site_info can't find the site:"+element.to_site_id);
              return;
            }
            if(this.from_site_info_list.find((n)=>n.site_id==element.from_site_id)==undefined){
              console.log("from_site_info can't find the site:"+element.from_site_id);
              return;
            }
            this.record_data.push({
              "task_id":element.task_id,
              "day_night_display":element.day_night==1?"白班":"夜班",
              "day_night":element.day_night,
              "send_time":element.send_time.replace("T"," "),
              "task_type_display":element.task_type==1?"自动":(element.task_type==2?"手动":"一键连续排班"),
              "from_site":this.from_site_info_list.find((n)=>n.site_id==element.from_site_id).site_name,
              "to_site":this.to_site_info_list.find((n)=>n.site_id==element.to_site_id).site_name,
              "permit":(element.permit_id==null||element.permit_id==0)?"无":this.permit_info_list.find((n)=>n.permit_id==element.permit_id).permit_name,
              "truck_count":element.truck_count,
              "calc_result":element.calc_result,
              "is_deleted":element.is_deleted==0?"":"任务已撤销",
            });
          });
          
        });
       },
       DisbaleBtnClick(){
         this.$message.error("功能未开放，敬请期待");
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