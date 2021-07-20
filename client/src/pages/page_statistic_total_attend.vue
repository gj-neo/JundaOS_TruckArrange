<template>
<div>
  <!--标题栏-->
  <el-row>
    <el-col :span="24">
        <div class="grid-content">
         <el-divider content-position="left">车辆总出勤</el-divider>
        </div>
      </el-col>
  </el-row>
    <el-row>
      <el-col :span="8">
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
      <el-col :span="8">
      
      </el-col>
      <el-col :span="6">
        <el-button @click="QueryBtnClick">查询</el-button>
        <el-button @click="ExportExcelClick">导出</el-button>
      </el-col>
    </el-row>
    <el-row>
     <el-table id="record_data_table"
    :data="truck_total_attend_data"
    stripe
    style="width: 100%">
    <el-table-column
      prop="truck_num"
      label="车辆编号"
      sortable
      width="150">
    </el-table-column>
    <el-table-column
      prop="own_permit_count"
      sortable
       width="150"
      label="拥有通行证数量">
    </el-table-column>
     <el-table-column
      prop="total_attend_count"
      label="出勤次数"
      width="150"
      sortable>
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
    name: 'home',
    data(){
      return {
        tableDataHeader:[{
          "col":"车辆编号",
          "prop":"truck_id",
        }],
        truck_total_attend_data:[],
        selectedStartTime:'',
        selectedEndTime:'',
        permit_info_list:[],
        pickerOptions: {
          // disabledDate(time) {
          //   //return time.getTime() > Date.now();
          // },
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
          }, {
            text: '一月前',
            onClick(picker) {
              const date = new Date();
              date.setTime(date.getTime() - 3600 * 1000 * 24 * 30);
              picker.$emit('pick', date);
            }
          }]
        },
      }
    },
    components:{
     
    },
    created(){
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
          "permit_name":"非通行证类出勤",
        });
      });
    },
    methods:{
      IsNeedPermitChange(){

      },
      //导出excel
      ExportExcelClick(){
        if(this.truck_total_attend_data.length==0){
          this.$message.warning("未查询出任何数据");
           return;
        }
        var xlsxParam={raw:true}
        var wb=XLSX.utils.table_to_book(document.querySelector('#total_count_record_table'),xlsxParam);
        var wbout=XLSX.write(wb,{bookType:'xlsx',bookSST:true,type:'array'});
        try
        {
          var fileName="总出勤统计_"+this.selectedStartTime+"-"+this.selectedEndTime+".xlsx";
          FileSaver.saveAs(new Blob([wbout],{type:'application/octet-stream'}),fileName);
        }
        catch(e)
        {
             console.log(e,wbout);
        }
        return wbout;
      },
       QueryBtnClick(){
         //truck_total_attend_data
         if(this.selectedStartTime>this.selectedEndTime){
           this.$message.warning("截止时间不能早于开始时间！");
           return;
         }
        console.log(this.selectedStartTime+","+this.selectedEndTime);
        var data={"startTime":this.selectedStartTime,"endTime":this.selectedEndTime}
          axios.get(Common.GetApiUrl('api/statistics/get_truck_total_attend?startTime='+this.selectedStartTime+"&endTime="+this.selectedEndTime))
         .then(res=>{
           if(res.data.code!=1000){
             loadingInstance.close();
            this.$message({ message:res.data.message,type:'warning' })
            return;
           }
           console.log("calc reuslt",res.data);
            this.truck_total_attend_data=[];
           //遍历条数，每条 {truck_id:1, own_permit_count  site_count_list}
           res.data.data.forEach((item)=>{
             console.log("calc_result",item);
             //先解决列标题自动填充  { permit_id,site_count}
             this.truck_total_attend_data.push(
               {"truck_num":item["truck_num"],
               "own_permit_count":item["own_permit_count"],
               "total_attend_count":item["total_attend_count"]}
             )
           });
           
         })
          console.log("this.attend_table_data"+this.truck_total_attend_data);
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