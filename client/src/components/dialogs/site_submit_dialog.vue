<template>
  <div>    
  <el-dialog title="收货地址" :visible.sync="FormVisible">
  <el-form :model="form">
    <el-form-item label="活动名称" :label-width="formLabelWidth">
      <el-input v-model="form.name" autocomplete="off"></el-input>
    </el-form-item>
    <el-form-item label="活动区域" :label-width="formLabelWidth">
      <el-select v-model="form.region" placeholder="请选择活动区域">
        <el-option label="区域一" value="shanghai"></el-option>
        <el-option label="区域二" value="beijing"></el-option>
      </el-select>
    </el-form-item>
  </el-form>
  <div slot="footer" class="dialog-footer">
    <el-button @click="FormVisible = false">取 消</el-button>
    <el-button type="primary" @click="FormVisible = false">确 定</el-button>
  </div>
</el-dialog>
  </div>  
</template>
<script>
import axios from 'axios'
import Common from '../utlis/common'

   export default {
     props:{
       'FormVisible':Boolean,     
     },
    data() {
      return {
        form: {
          name: '',
          region: '',
          date1: '',
          date2: '',
          delivery: false,
          type: [],
          resource: '',
          desc: ''
        },
        formLabelWidth: '120px'
      }
    },
    methods:{
      
      //点击 运营中的车辆
      activeTruckClick(truck_id){
        console.log(truck_id);
        if(this.selectedTrucks.indexOf(truck_id)>=0){          
           this.$message({ message:"请勿重复选择",type:'warning' })
           return;
        }
        this.selectedTrucks.push(truck_id);
      },
      //点击 停运中的车辆
      stopTruckClick(){

      },
      //恢复运营
      SetTruckActiveClick(){
        if(!this.CheckSelectedTrucks(0)) return;
        //获取选中的车辆
      axios
      .get(Common.GetApiUrl('api/truck/set_active?truck_list=')+this.selectedTrucks)
      .then((response)=>{
        if(response.data.code!=1000){
          this.$message.error(response.data.message);
          return;
        }
        console.log("truck/set_active"+response.data.data);
        this.$message({ message:"操作成功",type:'success' })
        this.$router.go(0);  
      })
      },
      //停运  1=停运，2=请假
      SetTruckStopClick(stopType){
        
        if(!this.CheckSelectedTrucks(0)) return;
        let url;
        if(stopType==1){
          url=Common.GetApiUrl('api/truck/set_stop?truck_list=')+this.selectedTrucks;
        }
        else {
          url=Common.GetApiUrl('api/truck/set_leave?truck_list=')+this.selectedTrucks
        }
        axios
      .get(url)
      .then((response)=>{
        if(response.data.code!=1000){
          this.$message.error(response.data.message);
          return;
        }
        console.log("truck/set_active/Stop"+response.data.data);
        this.$router.go(0);  
      })
      },
      //编辑车辆信息
      OpenTruckEdit(){
        this.$message.error("功能未开放，敬请期待"); 
          return;
         if(!this.CheckSelectedTrucks(1)) return;

      },
      //新增车辆
      AddNewTruck(){
        this.$message.error("功能未开放，敬请期待"); 
          return;
      },
      //删除车辆
      DeleteTruck(){    
        if(!this.CheckSelectedTrucks(1)) return;    
        this.$confirm('此操作将删除车辆以及该车辆的历史排班数据, 是否继续?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning',
          center: true
        }).then(() => {
          axios
      .get(Common.GetApiUrl('api/truck/delete?truck_id=')+this.selectedTrucks[0])
      .then((response)=>{
        if(response.data.code!=1000){
          this.$message.error(response.data.message);
          return;
        }
        console.log("truck/delete"+response.data.data);
        this.$message({
            type: 'success',
            message: '删除成功!'
          });
        this.$router.go(0);  
      })
          
        }).catch(() => {
          this.$message({
            type: 'info',
            message: '已取消删除'
          });
        });
         
         
      },
      //判断选中车辆的数目是否符合需求  needNum=需求的数量限制，0=不限制，1=只允许1个
      CheckSelectedTrucks(needNum){
          if(needNum==0&&this.selectedTrucks.length==0){
            this.$message({ message:"当前未选择任何车辆",type:'warning' })
            return false;
          }
          if(needNum==1){
            if(this.selectedTrucks.length==0){
              this.$message({ message:"当前未选择任何车辆",type:'warning' })
              return false;
            }else if(this.selectedTrucks.length>1){
              this.$message({ message:"只能选中一个车辆并进行操作",type:'warning' })
              return false;
            }
          }
          return true;
      }
    }
  }

</script>
<style scoped>
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
</style>